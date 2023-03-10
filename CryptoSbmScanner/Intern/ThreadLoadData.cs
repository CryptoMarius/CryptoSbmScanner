using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Binance.Net;
using Binance.Net.Clients;
using Binance.Net.Enums;
using Binance.Net.Objects.Models.Spot;
using CryptoExchange.Net.Objects;
using Newtonsoft.Json;
using CryptoExchange.Net.CommonObjects;
using static System.Net.Mime.MediaTypeNames;
using System.Reflection;
using Microsoft.Office.Interop.Excel;
using TradingView;

namespace CryptoSbmScanner
{
    public class ThreadLoadData
    {

        private void CalculateMissingCandles()
        {
            //************************************************************************************
            // Alle intervallen herberekenen (het is een bulk hercalculatie voor de laatste in het geheugen gelezen candles)
            // In theorie is dit allemaal reeds in de database opgeslagen, maar baat het niet dan schaad het niet
            //************************************************************************************
            foreach (CryptoExchange exchange in GlobalData.ExchangeListName.Values)
            {
                //break; //Laat maar even... (HEEL waarschijnlijk een verschil in UTC bij de omzetactie)

                GlobalData.AddTextToLogTab("Calculating candle intervals for " + exchange.Name + " (" + exchange.SymbolListName.Count().ToString() + " symbols)");
                foreach (CryptoSymbol symbol in exchange.SymbolListName.Values)
                {
                    //De "barometer" munten overslagen AUB, die hebben slechts 3 intervallen (beetje quick en dirty allemaal)
                    if (symbol.IsBarometerSymbol())
                        continue;

                    if (symbol.CandleList.Any())
                    {
                        try
                        {
                            // Van laag naar hoog zodat de hogere intervallen worden berekend
                            foreach (CryptoSymbolInterval symbolPeriod in symbol.IntervalPeriodList)
                            {
                                CryptoInterval interval = symbolPeriod.Interval;
                                if (interval.ConstructFrom != null)
                                {
                                    // Voeg een candle toe aan een hogere tijd interval (eventueel uit db laden)
                                    SortedList<long, CryptoCandle> candlesInterval = symbolPeriod.CandleList;
                                    if (candlesInterval.Values.Count > 0)
                                    {
                                        // Periode start
                                        long unixFirst = candlesInterval.Values.First().OpenTime;
                                        unixFirst = unixFirst - unixFirst % interval.Duration;
                                        DateTime dateFirst = CandleTools.GetUnixDate(unixFirst);

                                        // Periode einde
                                        long unixLast = candlesInterval.Values.Last().OpenTime;
                                        unixLast = unixLast - unixLast % interval.Duration;
                                        DateTime dateLast = CandleTools.GetUnixDate(unixLast);


                                        // TODO: Het aantal variabelen verminderen
                                        long unixLoop = unixFirst;
                                        DateTime dateLoop = CandleTools.GetUnixDate(unixLoop);

                                        // Herbereken deze periode opnieuw uit het onderliggende interval
                                        while (unixLoop <= unixLast)
                                        {
                                            CandleTools.CalculateCandleForInterval(interval, interval.ConstructFrom, symbol, unixLoop);

                                            unixLoop += interval.Duration;
                                            dateLoop = CandleTools.GetUnixDate(unixLoop); //ter debug want een unix date is onleesbaar
                                        }
                                    }
                                }

                                // De laatste datum bijwerken (zodat we minder candles hoeven op te halen)
                                CandleTools.UpdateCandleFetched(symbol, interval);
                            }
                        }
                        catch (Exception error)
                        {
                            GlobalData.Logger.Error(error);
                            GlobalData.AddTextToLogTab(error.ToString());
                            throw;
                        }

                    }
                }
            }
        }


        private void RecalculateLastXCandles(int lookback = 1)
        {
            // PAS OP, die extra candles moeten overeenkomen met de extra 10 in de GetCandleFetchStart()! Dus 260 + X
            if (GlobalData.Settings.Signal.SignalsActive)
            {
                while (lookback > 0)
                {
                    foreach (CryptoSbmScanner.CryptoExchange exchange in GlobalData.ExchangeListName.Values)
                    {
                        foreach (CryptoQuoteData quoteData in GlobalData.Settings.QuoteCoins.Values)
                        {
                            if (quoteData.CreateSignals)
                            {
                                foreach (CryptoSymbol symbol in quoteData.SymbolList)
                                {
                                    if ((symbol.Status == 1) && !symbol.IsBarometerSymbol() && symbol.IsSpotTradingAllowed)
                                    {
                                        //foreach (CryptoSymbolInterval period in symbol.IntervalPeriodList)
                                        //{
                                        //    GlobalData.AddTextToLogTab(string.Format("{0} {1} candles={2}", symbol.Name, period.Interval.Name, period.CandleList.Count));
                                        //}

                                        // Aanbieden voor analyse
                                        if (symbol.CandleList.Any() && (symbol.CandleList.Count - lookback >= 0))
                                        {
                                            CryptoCandle candle = symbol.CandleList.Values[symbol.CandleList.Count - lookback];
                                            GlobalData.ThreadCreateSignal.AddToQueue(candle);
                                        }
                                    }
                                }
                            }

                        }
                    }

                    lookback--;
                }
            }
        }


        public async Task ExecuteAsync()
        {
            try
            {
                // Reset alles zodat we opnieuw een LoadData kunnen doen?
                GlobalData.ExchangeListName.Clear();
                GlobalData.InitializeIntervalList();
                GlobalData.SymbolBlackListOversold.Clear();
                GlobalData.SymbolWhiteListOversold.Clear();
                GlobalData.SymbolBlackListOverbought.Clear();
                GlobalData.SymbolWhiteListOverbought.Clear();

                DataStore dataStore = new DataStore();
                {
                    //************************************************************************************
                    //Informatie uit de database lezen
                    //************************************************************************************
                    GlobalData.InitExchanges();
                    //dataStore.LoadExchanges();
                    //dataStore.LoadSymbols(); overbodig


                    //************************************************************************************
                    // Alle symbols van de exchange halen en mergen met de ingelezen symbols.
                    // Via een event worden de muntparen in de userinterface gezet (dat duurt even)
                    //************************************************************************************
                    BinanceFetchSymbols fetchSymbols = new BinanceFetchSymbols();
                    await Task.Run(async () => { await fetchSymbols.ExecuteAsync(); }); // Geen await, deze mag/MOET parallel

                    // Na het inlezen van de symbols de lijsten goed zetten
                    GlobalData.InitWhiteAndBlackListSettings();
                    // De (interne) barometer symbols toevoegen
                    GlobalData.InitBarometerSymbols();


                    GlobalData.AddTextToLogTab("Reading candle information");
                    dataStore.LoadCandles();



                    //************************************************************************************
                    // Alle intervallen herberekenen (het is een bulk hercalculatie voor de laatste in het geheugen gelezen candles)
                    // In theorie is dit allemaal reeds in de database opgeslagen, maar baat het niet dan schaad het niet
                    //************************************************************************************
                    foreach (CryptoExchange exchange in GlobalData.ExchangeListName.Values)
                    {
                        //break; //Laat maar even... (HEEL waarschijnlijk een verschil in UTC bij de omzetactie)

                        GlobalData.AddTextToLogTab("Calculating candle intervals for " + exchange.Name + " (" + exchange.SymbolListName.Count().ToString() + " symbols)");
                        foreach (CryptoSymbol symbol in exchange.SymbolListName.Values)
                        {
                            //De "barometer" munten overslagen AUB, die hebben slechts 3 intervallen (beetje quick en dirty allemaal)
                            if (symbol.IsBarometerSymbol())
                                continue;

                            if (symbol.CandleList.Any())
                            {
                                try
                                {
                                    // Van laag naar hoog zodat de hogere intervallen worden berekend
                                    foreach (CryptoSymbolInterval symbolPeriod in symbol.IntervalPeriodList)
                                    {
                                        CryptoInterval interval = symbolPeriod.Interval;
                                        if (interval.ConstructFrom != null)
                                        {
                                            // Voeg een candle toe aan een hogere tijd interval (eventueel uit db laden)
                                            SortedList<long, CryptoCandle> candlesInterval = symbolPeriod.CandleList;
                                            if (candlesInterval.Values.Count > 0)
                                            {
                                                // Periode start
                                                long unixFirst = candlesInterval.Values.First().OpenTime;
                                                unixFirst = unixFirst - (unixFirst % interval.Duration);
                                                DateTime dateFirst = CandleTools.GetUnixDate(unixFirst);

                                                // Periode einde
                                                long unixLast = candlesInterval.Values.Last().OpenTime;
                                                unixLast = unixLast - (unixLast % interval.Duration);
                                                DateTime dateLast = CandleTools.GetUnixDate(unixLast);


                                                // TODO: Het aantal variabelen verminderen
                                                long unixLoop = unixFirst;
                                                DateTime dateLoop = CandleTools.GetUnixDate(unixLoop);

                                                // Herbereken deze periode opnieuw uit het onderliggende interval
                                                while (unixLoop <= unixLast)
                                                {
                                                    CandleTools.CalculateCandleForInterval(interval, interval.ConstructFrom, symbol, unixLoop);

                                                    unixLoop += interval.Duration;
                                                    dateLoop = CandleTools.GetUnixDate(unixLoop); //ter debug want een unix date is onleesbaar
                                                }
                                            }
                                        }

                                        // De laatste datum bijwerken (zodat we minder candles hoeven op te halen)
                                        CandleTools.UpdateCandleFetched(symbol, interval); // alleen relevant voor 1m
                                    }
                                }
                                catch (Exception error)
                                {
                                    GlobalData.Logger.Error(error);
                                    GlobalData.AddTextToLogTab(error.ToString());
                                    throw;
                                }

                            }
                        }
                    }


                    int aantalTotaal = 0;
                    foreach (CryptoSbmScanner.CryptoExchange exchange in GlobalData.ExchangeListName.Values)
                    {
                        foreach (CryptoSymbol symbol in exchange.SymbolListName.Values)
                        {
                            foreach (CryptoSymbolInterval symbolPeriod in symbol.IntervalPeriodList)
                            {
                                aantalTotaal += symbolPeriod.CandleList.Count;
                            }
                        }
                        if (aantalTotaal > 0)
                            GlobalData.AddTextToLogTab("Total amount of candles in memory " + aantalTotaal.ToString("N0") + " candles");
                    }
                }


                //************************************************************************************
                // De Telegram bot opstarten
                //************************************************************************************
                //GlobalData.AddTextToLogTab("Starting Telegram bot");
                //new ThreadTelegramBot().Thread.Start();


                //************************************************************************************
                // Vanaf dit moment worden de 1m candles bijgewerkt 
                // Vanaf dit moment worden de aangeboden 1m candles in ons systeem verwerkt
                // (Dit moet overlappen met "achterstand bijwerken" want anders ontstaan er gaten)
                // BUG/Probleem! na nieuwe munt of instellingen wordt dit niet opnieuw gedaan (herstart nodig)
                //************************************************************************************
                {
                    // Deze methode werkt alleen op Binance
                    CryptoExchange exchange;
                    if (GlobalData.ExchangeListName.TryGetValue("Binance", out exchange))
                    {
                        foreach (CryptoQuoteData quoteData in GlobalData.Settings.QuoteCoins.Values)
                        {
                            if ((quoteData.FetchCandles) && (quoteData.SymbolList.Count > 0))
                            {
                                List<CryptoSymbol> symbols = quoteData.SymbolList.ToList();

                                while (symbols.Count > 0)
                                {
                                    BinanceStream1mCandles BinanceStream1mCandles = new BinanceStream1mCandles(quoteData);

                                    // Met de volle mep reageert Binance niet snel genoeg (timeout errors enzovoort)
                                    // Dit is een quick fix na de update van Binance.Net van 7 -> 8
                                    while (symbols.Count > 0)
                                    {
                                        CryptoSymbol symbol = symbols[0];
                                        symbols.Remove(symbol);

                                        BinanceStream1mCandles.symbols.Add(symbol.Name);

                                        // Ergens een lijn trekken? 
                                        if (BinanceStream1mCandles.symbols.Count >= 175)
                                            break;
                                    }

                                    // opvullen tot circa 150 coins?
                                    quoteData.BinanceStream1mCandles.Add(BinanceStream1mCandles);
                                    await BinanceStream1mCandles.StartAsync(); // bewust geen await
                                }
                            }
                        }
                    }
                }


                //************************************************************************************
                // Om het volume per symbol en laatste prijs te achterhalen (weet geen betere manier)
                //************************************************************************************
                // Deze methode werkt alleen op Binance
                GlobalData.TaskBinanceStreamPriceTicker = new BinanceStreamPriceTicker();
                var whatever2 = Task.Run(async () => { await GlobalData.TaskBinanceStreamPriceTicker.ExecuteAsync(); }); // Geen await, forever long running


                //************************************************************************************
                // De (ontbrekende) candles downloaden (en de achterstand inhalen, blocking!)
                //************************************************************************************
                // Deze methode werkt alleen op Binance
                BinanceFetchCandles binanceFetchCandles = new BinanceFetchCandles();
                await Task.Run(async () => { await binanceFetchCandles.ExecuteAsync(); }); // wachten tot deze klaar is

                //Ze zijn er allemaal wel, deze is overbodig
                //CalculateMissingCandles();


                Task.Factory.StartNew(() => new TradingViewSymbolInfo().Start("TVC:DXY", "US Dollar Index", "N2", GlobalData.TradingViewDollarIndex, 10));
                Task.Factory.StartNew(() => new TradingViewSymbolInfo().Start("SP:SPX", "S&P 500", "N2", GlobalData.TradingViewSpx500, 10));
                Task.Factory.StartNew(() => new TradingViewSymbolInfo().Start("CRYPTOCAP:BTC.D", "BTC Dominance", "N2", GlobalData.TradingViewBitcoinDominance, 10));
                Task.Factory.StartNew(() => new TradingViewSymbolInfo().Start("CRYPTOCAP:TOTAL3", "Market Cap total", "N0", GlobalData.TradingViewMarketCapTotal, 10));


                //************************************************************************************
                // Nu we de achterstand ingehaald hebben kunnen/mogen we analyseren (signals maken)
                //************************************************************************************
                GlobalData.AddTextToLogTab("Starting task for creating signals");
                var whatever3 = Task.Run(() => { GlobalData.ThreadCreateSignal.Execute(); }); // Geen await, forever long running


                var assembly = Assembly.GetExecutingAssembly().GetName();
                string appName = assembly.Name.ToString();
                string appVersion = assembly.Version.ToString();
                GlobalData.AddTextToLogTab(appName + " " + appVersion + " ready", true);


                // Heb me dag lopen af te vragen waarom er geen signalen kwamen, iets met white&black, right
                if (GlobalData.Settings.UseWhiteListOversold)
                    GlobalData.AddTextToLogTab("Oversold whitelist activated " + string.Join(",", GlobalData.Settings.WhiteListOversold));
                if (GlobalData.Settings.UseBlackListOversold)
                    GlobalData.AddTextToLogTab("Oversold blacklist activated " + string.Join(",", GlobalData.Settings.WhiteListOversold));

                if (GlobalData.Settings.UseWhiteListOverbought)
                    GlobalData.AddTextToLogTab("Overbought whitelist activated " + string.Join(",", GlobalData.Settings.WhiteListOverbought));
                if (GlobalData.Settings.UseBlackListOverbought)
                    GlobalData.AddTextToLogTab("Overbought blacklist activated " + string.Join(",", GlobalData.Settings.WhiteListOverbought));

                // Dit is een enorme cpu drain, eventjes 3 * 250 * ~3 intervallen bijlangs
                //RecalculateLastXCandles(1);
            }
            catch (Exception error)
            {
                GlobalData.Logger.Error(error);
                GlobalData.AddTextToLogTab(error.ToString());
                throw;
            }
        }


    }
}

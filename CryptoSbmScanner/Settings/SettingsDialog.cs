using Microsoft.IdentityModel.Tokens;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace CryptoSbmScanner
{
    public partial class FrmSettings : Form
    {

        private Settings settings;

        public FrmSettings()
        {
            InitializeComponent();

            tabExtra.Visible = GlobalData.ShowExtraStuff;


            toolTip1.SetToolTip(EditAnalysisShowStobbOverbought, "Dit type signaal is een dubbele indicatie dat een munt overbought is en die bestaat uit:" +
                "\n-een candle die opent of sluit boven de bovenste bollingerband\n" +
                "-zowel de %d als %k van de stochastic zijn boven de 80\n" +
                "(dit kan een instapmoment zijn voor een short positie)");
            toolTip1.SetToolTip(EditAnalysisShowStobbOversold, "Dit type signaal is een dubbele indicatie dat een munt oversold is en bestaat uit:\n" +
                "-een candle die opent of sluit onder de onderste bollingerbands\n" +
                "-zowel de % d als % k van de stochastic zijn onder de 20\n" +
                "(dit kan een instapmoment zijn voor een long positie).");

            toolTip1.SetToolTip(EditAnalysisShowSbmOverbought, "Dit is een variatie op de stobb overbought signaal en bestaat uit:\n" +
                "-een stobb overbought signaal\n" +
                "-de ma200 onder de ma50 is\n" +
                "-de ma50 onder de ma20 is\n" +
                "-de psar op of boven de ma20\n" +
                "(dit kan een instapmoment zijn voor een short positie)");
            toolTip1.SetToolTip(EditAnalysisShowSbmOversold, "Dit is een variatie op de stobb oversold signaal en bestaat uit:\n" +
                "-een stobb oversold signaal\n" +
                "-de ma200 boven de ma50 is\n" +
                "-de ma50 boven de ma20 is\n" +
                "-de psar op of onder de ma20\n" +
                "(dit kan een instapmoment zijn voor een long positie)");

            toolTip1.SetToolTip(EditAnalysisShowCandleJumpUp, "Een signaal dat een munt een bepaald percentage naar boven \"spingt\" (info)");
            toolTip1.SetToolTip(EditAnalysisShowCandleJumpDown, "Een signaal dat een munt een bepaald percentage naar beneden \"spingt\"(info)");
            toolTip1.SetToolTip(EditAnalysisCandleJumpPercentage, "Percentage dat de munt naar boven of beneden moet bewegen");


            // Stupid designer removes events (after moving, sick of it...)
            EditPlaySoundSbmSignal.Click += SetGrayed;
            EditPlaySoundStobbSignal.Click += SetGrayed;
            EditPlaySoundCandleJumpSignal.Click += SetGrayed;

            buttonReset.Click += buttonReset_Click;
            buttonTestSpeech.Click += buttonTestSpeech_Click;
            buttonFontDialog.Click += buttonFontDialog_Click;

            buttonColorBTC.Click += buttonColorBTC_Click;
            buttonColorETH.Click += buttonColorETH_Click;
            buttonColorBNB.Click += buttonColorBNB_Click;
            buttonColorBUSD.Click += buttonColorBUSD_Click;
            buttonColorUSDT.Click += buttonColorUSDT_Click;

            buttonColorStobb.Click += buttonColorStobb_Click;
            buttonColorSbm.Click += buttonColorSbm_Click;
            buttonColorJump.Click += buttonColorJump_Click;

            buttonSelectSoundStobbOverbought.Click += buttonSelectSoundStobbOverbought_Click;
            buttonSelectSoundStobbOversold.Click += buttonSelectSoundStobbOversold_Click;
            buttonSelectSoundSbmOverbought.Click += buttonSelectSoundSbmOverbought_Click;
            buttonSelectSoundSbmOversold.Click += buttonSelectSoundSbmOversold_Click;
            buttonSelectSoundCandleJumpUp.Click += buttonSelectSoundCandleJumpUp_Click;
            buttonSelectSoundCandleJumpDown.Click += buttonSelectSoundCandleJumpDown_Click;

            buttonPlaySoundStobbOverbought.Click += buttonPlaySoundStobbOverbought_Click;
            buttonPlaySoundStobbOversold.Click += buttonPlaySoundStobbOversold_Click;
            buttonPlaySoundSbmOverbought.Click += buttonPlaySoundSbmOverbought_Click;
            buttonPlaySoundSbmOversold.Click += buttonPlaySoundSbmOversold_Click;
            buttonPlaySoundCandleJumpUp.Click += buttonPlaySoundCandleJumpUp_Click;
            buttonPlaySoundCandleJumpDown.Click += buttonPlaySoundCandleJumpDown_Click;

        }

        private void SetGrayed(object sender, EventArgs e)
        {
            // Stobb
            EditSoundStobbOverbought.Enabled = EditPlaySoundStobbSignal.Checked;
            buttonPlaySoundStobbOverbought.Enabled = EditPlaySoundStobbSignal.Checked;
            buttonSelectSoundStobbOverbought.Enabled = EditPlaySoundStobbSignal.Checked;
            buttonPlaySoundStobbOverbought.Enabled = EditPlaySoundStobbSignal.Checked;

            EditSoundStobbOversold.Enabled = EditPlaySoundStobbSignal.Checked;
            buttonPlaySoundStobbOversold.Enabled = EditPlaySoundStobbSignal.Checked;
            buttonSelectSoundStobbOversold.Enabled = EditPlaySoundStobbSignal.Checked;
            buttonPlaySoundStobbOversold.Enabled = EditPlaySoundStobbSignal.Checked;

            // Sbm
            EditSoundFileSbmOverbought.Enabled = EditPlaySoundSbmSignal.Checked;
            buttonPlaySoundSbmOverbought.Enabled = EditPlaySoundSbmSignal.Checked;
            buttonSelectSoundSbmOverbought.Enabled = EditPlaySoundSbmSignal.Checked;
            buttonPlaySoundSbmOverbought.Enabled = EditPlaySoundSbmSignal.Checked;

            EditSoundFileSbmOversold.Enabled = EditPlaySoundSbmSignal.Checked;
            buttonPlaySoundSbmOversold.Enabled = EditPlaySoundSbmSignal.Checked;
            buttonSelectSoundSbmOversold.Enabled = EditPlaySoundSbmSignal.Checked;
            buttonPlaySoundSbmOversold.Enabled = EditPlaySoundSbmSignal.Checked;

            // Candle jump UP
            EditSoundFileCandleJumpUp.Enabled = EditPlaySoundCandleJumpSignal.Checked;
            buttonPlaySoundCandleJumpUp.Enabled = EditPlaySoundCandleJumpSignal.Checked;
            buttonSelectSoundCandleJumpUp.Enabled = EditPlaySoundCandleJumpSignal.Checked;
            buttonPlaySoundCandleJumpUp.Enabled = EditPlaySoundCandleJumpSignal.Checked;

            // Candle jump Down
            EditSoundFileCandleJumpDown.Enabled = EditPlaySoundCandleJumpSignal.Checked;
            buttonPlaySoundCandleJumpDown.Enabled = EditPlaySoundCandleJumpSignal.Checked;
            buttonSelectSoundCandleJumpDown.Enabled = EditPlaySoundCandleJumpSignal.Checked;
            buttonPlaySoundCandleJumpDown.Enabled = EditPlaySoundCandleJumpSignal.Checked;
        }

        public void InitSettings(Settings settings)
        {
            this.settings = settings;

            if ((GlobalData.Settings.General.FontSize != this.Font.Size) || (GlobalData.Settings.General.FontName.Equals(this.Font.Name)))
            {
                this.Font = new System.Drawing.Font(GlobalData.Settings.General.FontName, GlobalData.Settings.General.FontSize,
                    System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            }

            //EditGetCandleInterval.Value = settings.General.GetCandleInterval;
            // ------------------------------------------------------------------------------
            // General
            // ------------------------------------------------------------------------------
            EditBlackTheming.Checked = settings.General.BlackTheming;
            EditTradingApp.SelectedIndex = (int)settings.General.TradingApp;
            EditDoubleClickAction.SelectedIndex = (int)settings.General.DoubleClickAction;
            EditSoundHeartBeatMinutes.Value = settings.Signal.SoundHeartBeatMinutes;

            EditHideTechnicalStuffSignals.Checked = settings.Signal.HideTechnicalStuffSignals;
            EditGlobalDataRemoveSignalAfterxCandles.Value = settings.Signal.RemoveSignalAfterxCandles;

            EditBarometer1hMinimal.Value = settings.Signal.Barometer1hMinimal;
            EditLogBarometerToLow.Checked = settings.Signal.LogBarometerToLow;

            // ------------------------------------------------------------------------------
            // Base coins
            // ------------------------------------------------------------------------------

            SetQuoteCoin("BNB", EditFetchCandlesBNB, EditMinVolumeBNB, EditMinPriceBNB, EditCreateSignalsBNB, panelColorBNB);
            SetQuoteCoin("BTC", EditFetchCandlesBTC, EditMinVolumeBTC, EditMinPriceBTC, EditCreateSignalsBTC, panelColorBTC);
            SetQuoteCoin("BUSD", EditFetchCandlesBUSD, EditMinVolumeBUSD, EditMinPriceBUSD, EditCreateSignalsBUSD, panelColorBUSD);
            SetQuoteCoin("ETH", EditFetchCandlesETH, EditMinVolumeETH, EditMinPriceETH, EditCreateSignalsETH, panelColorETH);
            SetQuoteCoin("USDT", EditFetchCandlesUSDT, EditMinVolumeUSDT, EditMinPriceUSDT, EditCreateSignalsUSDT, panelColorUSDT);
            SetQuoteCoin("EUR", EditFetchCandlesEUR, EditMinVolumeEUR, EditMinPriceEUR, EditCreateSignalsEUR, panelColorEUR);


            // ------------------------------------------------------------------------------
            // Signals
            // ------------------------------------------------------------------------------
            EditAnalyseInterval1m.Checked = settings.Signal.AnalyseInterval[(int)CryptoIntervalPeriod.interval1m];
            EditAnalyseInterval2m.Checked = settings.Signal.AnalyseInterval[(int)CryptoIntervalPeriod.interval2m];
            EditAnalyseInterval3m.Checked = settings.Signal.AnalyseInterval[(int)CryptoIntervalPeriod.interval3m];
            EditAnalyseInterval5m.Checked = settings.Signal.AnalyseInterval[(int)CryptoIntervalPeriod.interval5m];
            EditAnalyseInterval10m.Checked = settings.Signal.AnalyseInterval[(int)CryptoIntervalPeriod.interval10m];
            EditAnalyseInterval15m.Checked = settings.Signal.AnalyseInterval[(int)CryptoIntervalPeriod.interval15m];
            EditAnalyseInterval30m.Checked = settings.Signal.AnalyseInterval[(int)CryptoIntervalPeriod.interval30m];
            EditAnalyseInterval1h.Checked = settings.Signal.AnalyseInterval[(int)CryptoIntervalPeriod.interval1h];
            EditAnalyseInterval2h.Checked = settings.Signal.AnalyseInterval[(int)CryptoIntervalPeriod.interval2h];
            EditAnalyseInterval4h.Checked = settings.Signal.AnalyseInterval[(int)CryptoIntervalPeriod.interval4h];
            EditAnalyseInterval6h.Checked = settings.Signal.AnalyseInterval[(int)CryptoIntervalPeriod.interval6h];
            EditAnalyseInterval8h.Checked = settings.Signal.AnalyseInterval[(int)CryptoIntervalPeriod.interval8h];
            EditAnalyseInterval12h.Checked = settings.Signal.AnalyseInterval[(int)CryptoIntervalPeriod.interval12h];
            EditAnalyseInterval1d.Checked = settings.Signal.AnalyseInterval[(int)CryptoIntervalPeriod.interval1d];

            // STOBB
            EditStobbBBMinPercentage.Value = (decimal)settings.Signal.StobbBBMinPercentage;
            EditStobbBBMaxPercentage.Value = (decimal)settings.Signal.StobbBBMaxPercentage;
            EditStobbUseLowHigh.Checked = settings.Signal.StobbUseLowHigh;
            EditPlaySoundStobbSignal.Checked = settings.Signal.PlaySoundStobbSignal;
            EditPlaySpeechStobbSignal.Checked = settings.Signal.PlaySpeechStobbSignal;
            EditAnalysisShowStobbOversold.Checked = settings.Signal.AnalysisShowStobbOversold;
            EditAnalysisShowStobbOverbought.Checked = settings.Signal.AnalysisShowStobbOverbought;
            EditSoundStobbOverbought.Text = settings.Signal.SoundStobbOverbought;
            EditSoundStobbOversold.Text = settings.Signal.SoundStobbOversold;
            EditStobIncludeRsi.Checked = settings.Signal.StobIncludeRsi;
            EditStobIncludeSoftSbm.Checked = settings.Signal.StobIncludeSoftSbm;
            panelColorStobb.BackColor = settings.Signal.ColorStobb;

            // SBM
            EditSbmBBMinPercentage.Value = (decimal)settings.Signal.SbmBBMinPercentage;
            EditSbmBBMaxPercentage.Value = (decimal)settings.Signal.SbmBBMaxPercentage;
            EditSbmUseLowHigh.Checked = settings.Signal.SbmUseLowHigh;
            EditPlaySoundSbmSignal.Checked = settings.Signal.PlaySoundSbmSignal;
            EditPlaySpeechSbmSignal.Checked = settings.Signal.PlaySpeechSbmSignal;
            EditAnalysisShowSbmOversold.Checked = settings.Signal.AnalysisShowSbmOversold;
            EditAnalysisShowSbmOverbought.Checked = settings.Signal.AnalysisShowSbmOverbought;
            EditSoundFileSbmOverbought.Text = settings.Signal.SoundSbmOverbought;
            EditSoundFileSbmOversold.Text = settings.Signal.SoundSbmOversold;
            panelColorSbm.BackColor = settings.Signal.ColorSbm;

            // JUMP
            EditPlaySoundCandleJumpSignal.Checked = settings.Signal.PlaySoundCandleJumpSignal;
            EditPlaySpeechCandleJumpSignal.Checked = settings.Signal.PlaySpeechCandleJumpSignal;
            EditAnalysisShowCandleJumpDown.Checked = settings.Signal.AnalysisShowCandleJumpDown;
            EditAnalysisShowCandleJumpUp.Checked = settings.Signal.AnalysisShowCandleJumpUp;
            EditSoundFileCandleJumpDown.Text = settings.Signal.SoundCandleJumpDown;
            EditSoundFileCandleJumpUp.Text = settings.Signal.SoundCandleJumpUp;
            EditAnalysisCandleJumpPercentage.Value = (decimal)settings.Signal.AnalysisCandleJumpPercentage;
            EditJumpCandlesLookbackCount.Value = settings.Signal.JumpCandlesLookbackCount;
            EditJumpUseLowHighCalculation.Checked = settings.Signal.JumpUseLowHighCalculation;
            panelColorJump.BackColor = settings.Signal.ColorJump;


            
            EditAnalysisMinChangePercentage.Value = settings.Signal.AnalysisMinChangePercentage;
            EditAnalysisMaxChangePercentage.Value = settings.Signal.AnalysisMaxChangePercentage;
            EditLogAnalysisMinMaxChangePercentage.Checked = settings.Signal.LogAnalysisMinMaxChangePercentage;

            EditSymbolMustExistsDays.Value = settings.Signal.SymbolMustExistsDays;
            EditLogSymbolMustExistsDays.Checked = settings.Signal.LogSymbolMustExistsDays;

            EditMinimumTickPercentage.Value = settings.Signal.MinimumTickPercentage;
            EditLogMinimumTickPercentage.Checked = settings.Signal.LogMinimumTickPercentage;
       
            EditSbmMa200AndMa50Percentage.Value = settings.Signal.SbmMa200AndMa50Percentage;
            EditSbmMa50AndMa20Percentage.Value = settings.Signal.SbmMa50AndMa20Percentage;
            EditSbmMa200AndMa20Percentage.Value = settings.Signal.SbmMa200AndMa20Percentage;
            EditSbm2CandlesForMacdRecovery.Value = settings.Signal.Sbm2CandlesForMacdRecovery;
            EditSbm2CandlesLookbackCount.Value = settings.Signal.Sbm2CandlesLookbackCount;
            EditSbm2UpperPartOfBbPercentage.Value = settings.Signal.Sbm2UpperPartOfBbPercentage;
            EditSbm2LowerPartOfBbPercentage.Value = settings.Signal.Sbm2LowerPartOfBbPercentage;
            EditSbm3CandlesForBBRecovery.Value = settings.Signal.Sbm3CandlesLookbackCount;
            EditSbm3CandlesForBBRecoveryPercentage.Value = settings.Signal.Sbm3CandlesBbRecoveryPercentage;

            checkBoxAnalysisSbm2Oversold.Checked = settings.Signal.AnalysisSbm2Oversold;
            checkBoxAnalysisSbm3Oversold.Checked = settings.Signal.AnalysisSbm3Oversold;
            checkBoxAnalysisSbm2Overbought.Checked = settings.Signal.AnalysisSbm2Overbought;
            checkBoxAnalysisSbm3Overbought.Checked = settings.Signal.AnalysisSbm3Overbought;

            EditSbmMa200AndMa20Crossing.Checked = settings.Signal.SbmMa200AndMa20Crossing;
            EditSbmMa200AndMa20Lookback.Value = settings.Signal.SbmMa200AndMa20Lookback;
            EditSbmMa200AndMa50Crossing.Checked = settings.Signal.SbmMa200AndMa50Crossing;
            EditSbmMa200AndMa50Lookback.Value = settings.Signal.SbmMa200AndMa50Lookback;
            EditSbmMa50AndMa20Crossing.Checked = settings.Signal.SbmMa50AndMa20Crossing;
            EditSbmMa50AndMa20Lookback.Value = settings.Signal.SbmMa50AndMa20Lookback;

            EditAnalysisPriceCrossingMa.Checked = settings.Signal.AnalysisPriceCrossingMa;


            // --------------------------------------------------------------------------------
            // Black & White list
            // --------------------------------------------------------------------------------

            checkBoxUseBlackListOversold.Checked = settings.UseBlackListOversold;
            textBoxBlackListOversold.Text = string.Join(",", settings.BlackListOversold);

            checkBoxUseWhiteListOversold.Checked = settings.UseWhiteListOversold;
            textBoxWhiteListOversold.Text = string.Join(",", settings.WhiteListOversold);

            checkBoxUseBlackListOverbought.Checked = settings.UseBlackListOverbought;
            textBoxBlackListOverbought.Text = string.Join(",", settings.BlackListOverbought);

            checkBoxUseWhiteListOverbought.Checked = settings.UseWhiteListOverbought;
            textBoxWhiteListOverbought.Text = string.Join(",", settings.WhiteListOverbought);

            SetGrayed(null, null);
        }


        private void SetQuoteCoin(string quoteName, CheckBox fetchCandles, NumericUpDown minVolume, NumericUpDown minPrice, CheckBox createSignals, Panel panelColor)
        {
            CryptoQuoteData quote;
            if (!settings.QuoteCoins.TryGetValue(quoteName, out quote))
            {
                quote = new CryptoQuoteData();
                quote.Name = quoteName;
                settings.QuoteCoins.Add(quote.Name, quote);
            }
            fetchCandles.Checked = quote.FetchCandles;
            minVolume.Value = quote.MinimalVolume;
            minPrice.Value = quote.MinimalPrice;
            createSignals.Checked = quote.CreateSignals;
            panelColor.BackColor = quote.DisplayColor;
        }

        private void GetQuoteCoin(string quoteName, CheckBox fetchCandles, NumericUpDown minVolume, NumericUpDown minPrice, CheckBox createSignals, Panel panelColor)
        {
            CryptoQuoteData quote;
            if (!settings.QuoteCoins.TryGetValue(quoteName, out quote))
            {
                quote = new CryptoQuoteData();
                quote.Name = quoteName;
                settings.QuoteCoins.Add(quote.Name, quote);
            }
            quote.FetchCandles = fetchCandles.Checked;
            quote.MinimalVolume = minVolume.Value;
            quote.MinimalPrice = minPrice.Value;
            quote.CreateSignals = createSignals.Checked;
            quote.DisplayColor = panelColor.BackColor;
        }


        private void buttonCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }


        private void ButtonOk_Click(object sender, EventArgs e)
        {
            //settings.General.GetCandleInterval = (int)EditGetCandleInterval.Value;

            // ------------------------------------------------------------------------------
            // General
            // ------------------------------------------------------------------------------
            settings.General.BlackTheming = EditBlackTheming.Checked;
            settings.General.TradingApp = (TradingApp)EditTradingApp.SelectedIndex;
            settings.General.DoubleClickAction = (DoubleClickAction)EditDoubleClickAction.SelectedIndex;
            settings.Signal.SoundHeartBeatMinutes = (int)EditSoundHeartBeatMinutes.Value;
            settings.General.FontName = this.Font.Name;
            settings.General.FontSize = this.Font.Size;

            settings.Signal.HideTechnicalStuffSignals = EditHideTechnicalStuffSignals.Checked;
            settings.Signal.RemoveSignalAfterxCandles = (int)EditGlobalDataRemoveSignalAfterxCandles.Value;


            // ------------------------------------------------------------------------------
            // Base coins
            // ------------------------------------------------------------------------------
            GetQuoteCoin("BNB", EditFetchCandlesBNB, EditMinVolumeBNB, EditMinPriceBNB, EditCreateSignalsBNB, panelColorBNB);
            GetQuoteCoin("BTC", EditFetchCandlesBTC, EditMinVolumeBTC, EditMinPriceBTC, EditCreateSignalsBTC, panelColorBTC);
            GetQuoteCoin("BUSD", EditFetchCandlesBUSD, EditMinVolumeBUSD, EditMinPriceBUSD, EditCreateSignalsBUSD, panelColorBUSD);
            GetQuoteCoin("ETH", EditFetchCandlesETH, EditMinVolumeETH, EditMinPriceETH, EditCreateSignalsETH, panelColorETH);
            GetQuoteCoin("USDT", EditFetchCandlesUSDT, EditMinVolumeUSDT, EditMinPriceUSDT, EditCreateSignalsUSDT, panelColorUSDT);
            GetQuoteCoin("EUR", EditFetchCandlesEUR, EditMinVolumeEUR, EditMinPriceEUR, EditCreateSignalsEUR, panelColorEUR);

            // ------------------------------------------------------------------------------
            // Signals
            // ------------------------------------------------------------------------------
            settings.Signal.AnalysisMinChangePercentage = EditAnalysisMinChangePercentage.Value;
            settings.Signal.AnalysisMaxChangePercentage = EditAnalysisMaxChangePercentage.Value;
            settings.Signal.LogAnalysisMinMaxChangePercentage = EditLogAnalysisMinMaxChangePercentage.Checked;

            settings.Signal.Barometer1hMinimal = EditBarometer1hMinimal.Value;
            settings.Signal.LogBarometerToLow = EditLogBarometerToLow.Checked;

            settings.Signal.SymbolMustExistsDays = EditSymbolMustExistsDays.Value;
            settings.Signal.LogSymbolMustExistsDays = EditLogSymbolMustExistsDays.Checked;

            settings.Signal.MinimumTickPercentage = EditMinimumTickPercentage.Value;
            settings.Signal.LogMinimumTickPercentage = EditLogMinimumTickPercentage.Checked;

            settings.Signal.AnalyseInterval = new bool[Enum.GetNames(typeof(CryptoIntervalPeriod)).Length];
            settings.Signal.AnalyseInterval[(int)CryptoIntervalPeriod.interval1m] = EditAnalyseInterval1m.Checked;
            settings.Signal.AnalyseInterval[(int)CryptoIntervalPeriod.interval2m] = EditAnalyseInterval2m.Checked;
            settings.Signal.AnalyseInterval[(int)CryptoIntervalPeriod.interval3m] = EditAnalyseInterval3m.Checked;
            settings.Signal.AnalyseInterval[(int)CryptoIntervalPeriod.interval5m] = EditAnalyseInterval5m.Checked;
            settings.Signal.AnalyseInterval[(int)CryptoIntervalPeriod.interval10m] = EditAnalyseInterval10m.Checked;
            settings.Signal.AnalyseInterval[(int)CryptoIntervalPeriod.interval15m] = EditAnalyseInterval15m.Checked;
            settings.Signal.AnalyseInterval[(int)CryptoIntervalPeriod.interval30m] = EditAnalyseInterval30m.Checked;
            settings.Signal.AnalyseInterval[(int)CryptoIntervalPeriod.interval1h] = EditAnalyseInterval1h.Checked;
            settings.Signal.AnalyseInterval[(int)CryptoIntervalPeriod.interval2h] = EditAnalyseInterval2h.Checked;
            settings.Signal.AnalyseInterval[(int)CryptoIntervalPeriod.interval4h] = EditAnalyseInterval4h.Checked;
            settings.Signal.AnalyseInterval[(int)CryptoIntervalPeriod.interval6h] = EditAnalyseInterval6h.Checked;
            settings.Signal.AnalyseInterval[(int)CryptoIntervalPeriod.interval8h] = EditAnalyseInterval8h.Checked;
            settings.Signal.AnalyseInterval[(int)CryptoIntervalPeriod.interval12h] = EditAnalyseInterval12h.Checked;
            settings.Signal.AnalyseInterval[(int)CryptoIntervalPeriod.interval1d] = EditAnalyseInterval1d.Checked;

            // STOBB
            settings.Signal.StobbBBMinPercentage = EditStobbBBMinPercentage.Value;
            settings.Signal.StobbBBMaxPercentage = EditStobbBBMaxPercentage.Value;
            settings.Signal.PlaySoundStobbSignal = EditPlaySoundStobbSignal.Checked;
            settings.Signal.PlaySpeechStobbSignal = EditPlaySpeechStobbSignal.Checked;
            settings.Signal.AnalysisShowStobbOversold = EditAnalysisShowStobbOversold.Checked;
            settings.Signal.AnalysisShowStobbOverbought = EditAnalysisShowStobbOverbought.Checked;
            settings.Signal.SoundStobbOverbought = EditSoundStobbOverbought.Text;
            settings.Signal.SoundStobbOversold = EditSoundStobbOversold.Text;
            settings.Signal.StobIncludeRsi = EditStobIncludeRsi.Checked;
            settings.Signal.StobIncludeSoftSbm = EditStobIncludeSoftSbm.Checked;
            settings.Signal.ColorStobb = panelColorStobb.BackColor;

            // SBM x
            settings.Signal.SbmBBMinPercentage = EditSbmBBMinPercentage.Value;
            settings.Signal.SbmBBMaxPercentage = EditSbmBBMaxPercentage.Value;
            settings.Signal.SbmUseLowHigh = EditStobbUseLowHigh.Checked;

            // SBM 1
            settings.Signal.PlaySoundSbmSignal = EditPlaySoundSbmSignal.Checked;
            settings.Signal.PlaySpeechSbmSignal = EditPlaySpeechSbmSignal.Checked;
            settings.Signal.AnalysisShowSbmOversold = EditAnalysisShowSbmOversold.Checked;
            settings.Signal.AnalysisShowSbmOverbought = EditAnalysisShowSbmOverbought.Checked;
            settings.Signal.SoundSbmOverbought = EditSoundFileSbmOverbought.Text;
            settings.Signal.SoundSbmOversold = EditSoundFileSbmOversold.Text;
            settings.Signal.ColorSbm = panelColorSbm.BackColor;

            // SBM-X
            settings.Signal.SbmMa200AndMa50Percentage = EditSbmMa200AndMa50Percentage.Value;
            settings.Signal.SbmMa50AndMa20Percentage = EditSbmMa50AndMa20Percentage.Value;
            settings.Signal.SbmMa200AndMa20Percentage = EditSbmMa200AndMa20Percentage.Value;
            settings.Signal.Sbm2CandlesForMacdRecovery = (int)EditSbm2CandlesForMacdRecovery.Value;
            settings.Signal.Sbm2CandlesLookbackCount = (int)EditSbm2CandlesLookbackCount.Value;
            settings.Signal.Sbm2UpperPartOfBbPercentage = EditSbm2UpperPartOfBbPercentage.Value;
            settings.Signal.Sbm2LowerPartOfBbPercentage = EditSbm2LowerPartOfBbPercentage.Value;
            settings.Signal.Sbm3CandlesLookbackCount = (int)EditSbm3CandlesForBBRecovery.Value;
            settings.Signal.Sbm3CandlesBbRecoveryPercentage = EditSbm3CandlesForBBRecoveryPercentage.Value;

            settings.Signal.AnalysisSbm2Oversold = checkBoxAnalysisSbm2Oversold.Checked;
            settings.Signal.AnalysisSbm3Oversold = checkBoxAnalysisSbm3Oversold.Checked;
            settings.Signal.AnalysisSbm2Overbought = checkBoxAnalysisSbm2Overbought.Checked;
            settings.Signal.AnalysisSbm3Overbought = checkBoxAnalysisSbm3Overbought.Checked;

            settings.Signal.SbmMa200AndMa20Crossing = EditSbmMa200AndMa20Crossing.Checked;
            settings.Signal.SbmMa200AndMa20Lookback = (int)EditSbmMa200AndMa20Lookback.Value;
            settings.Signal.SbmMa200AndMa50Crossing = EditSbmMa200AndMa50Crossing.Checked;
            settings.Signal.SbmMa200AndMa50Lookback = (int)EditSbmMa200AndMa50Lookback.Value;
            settings.Signal.SbmMa50AndMa20Crossing = EditSbmMa50AndMa20Crossing.Checked;
            settings.Signal.SbmMa50AndMa20Lookback = (int)EditSbmMa50AndMa20Lookback.Value;

            // JUMP
            settings.Signal.PlaySoundCandleJumpSignal = EditPlaySoundCandleJumpSignal.Checked;
            settings.Signal.PlaySpeechCandleJumpSignal = EditPlaySpeechCandleJumpSignal.Checked;
            settings.Signal.AnalysisShowCandleJumpDown = EditAnalysisShowCandleJumpDown.Checked;
            settings.Signal.AnalysisShowCandleJumpUp = EditAnalysisShowCandleJumpUp.Checked;
            settings.Signal.SoundCandleJumpDown = EditSoundFileCandleJumpDown.Text;
            settings.Signal.SoundCandleJumpUp = EditSoundFileCandleJumpUp.Text;
            settings.Signal.AnalysisCandleJumpPercentage = EditAnalysisCandleJumpPercentage.Value;
            settings.Signal.JumpCandlesLookbackCount = (int)EditJumpCandlesLookbackCount.Value;
            settings.Signal.JumpUseLowHighCalculation = EditJumpUseLowHighCalculation.Checked;
            settings.Signal.ColorJump = panelColorJump.BackColor;


            // --------------------------------------------------------------------------------
            // Black & White list
            // --------------------------------------------------------------------------------

            settings.UseBlackListOversold = checkBoxUseBlackListOversold.Checked;
            string blackText = textBoxBlackListOversold.Text.Replace(" ", "").Replace("\r\n", "");
            settings.BlackListOversold = blackText.Split(',').ToList<string>();
            settings.BlackListOversold.Sort();

            settings.UseWhiteListOversold = checkBoxUseWhiteListOversold.Checked;
            string whiteText = textBoxWhiteListOversold.Text.Replace(" ", "").Replace("\r\n", "");
            settings.WhiteListOversold = whiteText.Split(',').ToList<string>();
            settings.WhiteListOversold.Sort();

            settings.UseBlackListOverbought = checkBoxUseBlackListOverbought.Checked;
            blackText = textBoxBlackListOverbought.Text.Replace(" ", "").Replace("\r\n", "");
            settings.BlackListOverbought = blackText.Split(',').ToList<string>();
            settings.BlackListOverbought.Sort();

            settings.UseWhiteListOverbought = checkBoxUseWhiteListOverbought.Checked;
            whiteText = textBoxWhiteListOverbought.Text.Replace(" ", "").Replace("\r\n", "");
            settings.WhiteListOverbought = whiteText.Split(',').ToList<string>();
            settings.WhiteListOverbought.Sort();


            // --------------------------------------------------------------------------------
            // Extra
            // --------------------------------------------------------------------------------
            settings.Signal.AnalysisPriceCrossingMa = EditAnalysisPriceCrossingMa.Checked;

            GlobalData.InitWhiteAndBlackListSettings();
            DialogResult = DialogResult.OK;
        }

        private void buttonTestSpeech_Click(object sender, EventArgs e)
        {
            GlobalData.PlaySomeSpeech("Found a signal for BTC/BUSD interval 1m (it is going to the moon)", true);
        }

        private void browseForWavFile(ref System.Windows.Forms.TextBox textBox)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "wav bestanden|*.wav";
            if (!textBox.Text.IsNullOrEmpty())
                openFileDialog.FileName = System.IO.Path.GetFileName(textBox.Text);
            if (!textBox.Text.IsNullOrEmpty())
                openFileDialog.InitialDirectory = System.IO.Path.GetDirectoryName(textBox.Text);

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string fileName = openFileDialog.FileName;
                if (System.IO.File.Exists(fileName))
                {
                    textBox.Text = fileName;
                }
                else
                {
                    MessageBox.Show("Selected file doesn't exist.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void buttonSelectSoundStobbOverbought_Click(object sender, EventArgs e)
        {
            browseForWavFile(ref EditSoundStobbOverbought);
        }

        private void buttonSelectSoundStobbOversold_Click(object sender, EventArgs e)
        {
            browseForWavFile(ref EditSoundStobbOversold);
        }

        private void buttonSelectSoundSbmOverbought_Click(object sender, EventArgs e)
        {
            browseForWavFile(ref EditSoundFileSbmOverbought);
        }

        private void buttonSelectSoundSbmOversold_Click(object sender, EventArgs e)
        {
            browseForWavFile(ref EditSoundFileSbmOversold);
        }

        private void buttonSelectSoundCandleJumpUp_Click(object sender, EventArgs e)
        {
            browseForWavFile(ref EditSoundFileCandleJumpUp);
        }

        private void buttonSelectSoundCandleJumpDown_Click(object sender, EventArgs e)
        {
            browseForWavFile(ref EditSoundFileCandleJumpDown);
        }

        private void buttonPlaySoundStobbOverbought_Click(object sender, EventArgs e)
        {
            GlobalData.PlaySomeMusic(EditSoundStobbOverbought.Text, true);
        }

        private void buttonPlaySoundStobbOversold_Click(object sender, EventArgs e)
        {
            GlobalData.PlaySomeMusic(EditSoundStobbOversold.Text, true);
        }

        private void buttonPlaySoundSbmOverbought_Click(object sender, EventArgs e)
        {
            GlobalData.PlaySomeMusic(EditSoundFileSbmOverbought.Text, true);
        }

        private void buttonPlaySoundSbmOversold_Click(object sender, EventArgs e)
        {
            GlobalData.PlaySomeMusic(EditSoundFileSbmOversold.Text, true);
        }

        private void buttonPlaySoundCandleJumpUp_Click(object sender, EventArgs e)
        {
            GlobalData.PlaySomeMusic(EditSoundFileCandleJumpUp.Text, true);
        }

        private void buttonPlaySoundCandleJumpDown_Click(object sender, EventArgs e)
        {
            GlobalData.PlaySomeMusic(EditSoundFileCandleJumpDown.Text, true);
        }

        private void PickColor(ref Panel panel)
        {
            ColorDialog dlg = new ColorDialog();
            dlg.Color = panel.BackColor;
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                panel.BackColor = dlg.Color;
            }
        }

        private void buttonColorStobb_Click(object sender, EventArgs e)
        {
            PickColor(ref panelColorStobb);
        }

        private void buttonColorSbm_Click(object sender, EventArgs e)
        {
            PickColor(ref panelColorSbm);
        }

        private void buttonColorJump_Click(object sender, EventArgs e)
        {
            PickColor(ref panelColorJump);
        }

        private void buttonReset_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Alle instellingen resetten?", "Attentie!", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                GlobalData.Settings = new Settings();
                GlobalData.DefaultSettings();
                InitSettings(GlobalData.Settings);
            }
        }

        private void buttonColorBTC_Click(object sender, EventArgs e)
        {
            PickColor(ref panelColorBTC);
        }

        private void buttonColorETH_Click(object sender, EventArgs e)
        {
            PickColor(ref panelColorETH);
        }

        private void buttonColorBNB_Click(object sender, EventArgs e)
        {
            PickColor(ref panelColorBNB);
        }

        private void buttonColorBUSD_Click(object sender, EventArgs e)
        {
            PickColor(ref panelColorBUSD);
        }

        private void buttonColorUSDT_Click(object sender, EventArgs e)
        {
            PickColor(ref panelColorUSDT);
        }

        private void buttonColorEUR_Click(object sender, EventArgs e)
        {
            PickColor(ref panelColorEUR);
        }

        private void buttonFontDialog_Click(object sender, EventArgs e)
        {
            FontDialog dialog = new System.Windows.Forms.FontDialog();
            dialog.Font = this.Font;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                this.Font = dialog.Font;
            }

        }
    }
}

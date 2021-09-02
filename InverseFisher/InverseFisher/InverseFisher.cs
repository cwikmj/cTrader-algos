using System;
using cAlgo.API;
using cAlgo.API.Indicators;

namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = false, AccessRights = AccessRights.None)]
    public class InverseFisher : Indicator
    {
        [Parameter(DefaultValue = 5, MinValue = 4)]
        public int Length { get; set; }

        [Output("InverseFisher", LineColor = "OrangeRed", Thickness = 2)]
        public IndicatorDataSeries invfisher { get; set; }

        [Output("5", LineColor = "LightGray", LineStyle = LineStyle.DotsVeryRare)]
        public IndicatorDataSeries plus { get; set; }

        [Output("-5", LineColor = "LightGray", LineStyle = LineStyle.DotsVeryRare)]
        public IndicatorDataSeries minus { get; set; }

        private WeightedMovingAverage tma;
        private RelativeStrengthIndex rsi;
        private IndicatorDataSeries value1;
        private IndicatorDataSeries smooth;

        protected override void Initialize()
        {
            tma = Indicators.WeightedMovingAverage(Bars.TypicalPrices, 8);
            rsi = Indicators.RelativeStrengthIndex(tma.Result, Length);
            value1 = CreateDataSeries();
            smooth = CreateDataSeries();
        }

        public override void Calculate(int index)
        {
            plus[index] = 0.5;
            minus[index] = -0.5;

            double rsiL = rsi.Result[index];
            double rsiH = rsi.Result[index];
            for (int i = index - Length + 1; i <= index; i++)
            {
                if (rsiH < rsi.Result[i])
                {
                    rsiH = rsi.Result[i];
                }
                if (rsiL > rsi.Result[i])
                {
                    rsiL = rsi.Result[i];
                }
            }
            if (rsiH != rsiL)
            {
                value1[index] = 0.1 * (rsi.Result[index] - 50);
                smooth[index] = (value1[index] + 2 * value1[index - 1] + 2 * value1[index - 2] + value1[index - 3]) / 6;
            }
            invfisher[index] = (Math.Exp(2 * smooth[index]) - 1) / (Math.Exp(2 * smooth[index]) + 1);
        }
    }
}

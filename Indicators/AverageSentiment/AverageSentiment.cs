using System;
using cAlgo.API;
using cAlgo.API.Internals;

namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = false, TimeZone = TimeZones.UTC, AccessRights = AccessRights.None, ScalePrecision = 0)]
    public class AverageSentiment : Indicator
    {
        [Parameter("Length", DefaultValue = 8)]
        public int length { get; set; }

        [Parameter("Mode", DefaultValue = 2, MinValue = 0, MaxValue = 2)]
        public int mode { get; set; }

        [Output("Bulls", LineColor = "DarkGreen", Thickness = 2)]
        public IndicatorDataSeries bulls { get; set; }

        [Output("Bears", LineColor = "DarkRed", Thickness = 2)]
        public IndicatorDataSeries bears { get; set; }

        private IndicatorDataSeries intrarange, intrabarbulls, groupbulls, intrabarbears, groupbears;
        private IndicatorDataSeries grouplow, grouphigh, groupopen, grouprange;
        private IndicatorDataSeries TempBufferBulls, TempBufferBears, K1, K2;

        protected override void Initialize()
        {
            intrarange = CreateDataSeries();
            grouplow = CreateDataSeries();
            grouphigh = CreateDataSeries();
            groupopen = CreateDataSeries();
            grouprange = CreateDataSeries();
            intrabarbulls = CreateDataSeries();
            groupbulls = CreateDataSeries();
            intrabarbears = CreateDataSeries();
            groupbears = CreateDataSeries();
            TempBufferBulls = CreateDataSeries();
            TempBufferBears = CreateDataSeries();
            K1 = CreateDataSeries();
            K2 = CreateDataSeries();
        }

        public override void Calculate(int index)
        {
            intrarange[index] = Bars.HighPrices[index] - Bars.LowPrices[index];
            grouplow[index] = Math.Min(Bars.LowPrices[index], Bars.LowPrices.Minimum(length));
            grouphigh[index] = Math.Max(Bars.HighPrices[index], Bars.HighPrices.Maximum(length));
            groupopen[index] = Bars.OpenPrices.Last(length - 1);
            grouprange[index] = grouphigh[index] - grouplow[index];

            if (intrarange[index] == 0)
            {
                K1[index] = 1;
            }
            else
            {
                K1[index] = intrarange[index];
            }
            if (grouprange[index] == 0)
            {
                K2[index] = 1;
            }
            else
            {
                K2[index] = grouprange[index];
            }

            intrabarbulls[index] = ((((Bars.ClosePrices[index] - Bars.LowPrices[index]) + (Bars.HighPrices[index] - Bars.OpenPrices[index])) / 2) * 100) / K1[index];
            groupbulls[index] = ((((Bars.ClosePrices[index] - grouplow[index]) + (grouphigh[index] - groupopen[index])) / 2) * 100) / K2[index];
            intrabarbears[index] = ((((Bars.HighPrices[index] - Bars.ClosePrices[index]) + (Bars.OpenPrices[index] - Bars.LowPrices[index])) / 2) * 100) / K1[index];
            groupbears[index] = ((((grouphigh[index] - Bars.ClosePrices[index]) + (groupopen[index] - grouplow[index])) / 2) * 100) / K2[index];

            if (mode == 0)
            {
                TempBufferBulls[index] = (intrabarbulls[index] + groupbulls[index]) / 2;
                TempBufferBears[index] = (intrabarbears[index] + groupbears[index]) / 2;
            }
            if (mode == 1)
            {
                TempBufferBulls[index] = intrabarbulls[index];
                TempBufferBears[index] = intrabarbears[index];
            }
            if (mode == 2)
            {
                TempBufferBulls[index] = groupbulls[index];
                TempBufferBears[index] = groupbears[index];
            }

            // computing Bulls (with EMA)
            double sumbulls = 0;
            for (var i = index - length + 1; i <= index; i++)
            {
                if (double.IsNaN(TempBufferBulls[index - 1]))
                {
                    bulls[index] = TempBufferBulls[index];
                }
                else
                {
                    sumbulls += TempBufferBulls[i] * 2 + (-1) * TempBufferBulls[i - 1];
                }
            }
            bulls[index] = sumbulls / length;

            // computing Bears (with EMA)
            double sumbears = 0;
            for (var i = index - length + 1; i <= index; i++)
            {
                if (double.IsNaN(TempBufferBears[index - 1]))
                {
                    bears[index] = TempBufferBears[index];
                }
                else
                {
                    sumbears += TempBufferBears[i] * 2 + (-1) * TempBufferBears[i - 1];
                }
            }
            bears[index] = sumbears / length;
        }
    }
}

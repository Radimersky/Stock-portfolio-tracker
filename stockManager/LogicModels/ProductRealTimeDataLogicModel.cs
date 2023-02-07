using stockManager.LogicModels;
using System;

namespace stockManager.ViewModels
{
    public class ProductRealTimeDataLogicModel
    {
        public DateTime? ExDividendDate { get; set; }

        public double? ForwardPE { get; set; }
        public double? TrailingPE { get; set; }
        public DateTime? DividendDate { get; set; }
        public double? TrailingAnnualDividendRate { get; set; }
        public double? TrailingAnnualDividendYield { get; set; }
        public double? SharesOutstanding { get; set; }
        public double? EpsForward { get; set; }
        public double? EpsTrailingTwelveMonths { get; set; }
        public double? BookValue { get; set; }
        public double? RegularMarketPrice { get; set; }
        public double? RegularMarketChange { get; set; }
        public double? RegularMarketChangePercent { get; set; }
        public double? FiftyTwoWeekLowChangePercent { get; set; }
        public double? FiftyTwoWeekLowChange { get; set; }
        public double? FiftyTwoWeekHighChange { get; set; }
        public double? FiftyTwoWeekHighChangePercent { get; set; }

        public double? ForwardDividendYield { get; set; }

        public double? ForwardDividendRate { get; set; }

        public double? FiftyTwoWeekLow { get; set; }

        public double? FiftyTwoWeekHigh { get; set; }

        public MonetaryLogicModel CurrentPrice { get; set; }

    }
}

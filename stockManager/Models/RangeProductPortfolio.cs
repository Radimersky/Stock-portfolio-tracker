using System;
using stockManager.LogicModels;
using YahooFinanceApi;

namespace stockManager.Models
{
    public class RangeProductPortfolio
    {
        public Product Product { get; set; }
        public decimal StockAmountUntilDateFrom { get; set; }
        public decimal BoughtStockAmountAfterDateFrom { get; set; }
        public decimal SoldStockAmountAfterDateFrom { get; set; }
        public Candle StockPriceCandleInDateFrom { get; set; }
        public Candle StockPriceCandleInDateTo { get; set; }
        public MonetaryLogicModel TotalMoneyInvested { get; set; }
        public DateTime dateFrom { get; set; }
        public DateTime dateTo { get; set; }

        public MonetaryLogicModel SoldStockEarnings { get; set; }
        public MonetaryLogicModel MoneyReturnBySelling { get; set; }
        public MonetaryLogicModel UnsoldStocksEarnings { get; set; }
        public decimal UnsoldStocksEarningsPercentage { get; set; }
    }
}

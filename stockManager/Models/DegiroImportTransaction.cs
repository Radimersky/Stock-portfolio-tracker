

using CsvHelper.Configuration.Attributes;

namespace stockManager.Models
{
    public class DegiroImportTransaction
    {
        [Index(0)]
        public string Date { get; set; }
        [Index(1)]
        public string Time { get; set; }
        [Index(2)]
        public string ProductName { get; set; }
        [Index(3)]
        public string ISIN { get; set; }
        [Index(4)]
        public string Reference { get; set; }
        [Index(5)]
        public string MIC { get; set; }
        [Index(6)]
        public string Amount { get; set; }
        [Index(7)]
        public string StockPrice { get; set; }
        [Index(8)]
        public string StockPriceCurrency { get; set; }
        [Index(9)]
        public string HomeCurrencyNoFeesValue { get; set; }
        [Index(10)]
        public string HomeCurrencyNoFeesValueCurrency { get; set; }
        [Index(11)]
        public string NoFeesValue { get; set; }
        [Index(12)]
        public string NoFeesValueCurrency { get; set; }
        [Index(13)]
        public string ExchangeRate { get; set; }
        [Index(14)]
        public string TransactionCosts { get; set; }
        [Index(15)]
        public string TransactionCostsCurrency { get; set; }
        [Index(16)]
        public string TotalValue { get; set; }
        [Index(17)]
        public string TotalValueCurrency { get; set; }
        [Index(18)]
        public string OrderId { get; set; }
    }
}

using CsvHelper.Configuration.Attributes;
using System;

namespace stockManager.Models
{
    public class RevolutImportTransaction
    {

        [Index(1)]
        public string Date { get; set; }

        [Index(2)]
        public string SettleDate { get; set; }

        [Index(3)]
        public string StockPriceCurrency { get; set; }

        [Index(4)]
        public string ActivityType { get; set; }

        [Index(5)]
        public string StockPrice { get; set; }

        [Index(6)]
        public string TotalValue { get; set; }
        
        [Index(7)]
        public string Ticker { get; set; }

        [Index(8)]
        public string Commission { get; set; }

        [Index(9)]
        public string Amount { get; set; }
    }
}
 
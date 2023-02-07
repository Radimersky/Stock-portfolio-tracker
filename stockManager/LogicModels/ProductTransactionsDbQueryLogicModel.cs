using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using stockManager.Models;

namespace stockManager.LogicModels
{
    public class ProductTransactionsDbQueryLogicModel
    {
        public int ProductId { get; set; }

        public int MarketId { get; set; }
        public string MarketMIC { get; set; }
        public string MarketName { get; set; }
        public string ProductName { get; set; }
        public string ProductISIN { get; set; }
        public string ProductTicker { get; set; }
        public Currency ProductCurrency { get; set; }
        public DateTime TransactionDate { get; set; }
        public bool TransactionIsBuy { get; set; }
        public decimal TransactionAmount { get; set; }
        public decimal TransactionTotalValue { get; set; }
        public Currency TransactionTotalValueCurrency { get; set; }
        public decimal TransactionFees { get; set; }
        public Currency TransactionFeesCurrency { get; set; }
        public decimal TransactionStockPrice { get; set; }
        public Currency TransactionStockPriceCurrency { get; set; }
    }
}

using System;
using System.ComponentModel.DataAnnotations;

namespace stockManager.Models
{
    public class Transaction
    {
        public int Id { get; set; }

        [Display(Name = "Date and Time")]
        [Required]
        public DateTime DateTime { get; set; }

        [Required]
        public decimal Amount { get; set; }

        [Display(Name = "Stock Price")]
        [Required]
        public decimal StockPrice { get; set; }

        [Display(Name = "Stock Price Currency")]
        public Currency StockPriceCurrency { get; set; }

        public int StockPriceCurrencyId { get; set; }

        [Required]
        public Product Product { get; set; }
        public int ProductId { get; set; }

        [Required]
        public Market Market { get; set; }
        public int MarketId { get; set; }
        
        [Display(Name = "Exchange Rate")]
        public decimal ExchangeRate { get; set; }

        [Display(Name = "Transaction Costs")]
        public decimal TransactionCosts { get; set; }

        [Display(Name = "Transaction Costs Currency")]
        public Currency TransactionCostsCurrency { get; set; }

        public int? TransactionCostsCurrencyId { get; set; }

        [Display(Name = "Total Value")]
        public decimal TotalValue { get; set; }

        [Display(Name = "Total Value Currency")]
        public Currency TotalValueCurrency { get; set; }
        public int? TotalValueCurrencyId { get; set; }

        [Display(Name = "Buy Transaction")]
        public bool IsBuy { get; set; }

        public string Note { get; set; }
    }
}
 
namespace stockManager.Models
{
    public class CurrentStockState
    {
        public Product Product { get; set; }

        public decimal Amount { get; set; }

        public decimal Value { get; set; }

        public Currency ValueCurrency { get; set; }

        public decimal NetEarnings { get; set; }

        public Currency NetEarningsCurrency { get; set; }

        public decimal NetEarningsPercentage { get; set; }

    }
}
 
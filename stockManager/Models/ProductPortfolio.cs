using stockManager.LogicModels;
using stockManager.ViewModels;

namespace stockManager.Models
{
    public class ProductPortfolio
    {
        public Product Product { get; set; }

        public decimal Amount { get; set; }

        public MonetaryLogicModel MoneyInvested { get; set; }

        public MonetaryLogicModel ProductTotalValue { get; set; }

        public MonetaryLogicModel Fees { get; set; }

        public MonetaryLogicModel EarningsWithoutFees { get; set; }

        public decimal EarningsWithoutFeesPercentage { get; set; } = 0;

        public MonetaryLogicModel NetEarnings { get; set; }

        public decimal NetEarningsPercentage { get; set; } = 0;

        public ProductRealTimeDataLogicModel ProductRealTimeData { get; set; }
    }
}

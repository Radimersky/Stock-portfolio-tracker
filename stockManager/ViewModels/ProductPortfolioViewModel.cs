using System.Collections.Generic;
using stockManager.LogicModels;
using stockManager.Models;

namespace stockManager.ViewModels
{
    public class ProductPortfolioViewModel
    {
        public List<ProductPortfolio> ProductPortfolios { get; set; }

        public decimal Amount { get; set; }


        public MonetaryLogicModel NetEarnings { get; set; }
        public decimal NetEarningsPercentage { get; set; }


        public MonetaryLogicModel NoFeeEarnings { get; set; }
        public decimal NoFeeEarningsPercentage { get; set; }

        public MonetaryLogicModel TotalMoneyInvested { get; set; }


    }
}

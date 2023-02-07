using System;
using System.Collections.Generic;
using stockManager.Models;

namespace stockManager.ViewModels
{
    public class RangePortfolioViewModel
    {
        public List<RangeProductPortfolio> rangeProductPortfolios { get; set; }

        public DateTime dateFrom { get; set; }
        public DateTime dateTo { get; set; }
    }
}

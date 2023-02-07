using stockManager.Models;
using System.Collections.Generic;

namespace stockManager.ViewModels
{
    public class NewMarketViewModel
    {

        public IEnumerable<Market> Markets { get; set; }

        public Market Market { get; set; }

    }
}

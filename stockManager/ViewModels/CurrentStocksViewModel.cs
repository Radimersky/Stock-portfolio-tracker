using stockManager.Models;
using System.Collections.Generic;

namespace stockManager.ViewModels
{
    public class CurrentStocksViewModel
    {

        public IEnumerable<CurrentStockState> CurrentStockStates { get; set; }

    }
}

using stockManager.Models;
using System.Collections.Generic;

namespace stockManager.ViewModels
{
    public class NewCurrencyViewModel
    {

        public IEnumerable<Currency> Currencies { get; set; }

        public Currency Currency { get; set; }


    }
}

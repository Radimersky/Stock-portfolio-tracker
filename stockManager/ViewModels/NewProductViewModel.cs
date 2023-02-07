using stockManager.Models;
using System.Collections.Generic;

namespace stockManager.ViewModels
{
    public class NewProductViewModel
    {

        public IEnumerable<Product> Products { get; set; }


        public Product Product { get; set; }

        public IEnumerable<Currency> Currencies { get; set; }

        public Currency Currency { get; set; }
    }
}

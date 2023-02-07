using stockManager.Models;
using System.Collections.Generic;

namespace stockManager.ViewModels
{
    public class NewTransactionViewModel
    {
        public IEnumerable<Currency> Currencies { get; set; }

        public IEnumerable<Market> Markets { get; set; }

        public IEnumerable<Product> Products { get; set; }

        public IEnumerable<Transaction> Transactions { get; set; }

        public Transaction Transaction { get; set; }
    }
}

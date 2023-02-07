using System.Collections.Generic;
using System.Linq;
using stockManager.Models;

namespace stockManager.Data
{
    public class CurrencyDao
    {
        private readonly StockTrackerContext _context;

        public CurrencyDao()
        {
            _context = new StockTrackerContext();
        }

        public Currency Get(string name)
        {
            return  _context.Currencies.Single(c => c.Name == name);
        }

        public Currency Get(int id)
        {
            return _context.Currencies.Single(c => c.Id == id);
        }

        public Currency Get(Currency currency)
        {
            return Get(currency.Id);
        }

        public List<Currency> GetAll()
        {
            return _context.Currencies.ToList();
        }
    }
}

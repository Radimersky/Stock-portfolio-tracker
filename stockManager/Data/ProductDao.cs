using stockManager.Models;
using System.Collections.Generic;
using System.Linq;

namespace stockManager.Data
{
    public class ProductDao
    {
        private readonly StockTrackerContext _context;
        private CurrencyDao currencyDao;

        public ProductDao()
        {
            _context = new StockTrackerContext();
            currencyDao = new CurrencyDao();
        }

        public List<Product> GetAll()
        {
            List<Product> products = _context.Products.ToList();
            foreach (var product in products)
            {
                product.ProductCurrency = currencyDao.Get(product.ProductCurrencyId);
            }

            return products;
        }

        public Product Get(string name)
        {
            return  _context.Products.Single(c => c.Name == name);
        }

        public Product Get(int id)
        {
            return _context.Products.Single(c => c.Id == id);
        }

        public Product Get(Product product)
        {
            return Get(product.Id);
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using stockManager.Data;
using stockManager.Models;
using stockManager.ViewModels;
using System.Linq;


namespace stockManager.Controllers
{
    public class ProductsController : Controller
    {
        private readonly StockTrackerContext _context;
        private readonly CurrencyDao _currencyDao;
        private readonly ProductDao _productDao;

        public ProductsController()
        {
            _context = new StockTrackerContext();
            _currencyDao = new CurrencyDao();
            _productDao = new ProductDao();
        }
        public ActionResult New()
        {
            var viewModel = new NewProductViewModel()
            {
                Products = _productDao.GetAll(),
                Currencies = _currencyDao.GetAll()
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Save(Product product)
        {
            var products = _productDao.GetAll();

            // Already exists
            if (products.Any(item => item.Ticker == product.Ticker))
            {
                if (product.Id == 0)
                {
                    var productsList = _context.Products.ToList();
                    var viewModel = new NewProductViewModel()
                    {
                        Products = productsList,
                        Product = product,
                        Currency = _currencyDao.Get(product.ProductCurrency.Id),
                        Currencies = _currencyDao.GetAll()
                    };

                    ModelState.AddModelError("Product.Ticker", "Ticker already exists");
                    return View("New", viewModel);
                }
            }

            // Add new
            if (product.Id == 0)
            {
                _context.Products.Add(product);
            }
            // Edit currency
            else
            {
                var productInDb = _context.Products.Single(m => m.Id == product.Id);

                productInDb.Name = product.Name;
                productInDb.Ticker = product.Ticker;
                productInDb.ISIN = product.ISIN;
                productInDb.ProductCurrencyId = product.ProductCurrencyId;
            }

            _context.SaveChanges();

            return RedirectToAction("New", "Products");

        }

        public ActionResult Edit(int id)
        {
            var product = _productDao.Get(id);

            if (product == null)
                return NotFound();


            var viewModel = new NewProductViewModel()
            {
                Product = product,
                Products = _productDao.GetAll(),
                Currency = _currencyDao.Get("USD"),
                Currencies = _currencyDao.GetAll()
            };

            return View("Edit", viewModel);

        }


        public ActionResult Remove(int id)
        {
            var product = new Product
            {
                Id = id
            };

            _context.Products.Attach(product);
            _context.Products.Remove(product);
            _context.SaveChanges();

            return RedirectToAction("New", "Products");
        }
    }
}

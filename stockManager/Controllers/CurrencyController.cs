using Microsoft.AspNetCore.Mvc;
using stockManager.Data;
using stockManager.Models;
using stockManager.ViewModels;
using System.Linq;

namespace stockManager.Controllers
{
    public class CurrencyController : Controller
    {
        private readonly StockTrackerContext _context;

        public CurrencyController()
        {
            _context = new StockTrackerContext();
        }

        public ActionResult New()
        {
            var currencies = _context.Currencies.ToList();
            var viewModel = new NewCurrencyViewModel()
            {
                Currencies = currencies
            };

            return View("New", viewModel);
        }

        private bool isInDatabase(Currency currency)
        {
            var currencies = _context.Currencies.ToList();

            return currencies.Any(item => item.Name == currency.Name);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Save(Currency currency)
        {
            // Currency already exist
            if (isInDatabase(currency))
            {
                if (currency.Id == 0)
                {
                    var currencyList = _context.Currencies.ToList();
                    var viewModel = new NewCurrencyViewModel()
                    {
                        Currencies = currencyList,
                        Currency = currency
                    };
                    ModelState.AddModelError("Currency.Name", "Currency already exists");
                    return View("New", viewModel);
                }
            }

            // Add new currency
            if (currency.Id == 0)
            {
                _context.Currencies.Add(currency);
            }
            // Edit currency
            else
            {
                var currencyInDb = _context.Currencies.Single(c => c.Id == currency.Id);
                currencyInDb.Name = currency.Name;
            }

            _context.SaveChanges();
            return RedirectToAction("New", "Currency");
        }

        public ActionResult Edit(int id)
        {
            var currency = _context.Currencies.SingleOrDefault(c => c.Id == id);

            if (currency == null)
                return NotFound();


            var viewModel = new Currency
            {
                Id = currency.Id,
                Name = currency.Name
            };

            return View("Edit", viewModel);

        }


        public ActionResult Remove(int id)
        {
            var currency = new Currency
            {
                Id = id
            };

            _context.Currencies.Attach(currency);
            _context.Currencies.Remove(currency);
            _context.SaveChanges();

            return RedirectToAction("New", "Currency");
        }
    }
}

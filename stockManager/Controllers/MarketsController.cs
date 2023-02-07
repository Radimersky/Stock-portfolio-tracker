using Microsoft.AspNetCore.Mvc;
using stockManager.Data;
using stockManager.Models;
using stockManager.ViewModels;
using System.Linq;

namespace stockManager.Controllers
{
    public class MarketsController : Controller
    {

        private readonly StockTrackerContext _context;

        public MarketsController()
        {
            _context = new StockTrackerContext();
        }

        public ActionResult New()
        {
            var markets = _context.Market.ToList();
            var viewModel = new NewMarketViewModel()
            {
                Markets = markets
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Save(Market market)
        {
            var markets = _context.Market.ToList();

            // Already exists
            if (markets.Any(item => item.MIC == market.MIC))
            {
                if (market.Id == 0)
                {
                    var viewModel = new NewMarketViewModel()
                    {
                        Markets = markets,
                        Market = market
                    };

                    ModelState.AddModelError("Market.MIC", "Market MIC already exists");
                    return View("New", viewModel);
                }

            }
            // Add new
            if (market.Id == 0)
            {
                    _context.Market.Add(market);
            }
            // Edit
            else
            {
                var marketInDb = _context.Market.Single(m => m.Id == market.Id);

                marketInDb.Name = market.Name;
                marketInDb.MIC = market.MIC;
            }

            _context.SaveChanges();

            return RedirectToAction("New", "Markets");
        }

        public ActionResult Edit(int id)
        {
            var market = _context.Market.SingleOrDefault(c => c.Id == id);

            if (market == null)
                return NotFound();

            var viewModel = new Market
            {
                Id = market.Id,
                Name = market.Name,
                MIC = market.MIC
            };

            return View("Edit", viewModel);
        }

        public ActionResult Remove(int id)
        {
            var market = new Market
            {
                Id = id
            };

            _context.Market.Attach(market);
            _context.Market.Remove(market);
            _context.SaveChanges();

            return RedirectToAction("New", "Markets");
        }
    }
}

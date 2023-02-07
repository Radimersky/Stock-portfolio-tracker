using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using stockManager.Data;
using stockManager.Models;
using stockManager.ViewModels;

namespace stockManager.Controllers
{
    public class TransactionsController : Controller
    {
        private readonly StockTrackerContext _context;
        private readonly CurrencyDao _currencyDao;

        public TransactionsController()
        {
            _context = new StockTrackerContext();
            _currencyDao = new CurrencyDao();
        }

        // GET: Transactions
        public async Task<IActionResult> Index()
        {
            return View(await _context.Transactions.ToListAsync());
        }

        public ActionResult New()
        {
            var currencies = _context.Currencies.ToList();
            var markets = _context.Market.ToList();
            var products = _context.Products.ToList();
            var transactions = _context.Transactions.ToList();

            var emptyCurrency = new Currency
            {
                Name = "-"
            };

            foreach (var transaction in transactions)
            {
                transaction.StockPriceCurrency = _context.Currencies.Single(c => c.Id == transaction.StockPriceCurrencyId);
                transaction.TotalValueCurrency = _context.Currencies.Single(c => c.Id == transaction.TotalValueCurrencyId);


                if (transaction.TransactionCostsCurrencyId == null || transaction.TransactionCostsCurrencyId == 0)
                {
                    transaction.TransactionCostsCurrency = emptyCurrency;
                }
                else
                {
                    transaction.TransactionCostsCurrency = _context.Currencies.Single(c => c.Id == transaction.TransactionCostsCurrencyId);

                }
            }
            var viewModel = new NewTransactionViewModel()
            {
                Currencies = currencies,
                Markets = markets,
                Products = products,
                Transactions = transactions
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Save(Transaction transaction)
        {
            if (IsTransactionInDb(transaction))
            {
                if (transaction.Id == 0)
                {
                    var transactions = _context.Transactions.ToList();
                    var currencies = _context.Currencies.ToList();
                    var markets = _context.Market.ToList();
                    var products = _context.Products.ToList();
                    var viewModel = new NewTransactionViewModel()
                    {
                        Currencies = currencies,
                        Markets = markets,
                        Products = products,
                        Transaction = transaction,
                        Transactions = transactions
                    };
                    ModelState.AddModelError("Transaction.DateTime", "Transaction already exists");
                    return View("New", viewModel);
                }
                else
                {
                    ModelState.AddModelError("DateTime", "Transaction already exists");
                    return View("Edit", transaction);
                }
            }

            // Fill null values
            transaction.Product = _context.Products.Single(c => c.Id == transaction.Product.Id);
            transaction.Market = _context.Market.Single(c => c.Id == transaction.Market.Id);
            transaction.StockPriceCurrency = _context.Currencies.Single(c => c.Id == transaction.StockPriceCurrencyId);
            transaction.TotalValueCurrency = _context.Currencies.Single(c => c.Id == transaction.TotalValueCurrencyId);
            transaction.TransactionCostsCurrency = _context.Currencies.Single(c => c.Id == transaction.TransactionCostsCurrencyId);

            // Add new 
            if (transaction.Id == 0)
            {
                _context.Transactions.Add(transaction);
            }
            // Edit 
            else
            {
                var transactionInDb = _context.Transactions.Single(c => c.Id == transaction.Id);
                transactionInDb.IsBuy = transaction.IsBuy;
                transactionInDb.Market = transaction.Market;
                transactionInDb.Note = transaction.Note;
                transactionInDb.Product = transaction.Product;
                transactionInDb.StockPrice = transaction.StockPrice;
                transactionInDb.StockPriceCurrency = transaction.StockPriceCurrency;
                transactionInDb.StockPriceCurrencyId = transaction.StockPriceCurrencyId;
                transactionInDb.TotalValue = transaction.TotalValue;
                transactionInDb.TotalValueCurrency = transaction.TotalValueCurrency;
                transactionInDb.TotalValueCurrencyId = transaction.TotalValueCurrencyId;
                transactionInDb.TransactionCosts = transaction.TransactionCosts;
                transactionInDb.TransactionCostsCurrency = transaction.TransactionCostsCurrency;
                transactionInDb.TransactionCostsCurrencyId = transaction.TransactionCostsCurrencyId;
            }

            _context.SaveChanges();

            return RedirectToAction("New", "Transactions");
        }

        public ActionResult Edit(int id)
        {
            var transaction = _context.Transactions.SingleOrDefault(c => c.Id == id);

            if (transaction == null)
                return NotFound();

            var currencies = _context.Currencies.ToList();
            var markets = _context.Market.ToList();
            var products = _context.Products.ToList();
            var viewModel = new NewTransactionViewModel
            {
                Transaction = transaction,
                Currencies = currencies,
                Markets = markets,
                Products = products

            };

            return View("Edit", viewModel);
        }

        public ActionResult Remove(int id)
        {
            var transaction = _context.Transactions.SingleOrDefault(c => c.Id == id);

            _context.Transactions.Attach(transaction);
            _context.Transactions.Remove(transaction);
            _context.SaveChanges();

            return RedirectToAction("New", "Transactions");
        }

        public ActionResult DegiroImportView()
        {
            var viewModel = new ImportTransactionsViewModel();

            return View("DegiroImport", viewModel);
        }

        public ActionResult RevolutImportView()
        {
            var viewModel = new ImportTransactionsViewModel();

            return View("RevolutImport", viewModel);
        }

        public ActionResult DegiroImport(ImportTransactionsViewModel importTransactions)
        {
            using (TextReader sr = new StringReader(importTransactions.CSV))
            //using (var csv = new CsvReader(sr, CultureInfo.InvariantCulture))
            using (var csv = new CsvReader(sr))
            {
                csv.Configuration.HeaderValidated = null;
                var records = csv.GetRecords<DegiroImportTransaction>().ToList();
                SaveDegiroImportToDb(records);
            }

            return RedirectToAction("New", "Transactions");
        }

        public ActionResult RevolutImport(ImportTransactionsViewModel importTransactions)
        {
            importTransactions.CSV = importTransactions.CSV.Replace(",", ".");
            importTransactions.CSV = importTransactions.CSV.Replace(";", ",");
            using (TextReader sr = new StringReader(importTransactions.CSV))
            //using (var csv = new CsvReader(sr, CultureInfo.InvariantCulture))
            using (var csv = new CsvReader(sr))
            {
                csv.Configuration.HeaderValidated = null;
                //csv.Configuration.Delimiter = ";";
                var records = csv.GetRecords<RevolutImportTransaction>().ToList();

                SaveRevolutImportToDb(records);
            }

            return RedirectToAction("New", "Transactions");
        }

        private Transaction DegiroImportTransactionToTransactionMapper(DegiroImportTransaction importTransaction)
        {
            if (string.IsNullOrEmpty(importTransaction.Amount) &&
               string.IsNullOrEmpty(importTransaction.StockPrice) &&
               string.IsNullOrEmpty(importTransaction.StockPriceCurrency) &&
               string.IsNullOrEmpty(importTransaction.TotalValue) &&
               string.IsNullOrEmpty(importTransaction.TotalValueCurrency) &&
               string.IsNullOrEmpty(importTransaction.ISIN) &&
               string.IsNullOrEmpty(importTransaction.MIC))
            {
                throw new NullReferenceException("Required values are null or empty");
            }

            Currency transactionCostsCurrency = _currencyDao.Get("-");
            if (!string.IsNullOrEmpty(importTransaction.TransactionCostsCurrency))
            {
                transactionCostsCurrency = new Currency
                {
                    Name = importTransaction.TransactionCostsCurrency.ToUpper()
                };
            }

            Currency stockPriceCurrency = new Currency
            {
                Name = importTransaction.StockPriceCurrency?.ToUpper()
            };

            Product product = new Product
            {
                ISIN = importTransaction.ISIN?.ToUpper(),
                Name = importTransaction.ProductName,
                ProductCurrency = stockPriceCurrency,
            };

            Market market = new Market
            {
                MIC = importTransaction.MIC.ToUpper()
            };


            Currency totalValueCurrency = new Currency
            {
                Name = importTransaction.TotalValueCurrency?.ToUpper()
            };


            Transaction transaction = new Transaction
            {
                DateTime = ParseDateTime(importTransaction.Date, importTransaction.Time),
                Amount = Convert.ToDecimal(importTransaction.Amount, CultureInfo.InvariantCulture),
                StockPrice = Convert.ToDecimal(importTransaction.StockPrice, CultureInfo.InvariantCulture),
                StockPriceCurrency = stockPriceCurrency,
                StockPriceCurrencyId = totalValueCurrency.Id,
                Product = product,
                Market = market,
                TotalValue = Convert.ToDecimal(importTransaction.TotalValue, CultureInfo.InvariantCulture),
                TotalValueCurrency = totalValueCurrency,
                TotalValueCurrencyId = totalValueCurrency.Id,
                IsBuy = Convert.ToDecimal(importTransaction.Amount, CultureInfo.InvariantCulture) >= 0,
                TransactionCostsCurrency = transactionCostsCurrency,
                TransactionCostsCurrencyId = transactionCostsCurrency.Id,
            };

            if (!string.IsNullOrEmpty(importTransaction.TransactionCosts))
            {
                transaction.TransactionCosts = Convert.ToDecimal(importTransaction.TransactionCosts, CultureInfo.InvariantCulture);
            }
            else
            {
                var emptyCurrency = new Currency
                {
                    Name = "-"
                };

                if (!IsCurrencyInDb(emptyCurrency))
                {
                    _context.Currencies.Add(emptyCurrency);
                    _context.SaveChanges();
                    var dbCurrency = _context.Currencies.Single(c => c.Name == emptyCurrency.Name);
                    transaction.TransactionCostsCurrency = dbCurrency;
                    transaction.TransactionCostsCurrencyId = dbCurrency.Id;
                }
            }

            if (!string.IsNullOrEmpty(importTransaction.ExchangeRate))
            {
                transaction.ExchangeRate = Convert.ToDecimal(importTransaction.ExchangeRate, CultureInfo.InvariantCulture);
            }

            return transaction;
        }

        private Transaction RevolutImportTransactionToTransactionMapper(RevolutImportTransaction importTransaction)
        {
            String[] dateSplit = importTransaction.Date.Split("/");

            Currency stockPriceCurrency = new Currency
            {
                Name = importTransaction.StockPriceCurrency.ToUpper()
            };

            Product product = new Product
            {
                ISIN = importTransaction.Ticker,
                Ticker = importTransaction.Ticker,
                ProductCurrency = stockPriceCurrency,
            };

            Market market = new Market
            {
                MIC = "-"
            };

            Transaction transaction = new Transaction
            {
                DateTime = ParseDateTime(dateSplit[1] + "-" + dateSplit[0] + "-" + dateSplit[2], "00:00"),
                Amount = Convert.ToDecimal(importTransaction.Amount, CultureInfo.InvariantCulture),
                StockPrice = Convert.ToDecimal(importTransaction.StockPrice, CultureInfo.InvariantCulture),
                StockPriceCurrency = stockPriceCurrency,
                StockPriceCurrencyId = 0,
                Product = product,
                Market = market,
                TotalValue = Convert.ToDecimal(importTransaction.TotalValue, CultureInfo.InvariantCulture),
                TotalValueCurrency = stockPriceCurrency,
                TotalValueCurrencyId = 0,
                IsBuy = Convert.ToDecimal(importTransaction.Amount, CultureInfo.InvariantCulture) >= 0,
                TransactionCostsCurrency = stockPriceCurrency,
                TransactionCostsCurrencyId = 0,
            };

            return transaction;
        }


        private bool IsCurrencyInDb(Currency currency)
        {
            if (currency == null)
                return true;

            var matched = _context.Currencies.FirstOrDefault(item => item.Name.Equals(currency.Name, StringComparison.OrdinalIgnoreCase));
            return matched != null;
        }

        private bool IsProductInDb(Product product)
        {
            if (product == null)
                return true;

            var matched = _context.Products.FirstOrDefault(item => item.ISIN.Equals(product.ISIN, StringComparison.OrdinalIgnoreCase));
            return matched != null;
        }

        private bool IsMarketInDb(Market market)
        {
            if (market == null)
                return true;

            var matched = _context.Market.FirstOrDefault(item => item.MIC.Equals(market.MIC, StringComparison.OrdinalIgnoreCase));
            return matched != null;
        }

        private void SaveDegiroImportToDb(IEnumerable<DegiroImportTransaction> importTransactions)
        {
            List<Transaction> transactions = new List<Transaction>();

            foreach (var importTransaction in importTransactions)
                transactions.Add(DegiroImportTransactionToTransactionMapper(importTransaction));

            foreach (var transaction in transactions)
            {
                if (!IsTransactionInDb(transaction))
                {
                    if (!IsCurrencyInDb(transaction.Product.ProductCurrency))
                    {
                        _context.Currencies.Add(transaction.Product.ProductCurrency);
                        _context.SaveChanges();
                    }
                    transaction.Product.ProductCurrency = _context.Currencies.Single(c => c.Name == transaction.Product.ProductCurrency.Name);

                    if (!IsCurrencyInDb(transaction.StockPriceCurrency))
                    {
                        _context.Currencies.Add(transaction.StockPriceCurrency);
                        _context.SaveChanges();
                    }
                    transaction.StockPriceCurrency = _context.Currencies.Single(c => c.Name == transaction.StockPriceCurrency.Name);

                    if (!IsCurrencyInDb(transaction.TotalValueCurrency))
                    {
                        _context.Currencies.Add(transaction.TotalValueCurrency);
                        _context.SaveChanges();
                    }
                    transaction.TotalValueCurrency = _context.Currencies.Single(c => c.Name == transaction.TotalValueCurrency.Name);

                    if (transaction.TransactionCostsCurrency != null)
                    {
                        if (!IsCurrencyInDb(transaction.TransactionCostsCurrency))
                        {
                            _context.Currencies.Add(transaction.TransactionCostsCurrency);
                            _context.SaveChanges();
                        }
                        transaction.TransactionCostsCurrency = _context.Currencies.Single(c => c.Name == transaction.TransactionCostsCurrency.Name);
                    }

                    if (!IsMarketInDb(transaction.Market))
                    {
                        _context.Market.Add(transaction.Market);
                        _context.SaveChanges();
                    }
                    transaction.Market = _context.Market.Single(c => c.MIC == transaction.Market.MIC);


                    if (!IsProductInDb(transaction.Product))
                    {
                        _context.Products.Add(transaction.Product);
                        _context.SaveChanges();
                    }
                    transaction.Product = _context.Products.Single(c => c.ISIN == transaction.Product.ISIN);


                    _context.Transactions.Add(transaction);

                }

                _context.SaveChanges();
            }
        }

        private void SaveRevolutImportToDb(IEnumerable<RevolutImportTransaction> importTransactions)
        {
            List<Transaction> transactions = new List<Transaction>();

            foreach (var importTransaction in importTransactions)
            {
                if (importTransaction.ActivityType == "BUY" || importTransaction.ActivityType == "SELL")
                    transactions.Add(RevolutImportTransactionToTransactionMapper(importTransaction));
            }

            foreach (var transaction in transactions)
            {
                if (!IsRevolutTransactionInDb(transaction))
                {
                    if (!IsCurrencyInDb(transaction.StockPriceCurrency))
                    {
                        _context.Currencies.Add(transaction.StockPriceCurrency);
                        _context.SaveChanges();
                    }
                    transaction.StockPriceCurrency = _context.Currencies.Single(c => c.Name == transaction.StockPriceCurrency.Name);

                    if (!IsCurrencyInDb(transaction.TotalValueCurrency))
                    {
                        _context.Currencies.Add(transaction.TotalValueCurrency);
                        _context.SaveChanges();
                    }
                    transaction.TotalValueCurrency = _context.Currencies.Single(c => c.Name == transaction.TotalValueCurrency.Name);

                    if (transaction.TransactionCostsCurrency != null)
                    {
                        if (!IsCurrencyInDb(transaction.TransactionCostsCurrency))
                        {
                            _context.Currencies.Add(transaction.TransactionCostsCurrency);
                            _context.SaveChanges();
                        }
                        transaction.TransactionCostsCurrency = _context.Currencies.Single(c => c.Name == transaction.TransactionCostsCurrency.Name);
                    }

                    if (transaction.Product.ProductCurrency != null)
                    {
                        if (!IsCurrencyInDb(transaction.Product.ProductCurrency))
                        {
                            _context.Currencies.Add(transaction.Product.ProductCurrency);
                            _context.SaveChanges();
                        }
                        transaction.Product.ProductCurrency = _context.Currencies.Single(c => c.Name == transaction.Product.ProductCurrency.Name);
                    }

                    if (!IsMarketInDb(transaction.Market))
                    {
                        _context.Market.Add(transaction.Market);
                        _context.SaveChanges();
                    }
                    transaction.Market = _context.Market.Single(c => c.MIC == transaction.Market.MIC);


                    if (!IsProductInDb(transaction.Product))
                    {
                        _context.Products.Add(transaction.Product);
                        _context.SaveChanges();
                    }
                    transaction.Product = _context.Products.Single(c => c.ISIN == transaction.Product.ISIN);


                    _context.Transactions.Add(transaction);

                }

                _context.SaveChanges();
            }
        }

        private bool IsTransactionInDb(Transaction transaction)
        {
            var matched = _context.Transactions.FirstOrDefault(item =>
             item.DateTime.Equals(transaction.DateTime) &&
            item.IsBuy.Equals(transaction.IsBuy) &&
            item.Product.ISIN.Equals(transaction.Product.ISIN) &&
            item.Market.MIC.Equals(transaction.Market.MIC)
            );
            return matched != null;
        }

        private bool IsRevolutTransactionInDb(Transaction transaction)
        {
            var matched = _context.Transactions.FirstOrDefault(item =>
                item.DateTime.Equals(transaction.DateTime) &&
                item.IsBuy.Equals(transaction.IsBuy) &&
                item.Product.Ticker.Equals(transaction.Product.Ticker) &&
                Math.Round(item.Amount, 5).Equals(Math.Round(transaction.Amount, 5))
            );
            return matched != null;
        }

        private DateTime ParseDateTime(string date, string time)
        {
            if (date == null || time == null)
            {
                throw new NullReferenceException("Date or time is null");
            }

            var cultureInfo = new CultureInfo("de-DE");
            string dateString = date + " " + time;
            var dateTime = DateTime.Parse(dateString, cultureInfo);

            return dateTime;
        }

    }
}

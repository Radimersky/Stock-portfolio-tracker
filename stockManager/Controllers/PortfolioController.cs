#nullable enable
using Microsoft.AspNetCore.Mvc;
using stockManager.Data;
using stockManager.LogicModels;
using stockManager.Models;
using stockManager.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YahooFinanceApi;

namespace stockManager.Controllers
{
    public class PortfolioController : Controller
    {

        private readonly StockTrackerContext _context;
        private CurrencyDao currencyDao;

        public PortfolioController()
        {
            _context = new StockTrackerContext();
            currencyDao = new CurrencyDao();
        }

        private DateTime? timeStampToDateTime(long timeStamp)
        {
            DateTime dateTime = new System.DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(timeStamp);
            return dateTime;

        }

        private async Task<IReadOnlyList<Candle>> GetHistoricalDataAsync(string ticker, DateTime startDate, DateTime endDate)
        {
            var history = await Yahoo.GetHistoricalAsync(ticker, startDate, endDate, Period.Daily);
            return history;
        }

        private IReadOnlyList<Candle> GetHistoricalData(string ticker, DateTime startDate, DateTime endDate)
        {
            Task<IReadOnlyList<Candle>> task = Task.Run<IReadOnlyList<Candle>>(async () => await GetHistoricalDataAsync(ticker, startDate, endDate));
            return task.Result;
        }

        private Candle? GetCandle(IReadOnlyList<Candle> listToSearch, DateTime dateToSearch)
        {
            foreach (var candle in listToSearch)
            {
                if (candle.DateTime.Date.Equals(dateToSearch.Date))
                    return candle;
            }

            return null;
        }

        public ActionResult RangePortfolio()
        {

            var transactionsQuery = _context.Transactions
                .Join(
                    _context.Products,
                    transaction => transaction.ProductId,
                    product => product.Id,
                    (transaction, product) => new
                    {
                        transaction.ProductId,
                        transaction.MarketId,
                        ProductISIN = product.ISIN,
                        ProductName = product.Name,
                        ProductTicker = product.Ticker,
                        product.ProductCurrency,
                        TransactionDate = transaction.DateTime,
                        TransactionIsBuy = transaction.IsBuy,
                        TransactionAmount = transaction.Amount,
                        TransactionTotalValue = transaction.TotalValue,
                        TransactionValueCurrency = transaction.TotalValueCurrency,
                        TransactionFees = transaction.TransactionCosts,
                        TransactionFeesCurrency = transaction.TransactionCostsCurrency,
                        TransactionStockPrice = transaction.StockPrice,
                        TransactionStockPriceCurrency = transaction.StockPriceCurrency,
                    }
                )
                .Join(_context.Market,
                    transaction => transaction.MarketId,
                    market => market.Id,
                    (transaction, market) => new ProductTransactionsDbQueryLogicModel
                    {
                        ProductId = transaction.ProductId,
                        MarketId = transaction.MarketId,
                        MarketMIC = market.MIC,
                        MarketName = market.Name,
                        ProductName = transaction.ProductName,
                        ProductISIN = transaction.ProductISIN,
                        ProductTicker = transaction.ProductTicker,
                        ProductCurrency = transaction.ProductCurrency,
                        TransactionDate = transaction.TransactionDate,
                        TransactionIsBuy = transaction.TransactionIsBuy,
                        TransactionAmount = transaction.TransactionAmount,
                        TransactionTotalValue = transaction.TransactionTotalValue,
                        TransactionTotalValueCurrency = transaction.TransactionValueCurrency,
                        TransactionFees = transaction.TransactionFees,
                        TransactionFeesCurrency = transaction.TransactionFeesCurrency,
                        TransactionStockPrice = transaction.TransactionStockPrice,
                        TransactionStockPriceCurrency = transaction.TransactionStockPriceCurrency,

                    }
                )
                .Where(c => c.ProductTicker != null)
                //.OrderBy(x => x.TransactionDate)
                .GroupBy(m => m.ProductId)
                //.OrderBy(a => a.Max(n => n.TransactionDate))
                .Select(x => new
                {
                    Created = x.Key,
                    Items = x.OrderBy(y => y.TransactionDate)
                })
                .Select(g => new
                {
                    ProductTransactions = g.Items.ToList()
                })
                .ToList();

            DateTime dateFrom = new DateTime(2020, 9, 1);
            DateTime dateTo = new DateTime(2021, 12, 31);

            List<RangeProductPortfolio> rangeProductPortfolios = new List<RangeProductPortfolio>();

            foreach (var transactionList in transactionsQuery)
            {
                if (transactionList.ProductTransactions != null)
                {
                    var firstTransaction = transactionList.ProductTransactions.First();
                    //DateTime dateFrom = dateFromGet < firstTransaction.TransactionDate
                    //    ? firstTransaction.TransactionDate.Date
                    //    : dateFromGet.Date;
                    //DateTime dateTo = dateToGet.Date;

                    IReadOnlyList<Candle> dataCandles;
                    try
                    {
                        dataCandles = GetHistoricalData(firstTransaction.ProductTicker, dateFrom.AddDays(-7), dateTo);
                        dateTo = DateTime.Now;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        continue;
                    }

                    Candle stockPriceInDateFrom = GetCandle(dataCandles, dateFrom);
                    Candle stockPriceInDateTo = dataCandles.Last();
                    if (stockPriceInDateFrom == null || stockPriceInDateTo == null)
                        continue;

                    Product product = new Product
                    {
                        Name = firstTransaction.ProductName,
                        ISIN = firstTransaction.ProductISIN,
                        Ticker = firstTransaction.ProductTicker,
                        ProductCurrency = firstTransaction.ProductCurrency
                    };

                    RangeProductPortfolio rangeProdPort = new RangeProductPortfolio
                    {
                        Product = product,
                        StockAmountUntilDateFrom = 0,
                        SoldStockAmountAfterDateFrom = 0,
                        BoughtStockAmountAfterDateFrom = 0,
                        StockPriceCandleInDateFrom = stockPriceInDateFrom,
                        StockPriceCandleInDateTo = stockPriceInDateTo,
                        TotalMoneyInvested = new MonetaryLogicModel(0, product.ProductCurrency),
                        SoldStockEarnings = new MonetaryLogicModel(0, product.ProductCurrency),
                        MoneyReturnBySelling = new MonetaryLogicModel(0, product.ProductCurrency),
                        UnsoldStocksEarnings = new MonetaryLogicModel(0, product.ProductCurrency),
                        dateFrom = dateFrom,
                        dateTo = dateTo,
                    };

                    bool candleNotFound = false;
                    // Create StockAmountUntilDateFrom, TotalMoneyInvested, SoldStockEarnings, SoldStockAmountAfterDateFrom, BoughtStockAmountAfterDateFrom
                    // Transaction must be in ascending order by date
                    foreach (var transaction in transactionList.ProductTransactions)
                    {
                        // Transactions older than dateTo are not useful
                        if (transaction.TransactionDate > dateTo)
                            break;

                        // Transactions older than dateFrom
                        if (transaction.TransactionDate < dateFrom)
                        {
                            rangeProdPort.StockAmountUntilDateFrom += transaction.TransactionAmount;

                            if (!transaction.TransactionIsBuy) // Is sell
                            {
                                decimal sellAmountToRemove = Math.Abs(transaction.TransactionAmount);
                                // Subtract stockAmountToRemove from oldest transactions to newest
                                foreach (var tr in transactionList.ProductTransactions)
                                {
                                    // Skip Sell transactions and already empty transactions
                                    if (!tr.TransactionIsBuy || tr.TransactionAmount.Equals(0))
                                        continue;

                                    // Subtract amount properly
                                    if (tr.TransactionAmount >= sellAmountToRemove)
                                    {
                                        tr.TransactionAmount -= sellAmountToRemove;
                                        sellAmountToRemove = 0;
                                    }
                                    else // tr.Amount < stockAmountToRemove
                                    {
                                        sellAmountToRemove -= tr.TransactionAmount;
                                        tr.TransactionAmount = 0;
                                    }

                                    // We are finished
                                    if (sellAmountToRemove.Equals(0))
                                        break;
                                }
                            }
                        }
                        // Process transactions in range dateFrom - dateTo
                        else if (transaction.TransactionDate <= dateTo)
                        {
                            if (transaction.TransactionIsBuy) // is Buy
                            {
                                rangeProdPort.BoughtStockAmountAfterDateFrom += transaction.TransactionAmount;
                                rangeProdPort.TotalMoneyInvested.Add(
                                    new MonetaryLogicModel(transaction.TransactionTotalValue, transaction.TransactionTotalValueCurrency)
                                    , transaction.TransactionDate);
                            }
                            else // Is Sell
                            {
                                rangeProdPort.SoldStockAmountAfterDateFrom += transaction.TransactionAmount;

                                rangeProdPort.MoneyReturnBySelling.Add(
                                    new MonetaryLogicModel(transaction.TransactionTotalValue, transaction.TransactionTotalValueCurrency)
                                    , transaction.TransactionDate);

                                decimal stockAmountToRemove = Math.Abs(transaction.TransactionAmount);
                                foreach (var tr in transactionList.ProductTransactions)
                                {
                                    Candle stockValueCandle = GetCandle(dataCandles, tr.TransactionDate.Date);
                                    if (stockValueCandle == null)
                                    {
                                        candleNotFound = true;
                                        break;
                                    }

                                    // Skip Selling transactions and already empty
                                    if (!tr.TransactionIsBuy || tr.TransactionAmount.Equals(0))
                                        continue;

                                    if (tr.TransactionAmount >= stockAmountToRemove)
                                    {
                                  
                                        decimal sellPartPrice = stockAmountToRemove * transaction.TransactionStockPrice;
                                        decimal buyPartPrice = stockAmountToRemove * tr.TransactionStockPrice;
                                        MonetaryLogicModel withoutFeeEarnings = new MonetaryLogicModel(sellPartPrice - buyPartPrice, tr.ProductCurrency);
                                        MonetaryLogicModel withFeeEarnings = new MonetaryLogicModel(withoutFeeEarnings.Value, tr.ProductCurrency);
                                        withFeeEarnings.Add(new MonetaryLogicModel(tr.TransactionFees, tr.TransactionFeesCurrency), tr.TransactionDate);
                                        rangeProdPort.SoldStockEarnings.Add(withFeeEarnings, tr.TransactionDate);

                                        tr.TransactionAmount -= stockAmountToRemove;
                                        stockAmountToRemove = 0;
                                    }
                                    else // tr.TransactionAmount < stockAmountToRemove
                                    {
                         
                                        decimal sellPartPrice = tr.TransactionAmount * transaction.TransactionStockPrice;
                                        decimal buyPartPrice = tr.TransactionAmount * tr.TransactionStockPrice;
                                        MonetaryLogicModel withoutFeeEarnings = new MonetaryLogicModel(sellPartPrice - buyPartPrice, tr.ProductCurrency);
                                        MonetaryLogicModel withFeeEarnings = new MonetaryLogicModel(withoutFeeEarnings.Value, tr.ProductCurrency);
                                        withFeeEarnings.Add(new MonetaryLogicModel(tr.TransactionFees, tr.TransactionFeesCurrency), tr.TransactionDate);
                                        rangeProdPort.SoldStockEarnings.Add(withFeeEarnings, tr.TransactionDate);

                                        stockAmountToRemove -= tr.TransactionAmount;
                                        tr.TransactionAmount = 0;
                                    }

                                    // We are finished
                                    if (stockAmountToRemove.Equals(0))
                                        break;
                                }
                            }
                        }
                    }

                    if (candleNotFound)
                    {
                        continue;
                    }

                    // Create UnsoldStocksEarnings
                    // Add value of stock in dateFrom
                    rangeProdPort.UnsoldStocksEarnings.Add(new MonetaryLogicModel(rangeProdPort.StockPriceCandleInDateFrom.Close * rangeProdPort.StockAmountUntilDateFrom, firstTransaction.ProductCurrency));
                    foreach (var transaction in transactionList.ProductTransactions)
                    {
                        if (transaction.TransactionDate < dateFrom)
                            continue;

                        if (transaction.TransactionDate > dateTo)
                            break;

                        Candle candleInTransactionDate = GetCandle(dataCandles, transaction.TransactionDate.Date);
                        if (candleInTransactionDate == null)
                            continue;

                        if (transaction.TransactionIsBuy && !transaction.TransactionAmount.Equals(0))
                        {
                            MonetaryLogicModel dateToValue = new MonetaryLogicModel(rangeProdPort.StockPriceCandleInDateTo.Close * transaction.TransactionAmount, transaction.ProductCurrency);
                            MonetaryLogicModel currentTransactionValue = new MonetaryLogicModel(transaction.TransactionTotalValue, transaction.ProductCurrency);
                            // Add earnings compared with dateTo // currentTransactionValue is negative for buy orders
                            rangeProdPort.UnsoldStocksEarnings.Add(dateToValue + currentTransactionValue);
                        }
                    }

                    if (rangeProdPort.UnsoldStocksEarnings.Value.Equals(0))
                    {
                        rangeProdPort.UnsoldStocksEarningsPercentage = 0;
                    }
                    else
                    {
                        rangeProdPort.UnsoldStocksEarningsPercentage =
                            ((((rangeProdPort.StockPriceCandleInDateTo.Close * (rangeProdPort.StockAmountUntilDateFrom + rangeProdPort.BoughtStockAmountAfterDateFrom + rangeProdPort.SoldStockAmountAfterDateFrom))
                               / rangeProdPort.UnsoldStocksEarnings.Value)
                              - 1)
                             * 100);
                    }


                    //rangeProdPort.SoldStockEarnings = rangeProdPort.MoneyReturnBySelling + rangeProdPort.SoldStockEarnings;
                    rangeProductPortfolios.Add(rangeProdPort);
                }
            }

            RangePortfolioViewModel viewModel = new RangePortfolioViewModel
            {
                rangeProductPortfolios = rangeProductPortfolios,
                dateFrom = dateFrom,
                dateTo = dateTo,
            };
            return View("RangePortfolio", viewModel);
        }

        public ActionResult Index()
        {
            var transactionsQuery = _context.Transactions
                .Join(
                    _context.Products,
                    transaction => transaction.ProductId,
                    product => product.Id,
                    (transaction, product) => new
                    {
                        transaction.ProductId,
                        transaction.MarketId,
                        ProductISIN = product.ISIN,
                        ProductName = product.Name,
                        ProductTicker = product.Ticker,
                        product.ProductCurrency,
                        TransactionDate = transaction.DateTime,
                        TransactionIsBuy = transaction.IsBuy,
                        TransactionAmount = transaction.Amount,
                        TransactionTotalValue = transaction.TotalValue,
                        TransactionValueCurrency = transaction.TotalValueCurrency,
                        TransactionFees = transaction.TransactionCosts,
                        TransactionFeesCurrency = transaction.TransactionCostsCurrency,

                    }
                )
                .Join(_context.Market,
                    transaction => transaction.MarketId,
                    market => market.Id,
                    (transaction, market) => new
                    {
                        transaction.ProductId,
                        transaction.MarketId,
                        MarketMIC = market.MIC,
                        MarketName = market.Name,
                        transaction.ProductName,
                        transaction.ProductISIN,
                        transaction.ProductTicker,
                        transaction.ProductCurrency,
                        transaction.TransactionDate,
                        transaction.TransactionIsBuy,
                        transaction.TransactionAmount,
                        transaction.TransactionTotalValue,
                        transaction.TransactionValueCurrency,
                        transaction.TransactionFees,
                        transaction.TransactionFeesCurrency,

                    }
                )
                .Where(c => c.ProductTicker != null)
                .OrderBy(x => x.TransactionDate)
                .GroupBy(m => m.ProductId)
                .Select(g => new
                {
                    ProductTransactions = g.ToList()
                })
                .ToList();

            List<ProductPortfolio> productPortfolios = new List<ProductPortfolio>();

            foreach (var transactionList in transactionsQuery)
            {
                if (transactionList.ProductTransactions != null)
                {
                    var firstTransaction = transactionList.ProductTransactions.First();

                    var securities = Yahoo.Symbols(firstTransaction.ProductTicker)
                        .Fields(
                            Field.TrailingPE, 
                            Field.ForwardPE, 
                            Field.DividendDate, 
                            Field.TrailingAnnualDividendRate, 
                            Field.TrailingAnnualDividendYield, 
                            Field.Currency, 
                            Field.Symbol, 
                            Field.SharesOutstanding, 
                            Field.EpsForward, 
                            Field.EpsTrailingTwelveMonths, 
                            Field.BookValue, 
                            Field.RegularMarketPrice,
                            Field.RegularMarketChange,
                            Field.RegularMarketChangePercent,
                            Field.FiftyTwoWeekHigh,
                            Field.FiftyTwoWeekLow, 
                            Field.FiftyTwoWeekLowChangePercent, 
                            Field.FiftyTwoWeekLowChange, 
                            Field.FiftyTwoWeekHighChange,
                            Field.FiftyTwoWeekHighChangePercent)
                        .QueryAsync();

                    var productRTData = securities.Result[firstTransaction.ProductTicker];
                    ProductRealTimeDataLogicModel productRealTimeData = new ProductRealTimeDataLogicModel
                    {
                        TrailingPE = productRTData.Fields.ContainsKey("TrailingPE") ? productRTData.TrailingPE : 0,
                        ForwardPE = productRTData.Fields.ContainsKey("ForwardPE") ? productRTData.ForwardPE : 0,
                        DividendDate = productRTData.Fields.ContainsKey("DividendDate") ? timeStampToDateTime(productRTData.DividendDate) : null,
                        ExDividendDate = productRTData.Fields.ContainsKey("ExDividendDate") ? timeStampToDateTime(productRTData.DividendDate) : null,
                        TrailingAnnualDividendRate = productRTData.Fields.ContainsKey("TrailingAnnualDividendRate") ? productRTData.TrailingAnnualDividendRate : 0,
                        TrailingAnnualDividendYield = productRTData.Fields.ContainsKey("TrailingAnnualDividendYield") ? productRTData.TrailingAnnualDividendYield : 0,
                        SharesOutstanding = productRTData.Fields.ContainsKey("SharesOutstanding") ? productRTData.SharesOutstanding : 0,
                        EpsForward = productRTData.Fields.ContainsKey("EpsForward") ? productRTData.EpsForward : 0,
                        EpsTrailingTwelveMonths = productRTData.Fields.ContainsKey("EpsTrailingTwelveMonths") ? productRTData.EpsTrailingTwelveMonths : 0,
                        BookValue = productRTData.Fields.ContainsKey("BookValue") ? productRTData.BookValue : 0,
                        RegularMarketPrice = productRTData.Fields.ContainsKey("RegularMarketPrice") ? productRTData.RegularMarketPrice : 0,
                        RegularMarketChange = productRTData.Fields.ContainsKey("RegularMarketChange") ? productRTData.RegularMarketChange : 0,
                        RegularMarketChangePercent = productRTData.Fields.ContainsKey("RegularMarketChangePercent") ? productRTData.RegularMarketChangePercent : 0,
                        ForwardDividendRate = productRTData.Fields.ContainsKey("TrailingAnnualDividendRate") ? productRTData.TrailingAnnualDividendRate : 0,
                        ForwardDividendYield = productRTData.Fields.ContainsKey("TrailingAnnualDividendYield") ? productRTData.TrailingAnnualDividendYield : 0,
                        FiftyTwoWeekHigh = productRTData.Fields.ContainsKey("FiftyTwoWeekHigh") ? productRTData.FiftyTwoWeekHigh : 0,
                        FiftyTwoWeekLow = productRTData.Fields.ContainsKey("FiftyTwoWeekLow") ? productRTData.FiftyTwoWeekLow : 0,
                        FiftyTwoWeekLowChangePercent = productRTData.Fields.ContainsKey("FiftyTwoWeekLowChangePercent") ? productRTData.FiftyTwoWeekLowChangePercent : 0,
                        FiftyTwoWeekLowChange = productRTData.Fields.ContainsKey("FiftyTwoWeekLowChange") ? productRTData.FiftyTwoWeekLowChange : 0,
                        FiftyTwoWeekHighChange = productRTData.Fields.ContainsKey("FiftyTwoWeekHighChange") ? productRTData.FiftyTwoWeekHighChange : 0,
                        FiftyTwoWeekHighChangePercent = productRTData.Fields.ContainsKey("FiftyTwoWeekHighChangePercent") ? productRTData.FiftyTwoWeekHighChangePercent : 0,
                        CurrentPrice = new MonetaryLogicModel(Convert.ToDecimal(productRTData.RegularMarketPrice), currencyDao.Get(productRTData.Currency))
                    };

                    Product product = new Product
                    {
                        Name = firstTransaction.ProductName,
                        ISIN = firstTransaction.ProductISIN,
                        Ticker = firstTransaction.ProductTicker,
                        ProductCurrency = firstTransaction.ProductCurrency
                    };

                    ProductPortfolio productPortfolio = new ProductPortfolio()
                    {
                        Product = product,
                        ProductRealTimeData = productRealTimeData,
                        Amount = 0,
                        ProductTotalValue = new MonetaryLogicModel(0, product.ProductCurrency),
                        NetEarnings = new MonetaryLogicModel(0, product.ProductCurrency),
                        MoneyInvested = new MonetaryLogicModel(0, product.ProductCurrency),
                        EarningsWithoutFees = new MonetaryLogicModel(0, product.ProductCurrency),
                        Fees = new MonetaryLogicModel(0, product.ProductCurrency),
                    };

                    foreach (var transaction in transactionList.ProductTransactions)
                    {
                        productPortfolio.Amount += transaction.TransactionAmount;
                        productPortfolio.MoneyInvested.Add(new MonetaryLogicModel(transaction.TransactionTotalValue, currencyDao.Get(transaction.TransactionValueCurrency.Name)), transaction.TransactionDate);
                        if (productPortfolio.MoneyInvested.Value >= 0)
                            productPortfolio.MoneyInvested.Value = 0;
                        
                        productPortfolio.Fees.Add(new MonetaryLogicModel(transaction.TransactionFees, currencyDao.Get(transaction.TransactionFeesCurrency.Name)), transaction.TransactionDate);
                    }

                    if (productPortfolio.Amount.Equals(0))
                    {
                        continue;
                    }

                    productPortfolio.ProductTotalValue.Add(
                            productPortfolio.Amount * productPortfolio.ProductRealTimeData.CurrentPrice.ConvertTo(productPortfolio.ProductTotalValue.Currency)
                        );


                    //productPortfolio.NetEarnings += productPortfolio.ProductTotalValue + (productPortfolio.ProductRealTimeData.CurrentPrice * productPortfolio.Amount);
                    productPortfolio.NetEarnings += productPortfolio.ProductTotalValue +
                                                    productPortfolio.MoneyInvested;
                    productPortfolio.NetEarnings.Value = Math.Round(productPortfolio.NetEarnings.Value, 2);

                    //productPortfolio.NetEarningsPercentage = Math.Round(
                    //        (((productPortfolio.ProductTotalValue.Value / productPortfolio.Amount) / productRealTimeData.CurrentPrice.ConvertTo("EUR").Value) + 1) * 100
                    //    , 2);


                    productPortfolio.NetEarningsPercentage = Math.Round(
                        (((productPortfolio.ProductTotalValue / productPortfolio.MoneyInvested).Value + 1) * -100)
                        , 2);



                    productPortfolio.EarningsWithoutFees += (productPortfolio.NetEarnings - productPortfolio.Fees);
                    productPortfolio.EarningsWithoutFees.Value = Math.Round(productPortfolio.EarningsWithoutFees.Value, 2);

                    //productPortfolio.EarningsWithoutFeesPercentage = Math.Round(
                    //    ((((productPortfolio.ProductTotalValue.Value - productPortfolio.Fees.Value) / productPortfolio.Amount) / productRealTimeData.CurrentPrice.ConvertTo("EUR").Value) + 1) * 100
                    //    , 2);

                    productPortfolio.EarningsWithoutFeesPercentage = Math.Round(
                        ((((productPortfolio.ProductTotalValue - productPortfolio.Fees) / productPortfolio.MoneyInvested).Value + 1) * -100)
                        , 2);


                    productPortfolios.Add(productPortfolio);
                }
            }

            MonetaryLogicModel noFeeEarningsTotal = new MonetaryLogicModel(0, currencyDao.Get("EUR"));
            MonetaryLogicModel netEarningsTotal = new MonetaryLogicModel(0, currencyDao.Get("EUR"));

            decimal noFeeEarningsTotalPercentage = 0;
            decimal netEarningsTotalPercentage = 0;

            MonetaryLogicModel totalMoneyInvested = new MonetaryLogicModel(0, currencyDao.Get("EUR"));


            foreach (var portfolio in productPortfolios)
            {
                noFeeEarningsTotal.Add(portfolio.EarningsWithoutFees);
                netEarningsTotal.Add(portfolio.NetEarnings);
                noFeeEarningsTotalPercentage += portfolio.EarningsWithoutFeesPercentage;
                netEarningsTotalPercentage += portfolio.NetEarningsPercentage;
                totalMoneyInvested.Add(portfolio.MoneyInvested);
            }

            noFeeEarningsTotalPercentage = Math.Round(noFeeEarningsTotalPercentage / productPortfolios.Count, 2);
            netEarningsTotalPercentage = Math.Round(netEarningsTotalPercentage / productPortfolios.Count, 2);

            ProductPortfolioViewModel viewModel = new ProductPortfolioViewModel()
            {
                ProductPortfolios = productPortfolios,
                Amount = productPortfolios.Count,
                NetEarnings = netEarningsTotal,
                NoFeeEarnings = noFeeEarningsTotal,
                NetEarningsPercentage = netEarningsTotalPercentage,
                NoFeeEarningsPercentage = noFeeEarningsTotalPercentage,
                TotalMoneyInvested = totalMoneyInvested,
            };



            return View("Index", viewModel);
        }

    }
}

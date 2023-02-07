using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using FusionCharts.DataEngine;
using FusionCharts.Visualization;
using Microsoft.AspNetCore.Mvc;
using stockManager.Data;
using stockManager.LogicModels;
using stockManager.Models;
using YahooFinanceApi;

namespace stockManager.Controllers
{
    public class ChartController : Controller
    {
        private readonly StockTrackerContext _context;

        public ChartController()
        {
            _context = new StockTrackerContext();
        }

        public ActionResult Index()
        {
            ViewData["Failed"] = false;

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
                .Where(c => c.ProductTicker != null)
                .OrderBy(x => x.TransactionDate)
                .ToList();

            // Dictonary of owned stocks and its values followed by time given in transaction order
            var ownedStockAmounts = new Dictionary<string, MonetaryLogicModel>();
            var soldStockProfit = new MonetaryLogicModel(0, new Currency { Name = "EUR" });

            DataTable ChartData = new DataTable();
            ChartData.Columns.Add("Date", typeof(System.DateTime));
            ChartData.Columns.Add("Invested EUR", typeof(System.Decimal));

            ChartData.Columns.Add("Date2", typeof(System.DateTime));
            ChartData.Columns.Add("Portfolio Value", typeof(System.Decimal));

            ChartData.Columns.Add("Date3", typeof(System.DateTime));
            ChartData.Columns.Add("Active Portfolio Profit", typeof(System.Decimal));

            ChartData.Columns.Add("Date4", typeof(System.DateTime));
            ChartData.Columns.Add("Total Profit", typeof(System.Decimal));

            // Total invested amount in EUR
            MonetaryLogicModel totalInvested = new MonetaryLogicModel(0, new Currency {Name = "EUR"});
            var investedInStocks = new Dictionary<string, MonetaryLogicModel>();

            foreach (var tranasction in transactionsQuery)
            {
                // Manage owned stocks in time to calculate portflio value at that time
                if (!ownedStockAmounts.ContainsKey(tranasction.ProductTicker))
                {
                    // Add stock to dictonary
                    ownedStockAmounts.Add(tranasction.ProductTicker, new MonetaryLogicModel(tranasction.TransactionAmount, tranasction.ProductCurrency));
                }
                else
                {
                    // Add transaction value to stock in dictonary
                    ownedStockAmounts[tranasction.ProductTicker] =
                        ownedStockAmounts[tranasction.ProductTicker] += tranasction.TransactionAmount;

                    // Remove stock if amount is less than zero
                    if (ownedStockAmounts[tranasction.ProductTicker].Value <= 0)
                        ownedStockAmounts.Remove(tranasction.ProductTicker);
                }

                // Total portfolio value in given transaction date in EUR
                MonetaryLogicModel ownedStockTotalValue = new MonetaryLogicModel(0, new Currency{Name = "EUR"});
                // Calculate value of each stock in given transaction date
                foreach (var entry in ownedStockAmounts)
                {
                    try
                    {
                        // Get historical stock value
                        var history = Yahoo.GetHistoricalAsync(entry.Key, tranasction.TransactionDate, tranasction.TransactionDate.AddDays(5), Period.Daily).GetAwaiter().GetResult();
                        var stockTotalValueInPortfolio = new MonetaryLogicModel((entry.Value.Value * history[0].Open),
                            new Currency { Name = entry.Value.Currency.Name });
                        ownedStockTotalValue.Add(stockTotalValueInPortfolio, tranasction.TransactionDate);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        ViewData["Failed"] = true;
                        ViewData["ExceptionStock"] = entry.Key;
                        return View();
                    }
                }

                if (!investedInStocks.ContainsKey(tranasction.ProductTicker))
                {
                    // Add stock to dictonary
                    investedInStocks.Add(tranasction.ProductTicker, new MonetaryLogicModel(0, tranasction.TransactionValueCurrency));
                }

                if (new MonetaryLogicModel(tranasction.TransactionTotalValue, tranasction.TransactionValueCurrency).ConvertTo(investedInStocks[tranasction.ProductTicker].Currency).Value > investedInStocks[tranasction.ProductTicker].Value)
                {
                    totalInvested.Add(new MonetaryLogicModel(investedInStocks[tranasction.ProductTicker].Value * -1, investedInStocks[tranasction.ProductTicker].Currency), tranasction.TransactionDate);
                    soldStockProfit.Add(new MonetaryLogicModel(new MonetaryLogicModel(tranasction.TransactionTotalValue, tranasction.TransactionValueCurrency).ConvertTo(investedInStocks[tranasction.ProductTicker].Currency).Value - investedInStocks[tranasction.ProductTicker].Value, investedInStocks[tranasction.ProductTicker].Currency), tranasction.TransactionDate);
                    investedInStocks[tranasction.ProductTicker].Value = 0;
                }
                else
                {
                    // Add transaction to total sum which will be displayed in chart
                    totalInvested.Add(new MonetaryLogicModel((tranasction.TransactionTotalValue * -1), tranasction.TransactionValueCurrency), tranasction.TransactionDate);
                    investedInStocks[tranasction.ProductTicker].Add(new MonetaryLogicModel(tranasction.TransactionTotalValue * -1, tranasction.TransactionValueCurrency), tranasction.TransactionDate);
                }


                
               
                // Add data to chart
                ChartData.Rows.Add(tranasction.TransactionDate, totalInvested.Value, tranasction.TransactionDate, ownedStockTotalValue.Value, tranasction.TransactionDate, ownedStockTotalValue.Value - totalInvested.Value, tranasction.TransactionDate, soldStockProfit.Value + ownedStockTotalValue.Value - totalInvested.Value);
            }


            StaticSource source = new StaticSource(ChartData);
            DataModel model = new DataModel();
            model.DataSources.Add(source);
            Charts.LineChart line = new Charts.LineChart("scroll_chart_db");
            line.Scrollable = true;
            line.Data.Source = model;
            line.Caption.Text = "Portfolio Development In Time";
            line.SubCaption.Text = "Values Are Displayed In Euro";
            line.XAxis.Text = "Time";
            line.YAxis.Text = "Portfolio value (EUR)";
            line.Width.Pixel(1300);
            line.Height.Pixel(1000);
            line.ThemeName = FusionChartsTheme.ThemeName.FUSION;

            try
            {
                ViewData["Chart"] = line.Render();
            }
            catch (Exception e)
            {
                ViewData["Failed"] = true;
            }

            return View();
        }
    }

}

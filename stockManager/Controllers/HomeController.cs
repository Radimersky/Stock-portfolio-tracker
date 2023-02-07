using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using stockManager.Models;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using stockManager.ViewModels;

namespace stockManager.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var tasks = new List<Task>();
            String[] urls = { 
                "https://newsapi.org/v2/everything?q=stockmarket&apiKey=5d2cae82c4a842348a37fe1e52a9df20",
                "https://newsapi.org/v2/everything?q=stocks&apiKey=5d2cae82c4a842348a37fe1e52a9df20",
                "https://newsapi.org/v2/everything?q=investments&apiKey=5d2cae82c4a842348a37fe1e52a9df20",
                "https://newsapi.org/v2/everything?q=crypto&apiKey=5d2cae82c4a842348a37fe1e52a9df20"};

            foreach (var value in urls)
                tasks.Add(GetAsync(value));

            await Task.WhenAll(tasks);

            var news = new List<News>();
            foreach (var task in tasks)
            {
                if (task.IsCompletedSuccessfully)
                {
                    var result = ((Task<string>) task).Result;
                    dynamic json = JObject.Parse(result);
                    for (int i = 0; i < 3; i++)
                    {
                        var article = new News
                        {
                            Title = json.articles[i].title,
                            Url = json.articles[i].url
                        };
                        news.Add(article);
                    }
                }
            }

            var viewModel = new HomeViewModel
            {
                News = news
            };

            return View("Index", viewModel);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private async Task<string> GetAsync(string uri)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            using (HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                return await reader.ReadToEndAsync();
            }
        }
    }
}

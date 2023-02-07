using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;

namespace stockManager.Misc
{
    public class CurrencyConverter
    {
        private static RestClient client = new RestClient("https://www.alphavantage.co/");
        //private static readonly string apiKey = "apikey=FG818G4L96OXYOJE";
        private static readonly string apiKey = "apikey=67YTSFGODPRWTYWP";

        private static Dictionary<(string, string), JObject> ConvertCache =
            new Dictionary<(string, string), JObject>();

        private static JObject TryGetValueFromCache(string from, string to)
        {
            JObject cache;
            ConvertCache.TryGetValue((from, to), out cache);

            return cache;
        }

        // https://www.alphavantage.co/query?function=CURRENCY_EXCHANGE_RATE&from_currency=USD&to_currency=EUR&apikey=FG818G4L96OXYOJE
        //public static decimal Convert(string from, string to, decimal amount)
        //{
        //    if (from == to || from == "-" || to == "-")
        //    {
        //        return amount;
        //    }



        //    var request = new RestRequest("query?function=CURRENCY_EXCHANGE_RATE&from_currency=" + from +
        //                                  "&to_currency=" + to + "&" + apiKey, Method.GET);


        //    var queryResult = client.Execute(request);
        //    dynamic content = JObject.Parse(queryResult.Content);
        //    return content["5. Exchange Rate"] * amount;
        //}

        public static decimal Convert(string from, string to, decimal amount)
        {
            return Convert(from, to, amount, DateTime.Today.AddDays(-1));
        }


        // https://www.alphavantage.co/query?function=FX_DAILY&from_symbol=EUR&to_symbol=USD&outputsize=full&apikey=FG818G4L96OXYOJE
        // https://www.alphavantage.co/query?function=FX_DAILY&from_symbol=EUR&to_symbol=USD&outputsize=full&apikey=67YTSFGODPRWTYWP
        public static decimal Convert(string from, string to, decimal amount, DateTime date)
        {
            if (from == to || from == "-" || to == "-")
            {
                return amount;
            }

            dynamic content = TryGetValueFromCache(from, to);
            
            if (content == null)
            {
                var request = new RestRequest("query?function=FX_DAILY&from_symbol=" + from + "&to_symbol=" + to + "&outputsize=full&" + apiKey, Method.GET);
                var queryResult = client.Execute(request);
                content = JObject.Parse(queryResult.Content);
                ConvertCache.Add((from, to), content);
            }

            // Currency in given date may not exists, if it is weekend day, we try 2 previous days if so
            for (int i = 0; i < 3; i++)
            {
                string dateString = date.AddDays(-i).ToString("yyyy-MM-dd");

                var daily = content["Time Series FX (Daily)"]?[dateString];
                if (daily == null)
                    continue;

                return daily["4. close"] * amount;
            }

            throw new Exception("cannot find exchange rate");
        }

    }
}

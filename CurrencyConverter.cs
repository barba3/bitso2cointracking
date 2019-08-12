using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Bitso2Cointracking
{
    public class CurrencyConverter
    {
        private Dictionary<string, decimal> ExchangeCache = new Dictionary<string, decimal>();

        public async Task<decimal> GetExchangeRate(string from, string to, DateTime date)
        {
            if (string.IsNullOrWhiteSpace(from))
            {
                throw new ArgumentException("Can't be empty", nameof(from));
            }

            if (string.IsNullOrWhiteSpace(to))
            {
                throw new ArgumentException("Can't be empty", nameof(to));
            }

            string formattedDate = date.ToString("yyyy-M-d");
            string cacheKey = GetCacheKey(from, to, formattedDate);
            decimal result;
            bool cacheHit = ExchangeCache.TryGetValue(cacheKey, out result);

            if (!cacheHit)
            {
                from = from.ToUpperInvariant();
                to = to.ToUpperInvariant();
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("https://api.exchangeratesapi.io");
                    var response = await client.GetAsync($"{formattedDate}?symbols={to}&base={from}");
                    var stringResult = await response.Content.ReadAsStringAsync();
                    response.EnsureSuccessStatusCode();
                    var parsedJson = JObject.Parse(stringResult);
                    result = decimal.Parse(parsedJson["rates"].First().First().ToString());
                }

                this.ExchangeCache[cacheKey] = result;
            }

            return result;
        }

        private string GetCacheKey(string from, string to, string formattedDate)
        {
            string key = $"{from.ToUpperInvariant()}_{to.ToUpperInvariant()}_{formattedDate}";
            return key;
        }
    }
}

namespace Bitso2Cointracking
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Newtonsoft.Json.Linq;

    public class CurrencyConverter
    {
        private Dictionary<string, decimal> exchangeCache = new Dictionary<string, decimal>();

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

            string formattedDate = date.ToString("yyyy-M-d", CultureInfo.InvariantCulture);
            string cacheKey = GetCacheKey(from, to, formattedDate);
            decimal result;
            bool cacheHit = this.exchangeCache.TryGetValue(cacheKey, out result);

            if (!cacheHit)
            {
                from = from.ToUpperInvariant();
                to = to.ToUpperInvariant();
                using (var client = new HttpClient())
                {
                    var baseUri = new Uri("https://api.exchangeratesapi.io");
                    var uri = new Uri(baseUri, $"{formattedDate}?symbols={to}&base={from}");
                    client.BaseAddress = baseUri;
                    var response = await client.GetAsync(uri).ConfigureAwait(true);
                    var stringResult = await response.Content.ReadAsStringAsync().ConfigureAwait(true);
                    response.EnsureSuccessStatusCode();
                    var parsedJson = JObject.Parse(stringResult);
                    result = decimal.Parse(parsedJson["rates"].First().First().ToString(), CultureInfo.InvariantCulture);
                }

                this.exchangeCache[cacheKey] = result;
            }

            return result;
        }

        private static string GetCacheKey(string from, string to, string formattedDate)
        {
            string key = $"{from.ToUpperInvariant()}_{to.ToUpperInvariant()}_{formattedDate}";
            return key;
        }
    }
}

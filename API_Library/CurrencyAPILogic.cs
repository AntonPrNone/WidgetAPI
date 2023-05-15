using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace API_Library
{
    public class CurrencyAPILogic
    {
        private readonly string _apiKey;
        private readonly string _baseUrl;

        public CurrencyAPILogic(string apiKey = "37OOCP9QPQT1W0DF")
        {
            _apiKey = apiKey;
            _baseUrl = "https://www.alphavantage.co/query";
        }

        public async Task<AlphaVantageResponse> GetExchangeRateAsync(string fromCurrencyCode, string toCurrencyCode)
        {
            string function = "CURRENCY_EXCHANGE_RATE";
            string fromCurrency = fromCurrencyCode.ToUpper();
            string toCurrency = toCurrencyCode.ToUpper();
            string url = $"{_baseUrl}?function={function}&from_currency={fromCurrency}&to_currency={toCurrency}&apikey={_apiKey}";

            using (HttpClient client = new HttpClient())
            {
                var response = await client.GetAsync(url);
                var json = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<AlphaVantageResponse>(json);
                return result;
            }
        }
    }
}

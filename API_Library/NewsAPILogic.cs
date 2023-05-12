using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace API_Library
{
    public class NewsAPILogic
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly string _countryAbr;

        public NewsAPILogic(string apiKey = "6cafd07bad1d43698779bc55a251edcd", string countryAbr = "ru")
        {
            _apiKey = apiKey;
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "WidgetAPI/0.1");
            _countryAbr = countryAbr;
        }

        public async Task<List<News>> GetNewsAsync()
        {
            string apiUrl;
            if (_countryAbr == "ru") apiUrl = $"https://newsapi.org/v2/top-headlines?sources=rbc&apiKey={_apiKey}";
            else apiUrl = $"https://newsapi.org/v2/top-headlines?country={_countryAbr}&apiKey={_apiKey}";
            var response = await _httpClient.GetAsync(apiUrl);
            var content = await response.Content.ReadAsStringAsync();
            var newsResponse = JsonConvert.DeserializeObject<NewsResponse>(content);

            return newsResponse?.Articles;
        }

        public static implicit operator List<object>(NewsAPILogic v)
        {
            throw new NotImplementedException();
        }
    }
}

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace API_Library
{
    public static class CatAPILogic
    {
        private static HttpClient _httpClient;
        public static async Task<string> GetRandomCatImageAsync()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://api.thecatapi.com/")
            };
            _httpClient.DefaultRequestHeaders.Add("x-api-key", "live_Sfi3ij1ZSNReQOrkkXrNeatc32tpk1hMOBqCuRHJTcMrVxHsTB2Fv4HTP06ezPWY");

            var response = await _httpClient.GetAsync("v1/images/search");
            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();
            var images = JsonConvert.DeserializeObject<List<Cat>>(responseContent);
            return images.FirstOrDefault()?.Url;
        }
    }
}

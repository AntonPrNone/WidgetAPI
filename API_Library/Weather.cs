using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace API_Library
{
    // OpenWeatherMap
    public class Weather
    {
        private readonly string _apiKey;
        private readonly string _country;
        private readonly string _city;
        private readonly string _units;
        private readonly int _days;

        public Weather(string apiKey = "bb422a0ecd67bfdb79b4808c42eafd12", string country = "Russia", string city = "Kazan", string units = "metric", int days = 5)
        {
            _apiKey = apiKey;
            _country = country;
            _city = city;
            _units = units;
            _days = days;
        }

        public async Task<CurrentWeather> GetCurrentWeatherAsync()
        {
            using (var httpClient = new HttpClient())
            {
                var url = $"https://api.openweathermap.org/data/2.5/weather?q={_city},{_country}&units={_units}&lang=ru&appid={_apiKey}";
                var response = await httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<CurrentWeather>(content);
                }

                return null;
            }
        }

        public async Task<WeeklyWeather> GetWeeklyWeatherAsync()
        {
            using (var httpClient = new HttpClient())
            {
                var url = $"https://api.openweathermap.org/data/2.5/forecast?q={_city},{_country}&cnt={_days}&units={_units}&lang=ru&appid={_apiKey}";
                var response = await httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<WeeklyWeather>(content);
                }

                return null;
            }
        }

    }
}

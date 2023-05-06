using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Net;
using System.Windows.Media.Imaging;

namespace API_Library
{
    // Класс для хранения данных о погоде за каждый день на неделе
    public class WeeklyWeather
    {
        [JsonProperty("list")]
        public List<CurrentWeather> DailyWeatherList { get; set; }
    }

    // Класс для хранения данных о текущей погоде
    public class CurrentWeather
    {
        // Погодные условия
        [JsonProperty("weather")]
        public List<Weather> WeatherInfo { get; set; }

        // Основные параметры (температура, влажность, давление и др.)
        [JsonProperty("main")]
        public Main MainInfo { get; set; }

        // Ветер
        [JsonProperty("wind")]
        public Wind WindInfo { get; set; }

        // Видимость (в метрах)
        [JsonProperty("visibility")]
        public int Visibility { get; set; }

        // Время расчета данных (Unix, UTC)
        [JsonProperty("dt")]
        public long Dt { get; set; }

        [JsonIgnore]
        public DateTime DateTime => DateTimeOffset.FromUnixTimeSeconds(Dt).DateTime;

        [JsonIgnore]
        public string DayOfWeek => CultureInfo.CurrentCulture.TextInfo.ToTitleCase(DateTime.ToString("dddd", new CultureInfo("ru-RU")));

        // Информация о солнце
        [JsonProperty("sys")]
        public Sys SysInfo { get; set; }

        // Название города (если не удалось определить - пустая строка)
        [JsonProperty("name")]
        public string Name { get; set; }

        // Класс для хранения данных о погодных условиях
        public class Weather
        {
            // Описание погодных условий
            private string description;
            [JsonProperty("description")]
            public string Description
            {
                get { return description; }
                set { description = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(value); }
            }

            // Код иконки
            [JsonProperty("icon")]
            public string IconCode { get; set; }

            // Ссылка на изображение иконки
            [JsonIgnore]
            public string IconUrl => $"http://openweathermap.org/img/w/{IconCode}.png";

            // Изображение иконки
            [JsonIgnore]
            public BitmapImage IconImage => GetIconImage();

            private BitmapImage GetIconImage()
            {
                try
                {
                    using (var client = new WebClient())
                    {
                        var data = client.DownloadData(IconUrl);
                        using (var stream = new MemoryStream(data))
                        {
                            var bitmapImage = new BitmapImage();
                            bitmapImage.BeginInit();
                            bitmapImage.StreamSource = stream;
                            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                            bitmapImage.EndInit();
                            bitmapImage.Freeze(); // Чтобы можно было использовать в другом потоке
                            return bitmapImage;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error downloading icon: {ex.Message}");
                    return null;
                }
            }
        }

        // Класс для хранения основных параметров
        public class Main
        {
            // Температура (в градусах Цельсия)
            [JsonProperty("temp")]
            public double Temperature { get; set; } = double.NaN;

            [JsonIgnore]
            public string TemperatureString => Temperature.ToString("0.#", CultureInfo.InvariantCulture) + "°";

            // Температура по ощущениям (в градусах Цельсия)
            [JsonProperty("feels_like")]
            public double FeelsLikeTemperature { get; set; } = double.NaN;

            [JsonIgnore]
            public string FeelsLikeTemperatureString => FeelsLikeTemperature.ToString("0.#", CultureInfo.InvariantCulture) + "°";

            // Минимальная температура (в градусах Цельсия)
            [JsonProperty("temp_min")]
            public double MinTemperature { get; set; } = double.NaN;

            [JsonIgnore]
            public string MinTemperatureString => MinTemperature.ToString("0.#", CultureInfo.InvariantCulture) + "°";

            // Максимальная температура (в градусах Цельсия)
            [JsonProperty("temp_max")]
            public double MaxTemperature { get; set; } = double.NaN;

            [JsonIgnore]
            public string MaxTemperatureString => MaxTemperature.ToString("0.#", CultureInfo.InvariantCulture) + "°";

            // Влажность (в процентах)
            [JsonProperty("humidity")]
            public int Humidity { get; set; }

            // Давление (в гектопаскалях)
            [JsonProperty("pressure")]
            public int Pressure { get; set; }
        }

        // Класс для хранения данных о ветре
        public class Wind
        {
            // Скорость ветра (в метрах в секунду)
            [JsonProperty("speed")]
            public double Speed { get; set; }

            [JsonIgnore]
            public string SpeedString => Speed.ToString("0.#", CultureInfo.InvariantCulture);

        }

        // Класс для хранения информации о солнце
        public class Sys
        {
            // Время восхода (Unix, UTC)
            [JsonProperty("sunrise")]
            public long Sunrise { get; set; }

            // Время заката (Unix, UTC)
            [JsonProperty("sunset")]
            public long Sunset { get; set; }
        }
    }
}

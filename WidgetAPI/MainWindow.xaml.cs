using API_Library;
using System.Text;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace WidgetAPI
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void Button_ClickAsync(object sender, RoutedEventArgs e)
        {
            // Создание экземпляра класса Weather
            var weather = new Weather();

            // Получение текущей погоды
            var currentWeather = await weather.GetCurrentWeatherAsync();

            // Получение описания текущей погоды
            TbCurrentWeather.Text = currentWeather.WeatherInfo[0].Description;
        }

        private async void Button2_ClickAsync(object sender, RoutedEventArgs e)
        {
            // Создание экземпляра класса Weather
            var weather = new Weather();

            // Получение текущей погоды
            var currentWeather = await weather.GetCurrentWeatherAsync();

            // Получение описания текущей погоды
            var weatherDescription = currentWeather.WeatherInfo[0].Description;

            // Получение погоды на неделю
            var weeklyWeather = await weather.GetWeeklyTemperatureAsync();

            // Получение списка погоды на каждый день недели
            var dailyWeatherList = weeklyWeather.DailyWeatherList;

            // Вывод погоды на каждый день недели
            var stringBuilder = new StringBuilder();
            foreach (var dailyWeather in dailyWeatherList)
            {
                stringBuilder.AppendLine($"День недели: {dailyWeather.DayOfWeek}");
                stringBuilder.AppendLine($"Дата и время: {dailyWeather.DateTime}");
                stringBuilder.AppendLine($"Описание погоды: {dailyWeather.WeatherInfo[0].Description}");
                stringBuilder.AppendLine($"Температура: {dailyWeather.MainInfo.TemperatureString}");
                stringBuilder.AppendLine($"Ночная температура: {dailyWeather.MainInfo.MinTemperatureString}");
                stringBuilder.AppendLine($"Ощущается как: {dailyWeather.MainInfo.FeelsLikeTemperatureString}");
                stringBuilder.AppendLine($"Скорость ветра: {dailyWeather.WindInfo.SpeedString} м/с");
                stringBuilder.AppendLine();
            }

            TbWeeklyTemperature.Text = stringBuilder.ToString();

            var bitmapImage = currentWeather.WeatherInfo[0].IconImage;
            ImgBox.Source = bitmapImage;
        }

    }
}


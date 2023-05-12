using API_Library;
using System;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Input;
using System.IO;
using System.Linq;
using System.Resources;
using WidgetAPI.Properties;

namespace WidgetAPI
{
    /// <summary>
    /// Логика взаимодействия для WeatherWidget.xaml
    /// </summary>
    public partial class WeatherWidget : Window
    {
        private Timer _timer;
        WeatherAPILogic weather;
        string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "LogPas.txt");
        string userName;
        string password;
        User user;
        CurrentWeather resultCurrent;
        ThreeHoursWeather resultWeekly;

        public WeatherWidget()
        {
            InitializeComponent();
            GetUser();

            // Создаем таймер и настраиваем его
            _timer = new Timer(30 * 60 * 1000);
            _timer.Elapsed += new ElapsedEventHandler(Timer_Elapsed);
            _timer.AutoReset = true;
            _timer.Enabled = true;
        }


        private async void GetUser()
        {
            userName = File.ReadAllLines(path)[0];
            password = File.ReadAllLines(path)[1];
            user = await MongoDbClient.GetUserAsync(userName);
            if (user.Password != password) Error();                
            weather = new WeatherAPILogic(country: user.Country, city: user.City, units: user.Units.Split(' ')[0].ToLower());

            await UpdateDataFromApi();
        }

        private void Error()
        {
            MessageBox.Show("Неверный пароль в кэше, перезайдите в систему", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);

            File.WriteAllText(path, string.Empty);
            var mainWindow = new MainWindow();
            mainWindow.Show();
            Close();
        }

        private async void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            // Обновляем данные из API
            await UpdateDataFromApi();
        }

        private async Task UpdateDataFromApi()
        {
            // Здесь вызываем методы для получения данных из API
            // и обновляем соответствующие элементы интерфейса
            
            resultCurrent = await weather.GetCurrentWeatherAsync();
            resultWeekly = await weather.GetWeeklyWeatherAsync();

            if (user.Units.Split(' ')[0] == "Metric")
            {
                Resources["TempMM"] = "°C";
                WindMM_TextBlock.Text = "м/с";
            }
                
            else if (user.Units.Split(' ')[0] == "Standard")
            {
                Resources["TempMM"] = "K";
                WindMM_TextBlock.Text = "м/с";
                Win.Width = 475;
            }

            else if (user.Units.Split(' ')[0] == "Imperial")
            {
                Resources["TempMM"] = "°F";
                WindMM_TextBlock.Text = "миль/ч";
            }

            Dispatcher.Invoke(() =>
            {
                Temperature_TextBlock.Text = resultCurrent.MainInfo.TemperatureString.ToString();
                FeelsLikeTemperature_TextBlock.Text = resultCurrent.MainInfo.FeelsLikeTemperatureString.ToString();
                MinTemperature_TextBlock.Text = resultCurrent.MainInfo.MinTemperatureString.ToString();
                MaxTemperature_TextBlock.Text = resultCurrent.MainInfo.MaxTemperatureString.ToString();
                Description_TextBlock.Text = resultCurrent.WeatherInfo[0].Description.ToString();
                Icon_Image.Source = resultCurrent.WeatherInfo[0].IconImage;
                City_TextBlock.Text = user.City.ToString();
                Sunrise_TextBlock.Text = resultCurrent.SysInfo.SunriseDateTime.ToString(@"hh\:mm"); ;
                Sunset_TextBlock.Text = resultCurrent.SysInfo.SunsetDateTime.ToString(@"hh\:mm");
                Dt_TextBlock.Text = resultCurrent.DateTime.ToString(@"hh\:mm");
                Speed_TextBlock.Text = resultCurrent.WindInfo.SpeedString.ToString();
                Humidity_TextBlock.Text = resultCurrent.MainInfo.Humidity.ToString();
                Visibility_TextBlock.Text = resultCurrent.VisibilityString.ToString();
                Pressure_TextBlock.Text = resultCurrent.MainInfo.Pressure.ToString();

                Temperature1_TextBlock.Text = resultWeekly.DailyWeatherList[0].MainInfo.TemperatureString.ToString();
                Icon1_Image.Source = resultWeekly.DailyWeatherList[0].WeatherInfo[0].IconImage;
                Time1_TextBlock.Text = resultWeekly.DailyWeatherList[0].DateTime.ToString(@"hh\:mm");

                Temperature2_TextBlock.Text = resultWeekly.DailyWeatherList[1].MainInfo.TemperatureString.ToString();
                Icon2_Image.Source = resultWeekly.DailyWeatherList[1].WeatherInfo[0].IconImage;
                Time2_TextBlock.Text = resultWeekly.DailyWeatherList[1].DateTime.ToString(@"hh\:mm");

                Temperature3_TextBlock.Text = resultWeekly.DailyWeatherList[2].MainInfo.TemperatureString.ToString();
                Icon3_Image.Source = resultWeekly.DailyWeatherList[2].WeatherInfo[0].IconImage;
                Time3_TextBlock.Text = resultWeekly.DailyWeatherList[2].DateTime.ToString(@"hh\:mm");

                Temperature4_TextBlock.Text = resultWeekly.DailyWeatherList[3].MainInfo.TemperatureString.ToString();
                Icon4_Image.Source = resultWeekly.DailyWeatherList[3].WeatherInfo[0].IconImage;
                Time4_TextBlock.Text = resultWeekly.DailyWeatherList[3].DateTime.ToString(@"hh\:mm");

                Temperature5_TextBlock.Text = resultWeekly.DailyWeatherList[4].MainInfo.TemperatureString.ToString();
                Icon5_Image.Source = resultWeekly.DailyWeatherList[4].WeatherInfo[0].IconImage;
                Time5_TextBlock.Text = resultWeekly.DailyWeatherList[4].DateTime.ToString(@"hh\:mm");
            });
        }


        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Close();
        }

        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private async void Refrash_Image_MouseLeftButtonDownAsync(object sender, MouseButtonEventArgs e)
        {
            await UpdateDataFromApi();
        }
    }
}

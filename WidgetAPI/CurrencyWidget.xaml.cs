using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Input;
using API_Library;

namespace WidgetAPI
{
    /// <summary>
    /// Логика взаимодействия для CurrencyWidget.xaml
    /// </summary>
    public partial class CurrencyWidget : Window
    {
        private Timer _timer;
        User user;
        AlphaVantageResponse currency;

        string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "LogPas.txt");
        string userName;
        string password;

        public CurrencyWidget()
        {
            InitializeComponent();

            GetUser();

            _timer = new Timer();
            _timer.Interval = TimeSpan.FromSeconds(5 * 60).TotalMilliseconds;
            _timer.Elapsed += OnTimerElapsed;
            _timer.Start();

        }

        private async void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            await UpdateDataFromApi();
        }


        private async void GetUser()
        {
            var lines = File.ReadAllLines(path);
            userName = lines[0];
            password = lines[1];
            user = await MongoDbClient.GetUserAsync(userName);
            if (user.Password != password) Error();
            await UpdateDataFromApi();
        }
        private async Task UpdateDataFromApi()
        {
            abc.Text = "Обновляется...";
            currency = await new CurrencyAPILogic().GetExchangeRateAsync(user.From_CurrencyCode, user.To_CurrencyCode);

            Dispatcher.Invoke(() =>
            {
                From_CurrencyCode_TextBlock.Text = currency.ExchangeRate.FromCurrencyCode;
                To_CurrencyCode_TextBlock.Text = currency.ExchangeRate.ToCurrencyCode;
                From_CurrencyName_TextBlock.Text = $"({currency.ExchangeRate.FromCurrencyName})";
                To_CurrencyName_TextBlock.Text = $"({currency.ExchangeRate.ToCurrencyName})";
                Rate_TextBlock.Text = currency.ExchangeRate.RateString;
                BidPrice_TextBlock.Text = currency.ExchangeRate.BidPriceString;
                AskPrice_TextBlock.Text = currency.ExchangeRate.AskPriceString;
                LastRefreshed_TextBlock.Text = currency.ExchangeRate.LastRefreshed.ToString("HH\\:mm\\:ss");
            });
            abc.Text = "Обновление в";
        }

        private void Error()
        {
            System.Windows.MessageBox.Show("Неверный пароль в кэше, перезайдите в систему", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);

            File.WriteAllText(path, string.Empty);
            var mainWindow = new MainWindow();
            mainWindow.Show();
            Close();
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

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.FadeIn();

            WindowInteropHelper wndHelper = new WindowInteropHelper(this);
            int exStyle = (int)GetWindowLong(wndHelper.Handle, (int)GetWindowLongFields.GWL_EXSTYLE);
            exStyle |= (int)ExtendedWindowStyles.WS_EX_TOOLWINDOW;
            SetWindowLong(wndHelper.Handle, (int)GetWindowLongFields.GWL_EXSTYLE, (IntPtr)exStyle);

            // Чтение сохраненных значений положения окна из настроек приложения
            double left = Properties.Settings.Default.MainWindowLeftCurrency;
            double top = Properties.Settings.Default.MainWindowTopCurrency;

            // Установка положения окна
            if (left >= 0 && top >= 0 && left + Width <= SystemParameters.VirtualScreenWidth && top + Height <= SystemParameters.VirtualScreenHeight)
            {
                Left = left;
                Top = top;
            }
        }
        private async void Image_MouseLeftButtonDownAsync(object sender, MouseButtonEventArgs e)
        {
            await AnimationHelper.FadeOut2Async(this);

            // Сохранение положения окна в настройки приложения
            Properties.Settings.Default.MainWindowLeftCurrency = Left;
            Properties.Settings.Default.MainWindowTopCurrency = Top;
            Properties.Settings.Default.Save();


            Close();
        }

        #region Window styles
        [Flags]
        public enum ExtendedWindowStyles
        {
            // ...
            WS_EX_TOOLWINDOW = 0x00000080,
            // ...
        }

        public enum GetWindowLongFields
        {
            // ...
            GWL_EXSTYLE = (-20),
            // ...
        }

        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowLong(IntPtr hWnd, int nIndex);

        public static IntPtr SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong)
        {
            int error = 0;
            IntPtr result = IntPtr.Zero;
            // Win32 SetWindowLong doesn't clear error on success
            SetLastError(0);

            if (IntPtr.Size == 4)
            {
                // use SetWindowLong
                Int32 tempResult = IntSetWindowLong(hWnd, nIndex, IntPtrToInt32(dwNewLong));
                error = Marshal.GetLastWin32Error();
                result = new IntPtr(tempResult);
            }
            else
            {
                // use SetWindowLongPtr
                result = IntSetWindowLongPtr(hWnd, nIndex, dwNewLong);
                error = Marshal.GetLastWin32Error();
            }

            if ((result == IntPtr.Zero) && (error != 0))
            {
                throw new System.ComponentModel.Win32Exception(error);
            }

            return result;
        }

        [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr", SetLastError = true)]
        private static extern IntPtr IntSetWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        [DllImport("user32.dll", EntryPoint = "SetWindowLong", SetLastError = true)]
        private static extern Int32 IntSetWindowLong(IntPtr hWnd, int nIndex, Int32 dwNewLong);

        private static int IntPtrToInt32(IntPtr intPtr)
        {
            return unchecked((int)intPtr.ToInt64());
        }

        [DllImport("kernel32.dll", EntryPoint = "SetLastError")]
        public static extern void SetLastError(int dwErrorCode);
        #endregion
    }
}

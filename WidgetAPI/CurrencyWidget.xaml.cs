using System;
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

        public CurrencyWidget()
        {
            InitializeComponent();

            GetUser();

            // Создание таймера обновления
            _timer = new Timer();
            _timer.Interval = TimeSpan.FromSeconds(5 * 60).TotalMilliseconds;
            _timer.Elapsed += OnTimerElapsed;
            _timer.Start();
        }

        private async void OnTimerElapsed(object sender, ElapsedEventArgs e) // Обновление по таймеру
        {
            await UpdateDataFromApi();
        }

        private async void GetUser() // Получение конфигурации пользователя
        {
            var lines = File.ReadAllLines(path);
            userName = lines[0];
            user = await MongoDbClient.GetUserAsync(userName);
            await UpdateDataFromApi();
        }
        private async Task UpdateDataFromApi() // Обновление данных
        {
            abc.Text = "Обновляется...";
            try
            {
                currency = await new CurrencyAPILogic().GetExchangeRateAsync(user.From_CurrencyCode, user.To_CurrencyCode);

                if (currency.ExchangeRate != null)
                {
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
                }

                else
                {
                    Error();
                }
            }

            catch (Exception)
            {

                Error();
            }
            abc.Text = "Обновление в";
        }

        private void Error() // Отображение ошибки
        {
            System.Windows.MessageBox.Show("Ошибка со стороны сервиса API, либо неверно введённые пользовательские настройки. " +
                "Проверьте настройки и перезапустите виджет", "Ошибка | Currency", MessageBoxButton.OK, MessageBoxImage.Error);
            Close();
        }

        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e) // Перемещение окна
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private async void Refrash_Image_MouseLeftButtonDownAsync(object sender, MouseButtonEventArgs e) // Обновление данных по кнопке
        {
            await UpdateDataFromApi();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e) // После отрисовки окна
        {
            this.FadeIn();

            // Удаление окна из Alt+Tab
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
        private async void Image_MouseLeftButtonDownAsync(object sender, MouseButtonEventArgs e) // Закрытие окна по кнопке
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

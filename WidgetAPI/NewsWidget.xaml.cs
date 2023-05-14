using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Timers;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using API_Library;

namespace WidgetAPI
{
	/// <summary>
	/// Логика взаимодействия для NewsWidget.xaml
	/// </summary>
	public partial class NewsWidget : Window
	{
		private Timer _timer;
		User user;
		List<News> news;
		List<BitmapImage> imageList;

		string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "LogPas.txt");
		string userName;
		string password;

		int count;
		int active;
		public NewsWidget()
		{
			InitializeComponent();

            // Создание таймера и настройка его интервала
            _timer = new Timer(60 * 60 * 1000);
            // Регистрация обработчика события Elapsed
            _timer.Elapsed += Timer_Elapsed;
            // Запуск таймера
            _timer.Start();

            GetUser();

			// Инициализация imageList
			imageList = new List<BitmapImage>();

		}

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            // Вызов метода обновления данных из API
            Dispatcher.Invoke(() =>
            {
				UpdateDataFromApi();
            });
        }


        private async void GetUser()
		{
			var lines = File.ReadAllLines(path);
			userName = lines[0];
			password = lines[1];
			user = await MongoDbClient.GetUserAsync(userName);
			if (user.Password != password) Error();
			UpdateDataFromApi();	
		}

		private async void UpdateDataFromApi()
		{
			news = await new NewsAPILogic(countryAbr: user.CountryAbr).GetNewsAsync();

			// Инициализация imageList
			imageList = new List<BitmapImage>(news.Count);

			// Предзагрузка изображений в отдельном потоке
			foreach (var n in news)
			{
			    var image = new BitmapImage();
				image.BeginInit();
				image.UriSource = new Uri(n.UrlToImage, UriKind.Absolute);
				image.CacheOption = BitmapCacheOption.OnLoad;
				image.EndInit();

				imageList.Add(image);
			}

			// Загрузка первого изображения
			ImgTitile_Image.Source = imageList[0];

			count = news.Count;
			active = 0;
			UpdateData(0);
		}

		private void UpdateData(int active)
		{
			Progress_TextBlock.Text = $"{active + 1} / {count}";
			ImgTitile_Image.Source = imageList[active]; // Использование предзагруженного изображения

			Title_TextBlock.Text = news[active].Title;
			Description_TextBlock.Text = news[active].Description;
			AutorUrl_Hyperlink.NavigateUri = new Uri(news[active].Url);
			Autor_TextBlock.Text = news[active].Author;
			PublishedAt_TextBlock.Text = news[active].PublishedAt.ToString();
		}
		private void Error()	
		{
			MessageBox.Show("Неверный пароль в кэше, перезайдите в систему", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);

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

		private void Forward_Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (active == count - 1)
				UpdateData(active = 0);

			else
				UpdateData(++active);
		}

		private void Back_Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (active != 0)
				UpdateData(--active);

			else
				UpdateData(active = count - 1);
		}

        private void AutorUrl_Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.FadeIn();

            WindowInteropHelper wndHelper = new WindowInteropHelper(this);
            int exStyle = (int)GetWindowLong(wndHelper.Handle, (int)GetWindowLongFields.GWL_EXSTYLE);
            exStyle |= (int)ExtendedWindowStyles.WS_EX_TOOLWINDOW;
            SetWindowLong(wndHelper.Handle, (int)GetWindowLongFields.GWL_EXSTYLE, (IntPtr)exStyle);

            // Чтение сохраненных значений положения окна из настроек приложения
            double left = Properties.Settings.Default.MainWindowLeftNews;
            double top = Properties.Settings.Default.MainWindowTopNews;

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
            Properties.Settings.Default.MainWindowLeftNews = Left;
            Properties.Settings.Default.MainWindowTopNews = Top;
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

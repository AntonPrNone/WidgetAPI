using System;
using System.Collections.Generic;
using System.IO;
using System.Timers;
using System.Windows;
using System.Windows.Input;
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
	}
}

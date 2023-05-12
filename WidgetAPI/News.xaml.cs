using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using API_Library;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
using static API_Library.CurrentWeather;

namespace WidgetAPI
{
	/// <summary>
	/// Логика взаимодействия для NewsWin.xaml
	/// </summary>
	public partial class NewsWin : Window
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
		public NewsWin()
		{
			InitializeComponent();

			GetUser();

            // Инициализация imageList
            imageList = new List<BitmapImage>();

        }

		private async void GetUser()
		{
			userName = File.ReadAllLines(path)[0];
			password = File.ReadAllLines(path)[1];
			user = await MongoDbClient.GetUserAsync(userName);
			if (user.Password != password) Error();
			UpdateDataFromApi();
		}

		private async void UpdateDataFromApi()
		{
            news = await new NewsAPILogic(countryAbr: user.CountryAbr).GetNewsAsync();

            // Предзагрузка изображений
            foreach (var n in news)
            {
                var image = new BitmapImage();
                image.BeginInit();
                image.UriSource = new Uri(n.UrlToImage, UriKind.Absolute);
                image.CacheOption = BitmapCacheOption.OnLoad; // Установка CacheOption
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

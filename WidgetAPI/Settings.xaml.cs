using System;
using System.Windows;
using System.Windows.Input;
using System.IO;
using System.Windows.Controls;
using API_Library;
using System.Windows.Forms;
using System.Linq;
using System.Threading.Tasks;

namespace WidgetAPI
{
	/// <summary>
	/// Логика взаимодействия для Settings.xaml
	/// </summary>
	public partial class Settings : Window
	{
		System.Windows.Controls.CheckBox[] checki;
		string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "LogPas.txt");
		private NotifyIcon _notifyIcon;
		User user;

		public Settings()
		{
			InitializeComponent();

			// Доблавении значка в трею
			_notifyIcon = new NotifyIcon();
			_notifyIcon.Icon = new System.Drawing.Icon("./img/ico.ico");
			_notifyIcon.Visible = true;
			System.Windows.Forms.ContextMenu contextMenu = new System.Windows.Forms.ContextMenu();
			contextMenu.MenuItems.Add("Закрыть", new EventHandler(CloseMenuItem_ClickAsync));
			_notifyIcon.ContextMenu = contextMenu;
			_notifyIcon.Click += new EventHandler(NotifyIcon_ClickAsync);

			checki = new System.Windows.Controls.CheckBox[] { checkBoxCats, checkBoxWeather, checkBoxCurrency, checkBoxNews };
			string[] fileContents = File.ReadAllLines(path);
			FillingSettings(fileContents);
		}

		private void NotifyIcon_ClickAsync(object sender, EventArgs e) // Скрытие и отображении окна при нажатии на значок в трее
		{
			if (!this.IsVisible)
			{
				Show();
				Topmost = true;
				Topmost = false;
			}

			else
			{
				Hide();
			}
		}

		private async void FillingSettings(string[] fileContents) // Расставление настроек пользователя
		{
			try
			{
				user = await MongoDbClient.GetUserAsync(fileContents[0]);
			}

			// Обрабатываем исключение, если не будет получен ответ от сервера в течении 30 сек
			catch (Exception)
			{
				System.Windows.MessageBox.Show("Проверьте статус сервера БД", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
				System.Windows.Application.Current.Shutdown();
			}

			UserName_Label.Content = user.Login;
			if (user != null)
			{
				string selectedValue = user.Units;
				foreach (ComboBoxItem item in Units_ComboBox.Items)
				{
					if (item.Content.ToString() == selectedValue)
					{
						Units_ComboBox.SelectedItem = item;
						break;
					}
				}

				Сountry_TextBox.Text = user.Country;
				City_TextBox.Text = user.City;
				if (user.CountryAbr != null && user.CountryAbr != "") CountryAbr_TextBox.Text = user.CountryAbr;
				if (user.From_CurrencyCode != null && user.From_CurrencyCode != "") From_Currency_Code_TextBox.Text = user.From_CurrencyCode;
				if (user.To_CurrencyCode != null && user.To_CurrencyCode != "") To_Currency_Code_TextBox.Text = user.To_CurrencyCode;

				foreach (var widget in checki)
				{
					if (user.WidgetsOfInterest.ContainsKey(widget.Name.Replace("checkBox", "")) && user.WidgetsOfInterest[widget.Name.Replace("checkBox", "")])
					{
						widget.IsChecked = true;
					}
					else
					{
						widget.IsChecked = false;
					}
				}
			}

			else
			{
				System.Windows.MessageBox.Show("Ошибка при получении данных, выполните повторную авторизацию", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
				System.Windows.Application.Current.Shutdown();
			}
		}

		private async void Save_Button_ClickAsync(object sender, RoutedEventArgs e) // Сохранение настроек, скрытие окна настроек и отображение виджетов
		{
			string[] fileContents = File.ReadAllLines(path);
			user.Login = fileContents[0];
			user.Password = fileContents[1];

			// Заполнение объекта User данными из элементов управления
			user.Units = ((ContentControl)Units_ComboBox.SelectedItem).Content != null ? ((ContentControl)Units_ComboBox.SelectedItem).Content.ToString() : "";
			user.Country = Сountry_TextBox.Text;
			user.City = City_TextBox.Text;
			user.CountryAbr = CountryAbr_TextBox.Text;
			user.From_CurrencyCode = From_Currency_Code_TextBox.Text;
			user.To_CurrencyCode = To_Currency_Code_TextBox.Text;

			foreach (var widget in checki)
			{
				user.WidgetsOfInterest[widget.Name.Replace("checkBox", "")] = widget.IsChecked.Value;
			}

			try
			{
				await MongoDbClient.ReplaceUserAsync(user);
			}

			// Обрабатываем исключение, если не будет получен ответ от сервера в течении 30 сек
			catch (Exception)
			{
				System.Windows.MessageBox.Show("Проверьте статус сервера БД", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
				System.Windows.Application.Current.Shutdown();
			}
			
			// Открытие необходимых виджетов, проверка на запущенные экземпляры ---------------------------------------------------------
			if ((bool)checkBoxCats.IsChecked)
			{
				var wins = System.Windows.Application.Current.Windows;
				var first = wins.OfType<Window>().FirstOrDefault(w => w.Title == "CatWidget" && w.Visibility == Visibility.Visible);

				if (first == null)
				{
					CatWidget catWidget = new CatWidget();
					catWidget.Show();
				}

				else
				{
					Properties.Settings.Default.MainWindowLeftCat = first.Left;
					Properties.Settings.Default.MainWindowTopCat = first.Top;
					Properties.Settings.Default.Save();
					first.Close();
					CatWidget catWidget = new CatWidget();
					catWidget.Show();
				}
			}

			if ((bool)checkBoxWeather.IsChecked)
			{
				var wins = System.Windows.Application.Current.Windows;
				var first = wins.OfType<Window>().FirstOrDefault(w => w.Title == "WeatherWidget" && w.Visibility == Visibility.Visible);

				if (first == null)
				{
					WeatherWidget weatherWidget = new WeatherWidget();
					weatherWidget.Show();
				}

				else
				{
					Properties.Settings.Default.MainWindowLeftWeather = first.Left;
					Properties.Settings.Default.MainWindowTopWeather = first.Top;
					Properties.Settings.Default.Save();
					first.Close();
					WeatherWidget weatherWidget = new WeatherWidget();
					weatherWidget.Show();
				}
			}

			if ((bool)checkBoxCurrency.IsChecked)
			{
				var wins = System.Windows.Application.Current.Windows;
				var first = wins.OfType<Window>().FirstOrDefault(w => w.Title == "CurrencyWidget" && w.Visibility == Visibility.Visible);

				if (first == null)
				{
					CurrencyWidget currencyWidget = new CurrencyWidget();
					currencyWidget.Show();
				}

				else
				{
					Properties.Settings.Default.MainWindowLeftCurrency = first.Left;
					Properties.Settings.Default.MainWindowTopCurrency = first.Top;
					Properties.Settings.Default.Save();
					first.Close();
					CurrencyWidget currencyWidget = new CurrencyWidget();
					currencyWidget.Show();
				}
			}

			if ((bool)checkBoxNews.IsChecked)
			{
				var wins = System.Windows.Application.Current.Windows;
				var first = wins.OfType<Window>().FirstOrDefault(w => w.Title == "NewsWidget" && w.Visibility == Visibility.Visible);

				if (first == null)
				{
					NewsWidget newsWidget = new NewsWidget();
					newsWidget.Show();
				}

				else
				{
					Properties.Settings.Default.MainWindowLeftNews = first.Left;
					Properties.Settings.Default.MainWindowTopNews = first.Top;
					Properties.Settings.Default.Save();
					first.Close();
					NewsWidget newsWidget = new NewsWidget();
					newsWidget.Show();
				}
			}

			// -------------------------------------------------------------------------------------------------------------------------

			this.Hide();
		}

		private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)  // Перемещение окна
		{
			if (e.LeftButton == MouseButtonState.Pressed)
			{
				DragMove();
			}
		}

		private void Window_Loaded(object sender, RoutedEventArgs e) // После отрисовки окна
		{
			this.FadeIn();
		}
		private async void Image_MouseLeftButtonDownAsync(object sender, MouseButtonEventArgs e)  // Закрытие окна по кнопке
		{
			await AnimationHelper.FadeOut2Async(this);
			Close();
		}
		private async void CloseMenuItem_ClickAsync(object sender, EventArgs e) // Закрытие всей программы из контекстного меню
		{
            // Получение открытых виджетов и их закрытие
            var wins = System.Windows.Application.Current.Windows;
            var first1 = wins.OfType<Window>().FirstOrDefault(w => w.Title == "CatWidget" && w.Visibility == Visibility.Visible);
            var first2 = wins.OfType<Window>().FirstOrDefault(w => w.Title == "WeatherWidget" && w.Visibility == Visibility.Visible);
            var first3 = wins.OfType<Window>().FirstOrDefault(w => w.Title == "CurrencyWidget" && w.Visibility == Visibility.Visible);
            var first4 = wins.OfType<Window>().FirstOrDefault(w => w.Title == "NewsWidget" && w.Visibility == Visibility.Visible);

            if (first1 != null)
            {
                await AnimationHelper.FadeOut2Async(first1);
                first1.Close();
            }

            if (first2 != null)
            {
                await AnimationHelper.FadeOut2Async(first2);
                first2.Close();
            }

            if (first3 != null)
            {
                await AnimationHelper.FadeOut2Async(first3);
                first3.Close();
            }

            if (first4 != null)
            {
                await AnimationHelper.FadeOut2Async(first4);
                first4.Close();
            }

            this.Close();

			// Закрываем приложение
			System.Windows.Application.Current.Shutdown();
		}

        private async void Image_MouseLeftButtonDown_1(object sender, MouseButtonEventArgs e) // Выход из профиля
		{
			// Получение открытых виджетов и их закрытие
			var wins = System.Windows.Application.Current.Windows;
			var first1 = wins.OfType<Window>().FirstOrDefault(w => w.Title == "CatWidget" && w.Visibility == Visibility.Visible);
			var first2 = wins.OfType<Window>().FirstOrDefault(w => w.Title == "WeatherWidget" && w.Visibility == Visibility.Visible);
			var first3 = wins.OfType<Window>().FirstOrDefault(w => w.Title == "CurrencyWidget" && w.Visibility == Visibility.Visible);
			var first4 = wins.OfType<Window>().FirstOrDefault(w => w.Title == "NewsWidget" && w.Visibility == Visibility.Visible);

			if (first1 != null)
			{
				await AnimationHelper.FadeOut2Async(first1);
				first1.Close();
			}

			if (first2 != null)
			{
				await AnimationHelper.FadeOut2Async(first2);
				first2.Close();
			}

			if (first3 != null)
			{
				await AnimationHelper.FadeOut2Async(first3);
				first3.Close();
			}

			if (first4 != null)
			{
				await AnimationHelper.FadeOut2Async(first4);
				first4.Close();
			}

			File.WriteAllText(path, string.Empty);
			MainWindow mainWindow = new MainWindow();
			await AnimationHelper.FadeOut2Async(this);
			mainWindow.Show();
			this.Close();
		}

		private void WinSettings_Closed(object sender, EventArgs e) // Удаление значка перед выходом из программы
		{
			_notifyIcon.Visible = false;
		}
	}
}
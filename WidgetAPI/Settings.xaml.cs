using System;
using System.Windows;
using System.Windows.Input;
using System.IO;
using System.Windows.Controls;
using API_Library;
using System.Windows.Forms;
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

		public Settings()
		{
			InitializeComponent();

            _notifyIcon = new NotifyIcon();
            _notifyIcon.Icon = new System.Drawing.Icon("./img/ico.ico");
            _notifyIcon.Visible = true;
            System.Windows.Forms.ContextMenu contextMenu = new System.Windows.Forms.ContextMenu();
            contextMenu.MenuItems.Add("Закрыть", new EventHandler(CloseMenuItem_Click));
            _notifyIcon.ContextMenu = contextMenu;
            _notifyIcon.Click += new EventHandler(NotifyIcon_ClickAsync);

            checki = new System.Windows.Controls.CheckBox[] { checkBoxCats, checkBoxWeather, checkBoxCurrency, checkBoxNews };
			string[] fileContents = File.ReadAllLines(path);
            RestorationBuildings(fileContents);
        }

        private void CloseMenuItem_Click(object sender, EventArgs e)
        {
            _notifyIcon.Visible = false;
            // Закрываем приложение
            System.Windows.Application.Current.Shutdown();
        }

        private void NotifyIcon_ClickAsync(object sender, EventArgs e)
		{
			if (!this.IsVisible)
			{
				// Отображаем окно при клике на значок в трее
				Show();
                Topmost = true;
				Topmost = false;
			}

			else
			{
				Hide();
            }
		}

		protected override void OnStateChanged(EventArgs e)
		{
			base.OnStateChanged(e);

			Console.WriteLine("Window state changed: " + WindowState);

			// Скрываем окно при сворачивании
			if (WindowState == WindowState.Minimized)
			{
				Console.WriteLine("Hiding window");
				Hide();
			}
		}

		private async void RestorationBuildings(string[] fileContents)
		{
			var user = await MongoDbClient.GetUserAsync(fileContents[0]);
			UserName_Label.Content = user.Login;
			if (user != null)
			{
				// установить значения в текстбоксы
				string selectedValue = user.Units; // значение, которое нужно выбрать
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

				// использовать данные пользователя для настройки виджетов
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
		}

		private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed)
			{
				DragMove();
			}
		}

		private async void Save_Button_ClickAsync(object sender, RoutedEventArgs e)
		{
			// сохранить настройки виджетов
			User user = new User();

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

			await MongoDbClient.ReplaceUserAsync(user);

			if ((bool)checkBoxCats.IsChecked)
			{
				CatWidget catWidget = new CatWidget();
				catWidget.Show();
			}

			if ((bool)checkBoxWeather.IsChecked)
			{
				WeatherWidget weatherWidget = new WeatherWidget();
				weatherWidget.Show();
			}

			if ((bool)checkBoxCurrency.IsChecked)
			{
				CurrencyWidget currencyWidget = new CurrencyWidget();
				currencyWidget.Show();
			}

			if ((bool)checkBoxNews.IsChecked)
			{
				NewsWidget newsWidget = new NewsWidget();
				newsWidget.Show();
			}
            this.Hide();
		}

		private async void Image_MouseLeftButtonDown_1(object sender, MouseButtonEventArgs e)
		{
			File.WriteAllText(path, string.Empty);
			MainWindow mainWindow = new MainWindow();
            await AnimationHelper.FadeOut2Async(this);
            mainWindow.Show();
			this.Close();
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			this.FadeIn();
        }
		private async void Image_MouseLeftButtonDownAsync(object sender, MouseButtonEventArgs e)
		{
			await AnimationHelper.FadeOut2Async(this);
            _notifyIcon.Visible = false;
            Close();
		}
    }
}


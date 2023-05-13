using System;
using System.Windows;
using System.Windows.Input;
using System.IO;
using System.Windows.Controls;
using System.Threading.Tasks;
using API_Library;

namespace WidgetAPI
{
    /// <summary>
    /// Логика взаимодействия для Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {
        CheckBox[] checki;
        string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "LogPas.txt");

        public Settings()
        {
            InitializeComponent();

            checki = new CheckBox[] { checkBoxTime, checkBoxWeather, checkBoxCurrency, checkBoxNews };

            if (File.Exists(path))
            {
                string[] fileContents = File.ReadAllLines(path);

                if (fileContents.Length >= 2)
                {
                    RestorationBuildings(fileContents);
                }
                else
                {
                    // обработать ситуацию, когда файл не содержит логина и пароля
                }
            }
            else
            {
                // обработать ситуацию, когда файл не существует
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

        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Close();
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

            CatWidget catWidget = new CatWidget();
            WeatherWidget weatherWidget = new WeatherWidget();
            NewsWidget news = new NewsWidget();
            CurrencyWidget currencyWidget = new CurrencyWidget();

            catWidget.Show();
            weatherWidget.Show();
            news.Show();
            currencyWidget.Show();
            this.Close();
        }

        private void Image_MouseLeftButtonDown_1(object sender, MouseButtonEventArgs e)
        {
            File.WriteAllText(path, string.Empty);
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }
    }
}


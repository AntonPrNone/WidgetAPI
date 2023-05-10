using API_Library;
using System.Text;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows.Input;
using System.IO;

namespace WidgetAPI
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public bool LOGReg = true;
        string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "LogPas.txt");
        public MainWindow()
        {
            InitializeComponent();

            if (File.Exists(path))
            {
                string[] fileContents = File.ReadAllLines(path);

                if (fileContents.Length >= 2)
                {
                    Login_TextBox.Text = fileContents[0];
                    Password_TextBox.Password = fileContents[1];
                    Log_Button_ClickAsync(null, null);
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

        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private async void Login_TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(Login_TextBox.Text))
            {
                if (await MongoDbClient.GetUserAsync(Login_TextBox.Text) is null)
                    Dispatcher.Invoke(RegInterface);

                else
                    Dispatcher.Invoke(LogInterface);
            }
        }


        private void RegInterface()
        {
            LOGReg = false;
            RegPasLabel.Visibility = Visibility.Visible;
            Password2_TextBox.Visibility = Visibility.Visible;
            Log_Button.Content = "Зарегистрироваться";
        }

        private void LogInterface()
        {
            LOGReg = true;
            RegPasLabel.Visibility = Visibility.Collapsed;
            Password2_TextBox.Visibility = Visibility.Collapsed;
            Log_Button.Content = "Войти";
        }

        private async void Log_Button_ClickAsync(object sender, RoutedEventArgs e)
        {

            if (await ValidationCheck())
            {
                // Перезаписываем файл, если он уже существует
                using (StreamWriter writer = new StreamWriter(path, false, Encoding.Default))
                {
                    writer.WriteLine(Login_TextBox.Text);
                    writer.Write(Password_TextBox.Password);
                }

                if (!LOGReg) await MongoDbClient.AddUserAsync(new User() { Login = Login_TextBox.Text, Password = Password_TextBox.Password });
                Settings settingsWindow = new Settings();
                settingsWindow.Show();
                this.Close();
            }
        }

        private async Task<bool> ValidationCheck()
        {
            if (!LOGReg)
            {
                if (Password_TextBox.Password == Password2_TextBox.Password)
                {
                    return true;
                }

                else return false;
            }

            else
            {
                var user = await MongoDbClient.GetUserAsync(Login_TextBox.Text);
                if (user != null && Password_TextBox.Password == user.Password)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Close();
        }
    }
}


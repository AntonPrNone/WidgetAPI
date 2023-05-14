using System;
using System.Windows;
using System.IO;

namespace WidgetAPI
{
    /// <summary>
    /// Класс окна автоматической авторизации пользователя
    /// </summary>
    public partial class AutoLog : Window
    {
        // Путь к файлу, содержащему логин и пароль пользователя
        readonly string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "LogPas.txt"); 
        readonly string login;
        readonly string password; 
                                  
        public AutoLog()
        {
            InitializeComponent();
            this.Hide();

            // Если файл существует и содержит не менее двух строк
            if (File.Exists(path) && File.ReadAllLines(path).Length >= 2)
            {
                string[] fileContents = File.ReadAllLines(path);
                login = fileContents[0];
                password = fileContents[1];
                ValidationCheck();
            }

            else
            {
                ShowMainWindow();
                this.Close();
            }
        }

        private async void ValidationCheck() // Метод проверки валидности логина и пароля пользователя
        {
            try
            {
                var user = await MongoDbClient.GetUserAsync(login);

                // Если пользователь существует и пароль совпадает, открываем окно настроек
                if (user != null && password == user.Password)
                {
                    Settings settingsWindow = new Settings();
                    settingsWindow.Show();
                }

                // Иначе открываем окно авторизации
                else
                {
                    ShowMainWindow();
                }

                this.Close();
            }

            // Обрабатываем исключение, возникающее через 30 секунд после отсутствия ответа от сервера
            catch (Exception)
            {
                MessageBox.Show("Проверьте статус сервера БД", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                Application.Current.Shutdown();
            }
        }

        private void ShowMainWindow()
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
        }
    }
}
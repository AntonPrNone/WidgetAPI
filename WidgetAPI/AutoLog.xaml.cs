using System;
using System.Windows;
using System.IO;


namespace WidgetAPI
{
	/// <summary>
	/// Логика взаимодействия для AutoLog.xaml
	/// </summary>
	public partial class AutoLog : Window
	{
		string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "LogPas.txt");
		string string1;
		string string2;
        Settings settingsWindow = new Settings();
		MainWindow mainWindow = new MainWindow();
        public AutoLog()
        {
            InitializeComponent();

            if (File.Exists(path))
            {
                string[] fileContents = File.ReadAllLines(path);
                if (fileContents.Length >= 2)
                {
                    string1 = fileContents[0];
                    string2 = fileContents[1];
                    ValidationCheck();
                }

                else
                {
                    // обработать ситуацию, когда файл не содержит логина и пароля
                    settingsWindow.Close();
                    mainWindow.Show();
                }
            }

            else
            {
                // обработать ситуацию, когда файл не существует
                settingsWindow.Close();
                mainWindow.Show();
            }

            this.Close(); 
        }

        private async void ValidationCheck()
        {
            var user = await MongoDbClient.GetUserAsync(string1);
            if (user != null && string2 == user.Password)
            {
                mainWindow.Close();
                settingsWindow.Show();
            }
            else
            {
                settingsWindow.Close();
                mainWindow.Show();
            }
        }
    }
}

/*
using System;
using System.Windows;
using System.Windows.Input;
using System.IO;


namespace WidgetAPI
{
    /// <summary>
    /// Логика взаимодействия для Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {
        //private NotifyIcon _notifyIcon;

        //[DllImport("user32.dll")]
        //[return: MarshalAs(UnmanagedType.Bool)]
        //static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        //private const uint SWP_NOSIZE = 0x0001;
        //private const uint SWP_NOMOVE = 0x0002;
        //private const uint SWP_NOACTIVATE = 0x0010;
        //private const int WM_SYSCOMMAND = 0x0112;
        //private const int SC_MINIMIZE = 0xF020;
        //private static readonly IntPtr HWND_BOTTOM = new IntPtr(1);

        public Settings()
        {
            InitializeComponent();

            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "LogPas.txt");

            if (File.Exists(path))
            {
                string[] fileContents = File.ReadAllLines(path);
                if (fileContents.Length >= 2)
                {
                    if (MongoDbClient.GetUserAsync(fileContents[0]) != null) { };
                    // использовать содержимое первых двух строк файла
                }
                else
                {
                    // обработать ситуацию, когда файл содержит меньше двух строк
                }
            }
            else
            {
                // обработать ситуацию, когда файл не существует
            }


            _notifyIcon = new NotifyIcon();
            string projectDirectory = Directory.GetParent(Environment.CurrentDirectory).Parent.FullName;
            string imagePath = System.IO.Path.Combine(projectDirectory, "img", "close.ico");
            _notifyIcon.Icon = new System.Drawing.Icon(System.IO.Path.Combine(imagePath)); // Путь к иконке
            _notifyIcon.Visible = true;

            // Создаем элемент контекстного меню
            var closeMenuItem = new ToolStripMenuItem("Закрыть");

            // Добавляем обработчик события Click
            closeMenuItem.Click += (s, e) => Close();

            // Создаем контекстное меню и добавляем в него элемент
            var contextMenu = new ContextMenuStrip();
            contextMenu.Items.Add(closeMenuItem);

            // Устанавливаем контекстное меню для иконки в трее
            _notifyIcon.ContextMenuStrip = contextMenu;

            SetWindowPos(new WindowInteropHelper(this).Handle, HWND_BOTTOM, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_NOACTIVATE);
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

        private void Save_Button_Click(object sender, RoutedEventArgs e)
        {

        }

        //private void WinSettings_StateChanged(object sender, EventArgs e)
        //{
        //    if (this.WindowState == WindowState.Minimized)
        //    {
        //        this.WindowState = WindowState.Normal;
        //    }
        //}
    }
}



// Получаем размеры экрана
                var screenBounds = System.Windows.Forms.Screen.PrimaryScreen.Bounds;

                // Проверяем размеры окна
                if (this.Width == screenBounds.Width && this.Height == screenBounds.Height)
                {
                    // Мы находимся на рабочем столе
					Hide();
                    Topmost = true;
                }
                
				else
				{
					Topmost = false;
				}


*/
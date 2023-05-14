using API_Library;
using Microsoft.Win32;
using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace WidgetAPI
{
    /// <summary>
    /// Логика взаимодействия для CatWidget.xaml
    /// </summary>
    public partial class CatWidget : Window
    {
        string catUri;
        public CatWidget()
        {
            InitializeComponent();

            UpdateDataFromApi();
        }

        private async void UpdateDataFromApi()
        {
            AnimationHelper.FadeOut(Cat_Image);
            catUri = await CatAPILogic.GetRandomCatImageAsync();

            // Создание нового объекта BitmapImage
            BitmapImage bitmap = new BitmapImage();

            // Установка свойства URISource для загрузки изображения из URL
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(catUri, UriKind.Absolute);
            bitmap.EndInit();

            // Установка изображения в элемент управления Image
            Cat_Image.Source = bitmap;

            // Обработчик события DownloadCompleted
            bitmap.DownloadCompleted += (s, e) =>
            {
                // Запускаем анимацию FadeIn после загрузки изображения
                AnimationHelper.FadeIn(Cat_Image);
            };    
        }


        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void Refrash_Image_MouseLeftButtonDownAsync(object sender, MouseButtonEventArgs e)
        {
            UpdateDataFromApi();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.FadeIn();

            WindowInteropHelper wndHelper = new WindowInteropHelper(this);
            int exStyle = (int)GetWindowLong(wndHelper.Handle, (int)GetWindowLongFields.GWL_EXSTYLE);
            exStyle |= (int)ExtendedWindowStyles.WS_EX_TOOLWINDOW;
            SetWindowLong(wndHelper.Handle, (int)GetWindowLongFields.GWL_EXSTYLE, (IntPtr)exStyle);

            // Чтение сохраненных значений положения окна из настроек приложения
            double left = Properties.Settings.Default.MainWindowLeftCat;
            double top = Properties.Settings.Default.MainWindowTopCat;

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
            Properties.Settings.Default.MainWindowLeftCat = Left;
            Properties.Settings.Default.MainWindowTopCat = Top;
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

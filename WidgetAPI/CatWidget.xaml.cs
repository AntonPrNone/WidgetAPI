using API_Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

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
            catUri = await CatAPILogic.GetRandomCatImageAsync();

            // Создание нового объекта BitmapImage
            BitmapImage bitmap = new BitmapImage();

            // Установка свойства URISource для загрузки изображения из URL
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(catUri, UriKind.Absolute);
            bitmap.EndInit();

            // Установка изображения в элемент управления Image
            Cat_Image.Source = bitmap;
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

        private void Refrash_Image_MouseLeftButtonDownAsync(object sender, MouseButtonEventArgs e)
        {
            UpdateDataFromApi();
        }
    }
}

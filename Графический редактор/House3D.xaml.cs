using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;

namespace Графический_редактор
{
    /// <summary>
    /// Логика взаимодействия для House3D.xaml
    /// </summary>
    public partial class House3D : Window
    {
        public House3D()
        {
            InitializeComponent();
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            //camera.Position = new System.Windows.Media.Media3D.Point3D(camera.Position.X, camera.Position.Y, e.NewValue);
            camera.Transform = new TranslateTransform3D(0, 0, e.NewValue);
            //Console.WriteLine(camera.Position.Z);
        }

        private void Slider_ValueChanged_1(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            camera.Transform = new TranslateTransform3D(0, e.NewValue, 0);
        }
    }
}

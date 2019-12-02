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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Controls.Primitives;
using System.IO;

namespace Графический_редактор
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string path = "log.txt";

        public MainWindow()
        {           
            InitializeComponent();               
        }

        private void Log(string eventName)
        {
            using (StreamWriter logger = new StreamWriter(path, true))
            {
                logger.WriteLine(DateTime.Now.ToLongTimeString() + " - " + eventName);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            AddLineWindow newWindow = new AddLineWindow();
            newWindow.ShowDialog();
            if (newWindow.Line != null)
            {
                newWindow.Line.Data = data;
                newWindow.Line.Draw(canvas);
            }            
        }

        private Line CreateLine(double x1, double y1, double x2, double y2, SolidColorBrush brush)
        {
            Line line = new Line();
            line.X1 = x1;
            line.X2 = x2;
            line.Y1 = y1;
            line.Y2 = y2;
            line.Stroke = brush;
            return line;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            MyLine line = new MyLine(1, 1, 1, 0, 10);
            line.Data = data;
            line.Draw(canvas);
            for (int i = 0; i < 11;i++)
            {
                canvas.Children.Add(CreateLine(canvas.ActualWidth / 2 - i*50, 0, canvas.ActualWidth / 2 - i * 50, canvas.ActualHeight, Brushes.Black));
                canvas.Children.Add(CreateLine(canvas.ActualWidth / 2 + i * 50, 0, canvas.ActualWidth / 2 + i * 50, canvas.ActualHeight, Brushes.Black));
                canvas.Children.Add(CreateLine(0, canvas.ActualHeight / 2 - i * 50, canvas.ActualWidth, canvas.ActualHeight / 2 - i * 50, Brushes.Black));
                canvas.Children.Add(CreateLine(0, canvas.ActualHeight / 2 + i * 50, canvas.ActualWidth, canvas.ActualHeight / 2 + i * 50, Brushes.Black));
            }
            canvas.Children.Add(CreateLine(0, canvas.ActualHeight / 2, canvas.ActualWidth, canvas.ActualHeight / 2, Brushes.Red));
            canvas.Children.Add(CreateLine(canvas.ActualWidth / 2, 0, canvas.ActualWidth / 2, canvas.ActualHeight, Brushes.Red));
            canvas.MouseLeftButtonDown += Canvas_MouseLeftButtonDown;
        }

        private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            foreach(var c in canvas.Children)
            {
                var ml = c as MyLine;
                if (ml != null && ml.IsFocused==true)
                {
                    ml.LooseFocus();
                }
            }
            
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            MyLine focused = new MyLine();
            foreach (var c in canvas.Children)
            {
                var ml = c as MyLine;
                if (ml != null && ml.IsFocused == true)
                {
                    focused = ml;
                }
            }
            Matrix matrix = new Matrix(Double.Parse(m11.Text), Double.Parse(m12.Text), Double.Parse(m21.Text), Double.Parse(m22.Text), Double.Parse(m31.Text)*10, Double.Parse(m32.Text)*-10);
            focused.Transform(matrix);
        }
    }
}

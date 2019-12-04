using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Графический_редактор
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string path = "log.txt";

        private List<List<MyLine>> groups;

        private List<SolidColorBrush> colors = new List<SolidColorBrush>() { Brushes.Yellow, Brushes.Blue, Brushes.Red, Brushes.Purple };

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
            groups = new List<List<MyLine>>();

            for (int i = 0; i < 11; i++)
            {
                canvas.Children.Add(CreateLine(canvas.ActualWidth / 2 - i * 50, 0, canvas.ActualWidth / 2 - i * 50, canvas.ActualHeight, Brushes.Black));
                canvas.Children.Add(CreateLine(canvas.ActualWidth / 2 + i * 50, 0, canvas.ActualWidth / 2 + i * 50, canvas.ActualHeight, Brushes.Black));
                canvas.Children.Add(CreateLine(0, canvas.ActualHeight / 2 - i * 50, canvas.ActualWidth, canvas.ActualHeight / 2 - i * 50, Brushes.Black));
                canvas.Children.Add(CreateLine(0, canvas.ActualHeight / 2 + i * 50, canvas.ActualWidth, canvas.ActualHeight / 2 + i * 50, Brushes.Black));
            }
            canvas.Children.Add(CreateLine(0, canvas.ActualHeight / 2, canvas.ActualWidth, canvas.ActualHeight / 2, Brushes.Red));
            canvas.Children.Add(CreateLine(canvas.ActualWidth / 2, 0, canvas.ActualWidth / 2, canvas.ActualHeight, Brushes.Red));
            canvas.MouseLeftButtonDown += Canvas_MouseLeftButtonDown;
            MyLine line = new MyLine((double)1 / 2, (double)-1 / 2, 1, 2, 13);
            MyLine line1 = new MyLine((double)1 / 23, (double)-12 / 115, 1, 13, 1);
            MyLine line2 = new MyLine((double)-3 / 8, (double)-1 / 16, 1, 2, 1);
            line.Data = data;
            line.Draw(canvas);
            line1.Data = data;
            line1.Draw(canvas);
            line2.Data = data;
            line2.Draw(canvas);
        }

        private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            List<MyLine> focused = FindFocused();
            foreach (var ml in focused)
            {
                if (ml.Group == null)
                {
                    ml.IsFocused = false;
                    ml.SetColor(Brushes.Black);
                }
                ml.LooseFocus();
            }
        }

        private List<MyLine> FindFocused()
        {
            List<MyLine> focused = new List<MyLine>();
            foreach (var c in canvas.Children)
            {
                var ml = c as MyLine;
                if (ml != null && ml.IsFocused == true)
                {
                    focused.Add(ml);
                }
            }
            return focused;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            List<MyLine> focused = FindFocused();
            Matrix matrix = new Matrix(Double.Parse(m11.Text), Double.Parse(m12.Text), Double.Parse(m21.Text), Double.Parse(m22.Text), Double.Parse(m31.Text) * 10, Double.Parse(m32.Text) * -10);
            foreach (var ml in focused)
            {
                ml.Transform(matrix);
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            List<MyLine> focused = FindFocused();
            foreach (var ml in focused)
            {
                ml.Remove();
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.OemPlus)
            {
                List<MyLine> focused = FindFocused();
                foreach (var ml in focused)
                {
                    if (ml.Group == null)
                    {
                        ml.Color = colors[groups.Count];
                        ml.LooseFocus();
                        ml.Group = focused;
                    }
                }
                if (focused.Count != 0) groups.Add(focused);
            }
            if (e.Key == Key.Delete)
            {
                List<MyLine> focused = FindFocused();
                foreach (var ml in focused)
                {
                    ml.Remove();
                }
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            List<MyLine> focused = FindFocused();
            double angle = 0;
            if (!double.TryParse(radians.Text, out angle)) { MessageBox.Show("Введено некорректное число"); return; }
            angle *= -Math.PI / 180;
            //MessageBox.Show(angle.ToString());
            Matrix matrix = new Matrix(Math.Cos(angle), Math.Sin(angle), -Math.Sin(angle), Math.Cos(angle), 0, 0);
            foreach (var ml in focused)
            {
                ml.Transform(matrix);
            }
        }
    }
}

using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Collections.Generic;

namespace Графический_редактор
{
    /// <summary>
    /// Логика взаимодействия для RemoveHorizon.xaml
    /// </summary>
    public partial class RemoveHorizon : Window
    {
        public RemoveHorizon()
        {
            InitializeComponent();
        }

        private double yMult = 20;
        private double xMult = 10;
        private double shiftMult = 10;
        private List<List<Point>> points = new List<List<Point>>();

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

        private Line CreateLine(Point p1, Point p2, SolidColorBrush brush)
        {
            Line line = new Line();
            line.X1 = p1.X;
            line.X2 = p2.X;
            line.Y1 = p1.Y;
            line.Y2 = p2.Y;
            line.Stroke = brush;
            return line;
        }

        private Point MakePoint(double x, double shift)
        {
            return new Point(x*xMult + shift*shiftMult, yMult*Math.Cos(x*xMult) * Math.Sin(x * xMult) + canvas.ActualHeight - 50 - shift*shiftMult);
        }

        private void Draw()
        {
            points.Clear();
            for (int shift = 0; shift < 20; shift++)
            {
                points.Add(new List<Point>());
                for (int i = 0; i < canvas.ActualWidth/7; i++)
                {
                    points[shift].Add(MakePoint(i, shift));
                    //Point p2 = MakePoint(i + 1, shift);
                    //canvas.Children.Add(CreateLine(p1, p2, Brushes.Green));
                }
                for (int i = 0; i < points[shift].Count - 1; i++) 
                {
                    if (shift >= 1 && points[shift - 1][i + 1].Y <= points[shift][i].Y) points[shift][i] = new Point(points[shift][i].X, points[shift - 1][i + 1].Y);
                    canvas.Children.Add(CreateLine(points[shift][i], points[shift][i+1], Brushes.Green));
                    if (shift !=0) canvas.Children.Add(CreateLine(points[shift-1][i], points[shift][i], Brushes.Green));
                }
            }
            
            //for (int shift = 0; shift < 20; shift++)
            //{
            //    for (int i = 0; i < canvas.ActualWidth / xMult; i++)
            //    {
            //        double y1 = 0;
            //        double y2 = 0;
            //        if (yMult * Math.Cos((i + shift + 1) * xMult) * Math.Sin((i + shift + 1) * xMult) + canvas.ActualHeight - 50 - shift * shiftMult >= yMult * Math.Cos((i + shift) * xMult) * Math.Sin((i + shift) * xMult) + canvas.ActualHeight - 50 - shift * shiftMult)
            //        {
            //            y1 = yMult * Math.Cos((i + shift) * xMult) * Math.Sin((i + shift) * xMult) + canvas.ActualHeight - 50 - shift * shiftMult;
            //        }
            //        else
            //        {
            //            y1 = yMult * Math.Cos((i + shift + 1) * xMult) * Math.Sin((i + shift + 1) * xMult) + canvas.ActualHeight - 50 - shift * shiftMult;
            //        }
            //        if (yMult * Math.Cos((i + shift + 2) * xMult) * Math.Sin((i + shift + 2) * xMult) + canvas.ActualHeight - 50 - shift * xMult >= yMult * Math.Cos((i + shift + 1) * xMult) * Math.Sin((i + shift + 1) * xMult) + canvas.ActualHeight - 50 - shift * shiftMult)
            //        {
            //            y2 = yMult * Math.Cos((i + shift + 1) * xMult) * Math.Sin((i + shift + 1) * xMult) + canvas.ActualHeight - 50 - shift * shiftMult;
            //        }
            //        else
            //        {
            //            y2 = yMult * Math.Cos((i + shift + 2) * xMult) * Math.Sin((i + shift + 2) * xMult) + canvas.ActualHeight - 50 - shift * shiftMult;
            //        }
            //        canvas.Children.Add(CreateLine((i) * xMult, y1, (i + 1) * xMult, y2, Brushes.Green));
            //    }
            //}
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Draw();
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            canvas.Children.Clear();
            xMult = e.NewValue;
            Draw();
        }

        private void Slider_ValueChanged_1(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            canvas.Children.Clear();
            yMult = e.NewValue;
            Draw();
        }

        private void Slider_ValueChanged_2(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            canvas.Children.Clear();
            shiftMult = e.NewValue;
            Draw();
        }
    }
}

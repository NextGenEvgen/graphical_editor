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
using System.Windows.Shapes;

namespace Графический_редактор
{
    enum AddingMode
    {
        StartFigure,
        EndFigure,
        None
    }

    /// <summary>
    /// Логика взаимодействия для Morfing.xaml
    /// </summary>
    public partial class Morfing : Window
    {
        AddingMode mode = AddingMode.None;
        private List<Point> pBuffer;
        private List<Point> startEndPoints;
        private List<MyLine> sFigure;
        private List<MyLine> eFigure;

        public Morfing()
        {
            InitializeComponent();
        }

        private void StartFigure_Click(object sender, RoutedEventArgs e)
        {
            mode = AddingMode.StartFigure;
            startEndPoints.Clear();
            pBuffer.Clear();
            foreach(var ml in sFigure)
            {
                ml.Remove();
            }
            sFigure.Clear();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key==Key.Escape)
            {
                Point startPoint = pBuffer[pBuffer.Count - 1];
                Point endPoint = pBuffer[0];
                if (mode == AddingMode.StartFigure)
                {
                    MyLine line = new MyLine(startPoint.X, startPoint.Y, endPoint.X, endPoint.Y);
                    line.Draw(canvas);
                    sFigure.Add(line);
                    mode = AddingMode.None;
                    foreach (var ml in sFigure)
                    {
                        startEndPoints.Add(ml.StartPoint);
                        startEndPoints.Add(ml.EndPoint);
                        ml.Color = Brushes.Green;
                        ml.Group = sFigure;
                    }
                }
                if (mode == AddingMode.EndFigure)
                {
                    MyLine line = new MyLine(startPoint.X, startPoint.Y, endPoint.X, endPoint.Y);
                    line.Draw(canvas);
                    eFigure.Add(line);
                    mode = AddingMode.None;
                    foreach (var ml in eFigure)
                    {
                        ml.Color = Brushes.Yellow;
                        ml.Group = eFigure;
                    }
                }
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            sFigure = new List<MyLine>();
            eFigure = new List<MyLine>();
            pBuffer = new List<Point>();
            startEndPoints = new List<Point>();
        }

        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            pBuffer.Add(e.GetPosition(null));
            if (pBuffer.Count > 1)
            {
                Point startPoint = pBuffer[pBuffer.Count - 2];
                Point endPoint = pBuffer[pBuffer.Count - 1];
                MyLine line = new MyLine();
                if (mode != AddingMode.None)
                {
                    line = new MyLine(startPoint.X, startPoint.Y, endPoint.X, endPoint.Y);                    
                    line.Draw(canvas);
                }
                if (mode == AddingMode.StartFigure)
                {                    
                    sFigure.Add(line);
                }
                if (mode == AddingMode.EndFigure)
                {
                    eFigure.Add(line);
                }
            }
        }

        private void EndFigure_Click(object sender, RoutedEventArgs e)
        {
            mode = AddingMode.EndFigure;
            pBuffer.Clear();
            foreach (var ml in eFigure)
            {
                ml.Remove();
            }
            eFigure.Clear();
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (sFigure.Count != eFigure.Count)
            {
                MessageBox.Show("Количество точек начальной и конечной фигуры не совпадают!");
                return;
            }
            var t = e.NewValue;
            for (int i = 0; i < sFigure.Count;i++)
            {
                Point start = startEndPoints[2*i];
                Point end = startEndPoints[2 * i + 1];
                sFigure[i].MakeShift(start.X*(1-t) + eFigure[i].StartPoint.X*t,start.Y*(1-t) + eFigure[i].StartPoint.Y*t,
                    end.X * (1 - t) + eFigure[i].EndPoint.X * t, end.Y * (1 - t) + eFigure[i].EndPoint.Y * t, true);
            }
        }
    }
}

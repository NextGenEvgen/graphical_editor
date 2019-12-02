using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using static System.Math;
namespace Графический_редактор
{
    public class MyLine : UIElement
    {
        //Центр координат
        private double oX;
        private double oY;

        public bool IsFocused { get; set; }
        //Прямая
        private Line line;
        private Label startCoords;
        private Label endCoords;        
        private Point mouseCoords;
        private Ellipse startEllipse;
        private Ellipse endEllipse;
        private Canvas canvas;
        public Label Data { get; set; }

        /// <summary>
        /// Конструктор класса
        /// </summary>
        /// <param name="A">Коэффициент А</param>
        /// <param name="B">Коэффициент B</param>
        /// <param name="C">Коэффициент B</param>
        
        public MyLine()
        {

        }
        public MyLine(double A, double B, double C,double X1, double X2)
        {
            //Определение центра координат
            oX = 1083 / 2;
            oY = 734 / 2;

            IsFocused = false;
            //Инициализация
            line = new Line();
            startEllipse = new Ellipse();
            endEllipse = new Ellipse();
            //Установка свойств прямой
            line.Stroke = Brushes.Black;            
            line.StrokeThickness = 3;
            line.X1 = X1*10 + oX;
            line.Y1 = ((A * X1 + C) / B)*10 + oY;
            line.X2 = X2*10 + oX;
            line.Y2 = ((A * X2 + C) / B)*10 + oY;
            //Перемещение кругов
            startEllipse.RenderTransform = new TranslateTransform(line.X1 - 5, line.Y1 - 5);
            endEllipse.RenderTransform = new TranslateTransform(line.X2 - 5, line.Y2 - 5);
            startEllipse.Fill = Brushes.Black;
            endEllipse.Fill = Brushes.Black;
            startEllipse.Height = 10;
            startEllipse.Width = 10;
            endEllipse.Height = 10;
            endEllipse.Width = 10;

            startCoords = new Label();
            startCoords.Content = $"{Round((line.X1 - oX) / 10, 2)};{Round((line.Y1 - oY) / -10, 2)}";
            startCoords.Margin = new Thickness(line.X1 -  10, line.Y1, 0, 0);
            startCoords.Foreground = Brushes.White;

            endCoords = new Label();
            endCoords.Content = $"{Round((line.X2 - oX) / 10, 2)};{Round((line.Y2 - oY) / -10, 2)}";
            endCoords.Margin = new Thickness(line.X2 - 10, line.Y2, 0, 0);
            endCoords.Foreground = Brushes.White;
            //Назначение событий перемещения
            startEllipse.MouseDown += OnMouseDown;
            startEllipse.MouseMove += Start_MouseMove;
            startEllipse.MouseUp += OnMouseUp;            
            
            endEllipse.MouseDown += OnMouseDown;
            endEllipse.MouseMove += End_MouseMove;
            endEllipse.MouseUp += OnMouseUp;          
            
            
            line.MouseLeftButtonDown += OnMouseDown;
            line.MouseUp += OnMouseUp;
            line.MouseMove += Line_MouseMove;
            line.MouseEnter += Line_MouseEnter;
            line.MouseRightButtonDown += Line_MouseRightButtonDown;
        }

        public void LooseFocus()
        {
            IsFocused = false;
            line.Stroke = Brushes.Black;
            startEllipse.Fill = Brushes.Black;
            endEllipse.Fill = Brushes.Black;
            line.RenderTransform = null;
        }

        private void Line_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            foreach (var c in canvas.Children)
            {
                var ml = c as MyLine;
                if (ml != null && ml.IsFocused == true)
                {
                    ml.LooseFocus();
                }
            }
            IsFocused = true;
            line.Stroke = Brushes.Green;
            startEllipse.Fill = Brushes.Green;
            endEllipse.Fill = Brushes.Green;
        }

        private int GDC(int a, int b)
        {
            while (b != 0)
            {
                int temp = b;
                b = a % b;
                a = temp;
            }
            return a;
        }

        private void Line_MouseEnter(object sender, MouseEventArgs e)
        {            
            if (Data == null) throw new System.Exception("Объекту не передан Label для вывода данных");
            double A = -((line.Y2-oY)/-10) + ((line.Y1 - oY) / -10);
            double B = ((line.X2 - oX) / 10) - ((line.X1 - oX) / 10);
            double C = ((line.X1 - oX) / 10) * ((line.Y2 - oY) / -10) - ((line.X2 - oX) / 10) * ((line.Y1 - oY) / -10);
            int gdc = GDC((int)A, (int)B);
            A /= gdc;
            B /= gdc;
            C /= gdc;
            Data.Content = $"{Round(A, 2)}x{(B < 0 ? Round(B, 2).ToString() : "+" + Round(B, 2).ToString())}y{(C < 0 ? Round(C, 2).ToString() : "+" + Round(C, 2).ToString())}=0";
        }

        private void OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            
            line.Y1 = (startEllipse.RenderTransform as TranslateTransform).Y + 5;
            line.Y2 = (endEllipse.RenderTransform as TranslateTransform).Y + 5;
            startCoords.Content = $"{Round((line.X1 - oX) / 10, 2)};{Round((line.Y1 - oY) / -10, 2)}";
            startCoords.Margin = new Thickness(line.X1 - 5, line.Y1, 0, 0);
            endCoords.Content = $"{Round((line.X2 - oX) / 10, 2)};{Round((line.Y2 - oY) / -10, 2)}";
            endCoords.Margin = new Thickness(line.X2 - 5, line.Y2, 0, 0);
            mouseCoords = e.GetPosition(null);
            (sender as Shape).ReleaseMouseCapture();
        }

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            mouseCoords = e.GetPosition(line);
            (sender as Shape).CaptureMouse();
        }

        private void Start_MouseMove(object sender, MouseEventArgs e)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                startEllipse.RenderTransform = new TranslateTransform(e.GetPosition(null).X - 5, e.GetPosition(null).Y - 5); 
                line.X1 = e.GetPosition(line).X;
                line.Y1 = e.GetPosition(line).Y;
            }

        }

        private void End_MouseMove(object sender, MouseEventArgs e)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                endEllipse.RenderTransform = new TranslateTransform(e.GetPosition(null).X - 5, e.GetPosition(null).Y - 5);
                line.X2 = e.GetPosition(line).X;
                line.Y2 = e.GetPosition(line).Y;
            }

        }

        private void Line_MouseMove(object sender, MouseEventArgs e)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                startEllipse.RenderTransform = new TranslateTransform(line.X1 - 5, e.GetPosition(null).Y + line.Y1 - mouseCoords.Y - 5);
                endEllipse.RenderTransform = new TranslateTransform(line.X2 - 5, e.GetPosition(null).Y + line.Y2 - mouseCoords.Y - 5);
                //line.Y1 = (startEllipse.RenderTransform as TranslateTransform).Y;
                //line.Y2 = (startEllipse.RenderTransform as TranslateTransform).Y;
                //line.RenderTransform = new TranslateTransform(0, e.GetPosition(null).Y - mouseCoords.Y);
            }
        }

        /// <summary>
        /// Отрисовка объекта
        /// </summary>
        /// <param name="canvas">Полотно для рисования</param>
        public void Draw(Canvas canvas)
        {
            this.canvas = canvas;
            canvas.Children.Add(this);
            canvas.Children.Add(line);
            canvas.Children.Add(startEllipse);
            canvas.Children.Add(endEllipse);
            canvas.Children.Add(startCoords);
            canvas.Children.Add(endCoords);
        }

        public void Transform(Matrix matrix)
        {
            if (line == null)
            {
                MessageBox.Show("Ни одна прямая не была в фокусе");
                return;
            }
            line.X1 = (line.X1 - oX) * matrix.M11 + (line.Y1 - oY) * matrix.M21 + (matrix.OffsetX + oX);
            line.X2 = (line.X2 - oX) * matrix.M11 + (line.Y2 - oY) * matrix.M21 + (matrix.OffsetX + oX);
            line.Y1 = (line.X1 - oX) * matrix.M12 + (line.Y1 - oY) * matrix.M22 + (matrix.OffsetY + oY);
            line.Y2 = (line.X2 - oX) * matrix.M12 + (line.Y2 - oY) * matrix.M22 + (matrix.OffsetY + oY);
            startEllipse.RenderTransform = new TranslateTransform(line.X1 - 5, line.Y1 - 5);
            endEllipse.RenderTransform = new TranslateTransform(line.X2 - 5, line.Y2 - 5);
        }
    }
}

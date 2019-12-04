using System.Collections.Generic;
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
        /// <summary>
        /// Проверка наличия фокуса над прямой
        /// </summary>
        public bool IsFocused { get; set; }
        //Прямая
        private Line line;
        //Отображение начальных и конечных координат
        private Label startCoords;
        private Label endCoords;
        //Координаты мыши
        private Point mouseCoords;
        //Эллипсы вокруг начальной и конечной точкек прямой
        private Ellipse startEllipse;
        private Ellipse endEllipse;
        //Множество одинаковых эллипсов по отношению к нажатому
        private List<Ellipse> commonEllipses;
        //Полотно для рисования
        private Canvas canvas;
        //Группа, в которую входит прямая
        private List<MyLine> group;
        /// <summary>
        /// Label для вывода уравнения прямой
        /// </summary>
        public Label Data { get; set; }
        /// <summary>
        /// Группа, в которую входит прямая
        /// </summary>
        public List<MyLine> Group
        {
            set => group = value;
            get => group;
        }
        /// <summary>
        /// Начальная точка прямой
        /// </summary>
        public Point StartPoint { get => new Point(line.X1, line.Y1); }
        /// <summary>
        /// Конечная точка прямой
        /// </summary>
        public Point EndPoint { get => new Point(line.X2, line.Y2); }
        /// <summary>
        /// Эллипс в начале прямой
        /// </summary>
        public Ellipse StartEllipse { get => startEllipse; set => startEllipse = value; }
        /// <summary>
        /// Эллипс в конце прямой
        /// </summary>
        public Ellipse EndEllipse { get => endEllipse; set => endEllipse = value; }
        //Цвет прямой и эллипсов
        private SolidColorBrush color = Brushes.Black;
        /// <summary>
        /// Цвет прямой и эллипсов
        /// </summary>
        public SolidColorBrush Color
        {
            get { return color; }
            set
            {
                color = value;
                line.Stroke = value;
                startEllipse.Fill = value;
                endEllipse.Fill = value;
            }
        }
        /// <summary>
        /// Конструктор класса
        /// </summary>
        public MyLine()
        {

        }
        /// <summary>
        /// Конструктор класса
        /// </summary>
        /// <param name="A">Коэффициент А</param>
        /// <param name="B">Коэффициент B</param>
        /// <param name="C">Коэффициент B</param>
        /// <param name="X1">Начальная точка на оси Х</param>
        /// <param name="X2">Конечная точка на оси Y</param>
        public MyLine(double A, double B, double C, double X1, double X2)
        {
            //Определение центра координат
            oX = 1083 / 2;
            oY = 734 / 2;
            //Установка фокуса на false
            IsFocused = false;
            //Инициализация
            line = new Line();
            startEllipse = new Ellipse();
            endEllipse = new Ellipse();
            //Установка свойств прямой
            line.Stroke = Brushes.Black;
            line.StrokeThickness = 3;
            line.X1 = X1 * 10 + oX;
            line.Y1 = ((A * X1 + C) / B) * 10 + oY;
            line.X2 = X2 * 10 + oX;
            line.Y2 = ((A * X2 + C) / B) * 10 + oY;
            //Перемещение кругов в соответствующие им точки
            startEllipse.RenderTransform = new TranslateTransform(line.X1 - 5, line.Y1 - 5);
            endEllipse.RenderTransform = new TranslateTransform(line.X2 - 5, line.Y2 - 5);
            startEllipse.Fill = Brushes.Black;
            endEllipse.Fill = Brushes.Black;
            startEllipse.Height = 10;
            startEllipse.Width = 10;
            endEllipse.Height = 10;
            endEllipse.Width = 10;
            //Инициализация ярлыков
            startCoords = new Label();
            startCoords.Content = $"{Round((line.X1 - oX) / 10, 2)};{Round((line.Y1 - oY) / -10, 2)}";
            startCoords.Margin = new Thickness(line.X1 - 10, line.Y1, 0, 0);
            startCoords.Foreground = Brushes.White;
            endCoords = new Label();
            endCoords.Content = $"{Round((line.X2 - oX) / 10, 2)};{Round((line.Y2 - oY) / -10, 2)}";
            endCoords.Margin = new Thickness(line.X2 - 10, line.Y2, 0, 0);
            endCoords.Foreground = Brushes.White;
            //Назначение событий перемещения
            startEllipse.MouseDown += OnMouseDown;
            startEllipse.MouseMove += EllipseMove;
            startEllipse.MouseUp += OnEllipseMouseUp;
            endEllipse.MouseDown += OnMouseDown;
            endEllipse.MouseMove += EllipseMove;
            endEllipse.MouseUp += OnEllipseMouseUp;
            line.MouseLeftButtonDown += OnMouseDown;
            line.MouseUp += OnMouseUp;
            line.MouseMove += Line_MouseMove;
            line.MouseEnter += Line_MouseEnter;
            line.MouseRightButtonDown += Line_MouseRightButtonDown;
        }
        /// <summary>
        /// Расфокусировка прямой
        /// </summary>
        public void LooseFocus()
        {
            IsFocused = false;
            line.Stroke = Color;
            startEllipse.Fill = Color;
            endEllipse.Fill = Color;
        }

        private void Line_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            IsFocused = true;
            SetColor(Brushes.Green);
            if (Group != null)
            {
                foreach (var ml in Group)
                {
                    ml.IsFocused = true;
                    ml.SetColor(Brushes.Green);
                }
            }

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
            double A = -((line.Y2 - oY) / -10) + ((line.Y1 - oY) / -10);
            double B = ((line.X2 - oX) / 10) - ((line.X1 - oX) / 10);
            double C = ((line.X1 - oX) / 10) * ((line.Y2 - oY) / -10) - ((line.X2 - oX) / 10) * ((line.Y1 - oY) / -10);
            int gdc = GDC((int)A, (int)B);
            A /= gdc;
            B /= gdc;
            C /= gdc;
            Data.Content = $"{Round(A, 2)}x{(B < 0 ? Round(B, 2).ToString() : "+" + Round(B, 2).ToString())}y{(C < 0 ? Round(C, 2).ToString() : "+" + Round(C, 2).ToString())}=0";
        }

        private void OnEllipseMouseUp(object sender, MouseButtonEventArgs e)
        {
            //commonCoords.Clear();
            mouseCoords = e.GetPosition(null);
            (sender as Shape).ReleaseMouseCapture();
        }

        private void OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            double offsetX = (startEllipse.RenderTransform as TranslateTransform).X + 5 - line.X1;
            double offsetY = (startEllipse.RenderTransform as TranslateTransform).Y + 5 - line.Y1;
            if (Group != null)
            {
                foreach (var ml in Group)
                {
                    ml.MakeShift(offsetX, offsetY);
                }
            }
            if (Group == null) MakeShift(offsetX, offsetY);
            mouseCoords = e.GetPosition(null);
            (sender as Shape).ReleaseMouseCapture();
        }
        /// <summary>
        /// Сдвиг прямой
        /// </summary>
        /// <param name="offsetX1">Сдвиг начала прямой по оси Х</param>
        /// <param name="offsetY1">Сдвиг начала прямой по оси Y</param>
        /// <param name="offsetX2">Сдвиг конца прямой по оси Х</param>
        /// <param name="offsetY2">Сдвиг конца прямой по оси Y</param>
        public void MakeShift(double offsetX1, double offsetY1, double offsetX2, double offsetY2)
        {
            line.X1 += offsetX1;
            line.X2 += offsetX2;
            line.Y1 += offsetY1;
            line.Y2 += offsetY2;
            ReplaceLabels();
            //startEllipse.RenderTransform = new TranslateTransform(line.X1 - 5, line.Y1 - 5);
            //endEllipse.RenderTransform = new TranslateTransform(line.X2 - 5, line.Y2 - 5);
        }
        /// <summary>
        /// Сдвиг всей прямой
        /// </summary>
        /// <param name="offsetX">Сдвиг по оси Х</param>
        /// <param name="offsetY">Сдвиг по оси Y</param>
        public void MakeShift(double offsetX, double offsetY)
        {
            line.X1 += offsetX;
            line.X2 += offsetX;
            line.Y1 += offsetY;
            line.Y2 += offsetY;
            ReplaceLabels();
            startEllipse.RenderTransform = new TranslateTransform(line.X1 - 5, line.Y1 - 5);
            endEllipse.RenderTransform = new TranslateTransform(line.X2 - 5, line.Y2 - 5);
        }

        private Ellipse FindCommonElipses(Point coords, MyLine myLine)
        {
            if ((coords.X - 5 <= myLine.StartPoint.X && coords.X + 5 >= myLine.StartPoint.X) && (coords.Y - 5 <= myLine.StartPoint.Y && coords.Y + 5 >= myLine.StartPoint.Y))
            {
                return myLine.StartEllipse;
            }
            if ((coords.X - 5 <= myLine.EndPoint.X && coords.X + 5 >= myLine.EndPoint.X) && (coords.Y - 5 <= myLine.EndPoint.Y && coords.Y + 5 >= myLine.EndPoint.Y))
            {
                return myLine.EndEllipse;
            }
            return null;
        }

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            mouseCoords = e.GetPosition(line);
            (sender as Shape).CaptureMouse();
            commonEllipses = new List<Ellipse>();
            Point point = new Point();
            Ellipse ellipse = sender as Ellipse;
            if (ellipse == null) return;
            if (ellipse == startEllipse) point = StartPoint;
            else point = EndPoint;
            commonEllipses.Add(ellipse);
            if (group != null)
            {
                foreach (var ml in Group)
                {
                    var foundEllipse = FindCommonElipses(point, ml);
                    if (foundEllipse != ellipse)
                    {
                        canvas.Children.Remove(foundEllipse);
                        //foundEllipse = ellipse;
                    }
                    if (foundEllipse != null) commonEllipses.Add(foundEllipse);
                }
            }

        }

        private void EllipseMove(object sender, MouseEventArgs e)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                if (group == null)
                {
                    (sender as Ellipse).RenderTransform = new TranslateTransform(e.GetPosition(null).X - 5, e.GetPosition(null).Y - 5);
                    TranslateTransform startTransform = startEllipse.RenderTransform as TranslateTransform;
                    TranslateTransform endTransform = endEllipse.RenderTransform as TranslateTransform;
                    MakeShift(startTransform.X - line.X1 + 5, startTransform.Y - line.Y1 + 5, endTransform.X - line.X2 + 5, endTransform.Y - line.Y2 + 5);
                }
                else
                {
                    //Ellipse el = commonEllipses[0];
                    if (commonEllipses == null) return;
                    foreach (var el in commonEllipses)
                    {
                        el.RenderTransform = new TranslateTransform(e.GetPosition(null).X - 5, e.GetPosition(null).Y - 5);
                        foreach (var ml in group)
                        {
                            if (el == ml.StartEllipse || el == ml.EndEllipse)
                            {
                                TranslateTransform startTransform = ml.StartEllipse.RenderTransform as TranslateTransform;
                                TranslateTransform endTransform = ml.EndEllipse.RenderTransform as TranslateTransform;
                                ml.MakeShift(startTransform.X - ml.StartPoint.X + 5, startTransform.Y - ml.StartPoint.Y + 5, endTransform.X - ml.EndPoint.X + 5, endTransform.Y - ml.EndPoint.Y + 5);
                            }
                        }
                    }
                }
                //(sender as Ellipse).RenderTransform = new TranslateTransform(e.GetPosition(null).X - 5, e.GetPosition(null).Y - 5);
                //TranslateTransform startTransform = startEllipse.RenderTransform as TranslateTransform;
                //TranslateTransform endTransform = endEllipse.RenderTransform as TranslateTransform;
                //MakeShift(startTransform.X - line.X1, startTransform.Y - line.Y1, endTransform.X - line.X2, endTransform.Y - line.Y2);
                //startEllipse.RenderTransform = new TranslateTransform(e.GetPosition(null).X - 5, e.GetPosition(null).Y - 5);
                //line.X1 = e.GetPosition(line).X;
                //line.Y1 = e.GetPosition(line).Y;
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
                startEllipse.RenderTransform = new TranslateTransform(e.GetPosition(null).X + line.X1 - mouseCoords.X - 5, e.GetPosition(null).Y + line.Y1 - mouseCoords.Y - 5);
                endEllipse.RenderTransform = new TranslateTransform(e.GetPosition(null).X + line.X2 - mouseCoords.X - 5, e.GetPosition(null).Y + line.Y2 - mouseCoords.Y - 5);
                //startEllipse.RenderTransform = new TranslateTransform(line.X1 - 5, e.GetPosition(null).Y + line.Y1 - mouseCoords.Y - 5);
                //endEllipse.RenderTransform = new TranslateTransform(line.X2 - 5, e.GetPosition(null).Y + line.Y2 - mouseCoords.Y - 5);
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
        /// <summary>
        /// Удаление прямой с полотна
        /// </summary>
        public void Remove()
        {
            canvas.Children.Remove(line);
            canvas.Children.Remove(startEllipse);
            canvas.Children.Remove(endEllipse);
            canvas.Children.Remove(startCoords);
            canvas.Children.Remove(endCoords);
            canvas.Children.Remove(this);
        }
        /// <summary>
        /// Перемещение ярлыков
        /// </summary>
        public void ReplaceLabels()
        {
            startCoords.Content = $"{Round((line.X1 - oX) / 10, 2)};{Round((line.Y1 - oY) / -10, 2)}";
            startCoords.Margin = new Thickness(line.X1 - 5, line.Y1 + 5, 0, 0);
            endCoords.Content = $"{Round((line.X2 - oX) / 10, 2)};{Round((line.Y2 - oY) / -10, 2)}";
            endCoords.Margin = new Thickness(line.X2 - 5, line.Y2 + 5, 0, 0);
        }
        /// <summary>
        /// Применение матрицы преобразований к прямой
        /// </summary>
        /// <param name="matrix">Матрица преобразований</param>
        public void Transform(Matrix matrix)
        {
            if (line == null)
            {
                MessageBox.Show("Ни одна прямая не была в фокусе");
                return;
            }
            //matrix.OffsetX = (matrix.OffsetX == 0) ? -oX : matrix.OffsetX;
            //matrix.OffsetY = (matrix.OffsetY == 0) ? -oY : matrix.OffsetY;
            double a11 = line.X1 - oX;
            double a12 = line.Y1 - oY;
            double a21 = line.X2 - oX;
            double a22 = line.Y2 - oY;
            line.X1 = a11 * matrix.M11 + a12 * matrix.M21 + (matrix.OffsetX + oX);
            line.Y1 = a11 * matrix.M12 + a12 * matrix.M22 + (matrix.OffsetY + oY);
            line.X2 = a21 * matrix.M11 + a22 * matrix.M21 + (matrix.OffsetX + oX);
            line.Y2 = a21 * matrix.M12 + a22 * matrix.M22 + (matrix.OffsetY + oY);
            //line.X1 -= oX;
            //line.X2 -= oX;
            //line.Y1 += oY;
            //line.Y2 += oY;
            startEllipse.RenderTransform = new TranslateTransform(line.X1 - 5, line.Y1 - 5);
            endEllipse.RenderTransform = new TranslateTransform(line.X2 - 5, line.Y2 - 5);
            ReplaceLabels();
        }
        /// <summary>
        /// Назначение цвета, не задевая свойства Color
        /// </summary>
        /// <param name="color"></param>
        public void SetColor(SolidColorBrush color)
        {
            line.Stroke = color;
            startEllipse.Fill = color;
            endEllipse.Fill = color;
        }
    }
}

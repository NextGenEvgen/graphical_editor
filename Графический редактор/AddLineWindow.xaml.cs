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
    /// <summary>
    /// Логика взаимодействия для AddLineWindow.xaml
    /// </summary>
    public partial class AddLineWindow : Window
    {
        public AddLineWindow()
        {
            InitializeComponent();
        }
        private MyLine line;
        public MyLine Line { get => line; }     
        
        private double TurnToDouble(string str)
        {
            var s = str.Split(new char[] { '/' });
            return Double.Parse(s[0]) / Double.Parse(s[1]);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            foreach (var t in vGrid1.Children)
            {
                var tx = t as TextBox;
                if (tx != null) 
                {
                    if (tx.Text == "")
                    {
                        MessageBox.Show("Введите значение в поле");
                        return;
                    }
                    if (tx.Text.Contains("/")) tx.Text = TurnToDouble(tx.Text).ToString();
                }
            }
            foreach (var t in vGrid2.Children)
            {
                var tx = t as TextBox;
                if (tx != null)
                {
                    if (tx.Text == "")
                    {
                        MessageBox.Show("Введите значение в поле");
                        return;
                    }
                }
            }
            line = new MyLine(double.Parse(a.Text), double.Parse(b.Text), double.Parse(c.Text), double.Parse(x1.Text), double.Parse(x2.Text));
            Close();
        }
    }
}

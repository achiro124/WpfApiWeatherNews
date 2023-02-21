using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WpfApiWeatherNews
{
    /// <summary>
    /// Логика взаимодействия для InfWindow.xaml
    /// </summary>
    public partial class InfWindow : Window
    {
        public InfWindow(string text)
        {
            InitializeComponent();
            txtBlock.Text = text;

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}

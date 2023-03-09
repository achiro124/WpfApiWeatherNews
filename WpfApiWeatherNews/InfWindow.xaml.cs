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
using System.Windows.Threading;

namespace WpfApiWeatherNews
{
    /// <summary>
    /// Логика взаимодействия для InfWindow.xaml
    /// </summary>
    public partial class InfWindow : Window
    {
        private DispatcherTimer timer = new DispatcherTimer();
        public InfWindow((string, string) weather)
        {
            InitializeComponent();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += timer_Tick;
            timer.Start();


            txtTown.Text = weather.Item1 + ": " + weather.Item2;

            this.DataContext = this;
        }

        private void timer_Tick(object? sender, EventArgs e)
        {
            if(this.Opacity <= 0)
            {
                this.Close();
            }
            this.Opacity -= 0.05;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_MouseEnter(object sender, MouseEventArgs e)
        {
            timer.Stop();
            this.Opacity = 1;
        }

        private void Window_MouseLeave(object sender, MouseEventArgs e)
        {
            timer.Start();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if(e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }
    }
}

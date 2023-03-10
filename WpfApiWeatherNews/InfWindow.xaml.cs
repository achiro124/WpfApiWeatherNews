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
using System.Windows.Media.Animation;
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
        private DispatcherTimer timer;
        public InfWindow((string, string) weather)
        {
            InitializeComponent();
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(10);
            timer.Tick += Timer_Tick;
            timer.Start();


            txtTown.Text = weather.Item1 + ": " + weather.Item2;
            this.DataContext = this;
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            if (Opacity > 0)
            {
                Opacity -= 0.001;
            }
            else
            {
                timer.Stop();
                Close();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            timer.Stop();
            Opacity = 1;
        }

        private void Window_Deactivated(object sender, EventArgs e)
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

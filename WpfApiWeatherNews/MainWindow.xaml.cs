using Gma.System.MouseKeyHook;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WinForms = System.Windows.Forms;

namespace WpfApiWeatherNews
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    
    public partial class MainWindow : Window
    {
        private WinForms.NotifyIcon notifier = new WinForms.NotifyIcon();

        public MainWindow()
        {
            InitializeComponent();



            // Note: for the application hook, use the Hook.AppEvents() instead.
            //globalMouseHook = Hook.GlobalEvents();

            // Same as double click, so I didn't write here.
            //globalMouseHook.MouseDragFinished += CreateWindow;

            this.notifier.MouseDown += new WinForms.MouseEventHandler(notifier_MouseDown);
            this.notifier.Icon = new System.Drawing.Icon("D:\\C#\\WpfApiWeatherNews\\WpfApiWeatherNews\\Resources\\icon.ico");
            this.notifier.Visible = true;

        }


        void notifier_MouseDown(object sender, WinForms.MouseEventArgs e)

        {
            ContextMenu menu = (ContextMenu)this.FindResource("NotifierContextMenu");
            if (e.Button == WinForms.MouseButtons.Right)
            {
                menu.IsOpen = true;
            }
            else if(e.Button == WinForms.MouseButtons.Left)
            {
                menu.IsOpen = false;
            }

        }

        private void Menu_Open(object sender, RoutedEventArgs e)

        {

            this.Visibility = Visibility.Visible;

        }

        private void Menu_Close(object sender, RoutedEventArgs e)
        {
            this.notifier.Dispose();
            this.Close();

        }


        //private IKeyboardMouseEvents globalMouseHook;

        [STAThread]
        private async void CreateWindow(Key k, ModifierKeys mod)
        {

            IDataObject tmpClipboard = System.Windows.Clipboard.GetDataObject();
            //System.Windows.Clipboard.Clear();
            await Task.Delay(100);

            //Clipboard.SetText("123");
            // Send Ctrl+C, which is "copy"
            System.Windows.Forms.SendKeys.SendWait("^c");
            await Task.Delay(100);



            if (System.Windows.Clipboard.ContainsText())
            {
                string text = System.Windows.Clipboard.GetText();
                InfWindow infWindow = new InfWindow(text);
                infWindow.Show();
                // Your code
            }
            else
            {   
                // Restore the Clipboard.
                System.Windows.Clipboard.SetDataObject(tmpClipboard);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            var hotKeyHost = new HotKeyHost((HwndSource)PresentationSource.FromVisual(this));
            hotKeyHost.AddHotKey(new CustomHotKey(Key.None, ModifierKeys.Control, CreateWindow));
            hotKeyHost.AddHotKey(new CustomHotKey(Key.A, ModifierKeys.Control, CreateWindow));

            this.Visibility = Visibility.Hidden;
        }
    }
}

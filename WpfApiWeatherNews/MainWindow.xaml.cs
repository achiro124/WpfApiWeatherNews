using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using WinForms = System.Windows.Forms;
using System.Net;
using System.IO;
using IDataObject = System.Windows.IDataObject;
using RestSharp;
using System.Drawing;
using System.Windows.Forms;

namespace WpfApiWeatherNews
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 


    public partial class MainWindow : System.Windows.Window
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
        private async void TextSelection()
        {

            IDataObject tmpClipboard = System.Windows.Clipboard.GetDataObject();
            //System.Windows.Clipboard.Clear();
            await Task.Delay(50);
            // Send Ctrl+C, which is "copy"
            System.Windows.Forms.SendKeys.SendWait("^c");
            await Task.Delay(50);



            if (System.Windows.Clipboard.ContainsText())
            {
                string text = System.Windows.Clipboard.GetText();
                //ApiWeather(text);
                //ApiTranslate("HI");
                //ApiDetect("Привет");
                CreateNewWindow(GetMousePosition(), text);
                
            }
            else
            {
                // Restore the Clipboard.
                //System.Windows.Clipboard.SetDataObject(tmpClipboard);
            }
        }   
        private void ApiWeather(string text)
        {
            string url = $"https://api.openweathermap.org/data/2.5/weather?q={text}&appid=45d5a3898d82a7610684380eed0a7bff&units=metric";
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse res = (HttpWebResponse)req.GetResponse();
            StreamReader reader = new StreamReader(res.GetResponseStream());
            string response = reader.ReadToEnd();
        }

    //    private void ApiTranslate(string text)
    //    {
    //        var client = new RestClient("https://google-translate1.p.rapidapi.com/language/translate/v2");
    //        var request = new RestRequest("/resource/", Method.Get)
    //        {
    //            RequestFormat = RestSharp.DataFormat.Json
    //        };
    //        request.AddHeader("content-type", "application/x-www-form-urlencoded");
    //
    //        request.AddHeader("Accept-Encoding", "application/gzip");
    //        request.AddHeader("X-RapidAPI-Key", "73cbdbded0msh1b1e8244f1c101cp193587jsna6ed49a51a2f");
    //        request.AddHeader("X-RapidAPI-Host", "google-translate1.p.rapidapi.com");
    //        request.AddParameter("application/x-www-form-urlencoded", "q=Hello%2C%20world!&target=es&source=en", ParameterType.RequestBody);
    //        var client.Execute(request);
    //    }
    //
    //    private string ApiDetect(string text)
    //    {
    //        var client = new RestClient("https://google-translate1.p.rapidapi.com/language/translate/v2/detect");
    //        var request = new RestRequest("POST");
    //        request.AddHeader("content-type", "application/x-www-form-urlencoded");
    //        request.AddHeader("Accept-Encoding", "application/gzip");
    //        request.AddHeader("X-RapidAPI-Key", "73cbdbded0msh1b1e8244f1c101cp193587jsna6ed49a51a2f");
    //        request.AddHeader("X-RapidAPI-Host", "google-translate1.p.rapidapi.com");
    //        request.AddParameter("application/x-www-form-urlencoded", "q=%D0%9F%D1%80%D0%B8%D0%B2%D0%B5%D1%82", ParameterType.RequestBody);
    //        var response = client.Execute(request);
    //        return text;
    //    }
        private void CreateNewWindow(System.Windows.Point startLocation, string text)
        {
            var window = new InfWindow(text);
            window.Owner = this;
            var helper = new WindowInteropHelper(window);
            var hwndSource = HwndSource.FromHwnd(helper.EnsureHandle());
            var transformFromDevice = hwndSource.CompositionTarget.TransformFromDevice;

            System.Windows.Point wpfMouseLocation = transformFromDevice.Transform(GetMousePosition());

            System.Windows.Point resolution = transformFromDevice.Transform(new System.Windows.Point(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height));
            //Rectangle resolution = Screen.PrimaryScreen.Bounds;
            System.Windows.Point windowX_Y = transformFromDevice.Transform(new System.Windows.Point(window.Width,window.Height));

            if (wpfMouseLocation.Y + 40> resolution.Y)
            {
                window.Top = resolution.Y - window.Height - 40;
            }
            else
            {
                if(wpfMouseLocation.Y - windowX_Y.Y < 0)
                    window.Top = 0;
                else
                    window.Top = wpfMouseLocation.Y - window.Height;
            }

            if(wpfMouseLocation.X + windowX_Y.X> resolution.X)
            {
                window.Left = resolution.X - window.Width;
            }
            else
            {
                window.Left = wpfMouseLocation.X;
            }
            
            window.ShowDialog();


        }

        public System.Windows.Point GetMousePosition()
        {
            System.Drawing.Point point = System.Windows.Forms.Cursor.Position;
            return new System.Windows.Point(point.X, point.Y);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            var hotKeyHost = new HotKeyHost((HwndSource)PresentationSource.FromVisual(this));
            //hotKeyHost.AddHotKey(new CustomHotKey(Key.None, ModifierKeys.Control, TextSelection));
            hotKeyHost.AddHotKey(new CustomHotKey(Key.A, ModifierKeys.Control, TextSelection));

            this.Visibility = Visibility.Hidden;
        }
    }
}

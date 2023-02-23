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
using System.Net.Http;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Runtime.InteropServices;
using System.Diagnostics;

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

        [DllImport("User32.dll")]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("User32.dll")]
        static extern int SetForegroundWindow(IntPtr hWnd);


        [STAThread]
        private async void TextSelection()
        {
            //Process process = Process.Start("notepad.exe");
            IntPtr hWndNotepad = FindWindow("Notepad", null);
            SetForegroundWindow(hWndNotepad);

            IDataObject tmpClipboard = System.Windows.Clipboard.GetDataObject();
            //System.Windows.Clipboard.Clear();
            await Task.Delay(50);
            // Send Ctrl+C, which is "copy"
            System.Windows.Forms.SendKeys.SendWait("^c");
            await Task.Delay(50);

            if (System.Windows.Clipboard.ContainsText())
            {
                string text = System.Windows.Clipboard.GetText();
                string result;

                try
                {
                    //Определение к какому языку принадлежит слово(ru, eng и т.д.)
                    //string language = await ApiDetect(text);
                    //Перевод с определенного языка на английский
                    result = await ApiTranslate(text);
                    //Запрос
                    // 
                }
                catch (Exception)
                {
                    result = "Неккоректное слово";
                }

                CreateNewWindow(GetMousePosition(), result);
                
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

        private async Task<string> ApiTranslate(string text)
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"https://translated-mymemory---translation-memory.p.rapidapi.com/get?langpair=ru%7Cen&q={text}&onlyprivate=0&de=a%40b.c"),
                Headers =
                {
                    { "X-RapidAPI-Key", "73cbdbded0msh1b1e8244f1c101cp193587jsna6ed49a51a2f" },
                    { "X-RapidAPI-Host", "translated-mymemory---translation-memory.p.rapidapi.com" },
                },
            };
            using (var response = await client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                var body = await response.Content.ReadAsStringAsync();
                dynamic? d = JsonConvert.DeserializeObject(body);
                if (d != null)
                {
                    text = d.responseData.translatedText.ToString();
                }
            }

            return text;
        }
    
        private async Task<string> ApiDetect(string text)
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri("https://google-translate1.p.rapidapi.com/language/translate/v2/detect"),
                Headers =
                {
                    { "X-RapidAPI-Key", "73cbdbded0msh1b1e8244f1c101cp193587jsna6ed49a51a2f" },
                    { "X-RapidAPI-Host", "google-translate1.p.rapidapi.com" },
                },
                Content = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    { "q", $"{text}" },
                }),
            };
            using (var response = await client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                var body = await response.Content.ReadAsStringAsync();
                dynamic? d = JsonConvert.DeserializeObject(body);
                string language = d != null ? d.data.detections[0][0].language.ToString() : "";
                return language;
            }
        }
        private void CreateNewWindow(System.Windows.Point startLocation, string text)
        {
            var window = new InfWindow(text);
            window.Owner = this;
            var helper = new WindowInteropHelper(window);
            var hwndSource = HwndSource.FromHwnd(helper.EnsureHandle());
            var transformFromDevice = hwndSource.CompositionTarget.TransformFromDevice;

            System.Windows.Point wpfMouseLocation = transformFromDevice.Transform(GetMousePosition());
            System.Windows.Point resolution = transformFromDevice.Transform(new System.Windows.Point(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height));
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

            this.notifier.MouseDown += new WinForms.MouseEventHandler(notifier_MouseDown);
            this.notifier.Icon = new System.Drawing.Icon("D:\\C#\\WpfApiWeatherNews\\WpfApiWeatherNews\\Resources\\icon.ico");
            this.notifier.Visible = true;

            this.Visibility = Visibility.Hidden;
        }
    }
}

using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using System.Xml;
using System.Xml.Linq;
using IDataObject = System.Windows.IDataObject;
using WinForms = System.Windows.Forms;

namespace WpfApiWeatherNews
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 


    public partial class MainWindow : System.Windows.Window ,INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        private InfWindow? window;
        protected virtual void OnPropertyChanged(string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private WinForms.NotifyIcon notifier = new WinForms.NotifyIcon();

        private HotKeyHost? hotKeyHost;

        private (ModifierKeys, Key) keys;
        public (ModifierKeys,Key) Keys 
        {
            get => keys;
            set
            {
                keys = value;
                OnPropertyChanged(nameof(Keys));
            }
        }
 
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Menu_Close(object sender, EventArgs e)
        {
            this.notifier.Dispose();
            this.Close();
        }
        private async void TextSelection()
        {
            if (System.Windows.Clipboard.ContainsText())
            {
                string text = System.Windows.Clipboard.GetText();
                string result;
                (string, string) weather;

                try
                {
                    text = await ApiTranslate(text);
                    weather = ApiWeather(text); 
                }
                catch (Exception)
                {
                    weather.Item1 = "Неккоректное слово";
                    weather.Item2 = "";
                }

                CreateNewWindow(GetMousePosition(), weather);
                if(window?.ShowDialog() == false)
                {
                }
                
            }
        }   
        private (string,string) ApiWeather(string text)
        {
            string url = $"https://api.openweathermap.org/data/2.5/weather?q={text}&appid=45d5a3898d82a7610684380eed0a7bff&units=metric";
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse res = (HttpWebResponse)req.GetResponse();
            StreamReader reader = new StreamReader(res.GetResponseStream());
            string response = reader.ReadToEnd();
           // var body = response.Content();
            dynamic? d = JsonConvert.DeserializeObject(response);
            (string, string) weather = ("","");
            if (d != null)
            {
                weather.Item1 = d.name.ToString();
                weather.Item2 = d.main.temp.ToString() + " °C";
            }
            return (weather);
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
        private void CreateNewWindow(System.Windows.Point startLocation, (string,string) weather)
        {
            window = new InfWindow(weather);
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
        }

        public System.Windows.Point GetMousePosition()
        {
            System.Drawing.Point point = System.Windows.Forms.Cursor.Position;
            return new System.Windows.Point(point.X, point.Y);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            CreateXmlPersonKeys();
            hotKeyHost = new HotKeyHost((HwndSource)PresentationSource.FromVisual(this));
            hotKeyHost.AddHotKey(new CustomHotKey(Keys.Item2, Keys.Item1, TextSelection));

            ToolStripMenuItem settingMenuItem = new ToolStripMenuItem("Настройки");
            ToolStripMenuItem closeMenuItem = new ToolStripMenuItem("Выйти");
            settingMenuItem.Click += Menu_Settings;
            closeMenuItem.Click += Menu_Close;

            notifier.ContextMenuStrip = new ContextMenuStrip();
            notifier.ContextMenuStrip.Items.AddRange(new[] {settingMenuItem, closeMenuItem});

            var exePath = AppDomain.CurrentDomain.BaseDirectory;
            var path = Path.Combine(exePath, @"Resources\icon.ico");
            this.notifier.Icon = new System.Drawing.Icon(path);
            this.notifier.Visible = true;

            this.Visibility = Visibility.Hidden;
            txtBoxKeys.DataContext = this;

        }

        private void Menu_Settings(object sender, EventArgs e)
        {
            this.Visibility = Visibility.Visible;
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
            Hide();
            try
            {
                hotKeyHost.AddHotKey(new CustomHotKey(Keys.Item2, Keys.Item1, TextSelection));
            }
            catch (Exception)
            {
                return;
            }
        }
        private void btnChange_Click(object sender, RoutedEventArgs e)
        {
            hotKeyHost.RemoveHotKey(new CustomHotKey(Keys.Item2, Keys.Item1, TextSelection));
            this.KeyDown += Window_KeyDown;
        }
        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if(e.KeyboardDevice.Modifiers != ModifierKeys.None && e.Key != Key.None)
            {
                if(e.KeyboardDevice.Modifiers != ModifierKeys.Control || e.Key != Key.C)
                    Keys = (e.KeyboardDevice.Modifiers, e.Key);
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            NewPersonKeys(Keys);
            this.KeyDown -= Window_KeyDown;
            try
            {
                hotKeyHost.AddHotKey(new CustomHotKey(Keys.Item2, Keys.Item1, TextSelection));
            }
            catch (Exception)
            {
                return;
            }
        }
        private void CreateXmlPersonKeys()
        {
            var exePath = AppDomain.CurrentDomain.BaseDirectory;
            var path = Path.Combine(exePath, "Resources\\PersonKeys.xml");

            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(path);
            if(xDoc != null)
            {
                foreach (XmlNode node in xDoc.DocumentElement)
                {
                    (ModifierKeys, Key) _keys;
                    _keys.Item2 = (Key)int.Parse(node["Key"].InnerText);
                    _keys.Item1 = (ModifierKeys)int.Parse(node["Modifiers"].InnerText);
                    Keys = _keys;
                }                
            }
        }
        private void NewPersonKeys((ModifierKeys, Key) myCastomKeys)
        {
            var exePath = AppDomain.CurrentDomain.BaseDirectory;
            var path = Path.Combine(exePath, "Resources\\PersonKeys.xml");

            XDocument xdoc = XDocument.Load(path);
            var _key = xdoc.Element("Keys")?
                          .Elements("CastomKey")
                          .FirstOrDefault(p => p.Attribute("name")?.Value == "castomKey");

            if (_key != null)
            {
                var numKey = _key.Element("Key");
                if (numKey != null)
                {
                    int k = (int)myCastomKeys.Item2;
                    numKey.Value = $"{k}";
                }

                var numModif = _key.Element("Modifiers");
                if (numModif != null)
                {
                    int k = (int)myCastomKeys.Item1;
                    numModif.Value = $"{k}";
                }
                xdoc.Save(path);
            }
        }
    } 
}

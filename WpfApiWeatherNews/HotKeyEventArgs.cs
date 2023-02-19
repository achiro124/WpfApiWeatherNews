using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApiWeatherNews
{
    public class HotKeyEventArgs : EventArgs
    {
        public HotKey HotKey { get; private set; }

        public HotKeyEventArgs(HotKey hotKey)
        {
            HotKey = hotKey;
        }
    }
}

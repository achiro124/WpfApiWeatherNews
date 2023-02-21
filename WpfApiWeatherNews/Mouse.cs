using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WpfApiWeatherNews
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct Win32Point
    {
        public int X;
        public int Y;
    };
    internal class Mouse
    {
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetCursorPos(ref Win32Point pt);

        public static Point GetMousePosition()
        {
            Win32Point point = new Win32Point();
            GetCursorPos(ref point);
            return new Point(point.X, point.Y);
        }
    }
}

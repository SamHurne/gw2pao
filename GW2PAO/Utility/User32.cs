using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace GW2PAO.Utility
{
    /// <summary>
    /// User32 utility classes
    /// </summary>
    public static class User32
    {
        [DllImport("user32.dll")]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        [DllImport("user32.dll")]
        private static extern int GetWindowLong(IntPtr hwnd, int index);

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hwnd, int index, int newStyle);

        private static readonly IntPtr HWND_NOTOPMOST = new IntPtr(-2);
        private static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
        private const UInt32 SWP_NOACTIVATE = 0x0010;
        private const UInt32 SWP_NOSIZE = 0x0001;
        private const UInt32 SWP_NOMOVE = 0x0002;
        private const UInt32 SWP_SHOWWINDOW = 0x0040;
        private const int WS_EX_TRANSPARENT = 0x00000020;
        private const int GWL_EXSTYLE = (-20);

        /// <summary>
        /// Sets the given WPF window as topmost or not topmost using a user32 pinvoke call
        /// </summary>
        /// <param name="window">The window to set as topmost</param>
        /// <param name="topMost">True to set window as topmost, false to remove topmost property</param>
        public static void SetTopMost(System.Windows.Window window, bool topMost)
        {
            var handle = new System.Windows.Interop.WindowInteropHelper(window).Handle;
            if (topMost)
                User32.SetWindowPos(handle, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOACTIVATE | SWP_NOSIZE | SWP_NOMOVE);
            else
                User32.SetWindowPos(handle, HWND_NOTOPMOST, 0, 0, 0, 0, SWP_NOACTIVATE | SWP_NOSIZE | SWP_NOMOVE);
        }

        /// <summary>
        /// Sets the given WPF window as "transparent", which allows the mouse to "click-through" the window
        /// </summary>
        /// <param name="hwnd">The window to set as click-through transparent</param>
        public static void SetWindowExTransparent(System.Windows.Window window, bool isTransparent)
        {
            var handle = new System.Windows.Interop.WindowInteropHelper(window).Handle;
            var extendedStyle = GetWindowLong(handle, GWL_EXSTYLE);

            if (isTransparent)
            {
                SetWindowLong(handle, GWL_EXSTYLE, extendedStyle | WS_EX_TRANSPARENT);
            }
            else
            {
                SetWindowLong(handle, GWL_EXSTYLE, extendedStyle & ~WS_EX_TRANSPARENT);
            }
        }
    }
}

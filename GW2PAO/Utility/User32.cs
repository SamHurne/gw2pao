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
        private const int GWL_STYLE = (-16);
        private const int GWL_EXSTYLE = (-20);

        public static class WindowStyles
        {
            public static readonly Int32
            WS_BORDER = 0x00800000,
            WS_CAPTION = 0x00C00000,
            WS_CHILD = 0x40000000,
            WS_CHILDWINDOW = 0x40000000,
            WS_CLIPCHILDREN = 0x02000000,
            WS_CLIPSIBLINGS = 0x04000000,
            WS_DISABLED = 0x08000000,
            WS_DLGFRAME = 0x00400000,
            WS_GROUP = 0x00020000,
            WS_HSCROLL = 0x00100000,
            WS_ICONIC = 0x20000000,
            WS_MAXIMIZE = 0x01000000,
            WS_MAXIMIZEBOX = 0x00010000,
            WS_MINIMIZE = 0x20000000,
            WS_MINIMIZEBOX = 0x00020000,
            WS_OVERLAPPED = 0x00000000,
            WS_OVERLAPPEDWINDOW = WS_OVERLAPPED | WS_CAPTION | WS_SYSMENU | WS_THICKFRAME | WS_MINIMIZEBOX | WS_MAXIMIZEBOX,
            WS_POPUP = unchecked((int)0x80000000),
            WS_POPUPWINDOW = WS_POPUP | WS_BORDER | WS_SYSMENU,
            WS_SIZEBOX = 0x00040000,
            WS_SYSMENU = 0x00080000,
            WS_TABSTOP = 0x00010000,
            WS_THICKFRAME = 0x00040000,
            WS_TILED = 0x00000000,
            WS_TILEDWINDOW = WS_OVERLAPPED | WS_CAPTION | WS_SYSMENU | WS_THICKFRAME | WS_MINIMIZEBOX | WS_MAXIMIZEBOX,
            WS_VISIBLE = 0x10000000,
            WS_VSCROLL = 0x00200000;
        }

        public static class ExtendedWindowStyles
        {
            public static readonly Int32
            WS_EX_ACCEPTFILES = 0x00000010,
            WS_EX_APPWINDOW = 0x00040000,
            WS_EX_CLIENTEDGE = 0x00000200,
            WS_EX_COMPOSITED = 0x02000000,
            WS_EX_CONTEXTHELP = 0x00000400,
            WS_EX_CONTROLPARENT = 0x00010000,
            WS_EX_DLGMODALFRAME = 0x00000001,
            WS_EX_LAYERED = 0x00080000,
            WS_EX_LAYOUTRTL = 0x00400000,
            WS_EX_LEFT = 0x00000000,
            WS_EX_LEFTSCROLLBAR = 0x00004000,
            WS_EX_LTRREADING = 0x00000000,
            WS_EX_MDICHILD = 0x00000040,
            WS_EX_NOACTIVATE = 0x08000000,
            WS_EX_NOINHERITLAYOUT = 0x00100000,
            WS_EX_NOPARENTNOTIFY = 0x00000004,
            WS_EX_OVERLAPPEDWINDOW = WS_EX_WINDOWEDGE | WS_EX_CLIENTEDGE,
            WS_EX_PALETTEWINDOW = WS_EX_WINDOWEDGE | WS_EX_TOOLWINDOW | WS_EX_TOPMOST,
            WS_EX_RIGHT = 0x00001000,
            WS_EX_RIGHTSCROLLBAR = 0x00000000,
            WS_EX_RTLREADING = 0x00002000,
            WS_EX_STATICEDGE = 0x00020000,
            WS_EX_TOOLWINDOW = 0x00000080,
            WS_EX_TOPMOST = 0x00000008,
            WS_EX_TRANSPARENT = 0x00000020,
            WS_EX_WINDOWEDGE = 0x00000100;
        }

        /// <summary>
        /// Sets the NoActivate attribute for the given wpf window, preventing the system from brining the window
        /// to the foreground when clicked - particularly useful for prevent the taskbar from appearing
        /// </summary>
        /// <param name="window">The window to set NoActivate on</param>
        /// <param name="noActivate">True if NoActivate should be set, false to remove the NoActivate property</param>
        public static void SetNoActivate(System.Windows.Window window, bool noActivate)
        {
            var handle = new System.Windows.Interop.WindowInteropHelper(window).Handle;
            var extendedStyle = GetWindowLong(handle, GWL_EXSTYLE);

            if (noActivate)
                SetWindowLong(handle, GWL_EXSTYLE, extendedStyle | ExtendedWindowStyles.WS_EX_NOACTIVATE | ExtendedWindowStyles.WS_EX_APPWINDOW);
            else
                SetWindowLong(handle, GWL_EXSTYLE, extendedStyle & ~ExtendedWindowStyles.WS_EX_NOACTIVATE);
        }

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
                SetWindowLong(handle, GWL_EXSTYLE, extendedStyle | ExtendedWindowStyles.WS_EX_TRANSPARENT);
            }
            else
            {
                SetWindowLong(handle, GWL_EXSTYLE, extendedStyle & ~ExtendedWindowStyles.WS_EX_TRANSPARENT);
            }
        }
    }
}

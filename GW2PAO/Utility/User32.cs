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

        private static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
        private const UInt32 SWP_NOACTIVATE = 0x0010;
        private const UInt32 SWP_NOSIZE = 0x0001;
        private const UInt32 SWP_NOMOVE = 0x0002;
        private const UInt32 SWP_SHOWWINDOW = 0x0040;

        /// <summary>
        /// Sets the given WPF window as topmost using a user32 pinvoke call
        /// </summary>
        /// <param name="window">The window to set as topmost</param>
        public static void SetTopMost(System.Windows.Window window)
        {
            var handle = new System.Windows.Interop.WindowInteropHelper(window).Handle;
            User32.SetWindowPos(handle, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOACTIVATE | SWP_NOSIZE | SWP_NOMOVE);
        }
    }
}

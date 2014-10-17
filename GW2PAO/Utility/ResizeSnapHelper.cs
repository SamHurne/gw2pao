using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

namespace GW2PAO.Utility
{
    /// <summary>
    /// Utility class that provides resizing and resize snap capabilities to borderless windows
    /// </summary>
    public class ResizeSnapHelper
    {
        private enum ResizeDirection
        {
            Left = 61441,
            Right = 61442,
            Top = 61443,
            TopLeft = 61444,
            TopRight = 61445,
            Bottom = 61446,
            BottomLeft = 61447,
            BottomRight = 61448
        }

        /// <summary>
        /// The window that will be resized
        /// </summary>
        private Window window;

        /// <summary>
        /// Win32 window handle to the window that will be resized
        /// </summary>
        private HwndSource hwndSource;

        /// <summary>
        /// The vertical-resizing wpf framework element
        /// </summary>
        private FrameworkElement verticalResizeElement { get; set; }

        /// <summary>
        /// The horizontal-resizing wpf framework element
        /// </summary>
        private FrameworkElement horizontalResizeElement { get; set; }

        /// <summary>
        /// Sends a message to the win32 message queue
        /// </summary>
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        private const int WM_SIZING = 0x0214;
        private const int WMSZ_BOTTOM = 6;
        private const int WMSZ_BOTTOMLEFT = 7;
        private const int WMSZ_BOTTOMRIGHT = 8;
        private const int WMSZ_LEFT = 1;
        private const int WMSZ_RIGHT = 2;
        private const int WMSZ_TOP = 3;
        private const int WMSZ_TOPLEFT = 4;
        private const int WMSZ_TOPRIGHT = 5;

        /// <summary>
        /// Offset to use when snapping the resize width of the window
        /// </summary>
        public int SnappingWidthOffset { get; set; }

        /// <summary>
        /// Offset to use when snapping the resize height of the window
        /// </summary>
        public int SnappingHeightOffset { get; set; }

        /// <summary>
        /// Increment by which to snap the width of the window when resizing the width
        /// </summary>
        public int SnappingIncrementWidth { get; set; }

        /// <summary>
        /// Increment by which to snap the height of the window when resizing the height
        /// </summary>
        public int SnappingIncrementHeight { get; set; }

        /// <summary>
        /// Width threshold to meet before window width resize snapping can occur
        /// </summary>
        public int SnappingThresholdWidth { get; set; }

        /// <summary>
        /// Height threshold to meet before window height resize snapping can occur
        /// </summary>
        public int SnappingThresholdHeight { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="window">The window that will be resized</param>
        public ResizeSnapHelper(Window window)
        {
            this.window = window;
            this.window.SourceInitialized += window_SourceInitialized;

            // Defaults:
            this.SnappingWidthOffset = 0;
            this.SnappingHeightOffset = 0;
            this.SnappingIncrementWidth = 1;
            this.SnappingIncrementHeight = 1;
            this.SnappingThresholdWidth = 0;
            this.SnappingThresholdHeight = 0;
        }

        /// <summary>
        /// Initializes the resize framework elements
        /// </summary>
        /// <param name="verticalResizeElement">The vertical resize framework element, or null if none</param>
        /// <param name="horizontalResizeElement">The horizontal resize framework element, or null if none</param>
        public void InitializeResizeElements(FrameworkElement verticalResizeElement, FrameworkElement horizontalResizeElement)
        {
            this.verticalResizeElement = verticalResizeElement;
            this.horizontalResizeElement = horizontalResizeElement;

            if (this.verticalResizeElement != null)
            {
                this.verticalResizeElement.Cursor = Cursors.SizeNS;
                this.verticalResizeElement.PreviewMouseLeftButtonDown += ManualResize;
            }

            if (this.horizontalResizeElement != null)
            {
                this.horizontalResizeElement.Cursor = Cursors.SizeWE;
                this.horizontalResizeElement.PreviewMouseLeftButtonDown += ManualResize;
            }
        }

        /// <summary>
        /// Handles the source initialized event of the window in order to set up the message queue hook
        /// </summary>
        private void window_SourceInitialized(object sender, EventArgs e)
        {
            IntPtr handle = new WindowInteropHelper(window).Handle;
            hwndSource = HwndSource.FromHwnd(handle);
            hwndSource.AddHook(new HwndSourceHook(this.WindowProc));
        }

        /// <summary>
        /// Process message from the win32 window message queue
        /// </summary>
        private IntPtr WindowProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                case WM_SIZING:
                    RECT bounds = (RECT)Marshal.PtrToStructure(lParam, typeof(RECT));

                    int width = bounds.right - bounds.left;
                    int height = bounds.bottom - bounds.top;

                    switch (wParam.ToInt32())
                    {
                        case WMSZ_BOTTOM:
                            if (height > SnappingThresholdHeight)
                                bounds.bottom = bounds.top + ((int)((double)height / (double)SnappingIncrementHeight) * SnappingIncrementHeight) + SnappingHeightOffset;
                            break;
                        case WMSZ_BOTTOMLEFT:
                            if (height > SnappingThresholdHeight)
                                bounds.bottom = bounds.top + ((int)((double)height / (double)SnappingIncrementHeight) * SnappingIncrementHeight) + SnappingHeightOffset;
                            if (width > SnappingThresholdWidth)
                                bounds.left = bounds.right - ((int)((double)width / (double)SnappingIncrementWidth) * SnappingIncrementWidth) + SnappingWidthOffset;
                            break;
                        case WMSZ_BOTTOMRIGHT:
                            if (height > SnappingThresholdHeight)
                                bounds.bottom = bounds.top + ((int)((double)height / (double)SnappingIncrementHeight) * SnappingIncrementHeight) + SnappingHeightOffset;
                            if (width > SnappingThresholdWidth)
                                bounds.right = bounds.left + ((int)((double)width / (double)SnappingIncrementWidth) * SnappingIncrementWidth) + SnappingWidthOffset;
                            break;
                        case WMSZ_LEFT:
                            if (width > SnappingThresholdWidth)
                                bounds.left = bounds.right - ((int)((double)width / (double)SnappingIncrementWidth) * SnappingIncrementWidth) + SnappingWidthOffset;
                            break;
                        case WMSZ_RIGHT:
                            if (width > SnappingThresholdWidth)
                                bounds.right = bounds.left + ((int)((double)width / (double)SnappingIncrementWidth) * SnappingIncrementWidth) + SnappingWidthOffset;
                            break;
                        case WMSZ_TOP:
                            if (height > SnappingThresholdHeight)
                                bounds.top = bounds.bottom - ((int)((double)height / (double)SnappingIncrementHeight) * SnappingIncrementHeight) + SnappingHeightOffset;
                            break;
                        case WMSZ_TOPLEFT:
                            if (width > SnappingThresholdWidth)
                                bounds.left = bounds.right - ((int)((double)width / (double)SnappingIncrementWidth) * SnappingIncrementWidth) + SnappingWidthOffset;
                            if (height > SnappingThresholdHeight)
                                bounds.top = bounds.bottom - ((int)((double)height / (double)SnappingIncrementHeight) * SnappingIncrementHeight) + SnappingHeightOffset;
                            break;
                        case WMSZ_TOPRIGHT:
                            if (width > SnappingThresholdWidth)
                                bounds.right = bounds.left + ((int)((double)width / (double)SnappingIncrementWidth) * SnappingIncrementWidth) + SnappingWidthOffset;
                            if (height > SnappingThresholdHeight)
                                bounds.top = bounds.bottom - ((int)((double)height / (double)SnappingIncrementHeight) * SnappingIncrementHeight) + SnappingHeightOffset;
                            break;

                    }
                    Marshal.StructureToPtr(bounds, lParam, false);
                    break;
            }

            return IntPtr.Zero;
        }

        /// <summary>
        /// Force a resize of the window in a particular direction
        /// </summary>
        /// <param name="direction">The direction to begin resizing</param>
        private void ResizeWindow(ResizeDirection direction)
        {
            SendMessage(hwndSource.Handle, 0x112, (IntPtr)direction, IntPtr.Zero);
        }

        /// <summary>
        /// Forces a resize of the window in a particular direction, based on the sender
        /// </summary>
        private void ManualResize(object sender, MouseButtonEventArgs e)
        {
            if (sender == this.verticalResizeElement)
                ResizeWindow(ResizeDirection.Bottom);
            else
                ResizeWindow(ResizeDirection.Right);
        }
    }
}

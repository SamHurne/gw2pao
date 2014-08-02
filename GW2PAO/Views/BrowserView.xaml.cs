using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Awesomium.Core;
using Awesomium.Windows.Controls;
using NLog;

namespace GW2PAO.Views
{
    /// <summary>
    /// Interaction logic for BrowserView.xaml
    /// </summary>
    public partial class BrowserView : OverlayWindow
    {
        private static readonly DependencyPropertyKey NativeViewPropertyKey = DependencyProperty.RegisterReadOnly("NativeView", typeof(IntPtr), typeof(BrowserView), new FrameworkPropertyMetadata(IntPtr.Zero));
        private static readonly DependencyPropertyKey IsRegularWindowPropertyKey = DependencyProperty.RegisterReadOnly("IsRegularWindow", typeof(bool), typeof(BrowserView), new FrameworkPropertyMetadata(true));

        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register("Source", typeof(Uri), typeof(BrowserView), new FrameworkPropertyMetadata(null));
        public static readonly DependencyProperty NativeViewProperty = NativeViewPropertyKey.DependencyProperty;
        public static readonly DependencyProperty IsRegularWindowProperty = IsRegularWindowPropertyKey.DependencyProperty;

        /// <summary>
        /// Default logger
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// True if the user is resizing the window, else false
        /// </summary>
        private bool resizeInProcess = false;

        public Uri Source
        {
            get { return (Uri)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        public IntPtr NativeView
        {
            get { return (IntPtr)GetValue(NativeViewProperty); }
            private set { this.SetValue(BrowserView.NativeViewPropertyKey, value); }
        }

        public bool IsRegularWindow
        {
            get { return (bool)GetValue(IsRegularWindowProperty); }
            private set { this.SetValue(BrowserView.IsRegularWindowPropertyKey, value); }
        }

        public BrowserView()
        {
            InitializeComponent();

            webControl.ShowCreatedWebView += webControl_ShowCreatedWebView;
            this.Closed += BrowserView_Closed;

            this.Source = WebCore.Configuration.HomeURL;
        }

        public BrowserView(IntPtr nativeView)
        {
            InitializeComponent();

            // Always handle ShowCreatedWebView. This is fired for
            // links and forms with |target="_blank"| or for JavaScript
            // 'window.open' calls.
            webControl.ShowCreatedWebView += webControl_ShowCreatedWebView;
            // For popups, you usually want to handle WindowClose,
            // fired when the page calls 'window.close'.
            webControl.WindowClose += webControl_WindowClose;
            // Tell the WebControl that it should wrap a created child view.
            this.NativeView = nativeView;
            // This window will host a WebControl that is the result of 
            // JavaScript 'window.open'. Hide the address and status bar.
            this.IsRegularWindow = false;
        }

        public BrowserView(Uri url)
        {
            InitializeComponent();

            // Always handle ShowCreatedWebView. This is fired for
            // links and forms with |target="_blank"| or for JavaScript
            // 'window.open' calls.
            webControl.ShowCreatedWebView += webControl_ShowCreatedWebView;
            // For popups, you usually want to handle WindowClose,
            // fired when the page calls 'window.close'.
            webControl.WindowClose += webControl_WindowClose;
            // Tell the WebControl to load a specified target URL.
            this.Source = url;
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            // Destroy the WebControl and its underlying view.
            webControl.Dispose();
        }

        private void webControl_ShowCreatedWebView(object sender, ShowCreatedWebViewEventArgs e)
        {
            if (webControl == null)
                return;

            if (!webControl.IsLive)
                return;

            // An instance of our application's web window, 
            // that will host the new view instance, either 
            // we wrap the created child view, or we let the 
            // WebControl create a new underlying web-view.
            BrowserView newWindow;

            // Treat popups differently. If IsPopup is true, 
            // the event is always the result of 'window.open' 
            // (IsWindowOpen is also true, so no need to check it).
            // Our application does not recognize user defined, 
            // non-standard specs. Therefore child views opened 
            // with non-standard specs, will not be presented as 
            // popups but as regular new windows (still wrapping 
            // the child view however -- see below).
            if (e.IsPopup && !e.IsUserSpecsOnly)
            {
                // JSWindowOpenSpecs.InitialPosition indicates screen coordinates.
                Int32Rect screenRect = e.Specs.InitialPosition.GetInt32Rect();

                // Set the created native view as the underlying view of the
                // WebControl. This will maintain the relationship between
                // the parent view and the child, usually required when the 
                // new view is the result of 'window.open' (JS can access 
                // the parent window through 'window.opener'; the parent window 
                // can manipulate the child through the 'window' object returned 
                // from the 'window.open' call).
                newWindow = new BrowserView(e.NewViewInstance);
                // Do not show in the taskbar.
                newWindow.ShowInTaskbar = false;
                // Set a border-style to indicate a popup.
                newWindow.WindowStyle = WindowStyle.None;
                // Set resizing mode depending on the indicated specs.
                newWindow.ResizeMode = e.Specs.Resizable ? ResizeMode.CanResizeWithGrip : ResizeMode.NoResize;

                // If the caller has not indicated a valid size for the 
                // new popup window, let it be opened with the default 
                // size specified at design time.
                if ((screenRect.Width > 0) && (screenRect.Height > 0))
                {
                    // The indicated size, is client size.
                    double horizontalBorderHeight = SystemParameters.ResizeFrameHorizontalBorderHeight;
                    double verticalBorderWidth = SystemParameters.ResizeFrameVerticalBorderWidth;
                    double captionHeight = SystemParameters.CaptionHeight;

                    // Set the indicated size.
                    newWindow.Width = screenRect.Width + (verticalBorderWidth * 2);
                    newWindow.Height = screenRect.Height + captionHeight + (horizontalBorderHeight * 2);
                }

                // Show the window.
                newWindow.Show();

                // If the caller has not indicated a valid position for 
                // the new popup window, let it be opened in the default 
                // position specified at design time.
                if ((screenRect.Y > 0) && (screenRect.X > 0))
                {
                    // Move it to the indicated coordinates.
                    newWindow.Top = screenRect.Y;
                    newWindow.Left = screenRect.X;
                }
            }
            else if (e.IsWindowOpen || e.IsPost)
            {
                // No specs or only non-standard specs were specified, 
                // but the event is still the result of 'window.open' 
                // or of an HTML form with tagret="_blank" and method="post".
                // We will open a normal window but we will still wrap 
                // the new native child view, maintaining its relationship 
                // with the parent window.
                newWindow = new BrowserView(e.NewViewInstance);
                // Show the window.
                newWindow.Show();
            }
            else
            {
                // The event is not the result of 'window.open' or of an 
                // HTML form with tagret="_blank" and method="post"., 
                // therefore it's most probably the result of a link with 
                // target='_blank'. We will not be wrapping the created view; 
                // we let the WebControl hosted in MainWindow create its own 
                // underlying view. Setting Cancel to true tells the core 
                // to destroy the created child view.
                //
                // Why don't we always wrap the native view passed to 
                // ShowCreatedWebView?
                //
                // - In order to maintain the relationship with their parent 
                // view, child views execute and render under the same process 
                // (awesomium_process) as their parent view. If for any reason 
                // this child process crashes, all views related to it will be 
                // affected. When maintaining a parent-child relationship is not 
                // important, we prefer taking advantage of the isolated process 
                // architecture of Awesomium and let each view be rendered in 
                // a separate process.
                e.Cancel = true;
                // Note that we only explicitly navigate to the target URL, 
                // when a new view is about to be created, not when we wrap the 
                // created child view. This is because navigation to the target 
                // URL (if any), is already queued on created child views. 
                // We must not interrupt this navigation as we would still be 
                // breaking the parent-child relationship.
                newWindow = new BrowserView(e.TargetURL);
                // Show the window.
                newWindow.Show();
            }
        }

        private void BrowserView_Closed(object sender, EventArgs e)
        {
            webControl.Dispose();
            logger.Debug("BrowserView closed");
        }

        private void webControl_WindowClose(object sender, WindowCloseEventArgs e)
        {
            // The page called 'window.close'. If the call
            // comes from a frame, ignore it.
            if (!e.IsCalledFromFrame)
                this.Close();
        }

        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void CloseWindowButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void MinimizeWindowButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void Resize_Init(object sender, MouseButtonEventArgs e)
        {
            Grid senderRect = sender as Grid;

            if (senderRect != null)
            {
                resizeInProcess = true;
                senderRect.CaptureMouse();
            }
        }

        private void Resize_End(object sender, MouseButtonEventArgs e)
        {
            Grid senderRect = sender as Grid;
            if (senderRect != null)
            {
                resizeInProcess = false;
                senderRect.ReleaseMouseCapture();
            }
        }

        private void Resizeing_Form(object sender, MouseEventArgs e)
        {
            if (resizeInProcess)
            {
                Grid senderRect = sender as Grid;
                if (senderRect != null)
                {
                    double width = e.GetPosition(this).X;
                    double height = e.GetPosition(this).Y;
                    senderRect.CaptureMouse();
                    if (senderRect.Name == "ResizeWidth")
                    {
                        width += 1;
                        if (width > 0 && width < this.MaxWidth && width > this.MinWidth)
                        {
                            this.Width = width;
                        }
                    }
                    else if (senderRect.Name == "ResizeHeight")
                    {
                        height += 1;
                        if (height > 0 && height < this.MaxHeight && height > this.MinHeight)
                        {
                            this.Height = height;
                        }
                    }
                }
            }
        }
    }
}

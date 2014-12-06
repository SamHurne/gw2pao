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
using System.Windows.Navigation;
using System.Windows.Shapes;
using GW2PAO.Infrastructure;

namespace GW2PAO.Views
{
    /// <summary>
    /// Interaction logic for HotkeyUserControl.xaml
    /// </summary>
    public partial class HotkeyUserControl : UserControl
    {
        /// <summary>
        /// Dependency property for Label
        /// </summary>
        public static readonly DependencyProperty LabelProperty =
            DependencyProperty.Register("Label",
            typeof(string),
            typeof(HotkeyUserControl),
            new FrameworkPropertyMetadata(string.Empty));

        /// <summary>
        /// Label to use for this hotkey user control
        /// </summary>
        public string Label
        {
            get { return (string)GetValue(LabelProperty); }
            set { SetValue(LabelProperty, value); }
        }

        /// <summary>
        /// Dependency property for Hotkey
        /// </summary>
        public static readonly DependencyProperty HotkeyProperty =
            DependencyProperty.Register("Hotkey",
            typeof(Hotkey),
            typeof(HotkeyUserControl),
            new FrameworkPropertyMetadata(null));

        /// <summary>
        /// The hotkey configured by this user control
        /// </summary>
        public Hotkey Hotkey
        {
            get { return (Hotkey)GetValue(HotkeyProperty); }
            set { SetValue(HotkeyProperty, value); }
        }

        public HotkeyUserControl()
        {
            InitializeComponent();
            this.LayoutRoot.DataContext = this;
        }

        private void EntryBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (this.Hotkey != null)
            {
                if (e.Key != Key.LeftShift
                    && e.Key != Key.RightShift
                    && e.Key != Key.LeftCtrl
                    && e.Key != Key.RightCtrl
                    && e.Key != Key.LeftAlt
                    && e.Key != Key.RightAlt
                    && e.Key != Key.LWin
                    && e.Key != Key.RWin)
                {
                    this.Hotkey.Key = e.Key;
                    this.Hotkey.Shift = e.KeyboardDevice.IsKeyDown(Key.LeftShift) || e.KeyboardDevice.IsKeyDown(Key.RightShift);
                    this.Hotkey.Control = e.KeyboardDevice.IsKeyDown(Key.LeftCtrl) || e.KeyboardDevice.IsKeyDown(Key.RightCtrl);
                    this.Hotkey.Alt = e.KeyboardDevice.IsKeyDown(Key.LeftAlt) || e.KeyboardDevice.IsKeyDown(Key.RightAlt);
                    this.Hotkey.Windows = e.KeyboardDevice.IsKeyDown(Key.LWin) || e.KeyboardDevice.IsKeyDown(Key.RWin);
                }
            }

            this.EntryBox.GetBindingExpression(TextBox.TextProperty).UpdateTarget();
            e.Handled = true;
        }

        private void EntryBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (this.Hotkey != null)
            {
                if (e.Key == Key.Back || e.Key == Key.Delete)
                {
                    this.Hotkey.Key = Key.None;
                }
            }

            this.EntryBox.GetBindingExpression(TextBox.TextProperty).UpdateTarget();
            e.Handled = true;
        }

        private void EntryBox_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            ((TextBox)sender).SelectAll();
        }

        private void EntryBox_GotMouseCapture(object sender, MouseEventArgs e)
        {
            ((TextBox)sender).SelectAll();
        }
    }
}

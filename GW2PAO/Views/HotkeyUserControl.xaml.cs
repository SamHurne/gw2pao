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
using GW2PAO.Infrastructure.Hotkeys;
using GW2PAO.Infrastructure.Hotkeys.Interfaces;

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

        /// <summary>
        /// Dependency property for HotkeyManager
        /// </summary>
        public static readonly DependencyProperty HotkeyManagerProperty =
            DependencyProperty.Register("HotkeyManager",
            typeof(IHotkeyManager),
            typeof(HotkeyUserControl),
            new FrameworkPropertyMetadata(null));

        public IHotkeyManager HotkeyManager
        {
            get { return (IHotkeyManager)GetValue(HotkeyManagerProperty); }
            set { SetValue(HotkeyManagerProperty, value); }
        }

        public HotkeyUserControl()
        {
            InitializeComponent();
            this.LayoutRoot.DataContext = this;
            this.WarningTextBlock.Visibility = Visibility.Hidden;
        }

        private void EntryBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (this.HotkeyManager != null)
            {
                if (e.Key != Key.LeftShift
                    && e.Key != Key.RightShift
                    && e.Key != Key.LeftCtrl
                    && e.Key != Key.RightCtrl
                    && e.Key != Key.LeftAlt
                    && e.Key != Key.RightAlt
                    && e.Key != Key.LWin
                    && e.Key != Key.RWin
                    && e.Key != Key.Escape)
                {
                    // Before we go and apply the hotkey, first try it and make sure it will work
                    this.WarningTextBlock.Visibility = Visibility.Hidden;

                    var modifiers = KeyModifier.None;
                    if (e.KeyboardDevice.IsKeyDown(Key.LeftShift) || e.KeyboardDevice.IsKeyDown(Key.RightShift))
                        modifiers |= KeyModifier.Shift;
                    if (e.KeyboardDevice.IsKeyDown(Key.LeftCtrl) || e.KeyboardDevice.IsKeyDown(Key.RightCtrl))
                        modifiers |= KeyModifier.Ctrl;
                    if (e.KeyboardDevice.IsKeyDown(Key.LeftAlt) || e.KeyboardDevice.IsKeyDown(Key.RightAlt))
                        modifiers |= KeyModifier.Alt;
                    if (e.KeyboardDevice.IsKeyDown(Key.LWin) || e.KeyboardDevice.IsKeyDown(Key.RWin))
                        modifiers |= KeyModifier.Win;

                    var newHotkey = new Hotkey(e.Key, modifiers);

                    if (this.HotkeyManager.CanRegister(newHotkey))
                    {
                        // Valid, go ahead and set it
                        this.HotkeyManager.Unregister(this.Hotkey);
                        this.Hotkey.Key = e.Key;
                        this.Hotkey.KeyModifiers = modifiers;
                        this.HotkeyManager.Register(this.Hotkey);
                    }
                    else
                    {
                        // Invalid key combo
                        this.WarningTextBlock.Visibility = Visibility.Visible;
                    }
                }
                else if (e.Key == Key.Escape)
                {
                    // Remove focus
                    this.HiddenBox.Focus();
                }
            }

            this.EntryBox.GetBindingExpression(TextBox.TextProperty).UpdateTarget();
            e.Handled = true;
        }

        private void EntryBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (this.HotkeyManager != null)
            {
                if (e.Key == Key.Back || e.Key == Key.Delete)
                {
                    this.HotkeyManager.Unregister(this.Hotkey);
                    this.Hotkey.Key = Key.None;
                    this.Hotkey.KeyModifiers = KeyModifier.None;
                }
            }

            this.EntryBox.GetBindingExpression(TextBox.TextProperty).UpdateTarget();

            // Remove focus
            this.HiddenBox.Focus();

            e.Handled = true;
        }

        private void EntryBox_GotFocus(object sender, RoutedEventArgs e)
        {
            ((TextBox)sender).SelectAll();
            HotkeyCommands.PauseHotkeys.Execute(null);
        }

        private void EntryBox_LostFocus(object sender, RoutedEventArgs e)
        {
            HotkeyCommands.ResumeHotkeys.Execute(null);
        }
    }
}

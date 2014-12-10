using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml.Serialization;
using Microsoft.Practices.Prism.Mvvm;
using NHotkey;
using NHotkey.Wpf;
using NLog;

namespace GW2PAO.Infrastructure
{
    public class Hotkey : BindableBase
    {
        /// <summary>
        /// Default logger
        /// </summary>
        [XmlIgnore]
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        [XmlIgnore]
        private string id = Guid.NewGuid().ToString();

        [XmlIgnore]
        private bool isRegistered;

        private Key key;
        private bool shift;
        private bool control;
        private bool alt;
        private bool windows;
        private bool isEnabled;

        /// <summary>
        /// The actual key to use as the hotkey
        /// </summary>
        public Key Key
        {
            get { return this.key; }
            set { this.SetProperty(ref this.key, value); }
        }

        /// <summary>
        /// True if the Shift modifier key should be used, else false
        /// </summary>
        public bool Shift
        {
            get { return this.shift; }
            set { this.SetProperty(ref this.shift, value); }
        }

        /// <summary>
        /// True if the Control modifier key should be used, else false
        /// </summary>
        public bool Control
        {
            get { return this.control; }
            set { this.SetProperty(ref this.control, value); }
        }

        /// <summary>
        /// True if the Alt modifier key should be used, else false
        /// </summary>
        public bool Alt
        {
            get { return this.alt; }
            set { this.SetProperty(ref this.alt, value); }
        }

        /// <summary>
        /// True if the Windows modifier key should be used, else false
        /// </summary>
        public bool Windows
        {
            get { return this.windows; }
            set { this.SetProperty(ref this.windows, value); }
        }

        /// <summary>
        /// True if the hotkey does not have a registered key, else false
        /// </summary>
        public bool Empty
        {
            get { return this.Key == Key.None; }
        }

        /// <summary>
        /// True if the hotkey is registered, else false
        /// This is the primary property used for enabling/disabling the hotkey
        /// </summary>
        public bool IsEnabled
        {
            get { return this.isEnabled; }
            set
            {
                if (this.SetProperty(ref this.isEnabled, value))
                {
                    if (value)
                        this.Register(); // User-Enabled
                    else
                        this.Unregister(); // User-Disabled
                }
            }
        }

        /// <summary>
        /// Event raised when the hotkey with modifiers (if any) is pressed
        /// </summary>
        public event EventHandler Pressed;

        /// <summary>
        /// Event raised when a hotkey is already registered
        /// </summary>
        public event EventHandler AlreadyRegistered;

        /// <summary>
        /// Default constructor
        /// </summary>
        public Hotkey()
            : this(Key.None, false, false, false, false)
        {
        }

        /// <summary>
        /// Constructs a new hotkey
        /// </summary>
        /// <param name="key">The key to use for the hotkey</param>
        /// <param name="shift">True if the Shift modifier key should be used, else false</param>
        /// <param name="control">True if the Control modifier key should be used, else false</param>
        /// <param name="alt">True if the Alt modifier key should be used, else false</param>
        /// <param name="windows">True if the Windows modifier key should be used, else false</param>
        public Hotkey(Key key, bool shift, bool control, bool alt, bool windows)
        {
            // Assign properties
            this.Key = key;
            this.Shift = shift;
            this.Control = control;
            this.Alt = alt;
            this.Windows = windows;
        }

        /// <summary>
        /// Performs a refresh with the current hotkey configuration, including key and all modifiers
        /// </summary>
        /// <remarks>
        /// This should be called anytime the hotkey changes
        /// </remarks>
        public void Refresh()
        {
            this.Unregister();
            this.Register();
        }

        /// <summary>
        /// Registers the hotkey with modifies (if any)
        /// </summary>
        private void Register()
        {
            if (this.isRegistered)
                return; // Do nothing if we are already registered

            if (this.Key == Key.None)
                return; // Do nothing if the hotkey is "none"

            ModifierKeys modifiers = ModifierKeys.None;
            if (this.Shift)
                modifiers |= ModifierKeys.Shift;
            if (this.Control)
                modifiers |= ModifierKeys.Control;
            if (this.Alt)
                modifiers |= ModifierKeys.Alt;
            if (this.Windows)
                modifiers |= ModifierKeys.Windows;

            logger.Trace("Registering hotkey {0} | {1} | {2}", this.id, this.Key, modifiers);
            try
            {
                HotkeyManager.Current.AddOrReplace(this.id, this.Key, modifiers, this.OnPressed);
            }
            catch (HotkeyAlreadyRegisteredException)
            {
                // Key is already registered, so reset
                this.Key = Key.None;
                this.Alt = false;
                this.Control = false;
                this.Shift = false;
                this.Windows = false;

                this.isRegistered = false;
                this.OnHotkeyAlreadyRegistered(this, new HotkeyAlreadyRegisteredEventArgs(this.ToString()));
            }
        }

        /// <summary>
        /// Unregisters the hotkey
        /// </summary>
        private void Unregister()
        {
            if (this.isRegistered)
            {
                logger.Trace("Unregistering hotkey {0}", this.id);
                HotkeyManager.Current.Remove(this.id);
                this.IsEnabled = false;
            }
        }

        private void OnHotkeyAlreadyRegistered(object sender, HotkeyAlreadyRegisteredEventArgs e)
        {
            logger.Trace("{0} hotkey already registered", e.Name);
            if (this.AlreadyRegistered != null)
            {
                this.AlreadyRegistered(this, e);
            }
        }

        private void OnPressed(object o, HotkeyEventArgs e)
        {
            logger.Trace("{0} hotkey press detected", this.id);
            if (this.Pressed != null)
            {
                this.Pressed(this, e);
                e.Handled = false;
            }
        }

        public override string ToString()
        {
            // We can be empty
            if (this.Empty)
                return string.Empty;

            // Build key name
            string keyName = Enum.GetName(typeof(Key), this.Key); ;
            switch (this.Key)
            {
                case Key.Oem1:
                    keyName = "OemSemicolon";
                    break;
                case Key.Oem3:
                    keyName = "OemTilde";
                    break;
                case Key.Oem5:
                    keyName = "OemPipe";
                    break;
                case Key.Oem6:
                    keyName = "OemCloseBrackets";
                    break;
                case Key.Oem7:
                    keyName = "OemQuotes";
                    break;
                case Key.DeadCharProcessed:
                    keyName = "OemClear";
                    break;
                case Key.Next:
                    keyName = "PageDown";
                    break;
                case Key.Capital:
                    keyName = "CapsLock";
                    break;
                case Key.D0:
                case Key.D1:
                case Key.D2:
                case Key.D3:
                case Key.D4:
                case Key.D5:
                case Key.D6:
                case Key.D7:
                case Key.D8:
                case Key.D9:
                    // Strip the first character
                    keyName = keyName.Substring(1);
                    break;
                default:
                    // Leave everything alone
                    break;
            }

            // Strip "Oem" from the keyName if it is there
            if (keyName.StartsWith("Oem"))
                keyName = keyName.Substring(3);

            // Build modifiers
            string modifiers = string.Empty;
            if (this.shift)
                modifiers += "Shift + ";
            if (this.control)
                modifiers += "Ctrl + ";
            if (this.alt)
                modifiers += "Alt + ";
            if (this.windows)
                modifiers += "Win + ";

            // Return result
            return modifiers + keyName;
        }

        /// <summary>
        /// Tests to see if the given combination of keys is available as a hotkey
        /// </summary>
        /// <param name="key">The key to use for the hotkey</param>
        /// <param name="shift">True if the Shift modifier key should be used, else false</param>
        /// <param name="control">True if the Control modifier key should be used, else false</param>
        /// <param name="alt">True if the Alt modifier key should be used, else false</param>
        /// <param name="windows">True if the Windows modifier key should be used, else false</param>
        /// <returns>True if the given combination is valid, else false</returns>
        public static bool CanSet(Key key, bool shift, bool control, bool alt, bool windows)
        {
            if (key == Key.None)
                return false; // Do nothing if the hotkey is "none"

            ModifierKeys modifiers = ModifierKeys.None;
            if (shift)
                modifiers |= ModifierKeys.Shift;
            if (control)
                modifiers |= ModifierKeys.Control;
            if (alt)
                modifiers |= ModifierKeys.Alt;
            if (windows)
                modifiers |= ModifierKeys.Windows;

            var id = Guid.NewGuid().ToString();
            logger.Trace("Testing hotkey {0} | {1}", key, modifiers);
            try
            {
                HotkeyManager.Current.AddOrReplace(id, key, modifiers, (o, e) => { });
                HotkeyManager.Current.Remove(id);

                // Valid
                return true;
            }
            catch (HotkeyAlreadyRegisteredException)
            {
                // Not valid
                return false;
            }
        }
    }
}

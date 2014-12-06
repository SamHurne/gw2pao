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

        private Key key;
        private bool shift;
        private bool control;
        private bool alt;
        private bool windows;
        private bool isRegistered;

        /// <summary>
        /// The actual key to use as the hotkey
        /// </summary>
        public Key Key
        {
            get { return this.key; }
            set
            {
                if (this.key != value)
                {
                    this.key = value;
                    if (this.IsRegistered)
                        this.Reregister();
                }
            }
        }

        /// <summary>
        /// True if the Shift modifier key should be used, else false
        /// </summary>
        public bool Shift
        {
            get { return this.shift; }
            set
            {
                if (this.shift != value)
                {
                    this.shift = value;
                    if (this.IsRegistered)
                        this.Reregister();
                }
            }
        }

        /// <summary>
        /// True if the Control modifier key should be used, else false
        /// </summary>
        public bool Control
        {
            get { return this.control; }
            set
            {
                if (this.control != value)
                {
                    this.control = value;
                    if (this.IsRegistered)
                        this.Reregister();
                }
            }
        }

        /// <summary>
        /// True if the Alt modifier key should be used, else false
        /// </summary>
        public bool Alt
        {
            get { return this.alt; }
            set
            {
                if (this.alt != value)
                {
                    this.alt = value;
                    if (this.IsRegistered)
                        this.Reregister();
                }
            }
        }

        /// <summary>
        /// True if the Windows modifier key should be used, else false
        /// </summary>
        public bool Windows
        {
            get { return this.windows; }
            set
            {
                if (this.windows != value)
                {
                    this.windows = value;
                    if (this.IsRegistered)
                        this.Reregister();
                }
            }
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
        public bool IsRegistered
        {
            get { return this.isRegistered; }
            set
            {
                if (this.isRegistered != value)
                {
                    if (value)
                        this.Register();
                    else
                        this.Unregister();
                    this.OnPropertyChanged(() => this.IsRegistered);
                }
            }
        }

        /// <summary>
        /// Event raised when the hotkey with modifiers (if any) is pressed
        /// </summary>
        public event EventHandler Pressed;

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
        /// Registers the hotkey with modifies (if any)
        /// </summary>
        private void Register()
        {
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
            HotkeyManager.Current.AddOrReplace(this.id, this.Key, modifiers, this.OnPressed);
            this.isRegistered = true;
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
                this.isRegistered = false;
            }
        }

        private void Reregister()
        {
            this.Unregister();
            this.Register();
        }

        private void OnPressed(object o, HotkeyEventArgs e)
        {
            logger.Trace("{0} hotkey press detected", this.id);
            if (this.Pressed != null)
            {
                this.Pressed(this, e);
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
    }
}

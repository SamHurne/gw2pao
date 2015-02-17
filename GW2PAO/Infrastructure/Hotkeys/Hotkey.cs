using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml.Serialization;
using Microsoft.Practices.Prism.Mvvm;
using Newtonsoft.Json;

namespace GW2PAO.Infrastructure.Hotkeys
{
    [Serializable]
    public class Hotkey : BindableBase
    {
        private Key key;
        private KeyModifier keyModifiers;

        /// <summary>
        /// The hotkey key
        /// </summary>
        public Key Key
        {
            get { return this.key; }
            set { SetProperty(ref this.key, value); }
        }

        /// <summary>
        /// Modifier for the hotkey, such as ctrl, alt, etc
        /// </summary>
        public KeyModifier KeyModifiers
        {
            get { return this.keyModifiers; }
            set { SetProperty(ref this.keyModifiers, value); }
        }

        /// <summary>
        /// ID for the hotkey, based on key and keymodifier
        /// </summary>
        [JsonIgnore, XmlIgnore]
        public int KeyId
        {
            get { return this.VirtualKeyCode + ((int)this.KeyModifiers * 0x10000); }
        }

        /// <summary>
        /// Virtual key code used when registering the hotkey
        /// </summary>
        [JsonIgnore, XmlIgnore]
        public int VirtualKeyCode
        {
            get { return KeyInterop.VirtualKeyFromKey(this.Key); }
        }

        /// <summary>
        /// Event raised when the hotkey has been pressed
        /// </summary>
        [JsonIgnore, XmlIgnore]
        public EventHandler Pressed;

        /// <summary>
        /// Parameter-less constructor for serialization purposes
        /// </summary>
        protected Hotkey()
        {
        }

        /// <summary>
        /// Constructs a new hotkey
        /// </summary>
        public Hotkey(Key k, KeyModifier keyModifiers)
        {
            this.Key = k;
            this.KeyModifiers = keyModifiers;
        }

        /// <summary>
        /// Raises the Pressed event
        /// </summary>
        internal void RaisePressed()
        {
            if (this.Pressed != null)
                this.Pressed(this, new EventArgs());
        }

        /// <summary>
        /// Formats the hotkey data for display
        /// </summary>
        public override string ToString()
        {
            // We can be empty
            if (this.Key == Key.None)
                return string.Empty;

            // Build key name
            string keyName = Enum.GetName(typeof(Key), this.Key);
            switch (this.Key)
            {
                case Key.Oem1:
                    keyName = ";";
                    break;
                case Key.Oem3:
                    keyName = "~";
                    break;
                case Key.Oem5:
                    keyName = "|";
                    break;
                case Key.Oem6:
                    keyName = "]";
                    break;
                case Key.Oem7:
                    keyName = "\"";
                    break;
                case Key.DeadCharProcessed:
                    keyName = "Clear";
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
            if (this.KeyModifiers.HasFlag(KeyModifier.Shift))
                modifiers += "Shift + ";
            if (this.KeyModifiers.HasFlag(KeyModifier.Ctrl))
                modifiers += "Ctrl + ";
            if (this.KeyModifiers.HasFlag(KeyModifier.Alt))
                modifiers += "Alt + ";
            if (this.KeyModifiers.HasFlag(KeyModifier.Win))
                modifiers += "Win + ";

            // Return result
            return modifiers + keyName;
        }
    }
}

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Interop;
using GW2PAO.Infrastructure.Hotkeys.Interfaces;

namespace GW2PAO.Infrastructure.Hotkeys
{
    [Export(typeof(IHotkeyManager))]
    public class GlobalHotkeyManager : IHotkeyManager, IDisposable
    {
        /// <summary>
        /// Collection of registered hotkeys
        /// </summary>
        private ConcurrentDictionary<int, Hotkey> RegisteredHotkeys;

        /// <summary>
        /// True if this global hotkeys instance has been disposed, else false
        /// </summary>
        private bool disposed;

        /// <summary>
        /// True if all hotkeys are paused, else false
        /// </summary>
        private bool isPaused;

        /// <summary>
        /// Default constructor
        /// </summary>
        public GlobalHotkeyManager()
        {
            this.RegisteredHotkeys = new ConcurrentDictionary<int, Hotkey>();
            ComponentDispatcher.ThreadFilterMessage += new ThreadMessageEventHandler(ComponentDispatcherThreadFilterMessage);
        }

        /// <summary>
        /// Registers a hotkey
        /// </summary>
        /// <returns>True if successful, else false</returns>
        public bool Register(Hotkey hotkey)
        {
            bool result = User32.RegisterHotKey(IntPtr.Zero, hotkey.KeyId, (UInt32)hotkey.KeyModifiers, (UInt32)hotkey.VirtualKeyCode);
            if (result)
                result = RegisteredHotkeys.TryAdd(hotkey.KeyId, hotkey);

            if (isPaused)
                User32.UnregisterHotKey(IntPtr.Zero, hotkey.KeyId);

            return result;
        }

        /// <summary>
        /// Unregisters a hotkey
        /// </summary>
        /// <returns>True if successful, else false</returns>
        public bool Unregister(Hotkey hotkey)
        {
            bool result = false;
            Hotkey hotKey;
            if (RegisteredHotkeys.TryRemove(hotkey.KeyId, out hotKey))
            {
                result = User32.UnregisterHotKey(IntPtr.Zero, hotkey.KeyId);
            }
            return result;
        }

        /// <summary>
        /// </summary>
        /// <returns>True if the given hotkey has already been registered, else false</returns>
        public bool IsRegistered(Hotkey hotkey)
        {
            return this.RegisteredHotkeys.ContainsKey(hotkey.KeyId);
        }

        /// <summary>
        /// </summary>
        /// <returns>True if the given hotkey can be registered, else false</returns>
        public bool CanRegister(Hotkey hotkey)
        {
            bool result;
            if (this.IsRegistered(hotkey))
            {
                // Can't register a hotkey that is already registered
                result = false;
            }
            else
            {
                result = User32.RegisterHotKey(IntPtr.Zero, hotkey.KeyId, (UInt32)hotkey.KeyModifiers, (UInt32)hotkey.VirtualKeyCode);
                User32.UnregisterHotKey(IntPtr.Zero, hotkey.KeyId);
            }
            return result;
        }

        /// <summary>
        /// Pauses all hotkeys and turns off handling of all hotkeys
        /// </summary>
        public void PauseHotkeys()
        {
            if (!isPaused)
            {
                isPaused = true;
                foreach (var hotkey in this.RegisteredHotkeys.Values)
                    User32.UnregisterHotKey(IntPtr.Zero, hotkey.KeyId);
            }
        }

        /// <summary>
        /// Resumes handling for all hotkeys
        /// </summary>
        public void ResumeHotkeys()
        {
            if (isPaused)
            {
                isPaused = false;
                foreach (var hotkey in this.RegisteredHotkeys.Values)
                    User32.RegisterHotKey(IntPtr.Zero, hotkey.KeyId, (UInt32)hotkey.KeyModifiers, (UInt32)hotkey.VirtualKeyCode);
            }
        }

        /// <summary>
        /// Component dispatcher thread filter
        /// </summary>
        private void ComponentDispatcherThreadFilterMessage(ref MSG msg, ref bool handled)
        {
            if (!handled)
            {
                if (msg.message == User32.WmHotKey)
                {
                    Hotkey hotkey;

                    if (RegisteredHotkeys.TryGetValue((int)msg.wParam, out hotkey))
                    {
                        if (hotkey != null)
                            hotkey.RaisePressed();
                        handled = true;
                    }
                }
            }
        }

        #region IDisposable

        // ******************************************************************
        // Implement IDisposable.
        // Do not make this method virtual.
        // A derived class should not be able to override this method.
        public void Dispose()
        {
            Dispose(true);
            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.SupressFinalize to
            // take this object off the finalization queue
            // and prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        // ******************************************************************
        // Dispose(bool disposing) executes in two distinct scenarios.
        // If disposing equals true, the method has been called directly
        // or indirectly by a user's code. Managed and unmanaged resources
        // can be _disposed.
        // If disposing equals false, the method has been called by the
        // runtime from inside the finalizer and you should not reference
        // other objects. Only unmanaged resources can be _disposed.
        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!this.disposed)
            {
                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                if (disposing)
                {
                    // Dispose managed resources.
                    foreach (var hotkey in RegisteredHotkeys.Values)
                        Unregister(hotkey);
                }

                // Note disposing has been done.
                disposed = true;
            }
        }

        #endregion

        /// <summary>
        /// Private User32 interop methods
        /// </summary>
        private class User32
        {
            [DllImport("user32.dll")]
            public static extern bool RegisterHotKey(IntPtr hWnd, int id, UInt32 fsModifiers, UInt32 vlc);

            [DllImport("user32.dll")]
            public static extern bool UnregisterHotKey(IntPtr hWnd, int id);

            public const int WmHotKey = 0x0312;
        }
    }
}

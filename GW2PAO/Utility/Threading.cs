using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace GW2PAO.Utility
{
    /// <summary>
    /// Simple static helper class for simplifying common thread-related operations
    /// </summary>
    public static class Threading
    {
        /// <summary>
        /// Synchronously invokes an action on the UI thread, if required
        /// If not required, synchronously invokes the action
        /// </summary>
        /// <param name="action">The action to invoke</param>
        public static void InvokeOnUI(Action action)
        {
            if (Application.Current != null)
            {
                if (Application.Current.Dispatcher.CheckAccess())
                {
                    action.Invoke();
                }
                else
                {
                    Application.Current.Dispatcher.Invoke(action);
                }
            }
        }

        /// <summary>
        /// Asynchronously invokes an action on the UI thread, if required 
        /// If not required, synchronously invokes the action
        /// </summary>
        /// <param name="action">The action to invoke</param>
        public static void BeginInvokeOnUI(Action action)
        {
            if (Application.Current != null)
            {
                if (Application.Current.Dispatcher.CheckAccess())
                {
                    action.Invoke();
                }
                else
                {
                    Application.Current.Dispatcher.BeginInvoke(action);
                }
            }
        }
    }
}

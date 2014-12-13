using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GW2PAO.Infrastructure;
using GW2PAO.Modules.WebBrowser;
using GW2PAO.Modules.WebBrowser.Interfaces;
using GW2PAO.Modules.WebBrowser.Views;
using Microsoft.Practices.Prism.Commands;

namespace GW2PAO.Modules.WebBrowser
{
    [Export(typeof(IWebBrowserController))]
    public class WebBrowserController : IWebBrowserController
    {
#if !NO_BROWSER
        /// <summary>
        /// The browser object
        /// </summary>
        private BrowserView browser;
#endif

        /// <summary>
        /// Default constructor
        /// </summary>
        public WebBrowserController()
        {
#if !NO_BROWSER
            HotkeyCommands.ToggleWebBrowserCommand.RegisterCommand(new DelegateCommand(this.ToggleWebBrowser));
#endif
        }

        /// <summary>
        /// Opens the browser window
        /// </summary>
        public void OpenBrowser()
        {
#if !NO_BROWSER
            if (this.browser == null || !this.browser.IsVisible)
            {
                this.browser = new BrowserView();
                this.browser.Show();
            }
            else
            {
                this.browser.Focus();
            }
#endif
        }

        /// <summary>
        /// Closes the browser window
        /// </summary>
        public void CloseBrowser()
        {
#if !NO_BROWSER
            if (this.browser != null && this.browser.IsVisible)
            {
                this.browser.Close();
            }
#endif
        }

        /// <summary>
        /// Goes to a specific url.
        /// If the browser is open, sets the browser's url.
        /// If the browser is not open, opens a new browser window with the given url.
        /// </summary>
        /// <param name="url">The url to go to</param>
        public void GoToUrl(string url)
        {
#if !NO_BROWSER
            if (this.browser == null || !this.browser.IsVisible)
            {
                this.browser = new BrowserView(new Uri(url));
                this.browser.Show();
            }
            else
            {
                this.browser.webControl.Source = new Uri(url);
            }
#else
            // No browser, just start a process with the URL and let windows handle it
            Process.Start(url);
#endif
        }

        /// <summary>
        /// Toggles whether or not the web browser is open
        /// </summary>
        private void ToggleWebBrowser()
        {
#if !NO_BROWSER
            if (this.browser == null || !this.browser.IsVisible)
            {
                this.OpenBrowser();
            }
            else
            {
                this.CloseBrowser();
            }
#endif
        }
    }
}

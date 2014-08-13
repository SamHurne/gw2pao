using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GW2PAO.Controllers.Interfaces;
using GW2PAO.Views.WebBrowser;

namespace GW2PAO.Controllers
{
    public class BrowserController : IBrowserController
    {
        /// <summary>
        /// The browser object
        /// </summary>
        private BrowserView browser;

        /// <summary>
        /// Opens the browser window
        /// </summary>
        public void OpenBrowser()
        {
            if (this.browser == null || !this.browser.IsVisible)
            {
                this.browser = new BrowserView();
                this.browser.Show();
            }
            else
            {
                this.browser.Focus();
            }
        }

        /// <summary>
        /// Closes the browser window
        /// </summary>
        public void CloseBrowser()
        {
            if (this.browser != null && this.browser.IsVisible)
            {
                this.browser.Close();
            }
        }

        /// <summary>
        /// Goes to a specific url.
        /// If the browser is open, sets the browser's url.
        /// If the browser is not open, opens a new browser window with the given url.
        /// </summary>
        /// <param name="url">The url to go to</param>
        public void GoToUrl(string url)
        {
            if (this.browser == null || !this.browser.IsVisible)
            {
                this.browser = new BrowserView(new Uri(url));
                this.browser.Show();
            }
            else
            {
                this.browser.webControl.Source = new Uri(url);
            }
        }
    }
}

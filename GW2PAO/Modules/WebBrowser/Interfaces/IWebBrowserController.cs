using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GW2PAO.Modules.WebBrowser.Interfaces
{
    public interface IWebBrowserController
    {
        /// <summary>
        /// Opens the browser window
        /// </summary>
        void OpenBrowser();

        /// <summary>
        /// Closes the browser window
        /// </summary>
        void CloseBrowser();

        /// <summary>
        /// Goes to a specific url.
        /// If the browser is open, sets the browser's url.
        /// If the browser is not open, opens a new browser window with the given url.
        /// </summary>
        /// <param name="url">The url to go to</param>
        void GoToUrl(string url);
    }
}

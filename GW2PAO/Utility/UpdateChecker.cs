using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GW2PAO.Utility
{
    public static class UpdateChecker
    {
        /// <summary>
        /// Synchronous check to see if a new version of the software is available
        /// Based on http://www.hanselman.com/blog/HowToAutomaticallyNotifyTheUserThatItsTimeToUpgradeAWindowsApp.aspx
        /// </summary>
        /// <returns>True if a new version is available, else false</returns>
        public static bool IsNewVersionAvailable()
        {
            var http = new HttpClient();
            var request = http.GetStringAsync(new Uri("https://raw.githubusercontent.com/SamHurne/gw2pao/master/.gitignore"));
            request.Wait(); // Should be pretty quick

            string versionString = request.Result;
            Version latestVersion = new Version(versionString);

            // Get the assembly version and compare
            Assembly assembly = Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            Version myVersion = new Version(fvi.ProductVersion);

            if (latestVersion > myVersion)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}

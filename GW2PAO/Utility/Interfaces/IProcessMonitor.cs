using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GW2PAO.Utility.Interfaces
{
    public interface IProcessMonitor : IDisposable
    {
        /// <summary>
        /// Raised when the GW2 Process gains focus
        /// </summary>
        event EventHandler GW2Focused;

        /// <summary>
        /// Raised when the GW2 Process loses focus
        /// </summary>
        event EventHandler GW2LostFocus;
    }
}

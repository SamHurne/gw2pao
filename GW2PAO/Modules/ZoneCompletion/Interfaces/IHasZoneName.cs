using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GW2PAO.Modules.ZoneCompletion.Interfaces
{
    /// <summary>
    /// Interface for an object that contains a ZoneName property
    /// </summary>
    public interface IHasZoneName
    {
        /// <summary>
        /// Zone name
        /// </summary>
        string ZoneName { get; set; }
    }
}

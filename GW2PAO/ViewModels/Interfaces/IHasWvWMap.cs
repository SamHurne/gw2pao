using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GW2PAO.API.Data.Enums;

namespace GW2PAO.ViewModels.Interfaces
{
    /// <summary>
    /// Interface for object that has a Map property for WvW
    /// </summary>
    public interface IHasWvWMap
    {
        WvWMap Map { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GW2PAO.API.Services.Interfaces
{
    public interface IDungeonsService
    {
        DungeonsTable DungeonsTable { get; }
        void LoadTable();
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Prism.Commands;

namespace GW2PAO.Infrastructure
{
    public static class Commands
    {
        public static readonly CompositeCommand ApplicationShutdownCommand = new CompositeCommand();
    }
}

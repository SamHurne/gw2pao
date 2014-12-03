using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Prism.Events;

namespace GW2PAO.Infrastructure
{
    public class InsufficientPrivilegesEvent : CompositePresentationEvent<object> { }
}

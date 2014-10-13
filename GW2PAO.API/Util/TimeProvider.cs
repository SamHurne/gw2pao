using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GW2PAO.API.Util
{
    public interface ITimeProvider
    {
        /// <summary>
        /// Returns the current date/time of the system
        /// </summary>
        DateTimeOffset CurrentTime { get; }
    }

    public class DefaultTimeProvider : ITimeProvider
    {
        /// <summary>
        /// Returns the current date/time of the system as UTC
        /// </summary>
        public DateTimeOffset CurrentTime
        {
            get
            {
                return DateTimeOffset.UtcNow;
            }
        }
    }
}

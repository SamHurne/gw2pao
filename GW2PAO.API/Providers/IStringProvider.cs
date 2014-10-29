using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GW2PAO.API.Providers
{
    public interface IStringProvider<T>
    {
        void SetCulture(CultureInfo culture);
        string GetString(T id);
    }

    public interface IStringProvider<T1, T2>
    {
        void SetCulture(CultureInfo culture);
        string GetString(T1 id, T2 selector);
    }
}

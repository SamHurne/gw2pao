using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GW2PAO.Modules.Commerce.Interfaces
{
    public interface ICommerceViewController
    {
        void Initialize();
        void Shutdown();

        void DisplayRebuildItemNamesView();
        bool CanDisplayRebuildItemNamesView();

        void DisplayPriceNotificationsWindow();
        bool CanDisplayPriceNotificationsWindow();

        void DisplayPriceTracker();
        bool CanDisplayPriceTracker();

        void DisplayTPCalculator();
        bool CanDisplayTPCalculator();
    }
}

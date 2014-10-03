using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FeserWard.Controls;
using GW2PAO.API.Services.Interfaces;

namespace GW2PAO.ViewModels.PriceNotification
{
    public class ItemResultsProvider : IIntelliboxResultsProvider
    {
        private ICommerceService commerceService;

        public ItemResultsProvider(ICommerceService commerceService)
        {
            this.commerceService = commerceService;
        }
    
        public IEnumerable DoSearch(string searchTerm, int maxResults, object extraInfo)
        {
            var possibleItems = this.commerceService.ItemsDB.Values.Where(item => item.Name.Contains(searchTerm)).Take(maxResults);
            return possibleItems;
        }
    }
}

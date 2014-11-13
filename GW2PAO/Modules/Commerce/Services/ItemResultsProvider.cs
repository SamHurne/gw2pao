using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FeserWard.Controls;
using GW2PAO.API.Services.Interfaces;

namespace GW2PAO.Modules.Commerce.Services
{
    /// <summary>
    /// Provides basic search capabilities for GW2 items
    /// </summary>
    public class ItemResultsProvider : IIntelliboxResultsProvider
    {
        /// <summary>
        /// The commerce service
        /// </summary>
        private ICommerceService commerceService;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="commerceService">The commerce service providing access to the commerce API</param>
        public ItemResultsProvider(ICommerceService commerceService)
        {
            this.commerceService = commerceService;
        }

        /// <summary>
        /// Performs a search for a specific item
        /// </summary>
        /// <param name="searchTerm">Name of the item to use in the search</param>
        /// <param name="maxResults">Maximum amount of results to return</param>
        /// <param name="extraInfo">not used</param>
        /// <returns>An IEnumerable of search results</returns>
        public IEnumerable DoSearch(string searchTerm, int maxResults, object extraInfo)
        {
            var possibleItems = this.commerceService.ItemsDB.Values.Where(item => item.Name.Contains(searchTerm)).Take(maxResults);
            return possibleItems;
        }
    }
}

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
            var allItems = this.commerceService.ItemsDB.Values;

            // First, filter by all items that contain the given searchTerm
            var matches = allItems.Where(item => item.Name.ToLower().Contains(searchTerm.ToLower()));

            // Then, pull out any items where the name is an exact match
            var exactMatches = matches.Where(item => item.Name.ToLower().Equals(searchTerm.ToLower()));

            if (exactMatches.Count() >= maxResults)
            {
                return exactMatches.Take(maxResults);
            }
            else
            {
                var nonExactMatches = matches.Where(item => !item.Name.ToLower().Equals(searchTerm.ToLower()));
                var sortedNonExact = matches.OrderBy(item => item.Name.Length);

                return exactMatches.Concat(sortedNonExact).Take(maxResults);
            }
        }
    }
}

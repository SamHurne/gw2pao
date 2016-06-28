"use strict";

// This .js file contains the code required to pull the download counts from the GitHub releases API
(function() {
   function findAssetDownloadCount(releases, assetUrl) {
      // Search through the releases to find the specified asset (by URL)
      for (var i = 0; i < releases.length; i++) {
         var release = releases[i];
         for (var j = 0; j < release.assets.length; j++) {
            var asset = release.assets[j];
            if (asset.browser_download_url == assetUrl) {
               return asset.download_count;
            }
         }
      }
   }

   $(function() {
      $.ajax({
         url: "https://api.github.com/repos/SamHurne/gw2pao/releases?callback=?",
         type: "GET",
         dataType: 'json',
         cache: true,
         success: function(response, status, error) {
            var total = 0;
            for (var i = 0; i < response.data.length; i++) {
               var release = response.data[i];
               for (var j = 0; j < release.assets.length; j++) {
                  total += release.assets[j].download_count;
               }
            }
            $("#totalDownloadCounts").text(total);
            $("#installerDownloadCount").text(findAssetDownloadCount(response.data, $("#installerDownloadLink").attr("href")));
            $("#fullFeaturesDownloadCount").text(findAssetDownloadCount(response.data, $("#fullFeaturesDownloadLink").attr("href")));
            $("#noBrowserDownloadCount").text(findAssetDownloadCount(response.data, $("#noBrowserDownloadLink").attr("href")));
         },
         error: function(data, status, error) {
            console.log('error', data, status, error);
         }
      });
   });
})();
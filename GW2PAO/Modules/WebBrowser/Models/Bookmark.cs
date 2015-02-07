using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Prism.Mvvm;

namespace GW2PAO.Modules.WebBrowser.Models
{
    [Serializable]
    public class Bookmark : BindableBase
    {
        private string name;
        private string url;

        /// <summary>
        /// Name of the bookmark
        /// </summary>
        public string Name
        {
            get { return this.name; }
            set { SetProperty(ref this.name, value); }
        }

        /// <summary>
        /// URL of the bookmark
        /// </summary>
        public String URL
        {
            get { return this.url; }
            set { SetProperty(ref this.url, value); }
        }

        /// <summary>
        /// Parameter-less constructor for serialization purposes
        /// </summary>
        private Bookmark()
        {

        }

        /// <summary>
        /// Constructs a new bookmark object
        /// </summary>
        /// <param name="name">Name of the bookmark</param>
        /// <param name="url">URL of the bookmark</param>
        public Bookmark(string name, Uri url)
        {
            this.Name = name;
            this.URL = url.AbsoluteUri;
        }
    }
}

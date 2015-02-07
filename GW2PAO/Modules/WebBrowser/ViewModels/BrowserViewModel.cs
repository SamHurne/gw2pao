using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml.Serialization;
using Awesomium.Core;
using GW2PAO.Modules.WebBrowser.Models;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using NLog;

namespace GW2PAO.Modules.WebBrowser.ViewModels
{
    [Export]
    public class BrowserViewModel : BindableBase
    {
        /// <summary>
        /// Default logger
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private const string BOOKMARKS_FILENAME = "Bookmarks.xml";
        private Uri source;
        private Uri actualSource;

        /// <summary>
        /// Collection of user-defined bookmarks
        /// </summary>
        public ObservableCollection<Bookmark> Bookmarks { get; private set; }

        /// <summary>
        /// Current URL of the browser
        /// </summary>
        public Uri Source
        {
            get { return this.source; }
            set { SetProperty(ref this.source, value); }
        }

        /// <summary>
        /// Current URL of the browser
        /// </summary>
        public Uri ActualSource
        {
            get { return this.actualSource; }
            set
            {
                if (SetProperty(ref this.actualSource, value))
                    this.OnPropertyChanged(() => this.IsSourceBookmarked);
            }
        }

        /// <summary>
        /// True if the current source is a bookmark, else false
        /// </summary>
        public bool IsSourceBookmarked
        {
            get { return this.actualSource != null && this.Bookmarks.Any(b => b.URL == this.actualSource.AbsoluteUri); }
        }

        /// <summary>
        /// Command to go to a specific bookmark
        /// </summary>
        public ICommand GoToBookmarkCommand { get; private set; }

        /// <summary>
        /// Command to add a bookmark
        /// </summary>
        public ICommand AddBookmarkCommand { get; private set; }

        /// <summary>
        /// Command to delete a specific bookmark
        /// </summary>
        public ICommand DeleteBookmarkCommand { get; private set; }

        /// <summary>
        /// Constructs a new Browser ViewModel
        /// </summary>
        public BrowserViewModel()
        {
            this.Bookmarks = new ObservableCollection<Bookmark>();
            this.InitializeBookmarks();
            this.Bookmarks.CollectionChanged += (o, e) => this.SaveBookmarks();

            this.GoToBookmarkCommand = new DelegateCommand<Bookmark>((b) => this.Source = new Uri(b.URL));
            this.AddBookmarkCommand = new DelegateCommand<string>((name) => 
                {
                    this.Bookmarks.Add(new Bookmark(name, this.ActualSource));
                    this.OnPropertyChanged(() => this.IsSourceBookmarked);
                });
            this.DeleteBookmarkCommand = new DelegateCommand(() =>
                {
                    var bookmark = this.Bookmarks.First(b => b.URL == this.ActualSource.AbsoluteUri);
                    this.Bookmarks.Remove(bookmark);
                    this.OnPropertyChanged(() => this.IsSourceBookmarked);
                });

            this.Source = WebCore.Configuration.HomeURL;
        }

        /// <summary>
        /// Initializes bookmarks by loading the bookmarks file
        /// from disk or using the default set of bookmarks
        /// </summary>
        private void InitializeBookmarks()
        {
            if (!this.TryLoadBookmarks())
            {
                this.Bookmarks.Add(new Bookmark("GW2 Wiki", new Uri("http://wiki.guildwars2.com/wiki/Main_Page")));
                this.Bookmarks.Add(new Bookmark("Dulfy", new Uri("http://dulfy.net/category/gw2/")));
                this.Bookmarks.Add(new Bookmark("GW2 Crafts", new Uri("http://www.gw2crafts.net/")));
                this.Bookmarks.Add(new Bookmark("GW2 Spidy", new Uri("http://www.gw2spidy.com/")));
                this.Bookmarks.Add(new Bookmark("GW2 TP", new Uri("http://www.gw2tp.com/")));
                this.Bookmarks.Add(new Bookmark("GW2 Dungeons", new Uri("http://gw2dungeons.net/")));
                this.Bookmarks.Add(new Bookmark("GW2 Nodes", new Uri("http://gw2nodes.com/")));
                this.Bookmarks.Add(new Bookmark("Metabattle", new Uri("http://metabattle.com/wiki/MetaBattle_Wiki")));
                this.Bookmarks.Add(new Bookmark("GW2 Skill Points To Gold", new Uri("http://gw2sp2g.herokuapp.com/")));
                this.Bookmarks.Add(new Bookmark("Egg Baron Material Promotion Sheet", new Uri("https://docs.google.com/spreadsheet/lv?key=0As-wCpIszrT9dFB3YjVUVFhfenlDUUpXTVBIdm5qWmc")));
                this.Bookmarks.Add(new Bookmark("Ectoplasm Salvage Calculator", new Uri("http://gw.zweistein.cz/gw2ecto/")));
            }
        }

        /// <summary>
        /// Saves bookmarks to disk
        /// </summary>
        private void SaveBookmarks()
        {
            logger.Debug("Saving user data");
            XmlSerializer serializer = new XmlSerializer(this.Bookmarks.GetType());

            if (!Directory.Exists(GW2PAO.Data.UserData.UserData<object>.DataDirectory))
            {
                Directory.CreateDirectory(GW2PAO.Data.UserData.UserData<object>.DataDirectory);
            }

            string fullPath = GW2PAO.Data.UserData.UserData<object>.DataDirectory + BOOKMARKS_FILENAME;
            using (TextWriter writer = new StreamWriter(fullPath, false, Encoding.Unicode))
            {
                serializer.Serialize(writer, this.Bookmarks);
            }
        }

        /// <summary>
        /// Loads the bookmarks from the user data file
        /// </summary>
        /// <returns>True if successful, else false</returns>
        private bool TryLoadBookmarks()
        {
            XmlSerializer deserializer = new XmlSerializer(this.Bookmarks.GetType());
            object loadedBookmarks = null;

            string fullPath = GW2PAO.Data.UserData.UserData<object>.DataDirectory + BOOKMARKS_FILENAME;
            try
            {
                if (File.Exists(fullPath))
                {
                    using (TextReader reader = new StreamReader(fullPath))
                    {
                        loadedBookmarks = deserializer.Deserialize(reader);
                    }
                }

                if (loadedBookmarks != null)
                {
                    foreach (var bookmark in (ObservableCollection<Bookmark>)loadedBookmarks)
                    {
                        this.Bookmarks.Add(bookmark);
                    }

                    logger.Info("Bookmarks successfully loaded");
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                logger.Warn("Unable to load bookmarks! Exception: ", ex);
                return false;
            }
        }
    }
}

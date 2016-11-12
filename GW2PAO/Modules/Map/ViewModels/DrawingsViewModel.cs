using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Windows.Input;
using System.Xml.Serialization;
using GW2PAO.API.Services.Interfaces;
using GW2PAO.Modules.Map.Models;
using GW2PAO.Modules.Tasks;
using GW2PAO.Modules.Tasks.Interfaces;
using GW2PAO.Modules.Tasks.ViewModels;
using GW2PAO.PresentationCore;
using GW2PAO.Utility;
using Microsoft.Practices.Prism.Mvvm;
using Microsoft.Win32;
using NLog;

namespace GW2PAO.Modules.Map.ViewModels
{
    [Export(typeof(DrawingsViewModel))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class DrawingsViewModel : BindableBase
    {
        /// <summary>
        /// Default logger
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private CharacterPointerViewModel charPointerVm;
        private IZoneService zoneService;
        private IPlayerService playerService;
        private MapUserData userData;
        private int currentContinentId;
        private DrawingViewModel newDrawing;
        private bool penEnabled;

        /// <summary>
        /// The collection of drawings to show on the map
        /// </summary>
        public ObservableCollection<DrawingViewModel> Drawings
        {
            get;
            private set;
        }

        /// <summary>
        /// View model of the new drawing
        /// </summary>
        public DrawingViewModel NewDrawing
        {
            get { return this.newDrawing; }
            private set { SetProperty(ref this.newDrawing, value); }
        }

        /// <summary>
        /// True if the drawing pen is enabled, else false
        /// </summary>
        public bool PenEnabled
        {
            get { return this.penEnabled; }
            set { SetProperty(ref this.penEnabled, value); }
        }

        /// <summary>
        /// The list of color options for the drawing pen
        /// </summary>
        public List<string> PenColorOptions
        {
            get;
            private set;
        }

        /// <summary>
        /// Command to copy the character's trail into the New Drawing
        /// </summary>
        public ICommand CopyCharacterTrailCommand { get; private set; }

        /// <summary>
        /// Command to clear the new drawing's data
        /// </summary>
        public ICommand ClearNewDrawingCommand { get; private set; }

        /// <summary>
        /// Command to save the new drawing
        /// </summary>
        public ICommand SaveNewDrawingCommand { get; private set; }

        /// <summary>
        /// Command to import drawings
        /// </summary>
        public ICommand ImportDrawingsCommand { get; private set; }

        /// <summary>
        /// Command to export drawings
        /// </summary>
        public ICommand ExportDrawingsCommand { get; private set; }

        /// <summary>
        /// Constructs a new MarkersViewModel object
        /// </summary>
        [ImportingConstructor]
        public DrawingsViewModel(CharacterPointerViewModel charPointerVm, IZoneService zoneService, IPlayerService playerService, MapUserData userData)
        {
            this.charPointerVm = charPointerVm;
            this.zoneService = zoneService;
            this.playerService = playerService;
            this.userData = userData;

            if (this.playerService.HasValidMapId)
            {
                var continent = this.zoneService.GetContinentByMap(this.playerService.MapId);
                this.currentContinentId = continent.Id;
            }
            else
            {
                this.currentContinentId = 1;
            }

            this.NewDrawing = new DrawingViewModel(new Drawing(this.currentContinentId), this.currentContinentId, this.userData);
            this.PenEnabled = false;
            this.Drawings = new ObservableCollection<DrawingViewModel>();
            foreach (var drawing in userData.Drawings)
                this.Drawings.Add(new DrawingViewModel(drawing, this.currentContinentId, this.userData));
            this.Drawings.CollectionChanged += Drawings_CollectionChanged;
            this.userData.Drawings.CollectionChanged += UserDataDrawings_CollectionChanged;

            this.PenColorOptions = new List<string>()
            {
                "#FFFFFF",
                "#000000",
                "#DD2C00",
                "#6200EA",
                "#2962FF",
                "#00C853",
                "#FFEA00",
                "#FF6D00"
            };

            this.CopyCharacterTrailCommand = new DelegateCommand(this.CopyCharacterTrail);
            this.ClearNewDrawingCommand = new DelegateCommand(this.ClearNewDrawing);
            this.SaveNewDrawingCommand = new DelegateCommand(this.SaveNewDrawing);
            this.ImportDrawingsCommand = new DelegateCommand(this.ImportDrawings);
            this.ExportDrawingsCommand = new DelegateCommand(this.ExportDrawing);
        }

        public void OnContinentChanged(int currentContinentId)
        {
            this.currentContinentId = currentContinentId;
            foreach (var drawing in this.Drawings)
            {
                drawing.OnContinentChanged(currentContinentId);
            }
            this.ClearNewDrawing();
        }

        private void CopyCharacterTrail()
        {
            if (this.charPointerVm.PlayerTrail.Count > 1)
            {
                logger.Debug("Copying character trail into NewDrawing");
                if (this.newDrawing.ActivePolyline.Count > 0)
                    this.NewDrawing.BeginNewPolyline();
                foreach (var location in this.charPointerVm.PlayerTrail)
                {
                    this.NewDrawing.ActivePolyline.Add(location);
                }
                this.NewDrawing.BeginNewPolyline();
                this.charPointerVm.PlayerTrail.Clear();
            }
        }

        private void ClearNewDrawing()
        {
            logger.Debug("Clearing NewDrawing contents");
            this.NewDrawing.Polylines.Clear();
            this.NewDrawing.ActivePolyline.Clear();
        }

        private void SaveNewDrawing()
        {
            logger.Debug("Saving NewDrawing");
            this.Drawings.Add(this.NewDrawing);
            this.NewDrawing = new DrawingViewModel(new Drawing(this.currentContinentId), this.currentContinentId, this.userData);
        }

        private void Drawings_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.userData.Drawings.CollectionChanged -= UserDataDrawings_CollectionChanged;
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (DrawingViewModel drawingVm in e.NewItems)
                    {
                        this.userData.Drawings.Add(drawingVm.Drawing);
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (DrawingViewModel drawingVm in e.OldItems)
                    {
                        this.userData.Drawings.Remove(drawingVm.Drawing);
                    }
                    break;
                case NotifyCollectionChangedAction.Reset:
                    this.userData.Drawings.Clear();
                    break;
                default:
                    break;
            }
            this.userData.Drawings.CollectionChanged += UserDataDrawings_CollectionChanged;
        }

        private void UserDataDrawings_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.Drawings.CollectionChanged -= Drawings_CollectionChanged;
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (Drawing drawing in e.NewItems)
                    {
                        if (!this.Drawings.Any(vm => vm.Drawing.ID == drawing.ID))
                        {
                            this.Drawings.Add(new DrawingViewModel(drawing, this.currentContinentId, this.userData));
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (Drawing drawing in e.OldItems)
                    {
                        var drawingVm = this.Drawings.FirstOrDefault(vm => vm.Drawing.ID == drawing.ID);
                        if (drawingVm != null)
                        {
                            this.Drawings.Remove(drawingVm);
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Reset:
                    this.Drawings.Clear();
                    break;
                default:
                    break;
            }
            this.Drawings.CollectionChanged += Drawings_CollectionChanged;
        }

        private void ImportDrawings()
        {
            logger.Info("Importing drawings");

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.CheckPathExists = true;
            openFileDialog.Filter = "Drawing Files (*.xml)|*.xml";
            openFileDialog.Multiselect = false;
            if (openFileDialog.ShowDialog() == true)
            {
                logger.Info("Importing drawings from {0}", openFileDialog.FileName);

                XmlSerializer deserializer = new XmlSerializer(typeof(List<Drawing>));
                List<Drawing> loadedDrawings = null;

                try
                {
                    using (TextReader reader = new StreamReader(openFileDialog.FileName))
                    {
                        loadedDrawings = (List<Drawing>)deserializer.Deserialize(reader);
                    }

                    Threading.InvokeOnUI(() =>
                    {
                        foreach (var drawing in loadedDrawings)
                        {
                            if (this.userData.Drawings.All(d => d.ID != drawing.ID))
                            {
                                this.userData.Drawings.Add(drawing);
                            }
                        }
                    });

                    logger.Info("Successfully imported drawings from {0}", openFileDialog.FileName);
                }
                catch (Exception ex)
                {
                    logger.Error("Unable to import drawings!");
                    logger.Error(ex);
                }
            }
        }

        private void ExportDrawing()
        {
            var toExport = new List<Drawing>(this.Drawings.Where(d => d.IsSelected).Select(d => d.Drawing));
            if (toExport.Count > 0)
            {
                logger.Info("Exporting selected drawings");

                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.CheckPathExists = true;
                saveFileDialog.AddExtension = true;
                saveFileDialog.DefaultExt = ".xml";
                saveFileDialog.Filter = "Drawing Files (*.xml)|*.xml";
                if (saveFileDialog.ShowDialog() == true)
                {
                    logger.Info("Exporting selected drawings to {0}", saveFileDialog.FileName);
                    XmlSerializer serializer = new XmlSerializer(typeof(List<Drawing>));

                    using (TextWriter writer = new StreamWriter(saveFileDialog.FileName))
                    {
                        serializer.Serialize(writer, toExport);
                    }
                    logger.Info("Successfully exported drawings to {0}", saveFileDialog.FileName);

                }
            }
        }
    }
}

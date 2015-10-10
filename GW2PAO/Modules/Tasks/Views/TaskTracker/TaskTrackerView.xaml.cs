using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using GW2PAO.Modules.Tasks.ViewModels;
using GW2PAO.Views;
using NLog;

namespace GW2PAO.Modules.Tasks.Views.TaskTracker
{
    /// <summary>
    /// Interaction logic for TaskTrackerView.xaml
    /// </summary>
    public partial class TaskTrackerView : OverlayWindow
    {
        /// <summary>
        /// Default logger
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Actual height of an task in the list
        /// </summary>
        private double taskHeight;

        /// <summary>
        /// Height before collapsing the control
        /// </summary>
        private double beforeCollapseHeight;

        /// <summary>
        /// Task Tracker view model
        /// </summary>
        [Import]
        public TaskTrackerViewModel ViewModel
        {
            get
            {
                return this.DataContext as TaskTrackerViewModel;
            }
            set
            {
                this.DataContext = value;
            }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public TaskTrackerView()
        {
            logger.Debug("New TaskTrackerView created");
            InitializeComponent();

            this.ResizeHelper.InitializeResizeElements(null, null, this.ResizeGripper);

            // Save the height values for use when collapsing the window
            this.taskHeight = new TaskView().Height;
            
            this.Height = Properties.Settings.Default.TaskTrackerHeight;

            this.Loaded += this.TaskTrackerView_Loaded;
            this.Closing += this.TaskTrackerView_Closing;
            this.beforeCollapseHeight = this.Height;
        }

        private void TaskTrackerView_Loaded(object sender, RoutedEventArgs e)
        {
            // Set up resize snapping
            this.ResizeHelper.SnappingHeightOffset = 1;
            this.ResizeHelper.SnappingThresholdHeight = (int)this.TitleBar.ActualHeight;
            this.ResizeHelper.SnappingIncrementHeight = (int)this.taskHeight;
            this.MinHeight = this.taskHeight * 3 + (int)this.TitleBar.ActualHeight;
        }

        private void TaskTrackerView_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (this.WindowState == System.Windows.WindowState.Normal)
            {
                if (this.TasksContainer.Visibility == System.Windows.Visibility.Visible)
                {
                    Properties.Settings.Default.TaskTrackerHeight = this.Height;
                }
                else
                {
                    Properties.Settings.Default.TaskTrackerHeight = this.beforeCollapseHeight;
                }
                Properties.Settings.Default.TaskTrackerWidth = this.Width;
                Properties.Settings.Default.TaskTrackerX = this.Left;
                Properties.Settings.Default.TaskTrackerY = this.Top;
                Properties.Settings.Default.Save();
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            logger.Debug("TaskTrackerView closed");
        }

        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
            e.Handled = true;
        }

        private void CloseWindowButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void CollapseExpandButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.TasksContainer.Visibility == System.Windows.Visibility.Visible)
            {
                this.beforeCollapseHeight = this.Height;
                this.MinHeight = this.TitleBar.ActualHeight;
                this.Height = this.TitleBar.ActualHeight;
                this.TasksContainer.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                this.MinHeight = this.taskHeight * 3 + (int)this.TitleBar.ActualHeight;
                this.Height = this.beforeCollapseHeight;
                this.TasksContainer.Visibility = System.Windows.Visibility.Visible;
            }
        }

        private void TitleImage_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                Image image = sender as Image;
                ContextMenu contextMenu = image.ContextMenu;
                contextMenu.PlacementTarget = image;
                contextMenu.IsOpen = true;
                e.Handled = true;
            }
        }
    }
}

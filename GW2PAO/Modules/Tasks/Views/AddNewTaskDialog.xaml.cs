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
using GW2PAO.API.Services.Interfaces;
using GW2PAO.Modules.Tasks.Models;
using GW2PAO.Modules.Tasks.ViewModels;
using GW2PAO.Views;

namespace GW2PAO.Modules.Tasks.Views
{
    /// <summary>
    /// Interaction logic for AddNewTaskDialog.xaml
    /// </summary>
    public partial class AddNewTaskDialog : OverlayWindow
    {
        /// <summary>
        /// The player task that this dialog is adding or editing
        /// </summary>
        [Import]
        public NewTaskDialogViewModel TaskData
        {
            get
            {
                return this.DataContext as NewTaskDialogViewModel;
            }
            set
            {
                this.DataContext = value;
            }
        }

        /// <summary>
        /// Constructs a new AddNewTaskDialog window
        /// </summary>
        /// <param name="taskData">The task data</param>
        public AddNewTaskDialog()
        {
            InitializeComponent();
            this.CenterWindowOnScreen();
        }

        /// <summary>
        /// Centers the window on the screen
        /// </summary>
        private void CenterWindowOnScreen()
        {
            double screenWidth = System.Windows.SystemParameters.PrimaryScreenWidth;
            double screenHeight = System.Windows.SystemParameters.PrimaryScreenHeight;
            this.Left = (screenWidth / 2) - (this.Width / 2);
            this.Top = (screenHeight / 2) - (this.Height / 2);
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
            e.Handled = true;
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            this.TaskData.ApplyCommand.Execute(null);
            this.Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.IconPopup.IsOpen = true;
            e.Handled = true;
        }

        private void OnIntelliboxSuggestItem_MouseUp(object sender, MouseButtonEventArgs e)
        {
            this.ItemsEntryBox.ChooseCurrentItem();
        }
    }
}

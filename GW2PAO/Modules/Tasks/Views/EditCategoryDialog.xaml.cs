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
    /// Interaction logic for EditCategoryDialog.xaml
    /// </summary>
    public partial class EditCategoryDialog : OverlayWindow
    {
        public EditCategoryViewModel ViewModel
        {
            get
            {
                return this.DataContext as EditCategoryViewModel;
            }
            set
            {
                this.DataContext = value;
            }
        }

        protected override bool SetNoFocus { get { return false; } }

        /// <summary>
        /// Constructs a new AddNewTaskDialog window
        /// </summary>
        public EditCategoryDialog(EditCategoryViewModel vm)
        {
            this.ViewModel = vm;
            InitializeComponent();
            this.Loaded += EditCategoryDialog_Loaded;
            this.CenterWindowOnScreen();
        }

        private void EditCategoryDialog_Loaded(object sender, RoutedEventArgs e)
        {
            this.CenterWindowOnScreen();
            this.CategoryNameTextbox.Focus();
        }

        /// <summary>
        /// Centers the window on the screen
        /// </summary>
        private void CenterWindowOnScreen()
        {
            double screenWidth = System.Windows.SystemParameters.PrimaryScreenWidth;
            double screenHeight = System.Windows.SystemParameters.PrimaryScreenHeight;
            this.Left = (screenWidth / 2) - (this.ActualWidth / 2);
            this.Top = (screenHeight / 2) - (this.ActualHeight / 2);
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
            e.Handled = true;
        }

        private void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            this.ViewModel.ApplyCommand.Execute(null);
            this.Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}

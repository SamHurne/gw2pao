using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GW2PAO.PresentationCore;
using Microsoft.Practices.Prism.Mvvm;
using NLog;

namespace GW2PAO.Modules.Tasks.ViewModels
{
    public class EditCategoryViewModel : BindableBase
    {
        /// <summary>
        /// Default logger
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private TaskCategoryViewModel category;
        private string newCategoryName;

        /// <summary>
        /// The category's new name
        /// </summary>
        public string NewCategoryName
        {
            get { return this.newCategoryName; }
            set { SetProperty(ref this.newCategoryName, value); }
        }

        /// <summary>
        /// Command to apply the add/edit of the task
        /// </summary>
        public ICommand ApplyCommand { get; private set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public EditCategoryViewModel(TaskCategoryViewModel category)
        {
            this.category = category;
            this.NewCategoryName = category.CategoryName;
            this.ApplyCommand = new DelegateCommand(this.ApplyChanges);
        }

        /// <summary>
        /// Applies the changes to the category
        /// </summary>
        private void ApplyChanges()
        {
            this.category.CategoryName = this.NewCategoryName;
        }
    }
}

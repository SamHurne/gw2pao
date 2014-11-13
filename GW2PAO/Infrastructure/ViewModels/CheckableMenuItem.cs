using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GW2PAO.Infrastructure.Interfaces;
using Microsoft.Practices.Prism.Mvvm;

namespace GW2PAO.Infrastructure.ViewModels
{
    public class CheckableMenuItem : BindableBase, IMenuItem
    {
        /// <summary>
        /// Property info object for the property that holds the "IsChecked" value
        /// </summary>
        private PropertyInfo isCheckedProperty;

        /// <summary>
        /// Owner of the IsChecked property
        /// </summary>
        private INotifyPropertyChanged propertyOwner;

        /// <summary>
        /// Setter action for setting the isChecked property (alternative)
        /// </summary>
        private Action<bool> isCheckedSetter;

        /// <summary>
        /// Getter func for getting the isChecked property (alternative)
        /// </summary>
        private Func<bool> isCheckedGetter;

        /// <summary>
        /// Collection of Sub-menu items
        /// Always empty.
        /// </summary>
        public ObservableCollection<IMenuItem> SubMenuItems { get; private set; }

        /// <summary>
        /// Header text of the menu item
        /// </summary>
        public string Header { get; private set; }

        /// <summary>
        /// The on-click command
        /// Always null.
        /// </summary>
        public ICommand OnClickCommand { get; private set; }

        /// <summary>
        /// True if the menu item is checkable, else false
        /// Always true.
        /// </summary>
        public bool IsCheckable { get { return true; } }

        /// <summary>
        /// True if the menu item does not close the menu on click, else false
        /// Always true.
        /// </summary>
        public bool StaysOpen { get { return true; } }

        /// <summary>
        /// True if the menu item is checked, else false
        /// </summary>
        public bool IsChecked
        {
            get
            {
                if (this.isCheckedGetter != null)
                    return this.isCheckedGetter();
                else
                    return (bool)this.isCheckedProperty.GetValue(propertyOwner);
            }
            set 
            {
                if (this.isCheckedSetter != null)
                    this.isCheckedSetter(value);
                else
                    this.isCheckedProperty.SetValue(propertyOwner, value);
            }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="header">Header text to use for the menu item</param>
        /// <param name="action">Action to perform when clicking the menu item, if any</param>
        public CheckableMenuItem(string header, Expression<Func<bool>> isCheckedProperty, INotifyPropertyChanged propertyOwner)
        {
            this.SubMenuItems = new ObservableCollection<IMenuItem>();
            this.Header = header;

            var isCheckedLambdaBody = isCheckedProperty.Body as MemberExpression;
            this.isCheckedProperty = isCheckedLambdaBody.Member as PropertyInfo;
            this.propertyOwner = propertyOwner;

            this.propertyOwner.PropertyChanged += (o, e) =>
                {
                    if (e.PropertyName == isCheckedLambdaBody.Member.Name)
                        this.OnPropertyChanged(() => this.IsChecked);
                };
        }

        /// <summary>
        /// Alternate constructor
        /// </summary>
        /// <param name="header">Header text to use for the menu item</param>
        /// <param name="setter">Getter for the IsChecked information</param>
        /// <param name="getter">Setter for the IsChecked information</param>
        /// <param name="propertyOwner">Owner of the IsChecked PropertyChanged event</param>
        /// <param name="isCheckedProperty">Lambda containing the property name for use with the PropertyChanged event</param>
        public CheckableMenuItem(string header, Action<bool> setter = null, Func<bool> getter = null, INotifyPropertyChanged propertyOwner = null, Expression<Func<object>> isCheckedProperty = null)
        {
            this.SubMenuItems = new ObservableCollection<IMenuItem>();
            this.Header = header;

            this.isCheckedSetter = setter;
            this.isCheckedGetter = getter;

            if (propertyOwner != null && isCheckedProperty != null)
            {
                var isCheckedLambdaBody = isCheckedProperty.Body as MemberExpression;

                propertyOwner.PropertyChanged += (o, e) =>
                {
                    if (e.PropertyName == isCheckedLambdaBody.Member.Name)
                        this.OnPropertyChanged(() => this.IsChecked);
                };
            }
        }
    }
}

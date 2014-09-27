using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using GW2PAO.ViewModels;
using GW2PAO.ViewModels.EventNotification;
using NLog;

namespace GW2PAO.Views.PriceNotification
{
    /// <summary>
    /// Interaction logic for PriceView.xaml
    /// </summary>
    public partial class PriceView : UserControl
    {
        /// <summary>
        /// Default logger
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Default constructor
        /// </summary>
        public PriceView()
        {
            InitializeComponent();
        }

        private void TextBox_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            ((TextBox)sender).SelectAll();
        }

        private void TextBox_GotMouseCapture(object sender, MouseEventArgs e)
        {
            ((TextBox)sender).SelectAll();
        }

        private void TextBox_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            var tb = (TextBox)sender;
            
            if (tb.IsMouseOver)
            {
                int currentVal;
                if (int.TryParse(tb.Text, out currentVal))
                {
                    if (e.Delta > 0)
                    {
                        // Increment
                        tb.Text = (currentVal + 1).ToString();
                    }
                    else
                    {
                        if (currentVal > 0)
                        {
                            // Decrement
                            tb.Text = (currentVal - 1).ToString();
                        }
                    }
                }
            }
        }
    }
}

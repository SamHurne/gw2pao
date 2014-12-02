using GW2PAO.Modules.Commerce.ViewModels;
using OxyPlot;
using OxyPlot.Axes;
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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GW2PAO.Modules.Commerce.Views.PriceTracker
{
    /// <summary>
    /// Interaction logic for ItemPriceView.xaml
    /// </summary>
    public partial class ItemPriceView : UserControl
    {
        public ItemPriceView()
        {
            InitializeComponent();
            this.HistoricalPlotYAxis.LabelFormatter = (d) => string.Empty;
            this.HistoricalPlot.Controller = new PlotController();
            this.HistoricalPlot.Controller.BindMouseEnter(PlotCommands.HoverPointsOnlyTrack);
            this.DataContextChanged += ItemPriceView_DataContextChanged;
        }

        private void ItemPriceView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs args)
        {
            var vm = this.DataContext as ItemPriceViewModel;
            this.BuyOrderPlotSeries.ItemsSource = vm.PastBuyOrders;
            this.SellListingPlotSeries.ItemsSource = vm.PastSaleListings;
            this.HistoricalPlot.InvalidatePlot(true);
            vm.PastBuyOrders.CollectionChanged += (o, e) =>
            {
                this.HistoricalPlot.InvalidatePlot(true);
            };
            vm.PastSaleListings.CollectionChanged += (o, e) =>
            {
                this.HistoricalPlot.InvalidatePlot(true);
            };
        }

        private void GraphIcon_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (this.GraphPopup.IsOpen)
                this.GraphPopup.IsOpen = false;
            else
                this.GraphPopup.IsOpen = true;
        }
    }
}

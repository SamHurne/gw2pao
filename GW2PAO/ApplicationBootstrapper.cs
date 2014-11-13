using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using GW2PAO.Views;
using Microsoft.Practices.Prism.MefExtensions;

namespace GW2PAO
{
    public class ApplicationBootstrapper : MefBootstrapper
    {
        protected override System.Windows.DependencyObject CreateShell()
        {
            return this.Container.GetExportedValue<ShellView>();
        }

        protected override void InitializeShell()
        {
            base.InitializeShell();

            Application.Current.MainWindow = (ShellView)this.Shell;
            Application.Current.MainWindow.Show();
        }

        protected override void ConfigureAggregateCatalog()
        {
            base.ConfigureAggregateCatalog();

            // All modules are within this assembly
            this.AggregateCatalog.Catalogs.Add(new AssemblyCatalog(typeof(ApplicationBootstrapper).Assembly));

            // Register the API services
            this.AggregateCatalog.Catalogs.Add(new AssemblyCatalog(typeof(GW2PAO.API.Services.ZoneService).Assembly));

            // Register the Teamspeak services
            this.AggregateCatalog.Catalogs.Add(new AssemblyCatalog(typeof(GW2PAO.TS3.Services.TeamspeakService).Assembly));
        }

        protected override CompositionContainer CreateContainer()
        {
            var container = base.CreateContainer();
            container.ComposeExportedValue(container);
            return container;
        }
    }
}

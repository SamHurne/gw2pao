using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GW2PAO.API.Services.Interfaces;
using Microsoft.Practices.Prism.Mvvm;
using NLog;

namespace GW2PAO.Modules.DayNight.ViewModels
{
    [Export(typeof(DayNightTimerViewModel))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class DayNightTimerViewModel : BindableBase
    {
        /// <summary>
        /// Default logger
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private IPlayerService playerService;

        [ImportingConstructor]
        public DayNightTimerViewModel(IPlayerService playerService)
        {
            this.playerService = playerService;
        }
    }
}

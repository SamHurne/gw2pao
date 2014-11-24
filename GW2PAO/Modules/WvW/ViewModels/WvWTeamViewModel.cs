using GW2PAO.API.Data;
using GW2PAO.API.Data.Entities;
using GW2PAO.API.Data.Enums;
using GW2PAO.PresentationCore;
using Microsoft.Practices.Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GW2PAO.Modules.WvW.ViewModels
{
    public class WvWTeamViewModel : BindableBase
    {
        private World worldData;
        private string matchId;
        private WorldColor color;
        private int score;
        private int tickScore;

        /// <summary>
        /// The team's World ID
        /// </summary>
        public int WorldId { get { return this.worldData.ID; } }

        /// <summary>
        /// The team's World Name
        /// </summary>
        public string WorldName { get { return this.worldData.Name; } }

        /// <summary>
        /// The team's current Match ID
        /// </summary>
        public string MatchId
        {
            get { return this.matchId; }
            set { SetProperty(ref this.matchId, value); }
        }

        /// <summary>
        /// The team's current color
        /// </summary>
        public WorldColor Color
        {
            get { return this.color; }
            set { SetProperty(ref this.color, value); }
        }

        /// <summary>
        /// The team's current score
        /// </summary>
        public int Score
        {
            get { return this.score; }
            set { SetProperty(ref this.score, value); }
        }

        /// <summary>
        /// The team's current tick score
        /// </summary>
        public int TickScore
        {
            get { return this.tickScore; }
            set { SetProperty(ref this.tickScore, value); }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="worldData">The team's world data</param>
        public WvWTeamViewModel(World worldData)
        {
            this.worldData = worldData;
        }
    }
}

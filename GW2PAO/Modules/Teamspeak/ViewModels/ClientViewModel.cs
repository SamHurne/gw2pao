using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Prism.Mvvm;

namespace GW2PAO.Modules.Teamspeak.ViewModels
{
    public class ClientViewModel : BindableBase
    {
        private uint id;
        private string name;

        /// <summary>
        /// ID of the client
        /// </summary>
        public uint ID
        {
            get { return this.id; }
            set { this.SetProperty(ref this.id, value); }
        }

        /// <summary>
        /// Name of the client
        /// </summary>
        public string Name
        {
            get { return this.name; }
            set { this.SetProperty(ref this.name, value); }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="id">ID of the client</param>
        /// <param name="name">Name of the client</param>
        public ClientViewModel(uint id, string name)
        {
            this.ID = id;
            this.Name = name;
        }
    }
}

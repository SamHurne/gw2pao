using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GW2PAO.TS3.Constants;
using GW2PAO.TS3.Util;
using TS3QueryLib.Core.Common.Entities;

namespace GW2PAO.TS3.Data
{
    public class Channel
    {
        public uint ID { get; set; }
        public uint ParentID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public uint Order { get; set; }
        public uint ClientsCount { get; set; }
        public bool IsSpacer { get; set; }

        public Channel(uint id, string name, bool isSpacer = false)
        {
            this.ID = id;
            this.Name = name;
            this.IsSpacer = isSpacer;
        }

        internal static Channel FromChannelString(string channelString)
        {
            var parts = channelString.Split(' ', '\n', '\r');

            uint id = DecodeUtility.DecodeUIntProperty(channelString, Properties.ChannelID);

            uint parentId = 0;
            if (parts.FirstOrDefault(part => part.StartsWith(Properties.ParentID)) != null
                || parts.FirstOrDefault(part => part.StartsWith(Properties.ChannelParentID)) != null)
                parentId = DecodeUtility.DecodeUIntProperty(channelString, Properties.ParentID, Properties.ChannelParentID);

            uint order = 0;
            if (parts.FirstOrDefault(part => part.StartsWith(Properties.ChannelOrder)) != null)
                order = DecodeUtility.DecodeUIntProperty(channelString, Properties.ChannelOrder);

            string name = string.Empty;
            if (parts.FirstOrDefault(part => part.StartsWith(Properties.ChannelName)) != null)
                name = DecodeUtility.DecodeStringProperty(channelString, true, Properties.ChannelName);

            uint clientsCount = 0;
            if (parts.FirstOrDefault(part => part.StartsWith(Properties.ChannelClientsCount)) != null)
                clientsCount = DecodeUtility.DecodeUIntProperty(channelString, Properties.ChannelClientsCount);

            bool isSpacer = SpacerInfo.Parse(name) != null;

            Channel channelInfo = new Channel(id, name, isSpacer)
            {
                ParentID = parentId,
                Order = order,
                ClientsCount = clientsCount
            };

            return channelInfo;
        }
    }
}

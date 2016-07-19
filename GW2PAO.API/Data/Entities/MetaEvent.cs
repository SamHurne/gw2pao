using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GW2PAO.API.Data.Entities
{
    public class MetaEvent
    {
        public string Name { get; set; }
        public Guid ID { get; set; }
        public int MapID { get; set; }
        public string MapName { get; set; }
        public SerializableTimespan StartOffset { get; set; }
        public List<MetaEventStage> Stages { get; set; }
    }

    public class MetaEventStage
    {
        public string Name { get; set; }
        public SerializableTimespan Duration { get; set; }
    }
}

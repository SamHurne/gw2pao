namespace GW2PAO.API.Data.Entities
{
    using System;
    using System.Xml;
    using System.Xml.Schema;
    using System.Xml.Serialization;
    using GW2PAO.API.Data.Enums;

    /// <summary>
    /// World-vs-World Objective Unique Identifier, which is a combination of ID and Map
    /// </summary>
    public class WvWObjectiveId : IXmlSerializable
    {
        /// <summary>
        /// The numerical objective ID
        /// </summary>
        public int ObjectiveId { get; private set; }

        /// <summary>
        /// The map the objective is located on
        /// </summary>
        public WvWMap Map { get; private set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public WvWObjectiveId() { }

        /// <summary>
        /// Parameterised constructor
        /// </summary>
        public WvWObjectiveId(int objectiveId, WvWMap map)
        {
            this.ObjectiveId = objectiveId;
            this.Map = map;
        }

        public static bool operator == (WvWObjectiveId obj1, WvWObjectiveId obj2)
        {
            return obj1.Equals(obj2);
        }

        public static bool operator != (WvWObjectiveId obj1, WvWObjectiveId obj2)
        {
            return !obj1.Equals(obj2);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || this.GetType() != obj.GetType())
            {
                return false;
            }

            var other = (WvWObjectiveId)obj;

            return this.ObjectiveId == other.ObjectiveId &&
                   this.Map == other.Map;
        }

        public override int GetHashCode()
        {
            int code = this.ObjectiveId + (int)this.Map;
            return code.GetHashCode();
        }

        public override string ToString()
        {
            return this.Map + "-" + this.ObjectiveId + "-";
        }

        #region IXmlSerializable
        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            int parsedId = -1;
            if (int.TryParse(reader.GetAttribute("ObjectiveId"), out parsedId))
                this.ObjectiveId = parsedId;

            WvWMap parsedMap = WvWMap.Unknown;
            if (Enum.TryParse(reader.GetAttribute("Map"), out parsedMap))
                this.Map = parsedMap;

        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("ObjectiveId", this.ObjectiveId.ToString());
            writer.WriteAttributeString("Map", this.Map.ToString());
        }
        #endregion
    }
}

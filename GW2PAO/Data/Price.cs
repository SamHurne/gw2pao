using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using GW2PAO.PresentationCore;

namespace GW2PAO.Data
{
    /// <summary>
    /// Helper class containing the gold, silver, and copper components of a price in GW2
    /// </summary>
    public class Price : NotifyPropertyChangedBase
    {
        private int gold;
        private int silver;
        private int copper;
        private bool isNegative;

        /// <summary>
        /// True if the current value is negative, else false
        /// </summary>
        [XmlIgnore]
        public bool IsNegative
        {
            get { return this.isNegative; }
            set { SetField(ref this.isNegative, value); }
        }

        /// <summary>
        /// Gold-component for the price
        /// </summary>
        [XmlIgnore]
        public int Gold
        {
            get
            {
                return Math.Abs(this.gold); // Never display number as negative
            }
            set
            {
                SetField(ref this.gold, value);
            }
        }

        /// <summary>
        /// Silver-component for the price
        /// </summary>
        [XmlIgnore]
        public int Silver
        {
            get
            {
                return Math.Abs(this.silver); // Never display number as negative
            }
            set
            {
                if (value < 100)
                    SetField(ref this.silver, value);
                else
                    SetField(ref this.silver, 99);
            }
        }

        /// <summary>
        /// Copper-component for the price
        /// </summary>
        [XmlIgnore]
        public int Copper
        {
            get
            {
                return Math.Abs(this.copper); // Never display number as negative
            }
            set
            {
                if (value < 100)
                    SetField(ref this.copper, value);
                else
                    SetField(ref this.copper, 99);
            }
        }

        /// <summary>
        /// The total value, in copper
        /// </summary>
        public double Value
        {
            get
            {
                // Use backing fields instead of properties, as these contain negatives-info
                return (this.gold * 10000) + (this.silver * 100) + this.copper;
            }
            set
            {
                this.Gold = (int)(value / 10000.0);
                this.Silver = (int)((value - (this.gold * 10000.0)) / 100.0);
                this.Copper = (int)((value - (this.gold * 10000.0) - (this.silver * 100.0)));
                this.IsNegative = value < 0;
            }
        }
    }
}

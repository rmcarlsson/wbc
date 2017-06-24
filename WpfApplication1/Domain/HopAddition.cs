using GFCalc.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Grainsim.Domain
{

    public enum HopAdditionStage
    {
        Boil,
        Fermentation,
    }


    public enum HopAmountUnitE
    {
        IBU,
        GRAMS_PER_LITER
    }

    //[ValueConversion(typeof(Amount), typeof(AmountGrams))]
    public class HopAddition : IComparable<HopAddition>
    {
        [XmlElement("Hop")]
        public Hops Hop { set; get; }

        [XmlAttribute("Name")]
        public String Name { set; get; }

        [XmlAttribute("Amount")]
        public double Amount { get; set; }

        [XmlAttribute("AmountUnit")]
        public HopAmountUnitE AmountUnit { get; set; }

        [XmlAttribute("AmountGrams")]
        public double AmountGrams { get; set; }

        [XmlAttribute("Bitterness")]
        public int Bitterness { get; set; }

        [XmlAttribute("Duration")]
        public int Duration { get; set; }

        [XmlAttribute("Stage")]
        public HopAdditionStage Stage { get; set; }

        public override string ToString()
        {
            if (Stage == HopAdditionStage.Fermentation)
                return String.Format("{0:F0} grams of {1} during {2} days", AmountGrams, Hop.Name, Duration);
            else
                return String.Format("{0:F0} grams of {1} at {2} minutes before boil end", AmountGrams, Hop.Name, Duration);
        }

        public int CompareTo(HopAddition other)
        {
            if (other.Stage == this.Stage)
                return -(this.Duration.CompareTo(other.Duration));
            if (other.Stage == HopAdditionStage.Fermentation)
                return -1;
            else
                return 1;
        }
    }


    public enum FermentableStage
    {
        ColdSteep,
        Mash,
        Boil,
        Fermentor
    }

    public class GristPart
    {
        [XmlElement("FermentableAdjunct")]
        public FermentableAdjunct FermentableAdjunct { get; set; }
        [XmlAttribute("AmountPercent")]
        public int Amount { get; set; }
        [XmlAttribute("AmountGrams")]
        public int AmountGrams { get; set; }
        [XmlAttribute("Stage")]
        public FermentableStage Stage { get; set; }
        [XmlAttribute("GU")]
        public double GU { get; set; }


        public override string ToString()
        {
            if (Stage == FermentableStage.ColdSteep)
                return String.Format("{0:F0} grams of {1} added to {2:F1} liters of water", AmountGrams, FermentableAdjunct.Name, ColdSteep.GetWaterToAddToColdSteep(AmountGrams));
            else
                return String.Format("{0:F0} grams of {1}", AmountGrams, FermentableAdjunct.Name);
        }


    }


}

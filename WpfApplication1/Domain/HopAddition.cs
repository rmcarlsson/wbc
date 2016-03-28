using GFCalc.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Grainsim.Domain
{

    public enum HopAdditionStage {
        Boil,
        Fermentation,
        Keg
    } 

    //[ValueConversion(typeof(Amount), typeof(AmountGrams))]
    public class HopAddition
    {
        [XmlElement("Hop")]
        public Hops Hop { set; get; }
        [XmlAttribute("Name")]
        public double Amount { get; set; }
        [XmlAttribute("AmountGrams")]
        public double AmountGrams { get; set; }
        [XmlAttribute("Duration")]
        public int Duration { get; set; }
        [XmlAttribute("Stage")]
        public HopAdditionStage Stage { get; set; }
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
        public string Stage { get; set; }


    }


}

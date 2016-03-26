using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GFCalc.DataModel;
using Grainsim.Domain;
using System.Xml.Serialization;

namespace GFCalc.Domain
{
    [XmlRoot("Recepie"), Serializable]
    public class Recepie
    {

        public Recepie() 
        {
            BoilHops = new List<HopAddition>();
            MashProfile = new List<MashProfileStep>();
            Fermentables = new List<GristPart>();

        }
        [XmlAttribute("Name")]
        public string Name { get; set; }

        [XmlElement("Grist")]
        public List<GristPart> Fermentables { set; get; }
        [XmlElement("MashProfile")]
        public List<MashProfileStep> MashProfile { set; get; }

        [XmlElement("BoilHops")]
        public List<HopAddition> BoilHops { set; get; }

        [XmlElement("OtherIngredients")]
        public List<OtherIngredient> OtherIngredients { set; get; } 

        [XmlAttribute("BatchSize")]
        public double BatchSize { get; set; }
        [XmlAttribute("OriginalGravity")]
        public double OriginalGravity { get; set; }
        [XmlAttribute("BoilTime")]
        public int BoilTime { set; get; }
        [XmlAttribute("TopUpMashWater")]
        public double TopUpMashWater { set; get; }

    }

    public class MashProfileStep
    {
        [XmlAttribute("Time")]
        public int Time { get; set; }
        [XmlAttribute("Temp")]
        public double Temperature { get; set; }

    }

    public class OtherIngredient
    {
        [XmlAttribute("Name")]
        public string Name { get; set; }
        [XmlAttribute("Amount")]
        public double Amount { set; get; }
        [XmlAttribute("Notes")]
        public string Notes { set; get; }
    }
}

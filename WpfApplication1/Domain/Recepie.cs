using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GFCalc.DataModel;
using WpfApplication1.Domain;
using System.Xml.Serialization;

namespace GFCalc.Domain
{
    [XmlRoot("Recepie"), Serializable]
    public class Recepie
    {
        [XmlAttribute("Name")]
        public string Name { get; set; }

        // Mash part
        [XmlElement("Grist")]
        public List<GristPart> Grist { set; get; }
        [XmlElement("MashProfile")]
        public List<MashProfileStep> MashProfile { set; get; }

        // Boil part
        [XmlElement("BoilHops")]
        public List<HopBoilAddition> BoilHops { set; get; }
        [XmlElement("BoilFermentables")]
        public List<GristPart> BoilFermentables { set; get; }

        // Fermentation part
        [XmlElement("PostBoilFermentables")]
        public List<GristPart> PostBoilFermentables { set; get; }
        [XmlElement("FermetationHops")]
        public List<Hops> FermetationHops { set; get; }
        [XmlElement("FermenationAdjuncts")]
        public List<FermentableAdjunct> FermenationAdjuncts { set; get; }

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
        public int Time {get; set; }
        [XmlAttribute("Temp")]
        public double Temperature { get; set; }

    }
}


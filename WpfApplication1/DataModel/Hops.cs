using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace GFCalc.DataModel
{


    [XmlRoot("HopsData"), Serializable]
    public class HopsData
    {
        public HopsData()
        {
            Hopses = new List<Hops>();
        }

        [XmlElement("Hopses")]
        public List<Hops> Hopses { set; get; }

    }
    public class HopAcids
    {
        [XmlAttribute("Min")]
        public float Min { get; set; }
        [XmlAttribute("Max")]
        public float Max { get; set; }

        public override string ToString()
        {
            if (Max != 0)
                return string.Format("{0:F1}% - {1:F1}%", Min, Max);
            else
                return string.Format("{0:F1}%", Min, Max);
        }

        public float GetAphaAcid()
        {
            if (Max != 0)
                return (Min + Max) / 2;
            else
                return Min;
        }
    }

    public class Hops
    {
        [XmlAttribute("Name")]
        public string Name { get; set; }
        [XmlElement("Characteristics")]
        public string Characteristics { get; set; }

        // Enum -> Bittering, aroma
        [XmlElement("Purpose")]
        public string Purpose { get; set; }
        [XmlElement("AlphaAcid")]
        public HopAcids AlphaAcid { get; set; }
        [XmlElement("BetaAcid")]
        public HopAcids BetaAcid { get; set; }
        [XmlElement("CoHumulone")]
        public HopAcids CoHumulone { get; set; }
        [XmlElement("MyrceneOilComposition")]
        public HopAcids MyrceneOilComposition { get; set; }
        [XmlElement("HumuleneOilComposition")]
        public HopAcids HumuleneOilComposition { get; set; }
        [XmlElement("CaryophylleneOil")]
        public HopAcids CaryophylleneOil { get; set; }
        [XmlElement("FarneseneOil")]
        public HopAcids FarneseneOil { get; set; }
        [XmlElement("Country")]
        public string Country { get; set; }
        [XmlElement("ConeSize")]
        public string ConeSize { get; set; }
        [XmlElement("ConeDensity")]

        public string ConeDensity { get; set; }
        [XmlElement("SeasonalMaturity")]
        public string SeasonalMaturity { get; set; }
        [XmlElement("YieldAmount")]
        public string YieldAmount { get; set; }

        [XmlElement("Substitutes")]
        public string Substitutes { set; get; }
        [XmlElement("StyleGuide")]
        public string StyleGuide { set; get; }

    }
}

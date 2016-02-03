using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace GFCalc.DataModel
{

    [XmlRoot("FermentableAdjunctSerializables"), Serializable]
    public class FermentableAdjunctSerializables
    {

        [XmlElement("FermentableAdjunctSerializable")]
        public List<FermentableAdjunctSerializable> FermentableAdjunctSerializable { get; set; }

    }

    public class FermentableAdjunctSerializable
    {
        [XmlAttribute("Name")]
        public string Name { get; set; }

        [XmlAttribute("Origin")]
        public string Origin { get; set; }

        [XmlAttribute("AdjuctType")]
        public string AdjuctType { get; set; }

        [XmlAttribute("Color")]
        public int Color { get; set; }

        [XmlAttribute("MashNeeded")]
        public bool MashNeeded { get; set; }

        [XmlElement("Potential")]
        public float Potential { get; set; }

        [XmlAttribute("MaxPart")]
        public float MaxPart { get; set; }

    }
}
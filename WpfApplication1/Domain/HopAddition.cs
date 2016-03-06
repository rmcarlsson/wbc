﻿using GFCalc.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace WpfApplication1.Domain
{
    public class HopBoilAddition
    {
        [XmlElement("Hop")]
        public Hops Hop { set; get; }
        [XmlAttribute("Name")]
        public float Amount { get; set; }
        [XmlAttribute("Minutes")]
        public int Minutes { get; set; }
    }

    public class GristPart : FermentableAdjunct
    {
        [XmlElement("FermentableAdjunct")]
        public FermentableAdjunct FermentableAdjunct { get; set; }
        [XmlAttribute("AmountPercent")]
        public double Amount { get; set; }
        [XmlAttribute("AmountGrams")]
        public double AmountGrams { get; set; }


    }


}

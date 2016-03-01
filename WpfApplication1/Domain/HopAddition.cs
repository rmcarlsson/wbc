using GFCalc.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApplication1.Domain
{
    public class HopBoilAddition
    {
        public Hops Hop { set; get; }
        public float Amount { get; set; }
        public int Minutes { get; set; }
    }

    public class GristPart : FermentableAdjunctSerializable
    {
        public FermentableAdjunctSerializable FermentableAdjunct { get; set; }
        public double Amount { get; set; }
        public double AmountKg { get; set; }

    }


}

﻿using GFCalc.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApplication1.Domain
{
    public class HopAddition
    {
        public Hops Hop { set; get; }
        public float Amount { get; set; }
    }

    public class GristPart : FermentableAdjunctSerializable
    {
        public FermentableAdjunctSerializable FermentableAdjunct { get; set; }
        public float Amount { get; set; }
    }


}
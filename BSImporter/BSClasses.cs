using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSImporter
{
    public class BSGrainBill
    {
        public string FermentableName { set; get; }
        public double AmountPercent { set; get; }
    }

    public class BSMashStep
    {
        public int Temperature { set; get; }
        public int TimeMinutes { set; get; }
    }
}




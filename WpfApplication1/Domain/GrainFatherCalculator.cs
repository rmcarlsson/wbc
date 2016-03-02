using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GFCalc.Domain
{
    class GrainFatherCalculator
    {
        private static double SMALL_GRAINBILL = 4.5;
        private static readonly ILog logger = LogManager.GetLogger(typeof(GrainFatherCalculator));

        public static double MashEfficiency = 0.78;

        public static double CalculateMashVolume(double aGrainBillSize)
        {
            if (aGrainBillSize < SMALL_GRAINBILL)
                ;
            var ret = aGrainBillSize * 2.7 + 3.5;
            logger.Debug(string.Format("Total mash volume [L]: {0}L", ret));
            return ret;
        }
    }
}

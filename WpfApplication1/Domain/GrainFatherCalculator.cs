using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GFCalc.Domain
{
    class GrainfatherCalculator
    {
        public const double SMALL_GRAINBILL = 4500;

        private static readonly ILog logger = LogManager.GetLogger(typeof(GrainfatherCalculator));

        public static double MashEfficiency = 0.78;

        public static double BoilOffRate = 2;

        public static double BoilerLoss = 2;
        
        //
        // Input:
        //
        // 1. OG 
        // 2. Batch size (volume into fermentor)
        //
        // Output:
        // 
        // 1. Mash water volume 
        //    Needs: Grain bill size
        //
        // 2. Sparge water volume
        //    Needs: preboil volume = (batch size + boiler loss) + boil off
        //
        //  
        // System constants
        //
        // 1. Boil off rate => boil off volume
        // 2. Boiler loss
        //

        public static double CalcBoilOffVolume(double aPostBoilVolume, int aBoilTime)
        {
            return BoilOffRate * (aBoilTime / 60);
        }

        public static double CalcMashVolume(double aGrainbillSize)
        {
            var ret = ((aGrainbillSize/1000) * 2.7) + 3.5;
            logger.Debug(string.Format("Total mash volume [L]: {0}L", ret));
            return ret;
        }

        public static double CalcSpargeWaterVolume(double aGrainbillSize, double aPreBoilVolume, double aMashTopUpVolume)
        {
            double val = 0;

            
            if (aGrainbillSize > SMALL_GRAINBILL)
                // (28 - mash water volume in L) + (grain bill in kg x 0.8)  = Sparge water volume in L
                val = aPreBoilVolume - CalcMashVolume(aGrainbillSize) + ((aGrainbillSize/1000) * 0.8);
            else
                // (28 – (mash water volume in L + additional water in L)) + (grain bill in kg x 0.8) = Sparge water volume in L
                val = aPreBoilVolume - (CalcMashVolume(aGrainbillSize) + aMashTopUpVolume) + ((aGrainbillSize/1000) * 0.8);
            return val;
        }
    }
}

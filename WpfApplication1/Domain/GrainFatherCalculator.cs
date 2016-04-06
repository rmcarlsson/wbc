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

        public const double GRAINFATHER_MAX_PREBOILVOLUME = 30;

        public const double GRAINFATHER_WATER_TO_GRAIN_RATIO_IN_MASH = 2.7;

        //public const double GRAINFATHER_BOILER_TO_FERMENTOR_LOSS = 2.5;

        public const double GRAIN_WATER_ABSORBTION = (0.652/1000); // l/kg

        public const double MASH_VOLUME_INCREASE_WITH_GRAINS = (0.325/1000); //l/kg

        private static readonly ILog logger = LogManager.GetLogger(typeof(GrainfatherCalculator));

        public static double MashEfficiency = 0.78;

        public const double BOIL_OFF_RATE = 2;

        public static double GRAINFATHER_BOILER_TO_FERMENTOR_LOSS = 1;
        
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

        public static double CalcBoilOffVolume(int aBoilTime)
        {
            return BOIL_OFF_RATE * (aBoilTime / 60);
        }

        public static double CalcMashVolume(double aMashGrainbillSize)
        {
            var ret = ((aMashGrainbillSize / 1000) * GRAINFATHER_WATER_TO_GRAIN_RATIO_IN_MASH) + 3.5;
            logger.Debug(string.Format("Total mash volume [L]: {0}L", ret));

            var grainVolIncrease = (MASH_VOLUME_INCREASE_WITH_GRAINS * aMashGrainbillSize);
            if ((ret + grainVolIncrease) > GRAINFATHER_MAX_PREBOILVOLUME)
                throw new ArgumentException("Grainbill to big. It will not fit. Please reduce grainbill");

            return ret;
        }

        public static double CalcSpargeWaterVolume(double aMashGrainbillSize, double aPreBoilVolume, double aMashTopUpVolume)
        {
            double val = 0;

            
            if (aMashGrainbillSize > SMALL_GRAINBILL)
                // (28 - mash water volume in L) + (grain bill in kg x 0.8)  = Sparge water volume in L
                val = aPreBoilVolume - CalcMashVolume(aMashGrainbillSize) + ((aMashGrainbillSize/1000) * 0.8);
            else
                // (28 – (mash water volume in L + additional water in L)) + (grain bill in kg x 0.8) = Sparge water volume in L
                val = aPreBoilVolume - (CalcMashVolume(aMashGrainbillSize) + aMashTopUpVolume) + ((aMashGrainbillSize/1000) * 0.8);
            var grainAborbtionVol = (GRAIN_WATER_ABSORBTION * aMashGrainbillSize);
            if ((CalcMashVolume(aMashGrainbillSize) + val - grainAborbtionVol) > GRAINFATHER_MAX_PREBOILVOLUME)
                throw new ArgumentException("Grainbill to big. It will not fit. Please reduce grainbill");

            return val;
        }

        public static double CalcPreBoilVolume(double aBatchSize, int aBoilTime)
        {
            return CalcBoilOffVolume(aBoilTime) + aBatchSize;
        }
    }
}

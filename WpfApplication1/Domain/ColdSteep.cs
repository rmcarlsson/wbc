using GFCalc.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grainsim.Domain
{
    public class ColdSteepAddition
    {
        public int Weight { set; get; }
        public double WaterContribution { set; get; }
    }

    public class ColdSteep
    {
        public const double COLDSTEEP_WATER_TO_GRAIN_RATION = 5;
        public const double COLDSTEEP_EFFICIENCY = 0.80;
        public const double COLDSTEEP_VOLUME_TO_SPARGE_RATIO = 0.1;

        public static IEnumerable<GristPart> CalculateColdSteepGrainBill(IEnumerable<GristPart> aGrist)
        {
            var ret = aGrist.Where(x => (x.Stage == FermentableStage.ColdSteep));
            return ret;
        }

        public static void GetColdSteepCompensatedWeight(int aGrainWeight, out ColdSteepAddition aColdSteepAddition)
        {
            ColdSteepAddition ret = new ColdSteepAddition();
            ret.Weight = (int)Math.Round((double)aGrainWeight / COLDSTEEP_EFFICIENCY);
            ret.WaterContribution = ((double)ret.Weight * COLDSTEEP_WATER_TO_GRAIN_RATION)/1000d;
            ret.WaterContribution += ret.WaterContribution * COLDSTEEP_VOLUME_TO_SPARGE_RATIO;

            aColdSteepAddition = ret;
        }

        public static double GetColdSteepWaterContibution(int aGrainWeight)
        {
            // All weights are in grams, all volumes are in liters, hence divide by 1000;
            var totalVolToAdd = ((aGrainWeight / COLDSTEEP_EFFICIENCY) * COLDSTEEP_WATER_TO_GRAIN_RATION) / 1000;

            var grainAborbtionVol = (GrainfatherCalculator.GRAIN_WATER_ABSORBTION * aGrainWeight);

            return (totalVolToAdd - grainAborbtionVol);
        }

        //public static int GetGrainBillSize(double somePoints, double grainBillSize)
        //{
        //    double ret = 0;
        //    //finalPoints
        //    ret = ((grain.Amount * grainBillSize) / 100) / COLDSTEEP_EFFICIENCY;

        //    return (int)(Math.Round(ret));
        //}

        public static double GetWaterToAddToColdSteep(double aGrainAmountGrams)
        {
            return (aGrainAmountGrams * COLDSTEEP_WATER_TO_GRAIN_RATION)/1000;
        }


    }
}

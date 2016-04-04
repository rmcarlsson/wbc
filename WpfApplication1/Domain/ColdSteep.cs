using GFCalc.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grainsim.Domain
{
    public class ColdSteep
    {
        public const double COLDSTEEP_WATER_TO_GRAIN_RATION = 5;
        public const double COLDSTEEP_EFFICIENCY = 0.85;

        public static IEnumerable<GristPart> CalculateColdSteepGrainBill(IEnumerable<GristPart> aGrist)
        {
            var ret = aGrist.Where(x => (x.Stage == FermentableStage.ColdSteep));
            return ret;
        }

        public static double GetColdSteepWaterContibution(IEnumerable<GristPart> aGrist, double GrainbillSize)
        {

            var l = CalculateColdSteepGrainBill(aGrist);
            var grainBillSizeColdSteep = l.Sum(x => (((double)(x.Amount)/100d) * GrainbillSize));

            // All weights are in grams, all volumes are in liters, hence divide by 1000;
            var totalVolToAdd = ((grainBillSizeColdSteep/ COLDSTEEP_EFFICIENCY) * COLDSTEEP_WATER_TO_GRAIN_RATION) / 1000;

            var grainAborbtionVol = (GrainfatherCalculator.GRAIN_WATER_ABSORBTION * grainBillSizeColdSteep);

            return (totalVolToAdd - grainAborbtionVol);
        }

        public static int GetGrainBillSize(GristPart grain, double grainBillSize)
        {
            double ret = 0;

            ret = ((grain.Amount * grainBillSize) / 100) / COLDSTEEP_EFFICIENCY;

            return (int)(Math.Round(ret));
        }

        public static double GetWaterToAddToColdSteep(double aGrainAmountGrams)
        {
            return (aGrainAmountGrams * COLDSTEEP_WATER_TO_GRAIN_RATION)/1000;
        }


    }
}

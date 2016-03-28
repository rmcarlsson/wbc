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
            var ret = aGrist.Where(x => (x.Stage.Equals("Steep")));
            return ret;
        }

        public static double GetColdSteepWaterContibution(IEnumerable<GristPart> aGrist, double GrainbillSize)
        {
            double ret = 0;

            var l = CalculateColdSteepGrainBill(aGrist);
            ret = l.Sum(x => ((x.Amount * GrainbillSize) / COLDSTEEP_EFFICIENCY));

            // All weights are in grams, all volumes are in liters, hence divide by 1000;
            ret = (ret * COLDSTEEP_WATER_TO_GRAIN_RATION) / 1000;

            return ret;
        }

        public static int GetGrainBillSize(GristPart grain, double grainBillSize)
        {
            double ret = 0;

            ret = ((grain.Amount * grainBillSize) / 100) / COLDSTEEP_EFFICIENCY;

            return (int)(Math.Round(ret));
        }
    }
}

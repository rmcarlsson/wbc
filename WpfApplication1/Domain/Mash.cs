using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grainsim.Domain
{
    public class Mash
    {

        public static double CalculateMashGrainBillSize(IEnumerable<GristPart> aGrist, double aGrainBillSize)
        {
            var ret = aGrist.Where(x => (x.Stage == FermentableStage.Mash));
            var sum = ret.Sum( x => (x.AmountGrams)/100d);

            return sum;
        }


    }
}

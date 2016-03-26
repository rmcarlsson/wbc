using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grainsim.Domain
{
    static public class IbuAlgorithms
    {
        // 28.3495231 grams /
        // 3.78541178 liters
        // = 7.48915170862



        static public int CalcIbu(IEnumerable<HopAddition> aAdditions, double aGravityBoil, double aVolumeBoil)
        {
            if (aVolumeBoil == 0)
                return 0;
            if (aAdditions.Count() == 0)
                return 0;
            if (aGravityBoil < 1)
                return 0;

            var fGravity = 1.65 * Math.Pow(0.000125, (aGravityBoil - 1));
            var res = 0d;
            foreach (HopAddition hopAdd in aAdditions) 
            {
                var fTinseth = (1 - Math.Pow(Math.E, (- 0.04 * hopAdd.Minutes))) / 4.15;
                var hopAmountGrams = (hopAdd.Amount * aVolumeBoil);

                res += fGravity * fTinseth * (hopAdd.Hop.AlphaAcid.GetAphaAcid() * hopAmountGrams * 10 ) / aVolumeBoil;
            }

            return Convert.ToInt32(res);
        }
    }
}

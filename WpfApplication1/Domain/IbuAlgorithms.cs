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

            public const double IBU_TOLERANCE = 0.5;



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
                var fTinseth = (1 - Math.Pow(Math.E, (-0.04 * hopAdd.Duration))) / 4.15;
                var utilization = fGravity * fTinseth;
                float decimal_aa = 0;
                if (hopAdd.AlphaAcid != null)
                    decimal_aa = hopAdd.AlphaAcid.GetAphaAcid() / (100);
                else
                    decimal_aa = hopAdd.Hop.AlphaAcid.GetAphaAcid() / 100;
                if (hopAdd.AmountUnit == HopAmountUnitE.GRAMS_PER_LITER)
                {
                    var hopAmountGrams = (hopAdd.Amount * aVolumeBoil);
                    var mg_per_liter = (decimal_aa * hopAmountGrams * 1000) / aVolumeBoil;
                    var ibu = utilization * mg_per_liter;
                    hopAdd.Bitterness = Convert.ToInt32(Math.Round(ibu));
                    res += ibu;
                }
                else
                {
                    // Calculate amount instead
                    var gramAlphaAcidPerGramHumle = decimal_aa * utilization * 1000;
                    var totalmgAlpaAcidNeeded = aVolumeBoil * hopAdd.Bitterness;
                    hopAdd.AmountGrams = totalmgAlpaAcidNeeded / gramAlphaAcidPerGramHumle;
                    // Just add up the IBU to the total sum
                    res += hopAdd.Bitterness;

                }

                //hopAdd.AmountGrams = Math.Round((aVolumeBoil * hopAdd.Amount));
            }

            return Convert.ToInt32(res);
        }
    }
}

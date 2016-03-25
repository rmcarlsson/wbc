using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grainsim.Domain
{
    public class ColorAlgorithms
    {
        public const double SRM_TO_EBC = 1.97;
        public static int CalculateColor(List<GristPart> aMashFermentList, double aBatchSize)
        {
            // MCU = (Weight of grain in lbs) *(Color of grain in degrees lovibond) / (volume in gallons)
            double mcu = 0;
            foreach (GristPart g in aMashFermentList)
                mcu += (Weight.ConvertToPounds(g.AmountGrams) *
                        g.FermentableAdjunct.Color) / aBatchSize;

            double srm = 1.4922 * Math.Pow(mcu, 0.6859d);
            return (int)(Math.Round(srm));
        }
    }
}

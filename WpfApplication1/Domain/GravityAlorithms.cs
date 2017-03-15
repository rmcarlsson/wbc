using GFCalc.DataModel;
using GFCalc.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grainsim.Domain
{
    public class GravityAlgorithms
    {
        public static double GetPoints(double aGravity, double aVolume)
        {
            return (aGravity - 1) * 1000 * Volume.ConvertLitersToGallons(aVolume);
        }

        public static double GetGravity(double somePoints, double aVolume)
        {
            return ((somePoints / Volume.ConvertLitersToGallons(aVolume)) / 1000) + 1;
        }

        public static double GetGravity(double aVolume, List<GristPart> aFermentableList, double aMashEfficiency)
        {
            if (aVolume == 0 || (aFermentableList.Count == 0) || aFermentableList == null)
                return 1;

            double potentialSumPointsPerGallon = 0;
            if (aFermentableList != null)
                potentialSumPointsPerGallon += aFermentableList.Sum(x => (((x.FermentableAdjunct.Potential - 1) * 1000) * ((double)Weight.ConvertToPounds(x.AmountGrams))));
            var ppgLbMashEffComp = aMashEfficiency * potentialSumPointsPerGallon;
            var g = ppgLbMashEffComp / Volume.ConvertLitersToGallons(aVolume);

            return (1 + (g / 1000));
        }


        public static int GetGrainWeight(double somePoints, double aPotential, double aMashEfficiency)
        {
            var ppg = (aPotential - 1) * 1000 * aMashEfficiency;
            return Weight.ConvertPoundsToGrams(somePoints / ppg);

        }





        public static int GetTotalGravity(double aVolume, double aOriginalGravity)
        {
            var totalGravityPoints = (aOriginalGravity - 1) * 1000;
            return Convert.ToInt32(Math.Round(Volume.ConvertLitersToGallons(aVolume) * totalGravityPoints));
        }
    }
}

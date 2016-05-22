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


        public static double GetPostBoilVolume(double aGravity, List<GristPart> aMashFermentList, List<GristPart> aPostMashFermentList, int aMashEfficiency)
        {
            if (aGravity <= 0)
                return 0;

            double potentialSum = aMashFermentList.Sum(x => x.Amount);
            potentialSum += aPostMashFermentList.Sum(x => x.Amount);

            return potentialSum / aGravity;
        }

        public static int GetGrainWeight(double somePoints, double aPotential)
        {
            var gfc = new GrainfatherCalculator();

            var ppg = (aPotential - 1) * 1000 * gfc.MashEfficiency;
            return Weight.ConvertPoundsToGrams(somePoints / ppg);

        }

        public static int GetGrainBillWeight(double aGravity, double aBatchSizeVolume, List<GristPart> aFermentableList, double aMashEfficiency)
        {
            if (aGravity <= 1)
                return 0;

            // SG = (Malt weight) x(Malt ppg) x(mash efficiency) / (Solution Volume)
            // (Malt SG * (Solution Volume) ) / (Malt ppg) x(mash efficiency)

            double potentialSum = 0;
            if (aFermentableList != null)
                potentialSum += aFermentableList.Sum(x => (x.FermentableAdjunct.Potential * x.Amount) / 100);

            var ppg = (potentialSum - 1) * 1000;
            var bhe = aMashEfficiency;
            var ppgBheComp = bhe * ppg;
            var sgGallons = ((aGravity - 1) * 1000) * Volume.ConvertLitersToGallons(aBatchSizeVolume);


            var retLb = (sgGallons / ppgBheComp);
            var ret = Weight.ConvertPoundsToGrams(retLb);



            // points per kilogram per liter = 8.346 (points/ lb/gal)
            return ret;
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

        public static double GetGravityByPart(
            double aVolume,
            List<GristPart> aFermentableList,
            int aGrainbillWeight,
            double aMashEfficiency)
        {
            if (aVolume == 0 || (aFermentableList.Count == 0) || aFermentableList == null)
                return 1;

            var lbs = Weight.ConvertToPounds(aGrainbillWeight);

            double ppgLbMashEffComp = 0;
            foreach (var f in aFermentableList)
            {
                double eff = 1;
                switch (f.Stage)
                {
                    case FermentableStage.Mash:
                        var gfc = new GrainfatherCalculator();
                        eff = gfc.MashEfficiency;
                        break;
                    case FermentableStage.ColdSteep:
                        eff = ColdSteep.COLDSTEEP_EFFICIENCY;
                        break;
                    default:
                        eff = 1;
                        break;
                }
                ppgLbMashEffComp += eff * ((f.FermentableAdjunct.Potential - 1) * 1000) * lbs * ((double)(f.Amount) / 100d);

            }
            var g = ppgLbMashEffComp / Volume.ConvertLitersToGallons(aVolume);

            return (1 + (g / 1000));
        }

    }
}

using GFCalc.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grainsim.Domain
{
    public class GravityAlorithms
    {
        public static double GetPostBoilVolume(double aGravity, List<GristPart> aMashFermentList, List<GristPart> aPostMashFermentList, int aMashEfficiency)
        {
            if (aGravity <= 0)
                return 0;

            double potentialSum = aMashFermentList.Sum(x => x.Amount);
            potentialSum += aPostMashFermentList.Sum(x => x.Amount);

            return potentialSum / aGravity;
        }

        public static int GetGrainBillWeight(double aGravity, double aBatchSizeVolume, List<GristPart> aMashFermentList, List<GristPart> aPostMashFermentList, double aMashEfficiency)
        {
            if (aGravity <= 1)
                return 0;

            // SG = (Malt weight) x(Malt ppg) x(mash efficiency) / (Solution Volume)
            // (Malt SG * (Solution Volume) ) / (Malt ppg) x(mash efficiency)

            double potentialSum = 0;
            if (aMashFermentList != null)
                potentialSum += aMashFermentList.Sum(x => (x.FermentableAdjunct.Potential * x.Amount) / 100);

            var ppg = (potentialSum - 1) * 1000;
            var bhe = aMashEfficiency;
            var ppgBheComp = bhe * ppg;
            var sgGallons = ((aGravity - 1) * 1000) * Volume.ConvertLitersToGallons(aBatchSizeVolume);


            var retLb = (sgGallons / ppgBheComp);
            var ret = Weight.ConvertPoundsToGrams(retLb);



            // points per kilogram per liter = 8.346 (points/ lb/gal)
            return (int)(Math.Round(ret));
        }

        public static double CalcGravity(double aVolume, double aGrainBillSize, List<GristPart> aMashFermentList, double aMashEfficiency)
        {
            if (aVolume == 0)
                return 1;

            var gbsLb = Weight.ConvertToPounds(aGrainBillSize);
            double potentialSumPointsPerGallon = 0;
            if (aMashFermentList != null)
                potentialSumPointsPerGallon += aMashFermentList.Sum(x => (((x.FermentableAdjunct.Potential - 1) * 1000) * gbsLb * (double)((double)x.Amount / 100d)));
            var ppgLbMashEffComp = aMashEfficiency * potentialSumPointsPerGallon;
            var g = ppgLbMashEffComp / Volume.ConvertLitersToGallons(aVolume);

            return (1 + (g / 1000));
        }


    }
}

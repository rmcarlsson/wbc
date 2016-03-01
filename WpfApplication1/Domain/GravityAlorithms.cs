using GFCalc.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApplication1.Domain
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

        public static double GetMashGrainBillWeight(double aGravity, double aVolume, List<GristPart> aMashFermentList, List<GristPart> aPostMashFermentList, double aMashEfficiency)
        {
            if (aGravity <= 0)
                return 0;

            // Malt SG = (Malt weight) x(Malt ppg) x(Brewhouse efficiency) / (Solution Volume)
            // (Malt SG * (Solution Volume) ) / (Malt ppg) x(Brewhouse efficiency)

            double potentialSum = 0;
            if (aMashFermentList != null)
                potentialSum += aMashFermentList.Sum(x => (x.FermentableAdjunct.Potential * x.Amount)/100);
            if (aPostMashFermentList != null)
                potentialSum -= aPostMashFermentList.Sum(x => x.FermentableAdjunct.Potential);

            var potentialPlato = (potentialSum - 1) * 1000;
            var eff = (1 + (100 - aMashEfficiency)/100);
            var PlatoEffComp = eff * potentialPlato;
            var sgLiters = ((aGravity - 1) * 1000) * aVolume;


            var ret = (sgLiters / PlatoEffComp) / 8.346;


            // points per kilogram per liter = 8.346 (points/ lb/gal)
            return ret;
        }


    }
}

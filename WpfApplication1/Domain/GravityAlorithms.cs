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

        public static double GetMashGrainBillWeight(double aGravity, double aVolume, List<GristPart> aMashFermentList, List<GristPart> aPostMashFermentList, int aMashEfficiency)
        {
            if (aGravity <= 0)
                return 0;

            double potentialSum = aMashFermentList.Sum(x => x.Amount);
            potentialSum -= aPostMashFermentList.Sum(x => x.Amount);

            return ((potentialSum * aVolume)  / aGravity) / 100;
        }


    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grainsim.Domain
{
    public class Weight
    {
        static public double ConvertToPounds(double aWeightInGrams)
        {
            return aWeightInGrams * 0.00220462262d;
        }

        static public double ConvertPoundsToGrams(double aWeightInPounds)
        {
            return aWeightInPounds / 0.00220462262d;
        }
    }

    static class Volume
    {
        static public double ConvertLitersToGallons(double aVolumeInLiters)
        {
            return 0.264172052d * aVolumeInLiters;
        }
    }

}

using GFCalc.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grainsim.Domain
{

    public static class ColorAlgorithms
    {
        public const double SRM_TO_EBC = 1.97d;

        class ReferenceBeerColor
        {
            public double Color;
            public string Name;
        }

        static List<ReferenceBeerColor> RefColor = new List<ReferenceBeerColor> {

            new ReferenceBeerColor() { Color = 3.0, Name = "Pale yellow color" },
            new ReferenceBeerColor() { Color = 4.5, Name = "Medium yellow" },
            new ReferenceBeerColor() { Color = 7.5, Name = "Gold" },
            new ReferenceBeerColor() { Color = 9.0, Name = "Amber" },
            new ReferenceBeerColor() { Color = 11.0, Name = "Copper" },
            new ReferenceBeerColor() { Color = 14.0, Name = "Red/Brown" },
            new ReferenceBeerColor() { Color = 19.0, Name = "Brown" },
            new ReferenceBeerColor() { Color = 20.0, Name = "Black" }
        };

        public static string GetReferenceBeer(double aEcb)
        {
            if (aEcb < RefColor.First().Color)
                return RefColor.First().Name;

            if (aEcb > RefColor.Last().Color)
                return RefColor.Last().Name;

            int ix = RefColor.FindIndex(x => (x.Color > aEcb));
            return RefColor.ElementAt(ix - 1).Name;
        }

        public static double CalculateColor(List<GristPart> aMashFermentList, BrewVolumes aSetOfBrewVolumes)
        {
            // MCU = (Weight of grain in lbs) *(Color of grain in degrees lovibond) / (volume in gallons)
            double mcu = 0;
            foreach (GristPart g in aMashFermentList)
            {

                double w = g.AmountGrams;
                if (g.Stage == FermentableStage.Mash)
                {
                    double tapOffComp = (aSetOfBrewVolumes.PreBoilTapOff / (aSetOfBrewVolumes.PreBoilVolume + aSetOfBrewVolumes.PreBoilTapOff));
                    w = (1-tapOffComp) * g.AmountGrams;
                }
                if (g.Stage == FermentableStage.Fermentor)
                {
                    double boilToFerComp = GrainfatherCalculator.GRAINFATHER_BOILER_TO_FERMENTOR_LOSS / (aSetOfBrewVolumes.PostBoilVolume);
                    w = (1 + boilToFerComp) * g.AmountGrams;
                }

                mcu += (Weight.ConvertToPounds(w) * g.FermentableAdjunct.Color) / Volume.ConvertLitersToGallons(aSetOfBrewVolumes.PostBoilVolume);
            }
            double srm = 1.4922 * Math.Pow(mcu, 0.6859d);
            return (srm * SRM_TO_EBC);
        }
    }
}


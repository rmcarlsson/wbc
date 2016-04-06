namespace GFCalc.Domain
{
    internal interface IBrewSystemCalculator
    {

        double CalcBoilOffVolume(double aPostBoilVolume, int aBoilTime);

        double CalcMashVolume(double aMashGrainbillSize);

        double CalcSpargeWaterVolume(double aMashGrainbillSize, double aPreBoilVolume, double aMashTopUpVolume);

        double CalcPreBoilVolume(double aBatchSize, int aBoilTime);
    }
}
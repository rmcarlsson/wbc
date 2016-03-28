namespace BSImport
{
    public class BSGrainBill
    {
        public string FermentableName { set; get; }
        public int AmountPercent { set; get; }
    }

    public class BSHops
    {
        public string Name { get; set; }
        public double AlphaAcid { get; set; }
        public double DryHopDay { get; internal set; }
        public int BoilTime { get; internal set; }
        public double Amount { get; internal set; }
    }

    public class BSHopSchedule
    {
        public BSHops Hop { get; set; }
        public int BoilTime { get; set; }
    }

    public class BSMashStep
    {
        public int Temperature { set; get; }
        public int TimeMinutes { set; get; }
    }
}




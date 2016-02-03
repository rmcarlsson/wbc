using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GFCalc.DataModel;

namespace GFCalc.Domain
{
    class Recepie
    {

        // Mash part
        public List<FermentableAdjunct> Grist { set; get; }
        public MashProfile MashProfile { set; get; }

        //Boil part
        public List<Hops> BoilHops { set; get; }
        public List<FermentableAdjunct> BoilFermentables { set; get; }

        // Fermentation part
        public List<FermentableAdjunct> PostBoilFermentables { set; get; }
        public List<Hops> FermetationHops { set; get; }
        public List<FermentationAdjunct> FermenationAdjuncts { set; get; }

    }

    class MashProfile
    {
        public string Name { get; set; }
        public List<MashProfileStep> Steps {  get; set; }
 
    }

    class MashProfileStep
    {
        public TimeSpan Time {get; set; }
        public float Temperature { get; set; }

    }
}


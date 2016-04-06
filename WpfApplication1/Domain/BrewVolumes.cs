using GFCalc.Domain;
using Grainsim.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grainsim.Domain
{
    class BrewVolumes
    {

        private double finalBatchVolume;
        private double preBoilTapOff;

        public double TotalBatchVolume { set; get; }
        public double FinalBatchVolume
        {
            set
            {
                finalBatchVolume = value;
                TotalBatchVolume = finalBatchVolume + PreBoilTapOff;
            }
            get
            {
                return finalBatchVolume;
            }
        }

        public double PreBoilTapOff
        {
            set
            {
                preBoilTapOff = value;
                TotalBatchVolume = finalBatchVolume + preBoilTapOff;
            }
            get
            {
                return preBoilTapOff;
            }
        }
    }
}
using GFCalc.Domain;
using Grainsim.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grainsim.Domain
{
    public class BrewVolumes
    {

        private double finalBatchVolume;
        private double boilerToFermentorLoss;
        private double boilOffLoss;
        private double coldSteepVolume;
        private double preBoilVolume;

        /// <summary>
        /// This volume is the expected volume out from the counter flow chiller going into the fermentor
        /// </summary>
        public double FinalBatchVolume
        {
            set
            {
                finalBatchVolume = value;
                UpdateTotalBatchVolume();
            }
            get
            {
                return finalBatchVolume;
            }
        }

   
        /// <summary>
        /// This is the volume contribution froma cold steep contribution. It is added 
        /// after mash in the end of the boil.
        /// </summary>
        public double ColdSteepVolume
        {
            set
            {
                coldSteepVolume = value;
                UpdateTotalBatchVolume();
            }
            get
            {
                return coldSteepVolume;
            }
        }

        /// <summary>
        /// This is the volume loss when moving from boiler to fermentor.
        /// </summary>
        public double BoilerToFermentorLoss
        {
            set
            {
                boilerToFermentorLoss = value;
                UpdateTotalBatchVolume();
            }
            get
            {
                return boilerToFermentorLoss;
            }
        }

        /// <summary>
        /// Vaporation loss during boil
        /// </summary>
        public double BoilOffLoss
        {
            set
            {
                boilOffLoss = value;
                UpdateTotalBatchVolume();
            }
            get
            {
                return boilOffLoss;
            }
        }

        /// <summary>
        /// Volume needed before boil to get FinalBatchVolume.
        /// 
        /// Calculated value. Not allowed to set out side class.
        /// </summary>
        public double PreBoilVolume
        {
            get
            {
                return preBoilVolume;
            }
            set
            {
                preBoilVolume = value;
                UpdateTotalBatchVolume();
            }
        }

        /// <summary>
        /// This is the volume that a brew would like to "tap off" after mashing, before boil.
        /// 
        /// Examples might be wort to save for future starters, in this case the brewer wants 
        /// the wort before he/she adds hops to the wort.
        /// </summary>
        public double PreBoilTapOff;

        public double PostBoilVolume { get; private set; }

        private void UpdateTotalBatchVolume()
        {
            // One part left in the boiler, one part out of the counter flow 
            // chiller and one part taken away between mash and boil.
            PostBoilVolume = boilerToFermentorLoss + finalBatchVolume;

            preBoilVolume = PostBoilVolume + boilOffLoss - ColdSteepVolume;
        }
    }
}
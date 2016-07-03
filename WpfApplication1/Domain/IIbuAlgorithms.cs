using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApplication1.Domain
{
    interface IIbuAlgorithms
    {
        int CalcIbu(IEnumerable<Grainsim.Domain.HopAddition> aAdditions, double aGravityBoil, double aPostBoilVolume);
    }
}

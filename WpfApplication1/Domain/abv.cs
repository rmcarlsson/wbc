using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApplication1.Domain
{
    static class Abv
    {
        static public double CalculateAbv(double og, double fg)
        {
            var abv = (76.08 * (og - fg) / (1.775 - og)) * (fg / 0.794);
            return abv;
        }
    }
}

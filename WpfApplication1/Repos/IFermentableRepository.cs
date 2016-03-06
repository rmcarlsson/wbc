using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GFCalc.DataModel;

namespace GFCalc.Repos
{
    interface IFermentableRepository
    {
        double GetPotential(string aMaltName);
        IEnumerable<FermentableAdjunct> Get();
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApplication1.DataModel;

namespace WpfApplication1.Repos
{
    interface IFermentableRepository
    {
        double GetPotential(string aMaltName);
        IEnumerable<FermentableAdjunct> Get();
    }
}

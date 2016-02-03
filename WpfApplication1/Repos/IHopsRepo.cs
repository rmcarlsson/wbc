using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GFCalc.DataModel;

namespace GFCalc.Repos
{
    interface IHopsRepo
    {
        IEnumerable<Hops> Get();
       
    }
}

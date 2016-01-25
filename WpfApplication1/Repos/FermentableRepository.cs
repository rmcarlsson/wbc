using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApplication1.DataModel;

namespace WpfApplication1.Repos
{
    public class FermentableRepository : IFermentableRepository
    {

        public FermentableRepository()
        {
            ferms = new List<FermentableAdjunct>();
            ferms.Add(new FermentableAdjunct { Name = "Pilsner malt", Potential = 1.032 });
            ferms.Add(new FermentableAdjunct { Name = "Wheat malt", Potential = 1.032 });
            ferms.Add(new FermentableAdjunct { Name = "Sugar", Potential = 1.046 });
        }

        public List<FermentableAdjunct> ferms;

        public double GetPotential(string aMaltName)
        {
            double ret = 0;

            ferms.First(s => s.Name.Equals(aMaltName));

            return ret;
        }

        public IEnumerable<FermentableAdjunct> Get()
        {
            return ferms;
        }

    }
}

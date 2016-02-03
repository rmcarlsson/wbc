using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using GFCalc.DataModel;

namespace GFCalc.Repos
{
    public class FermentableRepository : IFermentableRepository
    {

        public FermentableRepository()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(FermentableAdjunctSerializables));
            FileStream loadStream = new FileStream(@"C:\Users\rmcar\Documents\Visual Studio 2015\Projects\ConsoleApplication1\ConsoleApplication1\bin\Debug\maltsOut.xml", FileMode.Open, FileAccess.Read);
            FermentableAdjunctSerializables loadedObject = (FermentableAdjunctSerializables)serializer.Deserialize(loadStream);
            loadStream.Close();
            ferms = loadedObject.FermentableAdjunctSerializable;

            //ferms = new List<FermentableAdjunctSerializable>();
            //ferms.Add(new FermentableAdjunctSerializable { Name = "Pilsner malt", Potential = 1.032F, Color = 32, MashNeeded = false, MaxPart = 10.0F, Origin = "Sweden", AdjuctType = "Other" });
            //ferms.Add(new FermentableAdjunctSerializable { Name = "Wheat malt", Potential = 1.032F, Color = 32, MashNeeded = false, MaxPart = 10.0F, Origin = "Sweden", AdjuctType = "Other" });
            //ferms.Add(new FermentableAdjunctSerializable { Name = "Sugar", Potential = 1.046F, Color = 32, MashNeeded = false, MaxPart = 10.0F, Origin = "Sweden", AdjuctType = "Other" });
            //var drop = new FermentableAdjunctSerializables();
            //drop.FermentableAdjunctSerializable = ferms;

            //XmlSerializer serializer = new XmlSerializer(typeof(FermentableAdjunctSerializables));
            //var outFile = new FileStream(@"C:\Users\rmcar\Documents\Visual Studio 2015\Projects\ConsoleApplication1\ConsoleApplication1\bin\Debug\maltsOut.xml", FileMode.CreateNew, FileAccess.Write);
            //serializer.Serialize(outFile, drop);
            //outFile.Close();

        }

        public List<FermentableAdjunctSerializable> ferms;

        public double GetPotential(string aMaltName)
        {
            double ret = 0;

            ferms.First(s => s.Name.Equals(aMaltName));

            return ret;
        }

        public IEnumerable<FermentableAdjunctSerializable> Get()
        {
            return ferms;
        }

    }
}

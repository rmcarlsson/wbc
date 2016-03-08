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

        public const string MALT_DATA_FILEPATH = @"C:\Users\rmcar\Documents\Visual Studio 2015\Projects\WpfApplication1\WpfApplication1\bin\Debug\maltsOut.xml";
        public const string MALT_DATA_FILEPATH_SAVE = @"maltData.xml";

        public FermentableRepository()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(FermentableAdjuncts));
            FileStream loadStream = new FileStream(MALT_DATA_FILEPATH, FileMode.Open, FileAccess.Read);
            FermentableAdjuncts loadedObject = (FermentableAdjuncts)serializer.Deserialize(loadStream);
            loadStream.Close();
            ferms = loadedObject.FermentableAdjunct;
        }

        public void Persist()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(FermentableAdjuncts));
            FileStream saveStream = new FileStream(MALT_DATA_FILEPATH_SAVE, FileMode.Create, FileAccess.Write);
            var f = new FermentableAdjuncts();
            f.FermentableAdjunct = ferms;
            serializer.Serialize(saveStream, f);
            saveStream.Close();
        }


        public List<FermentableAdjunct> ferms;

        public void AddFermentable(FermentableAdjunct aFermentable, bool aUpdatedEnabled)
        {
            bool ret = true;

            var found = ferms.FirstOrDefault(x => x.Name == aFermentable.Name);
            if (aUpdatedEnabled)
            {
                ferms.Add(aFermentable);
                if (found != null)
                    ferms.Remove(found);
            }
            else if (found == null)
                ferms.Add(aFermentable);
            else
                throw new ArgumentException(String.Format("Fermetbale with name = {0] is already present. Please use another name.", aFermentable.Name));

        }


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

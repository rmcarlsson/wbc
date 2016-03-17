using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GFCalc.DataModel;
using System.Xml.Serialization;
using System.IO;

namespace GFCalc.Repos
{
    public class HopsRepository : IHopsRepo
    {
        public const string HOPS_DATA_FILEPATH = @"C:\Users\carltmik\Source\PrivateRepos\wbc\WpfApplication1\bin\Debug\hopses.xml";
        public const string HOPS_DATA_FILEPATH_SAVE = "hopsesData.xml";

        public List<Hops> hopses;
        public HopsRepository()
        {
            if (hopses == null)
            {
                XmlSerializer serializer = new XmlSerializer(typeof(HopsData));
                FileStream loadStream = new FileStream(HOPS_DATA_FILEPATH, FileMode.Open, FileAccess.Read);
                HopsData loadedObject = (HopsData)serializer.Deserialize(loadStream);
                loadStream.Close();
                hopses = loadedObject.Hopses;
            }
        }


        public void Persist()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(HopsData));
            FileStream saveStream = new FileStream(HOPS_DATA_FILEPATH_SAVE, FileMode.Create, FileAccess.Write);
            var h = new HopsData();
            h.Hopses = hopses;
            serializer.Serialize(saveStream, h);
            saveStream.Close();
        }

        public IEnumerable<Hops> Get()
        {
            return hopses;
        }


        public void AddHops(Hops aHop, bool aUpdatedEnabled)
        {

            var found = hopses.FirstOrDefault(x => x.Name == aHop.Name);
            if (aUpdatedEnabled)
            {
                hopses.Add(aHop);
                if (found != null)
                    hopses.Remove(found);
            }
            else if (found == null)
                hopses.Add(aHop);
            else
                throw new ArgumentException(String.Format("Hops with name = {0] is already present. Please use another name.", aHop.Name));

            Persist();

        }

        public void RemoveHops(Hops aHop)
        {
            hopses.Remove(aHop);
            Persist();
        }
    }
}

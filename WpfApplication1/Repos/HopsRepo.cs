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
    class HopsRepo : IHopsRepo
    {
        public List<Hops> hopses;
        public HopsRepo()
        {
            if (hopses == null)
            {
                XmlSerializer serializer = new XmlSerializer(typeof(HopsData));
                FileStream loadStream = new FileStream(@"hopses.xml", FileMode.Open, FileAccess.Read);
                HopsData loadedObject = (HopsData)serializer.Deserialize(loadStream);
                loadStream.Close();
                hopses = loadedObject.Hopses;
            }
        }
        public IEnumerable<Hops> Get()
        {
            return hopses;
        }
    }
}

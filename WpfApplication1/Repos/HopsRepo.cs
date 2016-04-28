using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GFCalc.DataModel;
using System.Xml.Serialization;
using System.IO;
using System.Reflection;
using System.Windows;

namespace GFCalc.Repos
{
    public class HopsRepository : IHopsRepo
    {
        public const string HOPS_DATA_FILENAME = "hopsesData.xml";

        public List<Hops> hopses;
        public HopsRepository()
        {
            if (hopses == null)
            {


                XmlSerializer serializer = new XmlSerializer(typeof(HopsData));
                HopsData loadedObject;
                var path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                var fullpath = String.Format("{0}\\{1}", path, HOPS_DATA_FILENAME);
                if (File.Exists(fullpath))
                {
                   FileStream loadStream = new FileStream(fullpath, FileMode.Open, FileAccess.Read);
                    loadedObject = (HopsData)serializer.Deserialize(loadStream);
                    loadStream.Close();
                }
                else
                {
                    var assembly = Assembly.GetExecutingAssembly();
                    var resourceName = "WpfApplication1.Resources.hopses.xml";
                    var stream =
                        assembly.GetManifestResourceStream(resourceName);
                    var reader = new System.IO.StreamReader(stream);
                    loadedObject = (HopsData)serializer.Deserialize(reader);

                }
                hopses = loadedObject.Hopses;
            }
        }


        public void Persist()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(HopsData));
            FileStream saveStream = null;
            try {
                var path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                var fullpath = String.Format("{0}\\{1}", path, HOPS_DATA_FILENAME);
                saveStream = new FileStream(fullpath, FileMode.Create, FileAccess.Write);
                var h = new HopsData();
                h.Hopses = hopses;
                serializer.Serialize(saveStream, h);

            }
            catch (Exception e)
            {
                MessageBox.Show(String.Format("Filed to open {0}, exception {1}", HOPS_DATA_FILENAME, e));
            }
            finally
            {
                if (saveStream != null)
                    saveStream.Close();
            }
        }

        public IEnumerable<Hops> Get()
        {
            return hopses;
        }


        public void AddHops(Hops aHop)
        {

            var hopInRepoFound = hopses.Any(x => x.Name == aHop.Name);
            if (hopInRepoFound)
            {
                // Try to remove it if we find it.
                var hopInRepo = hopses.FirstOrDefault(x => x.Name == aHop.Name);
                hopses.Remove(hopInRepo);
                // Always add whatever has been updated. This will cause duplicate data in some situations. 
                // This implies that name is not allowed to be changed during update. Improvement ... TODO
                hopses.Add(aHop);
            }
            else
            {
                hopses.Add(aHop);

            }
            Persist();

        }

        public void RemoveHops(Hops aHop)
        {
            hopses.Remove(aHop);
            Persist();
        }
    }
}

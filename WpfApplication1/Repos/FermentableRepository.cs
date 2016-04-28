using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using GFCalc.DataModel;
using System.Reflection;
using System.Windows;

namespace GFCalc.Repos
{
    public class FermentableRepository : IFermentableRepository
    {

        public const string MALT_DATA_FILEPATH_SAVE = @"maltsData.xml";

        public FermentableRepository()
        {
            FermentableAdjuncts loadedObject;
            XmlSerializer serializer = new XmlSerializer(typeof(FermentableAdjuncts));
            var path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var fullpath = String.Format("{0}\\{1}", path, MALT_DATA_FILEPATH_SAVE);

            if (!File.Exists(fullpath))
            {
                var assembly = Assembly.GetExecutingAssembly();
                var resourceName = "WpfApplication1.Resources.malts.xml";

                var stream = assembly.GetManifestResourceStream(resourceName);
                var reader = new System.IO.StreamReader(stream);
                loadedObject = (FermentableAdjuncts)serializer.Deserialize(reader);
            }
            else
            {
                FileStream loadStream = new FileStream(fullpath, FileMode.Open, FileAccess.Read);
                loadedObject = (FermentableAdjuncts)serializer.Deserialize(loadStream);

            }
            ferms = loadedObject.FermentableAdjunct;
        }

        public void Persist()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(FermentableAdjuncts));
            FileStream saveStream = null;
            try
            {
                var path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                var fullpath = String.Format("{0}\\{1}", path, MALT_DATA_FILEPATH_SAVE);

                saveStream = new FileStream(fullpath, FileMode.Create, FileAccess.Write);
                var f = new FermentableAdjuncts();
                f.FermentableAdjunct = ferms;
                serializer.Serialize(saveStream, f);
            }
            catch (Exception e)
            {
                MessageBox.Show(String.Format("Filed to open {0}, exception {1}", MALT_DATA_FILEPATH_SAVE, e));
            }
            finally
            {
                if (saveStream != null)
                    saveStream.Close();
            }
        }


        public List<FermentableAdjunct> ferms;

        public void AddFermentable(FermentableAdjunct aFermentable)
        {

            var found = ferms.Any(x => x.Name == aFermentable.Name);
            if (found)
            {
                var ferm = ferms.FirstOrDefault(x => x.Name == aFermentable.Name);
                ferms.Remove(ferm);

                ferms.Add(aFermentable);
            }
            else
                ferms.Add(aFermentable);

            Persist();

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

        public void RemoveFermentable(FermentableAdjunct aFermentable)
        {
            ferms.Remove(aFermentable);
            Persist();
        }
    }
}

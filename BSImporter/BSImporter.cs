using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;

namespace BSImport
{
    public class BSImporter
    {
        // XPaths
        private const string XPATH_FOR_RECIPIES = "//Data/Recipe";
        private const string RECIPE_NAME_ELEMENT = "F_R_NAME";

        private const string XPATH_FOR_MASHSTEPS = "//F_R_MASH/steps/Data/MashStep";
        private const string MASH_STEP_TIME_ELEMENT = "F_MS_STEP_TIME";
        private const string MASH_STEP_TEMP_ELEMENT = "F_MS_STEP_TEMP";

        private const string XPATH_FOR_GRAINS = "//Ingredients/Data/Grain";
        private const string XPATH_FOR_HOPS = "//Ingredients/Data/Hops";

        private const string XPATH_FOR_EQUIPMENTS = "//F_R_EQUIPMENT";
        private const string XPATH_FOR_BATCH_VOLUME_ELEMENT = "F_E_BATCH_VOL";
        private const string XPATH_FOR_BOIL_TIME_ELEMENT = "F_E_BOIL_TIME";
        private const string FINAL_BATCH_VOLUME_ELEMENT = "F_R_FINAL_VOL_MEASURED";

        private const string ORIGINAL_GRAVITY_ELEMENT = "F_R_OG_MEASURED";
        private XDocument XDoc;

        private string XmlFilename;

        public BSImporter(string aXmlFilePath)
        {

            XmlFilename = aXmlFilePath;

            // The Beersmith recipes includes some non-Unicode sequences. 
            //Fortunately these are of no interest to us, so just remove them
            Regex r = new Regex(@"&\w+;");
            string replacement = "";
            StringBuilder strBuild = new StringBuilder();
            string s = r.Replace(File.ReadAllText(XmlFilename), replacement);


            XDoc = XDocument.Parse(s);

            XmlFilename = aXmlFilePath;
        }
        public double getOriginalGravity(string aRecepieName)
        {
            double r = 0;
            string errMsg;

            var xrs = XDoc.XPathSelectElements(XPATH_FOR_RECIPIES);
            var vs = xrs.FirstOrDefault(x => x.Element(RECIPE_NAME_ELEMENT).Value.Equals(aRecepieName));
            var og = vs.Element(ORIGINAL_GRAVITY_ELEMENT).Value;
            if (!double.TryParse(og, NumberStyles.AllowDecimalPoint, CultureInfo.CreateSpecificCulture("en-US"), out r))
            {
                errMsg = "Unable to parse original gravity";
                throw new ArgumentException(errMsg);
            }

            return Math.Round(r,4);
        }
        

        public double getFinalBatchVolume(string aRecepieName)
        {
            double r = 0;
            string errMsg;

            var xrs = XDoc.XPathSelectElements(XPATH_FOR_RECIPIES);
            var vs = xrs.FirstOrDefault(x => x.Element(RECIPE_NAME_ELEMENT).Value.Equals(aRecepieName));
            var eqs = vs.XPathSelectElements(XPATH_FOR_EQUIPMENTS);
            var e = eqs.FirstOrDefault();
            var bv = e.Element(XPATH_FOR_BATCH_VOLUME_ELEMENT).Value;
            if (!double.TryParse(bv, NumberStyles.AllowDecimalPoint, CultureInfo.CreateSpecificCulture("en-US"), out r))
            {
                errMsg = "Unable to parse final batch volume";
                throw new ArgumentException(errMsg);
            }

            return Math.Round(ConvertUsCupsToLiters(r),1);
        }
        static public double ConvertUsCupsToLiters(double aVolumeInCups)
        {
            return 0.0295735296d * aVolumeInCups;
        }

        static public double ConvertUsOuncesToGrams( double aWeigthInOunces)
        {
            return (28.3495231d * aWeigthInOunces);
        } 


        public int getBoilTime(string aRecepieName)
        {
            double r = 0;
            string errMsg;
            var xrs = XDoc.XPathSelectElements(XPATH_FOR_RECIPIES);
            var vs = xrs.FirstOrDefault(x => x.Element(RECIPE_NAME_ELEMENT).Value.Equals(aRecepieName));
            var eqs = vs.XPathSelectElements(XPATH_FOR_EQUIPMENTS);
            var e = eqs.FirstOrDefault();
            var bt = e.Element(XPATH_FOR_BOIL_TIME_ELEMENT).Value;
            if (!double.TryParse(bt, NumberStyles.AllowDecimalPoint, CultureInfo.CreateSpecificCulture("en-US"), out r))
            {
                errMsg = "Unable to parse boil time";
                throw new ArgumentException(errMsg);
            }

            return (int)(Math.Round(r));

        }


        public IEnumerable<BSMashStep> getMashProfile(string aRecepieName)
        {
            var ret = new List<BSMashStep>();
            string errMsg = null;

            var xrs = XDoc.XPathSelectElements(XPATH_FOR_RECIPIES);
            var r = xrs.FirstOrDefault(x => x.Element(RECIPE_NAME_ELEMENT).Value.Equals(aRecepieName));
            var xmss = r.XPathSelectElements(XPATH_FOR_MASHSTEPS);

            foreach (XElement xms in xmss)
            {
                float f;
                var m = new BSMashStep();
                if (float.TryParse(xms.Element(MASH_STEP_TEMP_ELEMENT).Value,
                    NumberStyles.AllowDecimalPoint,
                    CultureInfo.CreateSpecificCulture("en-US"), out f))
                    m.Temperature = (int)Math.Round(f);
                else
                    errMsg = "Unable to parse one mash temperature in the mash profile";


                if (float.TryParse(xms.Element(MASH_STEP_TIME_ELEMENT).Value, NumberStyles.AllowDecimalPoint, CultureInfo.CreateSpecificCulture("en-US"), out f))
                    m.TimeMinutes = (int)Math.Round(f);
                else
                    errMsg = "Unable to parse one mash time in the mash profile";

                if (errMsg != null)
                    throw new ArgumentException(errMsg);

                ret.Add(m);
            }
            return ret;
        }

        public IEnumerable<string> GetAllRecipes()
        {
            var ret = new List<string>();
            var xrs = XDoc.XPathSelectElements(XPATH_FOR_RECIPIES);
            foreach (XElement xe in xrs)
            {
                ret.Add(xe.Element(RECIPE_NAME_ELEMENT).Value);
            }

            return ret;
        }

        public IEnumerable<BSGrainBill> GetGrainBill(string aRecepieName)
        {
            var ret = new List<BSGrainBill>();
            string errMsg = null;

            var xrs = XDoc.XPathSelectElements(XPATH_FOR_RECIPIES);
            var r = xrs.FirstOrDefault(x => x.Element(RECIPE_NAME_ELEMENT).Value.Equals(aRecepieName));

            var gs = r.XPathSelectElements(XPATH_FOR_GRAINS);
            foreach (XElement g in gs)
            {
                float f;
                var gb = new BSGrainBill();
                if (float.TryParse(g.Element("F_G_PERCENT").Value, NumberStyles.AllowDecimalPoint, CultureInfo.CreateSpecificCulture("en-US"), out f))
                    gb.AmountPercent = (int)(Math.Round(f));
                else
                    errMsg = "Unable to parse one grain percentage in the grainbill";

                if (errMsg != null)
                    throw new ArgumentException(errMsg);

                gb.FermentableName = g.Element("F_G_NAME").Value;
                ret.Add(gb);
            }

            return ret;
        }

        public IEnumerable<BSHops> GetBoilHops(string aRecepieName)
        {
            var ret = new List<BSHops>();
            string errMsg = null;

            var xrs = XDoc.XPathSelectElements(XPATH_FOR_RECIPIES);
            var r = xrs.FirstOrDefault(x => x.Element(RECIPE_NAME_ELEMENT).Value.Equals(aRecepieName));

            var hs = r.XPathSelectElements(XPATH_FOR_HOPS);
            foreach (XElement h in hs)
            {
                double f;
                var gb = new BSHops();

                if (double.TryParse(h.Element("F_H_AMOUNT").Value, NumberStyles.AllowDecimalPoint, CultureInfo.CreateSpecificCulture("en-US"), out f))
                {
                    gb.Amount = Math.Round(ConvertUsOuncesToGrams(f), 1);
                }
                else
                    errMsg = "Unable to parse hops amount";


                if (double.TryParse(h.Element("F_H_ALPHA").Value, NumberStyles.AllowDecimalPoint, CultureInfo.CreateSpecificCulture("en-US"), out f))
                    gb.AlphaAcid = f;
                else
                    errMsg = "Unable to parse hops alpha acid";

                if (double.TryParse(h.Element("F_H_BOIL_TIME").Value, NumberStyles.AllowDecimalPoint, CultureInfo.CreateSpecificCulture("en-US"), out f))
                    gb.BoilTime = (int)(Math.Round(f));
                else
                    errMsg = "Unable to parse hop boil time";

                if (double.TryParse(h.Element("F_H_DRY_HOP_TIME").Value, NumberStyles.AllowDecimalPoint, CultureInfo.CreateSpecificCulture("en-US"), out f))
                    gb.DryHopDay = f;
                else
                    errMsg = "Unable to parse hops dry hops days";

                if (errMsg != null)
                    throw new ArgumentException(errMsg);

                gb.Name = h.Element("F_H_NAME").Value;
                ret.Add(gb);
            }

            return ret;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Base;
using BH.oM.LifeCycleAnalysis;
using BH.Engine.Reflection;

namespace BH.Engine.LifeCycleAnalysis
{
    public static partial class Convert
    {
        /***************************************************/
        /****           Public Methods                  ****/
        /***************************************************/

        public static HealthProductDeclarationData ToHealthProductDeclarationData(this CustomObject obj)
        {
            HealthProductDeclarationData epd = new HealthProductDeclarationData
            {
                //Cpid = obj.PropertyValue("cpid")?.ToString() ?? "",
                //Version = obj.PropertyValue("version")?.ToString() ?? "",
                MasterFormat = obj.PropertyValue("Masterformat")?.ToString() ?? "",
                Uniformats = obj.PropertyValue("Uniformats")?.ToString() ?? "",
                CancerOrange = obj.PropertyValue("CancerOrange") != null ? System.Convert.ToDouble(obj.PropertyValue("CancerOrange")) : double.NaN,
                DevelopmentalOrange = obj.PropertyValue("DevelopmentalOrange") != null ? System.Convert.ToDouble(obj.PropertyValue("DevelopmentalOrange")) : double.NaN,
                EndocrineOrange = obj.PropertyValue("EndocrineOrange") != null ? System.Convert.ToDouble(obj.PropertyValue("EndocrineOrange")) : double.NaN,
                EyeIrritationOrange = obj.PropertyValue("EyeIrritationOrange") != null ? System.Convert.ToDouble(obj.PropertyValue("EyeIrritationOrange")) : double.NaN,
                MammalianOrange = obj.PropertyValue("MammalianOrange") != null ? System.Convert.ToDouble(obj.PropertyValue("MammalianOrange")) : double.NaN,
                MutagenicityOrange = obj.PropertyValue("MutagenicityOrange") != null ? System.Convert.ToDouble(obj.PropertyValue("MutagenicityOrange")) : double.NaN,
                NeurotoxicityOrange = obj.PropertyValue("NeurotoxicityOrange") != null ? System.Convert.ToDouble(obj.PropertyValue("NeurotoxicityOrange")) : double.NaN,
                OrganToxicantOrange = obj.PropertyValue("OrganToxicantOrange") != null ? System.Convert.ToDouble(obj.PropertyValue("OrganToxicantOrange")) : double.NaN,
                ReproductiveOrange = obj.PropertyValue("ReproductiveOrange") != null ? System.Convert.ToDouble(obj.PropertyValue("ReproductiveOrange")) : double.NaN,
                RespiratoryOrange = obj.PropertyValue("RespiratoryOrange") != null ? System.Convert.ToDouble(obj.PropertyValue("RespiratoryOrange")) : double.NaN,
                RespiratoryOccupationalOnlyOrange = obj.PropertyValue("RespiratoryOccupationalOnlyOrange") != null ? System.Convert.ToDouble(obj.PropertyValue("RespiratoryOccupationalOnlyOrange")) : double.NaN,
                SkinSensitizationOrange = obj.PropertyValue("SkinSensitizationOrange") != null ? System.Convert.ToDouble(obj.PropertyValue("SkinSensitizationOrange")) : double.NaN,
                CancerRed = obj.PropertyValue("CancerRed") != null ? System.Convert.ToDouble(obj.PropertyValue("CancerRed")) : double.NaN,
                CancerOccupationalOnlyRed = obj.PropertyValue("CancerOccupationalOnlyRed") != null ? System.Convert.ToDouble(obj.PropertyValue("CancerOccupationalOnlyRed")) : double.NaN,
                DevelopmentalRed = obj.PropertyValue("DevelopmentalRed") != null ? System.Convert.ToDouble(obj.PropertyValue("DevelopmentalRed")) : double.NaN,
                MutagenicityRed = obj.PropertyValue("MutagenicityRed") != null ? System.Convert.ToDouble(obj.PropertyValue("MutagenicityRed")) : double.NaN,
                PersistantBioaccumulativeToxicantRed = obj.PropertyValue("PersistantBioaccumulativeToxicantRed") != null ? System.Convert.ToDouble(obj.PropertyValue("PersistantBioaccumulativeToxicantRed")) : double.NaN,
                RespiratoryRed = obj.PropertyValue("RespiratoryRed") != null ? System.Convert.ToDouble(obj.PropertyValue("RespiratoryRed")) : double.NaN,
                PersistantBioaccumulativeToxicantPurple = obj.PropertyValue("PersistantBioaccumulativeToxicantPurple") != null ? System.Convert.ToDouble(obj.PropertyValue("PersistantBioaccumulativeToxicantPurple")) : double.NaN,
            };
            return epd;
        }
    }
}

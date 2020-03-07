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
                Cpid = obj.PropertyValue("cpid")?.ToString() ?? "",
                Version = obj.PropertyValue("version")?.ToString() ?? "",
                MasterFormat = obj.PropertyValue("masterformat")?.ToString() ?? "",
                Uniformats = obj.PropertyValue("uniformats")?.ToString() ?? "",
                CancerOrange = obj.PropertyValue("cancerOrange") != null ? System.Convert.ToDouble(obj.PropertyValue("cancerOrange")) : double.NaN,
                DevelopmentalOrange = obj.PropertyValue("developmentalOrange") != null ? System.Convert.ToDouble(obj.PropertyValue("developmentalOrange")) : double.NaN,
                EndocrineOrange = obj.PropertyValue("endocrineOrange") != null ? System.Convert.ToDouble(obj.PropertyValue("endocrineOrange")) : double.NaN,
                EyeIrritationOrange = obj.PropertyValue("eyeIrritationOrange") != null ? System.Convert.ToDouble(obj.PropertyValue("eyeIrritationOrange")) : double.NaN,
                MammalianOrange = obj.PropertyValue("mammalianOrange") != null ? System.Convert.ToDouble(obj.PropertyValue("mammalianOrange")) : double.NaN,
                MutagenicityOrange = obj.PropertyValue("mutagenicityOrange") != null ? System.Convert.ToDouble(obj.PropertyValue("mutagenicityOrange")) : double.NaN,
                NeurotoxicityOrange = obj.PropertyValue("neurotoxicityOrange") != null ? System.Convert.ToDouble(obj.PropertyValue("neurotoxicityOrange")) : double.NaN,
                OrganToxicantOrange = obj.PropertyValue("organToxicantOrange") != null ? System.Convert.ToDouble(obj.PropertyValue("organToxicantOrange")) : double.NaN,
                ReproductiveOrange = obj.PropertyValue("reproductiveOrange") != null ? System.Convert.ToDouble(obj.PropertyValue("reproductiveOrange")) : double.NaN,
                RespiratoryOrange = obj.PropertyValue("respiratoryOrange") != null ? System.Convert.ToDouble(obj.PropertyValue("respiratoryOrange")) : double.NaN,
                RespiratoryOccupationalOnlyOrange = obj.PropertyValue("respiratoryOccupationalOnlyOrange") != null ? System.Convert.ToDouble(obj.PropertyValue("respiratoryOccupationalOnlyOrange")) : double.NaN,
                SkinSensitizationOrange = obj.PropertyValue("skinSensitizationOrange") != null ? System.Convert.ToDouble(obj.PropertyValue("skinSensitizationOrange")) : double.NaN,
                CancerRed = obj.PropertyValue("cancerRed") != null ? System.Convert.ToDouble(obj.PropertyValue("cancerRed")) : double.NaN,
                CancerOccupationalOnlyRed = obj.PropertyValue("cancerOccupationalOnlyRed") != null ? System.Convert.ToDouble(obj.PropertyValue("cancerOccupationalOnlyRed")) : double.NaN,
                DevelopmentalRed = obj.PropertyValue("developmentalRed") != null ? System.Convert.ToDouble(obj.PropertyValue("developmentalRed")) : double.NaN,
                MutagenicityRed = obj.PropertyValue("mutagenicityRed") != null ? System.Convert.ToDouble(obj.PropertyValue("mutagenicityRed")) : double.NaN,
                PbtRed = obj.PropertyValue("pbtRed") != null ? System.Convert.ToDouble(obj.PropertyValue("pbtRed")) : double.NaN,
                RespiratoryRed = obj.PropertyValue("respiratoryRed") != null ? System.Convert.ToDouble(obj.PropertyValue("respiratoryRed")) : double.NaN,
                PbtPurple = obj.PropertyValue("pbtPurple") != null ? System.Convert.ToDouble(obj.PropertyValue("pbtPurple")) : double.NaN,
            };
            return epd;
        }
    }
}

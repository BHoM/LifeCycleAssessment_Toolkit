using System.Collections.Generic;
using BH.oM.Base;

namespace BH.oM.LifeCycleAssessment
{
    public class HealthProductDeclarationData : BHoMObject
    {
        /***************************************************/
        /**** Properties                                ****/
        /***************************************************/
        public string Cpid { get; set; } = "";
        public string Version { get; set; } = "";
        public string MasterFormat { get; set; } = "";
        public string Uniformats { get; set; } = "";
        public double CancerOrange { get; set; } = double.NaN;
        public double DevelopmentalOrange { get; set; } = double.NaN;
        public double EndocrineOrange { get; set; } = double.NaN;
        public double EyeIrritationOrange { get; set; } = double.NaN;
        public double MammalianOrange { get; set; } = double.NaN;
        public double MutagenicityOrange { get; set; } = double.NaN;
        public double NeurotoxicityOrange { get; set; } = double.NaN;
        public double OrganToxicantOrange { get; set; } = double.NaN;
        public double ReproductiveOrange { get; set; } = double.NaN;
        public double RespiratoryOrange { get; set; } = double.NaN;
        public double RespiratoryOccupationalOnlyOrange { get; set; } = double.NaN;
        public double SkinSensitizationOrange { get; set; } = double.NaN;
        public double CancerRed { get; set; } = double.NaN;
        public double CancerOccupationalOnlyRed { get; set; } = double.NaN;
        public double DevelopmentalRed { get; set; } = double.NaN;
        public double MutagenicityRed { get; set; } = double.NaN;
        public double PbtRed { get; set; } = double.NaN;
        public double ReproductiveRed { get; set; } = double.NaN;
        public double RespiratoryRed { get; set; } = double.NaN;
        public double PbtPurple { get; set; } = double.NaN;
        /***************************************************/
    }
}

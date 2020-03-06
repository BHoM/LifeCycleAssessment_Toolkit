using System.Collections.Generic;
using BH.oM.Base;

namespace BH.oM.LifeCycleAnalysis
{
    public class HPDData : BHoMObject
    {
        /***************************************************/
        /**** Properties                                ****/
        /***************************************************/
        string Cpid { get; set; } = "";
        string Version { get; set; } = "";
        string MasterFormat { get; set; } = "";
        string Uniformats { get; set; } = "";
        double CancerOrange { get; set; } = double.NaN;
        double DevelopmentalOrange { get; set; } = double.NaN;
        double EndocrineOrange { get; set; } = double.NaN;
        double EyeIrritationOrange { get; set; } = double.NaN;
        double MammalianOrange { get; set; } = double.NaN;
        double MutagenicityOrange { get; set; } = double.NaN;
        double NeurotoxicityOrange { get; set; } = double.NaN;
        double OrganToxicantOrange { get; set; } = double.NaN;
        double ReproductiveOrange { get; set; } = double.NaN;
        double RespiratoryOrange { get; set; } = double.NaN;
        double RespiratoryOccupationalOnlyOrange { get; set; } = double.NaN;
        double SkinSensitizationOrange { get; set; } = double.NaN;
        double CancerRed { get; set; } = double.NaN;
        double CancerOccupationalOnlyRed { get; set; } = double.NaN;
        double DevelopmentalRed { get; set; } = double.NaN;
        double MutagenicityRed { get; set; } = double.NaN;
        double PbtRed { get; set; } = double.NaN;
        double ReproductiveRed { get; set; } = double.NaN;
        double RespiratoryRed { get; set; } = double.NaN;
        double PbtPurple { get; set; } = double.NaN;
        /***************************************************/
    }
}

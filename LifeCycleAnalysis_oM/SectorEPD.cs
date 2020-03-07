using System.Collections.Generic;
using BH.oM.Base;

namespace BH.oM.LifeCycleAnalysis
{
    public class SectorEPD : BHoMObject, IEPDData
    {
        /***************************************************/
        /**** Properties                                ****/
        /***************************************************/
        public List<string> Publisher { get; set; } = new List<string>();
        public List<string> Jurisdiction { get; set; } = new List<string>();
        public string Id { get; set; } = "";
        public string Density { get; set; } = "";
        public string GwpPerKG { get; set; } = "";
        public string GwpPerDeclaredUnit { get; set; } = "";
        public double BiogenicEmbodiedCarbon { get; set; } = double.NaN;
        public string DeclaredUnit { get; set; } = "";
        public string Description { get; set; } = "";
        public string EPDScope { get; set; } = "";
        public double GlobalWarmingPotential { get; set; } = double.NaN;
        public double OzoneDepletionPotential { get; set; } = double.NaN;
        public double PhotochemicalOzoneCreationPotential { get; set; } = double.NaN;
        public double AcidificationPotential { get; set; } = double.NaN;
        public double EutrophicationPotential { get; set; } = double.NaN;
        public double DepletionOfAbioticResourcesFossilFuels { get; set; } = double.NaN;
        public double GlobalWarmingPotentialEOL { get; set; } = double.NaN;
        public double OzoneDepletionPotentialEOL { get; set; } = double.NaN;
        public double PhotochemicalOzoneCreationPotentialEOL { get; set; } = double.NaN;
        public double AcidificationPotentialEOL { get; set; } = double.NaN;
        public double EutrophicationPotentialEOL { get; set; } = double.NaN;
        public double DepletionOfAbioticResourcesFossilFuelsEOL { get; set; } = double.NaN;
        public string TreatmentEOL { get; set; } = "";

        /***************************************************/
    }
}

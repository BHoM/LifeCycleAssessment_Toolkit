using System.Collections.Generic;
using BH.oM.Base;

namespace BH.oM.LifeCycleAssessment
{
    public class SectorEnvironmentalProductDeclaration : BHoMObject, IEnvironmentalProductDeclarationData
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
        public string Scope { get; set; } = "";
        public double GlobalWarmingPotential { get; set; } = double.NaN;
        public double OzoneDepletionPotential { get; set; } = double.NaN;
        public double PhotochemicalOzoneCreationPotential { get; set; } = double.NaN;
        public double AcidificationPotential { get; set; } = double.NaN;
        public double EutrophicationPotential { get; set; } = double.NaN;
        public double DepletionOfAbioticResourcesFossilFuels { get; set; } = double.NaN;
        public double GlobalWarmingPotentialEndOfLife { get; set; } = double.NaN;
        public double OzoneDepletionPotentialEndOfLife { get; set; } = double.NaN;
        public double PhotochemicalOzoneCreationPotentialEndOfLife { get; set; } = double.NaN;
        public double AcidificationPotentialEndOfLife { get; set; } = double.NaN;
        public double EutrophicationPotentialEndOfLife { get; set; } = double.NaN;
        public double DepletionOfAbioticResourcesFossilFuelsEndOfLife { get; set; } = double.NaN;
        public string EndOfLifeTreatment { get; set; } = "";

        /***************************************************/
    }
}

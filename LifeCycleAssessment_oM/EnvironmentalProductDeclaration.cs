using System.Collections.Generic;
using BH.oM.Base;

namespace BH.oM.LifeCycleAssessment
{
    public class EnvironmentalProductDeclaration : BHoMObject, IEnvironmentalProductDeclarationData
    {
        /***************************************************/
        /**** Properties                                ****/
        /***************************************************/
        public string Manufacturer { get; set; } = "";
        public string Plant { get; set; } = "";
        public int PostalCode { get; set; } = 0;
        public List<string> IndustryStandards { get; set; } = new List<string>();
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
        public double PhotochemicalOzoneCreationPotential { get; set; } = double.NaN; //needs convert method "smogPotential"
        public double AcidificationPotential { get; set; } = double.NaN;
        public double EutrophicationPotential { get; set; } = double.NaN;
        public double DepletionOfAbioticResourcesFossilFuels { get; set; } = double.NaN; //needs convert method "primaryEnergyDemand"
        public double GlobalWarmingPotentialEndOfLife { get; set; } = double.NaN;
        public double OzoneDepletionPotentialEndOfLife { get; set; } = double.NaN;
        public double PhotochemicalOzoneCreationPotentialEndOfLife { get; set; } = double.NaN; //needs convert method "smogPotentialEol"
        public double AcidificationPotentialEndOfLife { get; set; } = double.NaN;
        public double EutrophicationPotentialEndOfLife { get; set; } = double.NaN;
        public double DepletionOfAbioticResourcesFossilFuelsEndOfLife { get; set; } = double.NaN; //needs convert method "primaryEnergyDemandEol"
        public string EndOfLifeTreatment { get; set; } = "";
        public string Masterformat { get; set; } = "";
        public string PostConsumerRecycledContent { get; set; } = "";
        public int ReferenceYear { get; set; } = 0;

        /***************************************************/
    }
}

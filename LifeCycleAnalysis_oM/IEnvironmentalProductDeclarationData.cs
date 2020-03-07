using System.Collections.Generic;
using BH.oM.Base;

namespace BH.oM.LifeCycleAnalysis
{
    public interface IEnvironmentalProductDeclarationData : IBHoMObject
    {
        /***************************************************/
        /**** Properties                                ****/
        /***************************************************/
        string Id { get; set; }
        string Density { get; set; }
        double BiogenicEmbodiedCarbon { get; set; }
        string DeclaredUnit { get; set; } // <---- make sure this is always populated 
        string Description { get; set; }
        string Scope { get; set; }
        double GlobalWarmingPotential { get; set; }
        double OzoneDepletionPotential { get; set; }
        double PhotochemicalOzoneCreationPotential { get; set; }
        double AcidificationPotential { get; set; }
        double EutrophicationPotential { get; set; }
        double DepletionOfAbioticResourcesFossilFuels { get; set; }
        double GlobalWarmingPotentialEndOfLife { get; set; }
        double OzoneDepletionPotentialEndOfLife { get; set; }
        double PhotochemicalOzoneCreationPotentialEndOfLife { get; set; }
        double AcidificationPotentialEndOfLife { get; set; }
        double EutrophicationPotentialEndOfLife { get; set; }
        double DepletionOfAbioticResourcesFossilFuelsEndOfLife { get; set; }
        string EndOfLifeTreatment { get; set; } 
        /***************************************************/
    }
}

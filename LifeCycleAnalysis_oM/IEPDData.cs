using System.Collections.Generic;
using BH.oM.Base;

namespace BH.oM.LifeCycleAnalysis
{
    public interface IEPDData : IBHoMObject
    {
        /***************************************************/
        /**** Properties                                ****/
        /***************************************************/

        string Id { get; set; }
        string Density { get; set; }
        string GwpPerKG { get; set; }
        string GwpPerDeclaredUnit { get; set; }
        double BiogenicEmbodiedCarbon { get; set; }
        string DeclaredUnit { get; set; }
        string Description { get; set; }
        string EPDScope { get; set; }
        double GlobalWarmingPotential { get; set; }
        double OzoneDepletionPotential { get; set; }
        double PhotochemicalOzoneCreationPotential { get; set; }
        double AcidificationPotential { get; set; }
        double EutrophicationPotential { get; set; }
        double DepletionOfAbioticResourcesFossilFuels { get; set; }
        double GlobalWarmingPotentialEOL { get; set; }
        double OzoneDepletionPotentialEOL { get; set; }
        double PhotochemicalOzoneCreationPotentialEOL { get; set; }
        double AcidificationPotentialEOL { get; set; }
        double EutrophicationPotentialEOL { get; set; }
        double DepletionOfAbioticResourcesFossilFuelsEOL { get; set; }
        string TreatmentEOL { get; set; }

        /***************************************************/
    }
}

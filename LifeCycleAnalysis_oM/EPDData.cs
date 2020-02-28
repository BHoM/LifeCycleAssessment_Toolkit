using System.Collections.Generic;
using BH.oM.Base;

namespace BH.oM.LifeCycleAnalysis
{
    public class EPDData : BHoMObject
    {
        /***************************************************/
        /**** Properties                                ****/
        /***************************************************/

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
        public double GlobalWarmingPotentialEOL { get; set; }
        public double OzoneDepletionPotentialEOL { get; set; }
        public double PhotochemicalOzoneCreationPotentialEOL { get; set; }
        public double AcidificationPotentialEOL { get; set; }
        public double EutrophicationPotentialEOL { get; set; }
        public double DepletionOfAbioticResourcesFossilFuelsEOL { get; set; }
        public string TreatmentEOL { get; set; }

        /***************************************************/
    }
}

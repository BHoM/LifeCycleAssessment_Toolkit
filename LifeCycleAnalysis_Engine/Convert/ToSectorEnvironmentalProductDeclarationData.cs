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

        public static SectorEnvironmentalProductDeclaration ToSectorEnvironmentalProductDeclarationData(this CustomObject obj)
        {
            SectorEnvironmentalProductDeclaration epd = new SectorEnvironmentalProductDeclaration
            {
                Id = obj.PropertyValue("_id")?.ToString() ?? "",
                Density = obj.PropertyValue("Density")?.ToString() ?? "",
                BiogenicEmbodiedCarbon = obj.PropertyValue("biogenicEmbodiedCarbon") != null ? System.Convert.ToDouble(obj.PropertyValue("biogenicEmbodiedCarbon")) : double.NaN,
                DeclaredUnit = obj.PropertyValue("DeclaredUnit")?.ToString() ?? "",
                EutrophicationPotentialEOL = obj.PropertyValue("EutrophicationPotentialEOL") != null ? System.Convert.ToDouble(obj.PropertyValue("EutrophicationPotentialEOL")) : double.NaN,
                AcidificationPotentialEOL = obj.PropertyValue("AcidificationPotentialEOL") != null ? System.Convert.ToDouble(obj.PropertyValue("AcidificationPotentialEOL")) : double.NaN,
                PhotochemicalOzoneCreationPotentialEOL = obj.PropertyValue("PhotochemicalOzoneCreationPotentialEOL") != null ? System.Convert.ToDouble(obj.PropertyValue("PhotochemicalOzoneCreationPotentialEOL")) : double.NaN,
                OzoneDepletionPotentialEOL = obj.PropertyValue("OzoneDepletionPotentialEOL") != null ? System.Convert.ToDouble(obj.PropertyValue("OzoneDepletionPotentialEOL")) : double.NaN,
                GlobalWarmingPotentialEOL = obj.PropertyValue("GlobalWarmingPotentialEOL") != null ? System.Convert.ToDouble(obj.PropertyValue("GlobalWarmingPotentialEOL")) : double.NaN,
                DepletionOfAbioticResourcesFossilFuelsEOL = obj.PropertyValue("DepletionOfAbioticResourcesFossilFuelsEOL") != null ? System.Convert.ToDouble(obj.PropertyValue("DepletionOfAbioticResourcesFossilFuelsEOL")) : double.NaN,
                DepletionOfAbioticResourcesFossilFuels = obj.PropertyValue("DepletionOfAbioticResourcesFossilFuels") != null ? System.Convert.ToDouble(obj.PropertyValue("DepletionOfAbioticResourcesFossilFuels")) : double.NaN,
                EutrophicationPotential = obj.PropertyValue("EutrophicationPotential") != null ? System.Convert.ToDouble(obj.PropertyValue("EutrophicationPotential")) : double.NaN,
                AcidificationPotential = obj.PropertyValue("AcidificationPotential") != null ? System.Convert.ToDouble(obj.PropertyValue("AcidificationPotential")) : double.NaN,
                PhotochemicalOzoneCreationPotential = obj.PropertyValue("PhotochemicalOzoneCreationPotential") != null ? System.Convert.ToDouble(obj.PropertyValue("PhotochemicalOzoneCreationPotential")) : double.NaN,
                OzoneDepletionPotential = obj.PropertyValue("OzoneDepletionPotential") != null ? System.Convert.ToDouble(obj.PropertyValue("OzoneDepletionPotential")) : double.NaN,
                GlobalWarmingPotential = obj.PropertyValue("GlobalWarmingPotential") != null ? System.Convert.ToDouble(obj.PropertyValue("GlobalWarmingPotential")) : double.NaN,
                EPDScope = obj.PropertyValue("EPDScope")?.ToString() ?? "",
                Description = obj.PropertyValue("Description")?.ToString() ?? "",
                GwpPerDeclaredUnit = obj.PropertyValue("GwpPerDeclaredUnit")?.ToString() ?? "",
                GwpPerKG = obj.PropertyValue("GwpPerKG")?.ToString() ?? "", //needs string splitting to extract units and convert to double.
                TreatmentEOL = obj.PropertyValue("TreatmentEOL")?.ToString() ?? "",
            };
            return epd;
        }
    }
}

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

        public static EnvironmentalProductDeclaration ToEnvironmentalProductDeclarationData(this CustomObject obj)
        {
            EnvironmentalProductDeclaration epd = new EnvironmentalProductDeclaration
            {
                Id = obj.PropertyValue("_id")?.ToString() ?? "",
                Name = obj.PropertyValue("name")?.ToString() ?? "",
                EutrophicationPotentialEndOfLife = obj.PropertyValue("EutrophicationPotentialEol") != null ? System.Convert.ToDouble(obj.PropertyValue("EutrophicationPotentialEol")) : double.NaN,
                AcidificationPotentialEndOfLife = obj.PropertyValue("AcidificationPotentialEol") != null ? System.Convert.ToDouble(obj.PropertyValue("AcidificationPotentialEol")) : double.NaN,
                PhotochemicalOzoneCreationPotentialEndOfLife = obj.PropertyValue("SmogPotentialEol") != null ? System.Convert.ToDouble(obj.PropertyValue("SmogPotentialEol")) : double.NaN,
                OzoneDepletionPotentialEndOfLife = obj.PropertyValue("OzoneDepletionPotentialEol") != null ? System.Convert.ToDouble(obj.PropertyValue("OzoneDepletionPotentialEol")) : double.NaN,
                GlobalWarmingPotentialEndOfLife = obj.PropertyValue("GlobalWarmingPotentialEol") != null ? System.Convert.ToDouble(obj.PropertyValue("GlobalWarmingPotentialEol")) : double.NaN,
                DepletionOfAbioticResourcesFossilFuelsEndOfLife = obj.PropertyValue("PrimaryEnergyDemandEol") != null ? System.Convert.ToDouble(obj.PropertyValue("PrimaryEnergyDemandEol")) : double.NaN,
                DepletionOfAbioticResourcesFossilFuels = obj.PropertyValue("PrimaryEnergyDemand") != null ? System.Convert.ToDouble(obj.PropertyValue("PrimaryEnergyDemand")) : double.NaN,
                EutrophicationPotential = obj.PropertyValue("EutrophicationPotential") != null ? System.Convert.ToDouble(obj.PropertyValue("EutrophicationPotential")) : double.NaN,
                AcidificationPotential = obj.PropertyValue("AcidificationPotential") != null ? System.Convert.ToDouble(obj.PropertyValue("AcidificationPotential")) : double.NaN,
                PhotochemicalOzoneCreationPotential = obj.PropertyValue("SmogPotential") != null ? System.Convert.ToDouble(obj.PropertyValue("SmogPotential")) : double.NaN,
                OzoneDepletionPotential = obj.PropertyValue("OzoneDepletionPotential") != null ? System.Convert.ToDouble(obj.PropertyValue("OzoneDepletionPotential")) : double.NaN,
                GlobalWarmingPotential = obj.PropertyValue("GlobalWarmingPotential") != null ? System.Convert.ToDouble(obj.PropertyValue("GlobalWarmingPotential")) : double.NaN,
                Scope = obj.PropertyValue("Scope")?.ToString() ?? "",
                Description = obj.PropertyValue("Description")?.ToString() ?? "",
                DeclaredUnit = obj.PropertyValue("DeclaredUnit")?.ToString() ?? "",
                BiogenicEmbodiedCarbon = obj.PropertyValue("BiogenicEmbodiedCarbon") != null ? System.Convert.ToDouble(obj.PropertyValue("BiogenicEmbodiedCarbon")) : double.NaN,
                GwpPerDeclaredUnit = obj.PropertyValue("GwpPerDeclaredUnit")?.ToString() ?? "",
                GwpPerKG = obj.PropertyValue("GwpPerKG")?.ToString() ?? "", //needs string splitting to extract units and convert to double.
                Density = obj.PropertyValue("Density")?.ToString() ?? "", //additional converts needed because this type is string not double.
                EndOfLifeTreatment = obj.PropertyValue("EolTreatment")?.ToString() ?? "",
            };
            return epd;
        }
    }
}

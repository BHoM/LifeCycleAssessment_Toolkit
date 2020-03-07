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
                EutrophicationPotentialEndOfLife = obj.PropertyValue("eutrophicationPotentialEol") != null ? System.Convert.ToDouble(obj.PropertyValue("eutrophicationPotentialEol")) : double.NaN,
                AcidificationPotentialEndOfLife = obj.PropertyValue("acidificationPotentialEol") != null ? System.Convert.ToDouble(obj.PropertyValue("acidificationPotentialEol")) : double.NaN,
                PhotochemicalOzoneCreationPotentialEndOfLife = obj.PropertyValue("smogPotentialEol") != null ? System.Convert.ToDouble(obj.PropertyValue("smogPotentialEol")) : double.NaN,
                OzoneDepletionPotentialEndOfLife = obj.PropertyValue("ozoneDepletionPotentialEol") != null ? System.Convert.ToDouble(obj.PropertyValue("ozoneDepletionPotentialEol")) : double.NaN,
                GlobalWarmingPotentialEndOfLife = obj.PropertyValue("globalWarmingPotentialEol") != null ? System.Convert.ToDouble(obj.PropertyValue("globalWarmingPotentialEol")) : double.NaN,
                DepletionOfAbioticResourcesFossilFuelsEndOfLife = obj.PropertyValue("primaryEnergyDemandEol") != null ? System.Convert.ToDouble(obj.PropertyValue("primaryEnergyDemandEol")) : double.NaN,
                DepletionOfAbioticResourcesFossilFuels = obj.PropertyValue("primaryEnergyDemand") != null ? System.Convert.ToDouble(obj.PropertyValue("primaryEnergyDemand")) : double.NaN,
                EutrophicationPotential = obj.PropertyValue("eutrophicationPotential") != null ? System.Convert.ToDouble(obj.PropertyValue("eutrophicationPotential")) : double.NaN,
                AcidificationPotential = obj.PropertyValue("acidificationPotential") != null ? System.Convert.ToDouble(obj.PropertyValue("acidificationPotential")) : double.NaN,
                PhotochemicalOzoneCreationPotential = obj.PropertyValue("smogPotential") != null ? System.Convert.ToDouble(obj.PropertyValue("smogPotential")) : double.NaN,
                OzoneDepletionPotential = obj.PropertyValue("ozoneDepletionPotential") != null ? System.Convert.ToDouble(obj.PropertyValue("ozoneDepletionPotential")) : double.NaN,
                GlobalWarmingPotential = obj.PropertyValue("globalWarmingPotential") != null ? System.Convert.ToDouble(obj.PropertyValue("globalWarmingPotential")) : double.NaN,
                Scope = obj.PropertyValue("scope")?.ToString() ?? "",
                Description = obj.PropertyValue("description")?.ToString() ?? "",
                DeclaredUnit = obj.PropertyValue("declaredUnit")?.ToString() ?? "",
                BiogenicEmbodiedCarbon = obj.PropertyValue("biogenicEmbodiedCarbon") != null ? System.Convert.ToDouble(obj.PropertyValue("biogenicEmbodiedCarbon")) : double.NaN,
                GwpPerDeclaredUnit = obj.PropertyValue("gwpPerDeclaredUnit")?.ToString() ?? "",
                GwpPerKG = obj.PropertyValue("gwpPerKG")?.ToString() ?? "", //needs string splitting to extract units and convert to double.
                Density = obj.PropertyValue("density")?.ToString() ?? "", //additional converts needed because this type is string not double.
                EndOfLifeTreatment = obj.PropertyValue("eolTreatment")?.ToString() ?? "",
            };
            return epd;
        }
    }
}

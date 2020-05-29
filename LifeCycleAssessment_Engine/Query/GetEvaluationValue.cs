using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using BH.Engine.Base;
using BH.Engine.Reflection;
using BH.oM.Base;
using BH.oM.LifeCycleAssessment;
using BH.oM.LifeCycleAssessment.MaterialFragments;

namespace BH.Engine.LifeCycleAssessment
{
    public static partial class Query
    {
        //public static double GetFragmentVolume(this IEnvironmentalProductDeclarationData epd)
        //{
        //    if(epd == null)
        //    {
        //        return double.NaN;
        //    }
        //    else
        //    {
        //        object volume = 0.0;
        //        volume = System.Convert.ToDouble(epd.PropertyValue("Volume"));

        //        return System.Convert.ToDouble(volume);
        //    }
        //}

        public static double GetFragmentDensity(this IEnvironmentalProductDeclarationData epd)
        {
            if(epd == null)
            {
                return double.NaN;
            }
            else
            {
                object density = 0.0;
                density = System.Convert.ToDouble(epd.PropertyValue("Density"));

                return System.Convert.ToDouble(density);
            }
        }

        public static object GetFragmentQuantityType(this IEnvironmentalProductDeclarationData epd)
        {
            if(epd == null)
            {
                BH.Engine.Reflection.Compute.RecordError("The input object must have a Volume property for this method to work.");
            }
            switch (epd.QuantityType)
            {
                case QuantityType.Area:
                case QuantityType.Item:
                case QuantityType.Mass:
                case QuantityType.Length:
                case QuantityType.Undefined:
                case QuantityType.Volume:
                default:
                    return QuantityType.Undefined;
            }
        }

        public static double GetEvaluationValue(this IEnvironmentalProductDeclarationData epd, EnvironmentalProductDeclarationField field)
        {
            if (epd == null)
                return double.NaN;

            switch(field)
            {
                case EnvironmentalProductDeclarationField.AcidificationPotential:
                    return epd.AcidificationPotential;
                case EnvironmentalProductDeclarationField.AcidificationPotentialEndOfLife:
                    return epd.AcidificationPotentialEndOfLife;
                case EnvironmentalProductDeclarationField.DepletionOfAbioticResourcesFossilFuels:
                    return epd.DepletionOfAbioticResourcesFossilFuels;
                case EnvironmentalProductDeclarationField.DepletionOfAbioticResourcesFossilFuelsEndOfLife:
                    return epd.DepletionOfAbioticResourcesFossilFuelsEndOfLife;
                case EnvironmentalProductDeclarationField.EutrophicationPotential:
                    return epd.EutrophicationPotential;
                case EnvironmentalProductDeclarationField.EutrophicationPotentialEndOfLife:
                    return epd.EutrophicationPotentialEndOfLife;
                case EnvironmentalProductDeclarationField.GlobalWarmingPotential:
                    return epd.GlobalWarmingPotential;
                case EnvironmentalProductDeclarationField.GlobalWarmingPotentialEndOfLife:
                    return epd.GlobalWarmingPotentialEndOfLife;
                case EnvironmentalProductDeclarationField.OzoneDepletionPotential:
                    return epd.OzoneDepletionPotential;
                case EnvironmentalProductDeclarationField.OzoneDepletionPotentialEndOfLife:
                    return epd.OzoneDepletionPotentialEndOfLife;
                case EnvironmentalProductDeclarationField.PhotochemicalOzoneCreationPotential:
                    return epd.PhotochemicalOzoneCreationPotential;
                case EnvironmentalProductDeclarationField.PhotochemicalOzoneCreationPotentialEndOfLife:
                    return epd.PhotochemicalOzoneCreationPotentialEndOfLife;
                default:
                    return double.NaN;
            }
        }

        public static double GetEvaluationValue(this IBHoMObject lcaScope, EnvironmentalProductDeclarationField field)
        {
            return lcaScope.GetEvaluationValue(field);
        }

        [Description("Return a sum of all Material Fragment values from a specified EnvironmentalProductDeclarationField within any StructuresScope object used within a ProjectLifeCycleAssessment.")]
        public static double GetEvaluationValueStructuresScope(this StructuresScope obj, EnvironmentalProductDeclarationField field)
        {
            double val = 0;

            val += obj.StructuresSlabs.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetEvaluationValue(field)).Sum();
            val += obj.StructuresBeams.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetEvaluationValue(field)).Sum();
            val += obj.StructuresColumns.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetEvaluationValue(field)).Sum();
            val += obj.StructuresCoreWalls.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetEvaluationValue(field)).Sum();

            return val;
        }

        [Description("Return a sum of all Material Fragment values from a specified EnvironmentalProductDeclarationField within any FoundationsScope object used within a ProjectLifeCycleAssessment.")]
        public static double GetEvaluationValueFoundationsScope(this FoundationsScope obj, EnvironmentalProductDeclarationField field)
        {
            double val = 0;

            val += obj.FoundationsFootings.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetEvaluationValue(field)).Sum();
            val += obj.FoundationsPiles.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetEvaluationValue(field)).Sum();
            val += obj.FoundationsSlabs.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetEvaluationValue(field)).Sum();
            val += obj.FoundationsWalls.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetEvaluationValue(field)).Sum();

            return val;
        }

        [Description("Return a sum of all Material Fragment values from a specified EnvironmentalProductDeclarationField within any EnclosuresScope object used within a ProjectLifeCycleAssessment.")]
        public static double GetEvaluationValueEnclosuresScope(this EnclosuresScope obj, EnvironmentalProductDeclarationField field)
        {
            double val = 0;

            val += obj.EnclosuresCurtainWalls.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetEvaluationValue(field)).Sum();
            val += obj.EnclosuresDoors.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetEvaluationValue(field)).Sum();
            val += obj.EnclosuresWalls.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetEvaluationValue(field)).Sum();
            val += obj.EnclosuresWindows.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetEvaluationValue(field)).Sum();

            return val;
        }

        [Description("Return a sum of all Material Fragment values from a specified EnvironmentalProductDeclarationField within any EnclosuresScope object used within a ProjectLifeCycleAssessment.")]
        public static double GetEvaluationValueMEPScope(this MEPScope obj, EnvironmentalProductDeclarationField field)
        {
            double val = 0;

            val += obj.MEPBatteries.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetEvaluationValue(field)).Sum();
            val += obj.MEPConduit.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetEvaluationValue(field)).Sum();
            val += obj.MEPDuctwork.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetEvaluationValue(field)).Sum();
            val += obj.MEPEquipment.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetEvaluationValue(field)).Sum();
            val += obj.MEPGenerators.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetEvaluationValue(field)).Sum();
            val += obj.MEPLighting.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetEvaluationValue(field)).Sum();
            val += obj.MEPPiping.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetEvaluationValue(field)).Sum();
            val += obj.MEPWiring.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetEvaluationValue(field)).Sum();

            return val;
        }

        [Description("Return a sum of all Material Fragment values from a specified EnvironmentalProductDeclarationField within any TenantImprovementScope object used within a ProjectLifeCycleAssessment.")]
        public static double GetEvaluationValueTenantImprovementScope(this TenantImprovementScope obj, EnvironmentalProductDeclarationField field)
        {
            double val = 0;

            val += obj.TenantImprovementsCeiling.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetEvaluationValue(field)).Sum();
            val += obj.TenantImprovementsFinishes.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetEvaluationValue(field)).Sum();
            val += obj.TenantImprovementsFlooring.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetEvaluationValue(field)).Sum();
            val += obj.TenantImprovementsFurniture.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetEvaluationValue(field)).Sum();
            val += obj.TenantImprovementsInteriorDoors.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetEvaluationValue(field)).Sum();
            val += obj.TenantImprovementsInteriorGlazing.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetEvaluationValue(field)).Sum();
            val += obj.TenantImprovementsPartitionWalls.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetEvaluationValue(field)).Sum();

            return val;
        }

    }
}

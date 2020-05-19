using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

using BH.oM.LifeCycleAssessment;
using BH.oM.LifeCycleAssessment.MaterialFragments;

namespace BH.Engine.LifeCycleAssessment
{
    public static partial class Query
    {
        public static double GetEvaluationValue(this IEnvironmentalProductDeclarationData epd, EnvironmentalProductDeclarationField field)
        {
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

        public static double GetEvaluationValue(this ILifeCycleAssessmentConstruction lcaConstruction, EnvironmentalProductDeclarationField field)
        {
            return lcaConstruction.Data.GetEvaluationValue(field);
        }
        public static double GetEvaluationValue(this StructuresScope structures, EnvironmentalProductDeclarationField field)
        {
            return structures.StructuresSlabs.GetEvaluationValue(field) + structures.StructuresCoreWalls.GetEvaluationValue(field) + structures.StructuresBeams.GetEvaluationValue(field) + structures.StructuresColumns.GetEvaluationValue(field);
        }
        public static double GetEvaluationValue(this EnclosuresScope enclosures, EnvironmentalProductDeclarationField field)
        {
            return enclosures.EnclosuresCurtainWalls.GetEvaluationValue(field) + enclosures.EnclosuresDoors.GetEvaluationValue(field) + enclosures.EnclosuresWindows.GetEvaluationValue(field) + enclosures.EnclosuresWalls.GetEvaluationValue(field);
        }
        public static double GetEvaluationValue(this FoundationsScope foundations, EnvironmentalProductDeclarationField field)
        {
            return foundations.FoundationsFootings.GetEvaluationValue(field) + foundations.FoundationsPiles.GetEvaluationValue(field) + foundations.FoundationsWalls.GetEvaluationValue(field) + foundations.FoundationsSlabs.GetEvaluationValue(field);
        }
        public static double GetEvaluationValue(this MEPScope mep, EnvironmentalProductDeclarationField field)
        {
            return mep.MEPEquipment.GetEvaluationValue(field) + mep.MEPDuctwork.GetEvaluationValue(field) + mep.MEPGenerators.GetEvaluationValue(field) + mep.MEPConduit.GetEvaluationValue(field) + mep.MEPWiring.GetEvaluationValue(field) + mep.MEPLighting.GetEvaluationValue(field) + mep.MEPPiping.GetEvaluationValue(field) + mep.MEPBatteries.GetEvaluationValue(field);
        }
        public static double GetEvaluationValue(this TenantImprovementScope tentant, EnvironmentalProductDeclarationField field)
        {
            return tentant.TenantImprovementsCeiling.GetEvaluationValue(field) + tentant.TenantImprovementsFlooring.GetEvaluationValue(field) + tentant.TenantImprovementsFinishes.GetEvaluationValue(field) + tentant.TenantImprovementsInteriorGlazing.GetEvaluationValue(field) + tentant.TenantImprovementsFurniture.GetEvaluationValue(field) + tentant.TenantImprovementsInteriorDoors.GetEvaluationValue(field) + tentant.TenantImprovementsPartitionWalls.GetEvaluationValue(field);
        }
    }
}

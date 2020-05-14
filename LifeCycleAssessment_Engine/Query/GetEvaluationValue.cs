using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

using BH.oM.LifeCycleAssessment;

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
            return -1;
        }

        public static double GetEvaluationValue(this EnclosuresScope enclosures, EnvironmentalProductDeclarationField field)
        {
            return enclosures.EnclosuresCurtainWalls.GetEvaluationValue(field) + enclosures.EnclosuresDoors.GetEvaluationValue(field) + enclosures.EnclosuresWindows.GetEvaluationValue(field) + enclosures.EnclosuresWalls.GetEvaluationValue(field);
        }
    }
}

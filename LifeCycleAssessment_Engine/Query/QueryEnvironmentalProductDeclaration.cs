using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BH.oM.LifeCycleAssessment;

namespace BH.Engine.LifeCycleAssessment
{
    public static partial class Query
    {
        public static double queryEnvironmentalProductDeclaration(this EnvironmentalProductDeclaration epd, EnvironmentalProductDeclarationField field)
        {
            switch(field)
            {
                case EnvironmentalProductDeclarationField.GlobalWarmingPotential:
                    return epd.GlobalWarmingPotential;
                case EnvironmentalProductDeclarationField.OzoneDepletionPotential:
                    return epd.OzoneDepletionPotential;
                case EnvironmentalProductDeclarationField.PhotochemicalOzoneCreationPotential:
                    return epd.PhotochemicalOzoneCreationPotential;
                case EnvironmentalProductDeclarationField.AcidificationPotential:
                    return epd.AcidificationPotential;
                case EnvironmentalProductDeclarationField.EutrophicationPotential:
                    return epd.EutrophicationPotential;
                case EnvironmentalProductDeclarationField.DepletionOfAbioticResourcesFossilFuels:
                    return epd.DepletionOfAbioticResourcesFossilFuels;
                case EnvironmentalProductDeclarationField.GlobalWarmingPotentialEndOfLife:
                    return epd.GlobalWarmingPotentialEndOfLife;
                case EnvironmentalProductDeclarationField.OzoneDepletionPotentialEndOfLife:
                    return epd.OzoneDepletionPotentialEndOfLife;
                case EnvironmentalProductDeclarationField.PhotochemicalOzoneCreationPotentialEndOfLife:
                    return epd.PhotochemicalOzoneCreationPotentialEndOfLife;
                case EnvironmentalProductDeclarationField.AcidificationPotentialEndOfLife:
                    return epd.AcidificationPotentialEndOfLife;
                case EnvironmentalProductDeclarationField.EutrophicationPotentialEndOfLife:
                    return epd.EutrophicationPotentialEndOfLife;
                case EnvironmentalProductDeclarationField.DepletionOfAbioticResourcesFossilFuelsEndOfLife:
                    return epd.DepletionOfAbioticResourcesFossilFuelsEndOfLife;
                default:
                    return 0;
            }
        }
    }
}

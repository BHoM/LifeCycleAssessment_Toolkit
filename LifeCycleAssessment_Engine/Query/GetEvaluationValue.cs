/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2021, the respective contributors. All rights reserved.
 *
 * Each contributor holds copyright over their respective contributions.
 * The project versioning (Git) records all such contribution source information.
 *                                           
 *                                                                              
 * The BHoM is free software: you can redistribute it and/or modify         
 * it under the terms of the GNU Lesser General Public License as published by  
 * the Free Software Foundation, either version 3.0 of the License, or          
 * (at your option) any later version.                                          
 *                                                                              
 * The BHoM is distributed in the hope that it will be useful,              
 * but WITHOUT ANY WARRANTY; without even the implied warranty of               
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the                 
 * GNU Lesser General Public License for more details.                          
 *                                                                            
 * You should have received a copy of the GNU Lesser General Public License     
 * along with this code. If not, see <https://www.gnu.org/licenses/lgpl-3.0.html>.      
 */

using BH.Engine.Matter;
using BH.oM.Dimensional;
using BH.oM.LifeCycleAssessment;
using BH.oM.LifeCycleAssessment.MaterialFragments;
using BH.oM.Reflection.Attributes;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using BH.oM.Physical.Constructions;

namespace BH.Engine.LifeCycleAssessment
{
    /***************************************************/
    /****   Public Methods                          ****/
    /***************************************************/
    public static partial class Query
    {
        [Description("Return a sum of all Material Fragment values from a specified EnvironmentalProductDeclarationField within any EPD object.")]
        [Input("epd", "Environmental Product Declaration to query the field value from.")]
        [Input("field", "Specific metric to query from provided Environmental Product Declarations.")]
        [Output("evaluationValue", "The Environmental Impact metric value for the specified field.")]
        public static double GetEvaluationValue(this IEnvironmentalProductDeclarationData epd, EnvironmentalProductDeclarationField field)
        {
            if (epd == null)
                return double.NaN;

            switch (field)
            {
                case EnvironmentalProductDeclarationField.AcidificationPotential:
                    return epd.AcidificationPotential;
                case EnvironmentalProductDeclarationField.DepletionOfAbioticResourcesFossilFuels:
                    return epd.DepletionOfAbioticResourcesFossilFuels;
                case EnvironmentalProductDeclarationField.EutrophicationPotential:
                    return epd.EutrophicationPotential;
                case EnvironmentalProductDeclarationField.GlobalWarmingPotential:
                    return epd.GlobalWarmingPotential;
                case EnvironmentalProductDeclarationField.OzoneDepletionPotential:
                    return epd.OzoneDepletionPotential;
                case EnvironmentalProductDeclarationField.PhotochemicalOzoneCreationPotential:
                    return epd.PhotochemicalOzoneCreationPotential;
                default:
                    return double.NaN;
            }
        }

        /***************************************************/

        [Description("Returns the Environmental Impact metric value for the specified field input from the Environmental Product Declaration found within the MaterialComposition of an object.")]
        [Input("elementM", "An IElementM object with a MaterialProperty from which to query the desired metric.")]
        [Input("field", "Specific metric to query from provided Environmental Product Declarations.")]
        [Input("type", "The quantityType to query.")]
        [Output("evaluationValue", "The Environmental Impact metric value for the specified field and quantityType.")]
        public static List<double> GetEvaluationValue(this IElementM elementM, EnvironmentalProductDeclarationField field, QuantityType type)
        {
            if (elementM == null)
                return new List<double>();

            List<double> quantityTypeValue = elementM.GetQuantityTypeValue(type);

            List<double> epdVal = elementM.IMaterialComposition().Materials.Select(x =>
            {
                var epd = x.Properties.Where(y => y is IEnvironmentalProductDeclarationData).FirstOrDefault() as IEnvironmentalProductDeclarationData;
                if (epd.QuantityType == type)
                    return GetEvaluationValue(epd, field);
                else
                    return double.NaN;

            }).ToList();

            //Division of GWP constant by QTV
            List<double> normalisedEpdVal = new List<double>();

            for (int x = 0; x < epdVal.Count; x++)
            {
                if (double.IsNaN(epdVal[x]))
                    normalisedEpdVal.Add(double.NaN);
                else
                    normalisedEpdVal.Add(epdVal[x] / quantityTypeValue[x]);
            }

            return normalisedEpdVal;
        }
        /***************************************************/

        [Description("Returns the Environmental Impact metric value for the specified field input from the Environmental Product Declaration found within a construction.")]
        [Input("construction", "An physical construction used to define material properties of an object.")]
        [Input("field", "Specific metric to query from provided Environmental Product Declarations.")]
        [Input("type", "The quantityType to query.")]
        [Output("evaluationValue", "The Environmental Impact metric value for the specified field and quantityType.")]
        public static List<double> GetEvaluationValue(this Construction construction, EnvironmentalProductDeclarationField field, QuantityType type)
        {
            if (construction == null)
                return new List<double>();

            List<double> quantityTypeValue = construction.GetQuantityTypeValue(type);

            List<double> epdVal = construction.Layers.Select(x =>
            {
                var epd = x.Material.Properties.Where(y => y is IEnvironmentalProductDeclarationData).FirstOrDefault() as IEnvironmentalProductDeclarationData;
                if (epd.QuantityType == type)
                    return GetEvaluationValue(epd, field);
                else
                    return double.NaN;
            }).ToList();

            //Division of GWP constant by QTV
            List<double> normalisedEpdVal = new List<double>();

            for (int x = 0; x < epdVal.Count; x++)
            {
                if (double.IsNaN(epdVal[x]))
                    normalisedEpdVal.Add(double.NaN);
                else
                    normalisedEpdVal.Add(epdVal[x] / quantityTypeValue[x]);
            }

            return normalisedEpdVal;
        }
        /***************************************************/
    }
}


/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2022, the respective contributors. All rights reserved.
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
using BH.oM.Base.Attributes;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using BH.oM.Physical.Constructions;
using BH.oM.Physical.Materials;

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
        [Input("phases", "A phase filter.")]
        [Input("exactMatch", "If true, the evaluation method will force an exact LCA phase match to solve for.")]
        [Output("evaluationValue", "The Environmental Impact metric value for the specified field.")]

        public static double GetEvaluationValue(this EnvironmentalProductDeclaration epd, EnvironmentalProductDeclarationField field, List<LifeCycleAssessmentPhases> phases, bool exactMatch = false)
        {
            if (epd == null)
                return double.NaN;

            IEnumerable<EnvironmentalMetric> filteredMetrics = epd.EnvironmentalMetric.Where(x => x.Field == field);
            if(filteredMetrics.Count() == 0)
            {
                BH.Engine.Base.Compute.RecordError("No metrics of the specified Field could be found.");
                return double.NaN;
            }

            filteredMetrics = filteredMetrics.Where(x => x.GetType().IsAssignableFrom(typeof(EnvironmentalMetric)));

            if(filteredMetrics.Count() == 0)
            {
                BH.Engine.Base.Compute.RecordError("No Environmental Metrics could be found.");
                return double.NaN;
            }

            if(!filteredMetrics.SelectMany(x => x.Phases).IsContaining(phases, exactMatch))
            {
                BH.Engine.Base.Compute.RecordError("There are no matching phases found within the Environmental Metrics of the provided EPDs.");
                return double.NaN;
            }

            return filteredMetrics.First().Quantity;
        }

        /***************************************************/

        [Description("Returns the Environmental Impact metric value for the specified field input from the Environmental Product Declaration found within the MaterialComposition of an object.")]
        [Input("elementM", "An IElementM object with a MaterialProperty from which to query the desired metric.")]
        [Input("field", "Specific metric to query from provided EPD.")]
        [Input("phases", "Provide phases of life you wish to evaluate. Phases of life must be documented within EPDs for this method to work.")]
        [Input("type", "The quantityType to query.")]
        [Input("exactMatch", "If true, the evaluation method will force an exact LCA phase match to solve for.")]
        [Output("evaluationValue", "The Environmental Impact metric value for the specified field and quantityType.")]
        public static List<double> GetEvaluationValue(this IElementM elementM, EnvironmentalProductDeclarationField field, List<LifeCycleAssessmentPhases> phases, QuantityType type, bool exactMatch = false)
        {
            MaterialComposition mc = elementM.IMaterialComposition();

            return GetEvaluationValue(elementM, field, phases, type, mc, exactMatch);
        }

        /***************************************************/

        [Description("Returns the Environmental Impact metric value for the specified field input from the Environmental Product Declaration found within the MaterialComposition of an object.")]
        [Input("elementM", "An IElementM object with a MaterialProperty from which to query the desired metric.")]
        [Input("field", "Specific metric to query from provided EPD.")]
        [Input("phases", "Provide phases of life you wish to evaluate. Phases of life must be documented within EPDs for this method to work.")]
        [Input("type", "The quantityType to query.")]
        [Input("materialComposition", "The material composition of the element using physical materials.")]
        [Input("exactMatch", "If true, the evaluation method will force an exact LCA phase match to solve for.")]
        [Output("evaluationValue", "The Environmental Impact metric value for the specified field and quantityType.")]

        private static List<double> GetEvaluationValue(this IElementM elementM, EnvironmentalProductDeclarationField field, List<LifeCycleAssessmentPhases> phases, QuantityType type, MaterialComposition materialComposition, bool exactMatch = false)
        {
            if(materialComposition == null)
            {
                BH.Engine.Base.Compute.RecordError("The material composition of the element could not be returned.");
                return new List<double>();
            }

            if (elementM == null)
                return new List<double>();

            List<double> quantityTypeValue = elementM.GetQuantityTypeValue(type, materialComposition);

            List<double> epdVal = materialComposition.Materials.Select(x =>
            {
                var epd = x.Properties.Where(y => y is EnvironmentalProductDeclaration).FirstOrDefault() as EnvironmentalProductDeclaration;

                if (epd.QuantityType == type && (epd.EnvironmentalMetric.Where(z => z.Phases.Where(a => phases.Contains(a)).Count() != 0).FirstOrDefault() != null))
                    return GetEvaluationValue(epd, field, phases, exactMatch);
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
        [Input("phases", "Provide phases of life you wish to evaluate. Phases of life must be documented within EPDs for this method to work.")]
        [Input("type", "The quantityType to query.")]
        [Input("exactMatch", "If true, the evaluation method will force an exact LCA phase match to solve for.")]
        [Output("evaluationValue", "The Environmental Impact metric value for the specified field and quantityType.")]
        public static List<double> GetEvaluationValue(this Construction construction, EnvironmentalProductDeclarationField field, List<LifeCycleAssessmentPhases> phases, QuantityType type, bool exactMatch = false)
        {
            if (construction == null)
                return new List<double>();

            List<double> quantityTypeValue = construction.GetQuantityTypeValue(type);

            List<double> epdVal = construction.Layers.Select(x =>
            {
                var epd = x.Material.Properties.Where(y => y is EnvironmentalProductDeclaration).FirstOrDefault() as EnvironmentalProductDeclaration;

                if (epd.QuantityType == type && (epd.EnvironmentalMetric.Where(z => z.Phases.Where(a => phases.Contains(a)).Count() != 0).FirstOrDefault() != null))
                    return GetEvaluationValue(epd, field, phases, exactMatch);
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

        private static bool IsContaining(this IEnumerable<LifeCycleAssessmentPhases> phasesToSearch, IEnumerable<LifeCycleAssessmentPhases> phasesToFilterBy, bool exact)
        {
            //This will return whether a collection of Phases (phasesToSearch) is contained within the filtering phases (phasesToFilterBy)
            //If the flag for exact is true then every phase in the phasesToFilterBy must exist in the phasesToSearch
            //If the flag for exact is false then only one phase from the phasesToFilterBy must exist in the phasesToSearch

            if (!exact)
                return phasesToSearch.Where(x => phasesToFilterBy.Contains(x)).Count() > 0;

            List<LifeCycleAssessmentPhases> phases = phasesToSearch.ToList();

            bool isContained = true;
            foreach (LifeCycleAssessmentPhases phase in phasesToFilterBy)
            {
                isContained &= phases.Contains(phase);
                phases.Remove(phase);
            }

            isContained &= phases.Count == 0;

            return isContained;
        }
    }
}



/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2023, the respective contributors. All rights reserved.
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

using BH.oM.Base.Attributes;
using BH.oM.LifeCycleAssessment.MaterialFragments;
using BH.oM.LifeCycleAssessment.Results;
using System.Collections.Generic;
using System.ComponentModel;

namespace BH.Engine.LifeCycleAssessment
{
    public static partial class Compute
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Evaluates the EnvironmentalMetric and returns a MaterialResult of a type corresponding to the metric. The evaluation is done by multiplying all phase data on the metric by the provided quantityValue.\n" +
                     "Please be mindful that the unit of the quantityValue should match the QuantityType on the EnvironmentalProductDeclaration to which the metric belongs.")]
        [Input("metric", "The EnvironmentalMetric to evaluate. Returned result will be a MaterialResult of a type corresponding to the metric.")]
        [Input("quantityValue", "The quantity value to evaluate all metrics by. All metric properties will be multiplied by this value. Quantity should correspond to the QuantityType on the EPD.")]
        [Input("epdName", "The name of the EnvironmentalProductDeclaration that owns the EnvironmentalMetric. Stored as an identifier on the returned result class.")]
        [Input("materialName", "The name of the Material that owns the EnvironmentalProductDeclaration. Stored as an identifier on the returned result class.")]
        [Output("result", "A MaterialResult of a type corresponding to the evaluated metric with phase data calculated as data on metric multiplied by the provided quantity value.")]
        public static MaterialResult EvaluateEnvironmentalMetric(EnvironmentalMetric metric, string epdName, string materialName, double quantityValue)
        {
            if (metric == null)
            {
                Base.Compute.RecordError($"Cannot evaluate a null {nameof(EnvironmentalMetric)}.");
                return null;
            }

            return Create.MaterialResult(metric.GetType(), materialName, epdName, metric.EvaluateEnvironmentalMetricValues(quantityValue));

        }

        /***************************************************/

        [Description("Gets the resulting values for each phase of the provided EnvironmentalMetric given the provided quantityValue.\n" +
                     "The resulting values are computed as the values on the metric for each phase multiplied by the quantity value.\n" +
                     "Please be mindful that the unit of the quantityValue should match the QuantityType on the EnvironmentalProductDeclaration to which the metric belongs.")]
        [Input("metric", "The EnvironmentalMetric to get resulting values for. All phase values on the metric will be extracted and multiplied by the qunatityValue.")]
        [Input("quantityValue", "The quantity value to evaluate all metrics by. All metric properties will be multiplied by this value. Quantity should correspond to the QuantityType on the EPD.")]
        [Output("resultValues", "The resulting values for each phase.")]
        public static List<double> EvaluateEnvironmentalMetricValues(this EnvironmentalMetric metric, double quantityValue)
        {
            if (metric == null)
            {
                Base.Compute.RecordError($"Cannot evaluate a null {nameof(EnvironmentalMetric)}.");
                return null;
            }

            List<double> resultingValues = new List<double>();
            foreach (double phaseData in metric.IPhaseDataValues())
            {
                resultingValues.Add(phaseData * quantityValue);  //Evaluation value is base phase data multiplied by quantity value
            }
            return resultingValues;
        }

        /***************************************************/
    }
}

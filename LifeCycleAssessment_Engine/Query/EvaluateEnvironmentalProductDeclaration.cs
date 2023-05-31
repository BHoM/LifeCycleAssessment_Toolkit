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

using BH.Engine.Base;
using BH.oM.Base;
using BH.oM.Base.Attributes;
using BH.oM.LifeCycleAssessment;
using BH.oM.LifeCycleAssessment.Configs;
using BH.oM.LifeCycleAssessment.MaterialFragments;
using BH.oM.LifeCycleAssessment.Results;
using BH.oM.Physical.Constructions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace BH.Engine.LifeCycleAssessment
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [PreviousVersion("6.2", "BH.Engine.LifeCycleAssessment.Compute.EvaluateReferenceValue(System.Double, BH.oM.LifeCycleAssessment.MaterialFragments.EnvironmentalProductDeclaration, BH.oM.LifeCycleAssessment.EnvironmentalProductDeclarationField, System.Collections.Generic.List<BH.oM.LifeCycleAssessment.LifeCycleAssessmentPhases>, System.Boolean)")]
        [Description("Evaluates all or selected metrics stored on the EnvironmentalProductDeclaration (EPD) and returns a result per metric.\n" +
                     "Each metric is evaluated by multiplying the values for each phase by the provided quantityValue.\n" +
                     "Please be mindful that the unit of the quantityValue should match the QuantityType on the EnvironmentalProductDeclaration.")]
        [Input("epd", "The EnvironmentalProductDeclaration to evaluate. Returned results will correspond to all, or selected, metrics stored on this object.")]
        [Input("quantityValue", "The quantity value to evaluate all metrics by. All metric properties will be multiplied by this value. Quantity should correspond to the QuantityType on the EPD.")]
        [Input("materialName", "The name of the Material that owns the EnvironmentalProductDeclaration. Stored as an identifier on the returned result classes.")]
        [Input("metricFilter", "Optional filter for the provided EnvironmentalProductDeclaration for selecting one or more of the provided metrics for calculation. This method also accepts multiple metric types simultaneously. If nothing is provided then no filtering is assumed, i.e. all metrics on the found EPDS are evaluated.")]
        [Input("evaluationConfig", "Config controlling how the metrics should be evaluated, may contain additional parameters for the evaluation. If no config is provided the default evaluation mechanism is used which computes resulting phase values as metric value times applicable quantity.")]
        [Output("results", "List of MaterialResults corresponding to the evaluated metrics on the EPD.")]
        [PreviousInputNames("quantityValue", "referenceValue")]
        public static List<MaterialResult> EvaluateEnvironmentalProductDeclaration(EnvironmentalProductDeclaration epd, double quantityValue, string materialName = "", List<EnvironmentalMetrics> metricFilter = null, IEvaluationConfig evaluationConfig = null)
        {
            if (epd == null)
            {
                Base.Compute.RecordError($"Cannot evaluate a null {nameof(EnvironmentalProductDeclaration)}.");
                return null;
            }

            if (!IValidateConfig(evaluationConfig, epd))
                return new List<MaterialResult>();

            List<MaterialResult> results = new List<MaterialResult>();

            foreach (EnvironmentalMetric metric in epd.FilteredMetrics(metricFilter))
            {
                results.Add(EvaluateEnvironmentalMetric(metric, epd.Name, materialName, quantityValue, evaluationConfig));
            }

            return results;
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static bool IValidateConfig(IEvaluationConfig config, EnvironmentalProductDeclaration epd)
        {
            if (config == null) //Null config is valid, as default case of evaluation is assumed for provided null config.
                return true;

            return ValidateConfig(config, epd);
        }

        /***************************************************/

        private static bool ValidateConfig(IStructEEvaluationConfig config, EnvironmentalProductDeclaration epd)
        {
            bool valid = epd.QuantityType == QuantityType.Mass;

            if (!valid)
                BH.Engine.Base.Compute.RecordError($"{nameof(IStructEEvaluationConfig)} is only valid to be used with epds with quantity type mass.");
            return valid;
        }

        /***************************************************/
        /**** Private Methods - Fallback                ****/
        /***************************************************/

        private static bool ValidateConfig(IEvaluationConfig config, EnvironmentalProductDeclaration epd)
        {
            return true;    //Default to true for fallback
        }

        /***************************************************/
    }
}

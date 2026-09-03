/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2026, the respective contributors. All rights reserved.
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


using BH.oM.Base;
using BH.oM.Base.Attributes;
using BH.oM.LifeCycleAssessment;
using BH.oM.LifeCycleAssessment.Configs;
using BH.oM.LifeCycleAssessment.Interfaces;
using BH.oM.LifeCycleAssessment.MaterialFragments;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;

namespace BH.Engine.LifeCycleAssessment
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [PreviousVersion("9.3", "BH.Engine.LifeCycleAssessment.Query.IResultingModuleValues(BH.oM.LifeCycleAssessment.MaterialFragments.IEnvironmentalMetric, System.Double, BH.oM.LifeCycleAssessment.Configs.IEvaluationConfig, System.Collections.Generic.Dictionary<BH.oM.LifeCycleAssessment.Module, BH.oM.LifeCycleAssessment.MaterialFragments.PrecomputedModuleValues>, System.Object)")]
        [Description("Gets the resulting values for each module of the provided EnvironmentalMetric given the provided quantityValue.\n" +
         "The resulting values are computed based on provided config, defaulting to the values on the metric for each module multiplied by the quantity value.\n" +
         "Please be mindful that the unit of the quantityValue should match the QuantityType on the EnvironmentalProductDeclaration to which the metric belongs.")]
        [Input("metric", "The EnvironmentalMetric to get resulting values for. All module values on the metric will be extracted and multiplied by the qunatityValue.")]
        [Input("quantityValue", "The quantity value to evaluate all metrics by. All metric properties will be multiplied by this value. Quantity should correspond to the QuantityType on the EPD.")]
        [Input("precomputedValues", "Precomputed values for particular modules and metrics where the result values have been computed by other means than taking the quantityvalue times the metric factor. This can be user set, but is also used by the internal system for IEvaluationCOnfigs as well as for pre-computed values for things like transport and waste management on the CombinedLifecycleAssessmentFactors.")]
        [Output("resultValues", "The resulting values for each module.")]
        public static Dictionary<Module, double> IResultingModuleValues(this IEnvironmentalMetric metric, double quantityValue, Dictionary<Module, PrecomputedModuleValues> precomputedValues)
        {
            if (metric == null)
            {
                Base.Compute.RecordError($"Cannot evaluate a null {nameof(IEnvironmentalMetric)}.");
                return null;
            }

            if (metric is IDeprecatedStandard)
            {
                MetricType type = metric.IMetricType();
                Base.Compute.RecordWarning($"Please note that the metric of type {type} that is evaluated comes from an older standard and that the resulting values are incompatible in terms of quantity and unit to metrics from the EN 15804+A2 standard.\n" +
                       $"Resulting values for the metrics can only be compared with other evaluated metrics from the exact same standard.");

            }
            if (metric.Indicators.Count == 0)
                return new Dictionary<Module, double>();

            return ResultingModuleValues(metric, quantityValue, precomputedValues);

        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        [Description("Default methodology for getting the resulting values for each module of the provided EnvironmentalMetric given the provided quantityValue.\n" +
                     "The resulting values are computed as the values on the metric for each module multiplied by the quantity value.\n" +
                     "Please be mindful that the unit of the quantityValue should match the QuantityType on the EnvironmentalProductDeclaration to which the metric belongs.")]
        [Input("metric", "The EnvironmentalMetric to get resulting values for. All module values on the metric will be extracted and multiplied by the qunatityValue.")]
        [Input("quantityValue", "The quantity value to evaluate all metrics by. All metric properties will be multiplied by this value. Quantity should correspond to the QuantityType on the EPD.")]
        [Output("resultValues", "The resulting values for each module.")]
        private static Dictionary<Module, double> ResultingModuleValues(this IEnvironmentalMetric metric, double quantityValue, Dictionary<Module, PrecomputedModuleValues> precomputedValues)
        {
            Dictionary<Module, double> resultingValues = new Dictionary<Module, double>();
            foreach (var moduleData in metric.Indicators)
            {
                resultingValues[moduleData.Key] =  moduleData.Value * quantityValue ;  //Evaluation value is base module data multiplied by quantity value
            }

            if (precomputedValues != null)
            {
                var partModules = PartOfCombinationModules();   //Get a dictionary that for each modules lists the combinations it is part of
                var combinationModules = CombinationModules(); //Get a dictionary that for each combination lists its parts
                HashSet<Module> precompuedValueSet = new HashSet<Module>();
                foreach (var precomputed in precomputedValues)
                {
                    bool setValue = precomputed.Value.OverwriteExistingValues || !resultingValues.ContainsKey(precomputed.Key); //Only set value if overwrite is true or if no value exists yet
                    if (!setValue)
                        continue;

                    if (precomputed.Value.ModuleValues.TryGetValue(metric.IMetricType(), out double val))   //Check if precomputed value exists for the metric type
                    {
                        precompuedValueSet.Add(precomputed.Key);
                        resultingValues[precomputed.Key] = val;         //Set the precomputed value. It is assumed that the precomputed value is already evaluated for the quantity
                        if (partModules.TryGetValue(precomputed.Key, out var combinations))
                        {
                            foreach(var combination in combinations)
                            {
                                if(!precompuedValueSet.Contains(combination))   //If not explicitly set
                                    resultingValues.Remove(combination);    //Remove any combination that includes the precomputed module as it will be different than the sum of its parts
                            }
                        }
                        resultingValues.RemoveCombinationParts(precomputed.Key, combinationModules, precompuedValueSet); //If precomputed value is a combination, then remove all its part as they will be different than the combination value
                    }

                }
            }
            return resultingValues;
        }

        /***************************************************/

        private static void RemoveCombinationParts(this Dictionary<Module, double> resultingValues, Module module, IReadOnlyDictionary<Module, IReadOnlyList<(Module, bool)>> combinations, HashSet<Module> precompuedValueSet)
        {
            if (combinations.TryGetValue(module, out var parts))
            {
                foreach (var part in parts)
                {
                    if (!precompuedValueSet.Contains(part.Item1))   //If not explicitly set
                        resultingValues.Remove(part.Item1);
                    RemoveCombinationParts(resultingValues, part.Item1, combinations, precompuedValueSet); //Recursively remove parts of parts
                }
            }
        }

        /***************************************************/

    }
}




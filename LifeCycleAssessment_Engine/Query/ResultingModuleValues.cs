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

        [PreviousVersion("10.0", "BH.Engine.LifeCycleAssessment.Query.IResultingModuleValues(BH.oM.LifeCycleAssessment.MaterialFragments.IEnvironmentalMetric, System.Double, BH.oM.LifeCycleAssessment.Configs.IEvaluationConfig, System.Collections.Generic.Dictionary<BH.oM.LifeCycleAssessment.Module, BH.oM.LifeCycleAssessment.MaterialFragments.PrecomputedModuleValues>, System.Object)")]
        [Description("Gets the resulting values for each module of the provided EnvironmentalMetric given the provided quantityValue.\n" +
         "The resulting values are computed based on provided config, defaulting to the values on the metric for each module multiplied by the quantity value.\n" +
         "Please be mindful that the unit of the quantityValue should match the QuantityType on the EnvironmentalProductDeclaration to which the metric belongs.")]
        [Input("metric", "The EnvironmentalMetric to get resulting values for. All module values on the metric will be extracted and multiplied by the qunatityValue.")]
        [Input("quantityValue", "The quantity value to evaluate all metrics by. All metric properties will be multiplied by this value. Quantity should correspond to the QuantityType on the EPD.")]
        [Input("precomputedValues", "Precomputed values for particular modules and metrics where the result values have been computed by other means than taking the quantityvalue times the metric factor. This can be user set, but is also used by the internal system for IEvaluationCOnfigs as well as for pre-computed values for things like transport and waste management on the CombinedLifecycleAssessmentFactors.")]
        [Output("resultValues", "The resulting values for each module.")]
        public static Dictionary<Module, double> ResultingModuleValues(this IEnvironmentalMetric metric, double quantityValue, Dictionary<Module, PrecomputedModuleValues> precomputedValues)
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


            //Compute the resulting values for each module based on the metric and quantity value
            //This is the main loop that computes the resulting values for each module based on the metric and quantity value and for most evaluation, this is the critical step
            Dictionary<Module, double> resultingValues = new Dictionary<Module, double>();
            foreach (var moduleData in metric.Indicators)
            {
                resultingValues[moduleData.Key] = moduleData.Value * quantityValue;  //Evaluation value is base module data multiplied by quantity value
            }

            //If precomputed values are provided, then they will be used to overwrite the resulting values for the corresponding modules.
            //This is critical to ensure that precomputed values are respected and that any combinations that include the precomputed modules are removed from the resulting values as they will be different than the sum of their parts.
            //This is, if a precomputed value is provided for for example A5_1, then the resulting values will not include A5, as it will be different than the sum of its parts (A5_1 + A5_2 + A5_3). Similarly, if a precomputed value is provided for A1toA3, then the resulting values will not include A1, A2 or A3, as they will be different than the combination value.
            //When called as part of generating MaterialResults, the Combination modules will be computed at the end of the evaluation, so that they are always included in the resulting values, hence do not need to be computed here.
            //The culling of combinations and parts is handled by the SetModuleValues method.
            if (precomputedValues != null)
            {
                List<Module> modules = new List<Module>();
                List<double> values = new List<double>();
                MetricType metricType = metric.IMetricType();
                //Loop through the precomputed values and store out the modules and values for the cases where the precomputed values contain a value for the metric type.
                foreach (var preComputed in precomputedValues)
                {
                    if (preComputed.Value.ModuleValues.TryGetValue(metricType, out double val))
                    {
                        modules.Add(preComputed.Key);
                        values.Add(val);
                    }
                }
                //Set the precomputed values on the resulting values. Called with boolean set to true which will overwrite any existing values for the modules and remove any combinations or parts that are included in the precomputed values.
                resultingValues.SetModuleValues(modules, values, true);
            }

            return resultingValues;
        }

        /***************************************************/

    }
}




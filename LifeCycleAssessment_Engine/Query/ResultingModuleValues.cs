/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2025, the respective contributors. All rights reserved.
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
using BH.Engine.Matter;
using BH.oM.Base;
using BH.oM.Base.Attributes;
using BH.oM.Dimensional;
using BH.oM.LifeCycleAssessment;
using BH.oM.LifeCycleAssessment.Configs;
using BH.oM.LifeCycleAssessment.Fragments;
using BH.oM.LifeCycleAssessment.MaterialFragments;
using BH.oM.LifeCycleAssessment.MaterialFragments.Transport;
using BH.oM.LifeCycleAssessment.Results;
using BH.oM.LifeCycleAssessment.Results.MetricsValues;
using BH.oM.Physical.Materials;
using System;
using System.Collections;
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

        [Description("Gets the resulting values for each phase of the provided EnvironmentalMetric given the provided quantityValue.\n" +
         "The resulting values are computed based on provided config, defaulting to the values on the metric for each phase multiplied by the quantity value.\n" +
         "Please be mindful that the unit of the quantityValue should match the QuantityType on the EnvironmentalProductDeclaration to which the metric belongs.")]
        [Input("metric", "The EnvironmentalMetric to get resulting values for. All phase values on the metric will be extracted and multiplied by the qunatityValue.")]
        [Input("quantityValue", "The quantity value to evaluate all metrics by. All metric properties will be multiplied by this value. Quantity should correspond to the QuantityType on the EPD.")]
        [Input("evaluationConfig", "Config controlling how the metrics should be evaluated, may contain additional parameters for the evaluation. If no config is provided the default evaluation mechanism is used which computes resulting phase values as metric value times applicable quantity.")]
        [Output("resultValues", "The resulting values for each phase.")]
        public static Dictionary<Module, double> IResultingModuleValues(this IEnvironmentalMetricFactors metric, double quantityValue, IEvaluationConfig evaluationConfig)
        {
            if (metric == null)
            {
                Base.Compute.RecordError($"Cannot evaluate a null {nameof(EnvironmentalMetric)}.");
                return null;
            }

            MetricType type = metric.IMetricType();

            switch (type)
            {
                case oM.LifeCycleAssessment.MetricType.EutrophicationCML:
                case oM.LifeCycleAssessment.MetricType.EutrophicationTRACI:
                case oM.LifeCycleAssessment.MetricType.PhotochemicalOzoneCreationCML:
                case oM.LifeCycleAssessment.MetricType.PhotochemicalOzoneCreationTRACI:
                    Base.Compute.RecordWarning($"Please note that the metric of type {type} that is evaluated comes from an older standard and that the resulting values are incompatible in terms of quantity and unit to metrics from the EN 15804+A2 standard.\n" +
                           $"Resulting values for the metrics can only be compared with other evaluated metrics from the exact same standard.");

                    break;
            }
            if (metric.Factors.Count == 0)
                return new Dictionary<Module, double>();



            if (evaluationConfig == null)   //For case of null config, use default evaluation methodology of phase data value * quantity for each phase
                return ResultingModuleValues(metric, quantityValue);
            else
                return ResultingModuleValues(metric, quantityValue, evaluationConfig as dynamic);
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        [Description("Default methodology for getting the resulting values for each phase of the provided EnvironmentalMetric given the provided quantityValue.\n" +
                     "The resulting values are computed as the values on the metric for each phase multiplied by the quantity value.\n" +
                     "Please be mindful that the unit of the quantityValue should match the QuantityType on the EnvironmentalProductDeclaration to which the metric belongs.")]
        [Input("metric", "The EnvironmentalMetric to get resulting values for. All phase values on the metric will be extracted and multiplied by the qunatityValue.")]
        [Input("quantityValue", "The quantity value to evaluate all metrics by. All metric properties will be multiplied by this value. Quantity should correspond to the QuantityType on the EPD.")]
        [Output("resultValues", "The resulting values for each phase.")]
        private static Dictionary<Module, double> ResultingModuleValues(this IEnvironmentalMetricFactors metric, double quantityValue)
        {
            Dictionary<Module, double> resultingValues = new Dictionary<Module, double>();
            foreach (var moduleData in metric.Factors)
            {
                resultingValues[moduleData.Key] =  moduleData.Value * quantityValue ;  //Evaluation value is base phase data multiplied by quantity value
            }
            return resultingValues;
        }

        /***************************************************/

        [Description("IStructE Evaluation methodology for getting the resulting values for each phase of the provided EnvironmentalMetric given the provided quantityValue.\n" +
                     "Evaluation method only applicable for the two CLimateChangeTotal metric types - all other metrics are evaluated using the default mechanism.\n" +
                     "Method works for most phases works the same as default evaluation mechanism, with exception for the C1 and A5 phase where project totals are acounted for.")]
        [Input("metric", "The EnvironmentalMetric to get resulting values for. All phase values on the metric will be extracted and multiplied by the qunatityValue.")]
        [Input("quantityValue", "The quantity value to evaluate all metrics by. All metric properties will be multiplied by this value. Quantity should correspond to the QuantityType on the EPD.")]
        [Output("resultValues", "The resulting values for each phase.")]
        private static Dictionary<Module, double> ResultingModuleValues(this IEnvironmentalMetricFactors metric, double quantityValue, IStructEEvaluationConfig evaluationConfig)
        {

            //Specific evaluation method using the config only applicable for evaluatingResultingModuleValues(this Dictionary < Module, double > moduleFactors climate change totals
            MetricType metricType = metric.IMetricType();
            List<oM.LifeCycleAssessment.MetricType> applicableTypes = new List<MetricType> { oM.LifeCycleAssessment.MetricType.ClimateChangeTotal, oM.LifeCycleAssessment.MetricType.ClimateChangeTotalNoBiogenic, oM.LifeCycleAssessment.MetricType.ClimateChangeFossil };
            if (!applicableTypes.Any(x => x == metricType))
            {
                Base.Compute.RecordNote($"The {nameof(IStructEEvaluationConfig)} evaluation is only applicable for evaluating metrics of type {string.Join(",", applicableTypes)}." +
                                  $"All other metrics are evaluated based on standard evaluation procedure of phase times quantity for all phases.");
                return ResultingModuleValues(metric, quantityValue);
            }

            double weight = quantityValue;
            double weightFactor = (evaluationConfig.TotalWeight == 0 || evaluationConfig.TotalWeight < weight) ? 0 : weight / evaluationConfig.TotalWeight;

            Dictionary<Module, double> resultingValues = new Dictionary<Module, double>();
            foreach (var moduleData in metric.Factors)
            {
                //Initial value is base phase data multiplied by quantity value
                resultingValues[moduleData.Key] = moduleData.Value * quantityValue;
            }

            //Special handling of A5 for site activities module with additional project factor
            resultingValues[Module.A5a] = evaluationConfig.ProjectCost * evaluationConfig.A5CarbonFactor * weightFactor;
            
            //Set A5 as sum of A5w and A5a
            if(resultingValues.TryGetValue(Module.A5w, out double a5w))
            {
                resultingValues[Module.A5] = resultingValues[Module.A5a] + a5w;
            }

            //C1 evaluated based on project level values
            resultingValues[Module.C1] = weightFactor * evaluationConfig.FloorArea * evaluationConfig.C1CarbonFactor;

            //Check if C1toC4 was computed and needs to be udpated given the explicit computation of C1
            if (resultingValues.ContainsKey(Module.C1toC4))
            {
                //If contains all parts -> update the total
                if (resultingValues.ContainsKey(Module.C2) && resultingValues.ContainsKey(Module.C3) && resultingValues.ContainsKey(Module.C4))
                {
                    resultingValues[Module.C1toC4] = resultingValues[Module.C1] + resultingValues[Module.C2] + resultingValues[Module.C3] + resultingValues[Module.C4];

                }
                else
                    resultingValues.Remove(Module.C1toC4);   //If not, remove as total will be different than parts
            }

            return resultingValues;
        }

        /***************************************************/
        /**** Private Methods - Evaluation - Fallback   ****/
        /***************************************************/

        [Description("Fallback method for unkown config provided, raising warning and calling the defautl evaluation mechanism. Please note that this method is not triggered for null config, which also calls default mechism, but without warning.")]
        [Input("metric", "The EnvironmentalMetric to get resulting values for. All phase values on the metric will be extracted and multiplied by the qunatityValue.")]
        [Input("quantityValue", "The quantity value to evaluate all metrics by. All metric properties will be multiplied by this value. Quantity should correspond to the QuantityType on the EPD.")]
        [Output("resultValues", "The resulting values for each phase.")]
        private static Dictionary<Module, double> ResultingModuleValues(this IEnvironmentalMetricFactors metric, double quantityValue, IEvaluationConfig evaluationConfig)
        {
            BH.Engine.Base.Compute.RecordWarning($"No evaluation method implemented for evaluation config of type {evaluationConfig}. Results returned are based on default evaluation method of phase values times quantity.");

            return ResultingModuleValues(metric, quantityValue);
        }

        /***************************************************/
    }
}



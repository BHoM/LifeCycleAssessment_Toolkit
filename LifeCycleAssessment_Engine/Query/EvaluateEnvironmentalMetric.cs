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
using BH.oM.LifeCycleAssessment;
using BH.oM.LifeCycleAssessment.Configs;
using BH.oM.LifeCycleAssessment.MaterialFragments;
using BH.oM.LifeCycleAssessment.Results;
using System.Collections.Generic;
using System.ComponentModel;

namespace BH.Engine.LifeCycleAssessment
{
    public static partial class Query
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
        [Input("evaluationConfig", "Config controlling how the metrics should be evaluated, may contain additional parameters for the evaluation. If no config is provided the default evaluation mechanism is used which computes resulting phase values as metric value times quantity.")]
        [Output("result", "A MaterialResult of a type corresponding to the evaluated metric with phase data calculated as data on metric multiplied by the provided quantity value.")]
        public static MaterialResult EvaluateEnvironmentalMetric(EnvironmentalMetric metric, string epdName, string materialName, double quantityValue, IEvaluationConfig evaluationConfig = null)
        {
            if (metric == null)
            {
                Base.Compute.RecordError($"Cannot evaluate a null {nameof(EnvironmentalMetric)}.");
                return null;
            }

            return Create.MaterialResult(metric.GetType(), materialName, epdName, metric.IEvaluateEnvironmentalMetricValues(quantityValue, evaluationConfig));
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        [Description("Gets the resulting values for each phase of the provided EnvironmentalMetric given the provided quantityValue.\n" +
                     "The resulting values are computed based on provided config, defaulting to the values on the metric for each phase multiplied by the quantity value.\n" +
                     "Please be mindful that the unit of the quantityValue should match the QuantityType on the EnvironmentalProductDeclaration to which the metric belongs.")]
        [Input("metric", "The EnvironmentalMetric to get resulting values for. All phase values on the metric will be extracted and multiplied by the qunatityValue.")]
        [Input("quantityValue", "The quantity value to evaluate all metrics by. All metric properties will be multiplied by this value. Quantity should correspond to the QuantityType on the EPD.")]
        [Input("evaluationConfig", "Config controlling how the metrics should be evaluated, may contain additional parameters for the evaluation. If no config is provided the default evaluation mechanism is used which computes resulting phase values as metric value times applicable quantity.")]
        [Output("resultValues", "The resulting values for each phase.")]
        private static List<double> IEvaluateEnvironmentalMetricValues(this EnvironmentalMetric metric, double quantityValue, IEvaluationConfig evaluationConfig)
        {
            if (evaluationConfig == null)   //For case of null config, use default evaluation methodology of phase data value * quantity for each phase
                return EvaluateEnvironmentalMetricValues(metric, quantityValue);
            else
                return EvaluateEnvironmentalMetricValues(metric, quantityValue, evaluationConfig as dynamic);
        }

        /***************************************************/

        [Description("Default methodology for getting the resulting values for each phase of the provided EnvironmentalMetric given the provided quantityValue.\n" +
                     "The resulting values are computed as the values on the metric for each phase multiplied by the quantity value.\n" +
                     "Please be mindful that the unit of the quantityValue should match the QuantityType on the EnvironmentalProductDeclaration to which the metric belongs.")]
        [Input("metric", "The EnvironmentalMetric to get resulting values for. All phase values on the metric will be extracted and multiplied by the qunatityValue.")]
        [Input("quantityValue", "The quantity value to evaluate all metrics by. All metric properties will be multiplied by this value. Quantity should correspond to the QuantityType on the EPD.")]
        [Output("resultValues", "The resulting values for each phase.")]
        private static List<double> EvaluateEnvironmentalMetricValues(this EnvironmentalMetric metric, double quantityValue)
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

        [Description("IStructE Evaluation methodology for getting the resulting values for each phase of the provided EnvironmentalMetric given the provided quantityValue.\n" +
                     "Evaluation method only applicable for the two CLimateChangeTotal metric types - all other metrics are evaluated using the default mechanism.\n" +
                     "Method works for most phases works the same as default evaluation mechanism, with exception for the C1 and A5 phase where project totals are acounted for.")]
        [Input("metric", "The EnvironmentalMetric to get resulting values for. All phase values on the metric will be extracted and multiplied by the qunatityValue.")]
        [Input("quantityValue", "The quantity value to evaluate all metrics by. All metric properties will be multiplied by this value. Quantity should correspond to the QuantityType on the EPD.")]
        [Output("resultValues", "The resulting values for each phase.")]
        private static List<double> EvaluateEnvironmentalMetricValues(this EnvironmentalMetric metric, double quantityValue, IStructEEvaluationConfig evaluationConfig)
        {
            if (metric == null)
            {
                Base.Compute.RecordError($"Cannot evaluate a null {nameof(EnvironmentalMetric)}.");
                return null;
            }

            //Specific evaluation method using the config only applicable for evaluating climate change totals
            if (metric.MetricType != EnvironmentalMetrics.ClimateChangeTotal && metric.MetricType != EnvironmentalMetrics.ClimateChangeTotalNoBiogenic)
            {
                Base.Compute.RecordNote($"The {nameof(IStructEEvaluationConfig)} evaluation is only applicable for evaluating metrics of type {EnvironmentalMetrics.ClimateChangeTotal} and {EnvironmentalMetrics.ClimateChangeTotalNoBiogenic}." +
                                  $"All other metrics are evaluated based on standard evaluation procedure of phase times quantity for all phases.");
                return EvaluateEnvironmentalMetricValues(metric, quantityValue);
            }

            double weight = quantityValue;
            double weightFactor = (evaluationConfig.TotalWeight == 0 || evaluationConfig.TotalWeight < weight) ? 0 : weight / evaluationConfig.TotalWeight;

            double a1 = metric.A1 * quantityValue;
            double a2 = metric.A2 * quantityValue;
            double a3 = metric.A3 * quantityValue;
            double a1toa3 = metric.A1toA3 * quantityValue;
            double a4 = metric.A4 * quantityValue;
            double a5 = metric.A5 * quantityValue + evaluationConfig.ProjectCost * evaluationConfig.A5CarbonFactor * weightFactor;  //Additional project factor for A5

            double b1 = metric.B1 * quantityValue;
            double b2 = metric.B2 * quantityValue;
            double b3 = metric.B3 * quantityValue;
            double b4 = metric.B4 * quantityValue;
            double b5 = metric.B5 * quantityValue;
            double b6 = metric.B6 * quantityValue;
            double b7 = metric.B7 * quantityValue;
            double b1tob7 = metric.B1toB7 * quantityValue;

            double c1 = weightFactor * evaluationConfig.FloorArea * evaluationConfig.C1CarbonFactor;         //C1 evaluated based on project level values
            double c2 = metric.C2 * quantityValue;
            double c3 = metric.C3 * quantityValue;
            double c4 = metric.C4 * quantityValue;

            double c1toc4 = c1 + c2 + c3 + c4;  //C1toC4 taken as sum of phases
            if (double.IsNaN(c1toc4))   //If NaN, instead take based on total on metric times quantity value
                c1toc4 = metric.C1toC4 * quantityValue;

            double d = metric.D * quantityValue;

            return new List<double> { a1, a2, a3, a1toa3, a4, a5, b1, b2, b3, b4, b5, b6, b7, b1tob7, c1, c2, c3, c4, c1toc4, d };
        }

        /***************************************************/
        /**** Private Methods - Fallback                ****/
        /***************************************************/

        [Description("Fallback method for unkown config provided, raising warning and calling the defautl evaluation mechanism. Please note that this method is not triggered for null config, which also calls default mechism, but without warning.")]
        [Input("metric", "The EnvironmentalMetric to get resulting values for. All phase values on the metric will be extracted and multiplied by the qunatityValue.")]
        [Input("quantityValue", "The quantity value to evaluate all metrics by. All metric properties will be multiplied by this value. Quantity should correspond to the QuantityType on the EPD.")]
        [Output("resultValues", "The resulting values for each phase.")]
        private static List<double> EvaluateEnvironmentalMetricValues(this EnvironmentalMetric metric, double quantityValue, IEvaluationConfig evaluationConfig)
        {
            BH.Engine.Base.Compute.RecordWarning($"No evaluation method implemented for evaluation config of type {evaluationConfig}. Results returned are based on default evaluation method of phase values times quantity.");

            return EvaluateEnvironmentalMetricValues(metric, quantityValue);
        }

        /***************************************************/
    }
}

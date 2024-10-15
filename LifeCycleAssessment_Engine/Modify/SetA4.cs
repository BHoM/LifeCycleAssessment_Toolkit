/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2024, the respective contributors. All rights reserved.
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
using BH.oM.LifeCycleAssessment.MaterialFragments;
using BH.oM.Quantities.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace BH.Engine.LifeCycleAssessment
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Method takes an EnvironmentalProductDeclaration or an CalculatedMaterialLifeCycleEnvironmentalImpactFactors and sets the A4 value to selected metrics. A new CalculatedMaterialLifeCycleEnvironmentalImpactFactors is returned with the modified metrics assigned.")]
        [Input("metricsProvider", "EPD or impact factors object to set A4 values to.")]
        [Input("a4Value", "The value for the A4 stage to be assigned to relevant metrics on the Metrics provider.")]
        [Input("metricType", "Metric filter to assign A4 value to. For default undefined case, ClimateChangeFossilMetric, ClimateChangeTotalMetric and ClimateChangeTotalNoBiogenicMetric will have their values updated to the provided value.")]
        [Output("lcaImpactFactors", "CalculatedMaterialLifeCycleEnvironmentalImpactFactors with metrics copied from the provided metricsProvider, with a4 value updated to the provided A4 value.")]
        public static CalculatedMaterialLifeCycleEnvironmentalImpactFactors SetA4(this IEnvironmentalMetricsProvider metricsProvider, double a4Value, EnvironmentalMetrics metricType = EnvironmentalMetrics.Undefined)
        {
            if (metricsProvider == null)
            {
                BH.Engine.Base.Compute.RecordError("Requires a valid metricsProvider to be able to set A4 values.");
                return null;
            }


            List<EnvironmentalMetrics> applicableMetricsTypes;

            if (metricType == EnvironmentalMetrics.Undefined)
                applicableMetricsTypes = new List<EnvironmentalMetrics> { EnvironmentalMetrics.ClimateChangeFossil, EnvironmentalMetrics.ClimateChangeTotal, EnvironmentalMetrics.ClimateChangeTotalNoBiogenic };
            else
                applicableMetricsTypes = new List<EnvironmentalMetrics> { metricType };


            CalculatedMaterialLifeCycleEnvironmentalImpactFactors impactFactors = new CalculatedMaterialLifeCycleEnvironmentalImpactFactors()
            {
                Name = metricsProvider.Name,
                CustomData = metricsProvider.CustomData,
                Fragments = metricsProvider.Fragments,
                QuantityType = metricsProvider.QuantityType,
                Tags = metricsProvider.Tags,
                Type = metricsProvider.Type
            };

            if (impactFactors.QuantityType != QuantityType.Mass)
                BH.Engine.Base.Compute.RecordWarning($"Please note that the {nameof(impactFactors.QuantityType)} on the {nameof(CalculatedMaterialLifeCycleEnvironmentalImpactFactors)} is set to {impactFactors.QuantityType} rather than {nameof(QuantityType.Mass)}. Please ensure that the provided A4 value is correctly scaled to acount for this.");


            List<EnvironmentalMetric> metrics = new List<EnvironmentalMetric>();

            foreach (EnvironmentalMetric metric in metricsProvider.EnvironmentalMetrics)
            {
                if (applicableMetricsTypes.Any(t => metric.MetricType == t))
                {
                    List<double> phaseDataValues = metric.IPhaseDataValues();
                    phaseDataValues[4] = a4Value;   //A4 corresponds to index 4 in the list (A1, A2, A3, A1toA3, A4....)

                    EnvironmentalMetric newMetric = Create.EnvironmentalMetric(metric.GetType(), metric.Name, phaseDataValues); //Create a new metric with updated A4 value
                    metrics.Add(newMetric);
                }
                else
                    metrics.Add(metric);
            }

            impactFactors.EnvironmentalMetrics = metrics;
            
            return impactFactors;
        }

        /***************************************************/
    }
}

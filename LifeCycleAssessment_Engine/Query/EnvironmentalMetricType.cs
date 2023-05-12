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

using BH.oM.Base;
using BH.oM.Base.Attributes;
using BH.oM.LifeCycleAssessment;
using BH.oM.LifeCycleAssessment.MaterialFragments;
using BH.oM.LifeCycleAssessment.Results;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace BH.Engine.LifeCycleAssessment
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods - Interface                ****/
        /***************************************************/

        [Description("Gets the metric type corresponding to the provided ILifeCycleAssessmentPhaseData item.")]
        [Input("lcaItem", "ILifeCycleAssessmentPhaseData to get the EnvironmentalMetric type from.")]
        [Output("metricType", "EnvironmentalMetrics enum corresponding to the provided item.")]
        public static EnvironmentalMetrics IEnvironmentalMetricType(this  ILifeCycleAssessmentPhaseData lcaItem)
        {
            if (lcaItem == null)
            {
                Base.Compute.RecordWarning($"Cannot query {nameof(EnvironmentalMetricType)} of a null {nameof(ILifeCycleAssessmentPhaseData)}. {EnvironmentalMetrics.Undefined} returned.");
                return EnvironmentalMetrics.Undefined;
            }
            return EnvironmentalMetricType(lcaItem as dynamic);
        }

        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Gets the metric type corresponding to the provided ILifeCycleAssessmentPhaseData item.")]
        [Input("lcaItem", "ILifeCycleAssessmentPhaseData to get the EnvironmentalMetric type from.")]
        [Output("metricType", "EnvironmentalMetrics enum corresponding to the provided item.")]
        public static EnvironmentalMetrics EnvironmentalMetricType(this GlobalWarmingPotentialMetrics lcaItem)
        {
            return EnvironmentalMetrics.GlobalWarmingPotential;
        }

        /***************************************************/

        [Description("Gets the metric type corresponding to the provided ILifeCycleAssessmentPhaseData item.")]
        [Input("lcaItem", "ILifeCycleAssessmentPhaseData to get the EnvironmentalMetric type from.")]
        [Output("metricType", "EnvironmentalMetrics enum corresponding to the provided item.")]
        public static EnvironmentalMetrics EnvironmentalMetricType(this GlobalWarmingPotentialElementResult lcaItem)
        {
            return EnvironmentalMetrics.GlobalWarmingPotential;
        }

        /***************************************************/

        [Description("Gets the metric type corresponding to the provided ILifeCycleAssessmentPhaseData item.")]
        [Input("lcaItem", "ILifeCycleAssessmentPhaseData to get the EnvironmentalMetric type from.")]
        [Output("metricType", "EnvironmentalMetrics enum corresponding to the provided item.")]
        public static EnvironmentalMetrics EnvironmentalMetricType(this GlobalWarmingPotentialMaterialResult lcaItem)
        {
            return EnvironmentalMetrics.GlobalWarmingPotential;
        }

        /***************************************************/

        [Description("Gets the metric type corresponding to the provided ILifeCycleAssessmentPhaseData item.")]
        [Input("lcaItem", "ILifeCycleAssessmentPhaseData to get the EnvironmentalMetric type from.")]
        [Output("metricType", "EnvironmentalMetrics enum corresponding to the provided item.")]
        public static EnvironmentalMetrics EnvironmentalMetricType(this AcidificationPotentialMetrics lcaItem)
        {
            return EnvironmentalMetrics.AcidificationPotential;
        }

        /***************************************************/

        [Description("Gets the metric type corresponding to the provided ILifeCycleAssessmentPhaseData item.")]
        [Input("lcaItem", "ILifeCycleAssessmentPhaseData to get the EnvironmentalMetric type from.")]
        [Output("metricType", "EnvironmentalMetrics enum corresponding to the provided item.")]
        public static EnvironmentalMetrics EnvironmentalMetricType(this AcidificationPotentialElementResult lcaItem)
        {
            return EnvironmentalMetrics.AcidificationPotential;
        }

        /***************************************************/

        [Description("Gets the metric type corresponding to the provided ILifeCycleAssessmentPhaseData item.")]
        [Input("lcaItem", "ILifeCycleAssessmentPhaseData to get the EnvironmentalMetric type from.")]
        [Output("metricType", "EnvironmentalMetrics enum corresponding to the provided item.")]
        public static EnvironmentalMetrics EnvironmentalMetricType(this AcidificationPotentialMaterialResult lcaItem)
        {
            return EnvironmentalMetrics.AcidificationPotential;
        }

        /***************************************************/
        /**** Private Methods - Fallback                ****/
        /***************************************************/

        [Description("Gets the metric type corresponding to the provided ILifeCycleAssessmentPhaseData item.")]
        [Input("lcaItem", "ILifeCycleAssessmentPhaseData to get the EnvironmentalMetric type from.")]
        [Output("metricType", "EnvironmentalMetrics enum corresponding to the provided item.")]
        public static EnvironmentalMetrics EnvironmentalMetricType(this ILifeCycleAssessmentPhaseData lcaItem)
        {
            Base.Compute.RecordWarning($"{nameof(EnvironmentalMetricType)} not implemented for {nameof(ILifeCycleAssessmentPhaseData)} of type {(lcaItem?.GetType()?.Name ?? "null")}.");
            return EnvironmentalMetrics.Undefined;
        }

        /***************************************************/
    }
}

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

using BH.oM.Base;
using BH.oM.Base.Attributes;
using BH.oM.LifeCycleAssessment.MaterialFragments;
using BH.oM.LifeCycleAssessment.MaterialFragments.Transport;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace BH.Engine.LifeCycleAssessment
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Gets all EnvironmentalMetrics stored on the IEnvironmentalMetricsProvider. Please note that the quantity type relating to these metrics in not stored on the metric, hence care needs to be taken if metrics are compared with metrics extracted from another metrics provider.")]
        [Input("metricsProvider", "The IEnvironmentalMetricsProvider to extract metrics from.")]
        [Output("metrics", "The list of EnvironmentalMetrics stored on the metrics provider.")]
        public static List<EnvironmentalMetric> Metrics(this IBasicEnvironmentalMetricsProvider metricsProvider)
        {
            if (metricsProvider == null)
            {
                BH.Engine.Base.Compute.RecordError($"Unable to extract {nameof(EnvironmentalMetric)}s from a null {nameof(IEnvironmentalMetricsProvider)}");
                return new List<EnvironmentalMetric>();
            }
            return metricsProvider.EnvironmentalMetrics;
        }

        /***************************************************/

        [Description("Gets all EnvironmentalMetrics stored on the IEnvironmentalMetricsProvider. Please note that the quantity type relating to these metrics in not stored on the metric, hence care needs to be taken if metrics are compared with metrics extracted from another metrics provider.")]
        [Input("metricsProvider", "The IEnvironmentalMetricsProvider to extract metrics from.")]
        [Output("metrics", "The list of EnvironmentalMetrics stored on the metrics provider.")]
        public static List<EnvironmentalMetric> Metrics(this FullTransportScenario metricsProvider)
        {
            if (metricsProvider == null)
            {
                BH.Engine.Base.Compute.RecordError($"Unable to extract {nameof(EnvironmentalMetric)}s from a null {nameof(IEnvironmentalMetricsProvider)}");
                return new List<EnvironmentalMetric>();
            }
            return metricsProvider.EnvironmentalMetrics;
        }

        /***************************************************/


        [Description("Gets all EnvironmentalMetrics stored on the IEnvironmentalMetricsProvider. Please note that the quantity type relating to these metrics in not stored on the metric, hence care needs to be taken if metrics are compared with metrics extracted from another metrics provider.")]
        [Input("metricsProvider", "The IEnvironmentalMetricsProvider to extract metrics from.")]
        [Output("metrics", "The list of EnvironmentalMetrics stored on the metrics provider.")]
        public static List<EnvironmentalMetric> Metrics(this DistanceTransportModeScenario metricsProvider)
        {
            if (metricsProvider == null)
            {
                BH.Engine.Base.Compute.RecordError($"Unable to extract {nameof(EnvironmentalMetric)}s from a null {nameof(IEnvironmentalMetricsProvider)}");
                return new List<EnvironmentalMetric>();
            }
            return metricsProvider.SingleTransportModeImpacts.SelectMany(x => x.VehicleEmissions.EnvironmentalMetrics).ToList(); ;
        }

        /***************************************************/

        [Description("Gets all EnvironmentalMetrics stored on the IEnvironmentalMetricsProvider. Please note that the quantity type relating to these metrics in not stored on the metric, hence care needs to be taken if metrics are compared with metrics extracted from another metrics provider.")]
        [Input("metricsProvider", "The IEnvironmentalMetricsProvider to extract metrics from.")]
        [Output("metrics", "The list of EnvironmentalMetrics stored on the metrics provider.")]
        public static List<EnvironmentalMetric> Metrics(this CombinedLifeCycleAssessmentFactors metricsProvider)
        {
            if (metricsProvider == null)
            {
                BH.Engine.Base.Compute.RecordError($"Unable to extract {nameof(EnvironmentalMetric)}s from a null {nameof(IEnvironmentalMetricsProvider)}");
                return new List<EnvironmentalMetric>();
            }

            List<EnvironmentalMetric> metrics = new List<EnvironmentalMetric>();
            if (metricsProvider.EnvironmentalProductDeclaration != null)
                metrics.AddRange(metricsProvider.EnvironmentalProductDeclaration.EnvironmentalMetrics);

            if (metricsProvider.TransportFactors != null)
                metrics.AddRange(metricsProvider.TransportFactors.IMetrics());

            return metrics;
        }

        /***************************************************/
        /**** Public Methods - Interface                ****/
        /***************************************************/

        [Description("Gets all EnvironmentalMetrics stored on the IEnvironmentalMetricsProvider. Please note that the quantity type relating to these metrics in not stored on the metric, hence care needs to be taken if metrics are compared with metrics extracted from another metrics provider.")]
        [Input("metricsProvider", "The IEnvironmentalMetricsProvider to extract metrics from.")]
        [Output("metrics", "The list of EnvironmentalMetrics stored on the metrics provider.")]
        public static List<EnvironmentalMetric> IMetrics(this IEnvironmentalMetricsProvider metricsProvider)
        {
            if (metricsProvider == null)
            {
                BH.Engine.Base.Compute.RecordError($"Unable to extract {nameof(EnvironmentalMetric)}s from a null {nameof(IEnvironmentalMetricsProvider)}");
                return new List<EnvironmentalMetric>();
            }
            return Metrics(metricsProvider as dynamic);
        }

        /***************************************************/
        /**** Private Methods - Fallback                ****/
        /***************************************************/

        [Description("Gets all EnvironmentalMetrics stored on the IEnvironmentalMetricsProvider. Please note that the quantity type relating to these metrics in not stored on the metric, hence care needs to be taken if metrics are compared with metrics extracted from another metrics provider.")]
        [Input("metricsProvider", "The IEnvironmentalMetricsProvider to extract metrics from.")]
        [Output("metrics", "The list of EnvironmentalMetrics stored on the metrics provider.")]
        public static List<EnvironmentalMetric> Metrics(this IEnvironmentalMetricsProvider metricsProvider)
        {
            BH.Engine.Base.Compute.RecordError($"Unable to extract {nameof(EnvironmentalMetric)}s from {nameof(IEnvironmentalMetricsProvider)}s of type {metricsProvider.GetType().Name}.");
            return new List<EnvironmentalMetric>();
        }

        /***************************************************/
    }
}

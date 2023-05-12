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
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Filters out the metrics on the EPD based on the provided metric types. If no types are provided, then all metrics on the EPD are returned.")]
        [Input("lcaItems", "The EnvironmentalProductDeclaration to get the EnvironmentalMetrics from.")]
        [Input("metricFilter", "Filter for the provided EnvironmentalProductDeclaration for selecting one or more of the provided metrics. This method also accepts multiple metric types simultaneously. If nothing is provided then no filtering is assumed, i.e. all metrics on the EPD are returned.")]
        [Output("materics", "The metrics on the EnvironmentalProductDeclaration corresponding to the provided filter, or all metrics on the epd if no metricType filters are provided.")]
        public static List<ILifeCycleAssessmentPhaseData> FilterMetrics(this IEnumerable<ILifeCycleAssessmentPhaseData> lcaItems, List<EnvironmentalMetrics> metricFilter)
        {
            if (lcaItems == null || metricFilter == null)
            {
                Base.Compute.RecordError("Cannot filter due to null inputs.");
                return null;
            }

            var metricLookup = lcaItems.ToLookup(x => x.IEnvironmentalMetricType());
            List<ILifeCycleAssessmentPhaseData> metrics = new List<ILifeCycleAssessmentPhaseData>();
            foreach (EnvironmentalMetrics type in metricFilter)
            {
                IEnumerable<ILifeCycleAssessmentPhaseData> metric = metricLookup[type];
                if (metric.Any())
                    metrics.AddRange(metric);
                else
                    Base.Compute.RecordWarning($"Provided items does not contain a {nameof(ILifeCycleAssessmentPhaseData)} corresponding to metric of type {type}.");
            }

            return metrics;
        }

        /***************************************************/
    }
}

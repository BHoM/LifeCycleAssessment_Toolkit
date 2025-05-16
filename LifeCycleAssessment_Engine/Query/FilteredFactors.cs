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

        [PreviousVersion("8.2", "BH.Engine.LifeCycleAssessment.Query.FilteredMetrics(BH.oM.LifeCycleAssessment.MaterialFragments.EnvironmentalProductDeclaration, System.Collections.Generic.List<BH.oM.LifeCycleAssessment.EnvironmentalMetrics>)")]
        [Description("Filters out the metrics on the EPD based on the provided metric types. If no types are provided, then all metrics on the EPD are returned.")]
        [Input("epd", "The EnvironmentalProductDeclaration to get the EnvironmentalMetrics from.")]
        [Input("metricFilter", "Filter for the provided EnvironmentalProductDeclaration for selecting one or more of the provided metrics. This method also accepts multiple metric types simultaneously. If nothing is provided then no filtering is assumed, i.e. all metrics on the EPD are returned.")]
        [Output("materics", "The metrics on the EnvironmentalProductDeclaration corresponding to the provided filter, or all metrics on the epd if no metricType filters are provided.")]
        public static List<IEnvironmentalMetricFactors> FilteredFactors(this IBaseLevelEnvironalmentalFactorsProvider epd, List<MetricType> metricFilter = null)
        {
            if(epd == null) 
            {
                BH.Engine.Base.Compute.RecordError($"Cannot extract null metrics from a null {nameof(IBaseLevelEnvironalmentalFactorsProvider)}.");
                return new List<IEnvironmentalMetricFactors>();
            }

            return epd.EnvironmentalFactors.FilterIndicators(metricFilter);
        }

        /***************************************************/
    }
}



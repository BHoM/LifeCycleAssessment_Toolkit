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

using System.ComponentModel;
using BH.oM.Base.Attributes;
using BH.Engine.Matter;
using BH.oM.Dimensional;
using BH.oM.LifeCycleAssessment.MaterialFragments;
using System.Collections.Generic;
using System.Linq;
using BH.oM.LifeCycleAssessment;

namespace BH.Engine.LifeCycleAssessment
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Query the Environmental Product Declarations from any IElementM with a MaterialComposition composed of IEPD materials.")]
        [Input("elementM", "A IElementM from which to query the EPD.")]
        [Output("environmentalMetric", "An Environmental Metric is used to store data regarding the environmental impacts of a given Environmental Product Declaration. \n"
        + "An EPD can host multiple EnvironmentalMetrics to describe the overall impact which will be used in any LCA calculation.")]
        public static List<List<EnvironmentalMetric>> ElementEnvironmentalMetrics(this IElementM elementM)
        {
            List<IEnvironmentalMetricsProvider> epd = elementM.ElementEpds();

            if (epd == null || epd.Count == 0)
                return new List<List<EnvironmentalMetric>>();

            List<List<EnvironmentalMetric>> metric = epd.Select(x => x.EnvironmentalMetrics).ToList();

            if (metric.Count() <= 0)
            {
                BH.Engine.Base.Compute.RecordError("No Environmental Metrics could be found.");
            }

            return metric;
        }

        /***************************************************/
    }
}



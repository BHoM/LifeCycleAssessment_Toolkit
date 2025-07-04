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
using BH.oM.Base;
using BH.oM.Base.Attributes;
using BH.oM.LifeCycleAssessment;
using BH.oM.LifeCycleAssessment.MaterialFragments;
using BH.oM.LifeCycleAssessment.Results;
using System;
using System.Collections.Concurrent;
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

        [PreviousVersion("8.2", "BH.Engine.LifeCycleAssessment.Query.TotalMaterialBreakdown(System.Collections.Generic.IEnumerable<BH.oM.LifeCycleAssessment.Results.IElementResult<BH.oM.LifeCycleAssessment.Results.MaterialResult>>)")]
        [Description("Gets total MaterialResults for all provided element results grouped by MaterialName, EPDName and Metric, and returns a single MaterialResult for each group containing the total evaluated.")]
        [Input("elementResults", "The element results to extract the material breakdown from.")]
        [Input("onlyIncludeIfAllAvailable", "If true, only sums up values for a particular module if it is available on all items provided. If false, sum for modules where only part of the data is available is added as well.")]
        [Output("materialResults", "Material results with the total quantity per materal type.")]
        public static List<MaterialResult> TotalMaterialBreakdown(this IEnumerable<IElementResult<MaterialResult>> elementResults, bool onlyIncludeIfAllAvailable = true)
        {
            if (elementResults == null || !elementResults.Any())
                return new List<MaterialResult>();

            return elementResults.SelectMany(x => x.MaterialResults).TotalMaterialBreakdown(onlyIncludeIfAllAvailable);
        }

        /***************************************************/

        [PreviousVersion("8.2", "BH.Engine.LifeCycleAssessment.Query.TotalMaterialBreakdown(System.Collections.Generic.IEnumerable<BH.oM.LifeCycleAssessment.Results.MaterialResult>)")]
        [Description("Gets total MaterialResults from list of individual material results grouped by MaterialName, EPDName and Type, and returns a single MaterialResult for each group containing the total evaluated.")]
        [Input("materialResults", "The individual MaterialResult results to extract the total from.")]
        [Input("onlyIncludeIfAllAvailable", "If true, only sums up values for a particular module if it is available on all items provided. If false, sum for modules where only part of the data is available is added as well.")]
        [Output("materialResults", "Material results with the total quantity per materal type.")]
        public static List<MaterialResult> TotalMaterialBreakdown(this IEnumerable<MaterialResult> materialResults, bool onlyIncludeIfAllAvailable = true)
        {
            if (materialResults == null || !materialResults.Any())
                return new List<MaterialResult>();

            List<MaterialResult> breakDown = new List<MaterialResult>();

            foreach (var group in materialResults.GroupBy(x => x.GetType()))
            {
                breakDown.AddRange(MaterialBreakdown(group, onlyIncludeIfAllAvailable));
            }

            return breakDown;
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        [Description("Gets total MaterialResults from list of individual material results grouped by MaterialName and EPDName, and returns a single MaterialResult for each group containing the total evaluated for each module.")]
        [Input("materialResults", "The individual MaterialResult results to extract the total from All assumed to be of the same type.")]
        [Input("onlyIncludeIfAllAvailable", "If true, only sums up values for a particular module if it is available on all items provided. If false, sum for modules where only part of the data is available is added as well.")]
        [Output("materialResults", "Material results with the total quantity per materal type.")]
        private static List<MaterialResult> MaterialBreakdown(IEnumerable<MaterialResult> materialResults, bool onlyIncludeIfAllAvailable)
        {
            List<MaterialResult> breakDown = new List<MaterialResult>();
            ////Get current type being evaluated
            Type type = materialResults.First().GetType();
            //Group results by EPD and Material name
            foreach (var group in materialResults.GroupBy(x => new { x.MaterialName, x.EnvironmentalProductDeclarationName }))
            {
                //Compute the total breakdown as sum value for each module
                Dictionary<Module, double> sumValues = group.ToList().SumModuleDataValues(onlyIncludeIfAllAvailable);
                //Call create and add to list
                breakDown.Add(Create.MaterialResult(group.Key.MaterialName, group.Key.EnvironmentalProductDeclarationName, group.First().IMetricType(), sumValues));

            }
            return breakDown;
        }

        /***************************************************/

    }
}




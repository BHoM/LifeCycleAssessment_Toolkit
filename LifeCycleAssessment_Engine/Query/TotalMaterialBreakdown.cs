/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2022, the respective contributors. All rights reserved.
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

        [Description("Gets total MaterialResults for all provided element results grouped by MaterialName, EPDName and Metric, and returns a single MaterialResult for each group containing the total evaluated.")]
        [Input("elementResults", "The element results to extract the material breakdown from.")]
        [Output("materialResults", "Material results with the total quantity per materal type.")]
        public static List<MaterialResult> TotalMaterialBreakdown(this IEnumerable<ElementResult> elementResults)
        {
            if (elementResults == null || !elementResults.Any())
                return new List<MaterialResult>();

            return elementResults.SelectMany(x => x.MaterialResults).TotalMaterialBreakdown();
        }

        /***************************************************/

        [Description("Gets total MaterialResults from list of individual material results grouped by MaterialName, EPDName and Metric, and returns a single MaterialResult for each group containing the total evaluated.")]
        [Input("materialResults", "The individual MaterialResult results to extract the total from.")]
        [Output("materialResults", "Material results with the total quantity per materal type.")]
        public static List<MaterialResult> TotalMaterialBreakdown(this IEnumerable<MaterialResult> materialResults)
        {
            if (materialResults == null || !materialResults.Any())
                return new List<MaterialResult>();

            List<MaterialResult> result = new List<MaterialResult>();

            foreach (var group in materialResults.GroupBy(x => x.MaterialName + x.EPDName + x.Metric))
            {
                List<MaterialResult> resultsOfType = group.ToList();
                List<LifeCycleAssessmentPhases> evaluatedPhases = resultsOfType[0].Phases.ToList();
                bool missMatchedPhases = false;

                for (int i = 1; i < resultsOfType.Count; i++)
                {
                    foreach (LifeCycleAssessmentPhases phase in resultsOfType[i].Phases)
                    {
                        if (!evaluatedPhases.Contains(phase))
                        {
                            missMatchedPhases = true;
                            evaluatedPhases.Add(phase);
                        }
                    }
                }
                if (missMatchedPhases)
                    Base.Compute.RecordWarning("Missmatch in phases between same material on different elements. Please check the results.");

                MaterialResult first = resultsOfType[0];
                result.Add(new MaterialResult(first.MaterialName, first.EPDName, evaluatedPhases, resultsOfType.Sum(x => x.Quantity), first.Metric));
            }
            return result;
        }

        /***************************************************/
    }
}

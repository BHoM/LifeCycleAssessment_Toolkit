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
using System.Reflection;

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

            foreach (var group in materialResults.GroupBy(x => x.MaterialName + x.EnvironmentalProductDeclarationName + x.Metric))
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
                result.Add(new MaterialResult(first.MaterialName, first.EnvironmentalProductDeclarationName, evaluatedPhases, resultsOfType.Sum(x => x.Quantity), first.Metric));
            }
            return result;
        }

        /***************************************************/

        [Description("Gets total MaterialResults for all provided element results grouped by MaterialName, EPDName and Metric, and returns a single MaterialResult for each group containing the total evaluated.")]
        [Input("elementResults", "The element results to extract the material breakdown from.")]
        [Output("materialResults", "Material results with the total quantity per materal type.")]
        public static List<MaterialResult2> TotalMaterialBreakdown(this IEnumerable<IElementResult<MaterialResult2>> elementResults)
        {
            if (elementResults == null || !elementResults.Any())
                return new List<MaterialResult2>();

            return elementResults.SelectMany(x => x.MaterialResults).TotalMaterialBreakdown();
        }

        /***************************************************/

        [Description("Gets total MaterialResults from list of individual material results grouped by MaterialName, EPDName and Type, and returns a single MaterialResult for each group containing the total evaluated.")]
        [Input("materialResults", "The individual MaterialResult results to extract the total from.")]
        [Output("materialResults", "Material results with the total quantity per materal type.")]
        public static List<MaterialResult2> TotalMaterialBreakdown(this IEnumerable<MaterialResult2> materialResults)
        {
            if (materialResults == null || !materialResults.Any())
                return new List<MaterialResult2>();

            List<MaterialResult2> breakDown = new List<MaterialResult2>();

            foreach (var group in materialResults.GroupBy(x => x.GetType()))
            {
                breakDown.AddRange(MaterialBreakdown(group));
            }

            return breakDown;
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        [Description("Gets total MaterialResults from list of individual material results grouped by MaterialName and EPDName, and returns a single MaterialResult for each group containing the total evaluated for each phase.")]
        [Input("materialResults", "The individual MaterialResult results to extract the total from All assumed to be of the same type.")]
        [Output("materialResults", "Material results with the total quantity per materal type.")]
        private static List<MaterialResult2> MaterialBreakdown(IEnumerable<MaterialResult2> materialResults)
        {
            List<MaterialResult2> breakDown = new List<MaterialResult2>();
            //Get MaterialResultConstructor of the same type as currently being evaluated
            Func<object[], MaterialResult2> cst = MaterialResultConstructor(materialResults.First().GetType());

            //Group results by EPD and Material name
            foreach (var group in materialResults.GroupBy(x => new { x.MaterialName, x.EnvironmentalProductDeclarationName }))
            {
                //First parameters of constructor is material name and EPD name
                List<object> parameters = new List<object> { group.Key.MaterialName, group.Key.EnvironmentalProductDeclarationName };

                //Compute the sume value for each phase and add to the list
                parameters.AddRange(group.ToList().SumPhaseDataValues().Cast<object>());
                //Call constructor and add to output list
                breakDown.Add(cst(parameters.ToArray()));
            }
            return breakDown;
        }

        /***************************************************/

    }
}


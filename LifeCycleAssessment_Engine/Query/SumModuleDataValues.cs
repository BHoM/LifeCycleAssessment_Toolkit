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
using BH.oM.LifeCycleAssessment.Interfaces;
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

        [PreviousVersion("8.2", "BH.Engine.LifeCycleAssessment.Query.SumPhaseDataValues(System.Collections.Generic.IReadOnlyList<BH.oM.LifeCycleAssessment.ILifeCycleAssessmentPhaseData>)")]
        [Description("Gets a list of doubles corresponding to the sum of values for each property of the provided ILifeCycleAssessmentPhaseData, i.e. the first value will be the sum of A1 for all the provided ILifeCycleAssessmentPhaseDatas.")]
        [Input("results", "List of ILifeCycleAssessmentPhaseData to get the sum data from.")]
        [Input("onlyIncludeIfAllAvailable", "If true, only sums up values for a particular module if it is available on all items provided. If false, sum for modules where only part of the data is available is added as well.")]
        [Output("values", "The values of the summed up material results as dictionary with the module as keys and resulting summed value for the module as value.")]
        public static Dictionary<Module, double> SumModuleDataValues(this IReadOnlyList<ILifeCycleAssessmentModuleData<IDictionary<Module, double>>> results, bool onlyIncludeIfAllAvailable = true)
        {
            if (results.IsNullOrEmpty())
                return new Dictionary<Module, double>();

            ILifeCycleAssessmentModuleData<IDictionary<Module, double>> first = results[0];
            if (results.Count > 1)
            {
                //If more than one value provided, check that all are of the same type
                Type firstType = first.GetType();
                if (results.Skip(1).Any(x => x.GetType() != firstType))
                {
                    Base.Compute.RecordError($"Only able to sum {nameof(ILifeCycleAssessmentModuleData<IDictionary<Module, double>>)} of the same type.");
                    return new Dictionary<Module, double>();
                }
            }

            Dictionary<Module, double> totalResults = new Dictionary<Module, double>();

            if (onlyIncludeIfAllAvailable)
            {
                //Get modules existing in all results
                List<Module> modules = results.SelectMany(x => x.Indicators.Keys).Distinct().ToList();
                foreach (var result in results)
                {
                    modules = modules.Intersect(result.Indicators.Keys).ToList();
                }

                //Only include total for modules defined for all parts
                foreach (var module in modules.Distinct())
                {
                    totalResults[module] = results.Sum(x => x.Indicators[module]);
                }
                return totalResults;
            }
            else
            {
                foreach (var result in results)
                {
                    foreach (var item in result.Indicators)
                    {
                        if(totalResults.ContainsKey(item.Key))
                            totalResults[item.Key] += item.Value;   //If already added, add to value
                        else
                            totalResults[item.Key] = item.Value;    //If not previously added, set

                    }
                }
            }

            return totalResults;
        }

        /***************************************************/
    }
}



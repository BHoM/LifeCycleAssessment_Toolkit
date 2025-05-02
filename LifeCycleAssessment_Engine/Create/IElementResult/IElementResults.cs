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
using BH.oM.Base.Attributes;
using BH.oM.LifeCycleAssessment;
using BH.oM.LifeCycleAssessment.MaterialFragments;
using BH.oM.LifeCycleAssessment.Results;
using BH.oM.LifeCycleAssessment.Results.MetricsValues;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace BH.Engine.LifeCycleAssessment
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Creates a list of ElementResults from a set of Identifier parameters and a list of MaterialResults. Material results are first grouped by type, and a single ElementResult is created for each type.")]
        [InputFromProperty("objectId")]
        [InputFromProperty("scope")]
        [InputFromProperty("category")]
        [Input("materialResults", "Material results used to create the element result. Results will first be grouped by type, and a single element result created per type. The phase values of the element result will be the sum of the MaterialResult values.")]
        [Output("results", "Created element results")]
        public static List<IElementResult2> IElementResults(IComparable objectId, ScopeType scope, ObjectCategory category, IEnumerable<IMaterialResult> materialResults)
        {
            if (materialResults.IsNullOrEmpty())
                return new List<IElementResult2>();

            List<IElementResult2> elementResults = new List<IElementResult2>();

            //Group all of the provided MaterialResult by their type
            foreach (var group in materialResults.GroupBy(x => x.GetType()))
            {
                //Create a single element result correpsonding to the group
                elementResults.Add(Create.ElementResult(group.First() as dynamic, group, objectId, scope, category));
            }
            return elementResults;
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        [Description("Creates an ElementResult<T> with a type matching that of the provided MaterialResult. Element result values are computed as the sum of the values on the material results.")]
        [Input("first", "First material result in the list. Provided to enable dynamic casting of the material results to their concrete type.")]
        [Input("materialResults", "The list of material results to evaluate. All should be of the same type T (same as first). The returned element result will have its result values computed as the sum of the phase values for all provided material results.")]
        [InputFromProperty("objectId")]
        [InputFromProperty("scope")]
        [InputFromProperty("category")]
        [Output("result", "Created ElementResult<T> from the provided MaterialResults.")]
        private static ElementResult2<T> ElementResult<T>(MaterialResult<T> first, IEnumerable<IMaterialResult> materialResults, IComparable objectId, ScopeType scope, ObjectCategory category) where T : IMetricValue, new()
        {
            //Cast the material results to the actual type
            List<MaterialResult<T>> castResults = materialResults.Cast<MaterialResult<T>>().ToList();

            Dictionary<LifeCycleAssessmentModule, T> totalMetrics = new Dictionary<LifeCycleAssessmentModule, T>();

            //Get modules existing in all results
            List<LifeCycleAssessmentModule> allModules = castResults.SelectMany(x => x.Metrics.Keys).Distinct().ToList();
            List<LifeCycleAssessmentModule> modules = allModules.ToList();

            foreach (var matResult in castResults)  
            {
                modules = modules.Intersect(matResult.Metrics.Keys).ToList();
            }

            //Only include total for modules defined for all parts
            foreach (var module in modules.Distinct())
            {
                totalMetrics[module] = new T { Value = castResults.Select(x => x.Metrics[module].Value).Sum() };
            }

            var nonCombinedModules = allModules.Except(modules).ToList();
            if (nonCombinedModules.Count > 0)
                BH.Engine.Base.Compute.RecordNote($"MaterialResults that make up ElementResult of type {typeof(T).Name} contains modules not present in all material results. These are ommited when creating the Combined results for the element. The information about these modules can be found on the {nameof(BH.oM.LifeCycleAssessment.Results.ElementResult2<T>.MaterialResults)}");

            //Call constructor
            return new ElementResult2<T>(objectId, scope, category, first.MetricType, castResults, totalMetrics);
        }

        /***************************************************/

    }
}



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
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;


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
        public static List<IElementResult<MaterialResult>> IElementResults(IComparable objectId, ScopeType scope, ObjectCategory category, IEnumerable<MaterialResult> materialResults)
        {
            if (materialResults.IsNullOrEmpty())
                return new List<IElementResult<MaterialResult>>();

            List<IElementResult<MaterialResult>> elementResults = new List<IElementResult<MaterialResult>>();

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
        private static IElementResult<MaterialResult> ElementResult<T>(T first, IEnumerable<MaterialResult> materialResults, IComparable objectId, ScopeType scope, ObjectCategory category) where T : MaterialResult
        {
            //Cast the material results to the actual type
            List<T> castResults = materialResults.Cast<T>().ToList();

            Dictionary<Module, double> totalResults = new Dictionary<Module, double>();

            //Get modules existing in all results
            List<Module> allModules = castResults.SelectMany(x => x.Results.Keys).Distinct().ToList();
            List<Module> modules = allModules.ToList();

            foreach (var matResult in castResults)
            {
                modules = modules.Intersect(matResult.Results.Keys).ToList();
            }

            //Only include total for modules defined for all parts
            foreach (var module in modules.Distinct())
            {
                totalResults[module] = castResults.Sum(x => x.Results[module]);
            }

            var nonCombinedModules = allModules.Except(modules).ToList();
            if (nonCombinedModules.Count > 0)
                BH.Engine.Base.Compute.RecordNote($"MaterialResults that make up ElementResult of type {typeof(T).Name} contains modules not present in all material results. These are ommited when creating the Combined results for the element. The information about these modules can be found on the {nameof(BH.oM.LifeCycleAssessment.Results.ElementResult<T>.MaterialResults)}");



            //Call final create
            return Create.ElementResult<T>(objectId, scope, category, castResults, totalResults);
        }

        /***************************************************/

        [Description("Creates an ElementResult based on general constructor parameters and a provided list of results.")]
        [InputFromProperty("objectId")]
        [InputFromProperty("scope")]
        [InputFromProperty("category")]
        [InputFromProperty("materialResults")]
        [Input("resultValues", "The resuling values to be stored on the result. Important that the order of the metrics extracted corresponds to the order of the constructor. General order should always be all the default phases (A1-A5, B1-B7, C1-C4 and D) followed by any additional phases corresponding to the metric currently being evaluated. For example, GlobalWarmingPotential will have an additional property corresponding to BiogenicCarbon.")]
        [Output("result", "The created element result.")]
        private static IElementResult<MaterialResult> ElementResult<T>(IComparable objectId, ScopeType scope, ObjectCategory category, List<T> materialResults, Dictionary<Module, double> resultValues) where T : MaterialResult
        {
            //Get the constructor for the element result of the type corresponding to the type of material result
            //This is done by finding the ElementResult able to store the particular type of MaterialResult
            //The constructor is pre-compiled to a function to speed up the execution of the particular method
            Func<object[], IElementResult<MaterialResult>> cst = ElementResultConstructor<T>();

            if (cst == null)
                return null;

            //Set up parameters for the constructor.
            //This always begin with objectId, scope, category and the list of the material results, and last is the result values
            object[] parameters = new object[] { objectId, scope, category, new ReadOnlyCollection<T>(materialResults), resultValues };

            //Call constructor
            return cst(parameters);
        }

        /***************************************************/

        [Description("Gets a function corresponding to the constructor for an Element result corresponding to the provided type.\n" +
                 "If a type of ElementResult is provided, then the returned constructor will be corresponding to the provided type.\n" +
                 "If a type of MaterialResult is provided, the constructor will correspond to the ElementResult able to store this type.\n" +
                 "For all other types, null is returned.")]
        [Input("t", "The type to find a matching constructor for. Should be a type of ElementResult or a type of MaterialResult.")]
        [Output("cstFunc", "The function corresponding to the constructor of the ElementResult related to the type.")]
        private static Func<object[], IElementResult<MaterialResult>> ElementResultConstructor<T>() where T : MaterialResult
        {
            Func<object[], IElementResult<MaterialResult>> cstFunc;
            Type t = typeof(T);
            //Try get chached constructor func
            if (!m_ElementResultConstructors.TryGetValue(t, out cstFunc))
            {
                //Get out constructor info matching the type
                System.Reflection.ConstructorInfo constructor = GetElementResultConstructorInfo<T>();
                if (constructor != null)
                {
                    //Pre-compile the constructor info to a function to increase performance
                    Func<object[], object> genericFunc = constructor.ToFunc();
                    cstFunc = x => (IElementResult<MaterialResult>)genericFunc(x);
                }
                else
                    cstFunc = null;

                m_ElementResultConstructors[t] = cstFunc;
            }

            if (cstFunc == null)
                Base.Compute.RecordError($"Unable to find a constructor for a type of {nameof(IElementResult<MaterialResult>)} based on provided type {t}");

            return cstFunc;
        }

        /***************************************************/

        [Description("Gets a ConstructorInfo from a ElementResult type matching the provided type t.")]
        private static System.Reflection.ConstructorInfo GetElementResultConstructorInfo<T>() where T : MaterialResult
        {
            Type elementResultType = BH.Engine.Base.Query.BHoMTypeList().FirstOrDefault(x => typeof(IElementResult<T>).IsAssignableFrom(x));

            if (elementResultType == null)
                return null;

            return elementResultType.GetConstructors().OrderByDescending(x => x.GetParameters().Count()).First();
        }

        /***************************************************/

        [Description("Gets an ElementResult type matching the metric type by name.")]
        private static Type ElementResultTypeFromMetric(Type metricType)
        {
            string metric = metricType.Name.Replace("Metric", "");
            Type elementResultType = BH.Engine.Base.Query.BHoMTypeList().Where(x => typeof(MaterialResult).IsAssignableFrom(x)).First(x => x.Name == metric + "ElementResult");
            return elementResultType;
        }

        /***************************************************/
        /**** Private Fields                            ****/
        /***************************************************/

        //Storage of the pre-compiled functions for future usage
        private static ConcurrentDictionary<Type, Func<object[], IElementResult<MaterialResult>>> m_ElementResultConstructors = new ConcurrentDictionary<Type, Func<object[], IElementResult<MaterialResult>>>();

        /***************************************************/
    }
}



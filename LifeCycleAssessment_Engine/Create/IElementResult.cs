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

            //Compute the result values for the element as sum of all material results
            List<double> resultValues = castResults.SumPhaseDataValues();

            //Call final create
            return Create.ElementResult<T>(objectId, scope, category, castResults, resultValues);
        }

        /***************************************************/

        [Description("Creates an ElementResult based on general constructor parameters and a provided list of results.")]
        [InputFromProperty("objectId")]
        [InputFromProperty("scope")]
        [InputFromProperty("category")]
        [InputFromProperty("materialResults")]
        [Input("resultValues", "The resuling values to be stored on the result. Important that the order of the metrics extracted corresponds to the order of the constructor. General order should always be all the default phases (A1-A5, B1-B7, C1-C4 and D) followed by any additional phases corresponding to the metric currently being evaluated. For example, GlobalWarmingPotential will have an additional property corresponding to BiogenicCarbon.")]
        [Output("result", "The created element result.")]
        private static IElementResult<MaterialResult> ElementResult<T>(IComparable objectId, ScopeType scope, ObjectCategory category, List<T> materialResults, List<double> resultValues) where T : MaterialResult
        {
            //Get the constructor for the element result of the type corresponding to the type of material result
            //This is done by finding the ElementResult able to store the particular type of MaterialResult
            //The constructor is pre-compiled to a function to speed up the execution of the particular method
            Func<object[], IElementResult<MaterialResult>> cst = typeof(T).ElementResultConstructor();

            //Set up parameters for the constructor.
            //This always begin with objectId, scope, category and the list of the material results
            List<object> parameters = new List<object> { objectId, scope, category, new ReadOnlyCollection<T>(materialResults) };
            //Add the values, computed as the sum of all the MaterialResult parts,
            //this means for example the A1 value of the element result will be the sum of A1 from all the MaterialResults that are part of the ElementResult
            parameters.AddRange(resultValues.Cast<object>());

            //Call constructor
            return cst(parameters.ToArray());
        }

        /***************************************************/

        [Description("Gets a function corresponding to the constructor for an Element result corresponding to the provided type.\n" +
                 "If a type of ElementResult is provided, then the returned constructor will be corresponding to the provided type.\n" +
                 "If a type of MaterialResult is provided, the constructor will correspond to the ElementResult able to store this type.\n" +
                 "For all other types, null is returned.")]
        [Input("t", "The type to find a matching constructor for. Should be a type of ElementResult or a type of MaterialResult.")]
        [Output("cstFunc", "The function corresponding to the constructor of the ElementResult related to the type.")]
        private static Func<object[], IElementResult<MaterialResult>> ElementResultConstructor(this Type t)
        {
            Func<object[], IElementResult<MaterialResult>> cstFunc;

            //Try get chached constructor func
            if (!m_ElementResultConstructors.TryGetValue(t, out cstFunc))
            {
                //Get out constructor info matching the type
                ConstructorInfo constructor = GetElementResultConstructorInfo(t);
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
        private static ConstructorInfo GetElementResultConstructorInfo(Type t)
        {
            Type elementResultType = null;
            if (typeof(IElementResult<MaterialResult>).IsAssignableFrom(t))    //Type of Elementresult -> simply return
                elementResultType = t;
            else if (typeof(MaterialResult).IsAssignableFrom(t))   //Type of material result -> Find element result able to store it
                elementResultType = typeof(Create).GetMethod(nameof(ElementResultTypeFromMaterialResultType), BindingFlags.Static | BindingFlags.NonPublic).MakeGenericMethod(t).Invoke(null, new object[] { }) as Type;
            else if (typeof(EnvironmentalMetric).IsAssignableFrom(t))  //Type of metric -> match by name
                elementResultType = ElementResultTypeFromMetric(t);

            if (elementResultType == null)
                return null;

            return elementResultType.GetConstructors().OrderByDescending(x => x.GetParameters().Count()).First();
        }

        /***************************************************/

        [Description("Gets an ElementResult type able to store MaterialResult type T.")]
        private static Type ElementResultTypeFromMaterialResultType<T>() where T : MaterialResult
        {
            Type materialResultType = BH.Engine.Base.Query.BHoMTypeList()
                .First(x => typeof(IElementResult<T>).IsAssignableFrom(x));
            return materialResultType;
        }

        /***************************************************/

        [Description("Gets an ElementResult type matching the metric type by name.")]
        private static Type ElementResultTypeFromMetric(Type metricType)
        {
            string metric = metricType.Name.Replace("Metric", "");
            Type elementResultType = BH.Engine.Base.Query.BHoMTypeList().Where(x => typeof(MaterialResult).IsAssignableFrom(x)).First(x => x.Name.Replace("ElementResult", "") == metric);
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

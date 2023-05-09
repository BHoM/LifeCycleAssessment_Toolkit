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
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Creates a MaterialResult of a type matched to the provided Type.")]
        [Input("type", "Type used to find which type of MaterialResult to create. Should be a type of EnvironmentalMeric, a type of MaterialResult or a type of ElementResult.")]
        [Input("epdName", "The name of the EnvironmentalProductDeclaration that was evaluated and that the result being created corresponds to. Stored as an identifier on the returned result class.")]
        [Input("materialName", "The name of the Material that owns the EnvironmentalProductDeclaration that was evaluated and that the result being created corresponds to.. Stored as an identifier on the returned result class.")]
        [Input("resultValues", "The resuling values to be stored on the result. Imporant that the order of the metrics extracted cooresponds to the order of the constructor. General order should always be all the default phases (A1-A5, B1-B7, C1-C4 and D) followed by any additional phases corresponding to the metric currently being evaluated. For example, GlobalWarmpingPotential will have an additional property corresponding to BiogenicCarbon.")]
        [Output("result", "The created MaterialResult.")]
        public static MaterialResult MaterialResult(Type type, string materialName, string epdName, List<double> resultValues)
        {
            //Get the constructor for the material result of the type corresponding to the metric currently being evaluated
            //This is done by finding the MaterialResult with matching name and exatracting the constructor from it
            //The constructor is pre-compiled to a function to speed up the execution of the particular method
            Func<object[], MaterialResult> cst = MaterialResultConstructor(type);

            //Collect all the relevant data for constructor (essentailly, all properties for the result in correct order)
            //First two parameters of all MaterialResults should always be name of the material and name of the EPD
            List<object> parameters = new List<object> { materialName, epdName };
            //Add the rest of the evaluation metrics
            //For most cases this will be the phases 
            //Imporant that the order of the metrics extracted cooresponds to the order of the constructor
            //General order should always be all the default phases (A1-A5, B1-B7, C1-C4 and D) followed by any additional phases corresponding to the metric currently being evaluated
            //For example, GlobalWarmpingPotential will have an additional property corresponding to BiogenicCarbon
            parameters.AddRange(resultValues.Cast<object>());  //Gets the resulting final metrics for each phase from the metric

            //Call the constructor function
            return cst(parameters.ToArray());
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        [Description("Gets a function corresponding to the constructor for a Material result corresponding to the provided type.\n" +
                   "If a type of MaterialResult is provided, then the returned constructor will be corresponding to the provided type.\n" +
                   "If a type of EnvironmentalMetric is provided, the constructor will correspond to the MaterialResult corresponding to this type.\n" +
                   "For all other types, null is returned.")]
        [Input("t", "The type to find a matching constructor for. Should be a type of MaterialResult or a type of EnvironmentalMetric.")]
        [Output("cstFunc", "The function correpsonding to the constructor of the MaterialResult related to the type.")]
        private static Func<object[], MaterialResult> MaterialResultConstructor(this Type t)
        {
            Func<object[], MaterialResult> cstFunc;

            //Try get chached constructor func
            if (!m_MaterialResultConstructors.TryGetValue(t, out cstFunc))
            {
                //Get out constructor info matching the type
                ConstructorInfo constructor = GetMaterialResultConstructorInfo(t);
                if (constructor != null)
                {
                    //Pre-compile the constructor info to a function to increase performance
                    Func<object[], object> genericFunc = constructor.ToFunc();
                    cstFunc = x => (MaterialResult)genericFunc(x);
                }
                else
                    cstFunc = null;

                m_MaterialResultConstructors[t] = cstFunc;
            }

            if (cstFunc == null)
                Base.Compute.RecordError($"Unable to find a constructor for a type of {nameof(MaterialResult)} based on provided type {t}");

            return cstFunc;
        }

        /***************************************************/

        [Description("Gets a ConstructorInfo from a MaterialResult type matcheching the provided type t.")]
        private static ConstructorInfo GetMaterialResultConstructorInfo(Type t)
        {
            Type materialResultType = null;
            if (typeof(MaterialResult).IsAssignableFrom(t))    //Type of material result -> simply use it
                materialResultType = t;
            else if (typeof(IEnvironmentalMetric).IsAssignableFrom(t))  //Type of metric -> match by name
                materialResultType = MaterialResultTypeFromMetric(t);
            else if (typeof(IElementResult<MaterialResult>).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)    //Type of element reuslt, get generic argument from base class
                materialResultType = t.BaseType?.GenericTypeArguments.FirstOrDefault();

            if (materialResultType == null)
                return null;

            return materialResultType.GetConstructors().OrderByDescending(x => x.GetParameters().Count()).First();
        }

        /***************************************************/

        [Description("Gets a MaterialResult type matching the metric type by name.")]
        private static Type MaterialResultTypeFromMetric(Type metricType)
        {
            string metric = metricType.Name.Replace("Metrics", "");
            Type materialResultType = BH.Engine.Base.Query.BHoMTypeList().Where(x => typeof(MaterialResult).IsAssignableFrom(x)).First(x => x.Name.StartsWith(metric));
            return materialResultType;
        }

        /***************************************************/
        /**** Private Fields                            ****/
        /***************************************************/

        //Storage of the pre-compiled functions for future usage
        private static ConcurrentDictionary<Type, Func<object[], MaterialResult>> m_MaterialResultConstructors = new ConcurrentDictionary<Type, Func<object[], MaterialResult>>();

        /***************************************************/
    }
}

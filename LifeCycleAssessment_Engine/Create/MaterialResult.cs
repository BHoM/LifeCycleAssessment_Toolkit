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
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace BH.Engine.LifeCycleAssessment
{
    public static partial class Create
    {

        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Creates a MaterialResult of a type matched to the provided Type.")]
        [Input("materialName", "The name of the Material that owns the EnvironmentalProductDeclaration that was evaluated and that the result being created corresponds to. Stored as an identifier on the returned result class.")]
        [Input("epdName", "The name of the EnvironmentalProductDeclaration that was evaluated and that the result being created corresponds to. Stored as an identifier on the returned result class.")]
        [Input("resultingIndicator", "The resulting values to be stored on the result per each module.")]
        [Output("result", "The created MaterialResult.")]
        public static MaterialResult MaterialResult(string materialName, string epdName, MetricType metricType, Dictionary<Module, double> resultingIndicator)
        {
            resultingIndicator = resultingIndicator.ComputeAndAddTotalModules();
            //Get the constructor for the material result of the type corresponding to the metric currently being evaluated
            //This is done by finding the MaterialResult with matching name and exatracting the constructor from it
            //The constructor is pre-compiled to a function to speed up the execution of the particular method
            Func<object[], MaterialResult> cst = MaterialResultConstructor(metricType);

            if (cst == null)
                return null;

            //Collect all the relevant data for constructor (essentailly, all properties for the result in correct order)
            //First two parameters of all MaterialResults should always be name of the material and name of the EPD
            //Thir parameter is the dictionary containing the resulting values
            object[] parameters = new object[] { materialName, epdName, resultingIndicator };


            //Call the constructor function
            return cst(parameters);
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/


        private static Dictionary<Module, double> ComputeAndAddTotalModules(this Dictionary<Module, double> metrics)
        {
            Dictionary<Module, double> metricsWithTotals = metrics.ToDictionary(x => x.Key, x => x.Value);

            //Try to add all "Sum" modules to be computed as the sum of the parts
            //Important to make sure combining results coming from EPDs set up in slightly different fashion possible, for example
            //Should be able to combine results from one EPD that has A1, A2 and A3 set up with one that has A1toA3 set up.
            foreach (var combinationModules in Query.CombinationModules())
            {
                AddIfNotPresent(metricsWithTotals, combinationModules.Key, combinationModules.Value);
            }

            return metricsWithTotals;
        }

        /***************************************************/

        private static void AddIfNotPresent(this Dictionary<Module, double> metricsWithTotal, Module moduleToAdd, IReadOnlyList<(Module, bool)> modulesToSum)
        {
            if (metricsWithTotal.ContainsKey(moduleToAdd))
                return; //Allready present

            double total = 0;
            bool hasAnyPart = false;
            foreach ((Module, bool) moduleRequired in modulesToSum)
            {
                Module module = moduleRequired.Item1;
                if (metricsWithTotal.TryGetValue(module, out double val))
                {
                    hasAnyPart = true; //At least one part is present
                    total += val;
                }
                else if (moduleRequired.Item2)
                    return;     //Required part not found and marked as required. Not able to add
            }

            //Compute new metric value as sum of parts
            if(hasAnyPart)
                metricsWithTotal[moduleToAdd] = total;

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
        private static Func<object[], MaterialResult> MaterialResultConstructor(this MetricType metricType)
        {
            Func<object[], MaterialResult> cstFunc;

            //Try get chached constructor func
            if (!m_MaterialResultConstructors.TryGetValue(metricType, out cstFunc))
            {
                //Get out constructor info matching the type
                System.Reflection.ConstructorInfo constructor = GetMaterialResultConstructorInfo(metricType);
                if (constructor != null)
                {
                    //Pre-compile the constructor info to a function to increase performance
                    Func<object[], object> genericFunc = constructor.ToFunc();
                    cstFunc = x => (MaterialResult)genericFunc(x);
                }
                else
                    cstFunc = null;

                m_MaterialResultConstructors[metricType] = cstFunc;
            }

            if (cstFunc == null)
                Base.Compute.RecordError($"Unable to find a constructor for a type of {nameof(MaterialResult)} based on provided type {metricType}");

            return cstFunc;
        }

        /***************************************************/

        [Description("Gets a ConstructorInfo from a MaterialResult type matcheching the provided type t.")]
        private static System.Reflection.ConstructorInfo GetMaterialResultConstructorInfo(MetricType t)
        {
            Type materialResultType = BH.Engine.Base.Query.BHoMTypeList().Where(x => typeof(MaterialResult).IsAssignableFrom(x)).FirstOrDefault(x => x.Name == t.ToString() + "MaterialResult");

            if (materialResultType == null)
                return null;

            return materialResultType.GetConstructors().OrderByDescending(x => x.GetParameters().Count()).First();
        }

        /***************************************************/
        /**** Private Fields                            ****/
        /***************************************************/

        //Storage of the pre-compiled functions for future usage
        private static ConcurrentDictionary<MetricType, Func<object[], MaterialResult>> m_MaterialResultConstructors = new ConcurrentDictionary<MetricType, Func<object[], MaterialResult>>();

        /***************************************************/
    }
}



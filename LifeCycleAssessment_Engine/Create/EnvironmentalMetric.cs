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

        [Description("Creates a EnvironmentalMetric of a type matched to the provided Type.")]
        [Input("type", "Type used to find which type of MaterialResult to create. Should be a type of EnvironmentalMetric, a type of MaterialResult or a type of ElementResult.")]
        [Input("name", "The name of the EnvironmentalMetric.")]
        [Input("metricValues", "The stage values of the metric. Important that the order of the metrics extracted corresponds to the order of the constructor. General order should always be all the default phases (A1-A5, B1-B7, C1-C4 and D) followed by any additional phases corresponding to the metric currently being evaluated.")]
        [Output("metric", "The created EnvironmentalMetric.")]
        public static EnvironmentalMetric EnvironmentalMetric(Type type, string name, List<double> metricValues)
        {
            //Get the constructor for the material metric of the type corresponding to the metric currently being evaluated
            //This is done by finding the EnvironmentalMetric with matching name and exatracting the constructor from it
            //The constructor is pre-compiled to a function to speed up the execution of the particular method
            Func<object[], EnvironmentalMetric> cst = EnvironmentalMetricConstructor(type);

            //Add the rest of the evaluation metrics
            //For most cases this will be the phases 
            //Imporant that the order of the metrics extracted cooresponds to the order of the constructor
            //General order should always be all the default phases (A1-A5, B1-B7, C1-C4 and D) followed by any additional phases corresponding to the metric currently being evaluated
            object[] parameters = metricValues.Cast<object>().ToArray();  //Gets the resulting final metrics for each phase from the metric

            //Call the constructor function
            EnvironmentalMetric metric = cst(parameters);

            //Set the name. Done separately as not part opf the constructor
            metric.Name = name;
            return metric;
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
        private static Func<object[], EnvironmentalMetric> EnvironmentalMetricConstructor(this Type t)
        {
            Func<object[], EnvironmentalMetric> cstFunc;

            //Try get chached constructor func
            if (!m_EnvironmentalMetricConstructors.TryGetValue(t, out cstFunc))
            {
                //Get out constructor info matching the type
                ConstructorInfo constructor = GetEnvironmentalMetricConstructorInfo(t);
                if (constructor != null)
                {
                    //Pre-compile the constructor info to a function to increase performance
                    Func<object[], object> genericFunc = constructor.ToFunc();
                    cstFunc = x => (EnvironmentalMetric)genericFunc(x);
                }
                else
                    cstFunc = null;

                m_EnvironmentalMetricConstructors[t] = cstFunc;
            }

            if (cstFunc == null)
                Base.Compute.RecordError($"Unable to find a constructor for a type of {nameof(MaterialResult)} based on provided type {t}");

            return cstFunc;
        }

        /***************************************************/

        [Description("Gets a ConstructorInfo from a MaterialResult type matcheching the provided type t.")]
        private static ConstructorInfo GetEnvironmentalMetricConstructorInfo(Type t)
        {
            Type environmentalMetricType = null;

            if (typeof(EnvironmentalMetric).IsAssignableFrom(t))  //Type of metric -> simply use it
                environmentalMetricType = t;
            else if (typeof(IEnvironmentalResult).IsAssignableFrom(t))    //Type of environmental result -> match by name
                environmentalMetricType = EnvironmentalMetricTypeFromResult(t);

            if (environmentalMetricType == null)
                return null;

            return environmentalMetricType.GetConstructors().OrderByDescending(x => x.GetParameters().Count()).First();
        }

        /***************************************************/

        [Description("Gets a MaterialResult type matching the metric type by name.")]
        private static Type EnvironmentalMetricTypeFromResult(Type resultType)
        {
            string metric = resultType.Name.Replace("MaterialResult", "").Replace("ElementResult", "") + "Metric";
            Type environmentalMetricType = BH.Engine.Base.Query.BHoMTypeList().Where(x => typeof(EnvironmentalMetric).IsAssignableFrom(x)).First(x => x.Name == metric);
            return environmentalMetricType;
        }

        /***************************************************/
        /**** Private Fields                            ****/
        /***************************************************/

        //Storage of the pre-compiled functions for future usage
        private static ConcurrentDictionary<Type, Func<object[], EnvironmentalMetric>> m_EnvironmentalMetricConstructors = new ConcurrentDictionary<Type, Func<object[], EnvironmentalMetric>>();

        /***************************************************/
    }
}


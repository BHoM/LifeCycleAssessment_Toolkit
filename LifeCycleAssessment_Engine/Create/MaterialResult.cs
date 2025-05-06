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
        [Input("resultValues", "The resulting values to be stored on the result. Important that the order of the metrics extracted corresponds to the order of the constructor. General order should always be all the default phases (A1-A5, B1-B7, C1-C4 and D) followed by any additional phases corresponding to the metric currently being evaluated. For example, GlobalWarmingPotential will have an additional property corresponding to BiogenicCarbon.")]
        [Output("result", "The created MaterialResult.")]
        public static IMaterialResult IMaterialResult(string materialName, string environmentalProductDeclarationName, MetricType metricType, IDictionary metrics)
        {
            return MaterialResult(materialName, environmentalProductDeclarationName, metricType, metrics as dynamic);
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        [Description("Creates a MaterialResult of a type matched to the provided Type.")]
        [Input("materialName", "The name of the Material that owns the EnvironmentalProductDeclaration that was evaluated and that the result being created corresponds to. Stored as an identifier on the returned result class.")]
        [Input("epdName", "The name of the EnvironmentalProductDeclaration that was evaluated and that the result being created corresponds to. Stored as an identifier on the returned result class.")]
        [Input("resultValues", "The resulting values to be stored on the result. Important that the order of the metrics extracted corresponds to the order of the constructor. General order should always be all the default phases (A1-A5, B1-B7, C1-C4 and D) followed by any additional phases corresponding to the metric currently being evaluated. For example, GlobalWarmingPotential will have an additional property corresponding to BiogenicCarbon.")]
        [Output("result", "The created MaterialResult.")]
        private static MaterialResult<T> MaterialResult<T>(string materialName, string environmentalProductDeclarationName, MetricType metricType, IReadOnlyDictionary<Module, T> metrics)
            where T : IMetricValue, new()
        {
            return new MaterialResult<T>(materialName, environmentalProductDeclarationName, metricType, metrics.ComputeAndAddTotalModules());
        }

        /***************************************************/

        private static IReadOnlyDictionary<Module, T> ComputeAndAddTotalModules<T>(this IReadOnlyDictionary<Module, T> metrics)
            where T : IMetricValue, new()
        {
            Dictionary<Module, T> metricsWithTotals = metrics.ToDictionary(x => x.Key, x => x.Value);

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

        private static void AddIfNotPresent<T>(this Dictionary<Module, T> metricsWithTotal, Module moduleToAdd, IReadOnlyList<Module> modulesToSum)
            where T : IMetricValue, new()
        { 
            if (metricsWithTotal.ContainsKey(moduleToAdd))
                return; //Allready present

            double total = 0;

            foreach (Module module in modulesToSum)
            {
                if (metricsWithTotal.TryGetValue(module, out T a))
                    total += a.Value;
                else
                    return;     //Required part not found. Not able to add
            }

            //Compute new metric value as sum of parts
            metricsWithTotal[moduleToAdd] = new T { Value = total };
        }

        /***************************************************/
    }
}



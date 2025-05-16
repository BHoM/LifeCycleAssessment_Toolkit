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
using BH.Engine.Matter;
using BH.oM.Analytical.Results;
using BH.oM.Base;
using BH.oM.Base.Attributes;
using BH.oM.Dimensional;
using BH.oM.LifeCycleAssessment;
using BH.oM.LifeCycleAssessment.Configs;
using BH.oM.LifeCycleAssessment.Fragments;
using BH.oM.LifeCycleAssessment.MaterialFragments;
using BH.oM.LifeCycleAssessment.MaterialFragments.Transport;
using BH.oM.LifeCycleAssessment.Results;
using BH.oM.Physical.Materials;
using System;
using System.Collections;
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

        [Description("Gets the factors for each module as a dictionary.")]
        [Input("moduleFactors", "The factors to extract the dictionary from.")]
        [Output("FactorsDictionary", "The factors for each module stored on the metric.")]
        public static Dictionary<MetricType, double> ITransportResults(this ITransportFactors transportFactors, TakeoffItem takeoffItem, List<MetricType> metricFilter)
        {
            double mass = takeoffItem.QuantityValue(QuantityType.Mass);
            return ITransportResults(transportFactors, mass, metricFilter);
        }

        /***************************************************/

        [Description("Gets the factors for each module as a dictionary.")]
        [Input("moduleFactors", "The factors to extract the dictionary from.")]
        [Output("FactorsDictionary", "The factors for each module stored on the metric.")]
        public static Dictionary<MetricType, double> ITransportResults(this ITransportFactors moduleFactors, double mass, List<MetricType> metricFilter)
        {
            return TransportResults(moduleFactors as dynamic, mass, metricFilter);
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        [Description("Gets the factors for each module as a dictionary.")]
        [Input("transportScenario", "The factors to extract the dictionary from.")]
        [Output("FactorsDictionary", "The factors for each module stored on the metric.")]
        public static Dictionary<MetricType, double> TransportResults(this FullTransportScenario transportScenario, double mass, List<MetricType> metricFilter)
        {
            Dictionary<MetricType, double> results = new Dictionary<MetricType, double>();

            List<IEnvironmentalFactor> factors = transportScenario.EnvironmentalFactors;

            if (metricFilter != null && metricFilter.Count != 0)
                factors = factors.Where(x => metricFilter.Contains(x.IMetricType())).ToList();

            foreach (IEnvironmentalFactor factor in factors)
            {
                MetricType type = factor.IMetricType();

                double resultingValue = mass * factor.Value;
                if (results.ContainsKey(type))
                    results[type] += resultingValue;
                else
                    results[type] = resultingValue;

            }

            return results;
        }

        /***************************************************/

        [Description("Gets the factors for each module as a dictionary.")]
        [Input("moduleFactors", "The factors to extract the dictionary from.")]
        [Output("FactorsDictionary", "The factors for each module stored on the metric.")]
        public static Dictionary<MetricType, double> TransportResults(this DistanceTransportModeScenario transportScenario, double mass, List<MetricType> metricFilter)
        {
            if (transportScenario == null || transportScenario.SingleTransportModeImpacts == null || transportScenario.SingleTransportModeImpacts.Count == 0)
                return new Dictionary<MetricType, double>();

            //Set to impact of first part of the journey
            Dictionary<MetricType, double> results = transportScenario.SingleTransportModeImpacts[0].TransportResults(mass, metricFilter);

            //Add all other journey parts
            for (int i = 1; i < transportScenario.SingleTransportModeImpacts.Count; i++)
            {
                Dictionary<MetricType, double> singleJourneyRes = transportScenario.SingleTransportModeImpacts[i].TransportResults(mass, metricFilter);

                foreach (var item in singleJourneyRes)
                {
                    if(results.ContainsKey(item.Key))
                        results[item.Key] += item.Value;
                    else
                        results[item.Key] = item.Value;
                }
            }

            return results;
        }

        /***************************************************/

        [Description("Gets the factors for each module as a dictionary.")]
        [Input("moduleFactors", "The factors to extract the dictionary from.")]
        [Output("FactorsDictionary", "The factors for each module stored on the metric.")]
        public static Dictionary<MetricType, double> TransportResults(this SingleTransportModeImpact transportScenario, double mass, List<MetricType> metricFilter)
        {
            Dictionary<MetricType, double> results = new Dictionary<MetricType, double>();

            List<IEnvironmentalFactor> factors = transportScenario.VehicleEmissions.EnvironmentalFactors;

            if (metricFilter != null && metricFilter.Count != 0)
                factors = factors.Where(x => metricFilter.Contains(x.IMetricType())).ToList();

            double quantity = mass * transportScenario.DistanceTraveled * (1 + transportScenario.VehicleEmissions.ReturnTripFactor);

            foreach (IEnvironmentalFactor factor in factors)
            {
                MetricType type = factor.IMetricType();

                double resultingValue = quantity * factor.Value;
                if (results.ContainsKey(type))
                    results[type] += resultingValue;
                else
                    results[type] = resultingValue;
            }

            return results;
        }

        /***************************************************/
    }
}



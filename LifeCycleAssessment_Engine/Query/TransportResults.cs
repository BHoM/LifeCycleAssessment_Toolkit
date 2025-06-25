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


using BH.oM.Base.Attributes;
using BH.oM.LifeCycleAssessment;
using BH.oM.LifeCycleAssessment.MaterialFragments;
using BH.oM.LifeCycleAssessment.MaterialFragments.Transport;
using BH.oM.Physical.Materials;
using BH.oM.Quantities.Attributes;
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
        /**** Public Methods - interface                ****/
        /***************************************************/

        [Description("Gets the resulting values for each metric as a dictionary where the key is the metric type and the value the resulting impact of that type.")]
        [Input("transportFactors", "The transport factors to evaluate.")]
        [Input("takeoffItem", "The takeoff item to extract the mass from.")]
        [Input("metricFilter", "Optional filter for the provided ITransportFactors for selecting one or more of the provided metrics for calculation. This method also accepts multiple metric types simultaneously. If nothing is provided then no filtering is assumed, i.e. all metrics on the found EPDS are evaluated.")]
        [Output("transportResults", "Resulting values for the trnsport factors as a dictionary where the key is the metric type and value the resulting value.")]
        public static Dictionary<MetricType, double> ITransportResults(this ITransportFactors transportFactors, TakeoffItem takeoffItem, List<MetricType> metricFilter)
        {
            double mass = takeoffItem.QuantityValue(QuantityType.Mass);
            return ITransportResults(transportFactors, mass, metricFilter);
        }

        /***************************************************/

        [Description("Gets the resulting values for each metric as a dictionary where the key is the metric type and the value the resulting impact of that type.")]
        [Input("transportFactors", "The transport factors to evaluate.")]
        [Input("mass", "Mass to evaluate the transport factors against.", typeof(Mass))]
        [Input("metricFilter", "Optional filter for the provided ITransportFactors for selecting one or more of the provided metrics for calculation. This method also accepts multiple metric types simultaneously. If nothing is provided then no filtering is assumed, i.e. all metrics on the found EPDS are evaluated.")]
        [Output("transportResults", "Resulting values for the trnsport factors as a dictionary where the key is the metric type and value the resulting value.")]
        public static Dictionary<MetricType, double> ITransportResults(this ITransportFactors transportFactors, double mass, List<MetricType> metricFilter)
        {
            return TransportResults(transportFactors as dynamic, mass, metricFilter);
        }

        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Gets the resulting values for each metric as a dictionary where the key is the metric type and the value the resulting impact of that type.")]
        [Input("transportScenario", "The FullTransportScenario to evaluate.")]
        [Input("mass", "Mass to evaluate the transport factors against.", typeof(Mass))]
        [Input("metricFilter", "Optional filter for the provided ITransportFactors for selecting one or more of the provided metrics for calculation. This method also accepts multiple metric types simultaneously. If nothing is provided then no filtering is assumed, i.e. all metrics on the found EPDS are evaluated.")]
        [Output("transportResults", "Resulting values for the trnsport factors as a dictionary where the key is the metric type and value the resulting value.")]
        public static Dictionary<MetricType, double> TransportResults(this FullTransportScenario transportScenario, double mass, List<MetricType> metricFilter)
        {
            Dictionary<MetricType, double> results = new Dictionary<MetricType, double>();

            List<IEnvironmentalFactor> factors = transportScenario.EnvironmentalFactors.FilterIndicators(metricFilter);

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

        [Description("Gets the resulting values for each metric as a dictionary where the key is the metric type and the value the resulting impact of that type.")]
        [Input("transportScenario", "The DistanceTransportModeScenario to evaluate.")]
        [Input("mass", "Mass to evaluate the transport factors against.", typeof(Mass))]
        [Input("metricFilter", "Optional filter for the provided ITransportFactors for selecting one or more of the provided metrics for calculation. This method also accepts multiple metric types simultaneously. If nothing is provided then no filtering is assumed, i.e. all metrics on the found EPDS are evaluated.")]
        [Output("transportResults", "Resulting values for the trnsport factors as a dictionary where the key is the metric type and value the resulting value.")]
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

        [Description("Gets the resulting values for each metric as a dictionary where the key is the metric type and the value the resulting impact of that type.")]
        [Input("transportScenario", "The SingleTransportModeImpact to evaluate.")]
        [Input("mass", "Mass to evaluate the transport factors against.", typeof(Mass))]
        [Input("metricFilter", "Optional filter for the provided ITransportFactors for selecting one or more of the provided metrics for calculation. This method also accepts multiple metric types simultaneously. If nothing is provided then no filtering is assumed, i.e. all metrics on the found EPDS are evaluated.")]
        [Output("transportResults", "Resulting values for the trnsport factors as a dictionary where the key is the metric type and value the resulting value.")]
        public static Dictionary<MetricType, double> TransportResults(this SingleTransportModeImpact transportScenario, double mass, List<MetricType> metricFilter)
        {
            Dictionary<MetricType, double> results = new Dictionary<MetricType, double>();

            List<IEnvironmentalFactor> factors = transportScenario.VehicleEmissions.EnvironmentalFactors.FilterIndicators(metricFilter);

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



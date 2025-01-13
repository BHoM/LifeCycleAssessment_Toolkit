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

using BH.oM.Base;
using BH.oM.Base.Attributes;
using BH.oM.LifeCycleAssessment.Enums;
using BH.oM.LifeCycleAssessment.MaterialFragments;
using BH.oM.LifeCycleAssessment.MaterialFragments.Transport;
using System;
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

        [Description("Creates transport factors to be used for LCA evaluation. Type of transport factors returned depends on the type of methodology provided.")]
        [Input("methodology", "The methodology used to create the trnsport factors. Will control which of the provided parameter that will be used.")]
        [Input("typicalScenario", "A full transport scenario, generally grabbed from a dataset. Used in the methodology is set to TypicalTransportScenario.")]
        [Input("vehicleEmissions", "Emissions for the vehicle used to transport the goods. Used in the methodology is set to DistanceTransportMode.")]
        [Input("distanceTraveled", "The distance traveled using the vehicle defined in the Vehicle emssions. Used in the methodology is set to DistanceTransportMode.")]
        [Input("lengthUnits", "Units for distance traveled. Supported units are metres, kilometres and miles (m, km and mi). Distance set on the created object will be in the SI unit meters. Used in the methodology is set to DistanceTransportMode.")]
        [Input("customFactor", "Custom factor for explicitly setting the climate change transport emissions. Value should be per mass (kg). Will create new metrics for ClimateChangeFossil, ClimateChangeTotal and ClimateCHangeTotalNoBiogenic and set the A4 value to this value, and return a custom full transport scenario with the metrics set.")]
        [Output("transportFactors", "The created transport factors, type depending on provided methodology.")]
        public static ITransportFactors TransportFactors(TransportMethodology methodology, FullTransportScenario typicalScenario = null, VehicleEmissions vehicleEmissions = null, double distanceTraveled = double.NaN, string lengthUnits = "m", double customFactor = double.NaN)
        {
            switch (methodology)
            {
                case TransportMethodology.DistanceTransportMode:
                    return DistanceTransportMode(vehicleEmissions, distanceTraveled, lengthUnits);
                case TransportMethodology.TypicalTransportScenario:
                    if (typicalScenario == null)
                        Base.Compute.RecordError("Methodology is set to typical transport scenario but the provided typical transport scenario is null");
                    return typicalScenario;
                case TransportMethodology.CustomTransportFactors:
                    if (double.IsNaN(customFactor))
                    {
                        Base.Compute.RecordError($"Cannot create custom transport factors given a unset {nameof(customFactor)}.");
                        return null;
                    }
                    return new FullTransportScenario
                    {
                        Name = "User custom scenario",
                        EnvironmentalMetrics = new List<EnvironmentalMetric>
                        {
                            Create.ClimateChangeFossilMetric(double.NaN, customFactor, double.NaN, double.NaN, double.NaN, double.NaN),
                            Create.ClimateChangeTotalMetric(double.NaN, customFactor, double.NaN, double.NaN, double.NaN, double.NaN),
                            Create.ClimateChangeTotalNoBiogenicMetric(double.NaN, customFactor, double.NaN, double.NaN, double.NaN, double.NaN),
                        }
                    };
                case TransportMethodology.Undefined:
                default:
                    Base.Compute.RecordError($"Unable to create transport factors with the methodology set to {methodology}");
                    return null;
            }
        }
        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static DistanceTransportModeScenario DistanceTransportMode(VehicleEmissions vehicleEmissions, double distanceTraveled, string lengthUnits)
        {
            if (vehicleEmissions == null)
            {
                Base.Compute.RecordError($"Cannot create transport factors given a null {nameof(VehicleEmissions)}.");
                return null;
            }

            if (double.IsNaN(distanceTraveled))
            {
                Base.Compute.RecordError($"Cannot create transport factors given a unset {nameof(distanceTraveled)}.");
                return null;
            }

            lengthUnits = lengthUnits.ToLower().Replace(" ", "");

            double scaleFactor = 1;
            if (lengthUnits == "m" || lengthUnits == "metre" || lengthUnits == "metres" || lengthUnits == "meter" || lengthUnits == "meters")
                scaleFactor = 1;
            else if (lengthUnits == "km" || lengthUnits == "kilometer" || lengthUnits == "kilometers" || lengthUnits == "kilometre" || lengthUnits == "kilometres")
                scaleFactor = 1000;
            else if (lengthUnits == "mi" || lengthUnits == "mile" || lengthUnits == "miles")
                scaleFactor = 1609.344;
            else
            {
                Base.Compute.RecordError("length unit is unkown. Supported values are 'm', 'meter', 'metre', 'metres' and 'meters' for metres, 'km', 'kilometer', 'kilometre', 'kilometres' and 'kilometers' for kilometres and 'mi','mile' and 'miles' for miles.");
                return null;
            }

            return new DistanceTransportModeScenario
            {
                SingleTransportModeImpacts = new List<SingleTransportModeImpact> { new SingleTransportModeImpact { VehicleEmissions = vehicleEmissions, DistanceTraveled = distanceTraveled * scaleFactor } }
            };

        }

        /***************************************************/
    }
}

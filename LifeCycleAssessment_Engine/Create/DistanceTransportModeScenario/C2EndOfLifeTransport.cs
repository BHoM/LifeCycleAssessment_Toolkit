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
using BH.oM.Base;
using BH.oM.Base.Attributes;
using BH.oM.LifeCycleAssessment.Enums;
using BH.oM.LifeCycleAssessment.MaterialFragments;
using BH.oM.LifeCycleAssessment.MaterialFragments.EndOfLife;
using BH.oM.LifeCycleAssessment.MaterialFragments.Transport;
using BH.oM.Quantities.Attributes;
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


        [Description("Creates a C2 end-of-life transport scenario for materials based on different disposal routes (landfill, recycling, reuse, and energy recovery incineration). The method calculates transport impacts for each disposal route weighted by the end-of-life route distribution factors.")]
        [Input("transportDistanceToLandfill", "Transport distance from the building to the landfill disposal site. Average distance to two closest landfill sites.", typeof(Length))]
        [Input("transportDistanceToRecycling", "Transport distance from the building to the recycling facility. Average distance to two closest construction waste processing sites.", typeof(Length))]
        [Input("transportDistanceToEnergyRecoveryIncineration", "Transport distance from the building to the energy recovery incineration facility. Average distance to two closest energy-from-waste sites.", typeof(Length))]
        [Input("vehicleEmission", "Vehicle emissions data containing environmental factors per unit mass transported.")]
        [Input("endOfLifeRoute", "End-of-life route distribution factors specifying the proportion of material going to each disposal route (waste, recycling, reuse, incineration).")]
        [Input("emptyRunningFactor", "Optional factor for empty return trips. If provided, overrides the return trip factor in the vehicle emissions. Default is NaN (uses vehicle emissions return trip factor).")]
        [Output("transportScenario", "A DistanceTransportModeScenario containing weighted transport impacts for all disposal routes, named with the end-of-life route distribution name.")]
        public static DistanceTransportModeScenario C2EndOfLifeTransport(double transportDistanceToLandfill, double transportDistanceToRecycling, double transportDistanceToEnergyRecoveryIncineration, VehicleEmissions vehicleEmission, EndOfLifeRouteDistribution endOfLifeRoute, double emptyRunningFactor = double.NaN)
        {
            if (!double.IsNaN(emptyRunningFactor))
            {
                vehicleEmission = vehicleEmission.DeepClone();
                vehicleEmission.ReturnTripFactor = emptyRunningFactor;
            }

            SingleTransportModeImpact landfill = new SingleTransportModeImpact
            {
                VehicleEmissions = vehicleEmission,
                DistanceTraveled = transportDistanceToLandfill,
                Factor = endOfLifeRoute.Waste,
                Name = "Landfill"
            };

            SingleTransportModeImpact recycling = new SingleTransportModeImpact
            {
                VehicleEmissions = vehicleEmission,
                DistanceTraveled = transportDistanceToRecycling,
                Factor = endOfLifeRoute.Recycling,
                Name = "Recycling"
            };

            SingleTransportModeImpact reuse = new SingleTransportModeImpact
            {
                VehicleEmissions = vehicleEmission,
                DistanceTraveled = transportDistanceToRecycling,
                Factor = endOfLifeRoute.Reuse,
                Name = "Reuse"
            };

            SingleTransportModeImpact incineration = new SingleTransportModeImpact
            {
                VehicleEmissions = vehicleEmission,
                DistanceTraveled = transportDistanceToEnergyRecoveryIncineration,
                Factor = endOfLifeRoute.Incineration,
                Name = "Incineration"
            };

            return new DistanceTransportModeScenario
            {
                Name = $"C2 impact {endOfLifeRoute.Name}",
                SingleTransportModeImpacts = new List<SingleTransportModeImpact> { landfill, recycling, reuse, incineration }
            };
        }

        /***************************************************/
    }
}

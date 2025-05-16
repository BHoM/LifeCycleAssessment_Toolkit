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
using BH.oM.LifeCycleAssessment.MaterialFragments;
using BH.oM.LifeCycleAssessment.MaterialFragments.Transport;
using BH.oM.Quantities.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;

namespace BH.Engine.LifeCycleAssessment
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Creates a VehicleEmissions given an emmisons factor. Method creates new metrics and assigns the emissions factor  climate change (fossil, total and total no biogenic) and assigns them to the returned VehicleEmissions.")]
        [Input("emmissionsFactor", "Emissions factor for explicitly setting the climate change transport emissions. Value should be per mass (kg). Will create new factors for ClimateChangeFossil, ClimateChangeTotal and ClimateChangeTotalNoBiogenic.", typeof(ClimateChangePerQuantity))]
        [InputFromProperty("name")]
        [InputFromProperty("returnTripFactor")]
        [Output("vehicleEmissions", "The created VehicleEmissions.")]
        public static VehicleEmissions VehicleEmissions(double emmissionsFactor, double returnTripFactor = 0, string name = "")
        {
            return new VehicleEmissions
            {
                Name = name,
                EnvironmentalFactors = new List<IEnvironmentalFactor>
                {
                    new ClimateChangeFossilFactor{ Value = emmissionsFactor },
                    new ClimateChangeTotalFactor{ Value = emmissionsFactor },
                    new ClimateChangeTotalNoBiogenicFactor{ Value = emmissionsFactor },
                }
            };
        }

        /***************************************************/
    }
}

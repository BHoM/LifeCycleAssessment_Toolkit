/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2021, the respective contributors. All rights reserved.
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using BH.oM.Base;
using BH.oM.Reflection.Attributes;
using BH.oM.Structure.MaterialFragments;

namespace BH.Engine.LifeCycleAssessment
{
    public static partial class Compute
    {
        /***************************************************/
        /****   Public Methods                          ****/
        /***************************************************/

        [Description("Calculates the acidification potential from volume, density, and embodied Sulfer Dioxide quantities. These quantities may be provided within Environmental Product Declaration documentation.")]
        [Input("volume", "Provide material volume in m^3.")]
        [Input("density", "Provide material density in kg/m^3. This value may be available within an EPD Dataset.")]
        [Input("embodiedSulfurDioxide", "Amount of embodied kg SO2/m^3 equivalent. Refer to EPD dataset for corresponding input metric.")]
        [Output("acidificationPotential", "The consequence of acids being emitted to the atmosphere and subsequently deposited in surface soils and waters measured in kg/SO2e.")]
        public static double AcidificationPotential(double volume = 0.0, double density = 0.0, double embodiedSulfurDioxide = 0.0)
        {
            return volume * density * embodiedSulfurDioxide;
            /***************************************************/
        }
    }
}




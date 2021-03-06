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

        [Description("Calculates the Eutrophication Potential  from volume, density, and embodied Phosphate quantities. These quantities may be provided within Environmental Product Declaration documentation.")]
        [Input("volume", "Provide material volume in m^3. ")]
        [Input("density", "Provide material density in kg/m^3. This value may be available within an EPD Dataset.")]
        [Input("embodiedPhosphate", "Amount of embodied kg PO4/m^3 equivalent. Refer to EPD dataset for corresponding input metric.")]
        [Output("eutrophicationPotential", "The pollution state of aquatic ecosystems in which the over-fertilization of water and soil has turned into an increased growth of biomass measured in kg/PO4e.")]
        public static double EutrophicationPotential(double volume = 0.0, double density = 0.0, double embodiedPhosphate = 0.0)
        {
            return volume * density * embodiedPhosphate;
        }
        /***************************************************/
    }
}


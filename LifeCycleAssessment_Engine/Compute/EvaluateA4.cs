/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2023, the respective contributors. All rights reserved.
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
using BH.oM.Dimensional;
using BH.oM.LifeCycleAssessment.MaterialFragments;
using BH.oM.LifeCycleAssessment.Results;
using BH.oM.LifeCycleAssessment;
using BH.oM.Physical.Materials;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using BH.Engine.Matter;
using BH.Engine.Spatial;
using BH.oM.Geospatial;

namespace BH.Engine.LifeCycleAssessment
{
    public static partial class Compute
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/


        [Description("This method calculates the numerical global warming potential impact of phase A4 of a material's Life Cycle")]
        [Input("transportationLinks", "A list of links with start and end points to define an entire journey that the building element has taken to resolve Phase A4 evaluations.")]
        [Output("result", "The total emissions per link in kgCO2e/kg.")]
        public static List<double> EvaluateA4(List<TransportationLink> transporationLinks)
        {
            if (transporationLinks == null || transporationLinks.Count() <= 0)
            {
                return null;
            }

            List<double> result = new List<double>();

            /**
             * Calculation for Phase A4
             * W*D*TEF
             * Weight * Distance (per leg) * Transport Emissions Factor (per mode of transport)
             */

            /**
             * Evaluate each link 
             * Calculate distance between start / end
             * Get the weight of the element
             * Get the TEF from the TransportationMetric within the link
             */

            return result;
        }

        /***************************************************/
    }
}


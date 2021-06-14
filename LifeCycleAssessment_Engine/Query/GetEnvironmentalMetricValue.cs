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

using BH.Engine.Matter;
using BH.oM.Dimensional;
using BH.oM.LifeCycleAssessment;
using BH.oM.LifeCycleAssessment.MaterialFragments;
using BH.oM.Reflection.Attributes;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using BH.Engine.Base;

namespace BH.Engine.LifeCycleAssessment
{
    public static partial class Query
    {
        /***************************************************/
        /****   Public Methods                          ****/
        /***************************************************/

        [Description("Return a list of all environmental metrics withing a provided EPD.")]
        [Input("epd", "An Environmental Product Declaration object or EPD.")]
        [Output("environmentalMetric", "A list of all Environmental Metrics that comprise a given EPD.")]
        public static List<double> GetEnvironmentalMetricValue(this EnvironmentalProductDeclaration epd, EnvironmentalProductDeclarationField field)
        {
            // EPD Null Check
            if (epd == null)
            {
                BH.Engine.Reflection.Compute.RecordError("No EPD found. Returning double.NaN.");
                return new List<double>();
            }

            // EPD Environmental Metric null check
            if (epd.EnvironmentalMetric.Count <= 0)
            {
                BH.Engine.Reflection.Compute.RecordError("No Environmental Metrics have been found. Returning double.NaN.");
                return new List<double>();
            }

            // get the Quantity Values
            List<double> quantity = (List<double>)epd.EnvironmentalMetric.Select(x => x.Quantity);

            return quantity;
        }

        /***************************************************/

        // possibly need one that requires IElementM 

    }
}

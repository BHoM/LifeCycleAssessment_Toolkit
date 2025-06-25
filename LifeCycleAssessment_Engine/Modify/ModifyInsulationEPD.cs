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

using BH.oM.Geometry;
using BH.oM.Dimensional;
using System;
using System.Collections.Generic;
using System.Linq;
using BH.oM.Base;
using BH.Engine.Geometry;
using BH.Engine.Spatial;
using BH.Engine.Base;
using BH.oM.Physical.FramingProperties;

using BH.oM.Base.Attributes;
using System.ComponentModel;
using BH.oM.LifeCycleAssessment.MaterialFragments;
using BH.oM.Quantities.Attributes;
using BH.oM.LifeCycleAssessment;

namespace BH.Engine.Facade
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Scales the quantity values in an area based insulation EPD based on project RSI (m2K/W).")]
        [Input("insulationEPD", "Insulation EPD to scale quantity values for.")]
        [Input("rSI", "RSI for insulation (m2K/W).")]
        [Output("modifiedEPD", "FrameEdgeProperty with modified section profile depth.")]
        public static EnvironmentalProductDeclaration ModifyInsulationEPD(this EnvironmentalProductDeclaration insulationEPD, double rSI)
        {
            if (rSI <= 0 || rSI == double.NaN)
            {
                Base.Compute.RecordError($"Valid RSI is not assigned.");
                return null;
            }

            if (insulationEPD == null)
            {
                Base.Compute.RecordError($"Cannot convert null EPD.");
                return null;
            }

            if (insulationEPD.QuantityType != oM.LifeCycleAssessment.QuantityType.Area)
            {
                Base.Compute.RecordError($"ModifyInsulationEPD only works on insulation EPDs with Area quantity type.");
                return null;
            }

            insulationEPD = insulationEPD.DeepClone();
            foreach (IEnvironmentalMetric environmentalMetric in insulationEPD.EnvironmentalMetrics)
            {
                foreach (Module module in environmentalMetric.Indicators.Keys)
                {
                    environmentalMetric.Indicators[module] = environmentalMetric.Indicators[module] * rSI;
                }
            }

            return insulationEPD;
        }
    }
}

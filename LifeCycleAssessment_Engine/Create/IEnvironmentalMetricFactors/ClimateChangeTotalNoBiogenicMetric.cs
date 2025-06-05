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
using BH.oM.LifeCycleAssessment;
using BH.oM.LifeCycleAssessment.MaterialFragments;
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

        [Description("Creates a ClimateChangeTotalNoBiogenicMetric to be applied as part of an EnvironmentalProductDeclaration. If no values are provided (NaN), the module will not be added to the created metric.")]
        [InputFromDescription("a1", Module.A1, typeof(ClimateChangePerQuantity))]
        [InputFromDescription("a2", Module.A2, typeof(ClimateChangePerQuantity))]
        [InputFromDescription("a3", Module.A3, typeof(ClimateChangePerQuantity))]
        [InputFromDescription("a4", Module.A4, typeof(ClimateChangePerQuantity))]
        [InputFromDescription("a5", Module.A5, typeof(ClimateChangePerQuantity))]
        [InputFromDescription("b1", Module.B1, typeof(ClimateChangePerQuantity))]
        [InputFromDescription("b2", Module.B2, typeof(ClimateChangePerQuantity))]
        [InputFromDescription("b3", Module.B3, typeof(ClimateChangePerQuantity))]
        [InputFromDescription("b4", Module.B4, typeof(ClimateChangePerQuantity))]
        [InputFromDescription("b5", Module.B5, typeof(ClimateChangePerQuantity))]
        [InputFromDescription("b6", Module.B6, typeof(ClimateChangePerQuantity))]
        [InputFromDescription("b7", Module.B7, typeof(ClimateChangePerQuantity))]
        [InputFromDescription("c1", Module.C1, typeof(ClimateChangePerQuantity))]
        [InputFromDescription("c2", Module.C2, typeof(ClimateChangePerQuantity))]
        [InputFromDescription("c3", Module.C3, typeof(ClimateChangePerQuantity))]
        [InputFromDescription("c4", Module.C4, typeof(ClimateChangePerQuantity))]
        [InputFromDescription("d", Module.D, typeof(ClimateChangePerQuantity))]
        [Output("metric", "Created ClimateChangeTotalNoBiogenicMetric.")]
        public static ClimateChangeTotalNoBiogenicMetric ClimateChangeTotalNoBiogenicMetric(
            double a1 = double.NaN,
            double a2 = double.NaN,
            double a3 = double.NaN,
            double a4 = double.NaN,
            double a5 = double.NaN,
            double b1 = double.NaN,
            double b2 = double.NaN,
            double b3 = double.NaN,
            double b4 = double.NaN,
            double b5 = double.NaN,
            double b6 = double.NaN,
            double b7 = double.NaN,
            double c1 = double.NaN,
            double c2 = double.NaN,
            double c3 = double.NaN,
            double c4 = double.NaN,
            double d = double.NaN)
        {
            return new ClimateChangeTotalNoBiogenicMetric() { Indicators = FactorsDictionary(a1, a2, a3, a4, a5, b1, b2, b3, b4, b5, b6, b7, c1, c2, c3, c4, d) };
        }

        /***************************************************/

        [Description("Creates a ClimateChangeTotalNoBiogenicMetric to be applied as part of an EnvironmentalProductDeclaration. Create method to be used when no discrete values for A1, A2 and A3 are available, but only a total value for those 3 phases. If no values are provided (NaN), the module will not be added to the created metric.")]
        [InputFromDescription("a1toa3", Module.A1toA3, typeof(ClimateChangePerQuantity))]
        [InputFromDescription("a4", Module.A4, typeof(ClimateChangePerQuantity))]
        [InputFromDescription("a5", Module.A5, typeof(ClimateChangePerQuantity))]
        [InputFromDescription("b1", Module.B1, typeof(ClimateChangePerQuantity))]
        [InputFromDescription("b2", Module.B2, typeof(ClimateChangePerQuantity))]
        [InputFromDescription("b3", Module.B3, typeof(ClimateChangePerQuantity))]
        [InputFromDescription("b4", Module.B4, typeof(ClimateChangePerQuantity))]
        [InputFromDescription("b5", Module.B5, typeof(ClimateChangePerQuantity))]
        [InputFromDescription("b6", Module.B6, typeof(ClimateChangePerQuantity))]
        [InputFromDescription("b7", Module.B7, typeof(ClimateChangePerQuantity))]
        [InputFromDescription("c1", Module.C1, typeof(ClimateChangePerQuantity))]
        [InputFromDescription("c2", Module.C2, typeof(ClimateChangePerQuantity))]
        [InputFromDescription("c3", Module.C3, typeof(ClimateChangePerQuantity))]
        [InputFromDescription("c4", Module.C4, typeof(ClimateChangePerQuantity))]
        [InputFromDescription("d", Module.D, typeof(ClimateChangePerQuantity))]
        [Output("metric", "Created ClimateChangeTotalNoBiogenicMetric.")]
        public static ClimateChangeTotalNoBiogenicMetric ClimateChangeTotalNoBiogenicMetric(
                double a1toa3 = double.NaN,
                double a4 = double.NaN,
                double a5 = double.NaN,
                double b1 = double.NaN,
                double b2 = double.NaN,
                double b3 = double.NaN,
                double b4 = double.NaN,
                double b5 = double.NaN,
                double b6 = double.NaN,
                double b7 = double.NaN,
                double c1 = double.NaN,
                double c2 = double.NaN,
                double c3 = double.NaN,
                double c4 = double.NaN,
                double d = double.NaN)
        {
            return new ClimateChangeTotalNoBiogenicMetric() { Indicators = FactorsDictionary(a1toa3, a4, a5, b1, b2, b3, b4, b5, b6, b7, c1, c2, c3, c4, d) };
        }

        /***************************************************/

        [Description("Creates a ClimateChangeTotalNoBiogenicMetric to be applied as part of an EnvironmentalProductDeclaration. Create method to be used when no discrete values for the phases in the Product stage (A1 - A3), use stage (B1-B7) or end of life stage (C1-C4) is given, but only the total value for the phases in those stages are available. If no values are provided (NaN), the module will not be added to the created metric.")]
        [InputFromDescription("a1toa3", Module.A1toA3, typeof(ClimateChangePerQuantity))]
        [InputFromDescription("a4", Module.A4, typeof(ClimateChangePerQuantity))]
        [InputFromDescription("a5", Module.A5, typeof(ClimateChangePerQuantity))]
        [InputFromDescription("b1tob7", Module.B1toB7, typeof(ClimateChangePerQuantity))]
        [InputFromDescription("c1toc4", Module.C1toC4, typeof(ClimateChangePerQuantity))]
        [InputFromDescription("d", Module.D, typeof(ClimateChangePerQuantity))]
        [Output("metric", "Created ClimateChangeTotalNoBiogenicMetric.")]
        public static ClimateChangeTotalNoBiogenicMetric ClimateChangeTotalNoBiogenicMetric(
                double a1toa3 = double.NaN,
                double a4 = double.NaN,
                double a5 = double.NaN,
                double b1tob7 = double.NaN,
                double c1toc4 = double.NaN,
                double d = double.NaN)
        {

            return new ClimateChangeTotalNoBiogenicMetric() { Indicators = FactorsDictionary(a1toa3, a4, a5, b1tob7, c1toc4, d) };
        }

        /***************************************************/
    }
}


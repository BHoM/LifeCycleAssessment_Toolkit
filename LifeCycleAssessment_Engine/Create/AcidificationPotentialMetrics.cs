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
using BH.oM.LifeCycleAssessment.MaterialFragments;
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

        [Description("")]
        [Input("", "")]
        [Output("", "")]
        public static AcidificationPotentialMetrics AcidificationPotentialMetrics(
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
            double a1toa3 = a1 + a2 + a3;
            return new AcidificationPotentialMetrics(a1, a2, a3, a1toa3, a4, a5, b1, b2, b3, b4, b5, b6, b7, c1, c2, c3, c4, d);
        }

        /***************************************************/

        [Description("")]
        [Input("", "")]
        [Output("", "")]
        public static AcidificationPotentialMetrics AcidificationPotentialMetrics(
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
            return new AcidificationPotentialMetrics(double.NaN, double.NaN, double.NaN, a1toa3, a4, a5, b1, b2, b3, b4, b5, b6, b7, c1, c2, c3, c4, d);

        }

        /***************************************************/
    }
}

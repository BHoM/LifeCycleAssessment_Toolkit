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

        [Description("Creates a dictioanry of factors for all non-NaN entries")]
        [Output("factors", "Created Factors dictionary.")]
        private static Dictionary<Module, double> FactorsDictionary(
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
            Dictionary<Module, double> factors = new Dictionary<Module, double>();
            factors.AddIfNotNan(Module.A1, a1);
            factors.AddIfNotNan(Module.A2, a2);
            factors.AddIfNotNan(Module.A3, a3);
            factors.AddIfNotNan(Module.A4, a4);
            factors.AddIfNotNan(Module.A5, a5);
            factors.AddIfNotNan(Module.B1, b1);
            factors.AddIfNotNan(Module.B2, b2);
            factors.AddIfNotNan(Module.B3, b3);
            factors.AddIfNotNan(Module.B4, b4);
            factors.AddIfNotNan(Module.B5, b5);
            factors.AddIfNotNan(Module.B6, b6);
            factors.AddIfNotNan(Module.B7, b7);
            factors.AddIfNotNan(Module.C1, c1);
            factors.AddIfNotNan(Module.C2, c2);
            factors.AddIfNotNan(Module.C3, c3);
            factors.AddIfNotNan(Module.C4, c4);
            factors.AddIfNotNan(Module.D, d);

            return factors;
        }

        /***************************************************/

        [Description("Creates a dictioanry of factors for all non-NaN entries")]
        [Output("factors", "Created Factors dictionary.")]
        private static Dictionary<Module, double> FactorsDictionary(
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
            Dictionary<Module, double> factors = new Dictionary<Module, double>();
            factors.AddIfNotNan(Module.A1toA3, a1toa3);
            factors.AddIfNotNan(Module.A4, a4);
            factors.AddIfNotNan(Module.A5, a5);
            factors.AddIfNotNan(Module.B1, b1);
            factors.AddIfNotNan(Module.B2, b2);
            factors.AddIfNotNan(Module.B3, b3);
            factors.AddIfNotNan(Module.B4, b4);
            factors.AddIfNotNan(Module.B5, b5);
            factors.AddIfNotNan(Module.B6, b6);
            factors.AddIfNotNan(Module.B7, b7);
            factors.AddIfNotNan(Module.C1, c1);
            factors.AddIfNotNan(Module.C2, c2);
            factors.AddIfNotNan(Module.C3, c3);
            factors.AddIfNotNan(Module.C4, c4);
            factors.AddIfNotNan(Module.D, d);

            return factors;
        }

        /***************************************************/

        [Description("Creates a dictioanry of factors for all non-NaN entries")]
        [Output("factors", "Created Factors dictionary.")]
        private static Dictionary<Module, double> FactorsDictionary(
                double a1toa3 = double.NaN,
                double a4 = double.NaN,
                double a5 = double.NaN,
                double b1tob7 = double.NaN,
                double c1toc4 = double.NaN,
                double d = double.NaN)
        {
            Dictionary<Module, double> factors = new Dictionary<Module, double>();
            factors.AddIfNotNan(Module.A1toA3, a1toa3);
            factors.AddIfNotNan(Module.A4, a4);
            factors.AddIfNotNan(Module.A5, a5);
            factors.AddIfNotNan(Module.B1toB7, b1tob7);
            factors.AddIfNotNan(Module.C1toC4, c1toc4);
            factors.AddIfNotNan(Module.D, d);

            return factors;
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static void AddIfNotNan(this Dictionary<Module, double> factors, Module module, double factor)
        {
            if (!double.IsNaN(factor))
                factors[module] = factor;
            
        }

        /***************************************************/
    }
}


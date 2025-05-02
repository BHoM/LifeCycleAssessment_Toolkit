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
using BH.oM.LifeCycleAssessment.MaterialFragments.EnvironmentalFactors;
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

        [Description("Creates a ClimateChangeFossilMetric to be applied as part of an EnvironmentalProductDeclaration. Values of A1toA3, B1toB7 and C1toC4 will be computed as the sum of the relevant phases.")]
        [InputFromProperty("a1")]
        [InputFromProperty("a2")]
        [InputFromProperty("a3")]
        [InputFromProperty("a4")]
        [InputFromProperty("a5")]
        [InputFromProperty("b1")]
        [InputFromProperty("b2")]
        [InputFromProperty("b3")]
        [InputFromProperty("b4")]
        [InputFromProperty("b5")]
        [InputFromProperty("b6")]
        [InputFromProperty("b7")]
        [InputFromProperty("c1")]
        [InputFromProperty("c2")]
        [InputFromProperty("c3")]
        [InputFromProperty("c4")]
        [InputFromProperty("d")]
        [Output("apMetric", "Created ClimateChangeFossilMetric.")]
        private static EnvironmentalMetricFactors<T> EnvironmentalMetricFactors<T>(
            MetricType metricType,
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
            double d = double.NaN) where T : class, IEnvironmentalFactor, new()
        {
            DynamicProperties<LifeCycleAssessmentModule, T> factors = new DynamicProperties<LifeCycleAssessmentModule, T>();
            factors.AddIfNotNan(LifeCycleAssessmentModule.A1, a1);
            factors.AddIfNotNan(LifeCycleAssessmentModule.A2, a2);
            factors.AddIfNotNan(LifeCycleAssessmentModule.A3, a3);
            factors.AddIfNotNan(LifeCycleAssessmentModule.A4, a4);
            factors.AddIfNotNan(LifeCycleAssessmentModule.A5, a5);
            factors.AddIfNotNan(LifeCycleAssessmentModule.B1, b1);
            factors.AddIfNotNan(LifeCycleAssessmentModule.B2, b2);
            factors.AddIfNotNan(LifeCycleAssessmentModule.B3, b3);
            factors.AddIfNotNan(LifeCycleAssessmentModule.B4, b4);
            factors.AddIfNotNan(LifeCycleAssessmentModule.B5, b5);
            factors.AddIfNotNan(LifeCycleAssessmentModule.B6, b6);
            factors.AddIfNotNan(LifeCycleAssessmentModule.B7, b7);
            factors.AddIfNotNan(LifeCycleAssessmentModule.C1, c1);
            factors.AddIfNotNan(LifeCycleAssessmentModule.C2, c2);
            factors.AddIfNotNan(LifeCycleAssessmentModule.C3, c3);
            factors.AddIfNotNan(LifeCycleAssessmentModule.C4, c4);
            factors.AddIfNotNan(LifeCycleAssessmentModule.D, d);

            return new EnvironmentalMetricFactors<T>() { Factors = factors };
        }

        /***************************************************/

        [Description("Creates a ClimateChangeFossilMetric to be applied as part of an EnvironmentalProductDeclaration. Create method to be used when no discrete values for A1, A2 and A3 are available, but only a total value for those 3 phases. Values of B1toB7 and C1toC4 will be computed as the sum of the relevant phases.")]
        [InputFromProperty("a1toa3", "A1toA3")]
        [InputFromProperty("a4")]
        [InputFromProperty("a5")]
        [InputFromProperty("b1")]
        [InputFromProperty("b2")]
        [InputFromProperty("b3")]
        [InputFromProperty("b4")]
        [InputFromProperty("b5")]
        [InputFromProperty("b6")]
        [InputFromProperty("b7")]
        [InputFromProperty("c1")]
        [InputFromProperty("c2")]
        [InputFromProperty("c3")]
        [InputFromProperty("c4")]
        [InputFromProperty("d")]
        [Output("apMetric", "Created ClimateChangeFossilMetric.")]
        private static EnvironmentalMetricFactors<T> EnvironmentalMetricFactors<T>(
            MetricType metricType,
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
                double d = double.NaN) where T : class, IEnvironmentalFactor, new()
        {
            DynamicProperties<LifeCycleAssessmentModule, T> factors = new DynamicProperties<LifeCycleAssessmentModule, T>();
            factors.AddIfNotNan(LifeCycleAssessmentModule.A1toA3, a1toa3);
            factors.AddIfNotNan(LifeCycleAssessmentModule.A4, a4);
            factors.AddIfNotNan(LifeCycleAssessmentModule.A5, a5);
            factors.AddIfNotNan(LifeCycleAssessmentModule.B1, b1);
            factors.AddIfNotNan(LifeCycleAssessmentModule.B2, b2);
            factors.AddIfNotNan(LifeCycleAssessmentModule.B3, b3);
            factors.AddIfNotNan(LifeCycleAssessmentModule.B4, b4);
            factors.AddIfNotNan(LifeCycleAssessmentModule.B5, b5);
            factors.AddIfNotNan(LifeCycleAssessmentModule.B6, b6);
            factors.AddIfNotNan(LifeCycleAssessmentModule.B7, b7);
            factors.AddIfNotNan(LifeCycleAssessmentModule.C1, c1);
            factors.AddIfNotNan(LifeCycleAssessmentModule.C2, c2);
            factors.AddIfNotNan(LifeCycleAssessmentModule.C3, c3);
            factors.AddIfNotNan(LifeCycleAssessmentModule.C4, c4);
            factors.AddIfNotNan(LifeCycleAssessmentModule.D, d);

            return new EnvironmentalMetricFactors<T>() { Factors = factors };
        }

        /***************************************************/

        [Description("Creates a ClimateChangeFossilMetric to be applied as part of an EnvironmentalProductDeclaration. Create method to be used when no discrete values for the phases in the Product stage (A1 - A3), use stage (B1-B7) or end of life stage (C1-C4) is given, but only the total value for the phases in those stages are available.")]
        [InputFromProperty("a1toa3", "A1toA3")]
        [InputFromProperty("a4")]
        [InputFromProperty("a5")]
        [InputFromProperty("b1tob7", "B1toB7")]
        [InputFromProperty("c1toc4", "C1toC4")]
        [InputFromProperty("d")]
        [Output("apMetric", "Created ClimateChangeFossilMetric.")]
        private static EnvironmentalMetricFactors<T> EnvironmentalMetricFactors<T>(
            MetricType metricType,
                double a1toa3 = double.NaN,
                double a4 = double.NaN,
                double a5 = double.NaN,
                double b1tob7 = double.NaN,
                double c1toc4 = double.NaN,
                double d = double.NaN) where T : class, IEnvironmentalFactor, new()
        {
            DynamicProperties<LifeCycleAssessmentModule, T> factors = new DynamicProperties<LifeCycleAssessmentModule, T>();
            factors.AddIfNotNan(LifeCycleAssessmentModule.A1toA3, a1toa3);
            factors.AddIfNotNan(LifeCycleAssessmentModule.A4, a4);
            factors.AddIfNotNan(LifeCycleAssessmentModule.A5, a5);
            factors.AddIfNotNan(LifeCycleAssessmentModule.B1toB7, b1tob7);
            factors.AddIfNotNan(LifeCycleAssessmentModule.C1toC4, c1toc4);
            factors.AddIfNotNan(LifeCycleAssessmentModule.D, d);

            return new EnvironmentalMetricFactors<T>() { Factors = factors };
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static void AddIfNotNan<T>(this Dictionary<LifeCycleAssessmentModule, T> factors, LifeCycleAssessmentModule module, double factor)
            where T : IEnvironmentalFactor, new()
        {
            if (!double.IsNaN(factor))
                factors[module] = new T { Value = factor };
            
        }

        /***************************************************/
    }
}


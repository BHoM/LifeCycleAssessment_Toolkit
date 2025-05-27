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

using AutoBogus;
using BH.Engine.LifeCycleAssessment;
using BH.oM.LifeCycleAssessment;
using BH.oM.LifeCycleAssessment.MaterialFragments;
using BH.oM.LifeCycleAssessment.Results;
using BH.oM.Physical.Materials;
using BH.oM.Test.NUnit;
using FluentAssertions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BH.Tests.Engine.LifeCycleAssessment
{
    public class TotalTests : NUnitTest
    {
        /***************************************************/
        /****   Public Methods                          ****/
        /***************************************************/

        [TestCaseSource(typeof(DataSource), nameof(DataSource.DummyMetrics), new object[] { 1, 0, false })]
        public void Total(IEnvironmentalMetric metricFactors)
        {
            double tol = 1e-12;
            metricFactors.Total().Should().BeApproximately(17, tol, "Initial value");

            metricFactors.Indicators[oM.LifeCycleAssessment.Module.A1toA3] = 3;
            metricFactors.Total().Should().BeApproximately(17, tol, "A1toA3");

            metricFactors.Indicators[oM.LifeCycleAssessment.Module.B1toB3] = 3;
            metricFactors.Total().Should().BeApproximately(17, tol, "B1toB3");

            metricFactors.Indicators[oM.LifeCycleAssessment.Module.B4toB5] = 2;
            metricFactors.Total().Should().BeApproximately(17, tol, "B4toB5");

            metricFactors.Indicators[oM.LifeCycleAssessment.Module.B1toB5] = 5;
            metricFactors.Total().Should().BeApproximately(17, tol, "B1toB5");

            metricFactors.Indicators[oM.LifeCycleAssessment.Module.B1toB7] = 7;
            metricFactors.Total().Should().BeApproximately(17, tol, "B1toB7");

            metricFactors.Indicators[oM.LifeCycleAssessment.Module.C3toC4] = 2;
            metricFactors.Total().Should().BeApproximately(17, tol, "C3toC4");

            metricFactors.Indicators[oM.LifeCycleAssessment.Module.C1toC4] = 4;
            metricFactors.Total().Should().BeApproximately(17, tol, "C1toC4");


        }

        /***************************************************/

        [TestCaseSource(typeof(DataSource), nameof(DataSource.DummyMetrics), new object[] { 1, 0, false })]
        public void TotalA(IEnvironmentalMetric metricFactors)
        {
            double tol = 1e-12;
            metricFactors.ATotal().Should().BeApproximately(5, tol, "Initial value");

            metricFactors.Indicators[oM.LifeCycleAssessment.Module.A1toA3] = 3;
            metricFactors.ATotal().Should().BeApproximately(5, tol, "A1toA3");
        }

        /***************************************************/

        [TestCaseSource(typeof(DataSource), nameof(DataSource.DummyMetrics), new object[] { 1, 0, false })]
        public void TotalB(IEnvironmentalMetric metricFactors)
        {
            double tol = 1e-12;
            metricFactors.BTotal().Should().BeApproximately(7, tol, "Initial value");
            metricFactors.Indicators[oM.LifeCycleAssessment.Module.B1toB3] = 3;
            metricFactors.BTotal().Should().BeApproximately(7, tol, "B1toB3");

            metricFactors.Indicators[oM.LifeCycleAssessment.Module.B4toB5] = 2;
            metricFactors.BTotal().Should().BeApproximately(7, tol, "B4toB5");

            metricFactors.Indicators[oM.LifeCycleAssessment.Module.B1toB5] = 5;
            metricFactors.BTotal().Should().BeApproximately(7, tol, "B1toB5");

            metricFactors.Indicators[oM.LifeCycleAssessment.Module.B1toB7] = 7;
            metricFactors.BTotal().Should().BeApproximately(7, tol, "B1toB7");
        }

        /***************************************************/

        [TestCaseSource(typeof(DataSource), nameof(DataSource.DummyMetrics), new object[] { 1, 0, false })]
        public void TotalC(IEnvironmentalMetric metricFactors)
        {
            double tol = 1e-12;
            metricFactors.CTotal().Should().BeApproximately(4, tol, "Initial value");

            metricFactors.Indicators[oM.LifeCycleAssessment.Module.C3toC4] = 2;
            metricFactors.CTotal().Should().BeApproximately(4, tol, "C3toC4");

            metricFactors.Indicators[oM.LifeCycleAssessment.Module.C1toC4] = 4;
            metricFactors.CTotal().Should().BeApproximately(4, tol, "C1toC4");
        }

        /***************************************************/

    }
}



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
using BH.oM.LifeCycleAssessment.Interfaces;
using BH.oM.LifeCycleAssessment.MaterialFragments;
using BH.oM.LifeCycleAssessment.Results;
using BH.oM.Physical.Materials;
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
    public class MetricTypeTests
    {
        /***************************************************/
        /****   Public Methods                          ****/
        /***************************************************/

        [TestCaseSource(typeof(DataSource), nameof(DataSource.DummyFactors), new object[] { 1, 0 })]
        [TestCaseSource(typeof(DataSource), nameof(DataSource.DummyMetrics), new object[] { 1, 0, false })]
        public void MetricTypeNotUndefined(ILifeCycleAssemsmentIndicator metricFactors)
        {
            MetricType metricType = metricFactors.IMetricType();
            metricType.Should().NotBe(MetricType.Undefined);
        }

        /***************************************************/

        [TestCaseSource(typeof(DataSource), nameof(DataSource.DummyFactorsList), new object[] { 1, 0 })]
        [TestCaseSource(typeof(DataSource), nameof(DataSource.DummyMetricsList), new object[] { 1, 0, false })]
        public void MetricTypesAllUnique(IEnumerable<ILifeCycleAssemsmentIndicator> metricFactors)
        {
            List<MetricType> metricTypes = metricFactors.Select(x => x.IMetricType()).ToList();
            metricTypes.Should().OnlyHaveUniqueItems();
        }

        /***************************************************/

    }
}



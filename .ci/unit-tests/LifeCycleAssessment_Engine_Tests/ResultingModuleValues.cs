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
using BH.oM.LifeCycleAssessment.MaterialFragments.Construction;
using BH.oM.LifeCycleAssessment.Results;
using BH.oM.Physical.Materials;
using BH.oM.Test.NUnit;
using FluentAssertions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Tests.Engine.LifeCycleAssessment
{
    public class ResultingModuleValues : NUnitTest
    {
        /***************************************************/
        /****   Public Methods                          ****/
        /***************************************************/


        [TestCaseSource(nameof(PartCases))]
        [Description("Tests that resulting module values do not contain both combination modules and their constituent parts after evaluation resulting module values, in particular when pre-computed values have been provided. This is critical to ensure that their is no misalignment in the data for when precomputed values are provided.")]
        public void ResultingModuleValuesDoesNotContainCombinationOrParts(IEnvironmentalMetric metric, Dictionary<Module, PrecomputedModuleValues> precomputedValues, List<Module> expectedModules)
        {

            var resultingValues = Query.IResultingModuleValues(metric, 1, precomputedValues);
            Assert.That(resultingValues.Keys, Is.EquivalentTo(expectedModules), "Resulting module values should contain either combination modules or their parts, but not both.");


        }

        /***************************************************/

        private static IEnumerable<object[]> PartCases()
        {
            yield return new object[]
            {
                new ClimateChangeTotalMetric()
                {
                    Indicators = new Dictionary<Module, double> { { Module.A1toA3, 1 }, { Module.A5, 2 } }
                },
                new Dictionary<Module, PrecomputedModuleValues>() 
                { 
                    { Module.A5_1, new PrecomputedModuleValues {  ModuleValues = new Dictionary<MetricType, double> { { MetricType.ClimateChangeTotal, 1 } } } }
                },
                new List<Module> { Module.A1toA3, Module.A5_1 }
            };

            yield return new object[]
            {
                new ClimateChangeTotalMetric()
                {
                    Indicators = new Dictionary<Module, double> { { Module.A1toA3, 1 }, { Module.A5_1, 2 } }
                },
                new Dictionary<Module, PrecomputedModuleValues>()
                {
                    { Module.A5, new PrecomputedModuleValues {  ModuleValues = new Dictionary<MetricType, double> { { MetricType.ClimateChangeTotal, 1 } } } }
                },
                new List<Module> { Module.A1toA3, Module.A5 }
            };

            yield return new object[]
            {
                new ClimateChangeTotalMetric()
                {
                    Indicators = new Dictionary<Module, double> { { Module.A1toA3, 1 }, { Module.B1toB7, 2 } }
                },
                new Dictionary<Module, PrecomputedModuleValues>()
                {
                    { Module.B1_1, new PrecomputedModuleValues {  ModuleValues = new Dictionary<MetricType, double> { { MetricType.ClimateChangeTotal, 1 } } } }
                },
                new List<Module> { Module.A1toA3, Module.B1_1 }
            };

            yield return new object[]
            {
                new ClimateChangeTotalMetric()
                {
                    Indicators = new Dictionary<Module, double> { { Module.A1toA3, 1 }, { Module.B1_1, 2 }, { Module.B1_2, 2 }, { Module.B3, 2 } }
                },
                new Dictionary<Module, PrecomputedModuleValues>()
                {
                    { Module.B1toB7, new PrecomputedModuleValues {  ModuleValues = new Dictionary<MetricType, double> { { MetricType.ClimateChangeTotal, 1 } } } }
                },
                new List<Module> { Module.A1toA3, Module.B1toB7 }
            };
        }


    }
}



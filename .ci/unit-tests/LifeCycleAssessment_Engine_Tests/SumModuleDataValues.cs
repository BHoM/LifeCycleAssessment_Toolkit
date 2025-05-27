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
    public class SumTests : NUnitTest
    {
        /***************************************************/
        /****   Public Methods                          ****/
        /***************************************************/

        [TestCaseSource(nameof(SumTestCases))]
        public void SumTest(IReadOnlyList<ILifeCycleAssessmentModuleData<IDictionary<Module, double>>> moduleData, bool onlyIncludeIfAllAvailable, Dictionary<Module, double> assumedSum)
        {
            Dictionary<Module, double> sum = moduleData.SumModuleDataValues(onlyIncludeIfAllAvailable);

            sum.Count.Should().Be(assumedSum.Count);
            foreach (var sumItem in sum)
            {
                sumItem.Value.Should().Be(assumedSum[sumItem.Key], sumItem.Key.ToString());
            }

        }

        /***************************************************/

        private static IEnumerable<object[]> SumTestCases()
        {
            Dictionary<Module, double> dict1 = new Dictionary<Module, double>();
            dict1[Module.A1] = 1;
            dict1[Module.A2] = 2;
            dict1[Module.A3] = 3;

            dict1[Module.B1toB7] = 6;

            dict1[Module.C2] = 2;
            dict1[Module.C3] = 3;
            dict1[Module.C4] = 4;
            dict1[Module.D] = 0;

            Dictionary<Module, double> dict2 = new Dictionary<Module, double>();
            foreach (var item in dict1)
            {
                dict2[item.Key] = item.Value + 10;
            }

            dict1[Module.C1] = 1;
            dict1[Module.A4] = 4;
            dict1[Module.A5] = 5;


            Dictionary<Module, double> sum1 = new Dictionary<Module, double>();
            sum1[Module.A1] = 12;
            sum1[Module.A2] = 14;
            sum1[Module.A3] = 16;

            sum1[Module.B1toB7] = 22;

            sum1[Module.C2] = 14;
            sum1[Module.C3] = 16;
            sum1[Module.C4] = 18;
            sum1[Module.D] = 10;

            ClimateChangeFossilMetric metric1 = new ClimateChangeFossilMetric() { Indicators = dict1 };
            ClimateChangeFossilMetric metric2 = new ClimateChangeFossilMetric(){ Indicators = dict2 };

            yield return new object[] { new List<IEnvironmentalMetric> { metric1, metric2 }, true, sum1 };   //Check for only including modules present in both results

            Dictionary<Module, double> sum2 = new Dictionary<Module, double>(sum1);
            sum2[Module.C1] = 1;
            sum2[Module.A4] = 4;
            sum2[Module.A5] = 5;

            yield return new object[] { new List<IEnvironmentalMetric> { metric1, metric2 }, false, sum2 };  //Check for including all

            ClimateChangeFossilMaterialResult res1 = new ClimateChangeFossilMaterialResult("", "", dict1);
            ClimateChangeFossilMaterialResult res2 = new ClimateChangeFossilMaterialResult("", "", dict2);

            //Test that works for material results
            yield return new object[] { new List<MaterialResult> { res1, res2 }, true, sum1 };
            yield return new object[] { new List<MaterialResult> { res1, res2 }, false, sum2 };


            ClimateChangeBiogenicMetric metricOtherType = new ClimateChangeBiogenicMetric() { Indicators = dict2 };

            //Check that differing types give no results
            yield return new object[] { new List<IEnvironmentalMetric> { metric1, metricOtherType }, true, new Dictionary<Module, double>() };
            yield return new object[] { new List<IEnvironmentalMetric> { metric1, metricOtherType }, false, new Dictionary<Module, double>() };
        }

        /***************************************************/

    }
}



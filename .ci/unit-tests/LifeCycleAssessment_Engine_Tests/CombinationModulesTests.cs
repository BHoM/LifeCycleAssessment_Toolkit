/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2026, the respective contributors. All rights reserved.
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
    public class CombinationModulesTests : NUnitTest
    {
        /***************************************************/
        /****   Public Methods                          ****/
        /***************************************************/

        [TestCase(Module.A1, new[] { Module.A1toA3 })]
        [TestCase(Module.A3, new[] { Module.A1toA3 })]
        [TestCase(Module.A5_1, new[] { Module.A5 })]
        [TestCase(Module.A5_3, new[] { Module.A5 })]
        [TestCase(Module.B1_1, new[] { Module.B1, Module.B1toB3, Module.B1toB5, Module.B1toB7 })]
        [TestCase(Module.B1, new[] { Module.B1toB3, Module.B1toB5, Module.B1toB7 })]
        [TestCase(Module.B2, new[] { Module.B1toB3, Module.B1toB5, Module.B1toB7 })]
        [TestCase(Module.B4_2, new[] { Module.B4, Module.B4toB5, Module.B1toB5, Module.B1toB7 })]
        [TestCase(Module.B5, new[] { Module.B4toB5, Module.B1toB5, Module.B1toB7 })]
        [TestCase(Module.B1toB3, new[] { Module.B1toB5, Module.B1toB7 })]
        [TestCase(Module.B1toB5, new[] { Module.B1toB7 })]
        [TestCase(Module.B6, new[] { Module.B1toB7 })]
        [TestCase(Module.B7_3, new[] { Module.B7, Module.B1toB7 })]
        [TestCase(Module.B8_2, new[] { Module.B8 })]
        [TestCase(Module.C1, new[] { Module.C1toC4 })]
        [TestCase(Module.C2, new[] { Module.C1toC4 })]
        [TestCase(Module.C3, new[] { Module.C3toC4, Module.C1toC4 })]
        [TestCase(Module.C4, new[] { Module.C3toC4, Module.C1toC4 })]
        [TestCase(Module.C3toC4, new[] { Module.C1toC4 })]
        [TestCase(Module.D_1, new[] { Module.D })]
        [Description("Tests that PartOfCombinationModules returns every combination a module is part of, including combinations that contain it only indirectly, e.g. B1_1 is part of B1, B1toB3, B1toB5 and B1toB7. Expected values are derived by hand from the module hierarchy.")]
        public void PartOfCombinationModulesIncludesAllParents(Module part, Module[] expectedCombinations)
        {
            var partOfCombinations = Query.PartOfCombinationModules();

            partOfCombinations.Should().ContainKey(part);
            partOfCombinations[part].Should().BeEquivalentTo(expectedCombinations);
        }

        /***************************************************/

        [TestCase(Module.A4)]
        [TestCase(Module.A1toA3)]
        [TestCase(Module.A5)]
        [TestCase(Module.B1toB7)]
        [TestCase(Module.B8)]
        [TestCase(Module.C1toC4)]
        [TestCase(Module.D)]
        [Description("Tests that modules that are not part of any combination, either stand-alone modules or the top level combinations, are not included in the PartOfCombinationModules result.")]
        public void PartOfCombinationModulesExcludesModulesWithoutParents(Module module)
        {
            Query.PartOfCombinationModules().Should().NotContainKey(module);
        }

        /***************************************************/

        [Test]
        [Description("Tests that PartOfCombinationModules is consistent with CombinationModules: for every combination, each of its parts must list that combination as a parent, and the parents of the combination itself must also be parents of the part.")]
        public void PartOfCombinationModulesConsistentWithCombinationModules()
        {
            var combinations = Query.CombinationModules();
            var partOfCombinations = Query.PartOfCombinationModules();

            Assert.Multiple(() =>
            {
                foreach (var combination in combinations)
                {
                    foreach (var part in combination.Value.Select(x => x.Item1))
                    {
                        Assert.That(partOfCombinations.ContainsKey(part), Is.True, $"{part} should be part of {combination.Key}");
                        if (!partOfCombinations.ContainsKey(part))
                            continue;

                        Assert.That(partOfCombinations[part], Does.Contain(combination.Key), $"{part} should list {combination.Key} as a parent");

                        if (partOfCombinations.TryGetValue(combination.Key, out var grandParents))
                            Assert.That(partOfCombinations[part], Is.SupersetOf(grandParents), $"{part} should also list the parents of {combination.Key}");
                    }
                }
            });
        }

        /***************************************************/

        [Test]
        [Description("Tests that the module hierarchy is free of cycles: no module is a part of itself, directly or through other combinations, and no combination lists duplicate parts.")]
        public void CombinationModulesHaveNoCyclesOrDuplicates()
        {
            var combinations = Query.CombinationModules();
            var partOfCombinations = Query.PartOfCombinationModules();

            Assert.Multiple(() =>
            {
                foreach (var combination in combinations)
                {
                    List<Module> parts = combination.Value.Select(x => x.Item1).ToList();
                    Assert.That(parts, Is.Unique, $"{combination.Key} has duplicate parts");
                    Assert.That(parts, Does.Not.Contain(combination.Key), $"{combination.Key} is listed as a part of itself");
                }

                foreach (var item in partOfCombinations)
                {
                    Assert.That(item.Value, Does.Not.Contain(item.Key), $"{item.Key} is indirectly a part of itself");
                    Assert.That(item.Value, Is.Unique, $"{item.Key} has duplicate parents");
                }
            });
        }

        /***************************************************/


    }
}




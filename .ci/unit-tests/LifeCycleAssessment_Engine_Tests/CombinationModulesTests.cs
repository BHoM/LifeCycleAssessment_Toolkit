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

        [TestCase(Module.A1, new[] { Module.A1toA3 })]
        [TestCase(Module.A1toA3, new[] { Module.A1, Module.A2, Module.A3 })]
        [TestCase(Module.A5, new[] { Module.A5_1, Module.A5_2, Module.A5_3, Module.A5_4 })]
        [TestCase(Module.A5_1, new[] { Module.A5 })]
        [TestCase(Module.B1_1, new[] { Module.B1, Module.B1toB3, Module.B1toB5, Module.B1toB7 })]
        [TestCase(Module.B1, new[] { Module.B1_1, Module.B1_2, Module.B1toB3, Module.B1toB5, Module.B1toB7 })]
        [TestCase(Module.B2, new[] { Module.B1toB3, Module.B1toB5, Module.B1toB7 })]
        [TestCase(Module.B1toB3, new[] { Module.B1, Module.B1_1, Module.B1_2, Module.B2, Module.B3, Module.B1toB5, Module.B1toB7 })]
        [TestCase(Module.B4, new[] { Module.B4_1, Module.B4_2, Module.B4toB5, Module.B1toB5, Module.B1toB7 })]
        [TestCase(Module.B4toB5, new[] { Module.B4, Module.B4_1, Module.B4_2, Module.B5, Module.B1toB5, Module.B1toB7 })]
        [TestCase(Module.B1toB5, new[] { Module.B1toB3, Module.B1, Module.B1_1, Module.B1_2, Module.B2, Module.B3, Module.B4toB5, Module.B4, Module.B4_1, Module.B4_2, Module.B5, Module.B1toB7 })]
        [TestCase(Module.B7, new[] { Module.B7_1, Module.B7_2, Module.B7_3, Module.B1toB7 })]
        [TestCase(Module.B1toB7, new[] { Module.B1toB5, Module.B1toB3, Module.B1, Module.B1_1, Module.B1_2, Module.B2, Module.B3, Module.B4toB5, Module.B4, Module.B4_1, Module.B4_2, Module.B5, Module.B6, Module.B7, Module.B7_1, Module.B7_2, Module.B7_3 })]
        [TestCase(Module.B8, new[] { Module.B8_1, Module.B8_2, Module.B8_3 })]
        [TestCase(Module.C3, new[] { Module.C3toC4, Module.C1toC4 })]
        [TestCase(Module.C3toC4, new[] { Module.C3, Module.C4, Module.C1toC4 })]
        [TestCase(Module.C1toC4, new[] { Module.C1, Module.C2, Module.C3, Module.C4, Module.C3toC4 })]
        [TestCase(Module.D, new[] { Module.D_1, Module.D_2 })]
        [TestCase(Module.D_2, new[] { Module.D })]
        [Description("Tests that OverlappingModules returns every module that overlaps with a module, i.e. all combinations the module is part of and all parts it is a combination of, both directly and indirectly. E.g. B1 overlaps with B1_1, B1_2, B1toB3, B1toB5 and B1toB7. Expected values are derived by hand from the module hierarchy.")]
        public void OverlappingModulesIncludesAllPartsAndCombinations(Module module, Module[] expectedOverlaps)
        {
            var overlappingModules = Query.OverlappingModules();

            overlappingModules.Should().ContainKey(module);
            overlappingModules[module].Should().BeEquivalentTo(expectedOverlaps);
        }

        /***************************************************/

        [Test]
        [Description("Tests that OverlappingModules is symmetrical and never lists a module as overlapping with itself: if module A overlaps with module B, then B must also overlap with A, and the overlaps of a module must be unique.")]
        public void OverlappingModulesAreSymmetricalAndUnique()
        {
            var overlappingModules = Query.OverlappingModules();

            Assert.Multiple(() =>
            {
                foreach (var item in overlappingModules)
                {
                    Assert.That(item.Value, Does.Not.Contain(item.Key), $"{item.Key} is listed as overlapping with itself");
                    Assert.That(item.Value, Is.Unique, $"{item.Key} has duplicate overlapping modules");

                    foreach (Module overlap in item.Value)
                    {
                        Assert.That(overlappingModules.ContainsKey(overlap), Is.True, $"{overlap} should have overlapping modules, as it overlaps with {item.Key}");
                        if (overlappingModules.TryGetValue(overlap, out var reverseOverlaps))
                            Assert.That(reverseOverlaps, Does.Contain(item.Key), $"{overlap} should list {item.Key} as overlapping, as {item.Key} lists {overlap}");
                    }
                }
            });
        }

        /***************************************************/

        [Test]
        [Description("Tests that OverlappingModules is consistent with CombinationModules and PartOfCombinationModules: the overlaps of a module must be exactly the union of the combinations it is part of and, if it is a combination, all its parts, directly or indirectly.")]
        public void OverlappingModulesConsistentWithCombinationModules()
        {
            var combinations = Query.CombinationModules();
            var partOfCombinations = Query.PartOfCombinationModules();
            var overlappingModules = Query.OverlappingModules();

            Assert.Multiple(() =>
            {
                foreach (Module module in Enum.GetValues(typeof(Module)).Cast<Module>())
                {
                    HashSet<Module> expected = new HashSet<Module>();
                    if (partOfCombinations.TryGetValue(module, out var parents))
                        expected.UnionWith(parents);

                    CollectAllParts(module, combinations, expected);


                    Assert.That(overlappingModules.ContainsKey(module), Is.True, $"{module} should be included in the overlapping modules");
                    if (overlappingModules.TryGetValue(module, out var overlaps))
                        Assert.That(overlaps, Is.EquivalentTo(expected), $"Overlaps for {module} do not match the combinations and parts of the module");
                }
            });
        }

        /***************************************************/

        [TestCase(Module.A1, new[] { Module.A1toA3 })]
        [TestCase(Module.A5, new[] { Module.A5_1, Module.A5_2, Module.A5_3, Module.A5_4 })]
        [TestCase(Module.B1, new[] { Module.B1_1, Module.B1_2, Module.B1toB3, Module.B1toB5, Module.B1toB7 })]
        [TestCase(Module.B1toB3, new[] { Module.B1, Module.B1_1, Module.B1_2, Module.B2, Module.B3, Module.B1toB5, Module.B1toB7 })]
        [TestCase(Module.C3, new[] { Module.C3toC4, Module.C1toC4 })]
        [TestCase(Module.A0, new Module[0])]
        [TestCase(Module.A4, new Module[0])]
        [Description("Tests that the OverlappingModules overload for a single module returns all combinations the module is part of and all parts it is a combination of, and an empty list, not null, for modules that do not overlap with any other module.")]
        public void OverlappingModulesForSingleModule(Module module, Module[] expectedOverlaps)
        {
            var overlapping = Query.OverlappingModules(module);

            overlapping.Should().NotBeNull();
            overlapping.Should().BeEquivalentTo(expectedOverlaps);
        }

        /***************************************************/

        [Test]
        [Description("Tests that the OverlappingModules overload for a single module gives the same result as the corresponding entry from the dictionary of all overlapping modules, for every module.")]
        public void OverlappingModulesForSingleModuleMatchesDictionary()
        {
            var overlappingModules = Query.OverlappingModules();

            Assert.Multiple(() =>
            {
                foreach (Module module in Enum.GetValues(typeof(Module)).Cast<Module>())
                {
                    IEnumerable<Module> expected = overlappingModules.TryGetValue(module, out var overlaps) ? overlaps : new List<Module>();
                    Assert.That(Query.OverlappingModules(module), Is.EquivalentTo(expected), $"Overlaps for {module} differ from the dictionary result");
                }
            });
        }

        /***************************************************/

        [TestCaseSource(nameof(MultipleModulesOverlapCases))]
        [Description("Tests that the OverlappingModules overload for a list of modules returns the combined overlaps of all the modules, with each overlapping module only listed once, and an empty list, not null, when none of the modules overlap with any other module.")]
        public void OverlappingModulesForMultipleModules(List<Module> modules, List<Module> expectedOverlaps)
        {
            var overlapping = Query.OverlappingModules(modules);

            overlapping.Should().NotBeNull();
            overlapping.Should().BeEquivalentTo(expectedOverlaps);
        }

        /***************************************************/
        /****   Private Methods                         ****/
        /***************************************************/

        private static IEnumerable<TestCaseData> MultipleModulesOverlapCases()
        {
            yield return new TestCaseData(
                new List<Module> { Module.A1, Module.C3 },
                new List<Module> { Module.A1toA3, Module.C3toC4, Module.C1toC4 }
            ).SetArgDisplayNames("Modules from separate hierarchies");

            yield return new TestCaseData(
                new List<Module> { Module.B1_1, Module.B1_2 },
                new List<Module> { Module.B1, Module.B1toB3, Module.B1toB5, Module.B1toB7 }
            ).SetArgDisplayNames("Modules sharing combinations are only listed once");

            yield return new TestCaseData(
                new List<Module> { Module.B6, Module.C4 },
                new List<Module> { Module.B1toB7, Module.C3toC4, Module.C1toC4 }
            ).SetArgDisplayNames("Parts from different combinations");

            yield return new TestCaseData(
                new List<Module> { Module.A0, Module.A4 },
                new List<Module>()
            ).SetArgDisplayNames("Standalone modules only");

            yield return new TestCaseData(
                new List<Module> { Module.A4, Module.A5 },
                new List<Module> { Module.A5_1, Module.A5_2, Module.A5_3, Module.A5_4 }
            ).SetArgDisplayNames("Standalone module and a combination");

            yield return new TestCaseData(
                new List<Module>(),
                new List<Module>()
            ).SetArgDisplayNames("Empty list");
        }

        /***************************************************/

        private static void CollectAllParts(Module module, IReadOnlyDictionary<Module, IReadOnlyList<(Module, bool)>> combinations, HashSet<Module> parts)
        {
            if (combinations.TryGetValue(module, out var directParts))
            {
                foreach (Module part in directParts.Select(x => x.Item1))
                {
                    parts.Add(part);
                    CollectAllParts(part, combinations, parts);
                }
            }
        }

        /***************************************************/

    }
}




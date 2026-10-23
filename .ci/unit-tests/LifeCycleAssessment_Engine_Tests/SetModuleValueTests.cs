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

using BH.Engine.LifeCycleAssessment;
using BH.oM.Base.Debugging;
using BH.oM.LifeCycleAssessment;
using BH.oM.LifeCycleAssessment.MaterialFragments;
using BH.oM.Test.NUnit;
using FluentAssertions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BH.Tests.Engine.LifeCycleAssessment
{
    public class SetModuleValueTests : NUnitTest
    {
        /***************************************************/
        /****   Public Methods                          ****/
        /***************************************************/

        [TestCaseSource(nameof(SingleModuleCases))]
        [Description("Tests SetModuleValue for a dictionary. Checks that the value is set in place, that exactly the expected overlapping combinations and parts are removed (and nothing at all when removeOverlaps is false), and that all other values are untouched. A null removeOverlaps means the default is used, which is to remove overlaps. Expected removed modules are derived by hand from the module hierarchy.")]
        public void SetModuleValueDictionary(Module module, Module[] expectedRemoved, bool? removeOverlaps)
        {
            Dictionary<Module, double> values = AllModuleValues();

            if (removeOverlaps.HasValue)
                values.SetModuleValue(module, 9, removeOverlaps.Value);
            else
                values.SetModuleValue(module, 9);

            AssertResult(values, AllModules(), new List<Module> { module }, new List<double> { 9 }, expectedRemoved);
        }

        /***************************************************/

        [TestCaseSource(nameof(MultipleModuleCases))]
        [Description("Tests SetModuleValues for a dictionary. Checks that the values are set in place, that exactly the expected overlapping combinations and parts are removed (and nothing at all when removeOverlaps is false), that modules set are never removed by each other, and that all other values are untouched. The result must be the same regardless of the order of the modules. A null removeOverlaps means the default is used, which is to remove overlaps. Expected removed modules are derived by hand from the module hierarchy.")]
        public void SetModuleValuesDictionary(Module[] existing, Module[] modules, Module[] expectedRemoved, bool? removeOverlaps)
        {
            List<Module> existingModules = existing?.ToList() ?? AllModules();
            List<Module> moduleList = modules.ToList();
            List<double> valueList = moduleList.Select((x, i) => 10.0 + i).ToList();

            //Run in both the order given and the reverse, as the order should not matter
            for (int i = 0; i < 2; i++)
            {
                Dictionary<Module, double> values = existingModules.ToDictionary(x => x, x => 1.0);

                if (removeOverlaps.HasValue)
                    values.SetModuleValues(moduleList, valueList, removeOverlaps.Value);
                else
                    values.SetModuleValues(moduleList, valueList);

                AssertResult(values, existingModules, moduleList, valueList, expectedRemoved);

                moduleList.Reverse();
                valueList.Reverse();
            }
        }

        /***************************************************/

        [TestCase(true)]
        [TestCase(false)]
        [Description("Tests that SetModuleValue for an IEnvironmentalMetric updates the indicators of the metric in place, in the same way as for a dictionary. Setting A1toA3 removes A1, A2 and A3 only when removeOverlaps is true.")]
        public void SetModuleValueMetric(bool removeOverlaps)
        {
            Dictionary<Module, double> indicators = AllModuleValues();
            IEnvironmentalMetric metric = new ClimateChangeTotalMetric { Indicators = indicators };

            metric.SetModuleValue(Module.A1toA3, 9, removeOverlaps);

            metric.Indicators.Should().BeSameAs(indicators, "the metric should be updated in place");
            AssertResult(indicators, AllModules(), new List<Module> { Module.A1toA3 }, new List<double> { 9 },
                removeOverlaps ? new[] { Module.A1, Module.A2, Module.A3 } : new Module[] { });
        }

        /***************************************************/

        [TestCase(true)]
        [TestCase(false)]
        [Description("Tests that SetModuleValues for an IEnvironmentalMetric updates the indicators of the metric in place, in the same way as for a dictionary. Setting C3 and C3toC4 keeps both, and removes C1toC4 and C4 only when removeOverlaps is true.")]
        public void SetModuleValuesMetric(bool removeOverlaps)
        {
            Dictionary<Module, double> indicators = AllModuleValues();
            IEnvironmentalMetric metric = new ClimateChangeTotalMetric { Indicators = indicators };

            metric.SetModuleValues(new List<Module> { Module.C3, Module.C3toC4 }, new List<double> { 2, 3 }, removeOverlaps);

            metric.Indicators.Should().BeSameAs(indicators, "the metric should be updated in place");
            AssertResult(indicators, AllModules(), new List<Module> { Module.C3, Module.C3toC4 }, new List<double> { 2, 3 },
                removeOverlaps ? new[] { Module.C1toC4, Module.C4 } : new Module[] { });
        }

        /***************************************************/

        [Test]
        [Description("Tests that SetModuleValue for a null dictionary records a single error and does not throw.")]
        public void SetModuleValueDictionaryInvalidInputs()
        {
            BH.Engine.Base.Compute.ClearCurrentEvents();
            Dictionary<Module, double> values = null;

            Action act = () => values.SetModuleValue(Module.A1, 1);
            act.Should().NotThrow();

            ErrorCount().Should().Be(1);
        }

        /***************************************************/

        [TestCase(true, TestName = "Null metric")]
        [TestCase(false, TestName = "Null indicators")]
        [Description("Tests that SetModuleValue for a null metric, or a metric with null indicators, records a single error and does not throw.")]
        public void SetModuleValueMetricInvalidInputs(bool nullMetric)
        {
            BH.Engine.Base.Compute.ClearCurrentEvents();
            IEnvironmentalMetric metric = nullMetric ? null : new ClimateChangeTotalMetric { Indicators = null };

            Action act = () => metric.SetModuleValue(Module.A1, 1);
            act.Should().NotThrow();

            ErrorCount().Should().Be(1);
        }

        /***************************************************/

        [TestCaseSource(nameof(InvalidInputs))]
        [Description("Tests that SetModuleValues for a dictionary records a single error, leaves the values unchanged and does not throw for invalid inputs: null dictionary, null modules or values, mismatched number of modules and values, and duplicate modules.")]
        public void SetModuleValuesDictionaryInvalidInputs(bool nullDictionary, List<Module> modules, List<double> valuesToSet)
        {
            BH.Engine.Base.Compute.ClearCurrentEvents();
            Dictionary<Module, double> values = nullDictionary ? null : AllModuleValues();

            Action act = () => values.SetModuleValues(modules, valuesToSet);
            act.Should().NotThrow();

            ErrorCount().Should().Be(1);
            if (values != null)
                values.Should().BeEquivalentTo(AllModuleValues(), "the values should be unchanged");
        }

        /***************************************************/

        [TestCase(true, TestName = "Null metric")]
        [TestCase(false, TestName = "Null indicators")]
        [Description("Tests that SetModuleValues for a null metric, or a metric with null indicators, records a single error and does not throw.")]
        public void SetModuleValuesMetricInvalidInputs(bool nullMetric)
        {
            BH.Engine.Base.Compute.ClearCurrentEvents();
            IEnvironmentalMetric metric = nullMetric ? null : new ClimateChangeTotalMetric { Indicators = null };

            Action act = () => metric.SetModuleValues(new List<Module> { Module.A1 }, new List<double> { 1 });
            act.Should().NotThrow();

            ErrorCount().Should().Be(1);
        }

        /***************************************************/
        /****   Private Methods                         ****/
        /***************************************************/

        private static IEnumerable<TestCaseData> SingleModuleCases()
        {
            Module[] none = new Module[] { };

            yield return SingleCase("Stand-alone module removes nothing", Module.A4, none);
            yield return SingleCase("Part removes its combination", Module.A1, new[] { Module.A1toA3 });
            yield return SingleCase("Nested part removes all combinations above it", Module.B1_1, new[] { Module.B1, Module.B1toB3, Module.B1toB5, Module.B1toB7 });
            yield return SingleCase("Combination removes its parts", Module.A5, new[] { Module.A5_1, Module.A5_2, Module.A5_3, Module.A5_4 });
            yield return SingleCase("Combination removes parts of its parts", Module.C1toC4, new[] { Module.C1, Module.C2, Module.C3, Module.C4, Module.C3toC4 });
            yield return SingleCase("Combination in the middle removes both its parts and the combinations above", Module.B1, new[] { Module.B1_1, Module.B1_2, Module.B1toB3, Module.B1toB5, Module.B1toB7 });
            yield return SingleCase("Top combination removes all parts recursively", Module.B1toB7, new[] { Module.B1toB5, Module.B1toB3, Module.B4toB5, Module.B1, Module.B1_1, Module.B1_2, Module.B2, Module.B3, Module.B4, Module.B4_1, Module.B4_2, Module.B5, Module.B6, Module.B7, Module.B7_1, Module.B7_2, Module.B7_3 });
            yield return SingleCase("No removal when turned off", Module.B1toB7, none, false);
            yield return SingleCase("Removal is the default", Module.A1toA3, new[] { Module.A1, Module.A2, Module.A3 }, null);
        }

        /***************************************************/

        private static IEnumerable<TestCaseData> MultipleModuleCases()
        {
            Module[] none = new Module[] { };

            //Modules set must never be removed by each other
            yield return MultipleCase("Part and its direct combination", new[] { Module.C3, Module.C3toC4 }, new[] { Module.C1toC4, Module.C4 });
            yield return MultipleCase("Part and its indirect combination", new[] { Module.C3, Module.C1toC4 }, new[] { Module.C1, Module.C2, Module.C4, Module.C3toC4 });
            yield return MultipleCase("Whole chain set", new[] { Module.C1toC4, Module.C3toC4, Module.C3 }, new[] { Module.C1, Module.C2, Module.C4 });
            yield return MultipleCase("Deeply nested part and top combination", new[] { Module.B1_1, Module.B1toB7 }, new[] { Module.B1toB5, Module.B1toB3, Module.B4toB5, Module.B1, Module.B1_2, Module.B2, Module.B3, Module.B4, Module.B4_1, Module.B4_2, Module.B5, Module.B6, Module.B7, Module.B7_1, Module.B7_2, Module.B7_3 });
            yield return MultipleCase("Two separate branches", new[] { Module.A1toA3, Module.B1_1 }, new[] { Module.A1, Module.A2, Module.A3, Module.B1, Module.B1toB3, Module.B1toB5, Module.B1toB7 });
            yield return MultipleCase("Two parts of the same combination", new[] { Module.A5_2, Module.A5_3 }, new[] { Module.A5 });

            //Removal turned off, and default
            yield return MultipleCase("No removal when turned off", new[] { Module.A1toA3, Module.B1_1 }, none, false);
            yield return MultipleCase("Removal is the default", new[] { Module.A1toA3, Module.B1_1 }, new[] { Module.A1, Module.A2, Module.A3, Module.B1, Module.B1toB3, Module.B1toB5, Module.B1toB7 }, null);

            //Only modules that exist can be removed
            yield return MultipleCase("Only existing modules removed", new[] { Module.B1_1, Module.A5_1 }, new[] { Module.B1toB3 }, true, new[] { Module.A1, Module.B1toB3 });
        }

        /***************************************************/

        private static IEnumerable<TestCaseData> InvalidInputs()
        {
            List<Module> modules = new List<Module> { Module.A1, Module.A2 };
            List<double> values = new List<double> { 1, 2 };

            yield return InvalidCase("Null dictionary", true, modules, values);
            yield return InvalidCase("Null modules", false, null, values);
            yield return InvalidCase("Null values", false, modules, null);
            yield return InvalidCase("Different number of modules and values", false, modules, new List<double> { 1 });
            yield return InvalidCase("Duplicate modules", false, new List<Module> { Module.A1, Module.A2, Module.A1 }, new List<double> { 1, 2, 3 });
        }

        /***************************************************/

        private static TestCaseData SingleCase(string name, Module module, Module[] expectedRemoved, bool? removeOverlaps = true)
        {
            return new TestCaseData(module, expectedRemoved, removeOverlaps).SetArgDisplayNames(name);
        }

        /***************************************************/

        private static TestCaseData MultipleCase(string name, Module[] modules, Module[] expectedRemoved, bool? removeOverlaps = true, Module[] existing = null)
        {
            return new TestCaseData(existing, modules, expectedRemoved, removeOverlaps).SetArgDisplayNames(name);
        }

        /***************************************************/

        private static TestCaseData InvalidCase(string name, bool nullDictionary, List<Module> modules, List<double> values)
        {
            return new TestCaseData(nullDictionary, modules, values).SetArgDisplayNames(name);
        }

        /***************************************************/

        private static List<Module> AllModules()
        {
            return Enum.GetValues(typeof(Module)).Cast<Module>().ToList();
        }

        /***************************************************/

        private static Dictionary<Module, double> AllModuleValues()
        {
            return AllModules().ToDictionary(x => x, x => 1.0);
        }

        /***************************************************/

        private static void AssertResult(Dictionary<Module, double> values, List<Module> existing, List<Module> modulesSet, List<double> valuesSet, Module[] expectedRemoved)
        {
            Assert.Multiple(() =>
            {
                values.Keys.Should().BeEquivalentTo(existing.Union(modulesSet).Except(expectedRemoved), "exactly the expected modules should be left");

                foreach (KeyValuePair<Module, double> item in values)
                {
                    int index = modulesSet.IndexOf(item.Key);
                    double expected = index >= 0 ? valuesSet[index] : 1.0;  //Set value for modules set, untouched for all others
                    item.Value.Should().Be(expected, $"the value for {item.Key} should be correct");
                }
            });
        }

        /***************************************************/

        private static int ErrorCount()
        {
            return BH.Engine.Base.Query.CurrentEvents().Count(x => x.Type == EventType.Error);
        }

        /***************************************************/
    }
}

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
using BH.oM.LifeCycleAssessment;
using BH.oM.LifeCycleAssessment.MaterialFragments;
using BH.oM.Test.NUnit;
using NUnit.Framework;
using System.Collections.Generic;

namespace BH.Tests.Engine.LifeCycleAssessment
{
    public class ModuleValueTests : NUnitTest
    {
        /***************************************************/
        /****   Public Methods                          ****/
        /***************************************************/

        [TestCaseSource(nameof(ModuleValueCases))]
        [Description("Tests the ModuleValue extension method for module dictionaries, checking how combination modules are computed from their parts. Parts flagged as required in CombinationModules must be present when requireAllRequiredSubParts is true, while optional parts are only added when present. When requireAllRequiredSubParts is false, any available part is summed. NaN is expected when the value can not be found.")]
        public void ModuleValue(Dictionary<Module, double> moduleData, Module module, bool requireAllRequiredSubParts, double expected)
        {
            double value = moduleData.ModuleValue(module, requireAllRequiredSubParts);

            Assert.That(value, Is.EqualTo(expected).Within(1e-12));
        }

        /***************************************************/

        [TestCase(true)]
        [TestCase(false)]
        [Description("Tests that ModuleValue returns NaN, rather than throwing, when the module data is null.")]
        public void ModuleValueNullData(bool requireAllRequiredSubParts)
        {
            Dictionary<Module, double> moduleData = null;

            Assert.That(moduleData.ModuleValue(Module.A1toA3, requireAllRequiredSubParts), Is.NaN);
        }

        /***************************************************/

        [TestCase(true)]
        [TestCase(false)]
        [Description("Tests that ModuleValue returns NaN for a combination made up of only optional parts when none of the parts are present, instead of returning a value of zero.")]
        public void ModuleValueOnlyOptionalPartsNonePresent(bool requireAllRequiredSubParts)
        {
            Dictionary<Module, double> moduleData = new Dictionary<Module, double> { { Module.A1, 1 } };

            Assert.That(moduleData.ModuleValue(Module.B8, requireAllRequiredSubParts), Is.NaN);
        }

        /***************************************************/

        [Test]
        [Description("Tests that requireAllRequiredSubParts set to false is applied to nested combinations as well, i.e. B1toB3 should sum B1_1, B2 and B3 even though B1_2, and with it a complete B1, is missing.")]
        public void ModuleValueNotAllRequiredAppliesToNestedCombinations()
        {
            Dictionary<Module, double> moduleData = new Dictionary<Module, double> { { Module.B1_1, 1 }, { Module.B2, 3 }, { Module.B3, 4 } };

            Assert.That(moduleData.ModuleValue(Module.B1toB3, false), Is.EqualTo(8).Within(1e-12));
        }

        /***************************************************/

        [TestCase(true)]
        [TestCase(false)]
        [Description("Tests that ModuleValue for an EPD gets the value from the metric of the provided metric type, and not from other metrics on the EPD, including values computed from parts for combination modules.")]
        public void ModuleValueEpd(bool requireAllRequiredSubParts)
        {
            EnvironmentalProductDeclaration epd = new EnvironmentalProductDeclaration
            {
                EnvironmentalMetrics = new List<IEnvironmentalMetric>
                {
                    new AcidificationMetric { Indicators = new Dictionary<Module, double> { { Module.A1, 100 }, { Module.A2, 200 }, { Module.A3, 300 }, { Module.A4, 400 } } },
                    new ClimateChangeTotalMetric { Indicators = new Dictionary<Module, double> { { Module.A1, 1 }, { Module.A2, 2 }, { Module.A3, 3 }, { Module.A4, 4 } } }
                }
            };

            Assert.Multiple(() =>
            {
                Assert.That(epd.ModuleValue(MetricType.ClimateChangeTotal, Module.A4, requireAllRequiredSubParts), Is.EqualTo(4).Within(1e-12), "Direct value");
                Assert.That(epd.ModuleValue(MetricType.ClimateChangeTotal, Module.A1toA3, requireAllRequiredSubParts), Is.EqualTo(6).Within(1e-12), "Combination from parts");
                Assert.That(epd.ModuleValue(MetricType.Acidification, Module.A1toA3, requireAllRequiredSubParts), Is.EqualTo(600).Within(1e-12), "Combination from parts for other metric");
            });
        }

        /***************************************************/

        [Test]
        [Description("Tests that ModuleValue for an EPD returns NaN, rather than throwing, when the EPD is null, has no metrics, or has no metric of the provided metric type.")]
        public void ModuleValueEpdMissingMetric()
        {
            EnvironmentalProductDeclaration epd = new EnvironmentalProductDeclaration
            {
                EnvironmentalMetrics = new List<IEnvironmentalMetric> { new AcidificationMetric { Indicators = new Dictionary<Module, double> { { Module.A1, 1 } } } }
            };
            EnvironmentalProductDeclaration noMetrics = new EnvironmentalProductDeclaration { EnvironmentalMetrics = null };
            EnvironmentalProductDeclaration nullEpd = null;

            Assert.Multiple(() =>
            {
                Assert.That(epd.ModuleValue(MetricType.ClimateChangeTotal, Module.A1), Is.NaN, "Metric type not on EPD");
                Assert.That(noMetrics.ModuleValue(MetricType.ClimateChangeTotal, Module.A1), Is.NaN, "Null metrics");
                Assert.That(nullEpd.ModuleValue(MetricType.ClimateChangeTotal, Module.A1), Is.NaN, "Null EPD");
            });
        }

        /***************************************************/

        [Test]
        [Description("Tests that ModuleValue for an EPD returns the values set with SetModuleValues for the same EPD, metric type and modules.")]
        public void ModuleValueEpdMatchesSetModuleValues()
        {
            EnvironmentalProductDeclaration epd = new EnvironmentalProductDeclaration();

            epd.SetModuleValues(MetricType.ClimateChangeTotal, new List<Module> { Module.A1toA3, Module.B1_1 }, new List<double> { 2, 3 });

            Assert.Multiple(() =>
            {
                Assert.That(epd.ModuleValue(MetricType.ClimateChangeTotal, Module.A1toA3), Is.EqualTo(2).Within(1e-12));
                Assert.That(epd.ModuleValue(MetricType.ClimateChangeTotal, Module.B1_1), Is.EqualTo(3).Within(1e-12));
            });
        }

        /***************************************************/
        /****   Private Methods                         ****/
        /***************************************************/

        private static IEnumerable<TestCaseData> ModuleValueCases()
        {
            double nan = double.NaN;

            // Explicitly set values
            yield return Case("Direct value", new() { { Module.A1, 5 } }, Module.A1, true, 5);
            yield return Case("Explicit combination value wins over parts", new() { { Module.A1toA3, 10 }, { Module.A1, 1 }, { Module.A2, 2 }, { Module.A3, 3 } }, Module.A1toA3, true, 10);
            yield return Case("Missing non-combination module", new(), Module.A1, true, nan);
            yield return Case("Missing non-combination module, not all required", new(), Module.A1, false, nan);

            // A1toA3: all parts required
            yield return Case("A1toA3 from all parts", new() { { Module.A1, 1 }, { Module.A2, 2 }, { Module.A3, 3 } }, Module.A1toA3, true, 6);
            yield return Case("A1toA3 missing required part", new() { { Module.A1, 1 }, { Module.A2, 2 } }, Module.A1toA3, true, nan);
            yield return Case("A1toA3 missing part, not all required", new() { { Module.A1, 1 }, { Module.A2, 2 } }, Module.A1toA3, false, 3);
            yield return Case("A1toA3 with no parts", new(), Module.A1toA3, true, nan);
            yield return Case("A1toA3 with no parts, not all required", new(), Module.A1toA3, false, nan);

            // A5: A5_2 and A5_3 required, A5_1 and A5_4 optional
            yield return Case("A5 from required parts only", new() { { Module.A5_2, 2 }, { Module.A5_3, 3 } }, Module.A5, true, 5);
            yield return Case("A5 from all parts", new() { { Module.A5_1, 1 }, { Module.A5_2, 2 }, { Module.A5_3, 3 }, { Module.A5_4, 4 } }, Module.A5, true, 10);
            yield return Case("A5 missing required A5_3", new() { { Module.A5_1, 1 }, { Module.A5_2, 2 } }, Module.A5, true, nan);
            yield return Case("A5 missing required A5_2", new() { { Module.A5_3, 3 }, { Module.A5_4, 4 } }, Module.A5, true, nan);
            yield return Case("A5 with only optional parts", new() { { Module.A5_1, 1 }, { Module.A5_4, 4 } }, Module.A5, true, nan);
            yield return Case("A5 missing required A5_3, not all required", new() { { Module.A5_1, 1 }, { Module.A5_2, 2 } }, Module.A5, false, 3);
            yield return Case("A5 with only optional parts, not all required", new() { { Module.A5_1, 1 }, { Module.A5_4, 4 } }, Module.A5, false, 5);

            // B8: all parts optional
            yield return Case("B8 with one optional part", new() { { Module.B8_1, 2 } }, Module.B8, true, 2);
            yield return Case("B8 with all optional parts", new() { { Module.B8_1, 1 }, { Module.B8_2, 2 }, { Module.B8_3, 3 } }, Module.B8, true, 6);

            // Nested combinations
            yield return Case("B1toB3 from all nested parts", new() { { Module.B1_1, 1 }, { Module.B1_2, 2 }, { Module.B2, 3 }, { Module.B3, 4 } }, Module.B1toB3, true, 10);
            yield return Case("B1toB3 with required nested part missing", new() { { Module.B1_1, 1 }, { Module.B2, 3 }, { Module.B3, 4 } }, Module.B1toB3, true, nan);
            yield return Case("B1toB3 using explicit B1", new() { { Module.B1, 3 }, { Module.B2, 3 }, { Module.B3, 4 } }, Module.B1toB3, true, 10);
            yield return Case("C1toC4 from nested parts", new() { { Module.C1, 1 }, { Module.C2, 2 }, { Module.C3, 3 }, { Module.C4, 4 } }, Module.C1toC4, true, 10);
            yield return Case("C1toC4 using explicit C3toC4", new() { { Module.C1, 1 }, { Module.C2, 2 }, { Module.C3toC4, 7 } }, Module.C1toC4, true, 10);
            yield return Case("C1toC4 missing C4", new() { { Module.C1, 1 }, { Module.C2, 2 }, { Module.C3, 3 } }, Module.C1toC4, true, nan);
        }

        /***************************************************/

        private static TestCaseData Case(string name, Dictionary<Module, double> moduleData, Module module, bool requireAllRequiredSubParts, double expected)
        {
            return new TestCaseData(moduleData, module, requireAllRequiredSubParts, expected).SetArgDisplayNames($"{name}");
        }

        /***************************************************/
    }
}

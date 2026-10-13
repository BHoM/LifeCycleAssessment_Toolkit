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
using BH.oM.LifeCycleAssessment.Results;
using BH.oM.Test.NUnit;
using FluentAssertions;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace BH.Tests.Engine.LifeCycleAssessment
{
    public class MaterialResultTests : NUnitTest
    {
        /***************************************************/
        /****   Public Methods                          ****/
        /***************************************************/

        [TestCaseSource(nameof(CombinationCases))]
        [Description("Tests that Create.MaterialResult adds combination modules as the sum of their parts, but only when all parts flagged as required in CombinationModules are present and at least one part exists. Combinations already provided must be left untouched and must not cause other combinations to be added, and the dictionary provided must not be modified.")]
        public void MaterialResultAddsCombinationModules(Dictionary<Module, double> indicators, Dictionary<Module, double> expected)
        {
            Dictionary<Module, double> inputCopy = new Dictionary<Module, double>(indicators);

            MaterialResult result = Create.MaterialResult("Material", "EPD", MetricType.ClimateChangeTotal, indicators);

            result.Should().NotBeNull();
            result.Indicators.Should().BeEquivalentTo(expected);
            indicators.Should().BeEquivalentTo(inputCopy, "the input should not be modified");
        }

        /***************************************************/

        [Test]
        [Description("Tests that Create.MaterialResult stores the names and returns a result of the type matching the provided MetricType.")]
        public void MaterialResultIdentifiers()
        {
            MaterialResult result = Create.MaterialResult("Material", "EPD", MetricType.ClimateChangeTotal, new Dictionary<Module, double> { { Module.A1, 1 } });

            result.MaterialName.Should().Be("Material");
            result.EnvironmentalProductDeclarationName.Should().Be("EPD");
            result.IMetricType().Should().Be(MetricType.ClimateChangeTotal);
        }

        /***************************************************/
        /****   Private Methods                         ****/
        /***************************************************/

        private static IEnumerable<TestCaseData> CombinationCases()
        {
            // A1toA3: all parts required
            yield return Case("A1toA3 added from all parts",
                new() { { Module.A1, 1 }, { Module.A2, 2 }, { Module.A3, 3 } },
                new() { { Module.A1, 1 }, { Module.A2, 2 }, { Module.A3, 3 }, { Module.A1toA3, 6 } });

            yield return Case("A1toA3 not added when required part missing",
                new() { { Module.A1, 1 }, { Module.A2, 2 } },
                new() { { Module.A1, 1 }, { Module.A2, 2 } });

            yield return Case("A1toA3 already present is kept",
                new() { { Module.A1toA3, 10 }, { Module.A1, 1 }, { Module.A2, 2 }, { Module.A3, 3 } },
                new() { { Module.A1toA3, 10 }, { Module.A1, 1 }, { Module.A2, 2 }, { Module.A3, 3 } });

            yield return Case("Only A1toA3 provided adds nothing",
                new() { { Module.A1toA3, 6 } },
                new() { { Module.A1toA3, 6 } });

            // A5: A5_2 and A5_3 required, A5_1 and A5_4 optional
            yield return Case("A5 added from required parts only",
                new() { { Module.A5_2, 2 }, { Module.A5_3, 3 } },
                new() { { Module.A5_2, 2 }, { Module.A5_3, 3 }, { Module.A5, 5 } });

            yield return Case("A5 added from all parts",
                new() { { Module.A5_1, 1 }, { Module.A5_2, 2 }, { Module.A5_3, 3 }, { Module.A5_4, 4 } },
                new() { { Module.A5_1, 1 }, { Module.A5_2, 2 }, { Module.A5_3, 3 }, { Module.A5_4, 4 }, { Module.A5, 10 } });

            yield return Case("A5 not added when required A5_3 missing",
                new() { { Module.A5_1, 1 }, { Module.A5_2, 2 }, { Module.A5_4, 4 } },
                new() { { Module.A5_1, 1 }, { Module.A5_2, 2 }, { Module.A5_4, 4 } });

            yield return Case("A5 not added when only optional parts present",
                new() { { Module.A5_1, 1 }, { Module.A5_4, 4 } },
                new() { { Module.A5_1, 1 }, { Module.A5_4, 4 } });

            // B8: all parts optional
            yield return Case("B8 added from a single optional part",
                new() { { Module.B8_2, 2 } },
                new() { { Module.B8_2, 2 }, { Module.B8, 2 } });

            yield return Case("B8 added from all optional parts",
                new() { { Module.B8_1, 1 }, { Module.B8_2, 2 }, { Module.B8_3, 3 } },
                new() { { Module.B8_1, 1 }, { Module.B8_2, 2 }, { Module.B8_3, 3 }, { Module.B8, 6 } });

            // Nested combinations
            yield return Case("B1 and B1toB3 added from sub parts",
                new() { { Module.B1_1, 1 }, { Module.B1_2, 2 }, { Module.B2, 3 }, { Module.B3, 4 } },
                new() { { Module.B1_1, 1 }, { Module.B1_2, 2 }, { Module.B2, 3 }, { Module.B3, 4 }, { Module.B1, 3 }, { Module.B1toB3, 10 } });

            yield return Case("B1toB3 not added when B1_2 missing",
                new() { { Module.B1_1, 1 }, { Module.B2, 3 }, { Module.B3, 4 } },
                new() { { Module.B1_1, 1 }, { Module.B2, 3 }, { Module.B3, 4 } });

            yield return Case("All B modules cascade up to B1toB7",
                new()
                {
                    { Module.B1_1, 1 }, { Module.B1_2, 1 }, { Module.B2, 1 }, { Module.B3, 1 },
                    { Module.B4_1, 1 }, { Module.B4_2, 1 }, { Module.B5, 1 }, { Module.B6, 1 },
                    { Module.B7_1, 1 }, { Module.B7_2, 1 }, { Module.B7_3, 1 }
                },
                new()
                {
                    { Module.B1_1, 1 }, { Module.B1_2, 1 }, { Module.B2, 1 }, { Module.B3, 1 },
                    { Module.B4_1, 1 }, { Module.B4_2, 1 }, { Module.B5, 1 }, { Module.B6, 1 },
                    { Module.B7_1, 1 }, { Module.B7_2, 1 }, { Module.B7_3, 1 },
                    { Module.B1, 2 }, { Module.B1toB3, 4 }, { Module.B4, 2 }, { Module.B4toB5, 3 }, { Module.B1toB5, 7 }, { Module.B7, 3 }, { Module.B1toB7, 11 }
                });

            yield return Case("C1toC4 added from nested parts",
                new() { { Module.C1, 1 }, { Module.C2, 2 }, { Module.C3, 3 }, { Module.C4, 4 } },
                new() { { Module.C1, 1 }, { Module.C2, 2 }, { Module.C3, 3 }, { Module.C4, 4 }, { Module.C3toC4, 7 }, { Module.C1toC4, 10 } });

            yield return Case("C1toC4 added using provided C3toC4",
                new() { { Module.C1, 1 }, { Module.C2, 2 }, { Module.C3toC4, 7 } },
                new() { { Module.C1, 1 }, { Module.C2, 2 }, { Module.C3toC4, 7 }, { Module.C1toC4, 10 } });

            yield return Case("C3toC4 added, but not C1toC4, when C1 and C2 are missing",
                new() { { Module.C3, 3 }, { Module.C4, 4 } },
                new() { { Module.C3, 3 }, { Module.C4, 4 }, { Module.C3toC4, 7 } });

            yield return Case("Empty input gives empty result",
                new(),
                new());
        }

        /***************************************************/

        private static TestCaseData Case(string name, Dictionary<Module, double> indicators, Dictionary<Module, double> expected)
        {
            return new TestCaseData(indicators, expected).SetName($"MaterialResult: {name}");
        }

        /***************************************************/
    }
}

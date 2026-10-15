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
using System.Collections.Generic;
using System.Linq;
using LCAModule = BH.oM.LifeCycleAssessment.Module;

namespace BH.Tests.Engine.LifeCycleAssessment
{
    public class CreateClimateChangeEnvironmentalProductDeclarationTests : NUnitTest
    {
        /***************************************************/
        /****   Public Methods                          ****/
        /***************************************************/

        [Test]
        [Description("Tests that the ClimateChangeEnvironmentalProductDeclaration create method assigns the provided properties and creates the five climate change metrics.")]
        public void ClimateChangeEPDProperties()
        {
            EnvironmentalProductDeclaration epd = BH.Engine.LifeCycleAssessment.Create.ClimateChangeEnvironmentalProductDeclaration(
                new List<LCAModule> { LCAModule.A1toA3 },
                new List<double> { 1 },
                new List<double> { 2 },
                new List<double> { 3 },
                QuantityType.Volume,
                EPDType.Sector,
                "Concrete");

            epd.Should().NotBeNull();
            epd.Name.Should().Be("Concrete");
            epd.Type.Should().Be(EPDType.Sector);
            epd.QuantityType.Should().Be(QuantityType.Volume);
            epd.EnvironmentalMetrics.Select(x => x.GetType()).Should().BeEquivalentTo(new[]
            {
                typeof(ClimateChangeFossilMetric),
                typeof(ClimateChangeBiogenicMetric),
                typeof(ClimateChangeLandUseMetric),
                typeof(ClimateChangeTotalMetric),
                typeof(ClimateChangeTotalNoBiogenicMetric),
            });
        }

        /***************************************************/

        [Test]
        [Description("Tests that the ClimateChangeEnvironmentalProductDeclaration create method uses Undefined EPDType and an empty name by default.")]
        public void ClimateChangeEPDDefaults()
        {
            EnvironmentalProductDeclaration epd = BH.Engine.LifeCycleAssessment.Create.ClimateChangeEnvironmentalProductDeclaration(
                new List<LCAModule> { LCAModule.A1toA3 },
                new List<double> { 1 },
                new List<double> { 2 },
                new List<double> { 3 },
                QuantityType.Mass);

            epd.Type.Should().Be(EPDType.Undefined);
            epd.Name.Should().BeEmpty();
        }

        /***************************************************/

        [Test]
        [Description("Tests that the ClimateChangeEnvironmentalProductDeclaration create method stores fossil, biogenic and LULUC values per module and computes the totals correctly.")]
        public void ClimateChangeEPDValues()
        {
            List<LCAModule> modules = new List<LCAModule> { LCAModule.A1toA3, LCAModule.C3toC4, LCAModule.D };
            List<double> fossil = new List<double> { 10, 1, -2 };
            List<double> biogenic = new List<double> { -4, 0.5, 0 };
            List<double> luluc = new List<double> { 0.25, 0.1, 0.05 };

            EnvironmentalProductDeclaration epd = BH.Engine.LifeCycleAssessment.Create.ClimateChangeEnvironmentalProductDeclaration(modules, fossil, biogenic, luluc, QuantityType.Mass);

            double tol = 1e-12;
            for (int i = 0; i < modules.Count; i++)
            {
                LCAModule m = modules[i];
                MetricIndicators(epd, MetricType.ClimateChangeFossil)[m].Should().BeApproximately(fossil[i], tol);
                MetricIndicators(epd, MetricType.ClimateChangeBiogenic)[m].Should().BeApproximately(biogenic[i], tol);
                MetricIndicators(epd, MetricType.ClimateChangeLandUse)[m].Should().BeApproximately(luluc[i], tol);
                MetricIndicators(epd, MetricType.ClimateChangeTotal)[m].Should().BeApproximately(fossil[i] + biogenic[i] + luluc[i], tol);
                MetricIndicators(epd, MetricType.ClimateChangeTotalNoBiogenic)[m].Should().BeApproximately(fossil[i] + luluc[i], tol);
            }

            foreach (IEnvironmentalMetric metric in epd.EnvironmentalMetrics)
                metric.Indicators.Keys.Should().BeEquivalentTo(modules);
        }

        /***************************************************/

        [Test]
        [Description("Tests that the ClimateChangeEnvironmentalProductDeclaration create method returns an EPD with empty metrics for empty module lists.")]
        public void ClimateChangeEPDNoModules()
        {
            EnvironmentalProductDeclaration epd = BH.Engine.LifeCycleAssessment.Create.ClimateChangeEnvironmentalProductDeclaration(
                new List<LCAModule>(), new List<double>(), new List<double>(), new List<double>(), QuantityType.Mass);

            epd.Should().NotBeNull();
            epd.EnvironmentalMetrics.Should().HaveCount(5);
            epd.EnvironmentalMetrics.Should().OnlyContain(x => x.Indicators.Count == 0);
        }

        /***************************************************/

        [TestCaseSource(nameof(FailingClimateChangeEPDInputs))]
        [Description("Tests that the ClimateChangeEnvironmentalProductDeclaration create method returns null and records an error for invalid inputs, such as null lists, mismatched list lengths, duplicate modules and NaN factors.")]
        public void ClimateChangeEPDFailingCases(List<LCAModule> modules, List<double> fossil, List<double> biogenic, List<double> luluc)
        {
            BH.Engine.Base.Compute.ClearCurrentEvents();

            EnvironmentalProductDeclaration epd = BH.Engine.LifeCycleAssessment.Create.ClimateChangeEnvironmentalProductDeclaration(modules, fossil, biogenic, luluc, QuantityType.Mass);

            epd.Should().BeNull();
            ErrorCount().Should().Be(1);
        }

        /***************************************************/
        /****   Test Case Sources                       ****/
        /***************************************************/

        private static IEnumerable<TestCaseData> FailingClimateChangeEPDInputs()
        {
            List<LCAModule> oneModule = new List<LCAModule> { LCAModule.A1toA3 };
            List<double> one = new List<double> { 1 };
            List<double> two = new List<double> { 1, 2 };
            List<double> nan = new List<double> { double.NaN };
            List<double> empty = new List<double>();

            yield return new TestCaseData(null, one, one, one).SetArgDisplayNames("Failing case: Null modules");
            yield return new TestCaseData(oneModule, null, one, one).SetArgDisplayNames("Failing case: Null fossil");
            yield return new TestCaseData(oneModule, one, null, one).SetArgDisplayNames("Failing case: Null biogenic");
            yield return new TestCaseData(oneModule, one, one, null).SetArgDisplayNames("Failing case: Null LULUC");

            yield return new TestCaseData(oneModule, empty, one, one).SetArgDisplayNames("Failing case: Fewer fossil than modules");
            yield return new TestCaseData(oneModule, one, empty, one).SetArgDisplayNames("Failing case: Fewer biogenic than modules");
            yield return new TestCaseData(oneModule, one, one, empty).SetArgDisplayNames("Failing case: Fewer LULUC than modules");
            yield return new TestCaseData(oneModule, two, one, one).SetArgDisplayNames("Failing case: More fossil than modules");
            yield return new TestCaseData(oneModule, one, two, one).SetArgDisplayNames("Failing case: More biogenic than modules");
            yield return new TestCaseData(oneModule, one, one, two).SetArgDisplayNames("Failing case: More LULUC than modules");

            yield return new TestCaseData(new List<LCAModule> { LCAModule.A1toA3, LCAModule.A1toA3 }, two, two, two).SetArgDisplayNames("Failing case: Duplicate modules");

            yield return new TestCaseData(oneModule, nan, one, one).SetArgDisplayNames("Failing case: NaN fossil");
            yield return new TestCaseData(oneModule, one, nan, one).SetArgDisplayNames("Failing case: NaN biogenic");
            yield return new TestCaseData(oneModule, one, one, nan).SetArgDisplayNames("Failing case: NaN LULUC");
        }

        /***************************************************/
        /****   Private Methods                         ****/
        /***************************************************/

        private static Dictionary<LCAModule, double> MetricIndicators(EnvironmentalProductDeclaration epd, MetricType metricType)
        {
            return epd.EnvironmentalMetrics.Single(x => x.IMetricType() == metricType).Indicators;
        }

        /***************************************************/

        private static int ErrorCount()
        {
            return BH.Engine.Base.Query.CurrentEvents().Count(x => x.Type == EventType.Error);
        }

        /***************************************************/
    }
}


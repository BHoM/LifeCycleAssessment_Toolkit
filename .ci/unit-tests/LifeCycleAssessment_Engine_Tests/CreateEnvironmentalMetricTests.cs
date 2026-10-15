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
    public class CreateEnvironmentalMetricTests : NUnitTest
    {
        /***************************************************/
        /****   Public Methods                          ****/
        /***************************************************/

        [TestCaseSource(nameof(DefinedMetricTypes))]
        [Description("Tests that all overloads of the EnvironmentalMetric create method return a metric of the type matching the provided MetricType, with the provided indicators stored on it and an empty name by default.")]
        public void EnvironmentalMetricMatchesMetricType(MetricType metricType)
        {
            Dictionary<Module, double> indicators = new Dictionary<Module, double>
            {
                { Module.A1toA3, 1.5 },
                { Module.C3toC4, 2.5 },
            };

            IEnvironmentalMetric typeOnly = BH.Engine.LifeCycleAssessment.Create.EnvironmentalMetric(metricType);
            IEnvironmentalMetric fromDictionary = BH.Engine.LifeCycleAssessment.Create.EnvironmentalMetric(metricType, indicators);
            IEnvironmentalMetric fromLists = BH.Engine.LifeCycleAssessment.Create.EnvironmentalMetric(metricType, indicators.Keys.ToList(), indicators.Values.ToList());

            typeOnly.Indicators.Should().BeEmpty();
            fromDictionary.Indicators.Should().BeEquivalentTo(indicators);
            fromLists.Indicators.Should().BeEquivalentTo(indicators);

            foreach (IEnvironmentalMetric metric in new[] { typeOnly, fromDictionary, fromLists })
            {
                metric.Should().NotBeNull();
                metric.IMetricType().Should().Be(metricType);
                metric.Name.Should().Be("");
            }
        }

        /***************************************************/

        [Test]
        [Description("Tests that all overloads of the EnvironmentalMetric create method set the provided name, and fall back to an empty name for null.")]
        public void EnvironmentalMetricName()
        {
            Dictionary<Module, double> indicators = new Dictionary<Module, double> { { Module.A1toA3, 1 } };

            IEnvironmentalMetric[] metrics = new[]
            {
                BH.Engine.LifeCycleAssessment.Create.EnvironmentalMetric(MetricType.Acidification, "My metric"),
                BH.Engine.LifeCycleAssessment.Create.EnvironmentalMetric(MetricType.Acidification, indicators, "My metric"),
                BH.Engine.LifeCycleAssessment.Create.EnvironmentalMetric(MetricType.Acidification, indicators.Keys.ToList(), indicators.Values.ToList(), "My metric"),
            };

            metrics.Select(x => x.Name).Should().OnlyContain(x => x == "My metric");

            //Cast needed to resolve the overload, as null is also a valid indicators input
            BH.Engine.LifeCycleAssessment.Create.EnvironmentalMetric(MetricType.Acidification, (string)null).Name.Should().Be("");
        }

        /***************************************************/

        [Test]
        [Description("Tests that the EnvironmentalMetric create method creates the expected concrete type.")]
        public void EnvironmentalMetricConcreteType()
        {
            IEnvironmentalMetric metric = BH.Engine.LifeCycleAssessment.Create.EnvironmentalMetric(MetricType.Acidification, new Dictionary<Module, double>());
            metric.Should().BeOfType<AcidificationMetric>();
        }

        /***************************************************/

        [Test]
        [Description("Tests that the EnvironmentalMetric create method does not share the provided indicators dictionary with the created metric.")]
        public void EnvironmentalMetricDoesNotShareIndicators()
        {
            Dictionary<Module, double> indicators = new Dictionary<Module, double> { { Module.A1toA3, 1 } };

            IEnvironmentalMetric metric = BH.Engine.LifeCycleAssessment.Create.EnvironmentalMetric(MetricType.Acidification, indicators);

            indicators[Module.A4] = 5;
            metric.Indicators.Should().NotContainKey(Module.A4);

            metric.Indicators[Module.C1] = 7;
            indicators.Should().NotContainKey(Module.C1);
        }

        /***************************************************/

        [TestCaseSource(nameof(InvalidInputs))]
        [Description("Tests that the EnvironmentalMetric create method returns null and records a single error for invalid inputs, without throwing.")]
        public void EnvironmentalMetricInvalidInputs(Func<IEnvironmentalMetric> create)
        {
            BH.Engine.Base.Compute.ClearCurrentEvents();

            IEnvironmentalMetric metric = null;

            create.Invoking(x => metric = x()).Should().NotThrow();
            metric.Should().BeNull();
            ErrorCount().Should().Be(1);
        }

        /***************************************************/
        /****   Private Methods                         ****/
        /***************************************************/

        private static IEnumerable<MetricType> DefinedMetricTypes()
        {
            return Enum.GetValues(typeof(MetricType)).Cast<MetricType>().Where(x => x != MetricType.Undefined);
        }

        /***************************************************/

        private static IEnumerable<TestCaseData> InvalidInputs()
        {
            Dictionary<Module, double> indicators = new Dictionary<Module, double> { { Module.A1toA3, 1 } };
            List<Module> modules = new List<Module> { Module.A1toA3 };
            List<double> values = new List<double> { 1 };

            //Cast needed to resolve the overload, as null is also a valid name
            yield return Case("Undefined type", () => BH.Engine.LifeCycleAssessment.Create.EnvironmentalMetric(MetricType.Undefined));
            yield return Case("Undefined type with indicators", () => BH.Engine.LifeCycleAssessment.Create.EnvironmentalMetric(MetricType.Undefined, indicators));
            yield return Case("Undefined type with modules and indicators", () => BH.Engine.LifeCycleAssessment.Create.EnvironmentalMetric(MetricType.Undefined, modules, values));
            yield return Case("Null indicators", () => BH.Engine.LifeCycleAssessment.Create.EnvironmentalMetric(MetricType.Acidification, (Dictionary<Module, double>)null));
            yield return Case("Null modules", () => BH.Engine.LifeCycleAssessment.Create.EnvironmentalMetric(MetricType.Acidification, null, values));
            yield return Case("Null indicators with modules", () => BH.Engine.LifeCycleAssessment.Create.EnvironmentalMetric(MetricType.Acidification, modules, (List<double>)null));
            yield return Case("Modules and indicators of different lengths", () => BH.Engine.LifeCycleAssessment.Create.EnvironmentalMetric(MetricType.Acidification, new List<Module> { Module.A1toA3, Module.C3toC4 }, values));
        }

        /***************************************************/

        private static TestCaseData Case(string name, Func<IEnvironmentalMetric> create)
        {
            return new TestCaseData(create).SetArgDisplayNames(name);
        }

        /***************************************************/

        private static int ErrorCount()
        {
            return BH.Engine.Base.Query.CurrentEvents().Count(x => x.Type == EventType.Error);
        }

        /***************************************************/
    }
}


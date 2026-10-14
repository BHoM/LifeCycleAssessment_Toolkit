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
        [Description("Tests that the EnvironmentalMetric create method returns a metric of the type matching the provided MetricType, with the provided indicators stored on it.")]
        public void EnvironmentalMetricMatchesMetricType(MetricType metricType)
        {
            Dictionary<Module, double> indicators = new Dictionary<Module, double>
            {
                { Module.A1toA3, 1.5 },
                { Module.C3toC4, 2.5 },
            };

            IEnvironmentalMetric metric = BH.Engine.LifeCycleAssessment.Create.EnvironmentalMetric(metricType, indicators);

            metric.Should().NotBeNull();
            metric.IMetricType().Should().Be(metricType);
            metric.Indicators.Should().BeEquivalentTo(indicators);
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

        [Test]
        [Description("Tests that the EnvironmentalMetric create method returns a metric with no indicators for an empty dictionary.")]
        public void EnvironmentalMetricEmptyIndicators()
        {
            IEnvironmentalMetric metric = BH.Engine.LifeCycleAssessment.Create.EnvironmentalMetric(MetricType.OzoneDepletion, new Dictionary<Module, double>());

            metric.Should().NotBeNull();
            metric.Indicators.Should().BeEmpty();
        }

        /***************************************************/

        [Test]
        [Description("Tests that the EnvironmentalMetric create method returns null and records an error for null indicators.")]
        public void EnvironmentalMetricNullIndicators()
        {
            BH.Engine.Base.Compute.ClearCurrentEvents();

            IEnvironmentalMetric metric = BH.Engine.LifeCycleAssessment.Create.EnvironmentalMetric(MetricType.Acidification, null);

            metric.Should().BeNull();
            ErrorCount().Should().Be(1);
        }

        /***************************************************/

        [Test]
        [Description("Tests that the EnvironmentalMetric create method returns null and records an error for the Undefined MetricType, which has no matching metric.")]
        public void EnvironmentalMetricUndefinedType()
        {
            BH.Engine.Base.Compute.ClearCurrentEvents();

            IEnvironmentalMetric metric = BH.Engine.LifeCycleAssessment.Create.EnvironmentalMetric(MetricType.Undefined, new Dictionary<Module, double>());

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
        private static int ErrorCount()
        {
            return BH.Engine.Base.Query.CurrentEvents().Count(x => x.Type == EventType.Error);
        }

        /***************************************************/
    }
}


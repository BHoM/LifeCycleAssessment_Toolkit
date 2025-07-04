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
using BH.oM.LifeCycleAssessment.Configs;
using BH.oM.LifeCycleAssessment.MaterialFragments;
using BH.oM.LifeCycleAssessment.MaterialFragments.Transport;
using BH.oM.LifeCycleAssessment.Results;
using BH.oM.Physical.Constructions;
using BH.oM.Physical.Elements;
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
    public class EvaluateIStructE : NUnitTest
    {
        /***************************************************/
        /****   Public Methods                          ****/
        /***************************************************/


        [TestCaseSource(typeof(DataSource), nameof(DataSource.DummyMetrics), new object[] { 1.234, 0.1432, true })]
        public void EvaluateIStructEMetricTest(IEnvironmentalMetric metric)
        {
            IStructEEvaluationConfig config = DummyConfig();
            double quantity = 50;
            double mass = 12;

            MaterialResult result = Query.EnvironmentalResults(metric, "", "", quantity, DummyConfig(), mass);
            ValidateMetricAndResult(metric, result, quantity, config.ProjectCost, config.FloorArea, config.TotalWeight, config.A5CarbonFactor, config.C1CarbonFactor, mass);

        }

        [TestCaseSource(typeof(DataSource), nameof(DataSource.DummyEPDs), new object[] { 1.2321, 0.0002, true })]
        public void EvaluateIStructEEPDTest(EnvironmentalProductDeclaration epd)
        {
            IStructEEvaluationConfig config = DummyConfig();
            double eval = 32.22;
            double mass = 23.3;
            List<MaterialResult> materialResults = Query.EnvironmentalResults(epd, eval, "", null, config, mass);

            if (epd.QuantityType != QuantityType.Mass)
            {
                Assert.That(materialResults, Is.Empty, "Should not return results for quantity types other than Mass");
                return;
            }

            Assert.That(materialResults, Is.Not.Empty, "No results generated");
            for (int i = 0; i < materialResults.Count; i++)
            {
                ValidateMetricAndResult(epd.EnvironmentalMetrics[i], materialResults[i], eval, config.ProjectCost, config.FloorArea, config.TotalWeight, config.A5CarbonFactor, config.C1CarbonFactor, mass, epd.Name);
            }
        }


        /***************************************************/

        [TestCaseSource(typeof(DataSource), nameof(DataSource.DummyCombinedLCAFactors), new object[] { 1.2321, 0.0002, true })]
        public void EvaluateIStructECombinedFactorsTest(CombinedLifeCycleAssessmentFactors combinedFactors)
        {
            IStructEEvaluationConfig config = DummyConfig();
            double eval = 32.22;
            double mass = 22.4;
            List<MaterialResult> materialResults = Query.EnvironmentalResults(combinedFactors, eval,mass, "", null, config, mass);
            if (combinedFactors.EnvironmentalProductDeclaration == null && combinedFactors.A4TransportFactors == null && combinedFactors.C2TransportFactors == null)
            {
                Assert.That(materialResults, Is.Empty, "Should nto give results for Combined factors with all nulls.");
                return;
            }
            Assert.That(materialResults, Is.Not.Empty, "No results generated");
            for (int i = 0; i < materialResults.Count; i++)
            {
                ValidateMetricAndResult(combinedFactors.EnvironmentalProductDeclaration?.EnvironmentalMetrics[i], materialResults[i], eval, config.ProjectCost, config.FloorArea, config.TotalWeight, config.A5CarbonFactor, config.C1CarbonFactor, mass, combinedFactors.Name, "", combinedFactors.A4TransportFactors, combinedFactors.C2TransportFactors);
            }
        }


        /***************************************************/

        [TestCaseSource(typeof(DataSource), nameof(DataSource.DummyTakeoffAndTemplates), new object[] { 1.2321, 0.0002, true })]
        public void EvaluateIStructETakeoff(GeneralMaterialTakeoff takeoff, List<Material> templates, bool containEpds)
        {
            IStructEEvaluationConfig config = DummyConfig();

            List<MaterialResult> materialResults = Query.EnvironmentalResults(takeoff, templates, true, null, config);

            Assert.That(materialResults, Is.Not.Empty, "No results generated");

            foreach (MaterialResult result in materialResults)
            {
                templates.Should().Contain(x => x.Name == result.MaterialName);
                Material mat = templates.First(x => x.Name == result.MaterialName);
                mat.Properties.Should().Contain(x => x.Name == result.EnvironmentalProductDeclarationName);


                takeoff.MaterialTakeoffItems.Should().Contain(x => x.Material.Name == result.MaterialName);
                TakeoffItem takeoffItem = takeoff.MaterialTakeoffItems.First(x => x.Material.Name == result.MaterialName);
                double eval = takeoffItem.Volume;

                if (containEpds)
                {
                    IMaterialProperties prop = mat.Properties.First(x => x.Name == result.EnvironmentalProductDeclarationName);
                    prop.Should().BeOfType<EnvironmentalProductDeclaration>();

                    EnvironmentalProductDeclaration epd = prop as EnvironmentalProductDeclaration;
                    epd.EnvironmentalMetrics.Should().Contain(x => x.IMetricType() == result.IMetricType());
                    IEnvironmentalMetric metric = epd.EnvironmentalMetrics.First(x => x.IMetricType() == result.IMetricType());

                    ValidateMetricAndResult(metric, result, eval, config.ProjectCost, config.FloorArea, config.TotalWeight, config.A5CarbonFactor, config.C1CarbonFactor, takeoffItem.Mass, epd.Name, mat.Name);
                }
                else
                {
                    IMaterialProperties prop = mat.Properties.First(x => x.Name == result.EnvironmentalProductDeclarationName);
                    prop.Should().BeOfType<CombinedLifeCycleAssessmentFactors>();

                    CombinedLifeCycleAssessmentFactors combinedFactors = prop as CombinedLifeCycleAssessmentFactors;
                    combinedFactors.EnvironmentalProductDeclaration.EnvironmentalMetrics.Should().Contain(x => x.IMetricType() == result.IMetricType());
                    IEnvironmentalMetric metric = combinedFactors.EnvironmentalProductDeclaration.EnvironmentalMetrics.First(x => x.IMetricType() == result.IMetricType());
                    ValidateMetricAndResult(metric, result, eval, config.ProjectCost, config.FloorArea, config.TotalWeight, config.A5CarbonFactor, config.C1CarbonFactor, takeoffItem.Mass, combinedFactors.Name, mat.Name, combinedFactors.A4TransportFactors, combinedFactors.C2TransportFactors);


                }

            }

        }

        [TestCaseSource(typeof(DataSource), nameof(DataSource.DummyElementsAndTemplates), new object[] { 1.2321, 0.0002, true })]
        public void EvaluateElement(Wall element, double area, List<Material> templates)
        {
            IStructEEvaluationConfig config = DummyConfig();
            List<IElementResult<MaterialResult>> elementResults = Query.EnvironmentalResults(element, templates,true, null, config);

            Assert.That(elementResults, Is.Not.Empty);

            Construction construction = element.Construction as Construction;

            foreach (IElementResult<MaterialResult> elementResult in elementResults)
            {
                foreach (var indicator in elementResult.Indicators)
                {
                    indicator.Value.Should().BeApproximately(elementResult.MaterialResults.SelectMany(x => x.Indicators.Where(y => y.Key == indicator.Key).Select(y => y.Value)).Sum(), 1e-12, indicator.Key + " element result should be equal to sum of parts");
                }

                foreach (MaterialResult result in elementResult.MaterialResults)
                {
                    result.IMetricType().Should().Be(elementResult.IMetricType());
                    templates.Should().Contain(x => x.Name == result.MaterialName);
                    Material mat = templates.First(x => x.Name == result.MaterialName);
                    mat.Properties.Should().Contain(x => x.Name == result.EnvironmentalProductDeclarationName);
                    IMaterialProperties prop = mat.Properties.First(x => x.Name == result.EnvironmentalProductDeclarationName);
                    prop.Should().BeOfType<EnvironmentalProductDeclaration>();

                    EnvironmentalProductDeclaration epd = prop as EnvironmentalProductDeclaration;
                    epd.EnvironmentalMetrics.Should().Contain(x => x.IMetricType() == result.IMetricType());
                    IEnvironmentalMetric metric = epd.EnvironmentalMetrics.First(x => x.IMetricType() == result.IMetricType());

                    construction.Layers.Should().Contain(x => x.Material.Name == result.MaterialName);
                    Layer layer = construction.Layers.First(x => x.Material.Name == result.MaterialName);
                    double eval = layer.Thickness * area;
                    double mass = eval * layer.Material.Density;

                    ValidateMetricAndResult(metric, result, eval, config.ProjectCost, config.FloorArea, config.TotalWeight, config.A5CarbonFactor, config.C1CarbonFactor, mass, epd.Name, mat.Name);
                }
            }
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static void ValidateMetricAndResult(IEnvironmentalMetric metric, MaterialResult result, double quantity, double projectCost, double floorArea, double totalWeight, double a5CarbonFactor, double c1CarbonFactor, double mass, string epdName = "", string materialName = "", ITransportFactors a4Factor = null, ITransportFactors c2Factor = null)
        {
            double tolerance = 1e-6;

            string initialMessage;
            if (metric != null)
                initialMessage = $"Evaluating {metric.GetType().Name} comparing against {result.GetType().Name}";
            else
                initialMessage = $"Checking result of type {result.GetType().Name}";

            if (!string.IsNullOrEmpty(epdName))
            {
                result.EnvironmentalProductDeclarationName.Should().Be(epdName, initialMessage);
            }

            if (!string.IsNullOrEmpty(materialName))
            {
                result.MaterialName.Should().Be(materialName, initialMessage);
            }

            if (metric != null)
                result.IMetricType().Should().Be(metric.IMetricType(), initialMessage);

            List<Module> evaluatedModules = metric?.Indicators?.Keys.ToList() ?? new List<Module>();

            if (a4Factor != null)
                evaluatedModules.Add(Module.A4);
            if (c2Factor != null)
                evaluatedModules.Add(Module.C2);
            List<MetricType> specialMetrics = new List<MetricType> { MetricType.ClimateChangeTotal, MetricType.ClimateChangeTotalNoBiogenic, MetricType.ClimateChangeFossil };
            bool specialTreatment = metric == null ? false : specialMetrics.Contains(result.IMetricType());

            if(specialTreatment)
            {
                evaluatedModules.Add(Module.A5);
                evaluatedModules.Add(Module.C1);
            }
            
            evaluatedModules = evaluatedModules.OrderBy(x => x).Distinct().ToList();

            Assert.Multiple(() =>
            {
                foreach (Module module in evaluatedModules)
                {
                    Console.WriteLine($"{result.IMetricType()}: {module}");
                    Assert.That(result.Indicators, Contains.Key(module), $"{module} missing while {metric}");
                    string message = $"Module: {module.ToString()} {initialMessage}";
                    if (specialTreatment && module == oM.LifeCycleAssessment.Module.C1)
                    {
                        Assert.That(result.Indicators[module], Is.EqualTo(mass / totalWeight * floorArea * c1CarbonFactor).Within(tolerance), $"{module} failed while {message}");
                    }
                    else if (specialTreatment && module == oM.LifeCycleAssessment.Module.A5)
                    {
                        Assert.That(result.Indicators[module], Is.EqualTo(metric.Indicators[oM.LifeCycleAssessment.Module.A5_3] * quantity + mass / totalWeight * a5CarbonFactor * projectCost).Within(tolerance), $"{module} failed while {message}");
                    }
                    else if (specialTreatment && module == oM.LifeCycleAssessment.Module.C1toC4)
                    {
                        Assert.That(result.Indicators[module], Is.EqualTo(result.Indicators[oM.LifeCycleAssessment.Module.C1] + result.Indicators[oM.LifeCycleAssessment.Module.C2] + result.Indicators[oM.LifeCycleAssessment.Module.C3] + result.Indicators[oM.LifeCycleAssessment.Module.C4]).Within(tolerance), $"{module} failed while {message}");
                    }
                    else if (a4Factor != null && module == oM.LifeCycleAssessment.Module.A4)
                    {
                        Assert.That(result.Indicators[module], Is.EqualTo(TransportImpact(a4Factor, result.IMetricType(), mass)).Within(tolerance), $"{module} failed while {message}");
                    }
                    else if (c2Factor != null && module == oM.LifeCycleAssessment.Module.C2)
                    {
                        Assert.That(result.Indicators[module], Is.EqualTo(TransportImpact(c2Factor, result.IMetricType(), mass)).Within(tolerance), $"{module} failed while {message}");
                    }
                    else
                    {
                        Assert.That(result.Indicators[module], Is.EqualTo(metric.Indicators[module] * quantity).Within(tolerance), $"{module} failed while {message}");
                    }
                }
            });
        }

        /***************************************************/
        private static double TransportImpact(ITransportFactors transport, MetricType metricType, double mass)
        {
            if (transport is FullTransportScenario fullScenario)
            {
                IEnvironmentalFactor factor = fullScenario.EnvironmentalFactors.FirstOrDefault(x => x.IMetricType() == metricType);
                if (factor == null)
                    return double.NaN;

                return factor.Value * mass;
            }
            if (transport is SingleTransportModeImpact singel)
            {
                IEnvironmentalFactor factor = singel.VehicleEmissions.EnvironmentalFactors.FirstOrDefault(x => x.IMetricType() == metricType);
                if (factor == null)
                    return double.NaN;

                return factor.Value * mass * singel.DistanceTraveled * (1 + singel.VehicleEmissions.ReturnTripFactor);
            }
            else if (transport is DistanceTransportModeScenario distance)
            {
                return distance.SingleTransportModeImpacts.Sum(x => TransportImpact(x, metricType, mass));
            }
            return double.NaN;
        }

        /***************************************************/
        private static IStructEEvaluationConfig DummyConfig()
        {
            return new IStructEEvaluationConfig
            {
                TotalWeight = 2000000,
                FloorArea = 500,
                ProjectCost = 1000000
            };
        }

        /***************************************************/

    }
}



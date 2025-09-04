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
using BH.oM.Dimensional;
using BH.oM.LifeCycleAssessment;
using BH.oM.LifeCycleAssessment.MaterialFragments;
using BH.oM.LifeCycleAssessment.MaterialFragments.Construction;
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
    public class Evaluate : NUnitTest
    {
        /***************************************************/
        /****   Public Methods                          ****/
        /***************************************************/

        [TestCaseSource(typeof(DataSource), nameof(DataSource.DummyMetrics), new object[] { 1.234, 0.1432, false })]
        public void EvaluateMetricTest(IEnvironmentalMetric metric)
        {
            double quantity = 4;
            MaterialResult result = Query.EnvironmentalResults(metric, "", "", quantity);
            ValidateMetricAndResult(metric, result, quantity);
        }

        /***************************************************/

        [TestCaseSource(typeof(DataSource), nameof(DataSource.DummyEPDs), new object[] { 1.2321, 0.0002, false })]
        public void EvaluatEPDTest(EnvironmentalProductDeclaration epd)
        {
            double eval = 32.22;
            List<MaterialResult> materialResults = Query.EnvironmentalResults(epd, eval);
            for (int i = 0; i < materialResults.Count; i++)
            {
                ValidateMetricAndResult(epd.EnvironmentalMetrics[i], materialResults[i], eval, epd.Name);
            }
        }

        /***************************************************/

        [TestCaseSource(typeof(DataSource), nameof(DataSource.DummyCombinedLCAFactors), new object[] { 1.2321, 0.0002, false })]
        public void EvaluatCombinedFactorsTest(CombinedLifeCycleAssessmentFactors factors)
        {
            double eval = 32.22;
            double mass = 22.42;
            List<MaterialResult> materialResults = Query.EnvironmentalResults(factors, eval, mass);
            for (int i = 0; i < materialResults.Count; i++)
            {
                ValidateMetricAndResult(factors.EnvironmentalProductDeclaration?.EnvironmentalMetrics[i], materialResults[i], eval, factors.Name, "", factors.A4TransportFactors, factors.C2TransportFactors, mass, factors.A5ConstructionEmissions);
            }
        }

        /***************************************************/

        [TestCaseSource(typeof(DataSource), nameof(DataSource.DummyTakeoffAndTemplates), new object[] { 1.2321, 0.0002, false })]
        public void EvaluatTakeoff(GeneralMaterialTakeoff takeoff, List<Material> templates, bool containEpds)
        {
            List<MaterialResult> materialResults = Query.EnvironmentalResults(takeoff, templates);

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

                    ValidateMetricAndResult(metric, result, eval, epd.Name, mat.Name);
                }
                else
                {
                    IMaterialProperties prop = mat.Properties.First(x => x.Name == result.EnvironmentalProductDeclarationName);
                    prop.Should().BeOfType<CombinedLifeCycleAssessmentFactors>();

                    CombinedLifeCycleAssessmentFactors combinedFactors = prop as CombinedLifeCycleAssessmentFactors;
                    combinedFactors.EnvironmentalProductDeclaration.EnvironmentalMetrics.Should().Contain(x => x.IMetricType() == result.IMetricType());
                    IEnvironmentalMetric metric = combinedFactors.EnvironmentalProductDeclaration.EnvironmentalMetrics.First(x => x.IMetricType() == result.IMetricType());

                    ValidateMetricAndResult(metric, result, eval, combinedFactors.Name, mat.Name, combinedFactors.A4TransportFactors, combinedFactors.C2TransportFactors, takeoffItem.Mass, combinedFactors.A5ConstructionEmissions);
                }
            }

        }

        /***************************************************/

        [TestCaseSource(typeof(DataSource), nameof(DataSource.DummyTakeoffAndTemplates), new object[] { 1.2321, 0.0002, false })]
        public void EvaluatTakeoffWithFilters(GeneralMaterialTakeoff takeoff, List<Material> templates, bool containEpds)
        {           
            List<MetricType> metricFilter = new List<MetricType> { MetricType.AbioticDepletionFossilResources, MetricType.ClimateChangeBiogenic, MetricType.EutrophicationTerrestrial };

            List<MaterialResult> materialResults = Query.EnvironmentalResults(takeoff, templates, true, metricFilter);

            materialResults.Should().AllSatisfy(x => metricFilter.Contains(x.IMetricType()));

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

                    ValidateMetricAndResult(metric, result, eval, epd.Name, mat.Name);
                }
                else
                {
                    IMaterialProperties prop = mat.Properties.First(x => x.Name == result.EnvironmentalProductDeclarationName);
                    prop.Should().BeOfType<CombinedLifeCycleAssessmentFactors>();

                    CombinedLifeCycleAssessmentFactors combinedFactors = prop as CombinedLifeCycleAssessmentFactors;
                    combinedFactors.EnvironmentalProductDeclaration.EnvironmentalMetrics.Should().Contain(x => x.IMetricType() == result.IMetricType());
                    IEnvironmentalMetric metric = combinedFactors.EnvironmentalProductDeclaration.EnvironmentalMetrics.First(x => x.IMetricType() == result.IMetricType());

                    ValidateMetricAndResult(metric, result, eval, combinedFactors.Name, mat.Name, combinedFactors.A4TransportFactors, combinedFactors.C2TransportFactors, takeoffItem.Mass, combinedFactors.A5ConstructionEmissions);
                }
            }

        }

        /***************************************************/

        [TestCaseSource(typeof(DataSource), nameof(DataSource.DummyElementsAndTemplates), new object[] { 1.2321, 0.0002, false })]
        public void EvaluateElement(Wall element, double area, List<Material> templates)
        {
            List<IElementResult<MaterialResult>> elementResults = Query.EnvironmentalResults(element, templates);

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
                    double eval = construction.Layers.First(x => x.Material.Name == result.MaterialName).Thickness * area;

                    ValidateMetricAndResult(metric, result, eval, epd.Name, mat.Name);
                }
            }
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        public static void ValidateResult(MaterialResult result, IEnvironmentalMetric environmentalMetric, double quantity, string epdName = "", string materialName = "", Dictionary<Module, double> specialCases = null)
        {
            var combinationModules = Query.CombinationModules();
            double tolerance = 1e-6;

            string message;
            if (environmentalMetric != null)
                message = $"Evaluating {environmentalMetric.GetType().Name} comparing against {result.GetType().Name}";
            else
                message = $"Checking result of type {result.GetType().Name}";

            if (!string.IsNullOrEmpty(epdName))
            {
                result.EnvironmentalProductDeclarationName.Should().Be(epdName, message);
            }

            if (!string.IsNullOrEmpty(materialName))
            {
                result.MaterialName.Should().Be(materialName, message);
            }

            if (environmentalMetric != null)
                result.IMetricType().Should().Be(environmentalMetric.IMetricType(), message);

            Assert.Multiple(() =>
            {
                foreach (var resultItem in result.Indicators.OrderBy(x => x.Key))
                {

                    Module module = resultItem.Key;
                    double value = resultItem.Value;
                    Console.WriteLine($"{result.IMetricType()}: {module}");
                    bool isCombination = combinationModules.ContainsKey(module);

                    if (isCombination)
                    {
                        double sum = 0;
                        var parts = combinationModules[module];
                        if (parts.Any(x => result.Indicators.ContainsKey(x.Item1))) //If contains any of the parts, then it should be the sum of the parts
                        {
                            foreach (var part in combinationModules[module])
                            {
                                sum += result.Indicators.ContainsKey(part.Item1) ? result.Indicators[part.Item1] : 0;
                            }
                            Assert.That(value, Is.EqualTo(sum).Within(tolerance), $"{module} should be sum of parts");
                        }
                    }

                    if (specialCases != null && specialCases.ContainsKey(module))
                    {
                        Assert.That(value, Is.EqualTo(specialCases[module]).Within(tolerance), $"{module} should match special case");
                    }
                    else
                    {
                        if (!isCombination)
                            Assert.That(environmentalMetric.Indicators, Contains.Key(module), $"{module} missing in metric");

                        if (environmentalMetric.Indicators.TryGetValue(module, out double metricValue))
                        {
                            Assert.That(metricValue, Is.Not.NaN, $"{module} in metric is NaN");
                            double expected = environmentalMetric.Indicators[module] * quantity;
                            Assert.That(value, Is.EqualTo(expected).Within(tolerance), $"{module} should match metric times quantity");
                        }
                    }
                }
            });
        }

        /***************************************************/

        private static void ValidateMetricAndResult(IEnvironmentalMetric metric, MaterialResult result, double quantity, string epdName = "", string materialName = "", ITransportFactors a4Factor= null, ITransportFactors c2Factor = null, double mass = 0, ConstructionEmissions a53Factors = null)
        {
            double tolerance = 1e-6;

            Dictionary<Module, double> specialCases = new Dictionary<Module, double>();
            if (a4Factor != null)
                specialCases[Module.A4] = TransportImpact(a4Factor, result.IMetricType(), mass);
            if (c2Factor != null)
                specialCases[Module.C2] = TransportImpact(c2Factor, result.IMetricType(), mass);
            if(a53Factors != null)
                specialCases[Module.A5_3] = WasteImpact(a53Factors, result);

            ValidateResult(result, metric, quantity, epdName, materialName, specialCases);

        }

        /***************************************************/

        public static double TransportImpact(ITransportFactors transport, MetricType metricType, double mass)
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

        public static double WasteImpact(ConstructionEmissions constructionEmissions, MaterialResult result)
        {
            double expected = 0;
            if (result.Indicators.TryGetValue(Module.A1toA3, out double a1toa3) && result.Indicators.TryGetValue(Module.A4, out double a4) && result.Indicators.TryGetValue(Module.C3toC4, out double c3c4))
                expected = a1toa3 + a4 + c3c4;
            else
                return 0;

            if (!constructionEmissions.ResuedOnSite)
            {
                if (result.Indicators.ContainsKey(Module.C2))
                    expected += result.Indicators[Module.C2];
                else
                    return 0;
            }

            expected *= (1 / (1 - constructionEmissions.WasteRate.Rate) - 1);
            return expected;
        }

        /***************************************************/
    }
}



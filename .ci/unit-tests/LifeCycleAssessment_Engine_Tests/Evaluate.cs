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

using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoBogus;
using BH.oM.LifeCycleAssessment.MaterialFragments;
using BH.oM.LifeCycleAssessment.Results;
using BH.Engine.LifeCycleAssessment;
using FluentAssertions;
using BH.oM.LifeCycleAssessment;
using BH.oM.Physical.Materials;
using BH.oM.Dimensional;
using BH.oM.Physical.Elements;
using BH.oM.Physical.Constructions;
using BH.oM.LifeCycleAssessment.MaterialFragments.Transport;

namespace BH.Tests.Engine.LifeCycleAssessment
{
    public class Evaluate
    {
        /***************************************************/
        /****   Public Methods                          ****/
        /***************************************************/

        [TestCaseSource(typeof(DataSource), nameof(DataSource.DummyMetrics), new object[] { 1.234, 0.1432, false })]
        public void EvaluateMetricTest(IEnvironmentalMetricFactors metric)
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
                ValidateMetricAndResult(epd.EnvironmentalFactors[i], materialResults[i], eval, epd.Name);
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
                ValidateMetricAndResult(factors.BaseFactors?.EnvironmentalFactors[i], materialResults[i], eval, factors.Name, "", factors.A4TransportFactors, factors.C2TransportFactors, mass);
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
                    epd.EnvironmentalFactors.Should().Contain(x => x.IMetricType() == result.IMetricType());
                    IEnvironmentalMetricFactors metric = epd.EnvironmentalFactors.First(x => x.IMetricType() == result.IMetricType());

                    ValidateMetricAndResult(metric, result, eval, epd.Name, mat.Name);
                }
                else
                {
                    IMaterialProperties prop = mat.Properties.First(x => x.Name == result.EnvironmentalProductDeclarationName);
                    prop.Should().BeOfType<CombinedLifeCycleAssessmentFactors>();

                    CombinedLifeCycleAssessmentFactors combinedFactors = prop as CombinedLifeCycleAssessmentFactors;
                    combinedFactors.BaseFactors.EnvironmentalFactors.Should().Contain(x => x.IMetricType() == result.IMetricType());
                    IEnvironmentalMetricFactors metric = combinedFactors.BaseFactors.EnvironmentalFactors.First(x => x.IMetricType() == result.IMetricType());

                    ValidateMetricAndResult(metric, result, eval, combinedFactors.Name, mat.Name, combinedFactors.A4TransportFactors, combinedFactors.C2TransportFactors, takeoffItem.Mass);
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
                    epd.EnvironmentalFactors.Should().Contain(x => x.IMetricType() == result.IMetricType());
                    IEnvironmentalMetricFactors metric = epd.EnvironmentalFactors.First(x => x.IMetricType() == result.IMetricType());

                    ValidateMetricAndResult(metric, result, eval, epd.Name, mat.Name);
                }
                else
                {
                    IMaterialProperties prop = mat.Properties.First(x => x.Name == result.EnvironmentalProductDeclarationName);
                    prop.Should().BeOfType<CombinedLifeCycleAssessmentFactors>();

                    CombinedLifeCycleAssessmentFactors combinedFactors = prop as CombinedLifeCycleAssessmentFactors;
                    combinedFactors.BaseFactors.EnvironmentalFactors.Should().Contain(x => x.IMetricType() == result.IMetricType());
                    IEnvironmentalMetricFactors metric = combinedFactors.BaseFactors.EnvironmentalFactors.First(x => x.IMetricType() == result.IMetricType());

                    ValidateMetricAndResult(metric, result, eval, combinedFactors.Name, mat.Name, combinedFactors.A4TransportFactors, combinedFactors.C2TransportFactors, takeoffItem.Mass);
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
                    epd.EnvironmentalFactors.Should().Contain(x => x.IMetricType() == result.IMetricType());
                    IEnvironmentalMetricFactors metric = epd.EnvironmentalFactors.First(x => x.IMetricType() == result.IMetricType());

                    construction.Layers.Should().Contain(x => x.Material.Name == result.MaterialName);
                    double eval = construction.Layers.First(x => x.Material.Name == result.MaterialName).Thickness * area;

                    ValidateMetricAndResult(metric, result, eval, epd.Name, mat.Name);
                }
            }
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static void ValidateMetricAndResult(IEnvironmentalMetricFactors metric, MaterialResult result, double quantity, string epdName = "", string materialName = "", ITransportFactors a4Factor= null, ITransportFactors c2Factor = null, double mass = 0)
        {
            double tolerance = 1e-10;

            string message;
            if (metric != null)
                message = $"Evaluating {metric.GetType().Name} comparing against {result.GetType().Name}";
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

            if(metric != null)
                result.IMetricType().Should().Be(metric.IMetricType(), message);

            List<Module> evaluatedModules = metric?.Indicators?.Keys.ToList() ?? new List<Module>();

            if (a4Factor != null)
                evaluatedModules.Add(Module.A4);
            if (c2Factor != null)
                evaluatedModules.Add(Module.C2);

            evaluatedModules = evaluatedModules.OrderBy(x => x).Distinct().ToList();
            
            Assert.Multiple(() =>
            {
                foreach (Module module in evaluatedModules)
                {
                    Console.WriteLine($"{result.IMetricType()}: {module}");
                    Assert.That(result.Indicators, Contains.Key(module), $"{module} missing while {metric}");
                    if (a4Factor != null && module == oM.LifeCycleAssessment.Module.A4)
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

    }
}



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
using BH.oM.LifeCycleAssessment.MaterialFragments.Construction;
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
    public class EvaluateGlobalEmissionsFactor : NUnitTest
    {
        /***************************************************/
        /****   Public Methods                          ****/
        /***************************************************/


        [TestCaseSource(typeof(DataSource), nameof(DataSource.DummyMetrics), new object[] { 1.234, 0.1432, true })]
        public void EvaluateGlobalEmissionsFactorMetricTest(IEnvironmentalMetric metric)
        {
            GlobalEmissionFactors config = DummyConfig();
            double quantity = 50;
            double mass = 12;

            MaterialResult result = Query.EnvironmentalResults(metric, "", "", quantity, config, mass);
            ValidateMetricAndResult(metric, result, quantity, config.ConstructionActivities.ConstructedFloorArea, config.PreConstructionDemolition.DemolishedFloorArea, config.TotalBuildingMass, config.PreConstructionDemolition.EnvironmentalFactors, config.ConstructionActivities.EnvironmentalFactors, mass);

        }

        [TestCaseSource(typeof(DataSource), nameof(DataSource.DummyEPDs), new object[] { 1.2321, 0.0002, true })]
        public void EvaluateGlobalEmissionsFactorEPDTest(EnvironmentalProductDeclaration epd)
        {
            GlobalEmissionFactors config = DummyConfig();
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
                ValidateMetricAndResult(epd.EnvironmentalMetrics[i], materialResults[i], eval, config.ConstructionActivities.ConstructedFloorArea, config.PreConstructionDemolition.DemolishedFloorArea, config.TotalBuildingMass, config.PreConstructionDemolition.EnvironmentalFactors, config.ConstructionActivities.EnvironmentalFactors, mass, epd.Name);
            }
        }


        /***************************************************/

        [TestCaseSource(typeof(DataSource), nameof(DataSource.DummyCombinedLCAFactors), new object[] { 1.2321, 0.0002, true })]
        public void EvaluateGlobalEmissionsFactorCombinedFactorsTest(CombinedLifeCycleAssessmentFactors combinedFactors)
        {
            GlobalEmissionFactors config = DummyConfig();
            double eval = 32.22;
            double mass = 22.4;
            List<MaterialResult> materialResults = Query.EnvironmentalResults(combinedFactors, eval, mass, "", null, config, mass);
            if (combinedFactors.EnvironmentalProductDeclaration == null && combinedFactors.A4TransportFactors == null && combinedFactors.C2TransportFactors == null)
            {
                Assert.That(materialResults, Is.Empty, "Should nto give results for Combined factors with all nulls.");
                return;
            }
            Assert.That(materialResults, Is.Not.Empty, "No results generated");
            for (int i = 0; i < materialResults.Count; i++)
            {
                ValidateMetricAndResult(combinedFactors.EnvironmentalProductDeclaration?.EnvironmentalMetrics[i], materialResults[i], eval, config.ConstructionActivities.ConstructedFloorArea, config.PreConstructionDemolition.DemolishedFloorArea, config.TotalBuildingMass, config.PreConstructionDemolition.EnvironmentalFactors, config.ConstructionActivities.EnvironmentalFactors, mass, combinedFactors.Name, "", combinedFactors.A4TransportFactors, combinedFactors.C2TransportFactors, combinedFactors.A5ConstructionEmissions, Evaluate.WasteAndDisposalImpact(combinedFactors?.C3C4WasteAndDisposalFactors, combinedFactors?.EnvironmentalProductDeclaration?.EnvironmentalMetrics, mass, eval, materialResults[i].IMetricType()));
            }
        }


        /***************************************************/

        [TestCaseSource(typeof(DataSource), nameof(DataSource.DummyTakeoffAndTemplates), new object[] { 1.2321, 0.0002, true })]
        public void EvaluateGlobalEmissionsFactorTakeoff(GeneralMaterialTakeoff takeoff, List<Material> templates, bool containEpds)
        {
            GlobalEmissionFactors config = DummyConfig();

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

                    ValidateMetricAndResult(metric, result, eval, config.ConstructionActivities.ConstructedFloorArea, config.PreConstructionDemolition.DemolishedFloorArea, config.TotalBuildingMass, config.PreConstructionDemolition.EnvironmentalFactors, config.ConstructionActivities.EnvironmentalFactors, takeoffItem.Mass, epd.Name, mat.Name);
                }
                else
                {
                    IMaterialProperties prop = mat.Properties.First(x => x.Name == result.EnvironmentalProductDeclarationName);
                    prop.Should().BeOfType<CombinedLifeCycleAssessmentFactors>();

                    CombinedLifeCycleAssessmentFactors combinedFactors = prop as CombinedLifeCycleAssessmentFactors;
                    combinedFactors.EnvironmentalProductDeclaration.EnvironmentalMetrics.Should().Contain(x => x.IMetricType() == result.IMetricType());
                    IEnvironmentalMetric metric = combinedFactors.EnvironmentalProductDeclaration.EnvironmentalMetrics.First(x => x.IMetricType() == result.IMetricType());
                    ValidateMetricAndResult(metric, result, eval, config.ConstructionActivities.ConstructedFloorArea, config.PreConstructionDemolition.DemolishedFloorArea, config.TotalBuildingMass, config.PreConstructionDemolition.EnvironmentalFactors, config.ConstructionActivities.EnvironmentalFactors, takeoffItem.Mass, combinedFactors.Name, mat.Name, combinedFactors.A4TransportFactors, combinedFactors.C2TransportFactors, combinedFactors.A5ConstructionEmissions);


                }

            }

        }

        [TestCaseSource(typeof(DataSource), nameof(DataSource.DummyElementsAndTemplates), new object[] { 1.2321, 0.0002, true })]
        public void EvaluateElement(Wall element, double area, List<Material> templates)
        {
            GlobalEmissionFactors config = DummyConfig();
            List<IElementResult<MaterialResult>> elementResults = Query.EnvironmentalResults(element, templates, true, null, config);

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

                    ValidateMetricAndResult(metric, result, eval, config.ConstructionActivities.ConstructedFloorArea, config.PreConstructionDemolition.DemolishedFloorArea, config.TotalBuildingMass, config.PreConstructionDemolition.EnvironmentalFactors, config.ConstructionActivities.EnvironmentalFactors, mass, epd.Name, mat.Name);
                }
            }
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static void ValidateMetricAndResult(IEnvironmentalMetric metric, MaterialResult result, double quantity, double constructedArea, double demolishedArea, double totalMass, List<IEnvironmentalFactor> preConstructionFactors, List<IEnvironmentalFactor> constructionFactors, double mass, string epdName = "", string materialName = "", ITransportFactors a4Factor = null, ITransportFactors c2Factor = null, ConstructionEmissions a5Factors = null, double c3c4Factor = double.NaN)
        {
            double tolerance = 1e-6;
            MetricType? metricType = metric?.IMetricType();

            Dictionary<Module, double> specialCases = new Dictionary<Module, double>();
            if (a4Factor != null)
                specialCases[Module.A4] = Evaluate.TransportImpact(a4Factor, result.IMetricType(), mass);
            if (c2Factor != null)
                specialCases[Module.C2] = Evaluate.TransportImpact(c2Factor, result.IMetricType(), mass);
            if (!double.IsNaN(c3c4Factor))
                specialCases[Module.C3toC4] = c3c4Factor;

            IEnvironmentalFactor preConstrcutionFactor = preConstructionFactors?.FirstOrDefault(x => x.IMetricType() == metricType);
            if(preConstrcutionFactor != null)
            {
                specialCases[Module.A5_1] = mass / totalMass * demolishedArea * preConstrcutionFactor.Value;
            }

            IEnvironmentalFactor constructionFactor = constructionFactors?.FirstOrDefault(x => x.IMetricType() == metricType);
            if(constructionFactor != null)
            {
                specialCases[Module.A5_2] = mass / totalMass * constructedArea * constructionFactor.Value;
            }

            if (a5Factors != null)
            {
                double wasteImpact = Evaluate.WasteImpact(a5Factors, result);
                specialCases[Module.A5_3] = wasteImpact;
            }

            Evaluate.ValidateResult(result, metric, quantity, epdName, materialName, specialCases);

        }

        /***************************************************/
        private static GlobalEmissionFactors DummyConfig()
        {
            return new GlobalEmissionFactors
            {
                TotalBuildingMass = 2000000,
                ConstructionActivities = new ConstructionActivities { ConstructedFloorArea = 10000 },
                PreConstructionDemolition = new PreConstructionDemolition { DemolishedFloorArea = 5000 },
                StructuresOnlyMass = false
            };
        }

        /***************************************************/

    }
}



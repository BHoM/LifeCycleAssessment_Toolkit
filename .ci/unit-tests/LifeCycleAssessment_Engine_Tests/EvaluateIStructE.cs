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
using System.Reflection;
using BH.oM.LifeCycleAssessment;
using BH.oM.Physical.Materials;
using BH.oM.LifeCycleAssessment.Configs;

namespace BH.Tests.Engine.LifeCycleAssessment
{
    public class EvaluateIStructE
    {
        /***************************************************/
        /****   Public Methods                          ****/
        /***************************************************/


        [TestCaseSource(typeof(DataSource), nameof(DataSource.DummyMetrics), new object[] { 1.234, 0.1432, true })]
        public void EvaluateIStructEMetricTest(IEnvironmentalMetricFactors metric)
        {
            IStructEEvaluationConfig config = DummyConfig();
            double quantity = 50;

            MaterialResult result = Query.EnvironmentalResults(metric, "", "", quantity, DummyConfig());
            ValidateMetricAndResult(metric, result, quantity, config.ProjectCost, config.FloorArea, config.TotalWeight, config.A5CarbonFactor, config.C1CarbonFactor);

        }

        [TestCaseSource(typeof(DataSource), nameof(DataSource.DummyEPDs), new object[] { 1.2321, 0.0002, true })]
        public void EvaluateIStructEEPDTest(EnvironmentalProductDeclaration epd)
        {
            IStructEEvaluationConfig config = DummyConfig();
            double eval = 32.22;
            List<MaterialResult> materialResults = Query.EnvironmentalResults(epd, eval, "", null, config);
            for (int i = 0; i < materialResults.Count; i++)
            {
                ValidateMetricAndResult(epd.EnvironmentalFactors[i], materialResults[i], eval, config.ProjectCost, config.FloorArea, config.TotalWeight, config.A5CarbonFactor, config.C1CarbonFactor, epd.Name);
            }
        }


        /***************************************************/

        [TestCaseSource(typeof(DataSource), nameof(DataSource.DummyTakeoffs), new object[] { 1.2321, 0.0002, true })]
        public void EvaluateIStructETakeoff(GeneralMaterialTakeoff takeoff, List<Material> templates)
        {
            IStructEEvaluationConfig config = DummyConfig();

            List<MaterialResult> materialResults = Query.EnvironmentalResults(takeoff, templates, true, null, config);

            foreach (MaterialResult result in materialResults)
            {
                templates.Should().Contain(x => x.Name == result.MaterialName);
                Material mat = templates.First(x => x.Name == result.MaterialName);
                mat.Properties.Should().Contain(x => x.Name == result.EnvironmentalProductDeclarationName);
                IMaterialProperties prop = mat.Properties.First(x => x.Name == result.EnvironmentalProductDeclarationName);
                prop.Should().BeOfType<EnvironmentalProductDeclaration>();

                EnvironmentalProductDeclaration epd = prop as EnvironmentalProductDeclaration;
                epd.EnvironmentalFactors.Should().Contain(x => x.IMetricType() == result.IMetricType());
                IEnvironmentalMetricFactors metric = epd.EnvironmentalFactors.First(x => x.IMetricType() == result.IMetricType());

                takeoff.MaterialTakeoffItems.Should().Contain(x => x.Material.Name == result.MaterialName);
                double eval = takeoff.MaterialTakeoffItems.First(x => x.Material.Name == result.MaterialName).Volume;

                ValidateMetricAndResult(metric, result, eval, config.ProjectCost, config.FloorArea, config.TotalWeight, config.A5CarbonFactor, config.C1CarbonFactor, epd.Name, mat.Name);
            }

        }


        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static void ValidateMetricAndResult(IEnvironmentalMetricFactors metric, MaterialResult result, double quantity, double projectCost, double floorArea, double totalWeight, double a5CarbonFactor, double c1CarbonFactor,  string epdName = "", string materialName = "")
        {
            double tolerance = 1e-12;
            string initialMessage = $"Evaluating {metric.GetType().Name} comparing against {result.GetType().Name}";
            if (!string.IsNullOrEmpty(epdName))
            {
                result.EnvironmentalProductDeclarationName.Should().Be(epdName, initialMessage);
            }

            if (!string.IsNullOrEmpty(materialName))
            {
                result.MaterialName.Should().Be(materialName, initialMessage);
            }

            result.IMetricType().Should().Be(metric.IMetricType(), initialMessage);

            List<MetricType> specialMetrics = new List<MetricType> { MetricType.ClimateChangeTotal, MetricType.ClimateChangeTotalNoBiogenic, MetricType.ClimateChangeFossil };
            bool specialTreatment = specialMetrics.Contains(result.IMetricType());

            foreach (var evaluatedMetric in metric.Indicators)
            {
                string message = $"Module: {evaluatedMetric.Key.ToString()} {initialMessage}";
                if (specialTreatment && evaluatedMetric.Key == oM.LifeCycleAssessment.Module.C1)
                {
                    result.Indicators[evaluatedMetric.Key].Should().BeApproximately(quantity / totalWeight * floorArea * c1CarbonFactor, tolerance, message);
                }
                else if (specialTreatment && evaluatedMetric.Key == oM.LifeCycleAssessment.Module.A5)
                {
                    result.Indicators[evaluatedMetric.Key].Should().BeApproximately(metric.Indicators[oM.LifeCycleAssessment.Module.A5] * quantity + quantity / totalWeight * a5CarbonFactor * projectCost, tolerance, message);
                }
                else if (specialTreatment && evaluatedMetric.Key == oM.LifeCycleAssessment.Module.C1toC4)
                {
                    result.Indicators[oM.LifeCycleAssessment.Module.C1toC4].Should().BeApproximately(result.Indicators[oM.LifeCycleAssessment.Module.C1] + result.Indicators[oM.LifeCycleAssessment.Module.C2] + result.Indicators[oM.LifeCycleAssessment.Module.C3] + result.Indicators[oM.LifeCycleAssessment.Module.C4], tolerance, message);
                }
                else
                {
                    result.Indicators.Should().ContainKey(evaluatedMetric.Key);
                    result.Indicators[evaluatedMetric.Key].Should().BeApproximately(evaluatedMetric.Value * quantity, tolerance, message);
                }
            }
        }

        /***************************************************/

        private static IStructEEvaluationConfig DummyConfig()
        {
            return new IStructEEvaluationConfig
            {
                TotalWeight = 20000,
                FloorArea = 500,
                ProjectCost = 1000000
            };
        }

        /***************************************************/

    }
}



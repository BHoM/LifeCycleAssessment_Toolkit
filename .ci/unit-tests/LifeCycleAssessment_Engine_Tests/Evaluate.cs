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

        [TestCaseSource(typeof(DataSource), nameof(DataSource.DummyTakeoffs), new object[] { 1.2321, 0.0002, false })]
        public void EvaluatTakeoff(GeneralMaterialTakeoff takeoff, List<Material> templates)
        {
            List<MaterialResult> materialResults = Query.EnvironmentalResults(takeoff, templates);

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

                ValidateMetricAndResult(metric, result, eval, epd.Name, mat.Name);
            }

        }

        /***************************************************/

        [TestCaseSource(typeof(DataSource), nameof(DataSource.DummyTakeoffs), new object[] { 1.2321, 0.0002, false })]
        public void EvaluatTakeoffWithFilters(GeneralMaterialTakeoff takeoff, List<Material> templates)
        {           
            List<MetricType> metricFilter = new List<MetricType> { MetricType.AbioticDepletionFossilResources, MetricType.ClimateChangeBiogenic, MetricType.EutrophicationTerrestrial };

            List<MaterialResult> materialResults = Query.EnvironmentalResults(takeoff, templates, true, metricFilter);

            materialResults.Should().AllSatisfy(x => metricFilter.Contains(x.IMetricType()));

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

                ValidateMetricAndResult(metric, result, eval, epd.Name, mat.Name);
            }

        }


        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static void ValidateMetricAndResult(IEnvironmentalMetricFactors metric, MaterialResult result, double quantity, string epdName = "", string materialName = "")
        {
            double tolerance = 1e-12;
            string message = $"Evaluating {metric.GetType().Name} comparing against {result.GetType().Name}";
            if (!string.IsNullOrEmpty(epdName))
            {
                result.EnvironmentalProductDeclarationName.Should().Be(epdName, message);
            }

            if (!string.IsNullOrEmpty(materialName))
            {
                result.MaterialName.Should().Be(materialName, message);
            }

            result.IMetricType().Should().Be(metric.IMetricType(), message);

            foreach (var evaluatedMetric in metric.Indicators)
            {
                result.Indicators.Should().ContainKey(evaluatedMetric.Key);
                result.Indicators[evaluatedMetric.Key].Should().BeApproximately(evaluatedMetric.Value*quantity, tolerance, message);
            }

        }

        /***************************************************/

    }
}



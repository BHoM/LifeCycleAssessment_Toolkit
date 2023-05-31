/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2023, the respective contributors. All rights reserved.
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

        [Test]
        public void EvaluateIStructEMetricTest()
        {
            List<EnvironmentalMetric> metrics = new List<EnvironmentalMetric>();

            double v = 1.234;
            double inc = 0.1432;
            foreach (Type type in MetricTypes())
            {
                metrics.Add(DummyMetric(type, ref v, inc));
            }

            IStructEEvaluationConfig config = DummyConfig();
            double quantity = 50;
            foreach (EnvironmentalMetric metric in metrics)
            {
                MaterialResult result = Query.EnvironmentalResults(metric, "", "", quantity, DummyConfig());
                ValidateMetricAndResult(metric, result, quantity, config.ProjectCost, config.FloorArea, config.TotalWeight, config.A5CarbonFactor, config.C1CarbonFactor);
            }

        }

        /***************************************************/

        [Test]
        public void EvaluateIStructEEPDTest()
        {
            int count = 3;

            double v = 1.2321;
            double inc = 0.0002;
            List<EnvironmentalProductDeclaration> epds = new List<EnvironmentalProductDeclaration>();

            for (int i = 0; i < count; i++)
            {
                epds.Add(DummyEPD(ref v, inc));
            }

            IStructEEvaluationConfig config = DummyConfig();

            double eval = 32.22;
            foreach (EnvironmentalProductDeclaration epd in epds)
            {
                List<MaterialResult> materialResults = Query.EnvironmentalResults(epd, eval, "", null, config);
                for (int i = 0; i < materialResults.Count; i++)
                {
                    ValidateMetricAndResult(epd.EnvironmentalMetrics[i], materialResults[i], eval, config.ProjectCost, config.FloorArea, config.TotalWeight, config.A5CarbonFactor, config.C1CarbonFactor, epd.Name);
                }
            }

        }


        /***************************************************/

        [Test]
        public void EvaluateIStructETakeoff()
        {
            List<string> names = new List<string>() { "Concrete", "Steel", "Glass" };

            double v = 1.2321;
            double inc = 0.0002;
            List<Material> templates = new List<Material>();

            foreach (string matName in names)
            {
                templates.Add(new Material
                {
                    Name = matName,
                    Properties = new List<IMaterialProperties> { DummyEPD(ref v, inc, matName + "EPD NAME", QuantityType.Volume) }
                });
            }

            IStructEEvaluationConfig config = DummyConfig();

            GeneralMaterialTakeoff takeoff = new GeneralMaterialTakeoff
            {
                MaterialTakeoffItems = names.Select((x, i) => new TakeoffItem { Material = new Material { Name = x }, Volume = (i + 1) * 42.543 }).ToList(),
            };

            List<MaterialResult> materialResults = Query.EnvironmentalResults(takeoff, templates, true, null, config);

            foreach (MaterialResult result in materialResults)
            {
                templates.Should().Contain(x => x.Name == result.MaterialName);
                Material mat = templates.First(x => x.Name == result.MaterialName);
                mat.Properties.Should().Contain(x => x.Name == result.EnvironmentalProductDeclarationName);
                IMaterialProperties prop = mat.Properties.First(x => x.Name == result.EnvironmentalProductDeclarationName);
                prop.Should().BeOfType<EnvironmentalProductDeclaration>();

                EnvironmentalProductDeclaration epd = prop as EnvironmentalProductDeclaration;
                epd.EnvironmentalMetrics.Should().Contain(x => x.MetricType == result.MetricType);
                EnvironmentalMetric metric = epd.EnvironmentalMetrics.First(x => x.MetricType == result.MetricType);

                takeoff.MaterialTakeoffItems.Should().Contain(x => x.Material.Name == result.MaterialName);
                double eval = takeoff.MaterialTakeoffItems.First(x => x.Material.Name == result.MaterialName).Volume;

                ValidateMetricAndResult(metric, result, eval, config.ProjectCost, config.FloorArea, config.TotalWeight, config.A5CarbonFactor, config.C1CarbonFactor, epd.Name, mat.Name);
            }

        }


        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static void ValidateMetricAndResult(EnvironmentalMetric metric, MaterialResult result, double quantity, double projectCost, double floorArea, double totalWeight, double a5CarbonFactor, double c1CarbonFactor,  string epdName = "", string materialName = "")
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

            result.MetricType.Should().Be(metric.MetricType, message);
            result.A1.Should().BeApproximately(metric.A1 * quantity, tolerance, message);
            result.A2.Should().BeApproximately(metric.A2 * quantity, tolerance, message);
            result.A3.Should().BeApproximately(metric.A3 * quantity, tolerance, message);
            result.A4.Should().BeApproximately(metric.A4 * quantity, tolerance, message);
            result.A1toA3.Should().BeApproximately(metric.A1toA3 * quantity, tolerance, message);

            result.B1.Should().BeApproximately(metric.B1 * quantity, tolerance, message);
            result.B2.Should().BeApproximately(metric.B2 * quantity, tolerance, message);
            result.B3.Should().BeApproximately(metric.B3 * quantity, tolerance, message);
            result.B4.Should().BeApproximately(metric.B4 * quantity, tolerance, message);
            result.B5.Should().BeApproximately(metric.B5 * quantity, tolerance, message);
            result.B6.Should().BeApproximately(metric.B6 * quantity, tolerance, message);
            result.B7.Should().BeApproximately(metric.B7 * quantity, tolerance, message);
            result.B1toB7.Should().BeApproximately(metric.B1toB7 * quantity, tolerance, message);

            result.C2.Should().BeApproximately(metric.C2 * quantity, tolerance, message);
            result.C3.Should().BeApproximately(metric.C3 * quantity, tolerance, message);
            result.C4.Should().BeApproximately(metric.C4 * quantity, tolerance, message);

            result.D.Should().BeApproximately(metric.D * quantity, tolerance, message);

            if (metric.MetricType == EnvironmentalMetrics.ClimateChangeTotalNoBiogenic || metric.MetricType == EnvironmentalMetrics.ClimateChangeTotal)
            {
                result.A5.Should().BeApproximately(metric.A5 * quantity + quantity / totalWeight * a5CarbonFactor * projectCost, tolerance, message);
                result.C1.Should().BeApproximately(quantity / totalWeight * floorArea * c1CarbonFactor, tolerance, message);
                result.C1toC4.Should().BeApproximately(result.C1 + result.C2 + result.C3 + result.C4, tolerance, message);
            }
            else
            {
                result.A5.Should().BeApproximately(metric.A5 * quantity, tolerance, message);
                result.C1.Should().BeApproximately(metric.C1 * quantity, tolerance, message);
                result.C1toC4.Should().BeApproximately(metric.C1toC4 * quantity, tolerance, message);
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

        private static EnvironmentalProductDeclaration DummyEPD(ref double v, double inc, string name = "", QuantityType quantityType = (QuantityType)(-1))
        {
            if (string.IsNullOrEmpty(name))
                name = new AutoBogus.AutoFaker<string>().Generate();

            if ((int)quantityType == -1)
            {
                Array values = Enum.GetValues(typeof(QuantityType));
                Random random = new Random();
                quantityType = (QuantityType)values.GetValue(random.Next(values.Length));
            }

            List<EnvironmentalMetric> metrics = new List<EnvironmentalMetric>();

            foreach (Type type in MetricTypes())
            {
                metrics.Add(DummyMetric(type, ref v, inc));
            }

            return new EnvironmentalProductDeclaration
            {
                Name = name,
                QuantityType = quantityType,
                EnvironmentalMetrics = metrics
            };
        }

        /***************************************************/

        private static EnvironmentalMetric DummyMetric(Type type, ref double v, double inc)
        {
            MethodInfo create = typeof(BH.Engine.LifeCycleAssessment.Create).GetMethods().Where(x => x.ReturnType == type).OrderByDescending(x => x.GetParameters().Length).FirstOrDefault();

            if (create == null)
                return null;

            object[] para = new object[create.GetParameters().Length];

            for (int i = 0; i < para.Length; i++)
            {
                para[i] = v;
                v += inc;
            }

            return create.Invoke(null, para) as EnvironmentalMetric;
        }

        /***************************************************/

        private static List<Type> MetricTypes()
        {
            return new List<Type>
            {
                typeof(AbioticDepletionFossilResourcesMetric),
                typeof(AbioticDepletionMineralsAndMetalsMetric),
                typeof(AcidificationMetric),
                typeof(ClimateChangeBiogenicMetric),
                typeof(ClimateChangeFossilMetric),
                typeof(ClimateChangeLandUseMetric),
                typeof(ClimateChangeTotalMetric),
                typeof(ClimateChangeTotalNoBiogenicMetric),
                typeof(EutrophicationAquaticFreshwaterMetric),
                typeof(EutrophicationAquaticMarineMetric),
                typeof(EutrophicationTerrestrialMetric),
                typeof(EutrophicationCMLMetric),
                typeof(EutrophicationTRACIMetric),
                typeof(OzoneDepletionMetric),
                typeof(PhotochemicalOzoneCreationMetric),
                typeof(WaterDeprivationMetric)
            };
        }

        /***************************************************/
    }
}

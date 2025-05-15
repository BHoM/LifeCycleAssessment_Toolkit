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

        [Test]
        public void EvaluateMetricTest()
        {
            List<IEnvironmentalMetricFactors> metrics = new List<IEnvironmentalMetricFactors>();

            double v = 1.234;
            double inc = 0.1432;
            foreach (Type type in MetricTypes())
            {
                metrics.Add(DummyMetric(type, ref v, inc));
            }

            double quantity = 4;
            foreach (IEnvironmentalMetricFactors metric in metrics)
            {
                MaterialResult result = Query.EnvironmentalResults(metric, "", "", quantity);
                ValidateMetricAndResult(metric, result, quantity);
            }

        }

        /***************************************************/

        [Test]
        public void EvaluatEPDTest()
        {
            int count = 3;

            double v = 1.2321;
            double inc = 0.0002;
            List<EnvironmentalProductDeclaration> epds = new List<EnvironmentalProductDeclaration>();

            for (int i = 0; i < count; i++)
            {
                epds.Add(DummyEPD(ref v, inc));
            }

            double eval = 32.22;
            foreach (EnvironmentalProductDeclaration epd in epds)
            {
                List<MaterialResult> materialResults = Query.EnvironmentalResults(epd, eval);
                for (int i = 0; i < materialResults.Count; i++)
                {
                    ValidateMetricAndResult(epd.EnvironmentalFactors[i], materialResults[i], eval, epd.Name);
                }
            }

        }


        /***************************************************/

        [Test]
        public void EvaluatTakeoff()
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


            GeneralMaterialTakeoff takeoff = new GeneralMaterialTakeoff
            {
                MaterialTakeoffItems = names.Select((x, i) => new TakeoffItem { Material = new Material { Name = x }, Volume = (i + 1) * 42.543 }).ToList(),
            };

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

        [Test]
        public void EvaluatTakeoffWithFilters()
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

            GeneralMaterialTakeoff takeoff = new GeneralMaterialTakeoff
            {
                MaterialTakeoffItems = names.Select((x, i) => new TakeoffItem { Material = new Material { Name = x }, Volume = (i + 1) * 42.543 }).ToList(),
            };

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

            //result.A1.Should().BeApproximately(metric.A1 * quantity, tolerance, message);
            //result.A2.Should().BeApproximately(metric.A2 * quantity, tolerance, message);
            //result.A3.Should().BeApproximately(metric.A3 * quantity, tolerance, message);
            //result.A4.Should().BeApproximately(metric.A4 * quantity, tolerance, message);
            //result.A5.Should().BeApproximately(metric.A5 * quantity, tolerance, message);
            //result.A1toA3.Should().BeApproximately(metric.A1toA3 * quantity, tolerance, message);

            //result.B1.Should().BeApproximately(metric.B1 * quantity, tolerance, message);
            //result.B2.Should().BeApproximately(metric.B2 * quantity, tolerance, message);
            //result.B3.Should().BeApproximately(metric.B3 * quantity, tolerance, message);
            //result.B4.Should().BeApproximately(metric.B4 * quantity, tolerance, message);
            //result.B5.Should().BeApproximately(metric.B5 * quantity, tolerance, message);
            //result.B6.Should().BeApproximately(metric.B6 * quantity, tolerance, message);
            //result.B7.Should().BeApproximately(metric.B7 * quantity, tolerance, message);
            //result.B1toB7.Should().BeApproximately(metric.B1toB7 * quantity, tolerance, message);

            //result.C1.Should().BeApproximately(metric.C1 * quantity, tolerance, message);
            //result.C2.Should().BeApproximately(metric.C2 * quantity, tolerance, message);
            //result.C3.Should().BeApproximately(metric.C3 * quantity, tolerance, message);
            //result.C4.Should().BeApproximately(metric.C4 * quantity, tolerance, message);
            //result.C1toC4.Should().BeApproximately(metric.C1toC4 * quantity, tolerance, message);

            //result.D.Should().BeApproximately(metric.D * quantity, tolerance, message);
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

            List<IEnvironmentalMetricFactors> metrics = new List<IEnvironmentalMetricFactors>();

            foreach (Type type in MetricTypes())
            {
                metrics.Add(DummyMetric(type, ref v, inc));
            }

            return new EnvironmentalProductDeclaration
            {
                Name = name,
                QuantityType = quantityType,
                EnvironmentalFactors = metrics,
            };
        }

        /***************************************************/

        private static IEnvironmentalMetricFactors DummyMetric(Type type, ref double v, double inc)
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

            return create.Invoke(null, para) as IEnvironmentalMetricFactors;
        }

        /***************************************************/

        private static List<Type> MetricTypes()
        {
            return new List<Type>
            {
                //typeof(AbioticDepletionFossilResourcesMetric),
                //typeof(AbioticDepletionMineralsAndMetalsMetric),
                //typeof(AcidificationMetric),
                typeof(ClimateChangeBiogenicMetric),
                typeof(ClimateChangeFossilMetric),
                typeof(ClimateChangeLandUseMetric),
                typeof(ClimateChangeTotalMetric),
                typeof(ClimateChangeTotalNoBiogenicMetric),
                //typeof(EutrophicationAquaticFreshwaterMetric),
                //typeof(EutrophicationAquaticMarineMetric),
                //typeof(EutrophicationTerrestrialMetric),
                //typeof(EutrophicationCMLMetric),
                //typeof(EutrophicationTRACIMetric),
                //typeof(OzoneDepletionMetric),
                //typeof(PhotochemicalOzoneCreationMetric),
                //typeof(PhotochemicalOzoneCreationCMLMetric),
                //typeof(PhotochemicalOzoneCreationTRACIMetric),
                //typeof(WaterDeprivationMetric)
            };
        }

        /***************************************************/
    }
}



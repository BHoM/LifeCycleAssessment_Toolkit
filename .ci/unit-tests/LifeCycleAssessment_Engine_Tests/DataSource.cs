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
using BH.oM.Physical.Elements;
using BH.oM.Geometry;
using BH.oM.Physical.Constructions;

namespace BH.Tests.Engine.LifeCycleAssessment
{
    public class DataSource
    {
        /***************************************************/
        /****   Public Methods                          ****/
        /***************************************************/

        public static IEnumerable<IEnvironmentalMetricFactors> DummyMetrics(double initialV, double increace, bool setA5ToWaste)
        {
            List<IEnvironmentalMetricFactors> metrics = new List<IEnvironmentalMetricFactors>();

            double v = initialV;
            double inc = increace;
            foreach (Type type in MetricTypes())
                yield return DummyMetric(type, ref v, inc, setA5ToWaste);

        }

        /***************************************************/

        public static IEnumerable<List<IEnvironmentalMetricFactors>> DummyMetricsList(double initialV, double increace, bool setA5ToWaste)
        {
            List<IEnvironmentalMetricFactors> metrics = new List<IEnvironmentalMetricFactors>();

            double v = initialV;
            double inc = increace;
            foreach (Type type in MetricTypes())
                metrics.Add(DummyMetric(type, ref v, inc, setA5ToWaste));

            yield return metrics;
        }

        /***************************************************/

        public static IEnumerable<EnvironmentalProductDeclaration> DummyEPDs(double initialV, double increace, bool setA5ToWaste)
        {
            List<IEnvironmentalMetricFactors> metrics = new List<IEnvironmentalMetricFactors>();

            double v = initialV;
            double inc = increace;
            for (int i = 0; i < 3; i++)
                yield return DummyEPD(ref v, inc, setA5ToWaste);

        }

        /***************************************************/

        public static IEnumerable<object[]> DummyTakeoffs(double initialV, double increace, bool setA5ToWaste)
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
                    Properties = new List<IMaterialProperties> { DummyEPD(ref v, inc, setA5ToWaste, matName + "EPD NAME", QuantityType.Volume) }
                });
            }


            GeneralMaterialTakeoff takeoff = new GeneralMaterialTakeoff
            {
                MaterialTakeoffItems = names.Select((x, i) => new TakeoffItem { Material = new Material { Name = x }, Volume = (i + 1) * 42.543 }).ToList(),
            };

            yield return new object[] { takeoff, templates };
        }

        /***************************************************/

        public static IEnumerable<object[]> DummyElementsAndTemplates(double initialV, double increace, bool setA5ToWaste)
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
                    Properties = new List<IMaterialProperties> { DummyEPD(ref v, inc, setA5ToWaste, matName + "EPD NAME", QuantityType.Volume) }
                });
            }
            Construction constructtion = new Construction
            {
                Layers = names.Select((x, i) => new Layer { Material = new Material { Name = x }, Thickness = (i + 1) * 0.1 }).ToList(),
            };
            Wall wall = BH.Engine.Physical.Create.Wall(constructtion, new Line { Start = new Point(), End = new Point { X = 10 } }, 10);


            yield return new object[] { wall, 100, templates };
        }

        /***************************************************/

        public static EnvironmentalProductDeclaration DummyEPD(ref double v, double inc, bool setA5ToWaste, string name = "", QuantityType quantityType = (QuantityType)(-1))
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
                metrics.Add(DummyMetric(type, ref v, inc, setA5ToWaste));
            }

            return new EnvironmentalProductDeclaration
            {
                Name = name,
                QuantityType = quantityType,
                EnvironmentalFactors = metrics,
            };
        }

        /***************************************************/

        public static IEnvironmentalMetricFactors DummyMetric(Type type, ref double v, double inc, bool setA5ToWaste)
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

            IEnvironmentalMetricFactors metric = create.Invoke(null, para) as IEnvironmentalMetricFactors;

            if (setA5ToWaste)
            {
                if (metric is ClimateChangeFossilMetric || metric is ClimateChangeTotalMetric || metric is ClimateChangeTotalNoBiogenicMetric)
                {
                    metric.Indicators[oM.LifeCycleAssessment.Module.A5w] = metric.Indicators[oM.LifeCycleAssessment.Module.A5];
                    metric.Indicators.Remove(oM.LifeCycleAssessment.Module.A5);
                }
            }
            return metric;
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
                typeof(PhotochemicalOzoneCreationCMLMetric),
                typeof(PhotochemicalOzoneCreationTRACIMetric),
                typeof(WaterDeprivationMetric)
            };
        }

        /***************************************************/
    }
}



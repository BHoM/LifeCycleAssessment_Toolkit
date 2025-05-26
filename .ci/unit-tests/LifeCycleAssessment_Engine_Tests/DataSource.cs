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
using BH.oM.LifeCycleAssessment.MaterialFragments.Transport;

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

        public static IEnumerable<IEnvironmentalFactor> DummyFactors(double initialV, double increace)
        {
            double v = initialV;
            double inc = increace;
            foreach (Type type in FactorTypes())
                yield return DummyFactor(type, ref v, inc);

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

        public static IEnumerable<List<IEnvironmentalFactor>> DummyFactorsList(double initialV, double increace)
        {
            List<IEnvironmentalFactor> factors = new List<IEnvironmentalFactor>();
            double v = initialV;
            double inc = increace;
            foreach (Type type in FactorTypes())
                factors.Add(DummyFactor(type, ref v, inc));

            yield return factors;
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

        public static IEnumerable<CombinedLifeCycleAssessmentFactors> DummyCombinedLCAFactors(double initialV, double increace, bool setA5ToWaste)
        {
            List<IEnvironmentalMetricFactors> metrics = new List<IEnvironmentalMetricFactors>();

            double v = initialV;
            double inc = increace;
            for (int i = 0; i < 4; i++)
                yield return DummyCombinedFactors(ref v, inc, setA5ToWaste, i);


            for (int i = 0; i < 4; i++)
            {
                CombinedLifeCycleAssessmentFactors combFactors = DummyCombinedFactors(ref v, inc, setA5ToWaste, i);
                combFactors.BaseFactors = null;
                yield return combFactors;
            }
        }

        /***************************************************/

        public static IEnumerable<object[]> DummyTakeoffAndTemplates(double initialV, double increace, bool setA5ToWaste)
        {

            double v = initialV;
            double inc = increace;
            List<Material> templates = MaterialsWithEpds(ref v, inc, setA5ToWaste);

            GeneralMaterialTakeoff takeoff = new GeneralMaterialTakeoff
            {
                MaterialTakeoffItems = templates.Select((x, i) => new TakeoffItem { Material = new Material { Name = x.Name, Density = 1 }, Volume = (i + 1) * 42.543, Mass = (i + 1) * 42.543 * x.Density }).ToList(),
            };

            yield return new object[] { takeoff, templates, true };

            List<Material> newTemplates = new List<Material>();

            for (int i = 0; i < templates.Count; i++)
            {
                newTemplates.Add(new Material { Name = templates[i].Name, Density = templates[i].Density, Properties = new List<IMaterialProperties> { DummyCombinedFactors(ref v, inc, setA5ToWaste, i, templates[i].Name + "EPD NAME", templates[i].Properties.OfType<EnvironmentalProductDeclaration>().FirstOrDefault().QuantityType) } });
            }

            yield return new object[] { takeoff, newTemplates, false };
        }

        /***************************************************/

        private static List<Material> MaterialsWithEpds(ref double v, double inc, bool setA5ToWaste)
        {
            QuantityType quantityType = setA5ToWaste ? QuantityType.Mass : QuantityType.Volume;
            return new List<Material>
            {
                new Material { Name = "Concrete", Density = 2400, Properties = new List<IMaterialProperties> { DummyEPD(ref v, inc, setA5ToWaste, " Concrete EPD NAME", quantityType) } },
                new Material { Name = "Steel", Density = 7800, Properties = new List<IMaterialProperties> { DummyEPD(ref v, inc, setA5ToWaste, " Steel EPD NAME", quantityType) } },
                new Material { Name = "Timber", Density = 700, Properties = new List<IMaterialProperties> { DummyEPD(ref v, inc, setA5ToWaste, " Timber EPD NAME", quantityType) } }
            };

        }

        /***************************************************/

        public static IEnumerable<object[]> DummyElementsAndTemplates(double initialV, double increace, bool setA5ToWaste)
        {
            List<string> names = new List<string>() { "Concrete", "Steel", "Glass",  };

            double v = 1.2321;
            double inc = 0.0002;
            List<Material> templates = MaterialsWithEpds(ref v, inc, setA5ToWaste);
            Construction constructtion = new Construction
            {
                Layers = templates.Select((x, i) => new Layer { Material = new Material { Name = x.Name, Density = x.Density }, Thickness = (i + 1) * 0.1 }).ToList(),
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
                if (setA5ToWaste)
                    quantityType = QuantityType.Mass;
                else
                {
                    Array values = Enum.GetValues(typeof(QuantityType));
                    Random random = new Random();
                    quantityType = (QuantityType)values.GetValue(random.Next(values.Length));
                }
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

        public static CombinedLifeCycleAssessmentFactors DummyCombinedFactors(ref double v, double inc, bool setA5ToWaste, int transportMode, string name = "", QuantityType quantityType = (QuantityType)(-1))
        {
            EnvironmentalProductDeclaration epd = DummyEPD(ref v, inc, setA5ToWaste, name, quantityType);

            return new CombinedLifeCycleAssessmentFactors { BaseFactors = epd, A4TransportFactors = DummyTransportFactor(ref v, inc, transportMode), C2TransportFactors = DummyTransportFactor(ref v, inc, transportMode), Name = name };
        }

        /***************************************************/

        public static ITransportFactors DummyTransportFactor(ref double v, double inc, int transportMode)
        {
            transportMode = transportMode % 4;
            if (transportMode == 0)
                return null;
            if (transportMode == 1)
            {
                FullTransportScenario scenario = new FullTransportScenario();
                foreach (Type type in FactorTypes())
                {
                    scenario.EnvironmentalFactors.Add(DummyFactor(type, ref v, inc));
                }
                return scenario;
            }
            if (transportMode == 2)
            {
                SingleTransportModeImpact singleTransportModeImpact = new SingleTransportModeImpact
                {
                    VehicleEmissions = DummyVehicleEmissions(ref v, inc),
                    DistanceTraveled = v * 1000,
                };
                return singleTransportModeImpact;
            }
            else
            { 
                DistanceTransportModeScenario scenario = new DistanceTransportModeScenario();
                for (int i = 0; i < 2; i++)
                {
                    scenario.SingleTransportModeImpacts.Add(DummyTransportFactor(ref v, inc, 2) as SingleTransportModeImpact);
                }
                return scenario;
            }
        }

        /***************************************************/

        public static VehicleEmissions DummyVehicleEmissions(ref double v, double inc)
        {
            VehicleEmissions vehicleEmissions = new VehicleEmissions();
            foreach (Type type in FactorTypes())
            {
                vehicleEmissions.EnvironmentalFactors.Add(DummyFactor(type, ref v, inc));
            }
            vehicleEmissions.ReturnTripFactor = inc;
            return vehicleEmissions;
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

        public static IEnvironmentalFactor DummyFactor(Type type, ref double v, double inc)
        {
            IEnvironmentalFactor factor = Activator.CreateInstance(type) as IEnvironmentalFactor;
            factor.Value = v;
            v += inc;
            return factor;
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

        private static List<Type> FactorTypes()
        {
            return new List<Type>
            {
                typeof(AbioticDepletionFossilResourcesFactor),
                typeof(AbioticDepletionMineralsAndMetalsFactor),
                typeof(AcidificationFactor),
                typeof(ClimateChangeBiogenicFactor),
                typeof(ClimateChangeFossilFactor),
                typeof(ClimateChangeLandUseFactor),
                typeof(ClimateChangeTotalFactor),
                typeof(ClimateChangeTotalNoBiogenicFactor),
                typeof(EutrophicationAquaticFreshwaterFactor),
                typeof(EutrophicationAquaticMarineFactor),
                typeof(EutrophicationTerrestrialFactor),
                typeof(EutrophicationCMLFactor),
                typeof(EutrophicationTRACIFactor),
                typeof(OzoneDepletionFactor),
                typeof(PhotochemicalOzoneCreationFactor),
                typeof(PhotochemicalOzoneCreationCMLFactor),
                typeof(PhotochemicalOzoneCreationTRACIFactor),
                typeof(WaterDeprivationFactor)
            };
        }

        /***************************************************/
    }
}



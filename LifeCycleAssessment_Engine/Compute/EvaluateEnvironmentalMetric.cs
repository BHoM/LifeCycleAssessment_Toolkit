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

using BH.Engine.Base;
using BH.oM.Base;
using BH.oM.Base.Attributes;
using BH.oM.LifeCycleAssessment.MaterialFragments;
using BH.oM.LifeCycleAssessment.Results;
using BH.oM.Physical.Constructions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace BH.Engine.LifeCycleAssessment
{
    public static partial class Compute
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static MaterialResult2 EvaluateEnvironmentalMetric(IEnvironmentalMetric metric, string epdName, string materialName, double quantityValue)
        {
            List<object> parameters = new List<object> { materialName, epdName };
            Output<List<Func<IEnvironmentalMetric, double>>, Func<object[], MaterialResult2>> constAndParameters = GetMaterialResultConstructorAndFuncs(metric as dynamic);
            List<Func<IEnvironmentalMetric, double>> metricFuncs = constAndParameters.Item1;
            Func<object[], MaterialResult2> cst = constAndParameters.Item2;

            foreach (Func<IEnvironmentalMetric, double> f in metricFuncs)
            {
                parameters.Add(f(metric) * quantityValue);
            }
            return cst(parameters.ToArray());
        }

        /***************************************************/

        public static MaterialResult2 EvaluateEnvironmentalMetric5(IEnvironmentalMetric metric, string epdName, string materialName, double quantityValue)
        {
            //Get the constructor for the material result of the type corresponding to the metric currently being evaluated
            //This is done by finding the MaterialResult with matching name and exatracting the constructor from it
            //The constructor is pre-compiled to a function to speed up the execution of the particular method
            Func<object[], MaterialResult2> cst = Query.MaterialResultConstructor(metric.GetType());

            //Collect all the relevant data
            //First two parameters of all MaterialResults should always be name of the material and name of the EPD
            List<object> parameters = new List<object> { materialName, epdName };
            //Collect the rest of the evaluation metrics
            //For most cases this will be the phases 
            //Imporant that the order of the metrics extracted cooresponds to the order of the constructor
            //General order should always be all the default phases (A1-A5, B1-B7, C1-C4 and D) followed by any additional phases corresponding to the metric currently being evaluated
            //For example, GlobalWarmpingPotential will have an additional property corresponding to BiogenicCarbon
            List<double> phaseDataValues = metric.IPhaseDataValues();
            foreach (double phaseData in phaseDataValues)
            {
                parameters.Add(phaseData * quantityValue);  //Evaluation value is base phase data scaled by quantity value
            }

            //Call the constructor function
            return cst(parameters.ToArray());
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/  

        private static MaterialResult2 IEvaluateEnvironmentalMetric3(IEnvironmentalMetric metric, string epdName, string materialName, double quantityValue)
        {
            return EvaluateMetric3(metric as dynamic, epdName, materialName, quantityValue);
        }

        /***************************************************/

        private static GlobalWarmingPotentialMaterialResult EvaluateMetric3(GlobalWarmingPotentialMetrics metric, string epdName, string materialName, double quantityValue)
        {
            return new GlobalWarmingPotentialMaterialResult(
                        materialName, 
                        epdName,
                        quantityValue * metric.A1,
                        quantityValue * metric.A2,
                        quantityValue * metric.A3,
                        quantityValue * metric.A1toA3,
                        quantityValue * metric.A4,
                        quantityValue * metric.A5,
                        quantityValue * metric.B1,
                        quantityValue * metric.B2,
                        quantityValue * metric.B3,
                        quantityValue * metric.B4,
                        quantityValue * metric.B5,
                        quantityValue * metric.B6,
                        quantityValue * metric.B7,
                        quantityValue * metric.C1,
                        quantityValue * metric.C2,
                        quantityValue * metric.C3,
                        quantityValue * metric.C4,
                        quantityValue * metric.D,
                        quantityValue * metric.BiogenicCarbon);
        }

        /***************************************************/

        private static AcidificationPotentialMaterialResult EvaluateMetric3(AcidificationPotentialMetrics metric, string epdName, string materialName, double quantityValue)
        {
            return new AcidificationPotentialMaterialResult(materialName, epdName,
                        quantityValue * metric.A1,
                        quantityValue * metric.A2,
                        quantityValue * metric.A3,
                        quantityValue * metric.A1toA3,
                        quantityValue * metric.A4,
                        quantityValue * metric.A5,
                        quantityValue * metric.B1,
                        quantityValue * metric.B2,
                        quantityValue * metric.B3,
                        quantityValue * metric.B4,
                        quantityValue * metric.B5,
                        quantityValue * metric.B6,
                        quantityValue * metric.B7,
                        quantityValue * metric.C1,
                        quantityValue * metric.C2,
                        quantityValue * metric.C3,
                        quantityValue * metric.C4,
                        quantityValue * metric.D);
        }

        /***************************************************/

        private static Func<object[], MaterialResult2> GetMaterialResultConstructor<T>(T metric) where T : IEnvironmentalMetric
        {
            Type t = typeof(T);

            Func<object[], MaterialResult2> cstFunc;

            if (!m_MaterialResultConstructors.TryGetValue(t, out cstFunc))
            {
                ConstructorInfo constructor = GetMaterialResultConstructor(t);
                Func<object[], object> genericFunc = constructor.ToFunc();
                cstFunc = x => (MaterialResult2)genericFunc(x);
                m_MaterialResultConstructors[t] = cstFunc;
            }

            return cstFunc;
        }

        /***************************************************/

        private static Output<List<Func<IEnvironmentalMetric, double>>, Func<object[], MaterialResult2>> GetMaterialResultConstructorAndFuncs<T>(T metric) where T : IEnvironmentalMetric
        {
            Type t = typeof(T);

            Func<object[], MaterialResult2> cstFunc;
            ConstructorInfo constructor = null;


            if (!m_MaterialResultConstructors.TryGetValue(t, out cstFunc))
            {
                constructor = GetMaterialResultConstructor(t);
                Func<object[], object> genericFunc = constructor.ToFunc();
                cstFunc = x => (MaterialResult2)genericFunc(x);
                m_MaterialResultConstructors[t] = cstFunc;
            }

            List<Func<IEnvironmentalMetric, double>> funcs;

            if (!m_MetricProperties.TryGetValue(t, out funcs))
            { 
                if(constructor == null)
                    constructor = GetMaterialResultConstructor(t);

                Dictionary<string, PropertyInfo> props = t.GetProperties(BindingFlags.Instance | BindingFlags.Public).Where(x => x.PropertyType == typeof(double)).ToDictionary(x => x.Name, StringComparer.OrdinalIgnoreCase);

                funcs = new List<Func<IEnvironmentalMetric, double>>();

                foreach (ParameterInfo parameter in constructor.GetParameters())
                {
                    if (parameter.ParameterType != typeof(double))
                        continue;

                    PropertyInfo prop;
                    if (props.TryGetValue(parameter.Name, out prop))
                    {
                        Func<T, double> func = (Func<T, double>)Delegate.CreateDelegate(typeof(Func<T, double>), prop.GetGetMethod());
                        funcs.Add(x => func((T)x));
                    }
                }
                m_MetricProperties[t] = funcs;
            }
            return new Output<List<Func<IEnvironmentalMetric, double>>, Func<object[], MaterialResult2>> { Item1 = funcs, Item2 = cstFunc };
        }

        /***************************************************/

        private static ConstructorInfo GetMaterialResultConstructor(Type metricType)
        {
            Type materialResultType = GetMaterialMaterialResultType(metricType);

            return materialResultType.GetConstructors().OrderByDescending(x => x.GetParameters().Count()).First();
        }

        /***************************************************/

        private static Type GetMaterialMaterialResultType(Type metricType)
        {
            string metric = metricType.Name.Replace("Metrics", "");
            Type materialResultType = BH.Engine.Base.Query.BHoMTypeList().Where(x => typeof(MaterialResult2).IsAssignableFrom(x)).First(x => x.Name.StartsWith(metric));
            return materialResultType;
        }

        /***************************************************/

        private static ConcurrentDictionary<Type, Func<object[], MaterialResult2>> m_MaterialResultConstructors = new ConcurrentDictionary<Type, Func<object[], MaterialResult2>>();
        private static ConcurrentDictionary<Type, List<Func<IEnvironmentalMetric, double>>> m_MetricProperties = new ConcurrentDictionary<Type, List<Func<IEnvironmentalMetric, double>>>();

    }
}

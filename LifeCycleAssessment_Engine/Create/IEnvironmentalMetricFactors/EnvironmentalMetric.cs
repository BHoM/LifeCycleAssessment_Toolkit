/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2026, the respective contributors. All rights reserved.
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
using BH.oM.Base.Attributes;
using BH.oM.LifeCycleAssessment;
using BH.oM.LifeCycleAssessment.MaterialFragments;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;

namespace BH.Engine.LifeCycleAssessment
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Creates an IEnvironmentalMetric of a type matched to the provided MetricType, with the provided values per module stored as its indicators.")]
        [Input("metricType", "The type of metric to create. Decides which type of IEnvironmentalMetric is returned, e.g. Acidification will return an AcidificationMetric.")]
        [Input("indicators", "The values to be stored on the metric per each module. Values should be per quantity corresponding to the QuantityType on the epd that this metric is associated with.")]
        [Output("metric", "The created IEnvironmentalMetric.")]
        public static IEnvironmentalMetric EnvironmentalMetric(MetricType metricType, Dictionary<Module, double> indicators)
        {
            if (indicators == null)
            {
                Base.Compute.RecordError($"Cannot create a {nameof(IEnvironmentalMetric)} from null indicators.");
                return null;
            }

            //Get the pre-compiled default constructor for the metric corresponding to the provided type
            Func<IEnvironmentalMetric> cst = EnvironmentalMetricConstructor(metricType);

            if (cst == null)
                return null;

            IEnvironmentalMetric metric = cst();

            //Indicators is get-only on the interface. Fill the dictionary of the new metric, which also means the provided dictionary is not shared with the metric.
            foreach (KeyValuePair<Module, double> indicator in indicators)
            {
                metric.Indicators[indicator.Key] = indicator.Value;
            }

            return metric;
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        [Description("Gets a function corresponding to the default constructor for the IEnvironmentalMetric corresponding to the provided MetricType.")]
        [Input("metricType", "The type of metric to find a matching constructor for.")]
        [Output("cstFunc", "The function corresponding to the constructor of the IEnvironmentalMetric related to the MetricType.")]
        private static Func<IEnvironmentalMetric> EnvironmentalMetricConstructor(this MetricType metricType)
        {
            Func<IEnvironmentalMetric> cstFunc;

            //Try get cached constructor func
            if (!m_EnvironmentalMetricConstructors.TryGetValue(metricType, out cstFunc))
            {
                System.Reflection.ConstructorInfo constructor = GetEnvironmentalMetricConstructorInfo(metricType);
                if (constructor != null)
                {
                    //Pre-compile "() => new XMetric()" to a parameterless function to increase performance.
                    //No object[] input or cast wrapper is needed as the constructor is known to have no parameters.
                    cstFunc = Expression.Lambda<Func<IEnvironmentalMetric>>(Expression.New(constructor)).Compile();
                }
                else
                    cstFunc = null;

                m_EnvironmentalMetricConstructors[metricType] = cstFunc;
            }

            if (cstFunc == null)
                Base.Compute.RecordError($"Unable to find a constructor for a type of {nameof(IEnvironmentalMetric)} based on provided type {metricType}");

            return cstFunc;
        }

        /***************************************************/

        [Description("Gets the default ConstructorInfo from an IEnvironmentalMetric type matching the provided MetricType.")]
        private static System.Reflection.ConstructorInfo GetEnvironmentalMetricConstructorInfo(MetricType metricType)
        {
            Type metricClassType = BH.Engine.Base.Query.BHoMTypeList().Where(x => typeof(IEnvironmentalMetric).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract).FirstOrDefault(x => x.Name == metricType.ToString() + "Metric");

            if (metricClassType == null)
                return null;

            return metricClassType.GetConstructor(Type.EmptyTypes);
        }

        /***************************************************/
        /**** Private Fields                            ****/
        /***************************************************/

        //Storage of the pre-compiled functions for future usage
        private static ConcurrentDictionary<MetricType, Func<IEnvironmentalMetric>> m_EnvironmentalMetricConstructors = new ConcurrentDictionary<MetricType, Func<IEnvironmentalMetric>>();

        /***************************************************/
    }
}

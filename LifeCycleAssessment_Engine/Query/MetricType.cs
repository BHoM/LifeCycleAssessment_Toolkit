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

using BH.Engine.Base;
using BH.Engine.Matter;
using BH.oM.Base;
using BH.oM.Base.Attributes;
using BH.oM.Dimensional;
using BH.oM.LifeCycleAssessment;
using BH.oM.LifeCycleAssessment.Configs;
using BH.oM.LifeCycleAssessment.Fragments;
using BH.oM.LifeCycleAssessment.Interfaces;
using BH.oM.LifeCycleAssessment.MaterialFragments;
using BH.oM.LifeCycleAssessment.MaterialFragments.Transport;
using BH.oM.LifeCycleAssessment.Results;
using BH.oM.Physical.Materials;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace BH.Engine.LifeCycleAssessment
{
    public static partial class Query
    {

        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static MetricType IMetricType(this ILifeCycleAssemsmentIndicator factor)
        {
            if (factor == null)
                return oM.LifeCycleAssessment.MetricType.Undefined;

            Type t = factor.GetType();
            if (m_MetricTypes.TryGetValue(t, out MetricType metricType))
                return metricType;

            //Gets the metric type that best matches the type name.
            //In general, the metricType should start with the type name
            //For some cases, there are some overlap, like ClimateChangeTotal vs CliamteChangeTotalNoBiogenic
            //For this case, the longest of the two names is picked, as that should be the closer match.
            metricType = Enum.GetValues(typeof(oM.LifeCycleAssessment.MetricType)).Cast<MetricType?>().Where(x => t.Name.StartsWith(x.ToString())).OrderByDescending(x => x.ToString().Length).FirstOrDefault() ?? oM.LifeCycleAssessment.MetricType.Undefined;

            m_MetricTypes[t] = metricType;

            return metricType;
        }

        /***************************************************/
        /**** Private feilds                            ****/
        /***************************************************/

        private static Dictionary<Type,MetricType> m_MetricTypes = new Dictionary<Type,MetricType>();

        /***************************************************/
    }
}



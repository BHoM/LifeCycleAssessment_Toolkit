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
using BH.oM.LifeCycleAssessment.MaterialFragments;
using BH.oM.LifeCycleAssessment.MaterialFragments.EnvironmentalFactors;
using BH.oM.LifeCycleAssessment.MaterialFragments.Transport;
using BH.oM.LifeCycleAssessment.Results;
using BH.oM.LifeCycleAssessment.Results.MetricsValues;
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

        [Description("Gets the factors for each module as a dictioanry.")]
        [Input("moduleFactors", "The factors to extract the dicitoanry from.")]
        [Output("FactorsDictionary", "The factors for each module stored on the metric.")]
        public static IReadOnlyDynamicProperties<Module, IEnvironmentalFactor> IFactors(this IEnvironmentalMetricFactors moduleFactors)
        {
            return Factors(moduleFactors as dynamic);
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        [Description("Gets the factors for each module as a dictioanry.")]
        [Input("moduleFactors", "The factors to extract the dicitoanry from.")]
        [Output("FactorsDictionary", "The factors for each module stored on the metric.")]
        public static IReadOnlyDynamicProperties<Module, IEnvironmentalFactor> Factors<T>(this EnvironmentalMetricFactors<T> moduleFactors)
            where T : class, IEnvironmentalFactor, new()
        {
            return moduleFactors.Factors;
        }

        /***************************************************/
        [Description("Gets the factors for each module as a dictioanry.")]
        [Input("moduleFactors", "The factors to extract the dicitoanry from.")]
        [Output("FactorsDictionary", "The factors for each module stored on the metric.")]
        public static IReadOnlyDictionary<Module, IEnvironmentalFactor> Factors(this IEnvironmentalMetricFactors moduleFactors)
        {
            if (BH.Engine.Base.Compute.TryRunExtensionMethod(moduleFactors, "Factors", out object result) && result != null && result is IReadOnlyDictionary<Module, IEnvironmentalFactor>)
            { 
                return result as IReadOnlyDictionary<Module, IEnvironmentalFactor>;
            }

            BH.Engine.Base.Compute.RecordError($"Unable to extract factors dictionary from module factors of type {moduleFactors.GetType()}.");
            return null;
        }

        /***************************************************/
    }
}



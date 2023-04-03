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

        [Description("Evaluates all or selected metrics stored on the EnvironmentalProductDeclaration (EPD) and returns a result per metric.\n" +
                     "Each metric is evaluated by multiplying the values for each phase by the provided quantityValue.\n" +
                     "Please be mindful that the unit of the quantityValue should match the QuantityType on the EnvironmentalProductDeclaration.")]
        [Input("epd", "The EnvironmentalProductDeclaration to evaluate. Returned results will correspond to all, or selected, metrics stored on this object.")]
        [Input("quantityValue", "The quatity value to evaluate all metrics by. All metric properties will be multiplied by this value. Quatity should correspond to the QuantityType on the EPD.")]
        [Input("materialName", "The name of the Material that owns the EnvironmentalProductDeclaration. Stored as an identifier on the returned result classes.")]
        [Input("metricTypes", "Optional filter for the provided EnvironmentalProductDeclaration for selecting one or more of the provided metrics for calculation. This method also accepts multiple metric types simultaneously. If nothing is provided then no filtering is assumed, i.e. all metrics on the found EPDS are evaluated.")]
        [Output("results", "List of MaterialResults corresponding to the evaluated metrics on the EPD.")]
        public static List<MaterialResult2> EvaluateEnvironmentalProductDeclaration(EnvironmentalProductDeclaration2 epd, double quantityValue, string materialName, List<Type> metricTypes = null)
        {
            if (epd == null)
            {
                Base.Compute.RecordError($"Cannot evaluate a null {nameof(EnvironmentalProductDeclaration2)}.");
                return null;
            }

            List<MaterialResult2> results = new List<MaterialResult2>();

            foreach (IEnvironmentalMetric metric in epd.FilteredMetrics(metricTypes))
            {
                results.Add(EvaluateEnvironmentalMetric(metric, epd.Name, materialName, quantityValue));
            }

            return results;
        }

        /***************************************************/
    }
}

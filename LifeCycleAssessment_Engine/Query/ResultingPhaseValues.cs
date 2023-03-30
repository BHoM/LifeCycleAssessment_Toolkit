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

using BH.oM.Base;
using BH.oM.Base.Attributes;
using BH.oM.LifeCycleAssessment.MaterialFragments;
using System;
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

        [Description("Gets the resulting values for each phase of the providing EnvironmentalMetric given the provided quantityValue.\n" +
                     "The resulting values are computed as the values on the metric for each phase multiplied by the quantity value.\n" +
                     "Please be mindful that the unit of the quantityValue should match the QuantityType on the EnvironmentalProductDeclaration to which the metric belongs.")]
        [Input("metric", "The IEnvironmentalMetric to get resulting values for. All phase values on the metric will be extracted and multiplied by the qunatityValue.")]
        [Input("quantityValue", "The quatity value to evaluate all metrics by. All metric properties will be multiplied by this value. Quatity should correspond to the QuantityType on the EPD.")]
        [Output("resultValues", "The resulting values for each phase.")]
        public static List<double> ResultingPhaseValues(this IEnvironmentalMetric metric, double quantityValue)
        {
            if (metric == null)
            {
                Base.Compute.RecordError($"Cannot get the resulting values from a null {nameof(IEnvironmentalMetric)}.");
                return null;
            }

            List<double> resultingValues = new List<double>();
            foreach (double phaseData in metric.IPhaseDataValues())
            {
                resultingValues.Add(phaseData * quantityValue);  //Evaluation value is base phase data multiplied by quantity value
            }
            return resultingValues;
        }

        /***************************************************/
    }
}

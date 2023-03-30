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

        [Description("Evaluates the IEnvironmentalMetric and returns a MaterialResult of a type corresponding to the metric. The evaluation is done by multiplying all phase data on the metric by the provided quantityValue.\n" +
                     "Please be mindful that the unit of the quantityValue should match the QuantityType on the EnvironmentalProductDeclaration to which the metric belongs.")]
        [Input("metric", "The IEnvironmentalMetric to evaluate. Returned result will be a MaterialResult of a type corresponding to the metric.")]
        [Input("quantityValue", "The quatity value to evaluate all metrics by. All metric properties will be multiplied by this value. Quatity should correspond to the QuantityType on the EPD.")]
        [Input("epdName", "The name of the EnvironmentalProductDeclaration that owns the IEnvironmentalMetric. Stored as an identifier on the returned result class.")]
        [Input("materialName", "The name of the Material that owns the EnvironmentalProductDeclaration. Stored as an identifier on the returned result class.")]
        [Output("result", "A MaterialResult of a type corresponding to the evaluated metric with phase data calculated as data on metric multiplied by the provided quantity value.")]
        public static MaterialResult2 EvaluateEnvironmentalMetric(IEnvironmentalMetric metric, string epdName, string materialName, double quantityValue)
        {
            if (metric == null)
            {
                Base.Compute.RecordError($"Cannot evaluate a null {nameof(IEnvironmentalMetric)}.");
                return null;
            }

            //Get the constructor for the material result of the type corresponding to the metric currently being evaluated
            //This is done by finding the MaterialResult with matching name and exatracting the constructor from it
            //The constructor is pre-compiled to a function to speed up the execution of the particular method
            Func<object[], MaterialResult2> cst = Query.MaterialResultConstructor(metric.GetType());

            //Collect all the relevant data for constructor (essentailly, all properties for the result in correct order)
            //First two parameters of all MaterialResults should always be name of the material and name of the EPD
            List<object> parameters = new List<object> { materialName, epdName };
            //Collect the rest of the evaluation metrics
            //For most cases this will be the phases 
            //Imporant that the order of the metrics extracted cooresponds to the order of the constructor
            //General order should always be all the default phases (A1-A5, B1-B7, C1-C4 and D) followed by any additional phases corresponding to the metric currently being evaluated
            //For example, GlobalWarmpingPotential will have an additional property corresponding to BiogenicCarbon
            parameters.AddRange(metric.ResultingPhaseValues(quantityValue).Cast<object>());  //Gets the resulting final metrics for each phase from the metric

            //Call the constructor function
            return cst(parameters.ToArray());
        }

        /***************************************************/
    }
}

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
    }
}

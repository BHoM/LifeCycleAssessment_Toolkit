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
using BH.oM.LifeCycleAssessment;
using BH.oM.LifeCycleAssessment.MaterialFragments;
using BH.oM.LifeCycleAssessment.Results;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace BH.Engine.LifeCycleAssessment
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods - Interface                ****/
        /***************************************************/

        [Description("Gets a list of doubles corresponding to the sum of values for each property of the provided ILifeCycleAssessmentPhaseData, i.e. the first value will be the sum of A1 for all the provided ILifeCycleAssessmentPhaseDatas.")]
        [Input("phaseData", "List of ILifeCycleAssessmentPhaseData to get the sum data from.")]
        [Output("values", "The values of the summed up material results as list of doubles where each item in the list corresponds to the sum for a particular phase, i.e. first item in the list will be the sum of all A1s in the provided phase data.")]
        public static List<double> SumPhaseDataValues(this IReadOnlyList<ILifeCycleAssessmentPhaseData> phaseData)
        {
            if(phaseData.IsNullOrEmpty())
                return new List<double>();

            ILifeCycleAssessmentPhaseData first = phaseData[0];
            if (phaseData.Count > 1)
            {
                //If more than one value provided, check that all are of the same type
                Type firstType = first.GetType();
                if (phaseData.Skip(1).Any(x => x.GetType() != firstType))
                {
                    Base.Compute.RecordError($"Only able to sum {nameof(ILifeCycleAssessmentPhaseData)} of the same type.");
                    return new List<double>();
                }
            }

            //Set up the base as the phase values for the first
            List<double> sum = first.IPhaseDataValues();

            //Loop through the rest (all but the first)
            for (int i = 1; i < phaseData.Count; i++)
            {
                //Get out data for the currently
                //Will be the same length and order as the first, as checked for matching type already
                List<double> current = phaseData[i].IPhaseDataValues();

                //Loop through all the phase data and add to the sum
                for (int j = 0; j < sum.Count; j++)
                {
                    sum[j] += current[j];
                }
            }

            return sum;
        }

        /***************************************************/
    }
}

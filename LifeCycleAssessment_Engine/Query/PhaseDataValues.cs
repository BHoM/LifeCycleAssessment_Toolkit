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

using BH.oM.Base;
using BH.oM.Base.Attributes;
using BH.oM.LifeCycleAssessment;
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

        [Description("Gets the values corresponding to the values of all phases as a list of doubles.")]
        [Input("phaseData", "ILifeCycleAssessmentPhaseData from which to extract all phase data values.")]
        [Output("values", "The values of the phase data as a list of doubles.")]
        public static List<double> IPhaseDataValues(this ILifeCycleAssessmentPhaseData phaseData)
        {
            if (phaseData == null)
            {
                Base.Compute.RecordError($"Unable to extract phase data values from a null {nameof(ILifeCycleAssessmentPhaseData)}.");
                return new List<double>();
            }
            return PhaseDataValues(phaseData as dynamic);
        }

        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/



        /***************************************************/
        /**** Private Methods - Fallback                ****/
        /***************************************************/
        
        [Description("Gets the values corresponding to the values of all phases as a list of doubles. Fallback case that is relevant for all metric without any additional phases outside the standard phase data.")]
        [Input("phaseData", "ILifeCycleAssessmentPhaseData from which to extract all phase data values.")]
        [Output("values", "The values of the phase data as a list of doubles.")]
        private static List<double> PhaseDataValues(this ILifeCycleAssessmentPhaseData phaseData)
        {
            return BasePhaseDataValues(phaseData);
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        [Description("Gets a list of doubles corresponding to the base phases applicable to all ILifeCycleAssessmentPhaseData.")]
        [Input("phaseData", "ILifeCycleAssessmentPhaseData from which to extract all base phase data values common to all metric/result types.")]
        [Output("values", "The values of the base phase data as a list of doubles.")]
        private static List<double> BasePhaseDataValues(this ILifeCycleAssessmentPhaseData phaseData)
        {
            return new List<double>{
                        phaseData.A1,
                        phaseData.A2,
                        phaseData.A3,
                        phaseData.A1toA3,
                        phaseData.A4,
                        phaseData.A5,
                        phaseData.B1,
                        phaseData.B2,
                        phaseData.B3,
                        phaseData.B4,
                        phaseData.B5,
                        phaseData.B6,
                        phaseData.B7,
                        phaseData.B1toB7,
                        phaseData.C1,
                        phaseData.C2,
                        phaseData.C3,
                        phaseData.C4,
                        phaseData.C1toC4,
                        phaseData.D};
        }

        /***************************************************/
    }
}



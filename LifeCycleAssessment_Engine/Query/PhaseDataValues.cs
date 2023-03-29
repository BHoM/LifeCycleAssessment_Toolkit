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
        [Output("values", "The values of the phaseData as a list of doubles.")]
        public static List<double> IPhaseDataValues(this ILifeCycleAssessmentPhaseData phaseData)
        {
            return PhaseDataValues(phaseData as dynamic);
        }

        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/


        [Description("Gets a list of doubles corresponding to the metric values being evaluated for the particular GlobalWarmingPotentialMetrics.")]
        [Input("phaseData", "GlobalWarmingPotentialMetrics being evaluated.")]
        [Input("quantityValue", "The quatity value to evaluate the metric by. All metric properties will be multiplied by this value. Quatity should correspond to the QueantityType on the EPD to which the metric belongs.")]
        [Output("values", "The values of the evaluated metric as a list of doubles.")]
        public static List<double> PhaseDataValues(this IGlobalWarmingPotentialPhaseData phaseData)
        {
            List<double> metrics = BasePhaseDataValues(phaseData);
            metrics.Add(phaseData.BiogenicCarbon);
            return metrics;
        }

        /***************************************************/
        /**** Private Methods - Fallback                ****/
        /***************************************************/

        [Description("Gets a list of doubles corresponding to the metric values being evaluated for the particular IEnvironmentalMetric. Fallback case that is relevant for all metric without any additional phases outside the standard phase data.")]
        [Input("phaseData", "IEnvironmentalMetric being evaluated.")]
        [Input("quantityValue", "The quatity value to evaluate the metric by. All metric properties will be multiplied by this value. Quatity should correspond to the QueantityType on the EPD to which the metric belongs.")]
        [Output("values", "The values of the evaluated metric as a list of doubles.")]
        private static List<double> PhaseDataValues(this ILifeCycleAssessmentPhaseData phaseData)
        {
            return BasePhaseDataValues(phaseData);
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        [Description("Gets a list of doubles corresponding to the base phases applicable to all IEnvironmentalMetric.")]
        [Input("phaseData", "IEnvironmentalMetric being evaluated.")]
        [Input("quantityValue", "The quatity value to evaluate the metric by. All metric properties will be multiplied by this value. Quatity should correspond to the QueantityType on the EPD to which the metric belongs.")]
        [Output("values", "The values of the evaluated metric as a list of doubles.")]
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
                        phaseData.C1,
                        phaseData.C2,
                        phaseData.C3,
                        phaseData.C4,
                        phaseData.D};
        }

        /***************************************************/
    }
}

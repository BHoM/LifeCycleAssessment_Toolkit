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
        /**** Public Methods - Interface                ****/
        /***************************************************/

        [Description("Gets a list of doubles corresponding to the metric values being evaluated for the particular IEnvironmentalMetric.")]
        [Input("phaseData", "ILifeCycleAssessmentPhaseData from which to extract all phase data values to be scaled by the quantityValue.")]
        [Input("quantityValue", "The quatity value to evaluate the metric by. All metric properties will be multiplied by this value. Quatity should correspond to the QueantityType on the EPD to which the metric belongs for the case where the PhaseData is an IEnvironmentalMetric.")]
        [Output("values", "The values of the evaluated metric as a list of doubles.")]
        public static List<double> IScalePhaseDataValues(this ILifeCycleAssessmentPhaseData phaseData, double quantityValue)
        {
            return ScalePhaseDataValues(phaseData as dynamic, quantityValue);
        }

        /***************************************************/

        [Description("Gets a list of doubles corresponding to the metric values being evaluated for the particular IEnvironmentalMetric.")]
        [Input("phaseData", "ILifeCycleAssessmentPhaseData from which to extract all phase data values to be scaled by the quantityValue.")]
        [Input("quantityValue", "The quatity value to evaluate the metric by. All metric properties will be multiplied by this value. Quatity should correspond to the QueantityType on the EPD to which the metric belongs for the case where the PhaseData is an IEnvironmentalMetric.")]
        [Output("values", "The values of the evaluated metric as a list of doubles.")]
        public static List<double> ScalePhaseDataValues2(this ILifeCycleAssessmentPhaseData phaseData, double quantityValue)
        {
            return phaseData.IPhaseDataValues().Select(x => x * quantityValue).ToList();
        }

        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/


        [Description("Gets a list of doubles corresponding to the metric values being evaluated for the particular GlobalWarmingPotentialMetrics.")]
        [Input("phaseData", "GlobalWarmingPotentialMetrics being evaluated.")]
        [Input("quantityValue", "The quatity value to evaluate the metric by. All metric properties will be multiplied by this value. Quatity should correspond to the QueantityType on the EPD to which the metric belongs.")]
        [Output("values", "The values of the evaluated metric as a list of doubles.")]
        public static List<double> ScalePhaseDataValues(this IGlobalWarmingPotentialPhaseData phaseData, double quantityValue)
        {
            List<double> metrics = ScaleBasePhaseDataValues(phaseData, quantityValue);
            metrics.Add(phaseData.BiogenicCarbon * quantityValue);
            return metrics;
        }

        /***************************************************/
        /**** Private Methods - Fallback                ****/
        /***************************************************/

        [Description("Gets a list of doubles corresponding to the metric values being evaluated for the particular IEnvironmentalMetric. Fallback case that is relevant for all metric without any additional phases outside the standard phase data.")]
        [Input("phaseData", "IEnvironmentalMetric being evaluated.")]
        [Input("quantityValue", "The quatity value to evaluate the metric by. All metric properties will be multiplied by this value. Quatity should correspond to the QueantityType on the EPD to which the metric belongs.")]
        [Output("values", "The values of the evaluated metric as a list of doubles.")]
        private static List<double> ScalePhaseDataValues(this ILifeCycleAssessmentPhaseData phaseData, double quantityValue)
        {
            return ScaleBasePhaseDataValues(phaseData, quantityValue);
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        [Description("Gets a list of doubles corresponding to the base phases applicable to all IEnvironmentalMetric.")]
        [Input("phaseData", "IEnvironmentalMetric being evaluated.")]
        [Input("quantityValue", "The quatity value to evaluate the metric by. All metric properties will be multiplied by this value. Quatity should correspond to the QueantityType on the EPD to which the metric belongs.")]
        [Output("values", "The values of the evaluated metric as a list of doubles.")]
        private static List<double> ScaleBasePhaseDataValues(this ILifeCycleAssessmentPhaseData phaseData, double quantityValue)
        {
            return new List<double>{
                        quantityValue * phaseData.A1,
                        quantityValue * phaseData.A2,
                        quantityValue * phaseData.A3,
                        quantityValue * phaseData.A1toA3,
                        quantityValue * phaseData.A4,
                        quantityValue * phaseData.A5,
                        quantityValue * phaseData.B1,
                        quantityValue * phaseData.B2,
                        quantityValue * phaseData.B3,
                        quantityValue * phaseData.B4,
                        quantityValue * phaseData.B5,
                        quantityValue * phaseData.B6,
                        quantityValue * phaseData.B7,
                        quantityValue * phaseData.C1,
                        quantityValue * phaseData.C2,
                        quantityValue * phaseData.C3,
                        quantityValue * phaseData.C4,
                        quantityValue * phaseData.D};
        }

        /***************************************************/
    }
}

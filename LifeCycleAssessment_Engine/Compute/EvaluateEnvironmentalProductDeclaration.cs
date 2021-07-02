/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2021, the respective contributors. All rights reserved.
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

using BH.oM.Dimensional;
using BH.oM.LifeCycleAssessment;
using BH.oM.LifeCycleAssessment.Results;
using BH.oM.Reflection.Attributes;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace BH.Engine.LifeCycleAssessment
{
    public static partial class Compute
    {
        /***************************************************/
        /****   Public Methods                          ****/
        /***************************************************/

        [Description("This method calculates the results of any selected metric within an Environmental Product Declaration. For example for an EPD of QuantityType Volume, results will reflect the objects volume * EPD Field metric.")]
        [Input("elementM", "This is a BHoM object used to calculate EPD metric. This obj must have an EPD MaterialFragment applied to the object.")]
        [Input("phases", "Provide phases of life you wish to evaluate. Phases of life must be documented within EPDs for this method to work.")]
        [Input("field", "Filter the provided EnvironmentalProductDeclaration by selecting one of the provided metrics for calculation. This method also accepts multiple fields simultaneously.")]
        [Input("exactMatch", "If true, the evaluation method will force an exact LCA phase match to solve for.")]
        [Output("result", "A LifeCycleElementResult that contains the LifeCycleAssessment data for the input object.")]
        public static LifeCycleAssessmentElementResult EvaluateEnvironmentalProductDeclaration(IElementM elementM, List<LifeCycleAssessmentPhases> phases, EnvironmentalProductDeclarationField field = EnvironmentalProductDeclarationField.GlobalWarmingPotential, bool exactMatch = false)
        {
            double value = 0;
            EnvironmentalMetricResult resultValue = null;

            List<QuantityType> qts = elementM.GetQuantityType();

            qts = qts.Distinct().ToList();

            foreach (QuantityType qt in qts)
            {
                switch (qt)
                {
                    case QuantityType.Undefined:
                        BH.Engine.Reflection.Compute.RecordError("The object's EPD QuantityType is Undefined and cannot be evaluated.");
                        return null;
                    case QuantityType.Area:
                        BH.Engine.Reflection.Compute.RecordNote("Evaluating object type: " + elementM.GetType() + " based on EPD Area QuantityType.");
                        var evalByArea = EvaluateEnvironmentalProductDeclarationByArea(elementM, phases, field, exactMatch);
                        value += evalByArea.Quantity;
                        if (resultValue == null)
                            resultValue = evalByArea;
                        break;
                    case QuantityType.Ampere:
                        BH.Engine.Reflection.Compute.RecordError("Ampere QuantityType is currently not supported.");
                        return null;
                    case QuantityType.Item:
                        BH.Engine.Reflection.Compute.RecordError("Length QuantityType is currently not supported. Try a different EPD with QuantityType values of either Area, Volume, or Mass.");
                        return null;
                    case QuantityType.Length:
                        BH.Engine.Reflection.Compute.RecordNote("Evaluating object type: " + elementM.GetType() + " based on EPD Length QuantityType.");
                        var evalByLength = EvaluateEnvironmentalProductDeclarationByLength(elementM, phases, field, exactMatch);
                        value += evalByLength.Quantity;
                        if (resultValue == null)
                            resultValue = evalByLength;
                        break;
                    case QuantityType.Mass:
                        BH.Engine.Reflection.Compute.RecordNote("Evaluating object type: " + elementM.GetType() + " based on EPD Mass QuantityType.");
                        var evalByMass = EvaluateEnvironmentalProductDeclarationByMass(elementM, phases, field, exactMatch);
                        value += evalByMass.Quantity;
                        if (resultValue == null)
                            resultValue = evalByMass;
                        break;
                    case QuantityType.Watt:
                        BH.Engine.Reflection.Compute.RecordError("Watt QuantityType is currently not supported.");
                        return null;
                    case QuantityType.VoltAmps:
                        BH.Engine.Reflection.Compute.RecordError("VoltAmps QuantityType is currently not supported.");
                        return null;
                    case QuantityType.Volume:
                        BH.Engine.Reflection.Compute.RecordNote("Evaluating object type: " + elementM.GetType() + " based on EPD Volume QuantityType.");
                        var evalByVolume = EvaluateEnvironmentalProductDeclarationByVolume(elementM, phases, field, exactMatch);
                        value += evalByVolume.Quantity;
                        if (resultValue == null)
                            resultValue = evalByVolume;
                        break;
                    case QuantityType.VolumetricFlowRate:
                        BH.Engine.Reflection.Compute.RecordError("VolumetricFlowRate QuantityType is currently not supported.");
                        return null;
                    default:
                        BH.Engine.Reflection.Compute.RecordWarning("The object you have provided does not contain an EPD Material Fragment.");
                        return null;
                }
            }

            resultValue.Quantity = value;
            resultValue.EnvironmentalProductDeclaration = elementM.GetElementEpd();
            return resultValue;
        }
        /***************************************************/


    }
}

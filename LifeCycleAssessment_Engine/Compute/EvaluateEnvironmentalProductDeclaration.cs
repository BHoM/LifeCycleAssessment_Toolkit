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
using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using BH.oM.Base;
using BH.oM.Reflection.Attributes;
using BH.oM.LifeCycleAssessment;
using BH.oM.Quantities.Attributes;
using BH.oM.LifeCycleAssessment.MaterialFragments;
using BH.oM.LifeCycleAssessment.Results;
using BH.oM.Physical.Elements;
using BH.oM.Dimensional;
using BH.Engine.Physical;
using BH.Engine.Base;
using BH.Engine.Reflection;
using BH.Engine.Spatial;
using BH.Engine.Matter;
using BH.oM.Reflection;

namespace BH.Engine.LifeCycleAssessment
{
    public static partial class Compute
    {
        /***************************************************/
        /****   Public Methods                          ****/
        /***************************************************/
        [Description("This method calculates the results of any selected metric within an Environmental Product Declaration. For example for an EPD of QuantityType Volume, results will reflect the objects volume * EPD Field metric.")]
        [Input("elementM", "This is a BHoM object used to calculate EPD metric. This obj must have an EPD MaterialFragment applied to the object.")]
        [Input("field", "Filter the provided EnvironmentalProductDeclaration by selecting one of the provided metrics for calculation. This method also accepts multiple fields simultaneously.")]
        [MultiOutput(0, "resultPerArea", "The LCA result of the object being evaluated using an EPD with Area QuantityType.")]
        [MultiOutput(1, "resultPerMass", "The LCA result of the object being evaluated using an EPD with Mass QuantityType")]
        [MultiOutput(2, "resultPerVolume", "The LCA result of the object being evaluated using an EPD with Volume QuantityType")]
        public static Output<LifeCycleAssessmentElementResult, LifeCycleAssessmentElementResult, LifeCycleAssessmentElementResult> EvaluateEnvironmentalProductDeclarationPerObject(IElementM elementM, EnvironmentalProductDeclarationField field = EnvironmentalProductDeclarationField.GlobalWarmingPotential)
        {
            GlobalWarmingPotentialResult area = null;
            GlobalWarmingPotentialResult mass = null;
            GlobalWarmingPotentialResult volume = null;            

            List<QuantityType> qts = elementM.GetFragmentQuantityType();

            foreach (QuantityType qt in qts)
            {
                switch (qt)
                {
                    case QuantityType.Undefined:
                        BH.Engine.Reflection.Compute.RecordError("The object's EPD QuantityType is Undefined and cannot be evaluated.");
                        return null;
                    case QuantityType.Item:
                        BH.Engine.Reflection.Compute.RecordError("Length QuantityType is currently not supported. Try a different EPD with QuantityType values of either Area, Volume, or Mass.");
                        return null;
                    case QuantityType.Length:
                        BH.Engine.Reflection.Compute.RecordError("Length QuantityType is currently not supported. Try a different EPD with QuantityType values of either Area, Volume, or Mass.");
                        return null;
                    case QuantityType.Area:
                        BH.Engine.Reflection.Compute.RecordNote("Evaluating object type: " + elementM.GetType() + " based on EPD Area QuantityType.");
                        var evalByArea = EvaluateEnvironmentalProductDeclarationByArea(elementM, field);
                        if (area == null)
                            area = evalByArea;
                        else
                            area.GlobalWarmingPotential += evalByArea.GlobalWarmingPotential;
                        break;
                    case QuantityType.Volume:
                        BH.Engine.Reflection.Compute.RecordNote("Evaluating object type: " + elementM.GetType() + " based on EPD Volume QuantityType.");
                        var evalByVolume = EvaluateEnvironmentalProductDeclarationByVolume(elementM, field);
                        if (volume == null)
                            volume = evalByVolume;
                        else
                            volume.GlobalWarmingPotential += evalByVolume.GlobalWarmingPotential;
                        break;
                    case QuantityType.Mass:
                        BH.Engine.Reflection.Compute.RecordNote("Evaluating object type: " + elementM.GetType() + " based on EPD Mass QuantityType.");
                        var evalByMass = EvaluateEnvironmentalProductDeclarationByMass(elementM, field);
                        if (mass == null)
                            mass = evalByMass;
                        else
                            mass.GlobalWarmingPotential += evalByMass.GlobalWarmingPotential;
                        break;
                    default:
                        BH.Engine.Reflection.Compute.RecordWarning("The object you have provided does not contain an EPD Material Fragment.");
                        return null;
                }
            }

            return new Output<LifeCycleAssessmentElementResult, LifeCycleAssessmentElementResult, LifeCycleAssessmentElementResult>
            {
                Item1 = area,
                Item2 = mass,
                Item3 = volume,
            };
        }
        /***************************************************/


    }
}

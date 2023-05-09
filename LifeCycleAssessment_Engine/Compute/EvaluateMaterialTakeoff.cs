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
using BH.oM.Dimensional;
using BH.oM.LifeCycleAssessment.MaterialFragments;
using BH.oM.LifeCycleAssessment;
using BH.oM.LifeCycleAssessment.Results;
using BH.oM.Physical.Materials;
using BH.oM.Quantities.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using BH.oM.LifeCycleAssessment.Fragments;
using BH.Engine.Base;

namespace BH.Engine.LifeCycleAssessment
{
    public static partial class Compute
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [PreviousVersion("6.2", "BH.Engine.LifeCycleAssessment.Compute.EvaluateMaterialTakeoff(BH.oM.Physical.Materials.VolumetricMaterialTakeoff, System.Collections.Generic.List<BH.oM.LifeCycleAssessment.LifeCycleAssessmentPhases>, BH.oM.LifeCycleAssessment.EnvironmentalProductDeclarationField, System.Boolean)")]
        [Description("Evaluates the materials in the VolumetricMaterialTakeoff and returns a MaterialResult per material in the takeoff. Requires the materials in the Takeoff to have EPDs assigned. Please use the AssignTemplate methods before calling this method.")]
        [Input("materialTakeoff", "The volumetric material takeoff to evaluate.")]
        [Input("templateMaterials", "Template materials to match to and assign properties from onto the model materials. Should generally have unique names. EPDs should be assigned to these materials and will be mapped over to the materials on the element with the same name and used in the evaluation.")]
        [Input("prioritiseTemplate", "Controls if main material or map material should be prioritised when conflicting information is found on both in terms of Density and/or Properties. If true, map is prioritised, if false, main material is prioritised.")]
        [Input("metricTypes", "Optional filter for the provided EnvironmentalProductDeclaration for selecting one or more of the provided metrics for calculation. This method also accepts multiple metric types simultaneously. If nothing is provided then no filtering is assumed, i.e. all metrics on the found EPDS are evaluated.")]
        [Output("result", "A MaterialResult per material and per metric that contains the LifeCycleAssessment data for the input takeoff.")]
        public static List<MaterialResult> EvaluateMaterialTakeoff(this GeneralMaterialTakeoff materialTakeoff, List<Material> templateMaterials = null, bool prioritiseTemplate = true, List<Type> metricTypes = null)
        {
            if (materialTakeoff == null)
                return new List<MaterialResult>();

            GeneralMaterialTakeoff mappedTakeoff;
            if (templateMaterials == null || templateMaterials.Count == 0)
                mappedTakeoff = materialTakeoff;
            else
                mappedTakeoff = Matter.Modify.AssignTemplate(materialTakeoff, templateMaterials, prioritiseTemplate);

            List<MaterialResult> materialResults = new List<MaterialResult>();

            for (int i = 0; i < mappedTakeoff.MaterialTakeoffItems.Count; i++)
            {
                TakeoffItem takeoffItem = mappedTakeoff.MaterialTakeoffItems[i];
                Material material = takeoffItem.Material;
                EnvironmentalProductDeclaration epd = material.Properties.OfType<EnvironmentalProductDeclaration>().FirstOrDefault();

                if (epd == null)
                {
                    Base.Compute.RecordError($"EPD not set to material {material.Name}. Unable to evaluate element.");
                    return null;
                }

                double quantityValue;
                switch (epd.QuantityType)
                {
                    case QuantityType.Volume:
                        quantityValue = takeoffItem.Volume;
                        break;
                    case QuantityType.Mass:
                        quantityValue = takeoffItem.Mass;
                        if (double.IsNaN(quantityValue) || quantityValue == 0)
                        {
                            double vol = takeoffItem.Volume;
                            if (vol == 0)
                                quantityValue = 0;
                            else if(!double.IsNaN(vol))
                            {
                                double density = material.Density;
                                if (double.IsNaN(density) || density == 0)
                                {
                                    EPDDensity epdDensity = epd.FindFragment<EPDDensity>();
                                    if (epdDensity != null)
                                        density = epdDensity.Density;
                                }
                                if(!double.IsNaN(density))
                                    quantityValue = vol * density;
                            }
                        }
                        break;
                    case QuantityType.Length:
                        quantityValue = takeoffItem.Length;
                        break;
                    case QuantityType.Area:
                        quantityValue = takeoffItem.Area;
                        break;
                    case QuantityType.Item:
                        quantityValue = takeoffItem.NumberItem;
                        break;
                    case QuantityType.Ampere:
                        quantityValue = takeoffItem.ElectricCurrent;
                        break;
                    case QuantityType.VoltAmps:
                    case QuantityType.Watt:
                        quantityValue = takeoffItem.Power;
                        break;
                    case QuantityType.VolumetricFlowRate:
                        quantityValue = takeoffItem.VolumetricFlowRate;
                        break;
                    case QuantityType.Energy:
                        quantityValue = takeoffItem.Energy;
                        break;
                    case QuantityType.Undefined:
                    default:
                        Base.Compute.RecordError($"{epd.QuantityType} QuantityType is currently not supported.");
                        return null;
                }

                if (double.IsNaN(quantityValue))
                    BH.Engine.Base.Compute.RecordError($"{epd.QuantityType} is NaN on MaterialTakeoff and will result in NaN result when evaluating epd named {epd.Name}");

                materialResults.AddRange(EvaluateEnvironmentalProductDeclaration(epd, quantityValue, material.Name, metricTypes));
            }

            return materialResults;
        }

        /***************************************************/
    }
}


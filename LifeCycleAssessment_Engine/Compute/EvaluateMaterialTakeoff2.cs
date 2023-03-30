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

namespace BH.Engine.LifeCycleAssessment
{
    public static partial class Compute
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Evaluates the materials in the VolumetricMaterialTakeoff and returns a MaterialResult per material in the takeoff. Requires the materials in the Takeoff to have EPDs assigned. Please use the AssignTemplate methods before calling this method.")]
        [Input("materialTakeoff", "The volumetric material takeoff to evaluate.")]
        [Input("metricTypes", "Optional filter for the provided EnvironmentalProductDeclaration for selecting one or more of the provided metrics for calculation. This method also accepts multiple metric types simultaneously. If nothing is provided then no filtering is assumed, i.e. all metrics on the found EPDS are evaluated.")]
        [Output("result", "A MaterialResult per material and per metric that contains the LifeCycleAssessment data for the input takeoff.")]
        public static List<MaterialResult2> EvaluateMaterialTakeoff(this VolumetricMaterialTakeoff materialTakeoff, List<Type> metricTypes = null)
        {
            if (materialTakeoff == null)
                return new List<MaterialResult2>();

            List<MaterialResult2> materialResults = new List<MaterialResult2>();

            for (int i = 0; i < materialTakeoff.Materials.Count; i++)
            {
                Material material = materialTakeoff.Materials[i];
                EnvironmentalProductDeclaration2 epd = material.Properties.OfType<EnvironmentalProductDeclaration2>().FirstOrDefault();

                if (epd == null)
                {
                    Base.Compute.RecordError($"EPD not set to material {material.Name}. Unable to evaluate element.");
                    return null;
                }

                double quantityValue;
                switch (epd.QuantityType)
                {
                    case QuantityType.Volume:
                        quantityValue = materialTakeoff.Volumes[i];
                        break;
                    case QuantityType.Mass:
                        if (double.IsNaN(material.Density))
                        {
                            Base.Compute.RecordError($"Density is not set for material {material.Name}. Cannot evaluate mass based EPD.");
                            return null;
                        }
                        if (material.Density == 0)
                        {
                            Base.Compute.RecordWarning($"Density of materials {material.Name} is 0 and will give no contribution for evaluating mass based EPD.");
                        }
                        quantityValue = materialTakeoff.Volumes[i] * material.Density;
                        break;
                    case QuantityType.Length:
                    case QuantityType.Area:
                    case QuantityType.Undefined:
                    case QuantityType.Item:
                    case QuantityType.Ampere:
                    case QuantityType.VoltAmps:
                    case QuantityType.VolumetricFlowRate:
                    case QuantityType.Watt:
                    case QuantityType.Energy:
                    default:
                        Base.Compute.RecordError($"{epd.QuantityType} QuantityType is currently not supported.");
                        return null;
                }

                materialResults.AddRange(EvaluateEnvironmentalProductDeclaration(epd, quantityValue, material.Name, metricTypes));
            }

            return materialResults;
        }

        /***************************************************/
    }
}


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

using BH.Engine.Matter;
using BH.oM.Base;
using BH.oM.Base.Attributes;
using BH.oM.Dimensional;
using BH.oM.LifeCycleAssessment;
using BH.oM.LifeCycleAssessment.Configs;
using BH.oM.LifeCycleAssessment.MaterialFragments;
using BH.oM.LifeCycleAssessment.Results;
using BH.oM.Physical.Materials;
using BH.oM.Quantities.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace BH.Engine.LifeCycleAssessment
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [PreviousVersion("6.2", "BH.Engine.LifeCycleAssessment.Compute.ElementEmbodiedCarbon(BH.oM.Dimensional.IElementM, System.Collections.Generic.List<BH.oM.Physical.Materials.Material>, System.Boolean)")]
        [Description("Evaluates the embodied carbon on the provided element based on IStructE methodology of evaluation.\n" +
                    "If you would like to evaluate other EPD metrics, please use one of the other Evaluation methods. \n" +
                    "TemplateMaterials can be provided helping with picking the correct EPD corresponding to each material on the element. Please note that this evaluation method only support mass-based EPDs.")]
        [Input("elementM", "Element for which to evaluate the embodied carbon.")]
        [Input("templateMaterials", "Template materials to match to and assign properties from onto the model materials. Should generally have unique names. EPDs should be assigned to these materials and will be mapped over to the materials on the element with the same name and used in the evaluation. Please note that this evaluation method only support mass-based EPDs.")]
        [Input("prioritiseTemplate", "Controls if main material or map material should be prioritised when conflicting information is found on both in terms of Density and/or Properties. If true, map is prioritised, if false, main material is prioritised.")]
        [Input("projectCost", "Cost of the project. Impact the calculation of the A5 module.")]
        [Input("floorArea", "Total floor area of the project. Impacts calculation of the C1 module.", typeof(Area))]
        [Input("totalWeight", "Total weight of the project. Impacts resulting C1 and A5 values.", typeof(Mass))]
        [Input("a5CarbonFactor", "Factor for A5 evaluation for additional carbon to be added for element based on project totals. Default value provided is generally recomended to be used.", typeof(ClimateChangePerQuantity))]
        [Input("c1CarbonFactor", "Factor on C1 evaluation for all elements. Default value provided is generally recomended to be used.", typeof(ClimateChangePerQuantity))]
        [Output("result", "Result containing the embodied carbon of the element as well as a breakdown per material in the element.")]
        public static List<IElementResult<MaterialResult>> ElementEmbodiedCarbon(this IElementM elementM, List<Material> templateMaterials = null, bool prioritiseTemplate = true, double projectCost = 0, double floorArea = 0, double totalWeight = 0, double a5CarbonFactor = 0.007, double c1CarbonFactor = 3.4)
        {
            List<EnvironmentalMetrics> metricsFilter = new List<EnvironmentalMetrics>
            {
                EnvironmentalMetrics.ClimateChangeTotal,
                EnvironmentalMetrics.ClimateChangeTotalNoBiogenic,
                EnvironmentalMetrics.ClimateChangeFossil,
                EnvironmentalMetrics.ClimateChangeLandUse,
                EnvironmentalMetrics.ClimateChangeBiogenic
            };

            IStructEEvaluationConfig config = new IStructEEvaluationConfig
            {
                ProjectCost = projectCost,
                FloorArea = floorArea,
                TotalWeight = totalWeight,
                A5CarbonFactor = a5CarbonFactor,
                C1CarbonFactor = c1CarbonFactor
            };

            return EvaluateElement(elementM, templateMaterials, prioritiseTemplate, metricsFilter, config);
        }

        /***************************************************/
    }
}


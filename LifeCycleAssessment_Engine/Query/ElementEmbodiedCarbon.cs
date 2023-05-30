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
using BH.oM.LifeCycleAssessment;
using BH.oM.LifeCycleAssessment.MaterialFragments;
using BH.oM.LifeCycleAssessment.Results;
using BH.oM.Physical.Materials;
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
        [Description("Evaluates the embodied carbon on the provided element.\n" +
            "If you would like to evaluate other EPD metrics, please use one of the other Evaluation methods. \n" +
            "TemplateMaterials can be provided helping with picking the correct EPD corresponding to each material on the element.")]
        [Input("elementM", "Element for which to evaluate the embodied carbon.")]
        [Input("templateMaterials", "Template materials to match to and assign properties from onto the model materials. Should generally have unique names. EPDs should be assigned to these materials and will be mapped over to the materials on the element with the same name and used in the evaluation.")]
        [Input("prioritiseTemplate", "Controls if main material or map material should be prioritised when conflicting information is found on both in terms of Density and/or Properties. If true, map is prioritised, if false, main material is prioritised.")]
        [Output("result", "Result containing the embodied carbon of the element as well as a breakdown per material in the element.")]
        public static List<IElementResult<MaterialResult>> ElementEmbodiedCarbon(this IElementM elementM, List<Material> templateMaterials = null, bool prioritiseTemplate = true)
        {
            List<EnvironmentalMetrics> metricsFilter = new List<EnvironmentalMetrics> 
            { 
                EnvironmentalMetrics.ClimateChangeTotal,
                EnvironmentalMetrics.ClimateChangeTotalNoBiogenic,
                EnvironmentalMetrics.ClimateChangeFossil,
                EnvironmentalMetrics.ClimateChangeLandUse,
                EnvironmentalMetrics.ClimateChangeBiogenic
            };

            return EvaluateElement(elementM, templateMaterials, prioritiseTemplate, metricsFilter);           
        }

        /***************************************************/
    }
}


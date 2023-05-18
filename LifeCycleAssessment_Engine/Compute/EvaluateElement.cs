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
using BH.oM.LifeCycleAssessment.Results;
using BH.oM.LifeCycleAssessment;
using BH.oM.Physical.Materials;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using BH.Engine.Matter;
using BH.oM.LifeCycleAssessment.MaterialFragments;
using BH.Engine.Spatial;
using System.Collections.Concurrent;
using System.Reflection;
using BH.Engine.Base;

namespace BH.Engine.LifeCycleAssessment
{
    public static partial class Compute
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [PreviousVersion("6.2", "BH.Engine.LifeCycleAssessment.Compute.EvaluateEnvironmentalProductDeclaration(BH.oM.Dimensional.IElementM, System.Collections.Generic.List<BH.oM.LifeCycleAssessment.LifeCycleAssessmentPhases>, BH.oM.LifeCycleAssessment.EnvironmentalProductDeclarationField, System.Boolean)")]
        [PreviousVersion("6.2", "BH.Engine.LifeCycleAssessment.Compute.EvaluateElement(BH.oM.Dimensional.IElementM, System.Collections.Generic.List<BH.oM.LifeCycleAssessment.LifeCycleAssessmentPhases>, BH.oM.LifeCycleAssessment.EnvironmentalProductDeclarationField, System.Boolean, System.Collections.Generic.List<BH.oM.Physical.Materials.Material>, System.Boolean)")]
        [Description("Evaluates the EnvironmentalMetrics for the provided element and returns an ElementResult for each evaluated metric type.\n" +
                     "Evaluation is done by extracting the material takeoff for the provided element, giving quantities and Materiality.\n" +
                     "Each Material in the takeoff is then evaluated by finding the EnvironmentalProductDeclaration (EPD), either stored on the material or from the list of template materials.\n" +
                     "Each metric, or filtered chosen metrics, on the EPD is then evaluated.\n" +
                     "Finally, an element result is returned per metric type. Each element result being the sum result of all metrics of the same type.")]
        [Input("elementM", "The element to evaluate. The materiality and quantities is extracted from the element.")]
        [Input("templateMaterials", "Template materials to match to and assign properties from onto the model materials. Should generally have unique names. EPDs should be assigned to these materials and will be mapped over to the materials on the element with the same name and used in the evaluation.")]
        [Input("prioritiseTemplate", "Controls if main material or map material should be prioritised when conflicting information is found on both in terms of Density and/or Properties. If true, map is prioritised, if false, main material is prioritised.")]
        [Input("metricFilter", "Optional filter for the provided EnvironmentalProductDeclaration for selecting one or more of the provided metrics for calculation. This method also accepts multiple metric types simultaneously. If nothing is provided then no filtering is assumed, i.e. all metrics on the found EPDS are evaluated.")]
        [Output("result", "A List of ElementResults, one per metric type, that contains the LifeCycleAssessment data for the input object(s).")]
        public static List<IElementResult<MaterialResult>> EvaluateElement(IElementM elementM, List<Material> templateMaterials = null, bool prioritiseTemplate = true, List<EnvironmentalMetrics> metricFilter = null)
        {
            if (elementM == null)
            {
                Base.Compute.RecordError("Unable to evaluate a null element.");
                return null;
            }

            //Gets the material takeoff from the element, with additional material properties mapped over from the provided template materials.
            GeneralMaterialTakeoff takeoff = elementM.IGeneralMaterialTakeoff();

            if (takeoff == null)
            {
                Base.Compute.RecordError($"Unable to extract a MaterialTakeoff from the provided {elementM.GetType().Name}.");
                return null;
            }

            if (takeoff.MaterialTakeoffItems.Count == 0)
            {
                BH.Engine.Base.Compute.RecordWarning($"The {nameof(GeneralMaterialTakeoff)} provided {elementM.GetType().Name} does not contain any {nameof(Material)}s. Nothing to evaluate.");
                return new List<IElementResult<MaterialResult>>();
            }

            List<MaterialResult> materialResults = EvaluateMaterialTakeoff(takeoff, templateMaterials, prioritiseTemplate, metricFilter);

            //Get out id as BHoM_Guid
            IComparable objectId = "";
            if (elementM is IBHoMObject bhObj)
                objectId = bhObj.BHoM_Guid;

            //Groups results by type and sums them up to single ElementResult per type
            return Create.IElementResults(objectId, elementM.ElementScope(), ObjectCategory.Undefined, materialResults);
        }

        /***************************************************/

    }
}

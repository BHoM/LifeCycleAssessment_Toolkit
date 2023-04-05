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
using BH.oM.LifeCycleAssessment.Results;
using BH.oM.LifeCycleAssessment;
using BH.oM.Physical.Materials;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using BH.Engine.Matter;
using BH.Engine.Spatial;
using BH.Engine.Structure;
using System.Dynamic;
using BH.oM.Structure.Elements;
using BH.oM.Physical.Constructions;
using BH.oM.Structure;
using BH.Engine.Base;

namespace BH.Engine.LifeCycleAssessment
{
    public static partial class Compute
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/


        [Description("Evaluates the environmental metrics found within a given EPD. \n" +
            "This method is only fit for the evaluation of phases A1-A3 (cradle to gate) at present. \n" +
            "To view a list of all possible metric evaluations, please view the EPDField enum. Note that not all fields can be evaluated. \n" +
            "The provided Template Materials allow each material within your object to have an associated Environmental Product Declaration.")]
        [Input("elementM", "This is a BHoM object used to calculate EPD metric. This obj must have an EPD MaterialFragment applied to the object.")]
        [Input("phases", "Provide phases of life you wish to evaluate. Phases of life must be documented within EPDs for this method to work.\n" +
            "Note that only phases A1-A3 combined are possible evaluations at present.")]
        [Input("field", "Filter the provided EnvironmentalProductDeclaration by selecting one of the provided metrics for calculation. This method also accepts multiple fields simultaneously.")]
        [Input("exactMatch", "If true, the evaluation method will force an exact LCA phase match to solve for.")]
        [Input("templateMaterials", "Template materials to match to and assign properties from onto the model materials. Should generally have unique names. EPDs should be assigned to these materials and will be mapped over to the materials on the element with the same name and used in the evaluation.")]
        [Input("prioritiseTemplate", "Controls if main material or map material should be prioritised when conflicting information is found on both in terms of Density and/or Properties. If true, map is prioritised, if false, main material is prioritised.")]
        [Output("result", "An ElementResult that contains the LifeCycleAssessment data for the input object(s).")]
        public static ElementResult EvaluateElement(IElementM elementM, List<LifeCycleAssessmentPhases> phases, EnvironmentalProductDeclarationField field = EnvironmentalProductDeclarationField.GlobalWarmingPotential, bool exactMatch = false, List<Material> templateMaterials = null, bool prioritiseTemplate = true)
        {
            if (elementM == null)
            {

                return null;
            }

            VolumetricMaterialTakeoff takeoff = elementM.MappedVolumetricMaterialTakeoff(templateMaterials, true, prioritiseTemplate);

            if (takeoff == null)
            {
                return null;
            }

            if (takeoff.Materials.Count == 0)
                return null;

            //Predefining parameters to be used for some item types in loop
            double area = double.NaN;
            double length = double.NaN;

            List<MaterialResult> materialResults = new List<MaterialResult>();

            for (int i = 0; i < takeoff.Materials.Count; i++)
            {
                Material material = takeoff.Materials[i];
                EnvironmentalProductDeclaration epd = material.Properties.OfType<EnvironmentalProductDeclaration>().FirstOrDefault();

                if (epd == null)
                {
                    Base.Compute.RecordError($"EPD not set to material {material.Name}. Unable to evaluate {field}.");
                    return null;
                }

                double evaluationValue = epd.GetEvaluationValue(field, phases, exactMatch) / epd.QuantityTypeValue;
                if (double.IsNaN(evaluationValue))
                {
                    Base.Compute.RecordError($"EPD {epd.Name} does not contain {field} for phases {string.Join(",", phases)}.");
                    return null;
                }

                double quantityValue;
                switch (epd.QuantityType)
                {
                    case QuantityType.Volume:
                        quantityValue = takeoff.Volumes[i];
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
                        quantityValue = takeoff.Volumes[i] * material.Density;
                        break;
                    case QuantityType.Area:
                        if (double.IsNaN(area))
                        {
                            IElement2D element2D = elementM as IElement2D;
                            if (element2D == null)
                            {
                                Base.Compute.RecordError($"Can only evaluate Area based epds on elements of a {nameof(IElement2D)} type.");
                                return null;
                            }

                            //Check count of layers of material
                            Construction c = null;
                            double layerCount = 1;
                            if(element2D is IAreaElement)
                            {
                                IAreaElement elem = element2D as IAreaElement;
                                c = elem.Property.Construction();
                            }
                            else 
                            { 
                                c = element2D.PropertyValue("Construction") as Construction;
                                if (c == null) c = element2D.PropertyValue("OpeningConstruction") as Construction;
                            }
                            
                            if (c!= null)
                            {
                                double counter = 0;
                                foreach (Layer layer in c.Layers) 
                                {
                                    Material mat = layer.Material;
                                    mat.Fragments.Clear();
                                    material.Fragments.Clear();
                                    string hash = mat.Hash();
                                    if(hash == material.Hash()) counter += 1;
                                }
                                layerCount = counter;
                            }
                            area = element2D.Area() * layerCount;
                        }
                        quantityValue = area;
                        break;
                    case QuantityType.Length:
                        if (double.IsNaN(length))
                        {
                            IElement1D element1D = elementM as IElement1D;
                            if (element1D == null)
                            {
                                Base.Compute.RecordError($"Can only evaluate Area based epds on elements of a {nameof(IElement1D)} type.");
                                return null;
                            }
                            length = element1D.Length();
                        }
                        quantityValue = length;
                        break;
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

                materialResults.Add(new MaterialResult(material.Name, epd.Name, epd.EnvironmentalMetric.Where(x => x.Field == field).First().Phases, quantityValue * evaluationValue, field));
            }

            List<LifeCycleAssessmentPhases> evaluatedPhases = materialResults[0].Phases.ToList();
            bool missMatchedPhases = false;

            for (int i = 1; i < materialResults.Count; i++)
            {
                foreach (LifeCycleAssessmentPhases phase in materialResults[i].Phases)
                {
                    if (!evaluatedPhases.Contains(phase))
                    {
                        missMatchedPhases = true;
                        evaluatedPhases.Add(phase);
                    }
                }
            }
            if (missMatchedPhases)
                Base.Compute.RecordWarning("Missmatch in phases between different materialtypes on element. Please check the results.");

            return new ElementResult(((IBHoMObject)elementM).BHoM_Guid, elementM.GetElementScope(), ObjectCategory.Undefined, evaluatedPhases, materialResults.Sum(x => x.Quantity), field, materialResults);

        }

        /***************************************************/
    }
}


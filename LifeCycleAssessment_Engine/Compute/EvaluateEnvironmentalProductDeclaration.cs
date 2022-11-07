/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2022, the respective contributors. All rights reserved.
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
using BH.oM.Base.Attributes;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using BH.oM.Physical.Materials;
using BH.Engine.Matter;
using BH.Engine.LifeCycleAssessment.Objects;
using BH.oM.LifeCycleAssessment.MaterialFragments;
using BH.Engine.Spatial;
using BH.oM.Base;
using BH.oM.Quantities.Attributes;
using System.Xml.Linq;

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
            MaterialComposition mc = elementM.IMaterialComposition();

            List<QuantityType> qts = elementM.GetQuantityType(mc);

            qts = qts.Distinct().ToList();

            foreach (QuantityType qt in qts)
            {
                switch (qt)
                {
                    case QuantityType.Undefined:
                        BH.Engine.Base.Compute.RecordError("The object's EPD QuantityType is Undefined and cannot be evaluated.");
                        return null;
                    case QuantityType.Area:
                        BH.Engine.Base.Compute.RecordNote("Evaluating object type: " + elementM.GetType() + " based on EPD Area QuantityType.");
                        var evalByArea = EvaluateEnvironmentalProductDeclarationByArea(elementM, phases, mc, field, exactMatch);
                        value += evalByArea.Quantity;
                        if (resultValue == null)
                            resultValue = evalByArea;
                        break;
                    case QuantityType.Ampere:
                        BH.Engine.Base.Compute.RecordError("Ampere QuantityType is currently not supported.");
                        return null;
                    case QuantityType.Energy:
                        BH.Engine.Base.Compute.RecordError("Energy QuantityType is currently not supported.");
                        return null;
                    case QuantityType.Item:
                        BH.Engine.Base.Compute.RecordError("Length QuantityType is currently not supported. Try a different EPD with QuantityType values of either Area, Volume, or Mass.");
                        return null;
                    case QuantityType.Length:
                        BH.Engine.Base.Compute.RecordNote("Evaluating object type: " + elementM.GetType() + " based on EPD Length QuantityType.");
                        var evalByLength = EvaluateEnvironmentalProductDeclarationByLength(elementM, phases, mc, field, exactMatch);
                        value += evalByLength.Quantity;
                        if (resultValue == null)
                            resultValue = evalByLength;
                        break;
                    case QuantityType.Mass:
                        BH.Engine.Base.Compute.RecordNote("Evaluating object type: " + elementM.GetType() + " based on EPD Mass QuantityType.");
                        var evalByMass = EvaluateEnvironmentalProductDeclarationByMass(elementM, phases, mc, field, exactMatch);
                        value += evalByMass.Quantity;
                        if (resultValue == null)
                            resultValue = evalByMass;
                        break;
                    case QuantityType.Watt:
                        BH.Engine.Base.Compute.RecordError("Watt QuantityType is currently not supported.");
                        return null;
                    case QuantityType.VoltAmps:
                        BH.Engine.Base.Compute.RecordError("VoltAmps QuantityType is currently not supported.");
                        return null;
                    case QuantityType.Volume:
                        BH.Engine.Base.Compute.RecordNote("Evaluating object type: " + elementM.GetType() + " based on EPD Volume QuantityType.");
                        var evalByVolume = EvaluateEnvironmentalProductDeclarationByVolume(elementM, phases, mc, field, exactMatch);
                        value += evalByVolume.Quantity;
                        if (resultValue == null)
                            resultValue = evalByVolume;
                        break;
                    case QuantityType.VolumetricFlowRate:
                        BH.Engine.Base.Compute.RecordError("VolumetricFlowRate QuantityType is currently not supported.");
                        return null;
                    default:
                        BH.Engine.Base.Compute.RecordWarning("The object you have provided does not contain an EPD Material Fragment.");
                        return null;
                }
            }

            resultValue.Quantity = value;
            resultValue.EnvironmentalProductDeclaration = elementM.GetElementEpd(mc);
            return resultValue;
        }
        /***************************************************/


        [Description("This method calculates the results of any selected metric within an Environmental Product Declaration. For example for an EPD of QuantityType Volume, results will reflect the objects volume * EPD Field metric.")]
        [Input("elementM", "This is a BHoM object used to calculate EPD metric. This obj must have an EPD MaterialFragment applied to the object.")]
        [Input("phases", "Provide phases of life you wish to evaluate. Phases of life must be documented within EPDs for this method to work.")]
        [Input("field", "Filter the provided EnvironmentalProductDeclaration by selecting one of the provided metrics for calculation. This method also accepts multiple fields simultaneously.")]
        [Input("exactMatch", "If true, the evaluation method will force an exact LCA phase match to solve for.")]
        [Output("result", "A LifeCycleElementResult that contains the LifeCycleAssessment data for the input object.")]
        public static ElementResult EvaluateEnvironmentalProductDeclaration2(IElementM elementM, List<LifeCycleAssessmentPhases> phases, EnvironmentalProductDeclarationField field = EnvironmentalProductDeclarationField.GlobalWarmingPotential, bool exactMatch = false, List<Material> templateMaterials = null, bool prioritiseTemplate = true)
        {
            if (elementM == null)
            {

                return null;
            }

            MaterialComposition mc = elementM.MappedMaterialComposition(templateMaterials, true, prioritiseTemplate);

            if (mc == null)
            {
                return null;
            }

            if (mc.Materials.Count == 0)
                return null;

            //Predefining parameters to be used for some item types in loop
            double totalVolume = double.NaN;
            double area = double.NaN;
            double length = double.NaN;

            List<MaterialResult> materialResults = new List<MaterialResult>();

            for (int i = 0; i < mc.Materials.Count; i++)
            {
                Material material = mc.Materials[i];
                EnvironmentalProductDeclaration epd = material.Properties.OfType<EnvironmentalProductDeclaration>().FirstOrDefault();

                if (epd == null)
                {
                    Engine.Base.Compute.RecordError($"EPD not set to material {material.Name}. Unable to evaluate {field}.");
                    return null;
                }

                double evaluationValue = epd.GetEvaluationValue(field, phases, exactMatch);
                if (double.IsNaN(evaluationValue))
                {
                    Base.Compute.RecordError($"EPD {epd.Name} does not contain {field} for phases {string.Join(",", phases)}.");
                    return null;
                }

                double quantityValue;
                switch (epd.QuantityType)
                {
                    case QuantityType.Volume:
                        if (double.IsNaN(totalVolume))
                            totalVolume = elementM.ISolidVolume();
                        quantityValue = totalVolume * mc.Ratios[i];
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
                            area = element2D.Area();
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
                    case QuantityType.Mass:
                        if (double.IsNaN(totalVolume))
                            totalVolume = elementM.ISolidVolume();
                        double materialVolume = totalVolume * mc.Ratios[i];

                        if (double.IsNaN(material.Density))
                        {
                            Base.Compute.RecordError($"Density is not set for material {material.Name}. Cannot evaluate mass based EPD.");
                            return null;
                        }
                        if (material.Density == 0)
                        {
                            Base.Compute.RecordWarning($"Density of materials {material.Name} is 0 and will give no contribution for evaluating mass based EPD.");
                        }
                        quantityValue = materialVolume * material.Density;
                        break;
                    case QuantityType.Undefined:
                    case QuantityType.Item:
                    case QuantityType.Ampere:
                    case QuantityType.VoltAmps:
                    case QuantityType.VolumetricFlowRate:
                    case QuantityType.Watt:
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


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

using BH.Engine.Base;
using BH.Engine.Matter;
using BH.oM.Base;
using BH.oM.Base.Attributes;
using BH.oM.Dimensional;
using BH.oM.LifeCycleAssessment;
using BH.oM.LifeCycleAssessment.Configs;
using BH.oM.LifeCycleAssessment.Fragments;
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
        [Input("evaluationConfig", "Config controlling how the metrics should be evaluated, may contain additional parameters for the evaluation. If no config is provided the default evaluation mechanism is used which computes resulting phase values as metric value times applicable quantity.")]
        [Output("result", "A List of ElementResults, one per metric type, that contains the LifeCycleAssessment data for the input object(s).")]
        public static List<IElementResult<MaterialResult>> EnvironmentalResults(this IElementM elementM, List<Material> templateMaterials = null, bool prioritiseTemplate = true, List<EnvironmentalMetrics> metricFilter = null, IEvaluationConfig evaluationConfig = null)
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
                BH.Engine.Base.Compute.RecordWarning($"The {nameof(GeneralMaterialTakeoff)} from the provided {elementM.GetType().Name} does not contain any {nameof(Material)}s. Nothing to evaluate.");
                return new List<IElementResult<MaterialResult>>();
            }

            List<MaterialResult> materialResults = EnvironmentalResults(takeoff, templateMaterials, prioritiseTemplate, metricFilter, evaluationConfig);

            //Get out id as BHoM_Guid
            IComparable objectId = "";
            if (elementM is IBHoMObject bhObj)
                objectId = bhObj.BHoM_Guid;

            //Groups results by type and sums them up to single ElementResult per type
            return Create.IElementResults(objectId, elementM.ElementScope(), ObjectCategory.Undefined, materialResults);
        }

        /***************************************************/

        [PreviousVersion("6.2", "BH.Engine.LifeCycleAssessment.Compute.EvaluateMaterialTakeoff(BH.oM.Physical.Materials.VolumetricMaterialTakeoff, System.Collections.Generic.List<BH.oM.LifeCycleAssessment.LifeCycleAssessmentPhases>, BH.oM.LifeCycleAssessment.EnvironmentalProductDeclarationField, System.Boolean)")]
        [Description("Evaluates the materials in the VolumetricMaterialTakeoff and returns a MaterialResult per material in the takeoff. Requires the materials in the Takeoff to have EPDs assigned. Please use the AssignTemplate methods before calling this method.")]
        [Input("materialTakeoff", "The volumetric material takeoff to evaluate.")]
        [Input("templateMaterials", "Template materials to match to and assign properties from onto the model materials. Should generally have unique names. EPDs should be assigned to these materials and will be mapped over to the materials on the element with the same name and used in the evaluation.")]
        [Input("prioritiseTemplate", "Controls if main material or map material should be prioritised when conflicting information is found on both in terms of Density and/or Properties. If true, map is prioritised, if false, main material is prioritised.")]
        [Input("metricFilter", "Optional filter for the provided EnvironmentalProductDeclaration for selecting one or more of the provided metrics for calculation. This method also accepts multiple metric types simultaneously. If nothing is provided then no filtering is assumed, i.e. all metrics on the found EPDS are evaluated.")]
        [Input("evaluationConfig", "Config controlling how the metrics should be evaluated, may contain additional parameters for the evaluation. If no config is provided the default evaluation mechanism is used which computes resulting phase values as metric value times applicable quantity.")]
        [Output("result", "A MaterialResult per material and per metric that contains the LifeCycleAssessment data for the input takeoff.")]
        public static List<MaterialResult> EnvironmentalResults(this GeneralMaterialTakeoff materialTakeoff, List<Material> templateMaterials = null, bool prioritiseTemplate = true, List<EnvironmentalMetrics> metricFilter = null, IEvaluationConfig evaluationConfig = null)
        {
            if (materialTakeoff == null)
            {
                Base.Compute.RecordError($"Cannot evaluate a null {nameof(GeneralMaterialTakeoff)}.");
                return new List<MaterialResult>();
            }

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
                            else if (!double.IsNaN(vol))
                            {
                                double density = material.Density;
                                if (double.IsNaN(density) || density == 0)
                                {
                                    //Keeping support for backwards compability for old workflows relying epd density assigned as fragments to EPDS
                                    //Generally not recomended to work with this fragment
                                    EPDDensity epdDensity = epd.FindFragment<EPDDensity>();
                                    if (epdDensity != null)
                                        density = epdDensity.Density;
                                }
                                if (!double.IsNaN(density))
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

                materialResults.AddRange(EnvironmentalResults(epd, quantityValue, material.Name, metricFilter, evaluationConfig));
            }

            return materialResults;
        }

        /***************************************************/

        [PreviousVersion("6.2", "BH.Engine.LifeCycleAssessment.Compute.EvaluateReferenceValue(System.Double, BH.oM.LifeCycleAssessment.MaterialFragments.EnvironmentalProductDeclaration, BH.oM.LifeCycleAssessment.EnvironmentalProductDeclarationField, System.Collections.Generic.List<BH.oM.LifeCycleAssessment.LifeCycleAssessmentPhases>, System.Boolean)")]
        [Description("Evaluates all or selected metrics stored on the EnvironmentalProductDeclaration (EPD) and returns a result per metric.\n" +
                     "Each metric is evaluated by multiplying the values for each phase by the provided quantityValue.\n" +
                     "Please be mindful that the unit of the quantityValue should match the QuantityType on the EnvironmentalProductDeclaration.")]
        [Input("epd", "The EnvironmentalProductDeclaration to evaluate. Returned results will correspond to all, or selected, metrics stored on this object.")]
        [Input("quantityValue", "The quantity value to evaluate all metrics by. All metric properties will be multiplied by this value. Quantity should correspond to the QuantityType on the EPD.")]
        [Input("materialName", "The name of the Material that owns the EnvironmentalProductDeclaration. Stored as an identifier on the returned result classes.")]
        [Input("metricFilter", "Optional filter for the provided EnvironmentalProductDeclaration for selecting one or more of the provided metrics for calculation. This method also accepts multiple metric types simultaneously. If nothing is provided then no filtering is assumed, i.e. all metrics on the found EPDS are evaluated.")]
        [Input("evaluationConfig", "Config controlling how the metrics should be evaluated, may contain additional parameters for the evaluation. If no config is provided the default evaluation mechanism is used which computes resulting phase values as metric value times applicable quantity.")]
        [Output("results", "List of MaterialResults corresponding to the evaluated metrics on the EPD.")]
        [PreviousInputNames("quantityValue", "referenceValue")]
        public static List<MaterialResult> EnvironmentalResults(this EnvironmentalProductDeclaration epd, double quantityValue, string materialName = "", List<EnvironmentalMetrics> metricFilter = null, IEvaluationConfig evaluationConfig = null)
        {
            if (epd == null)
            {
                Base.Compute.RecordError($"Cannot evaluate a null {nameof(EnvironmentalProductDeclaration)}.");
                return null;
            }

            if (!IValidateConfig(evaluationConfig, epd))
                return new List<MaterialResult>();

            List<MaterialResult> results = new List<MaterialResult>();

            foreach (EnvironmentalMetric metric in epd.FilteredMetrics(metricFilter))
            {
                results.Add(EnvironmentalResults(metric, epd.Name, materialName, quantityValue, evaluationConfig));
            }

            return results;
        }

        /***************************************************/

        [Description("Evaluates the EnvironmentalMetric and returns a MaterialResult of a type corresponding to the metric. The evaluation is done by multiplying all phase data on the metric by the provided quantityValue.\n" +
                     "Please be mindful that the unit of the quantityValue should match the QuantityType on the EnvironmentalProductDeclaration to which the metric belongs.")]
        [Input("metric", "The EnvironmentalMetric to evaluate. Returned result will be a MaterialResult of a type corresponding to the metric.")]
        [Input("quantityValue", "The quantity value to evaluate all metrics by. All metric properties will be multiplied by this value. Quantity should correspond to the QuantityType on the EPD.")]
        [Input("epdName", "The name of the EnvironmentalProductDeclaration that owns the EnvironmentalMetric. Stored as an identifier on the returned result class.")]
        [Input("materialName", "The name of the Material that owns the EnvironmentalProductDeclaration. Stored as an identifier on the returned result class.")]
        [Input("evaluationConfig", "Config controlling how the metrics should be evaluated, may contain additional parameters for the evaluation. If no config is provided the default evaluation mechanism is used which computes resulting phase values as metric value times quantity.")]
        [Output("result", "A MaterialResult of a type corresponding to the evaluated metric with phase data calculated as data on metric multiplied by the provided quantity value.")]
        public static MaterialResult EnvironmentalResults(this EnvironmentalMetric metric, string epdName, string materialName, double quantityValue, IEvaluationConfig evaluationConfig = null)
        {
            if (metric == null)
            {
                Base.Compute.RecordError($"Cannot evaluate a null {nameof(EnvironmentalMetric)}.");
                return null;
            }

            if (metric.MetricType == EnvironmentalMetrics.EutrophicationCML ||
               metric.MetricType == EnvironmentalMetrics.EutrophicationTRACI ||
               metric.MetricType == EnvironmentalMetrics.PhotochemicalOzoneCreationCML ||
               metric.MetricType == EnvironmentalMetrics.PhotochemicalOzoneCreationTRACI)
            {
                Base.Compute.RecordWarning($"Please note that the metric of type {metric.MetricType} that is evaluated comes from an older standard and that the resulting values are incompatible in terms of quantity and unit to metrics from the EN 15804+A2 standard.\n" +
                                           $"Resulting values for the metrics can only be compared with other evaluated metrics from the exact same standard.");
            }

            return Create.MaterialResult(metric.GetType(), materialName, epdName, metric.IResultingPhaseValues(quantityValue, evaluationConfig));
        }

        /***************************************************/
        /**** Private Methods - Evaluation              ****/
        /***************************************************/

        [Description("Gets the resulting values for each phase of the provided EnvironmentalMetric given the provided quantityValue.\n" +
                     "The resulting values are computed based on provided config, defaulting to the values on the metric for each phase multiplied by the quantity value.\n" +
                     "Please be mindful that the unit of the quantityValue should match the QuantityType on the EnvironmentalProductDeclaration to which the metric belongs.")]
        [Input("metric", "The EnvironmentalMetric to get resulting values for. All phase values on the metric will be extracted and multiplied by the qunatityValue.")]
        [Input("quantityValue", "The quantity value to evaluate all metrics by. All metric properties will be multiplied by this value. Quantity should correspond to the QuantityType on the EPD.")]
        [Input("evaluationConfig", "Config controlling how the metrics should be evaluated, may contain additional parameters for the evaluation. If no config is provided the default evaluation mechanism is used which computes resulting phase values as metric value times applicable quantity.")]
        [Output("resultValues", "The resulting values for each phase.")]
        private static List<double> IResultingPhaseValues(this EnvironmentalMetric metric, double quantityValue, IEvaluationConfig evaluationConfig)
        {
            if (evaluationConfig == null)   //For case of null config, use default evaluation methodology of phase data value * quantity for each phase
                return ResultingPhaseValues(metric, quantityValue);
            else
                return ResultingPhaseValues(metric, quantityValue, evaluationConfig as dynamic);
        }

        /***************************************************/

        [Description("Default methodology for getting the resulting values for each phase of the provided EnvironmentalMetric given the provided quantityValue.\n" +
                     "The resulting values are computed as the values on the metric for each phase multiplied by the quantity value.\n" +
                     "Please be mindful that the unit of the quantityValue should match the QuantityType on the EnvironmentalProductDeclaration to which the metric belongs.")]
        [Input("metric", "The EnvironmentalMetric to get resulting values for. All phase values on the metric will be extracted and multiplied by the qunatityValue.")]
        [Input("quantityValue", "The quantity value to evaluate all metrics by. All metric properties will be multiplied by this value. Quantity should correspond to the QuantityType on the EPD.")]
        [Output("resultValues", "The resulting values for each phase.")]
        private static List<double> ResultingPhaseValues(this EnvironmentalMetric metric, double quantityValue)
        {
            if (metric == null)
            {
                Base.Compute.RecordError($"Cannot evaluate a null {nameof(EnvironmentalMetric)}.");
                return null;
            }

            List<double> resultingValues = new List<double>();
            foreach (double phaseData in metric.IPhaseDataValues())
            {
                resultingValues.Add(phaseData * quantityValue);  //Evaluation value is base phase data multiplied by quantity value
            }
            return resultingValues;
        }

        /***************************************************/

        [Description("IStructE Evaluation methodology for getting the resulting values for each phase of the provided EnvironmentalMetric given the provided quantityValue.\n" +
                     "Evaluation method only applicable for the two CLimateChangeTotal metric types - all other metrics are evaluated using the default mechanism.\n" +
                     "Method works for most phases works the same as default evaluation mechanism, with exception for the C1 and A5 phase where project totals are acounted for.")]
        [Input("metric", "The EnvironmentalMetric to get resulting values for. All phase values on the metric will be extracted and multiplied by the qunatityValue.")]
        [Input("quantityValue", "The quantity value to evaluate all metrics by. All metric properties will be multiplied by this value. Quantity should correspond to the QuantityType on the EPD.")]
        [Output("resultValues", "The resulting values for each phase.")]
        private static List<double> ResultingPhaseValues(this EnvironmentalMetric metric, double quantityValue, IStructEEvaluationConfig evaluationConfig)
        {
            if (metric == null)
            {
                Base.Compute.RecordError($"Cannot evaluate a null {nameof(EnvironmentalMetric)}.");
                return null;
            }

            //Specific evaluation method using the config only applicable for evaluating climate change totals
            if (metric.MetricType != EnvironmentalMetrics.ClimateChangeTotal && metric.MetricType != EnvironmentalMetrics.ClimateChangeTotalNoBiogenic)
            {
                Base.Compute.RecordNote($"The {nameof(IStructEEvaluationConfig)} evaluation is only applicable for evaluating metrics of type {EnvironmentalMetrics.ClimateChangeTotal} and {EnvironmentalMetrics.ClimateChangeTotalNoBiogenic}." +
                                  $"All other metrics are evaluated based on standard evaluation procedure of phase times quantity for all phases.");
                return ResultingPhaseValues(metric, quantityValue);
            }

            double weight = quantityValue;
            double weightFactor = (evaluationConfig.TotalWeight == 0 || evaluationConfig.TotalWeight < weight) ? 0 : weight / evaluationConfig.TotalWeight;

            double a1 = metric.A1 * quantityValue;
            double a2 = metric.A2 * quantityValue;
            double a3 = metric.A3 * quantityValue;
            double a1toa3 = metric.A1toA3 * quantityValue;
            double a4 = metric.A4 * quantityValue;
            double a5 = metric.A5 * quantityValue + evaluationConfig.ProjectCost * evaluationConfig.A5CarbonFactor * weightFactor;  //Additional project factor for A5

            double b1 = metric.B1 * quantityValue;
            double b2 = metric.B2 * quantityValue;
            double b3 = metric.B3 * quantityValue;
            double b4 = metric.B4 * quantityValue;
            double b5 = metric.B5 * quantityValue;
            double b6 = metric.B6 * quantityValue;
            double b7 = metric.B7 * quantityValue;
            double b1tob7 = metric.B1toB7 * quantityValue;

            double c1 = weightFactor * evaluationConfig.FloorArea * evaluationConfig.C1CarbonFactor;         //C1 evaluated based on project level values
            double c2 = metric.C2 * quantityValue;
            double c3 = metric.C3 * quantityValue;
            double c4 = metric.C4 * quantityValue;

            double c1toc4 = c1 + c2 + c3 + c4;  //C1toC4 taken as sum of phases
            if (double.IsNaN(c1toc4))   //If NaN, instead take based on total on metric times quantity value
                c1toc4 = metric.C1toC4 * quantityValue;

            double d = metric.D * quantityValue;

            return new List<double> { a1, a2, a3, a1toa3, a4, a5, b1, b2, b3, b4, b5, b6, b7, b1tob7, c1, c2, c3, c4, c1toc4, d };
        }

        /***************************************************/
        /**** Private Methods - Evaluation - Fallback   ****/
        /***************************************************/

        [Description("Fallback method for unkown config provided, raising warning and calling the defautl evaluation mechanism. Please note that this method is not triggered for null config, which also calls default mechism, but without warning.")]
        [Input("metric", "The EnvironmentalMetric to get resulting values for. All phase values on the metric will be extracted and multiplied by the qunatityValue.")]
        [Input("quantityValue", "The quantity value to evaluate all metrics by. All metric properties will be multiplied by this value. Quantity should correspond to the QuantityType on the EPD.")]
        [Output("resultValues", "The resulting values for each phase.")]
        private static List<double> ResultingPhaseValues(this EnvironmentalMetric metric, double quantityValue, IEvaluationConfig evaluationConfig)
        {
            BH.Engine.Base.Compute.RecordWarning($"No evaluation method implemented for evaluation config of type {evaluationConfig}. Results returned are based on default evaluation method of phase values times quantity.");

            return ResultingPhaseValues(metric, quantityValue);
        }

        /***************************************************/
        /**** Private Methods - Validation              ****/
        /***************************************************/

        private static bool IValidateConfig(IEvaluationConfig config, EnvironmentalProductDeclaration epd)
        {
            if (config == null) //Null config is valid, as default case of evaluation is assumed for provided null config.
                return true;

            return ValidateConfig(config, epd);
        }

        /***************************************************/

        private static bool ValidateConfig(IStructEEvaluationConfig config, EnvironmentalProductDeclaration epd)
        {
            bool valid = epd.QuantityType == QuantityType.Mass;

            if (!valid)
                BH.Engine.Base.Compute.RecordError($"{nameof(IStructEEvaluationConfig)} is only valid to be used with epds with quantity type mass.");
            return valid;
        }

        /***************************************************/
        /**** Private Methods - Validation - Fallback   ****/
        /***************************************************/

        private static bool ValidateConfig(IEvaluationConfig config, EnvironmentalProductDeclaration epd)
        {
            return true;    //Default to true for fallback
        }

        /***************************************************/
    }
}

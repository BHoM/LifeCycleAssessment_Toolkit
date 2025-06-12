/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2025, the respective contributors. All rights reserved.
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
using BH.oM.LifeCycleAssessment.MaterialFragments.Transport;
using BH.oM.LifeCycleAssessment.Results;
using BH.oM.Physical.Materials;
using System;
using System.Collections;
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

        [Description("Evaluates the EnvironmentalMetrics for the provided element and returns an ElementResult for each evaluated metric type.\n" +
                    "Evaluation is done by extracting the material takeoff for the provided element, giving quantities and Materiality.\n" +
                    "Each Material in the takeoff is then evaluated by finding the EnvironmentalProductDeclaration (EPD), either stored on the material or from the list of template materials.\n" +
                    "Each metric, or filtered chosen metrics, on the EPD is then evaluated.\n" +
                    "Finally, an element result is returned per metric type. Each element result being the sum result of all metrics of the same type.")]
        [Input("elementM", "The element to evaluate. The materiality and quantities is extracted from the element.")]
        [Input("templateMaterials", "Template materials to match to and assign properties from onto the model materials. Should generally have unique names. EPDs should be assigned to these materials and will be mapped over to the materials on the element with the same name and used in the evaluation.")]
        [Input("prioritiseTemplate", "Controls if main material or map material should be prioritised when conflicting information is found on both in terms of Density and/or Properties. If true, map is prioritised, if false, main material is prioritised.")]
        [Input("metricFilter", "Optional filter for the provided EnvironmentalProductDeclaration for selecting one or more of the provided metrics for calculation. This method also accepts multiple metric types simultaneously. If nothing is provided then no filtering is assumed, i.e. all metrics on the found EPDS are evaluated.")]
        [Input("evaluationConfig", "Config controlling how the metrics should be evaluated, may contain additional parameters for the evaluation. If no config is provided the default evaluation mechanism is used which computes resulting module values as metric value times applicable quantity.")]
        [Output("result", "A List of ElementResults, one per metric type, that contains the LifeCycleAssessment data for the input object(s).")]
        public static List<IElementResult<MaterialResult>> EnvironmentalResults(this IElementM elementM, List<Material> templateMaterials = null, bool prioritiseTemplate = true, List<MetricType> metricFilter = null, IEvaluationConfig evaluationConfig = null)
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

        [Description("Evaluates the materials in the VolumetricMaterialTakeoff and returns a MaterialResult per material in the takeoff. Requires the materials in the Takeoff to have EPDs assigned. Please use the AssignTemplate methods before calling this method.")]
        [Input("materialTakeoff", "The volumetric material takeoff to evaluate.")]
        [Input("templateMaterials", "Template materials to match to and assign properties from onto the model materials. Should generally have unique names. EPDs should be assigned to these materials and will be mapped over to the materials on the element with the same name and used in the evaluation.")]
        [Input("prioritiseTemplate", "Controls if main material or map material should be prioritised when conflicting information is found on both in terms of Density and/or Properties. If true, map is prioritised, if false, main material is prioritised.")]
        [Input("metricFilter", "Optional filter for the provided EnvironmentalProductDeclaration for selecting one or more of the provided metrics for calculation. This method also accepts multiple metric types simultaneously. If nothing is provided then no filtering is assumed, i.e. all metrics on the found EPDS are evaluated.")]
        [Input("evaluationConfig", "Config controlling how the metrics should be evaluated, may contain additional parameters for the evaluation. If no config is provided the default evaluation mechanism is used which computes resulting module values as metric value times applicable quantity.")]
        [Output("result", "A MaterialResult per material and per metric that contains the LifeCycleAssessment data for the input takeoff.")]
        public static List<MaterialResult> EnvironmentalResults(this GeneralMaterialTakeoff materialTakeoff, List<Material> templateMaterials = null, bool prioritiseTemplate = true, List<MetricType> metricFilter = null, IEvaluationConfig evaluationConfig = null)
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
                materialResults.AddRange(EnvironmentalResults(mappedTakeoff.MaterialTakeoffItems[i], metricFilter, evaluationConfig));
            }

            return materialResults;
        }

        /***************************************************/



        [Description("Evaluates the materials in the VolumetricMaterialTakeoff and returns a MaterialResult per material in the takeoff. Requires the materials in the Takeoff to have EPDs assigned. Please use the AssignTemplate methods before calling this method.")]
        [Input("takeoffItem", "The material takeoff item to evaluate. Material in takeoff items is assumed to contain IEnvironmentalfactorsProvider.")]
        [Input("metricFilter", "Optional filter for the provided EnvironmentalProductDeclaration for selecting one or more of the provided metrics for calculation. This method also accepts multiple metric types simultaneously. If nothing is provided then no filtering is assumed, i.e. all metrics on the found EPDS are evaluated.")]
        [Input("evaluationConfig", "Config controlling how the metrics should be evaluated, may contain additional parameters for the evaluation. If no config is provided the default evaluation mechanism is used which computes resulting module values as metric value times applicable quantity.")]
        [Output("result", "A MaterialResult per material and per metric that contains the LifeCycleAssessment data for the input takeoff.")]
        public static List<MaterialResult> EnvironmentalResults(this TakeoffItem takeoffItem, List<MetricType> metricFilter = null, IEvaluationConfig evaluationConfig = null)
        {
            if (takeoffItem == null || takeoffItem.Material == null)
            {
                BH.Engine.Base.Compute.RecordError($"Cannot evaluate a null {nameof(TakeoffItem)} or a {nameof(TakeoffItem)} with a null {nameof(TakeoffItem.Material)}.");
                return new List<MaterialResult>();
            }

            List<MaterialResult> materialResults = new List<MaterialResult>();

            Material material = takeoffItem.Material;
            List<IEnvironmentalFactorsProvider> metricProviders = material.Properties.OfType<IEnvironmentalFactorsProvider>().ToList();

            if (metricProviders.Count == 0)
            {
                Base.Compute.RecordError($"No {nameof(EnvironmentalProductDeclaration)}, or {nameof(CombinedLifeCycleAssessmentFactors)} set to material {material.Name}. Unable to evaluate element.");
                return new List<MaterialResult>();
            }

            IEnvironmentalFactorsProvider metricProvider;
            if (metricProviders.Count == 1)
            {
                metricProvider = metricProviders[0];
            }
            else
            {
                //If more than one metric provider is present, check the one with highest priority. Pririty list in order:
                List<Type> priorityOrder = new List<Type>()
                {
                    typeof(CombinedLifeCycleAssessmentFactors),
                    typeof(EnvironmentalProductDeclaration),
                };

                metricProvider = null;
                foreach (Type type in priorityOrder)
                {
                    metricProvider = metricProviders.FirstOrDefault(x => x.GetType() == type);
                    if (metricProvider != null)
                        break;
                }

                if (metricProvider == null)
                    metricProvider = metricProviders.First();

                BH.Engine.Base.Compute.RecordNote($"More than one EnvironmentalfactorsProvider found on material named {material.Name}. Metric provider of type {metricProvider.GetType().Name} with name {metricProvider.Name} used for evaluation.");
            }

            return IEnvironmentalResults(metricProvider, takeoffItem, metricFilter, evaluationConfig);
        }

        /***************************************************/

        [PreviousVersion("8.2", "BH.Engine.LifeCycleAssessment.Query.EnvironmentalResults(BH.oM.LifeCycleAssessment.MaterialFragments.EnvironmentalProductDeclaration, System.Double, System.String, System.Collections.Generic.List<BH.oM.LifeCycleAssessment.EnvironmentalMetrics>, BH.oM.LifeCycleAssessment.Configs.IEvaluationConfig)")]
        [Description("Evaluates all or selected metrics stored on the EnvironmentalProductDeclaration (EPD) and returns a result per metric.\n" +
                     "Each metric is evaluated by multiplying the values for each module by the provided quantityValue.\n" +
                     "Please be mindful that the unit of the quantityValue should match the QuantityType on the EnvironmentalProductDeclaration.")]
        [Input("epd", "The EnvironmentalProductDeclaration to evaluate. Returned results will correspond to all, or selected, metrics stored on this object.")]
        [Input("quantityValue", "The quantity value to evaluate all metrics by. All metric properties will be multiplied by this value. Quantity should correspond to the QuantityType on the EPD.")]
        [Input("materialName", "The name of the Material that owns the EnvironmentalProductDeclaration. Stored as an identifier on the returned result classes.")]
        [Input("metricFilter", "Optional filter for the provided EnvironmentalProductDeclaration for selecting one or more of the provided metrics for calculation. This method also accepts multiple metric types simultaneously. If nothing is provided then no filtering is assumed, i.e. all metrics on the found EPDS are evaluated.")]
        [Input("evaluationConfig", "Config controlling how the metrics should be evaluated, may contain additional parameters for the evaluation. If no config is provided the default evaluation mechanism is used which computes resulting module values as metric value times applicable quantity.")]
        [Input("configData", "Additional data required for evaluation with the provided config. If no config is provided, this input can be left empty. Type of data expected depends on the config. For the IStructEEvaluationConfig the mass should be provided here.")]
        [Output("results", "List of MaterialResults corresponding to the evaluated metrics on the EPD.")]
        [PreviousInputNames("quantityValue", "referenceValue")]
        public static List<MaterialResult> EnvironmentalResults(this EnvironmentalProductDeclaration epd, double quantityValue, string materialName = "", List<MetricType> metricFilter = null, IEvaluationConfig evaluationConfig = null, object configData = null)
        {
            if (epd == null)
            {
                Base.Compute.RecordError($"Cannot evaluate a null {nameof(EnvironmentalProductDeclaration)}.");
                return null;
            }

            List<MaterialResult> results = new List<MaterialResult>();

            foreach (IEnvironmentalMetric metric in epd.FilteredFactors(metricFilter))
            {
                results.Add(EnvironmentalResults(metric, epd.Name, materialName, quantityValue, evaluationConfig, configData));
            }

            return results;
        }

        /***************************************************/

        [Description("Evaluates all or selected metrics stored on the EnvironmentalProductDeclaration (EPD) and returns a result per metric.\n" +
              "Each metric is evaluated by multiplying the values for each module by the provided quantityValue.\n" +
              "Please be mindful that the unit of the quantityValue should match the QuantityType on the EnvironmentalProductDeclaration.")]
        [Input("factorsProvider", "The CombinedLifeCycleAssessmentFactors to evaluate. Returned results will correspond to all, or selected, metrics stored on this object.")]
        [Input("quantityValue", "The quantity value to evaluate all metrics by. All metric properties will be multiplied by this value. Quantity should correspond to the QuantityType on the EPD.")]
        [Input("mass", "The mass value to transport scenarios by. Can for the case of quantity type of the APD being mass, this should be equal to the quantityValue.")]
        [Input("materialName", "The name of the Material that owns the EnvironmentalProductDeclaration. Stored as an identifier on the returned result classes.")]
        [Input("metricFilter", "Optional filter for the provided EnvironmentalProductDeclaration for selecting one or more of the provided metrics for calculation. This method also accepts multiple metric types simultaneously. If nothing is provided then no filtering is assumed, i.e. all metrics on the found EPDS are evaluated.")]
        [Input("evaluationConfig", "Config controlling how the metrics should be evaluated, may contain additional parameters for the evaluation. If no config is provided the default evaluation mechanism is used which computes resulting module values as metric value times applicable quantity.")]
        [Input("configData", "Additional data required for evaluation with the provided config. If no config is provided, this input can be left empty. Type of data expected depends on the config. For the IStructEEvaluationConfig the mass should be provided here.")]
        [Output("results", "List of MaterialResults corresponding to the evaluated metrics on the EPD.")]
        [PreviousInputNames("quantityValue", "referenceValue")]
        public static List<MaterialResult> EnvironmentalResults(this CombinedLifeCycleAssessmentFactors factorsProvider, double quantityValue, double mass, string materialName = "", List<MetricType> metricFilter = null, IEvaluationConfig evaluationConfig = null, object configData = null)
        {
            if (factorsProvider == null)
            {
                Base.Compute.RecordError($"Cannot evaluate a null {nameof(CombinedLifeCycleAssessmentFactors)}.");
                return null;
            }

            Dictionary<MetricType, double> a4TransportResults = factorsProvider.A4TransportFactors?.ITransportResults(mass, metricFilter) ?? new Dictionary<MetricType, double>();
            Dictionary<MetricType, double> c2TransportResults = factorsProvider.C2TransportFactors?.ITransportResults(mass, metricFilter) ?? new Dictionary<MetricType, double>();

            List<MaterialResult> results = new List<MaterialResult>();

            if (factorsProvider.EnvironmentalProductDeclaration != null)
            {
                foreach (IEnvironmentalMetric metric in factorsProvider.EnvironmentalProductDeclaration.FilteredFactors(metricFilter))
                {
                    MetricType type = metric.IMetricType();
                    Dictionary<Module, double> resultingValues = metric.IResultingModuleValues(quantityValue, evaluationConfig, configData);

                    //Check if C2 and A4 results are defined explicitly, and override them if they are
                    if (a4TransportResults.TryGetValue(type, out double a4))
                    {
                        resultingValues[Module.A4] = a4;
                        a4TransportResults.Remove(type);    //Remove as used up
                    }

                    if (c2TransportResults.TryGetValue(type, out double c2))
                    {
                        resultingValues[Module.C2] = c2;
                        c2TransportResults.Remove(type);    //Remove as used up

                        if (resultingValues.ContainsKey(Module.C1toC4))
                            resultingValues.Remove(Module.C1toC4);  //Remove C1toC4 as no longer ensured valid
                    }

                    results.Add(Create.MaterialResult(materialName, factorsProvider.Name, type, resultingValues));
                }
            }

            if (a4TransportResults.Count != 0 || c2TransportResults.Count != 0)
            {
                Dictionary<MetricType, Dictionary<Module, double>> fullTransportFactors = new Dictionary<MetricType, Dictionary<Module, double>>();

                //If any transport factors not already part of results in base factors, add them as single item results
                foreach (var res in a4TransportResults)
                {
                    fullTransportFactors[res.Key] = new Dictionary<Module, double> { {  Module.A4, res.Value } };
                }


                //If still C2 resutls left over, create as new results
                foreach (var res in c2TransportResults)
                {
                    if(fullTransportFactors.TryGetValue(res.Key, out var fullResults))
                        fullResults[Module.C2] = res.Value;
                    else
                        fullTransportFactors[res.Key] = new Dictionary<Module, double> { { Module.C2, res.Value } };
                }

                foreach (var transRes in fullTransportFactors)
                {
                    results.Add(Create.MaterialResult(materialName, factorsProvider.Name, transRes.Key, transRes.Value));
                }
            }


            return results;
        }

        /***************************************************/

        [PreviousVersion("8.2", "BH.Engine.LifeCycleAssessment.Query.EnvironmentalResults(BH.oM.LifeCycleAssessment.MaterialFragments.EnvironmentalMetric, System.String, System.String, System.Double, BH.oM.LifeCycleAssessment.Configs.IEvaluationConfig)")]
        [Description("Evaluates the EnvironmentalMetric and returns a MaterialResult of a type corresponding to the metric. The evaluation is done by multiplying all module data on the metric by the provided quantityValue.\n" +
                     "Please be mindful that the unit of the quantityValue should match the QuantityType on the EnvironmentalProductDeclaration to which the metric belongs.")]
        [Input("metric", "The EnvironmentalMetric to evaluate. Returned result will be a MaterialResult of a type corresponding to the metric.")]
        [Input("epdName", "The name of the EnvironmentalProductDeclaration that owns the EnvironmentalMetric. Stored as an identifier on the returned result class.")]
        [Input("materialName", "The name of the Material that owns the EnvironmentalProductDeclaration. Stored as an identifier on the returned result class.")]
        [Input("quantityValue", "The quantity value to evaluate all metrics by. All metric properties will be multiplied by this value. Quantity should correspond to the QuantityType on the EPD.")]
        [Input("evaluationConfig", "Config controlling how the metrics should be evaluated, may contain additional parameters for the evaluation. If no config is provided the default evaluation mechanism is used which computes resulting module values as metric value times quantity.")]
        [Input("configData", "Additional data required for evaluation with the provided config. If no config is provided, this input can be left empty. Type of data expected depends on the config. For the IStructEEvaluationConfig the mass should be provided here.")]
        [Output("result", "A MaterialResult of a type corresponding to the evaluated metric with module data calculated as data on metric multiplied by the provided quantity value.")]
        public static MaterialResult EnvironmentalResults(this IEnvironmentalMetric metric, string epdName, string materialName, double quantityValue, IEvaluationConfig evaluationConfig = null, object configData = null)
        {
            if (metric == null)
            {
                Base.Compute.RecordError($"Cannot evaluate a null {nameof(IEnvironmentalMetric)}.");
                return null;
            }

            IDictionary resultingValues = metric.IResultingModuleValues(quantityValue, evaluationConfig, configData);

            return Create.MaterialResult(materialName, epdName, metric.IMetricType(), resultingValues as dynamic);
        }

        /***************************************************/
        /**** Private Methods - Metric providers        ****/
        /***************************************************/

        private static List<MaterialResult> IEnvironmentalResults(this IEnvironmentalFactorsProvider factorsProvider, TakeoffItem takeoffItem, List<MetricType> metricFilter, IEvaluationConfig evaluationConfig)
        {
            if (ITryGetConfigData(evaluationConfig, takeoffItem, out object configData))
            {
                return EnvironmentalResults(factorsProvider as dynamic, takeoffItem, metricFilter, evaluationConfig, configData);
            }
            return new List<MaterialResult>();
        }

        /***************************************************/

        private static List<MaterialResult> EnvironmentalResults(this EnvironmentalProductDeclaration factorsProvider, TakeoffItem takeoffItem, List<MetricType> metricFilter, IEvaluationConfig evaluationConfig, object configData)
        {
            return EnvironmentalResults(factorsProvider, takeoffItem.QuantityValue(factorsProvider.QuantityType), takeoffItem.Material.Name, metricFilter, evaluationConfig, configData);
        }

        /***************************************************/

        private static List<MaterialResult> EnvironmentalResults(this CombinedLifeCycleAssessmentFactors factorsProvider, TakeoffItem takeoffItem, List<MetricType> metricFilter, IEvaluationConfig evaluationConfig, object configData)
        {
            double quantityValue = takeoffItem.QuantityValue(factorsProvider.EnvironmentalProductDeclaration.QuantityType);
            double mass = takeoffItem.QuantityValue(QuantityType.Mass);
            return EnvironmentalResults(factorsProvider, quantityValue, mass, takeoffItem.Material.Name, metricFilter, evaluationConfig, configData);
        }

        /***************************************************/


        private static List<MaterialResult> EnvironmentalResults(this IEnvironmentalFactorsProvider factorsProvider, TakeoffItem takeoffItem, List<MetricType> metricFilter, IEvaluationConfig evaluationConfig, object configData)
        {
            BH.Engine.Base.Compute.RecordWarning($"No evaluation method implemented for metric providers of type {factorsProvider.GetType().Name}");
            return new List<MaterialResult>();
        }

        /***************************************************/
        /**** Private Methods - Extract config data     ****/
        /***************************************************/

        private static bool ITryGetConfigData(this IEvaluationConfig evaluationConfig, TakeoffItem takeoffItem, out object configData)
        {
            if (evaluationConfig == null)
            { 
                configData = null;
                return true;
            }

            return TryGetConfigData(evaluationConfig as dynamic, takeoffItem, out configData);
        }

        /***************************************************/

        private static bool TryGetConfigData(this IStructEEvaluationConfig evaluationConfig, TakeoffItem takeoffItem, out object configData)
        {
            double mass = takeoffItem.QuantityValue(QuantityType.Mass);

            configData = mass;
            if (double.IsNaN(mass) || mass == 0)
            {
                BH.Engine.Base.Compute.RecordError($"Unable to extract required mass required to evaluate with the {nameof(IStructEEvaluationConfig)}.");
                return false;
            }

            return true;
        }

        /***************************************************/

        private static bool TryGetConfigData(this IEvaluationConfig evaluationConfig, TakeoffItem takeoffItem, out object configData)
        {
            //Fallback method
            BH.Engine.Base.Compute.RecordWarning($"No method exists to extract config data for config of type {evaluationConfig.GetType().Name}.");
            configData = null;
            return true;    //Return true to continue evaluation
        }

        /***************************************************/
    }
}



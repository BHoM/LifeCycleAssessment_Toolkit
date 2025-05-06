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
using BH.oM.LifeCycleAssessment.Results.MetricsValues;
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
        [Input("evaluationConfig", "Config controlling how the metrics should be evaluated, may contain additional parameters for the evaluation. If no config is provided the default evaluation mechanism is used which computes resulting phase values as metric value times applicable quantity.")]
        [Output("result", "A List of ElementResults, one per metric type, that contains the LifeCycleAssessment data for the input object(s).")]
        public static List<IElementResult2> EnvironmentalResults(this IElementM elementM, List<Material> templateMaterials = null, bool prioritiseTemplate = true, List<MetricType> metricFilter = null, IEvaluationConfig evaluationConfig = null)
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
                return new List<IElementResult2>();
            }

            List<IMaterialResult> materialResults = EnvironmentalResults(takeoff, templateMaterials, prioritiseTemplate, metricFilter, evaluationConfig);

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
        [Input("evaluationConfig", "Config controlling how the metrics should be evaluated, may contain additional parameters for the evaluation. If no config is provided the default evaluation mechanism is used which computes resulting phase values as metric value times applicable quantity.")]
        [Output("result", "A MaterialResult per material and per metric that contains the LifeCycleAssessment data for the input takeoff.")]
        public static List<IMaterialResult> EnvironmentalResults(this GeneralMaterialTakeoff materialTakeoff, List<Material> templateMaterials = null, bool prioritiseTemplate = true, List<MetricType> metricFilter = null, IEvaluationConfig evaluationConfig = null)
        {
            if (materialTakeoff == null)
            {
                Base.Compute.RecordError($"Cannot evaluate a null {nameof(GeneralMaterialTakeoff)}.");
                return new List<IMaterialResult>();
            }

            GeneralMaterialTakeoff mappedTakeoff;
            if (templateMaterials == null || templateMaterials.Count == 0)
                mappedTakeoff = materialTakeoff;
            else
                mappedTakeoff = Matter.Modify.AssignTemplate(materialTakeoff, templateMaterials, prioritiseTemplate);

            List<IMaterialResult> materialResults = new List<IMaterialResult>();

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
        [Input("evaluationConfig", "Config controlling how the metrics should be evaluated, may contain additional parameters for the evaluation. If no config is provided the default evaluation mechanism is used which computes resulting phase values as metric value times applicable quantity.")]
        [Output("result", "A MaterialResult per material and per metric that contains the LifeCycleAssessment data for the input takeoff.")]
        public static List<IMaterialResult> EnvironmentalResults(this TakeoffItem takeoffItem, List<MetricType> metricFilter = null, IEvaluationConfig evaluationConfig = null)
        {
            if (takeoffItem == null || takeoffItem.Material == null)
            {
                BH.Engine.Base.Compute.RecordError($"Cannot evaluate a null {nameof(TakeoffItem)} or a {nameof(TakeoffItem)} with a null {nameof(TakeoffItem.Material)}.");
                return new List<IMaterialResult>();
            }

            List<IMaterialResult> materialResults = new List<IMaterialResult>();

            Material material = takeoffItem.Material;
            List<IEnvironmentalFactorsProvider> metricProviders = material.Properties.OfType<IEnvironmentalFactorsProvider>().ToList();

            if (metricProviders.Count == 0)
            {
                Base.Compute.RecordError($"No {nameof(EnvironmentalProductDeclaration)}, {nameof(CalculatedMaterialLifeCycleEnvironmentalImpactFactors)} or {nameof(CombinedLifeCycleAssessmentFactors)} set to material {material.Name}. Unable to evaluate element.");
                return new List<IMaterialResult>();
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
                    typeof(CalculatedMaterialLifeCycleEnvironmentalImpactFactors),
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

        [PreviousInputNames("factorsProvider", "epd")]
        [PreviousVersion("8.1", "BH.Engine.LifeCycleAssessment.Query.EnvironmentalResults(BH.oM.LifeCycleAssessment.MaterialFragments.EnvironmentalProductDeclaration, System.Double, System.String, System.Collections.Generic.List<BH.oM.LifeCycleAssessment.EnvironmentalMetrics>, BH.oM.LifeCycleAssessment.Configs.IEvaluationConfig)")]
        [Description("Evaluates all or selected metrics stored on the EnvironmentalProductDeclaration (EPD) and returns a result per metric.\n" +
                     "Each metric is evaluated by multiplying the values for each phase by the provided quantityValue.\n" +
                     "Please be mindful that the unit of the quantityValue should match the QuantityType on the EnvironmentalProductDeclaration.")]
        [Input("factorsProvider", "The EnvironmentalProductDeclaration or CalculatedMaterialLifeCycleEnvironmentalImpactFactors to evaluate. Returned results will correspond to all, or selected, metrics stored on this object.")]
        [Input("quantityValue", "The quantity value to evaluate all metrics by. All metric properties will be multiplied by this value. Quantity should correspond to the QuantityType on the EPD.")]
        [Input("materialName", "The name of the Material that owns the EnvironmentalProductDeclaration. Stored as an identifier on the returned result classes.")]
        [Input("metricFilter", "Optional filter for the provided EnvironmentalProductDeclaration for selecting one or more of the provided metrics for calculation. This method also accepts multiple metric types simultaneously. If nothing is provided then no filtering is assumed, i.e. all metrics on the found EPDS are evaluated.")]
        [Input("evaluationConfig", "Config controlling how the metrics should be evaluated, may contain additional parameters for the evaluation. If no config is provided the default evaluation mechanism is used which computes resulting phase values as metric value times applicable quantity.")]
        [Output("results", "List of MaterialResults corresponding to the evaluated metrics on the EPD.")]
        [PreviousInputNames("quantityValue", "referenceValue")]
        public static List<IMaterialResult> EnvironmentalResults(this IBaseLevelEnvironalmentalFactorsProvider factorsProvider, double quantityValue, string materialName = "", List<MetricType> metricFilter = null, IEvaluationConfig evaluationConfig = null)
        {
            if (factorsProvider == null)
            {
                Base.Compute.RecordError($"Cannot evaluate a null {nameof(IBaseLevelEnvironalmentalFactorsProvider)}.");
                return null;
            }

            if (!IValidateConfig(evaluationConfig, factorsProvider))
                return new List<IMaterialResult>();

            List<IMaterialResult> results = new List<IMaterialResult>();

            foreach (IEnvironmentalMetricFactors metric in factorsProvider.FilteredFactors(metricFilter))
            {
                results.Add(EnvironmentalResults(metric, factorsProvider.Name, materialName, quantityValue, evaluationConfig));
            }

            return results;
        }

        /***************************************************/

        [Description("Evaluates all or selected metrics stored on the EnvironmentalProductDeclaration (EPD) and returns a result per metric.\n" +
              "Each metric is evaluated by multiplying the values for each phase by the provided quantityValue.\n" +
              "Please be mindful that the unit of the quantityValue should match the QuantityType on the EnvironmentalProductDeclaration.")]
        [Input("factorsProvider", "The CombinedLifeCycleAssessmentFactors to evaluate. Returned results will correspond to all, or selected, metrics stored on this object.")]
        [Input("quantityValue", "The quantity value to evaluate all metrics by. All metric properties will be multiplied by this value. Quantity should correspond to the QuantityType on the EPD.")]
        [Input("mass", "The mass value to transport scenarios by. Can for the case of quantity type of the APD being mass, this should be equal to the quantityValue.")]
        [Input("materialName", "The name of the Material that owns the EnvironmentalProductDeclaration. Stored as an identifier on the returned result classes.")]
        [Input("metricFilter", "Optional filter for the provided EnvironmentalProductDeclaration for selecting one or more of the provided metrics for calculation. This method also accepts multiple metric types simultaneously. If nothing is provided then no filtering is assumed, i.e. all metrics on the found EPDS are evaluated.")]
        [Input("evaluationConfig", "Config controlling how the metrics should be evaluated, may contain additional parameters for the evaluation. If no config is provided the default evaluation mechanism is used which computes resulting phase values as metric value times applicable quantity.")]
        [Output("results", "List of MaterialResults corresponding to the evaluated metrics on the EPD.")]
        [PreviousInputNames("quantityValue", "referenceValue")]
        public static List<IMaterialResult> EnvironmentalResults(this CombinedLifeCycleAssessmentFactors factorsProvider, double quantityValue, double mass, string materialName = "", List<MetricType> metricFilter = null, IEvaluationConfig evaluationConfig = null)
        {
            if (factorsProvider == null)
            {
                Base.Compute.RecordError($"Cannot evaluate a null {nameof(CombinedLifeCycleAssessmentFactors)}.");
                return null;
            }

            List<IMetricValue> a4TransportResults = factorsProvider.A4TransportFactors?.ITransportResults(mass, metricFilter) ?? new List<IMetricValue>();
            List<IMetricValue> c2TransportResults = factorsProvider.C2TransportFactors?.ITransportResults(mass, metricFilter) ?? new List<IMetricValue>();

            List<IMaterialResult> results = new List<IMaterialResult>();

            if (factorsProvider.BaseFactors != null)
            {
                if (!IValidateConfig(evaluationConfig, factorsProvider.BaseFactors))
                    return new List<IMaterialResult>();

                foreach (IEnvironmentalMetricFactors metric in factorsProvider.BaseFactors.FilteredFactors(metricFilter))
                {
                    IDictionary resultingValues = metric.IResultingModuleValues(quantityValue, evaluationConfig);
                    if (a4TransportResults.Count > 0)
                        AddModule(resultingValues as dynamic, a4TransportResults, Module.A4);

                    if (c2TransportResults.Count > 0)
                        AddModule(resultingValues as dynamic, c2TransportResults, Module.C2);

                    results.Add(Create.IMaterialResult(materialName, factorsProvider.Name, metric.MetricType, resultingValues));
                }
            }

            if (a4TransportResults.Count != 0 || c2TransportResults.Count != 0)
            {
                List<IDictionary> transportDicts = new List<IDictionary>();

                //If any transport factors not already part of results in base factors, add them as single item results
                foreach (IMetricValue res in a4TransportResults)
                {
                    transportDicts.Add(SingleModuleDictionary(res as dynamic, Module.A4));
                }

                //Try adding C2 results to the A4 results
                foreach (IDictionary transRes in transportDicts)
                {
                    AddModule(transRes as dynamic, c2TransportResults, Module.C2);
                }

                //If still C2 resutls left over, create as new results
                foreach (IMetricValue res in c2TransportResults)
                {
                    transportDicts.Add(SingleModuleDictionary(res as dynamic, Module.A4));
                }

                foreach (IDictionary transRes in transportDicts)
                {
                    results.Add(Create.IMaterialResult(materialName, factorsProvider.Name, (MetricType)(-1), transRes));
                }
            }


            return results;
        }

        /***************************************************/

        [Description("Evaluates the EnvironmentalMetric and returns a MaterialResult of a type corresponding to the metric. The evaluation is done by multiplying all phase data on the metric by the provided quantityValue.\n" +
                     "Please be mindful that the unit of the quantityValue should match the QuantityType on the EnvironmentalProductDeclaration to which the metric belongs.")]
        [Input("metric", "The EnvironmentalMetric to evaluate. Returned result will be a MaterialResult of a type corresponding to the metric.")]
        [Input("epdName", "The name of the IEnvironmentalfactorsProvider (EnvironmentalProductDeclaration or CalculatedMaterialLifeCycleEnvironmentalImpactFactors) that owns the EnvironmentalMetric. Stored as an identifier on the returned result class.")]
        [Input("materialName", "The name of the Material that owns the EnvironmentalProductDeclaration. Stored as an identifier on the returned result class.")]
        [Input("quantityValue", "The quantity value to evaluate all metrics by. All metric properties will be multiplied by this value. Quantity should correspond to the QuantityType on the EPD.")]
        [Input("evaluationConfig", "Config controlling how the metrics should be evaluated, may contain additional parameters for the evaluation. If no config is provided the default evaluation mechanism is used which computes resulting phase values as metric value times quantity.")]
        [Output("result", "A MaterialResult of a type corresponding to the evaluated metric with phase data calculated as data on metric multiplied by the provided quantity value.")]
        public static IMaterialResult EnvironmentalResults(this IEnvironmentalMetricFactors metric, string epdName, string materialName, double quantityValue, IEvaluationConfig evaluationConfig = null)
        {
            if (metric == null)
            {
                Base.Compute.RecordError($"Cannot evaluate a null {nameof(IEnvironmentalMetricFactors)}.");
                return null;
            }

            IDictionary resultingValues = metric.IResultingModuleValues(quantityValue, evaluationConfig);

            return Create.IMaterialResult(materialName, epdName, metric.MetricType, resultingValues as dynamic);
        }

        /***************************************************/
        /**** Private Methods - Metric providers        ****/
        /***************************************************/

        private static List<IMaterialResult> IEnvironmentalResults(this IEnvironmentalFactorsProvider factorsProvider, TakeoffItem takeoffItem, List<MetricType> metricFilter, IEvaluationConfig evaluationConfig)
        {
            return EnvironmentalResults(factorsProvider as dynamic, takeoffItem, metricFilter, evaluationConfig);
        }

        /***************************************************/

        private static List<IMaterialResult> EnvironmentalResults(this IBaseLevelEnvironalmentalFactorsProvider factorsProvider, TakeoffItem takeoffItem, List<MetricType> metricFilter, IEvaluationConfig evaluationConfig)
        {
            return EnvironmentalResults(factorsProvider, takeoffItem.QuantityValue(factorsProvider.QuantityType), takeoffItem.Material.Name, metricFilter, evaluationConfig);
        }

        /***************************************************/

        private static List<IMaterialResult> EnvironmentalResults(this CombinedLifeCycleAssessmentFactors factorsProvider, TakeoffItem takeoffItem, List<MetricType> metricFilter, IEvaluationConfig evaluationConfig)
        {
            double quantityValue = takeoffItem.QuantityValue(factorsProvider.BaseFactors.QuantityType);
            double mass = takeoffItem.QuantityValue(QuantityType.Mass);
            return EnvironmentalResults(factorsProvider, quantityValue, mass, takeoffItem.Material.Name, metricFilter, evaluationConfig);
        }

        /***************************************************/


        private static List<IMaterialResult> EnvironmentalResults(this IEnvironmentalFactorsProvider factorsProvider, TakeoffItem takeoffItem, List<MetricType> metricFilter, IEvaluationConfig evaluationConfig)
        {
            BH.Engine.Base.Compute.RecordWarning($"No evaluation method implemented for metric providers of type {factorsProvider.GetType().Name}");
            return new List<IMaterialResult>();
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static Dictionary<Module, T> AddModule<T>(this Dictionary<Module, T> resultingValues, List<IMetricValue> toAddFrom, Module module)
        {
            T toBeAdded = toAddFrom.OfType<T>().FirstOrDefault();
            if (toBeAdded != null)
            {
                resultingValues[module] = toBeAdded;
                toAddFrom.Remove(toBeAdded as IMetricValue);
            }

            return resultingValues;
        }

        /***************************************************/

        private static Dictionary<Module, T> SingleModuleDictionary<T>(this T metricValue, Module module)
        {
            Dictionary<Module, T> dict = new Dictionary<Module, T>();
            dict[module] = metricValue;
            return dict;
        }

        /***************************************************/

    }
}



/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2026, the respective contributors. All rights reserved.
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
using BH.oM.LifeCycleAssessment;
using BH.oM.LifeCycleAssessment.Configs;
using BH.oM.LifeCycleAssessment.Interfaces;
using BH.oM.LifeCycleAssessment.MaterialFragments;
using BH.oM.LifeCycleAssessment.Results;
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

        [Description("Provides pre-computed module values for the provided evaluation config. This is used to provide additional module values for modules that are evaluated based on project level values, such as A5_2 and C1.")]
        [Input("evaluationConfig", "The evaluation config to get the pre-computed module values for.")]
        [Input("quantityValue", "The quantity value of the element to be evaluated.")]
        [Input("mass", "The mass of the element to be evaluated. This is used to calculate the weight factor for the pre-computed module values.")]
        [Output("preComputedModuleValues", "The pre-computed module values for the provided evaluation config.")]
        public static Dictionary<Module, PrecomputedModuleValues> IEvaluationConfigData(this IEvaluationConfig evaluationConfig, double quantityValue, double mass)
        {
            if (evaluationConfig == null)
                return new Dictionary<Module, PrecomputedModuleValues>();

            return EvaluationConfigData(evaluationConfig as dynamic, quantityValue, mass);
        }

        /***************************************************/

        [Description("Provides pre-computed module values for the provided IStructEEvaluationConfig. This is used to provide additional module values for modules that are evaluated based on project level values, such as A5_2 and C1.")]
        [Input("evaluationConfig", "The IStructEEvaluationConfig to get the pre-computed module values for.")]
        [Input("quantityValue", "The quantity value of the element to be evaluated.")]
        [Input("mass", "The mass of the element to be evaluated. This is used to calculate the weight factor for the pre-computed module values.")]
        [Output("preComputedModuleValues", "The pre-computed module values for the provided IStructEEvaluationConfig.")]
        public static Dictionary<Module, PrecomputedModuleValues> EvaluationConfigData(this IStructEEvaluationConfig evaluationConfig, double quantityValue, double mass)
        {
            if(evaluationConfig == null)
                return new Dictionary<Module, PrecomputedModuleValues>();

            double weightFactor;

            if (evaluationConfig.TotalWeight == 0 || evaluationConfig.TotalWeight < mass)
            {
                BH.Engine.Base.Compute.RecordWarning($"The total weight is 0 or smaller than the mass of the element. The weightfactor has been set to 0. This has an influence on the {nameof(Module.A5_2)} and {nameof(Module.C1)} modules, which will be given 0 value results");
                weightFactor = 0;
            }
            else
                weightFactor = mass / evaluationConfig.TotalWeight;

            List<MetricType> applicableTypes = new List<MetricType> { MetricType.ClimateChangeTotal, MetricType.ClimateChangeTotalNoBiogenic, MetricType.ClimateChangeFossil };


            //Special handling of A5_2 for site activities module with additional project factor
            double a5_2Value = evaluationConfig.ProjectCost * evaluationConfig.A5CarbonFactor * weightFactor;  //Set as portion of total project value
            PrecomputedModuleValues a5_2Values = new PrecomputedModuleValues()
            {
                OverwriteExistingValues = true,
                ModuleValues = applicableTypes.ToDictionary(x => x, x => a5_2Value)
            };

            //C1 evaluated based on project level values
            double c1Value = evaluationConfig.FloorArea * evaluationConfig.C1CarbonFactor * weightFactor;  //Set as portion of total project value
            PrecomputedModuleValues c1Values = new PrecomputedModuleValues()
            {
                OverwriteExistingValues = true,
                ModuleValues = applicableTypes.ToDictionary(x => x, x => c1Value)
            };

            return new Dictionary<Module, PrecomputedModuleValues>
            {
                { Module.A5_2, a5_2Values },
                { Module.C1, c1Values }
            };
        }

        /***************************************************/

        [Description("Provides pre-computed module values for the provided GlobalEmissionFactors. This is used to provide additional module values for modules that are evaluated based on project level values, such as A5_1 and A5_2.")]
        [Input("evaluationConfig", "The GlobalEmissionFactors to get the pre-computed module values for.")]
        [Input("quantityValue", "The quantity value of the element to be evaluated.")]
        [Input("mass", "The mass of the element to be evaluated. This is used to calculate the weight factor for the pre-computed module values.")]
        [Output("preComputedModuleValues", "The pre-computed module values for the provided GlobalEmissionFactors.")]
        public static Dictionary<Module, PrecomputedModuleValues> EvaluationConfigData(this GlobalEmissionFactors evaluationConfig, double quantityValue, double mass)
        {
            if(evaluationConfig == null)
                return new Dictionary<Module, PrecomputedModuleValues>();

            double weightFactor;

            if (evaluationConfig.TotalBuildingMass == 0 || evaluationConfig.TotalBuildingMass < mass)
            {
                BH.Engine.Base.Compute.RecordWarning($"The total weight is 0 or smaller than the mass of the element. The weightfactor has been set to 0. This has an influence on the {nameof(Module.A5_1)} and {nameof(Module.A5_2)} modules, which will be given 0 value results");
                weightFactor = 0;
            }
            else
                weightFactor = mass / evaluationConfig.TotalBuildingMass;

            if (evaluationConfig.StructuresOnlyMass)
                weightFactor /= 2; //Divide weight factor by 2 if only structures mass is considered as the total building mass is for the whole building


            //Special handling of A5_1 for pre construction demolition module
            PrecomputedModuleValues a5_1Values = new PrecomputedModuleValues()
            {
                OverwriteExistingValues = true,
                ModuleValues = evaluationConfig.PreConstructionDemolition.EnvironmentalFactors.ToDictionary(x => x.IMetricType(), x => x.Value * evaluationConfig.PreConstructionDemolition.DemolishedFloorArea * weightFactor) //Set as portion of total project value
            };

            //Special handling of A5_2 for site activities module with additional project factor
            PrecomputedModuleValues a5_2Values = new PrecomputedModuleValues()
            {
                OverwriteExistingValues = true,
                ModuleValues = evaluationConfig.ConstructionActivities.EnvironmentalFactors.ToDictionary(x => x.IMetricType(), x => x.Value * evaluationConfig.ConstructionActivities.ConstructedFloorArea * weightFactor) //Set as portion of total project value
            };

            return new Dictionary<Module, PrecomputedModuleValues>
            {
                { Module.A5_1, a5_1Values },
                { Module.A5_2, a5_2Values }
            };
        }

        /***************************************************/
        /**** Private Method - fallback                 ****/
        /***************************************************/

        [Description("Fallback method for providing pre-computed module values for unsupported evaluation configs. This will log a warning and return an empty dictionary.")]
        private static Dictionary<Module, PrecomputedModuleValues> EvaluationConfigData(this IEvaluationConfig evaluationConfig, double quantityValue, double mass)
        {
            BH.Engine.Base.Compute.RecordWarning($"The provided evaluation config of type {evaluationConfig.GetType().Name} is not supported for pre-computation of module values. No pre-computed values will be provided for the modules.");
            return new Dictionary<Module, PrecomputedModuleValues>();
        }

        /***************************************************/
    }
}




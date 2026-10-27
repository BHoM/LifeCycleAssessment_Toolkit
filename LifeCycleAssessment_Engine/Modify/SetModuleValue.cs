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


using BH.oM.Base.Attributes;
using BH.oM.Geometry;
using BH.oM.LifeCycleAssessment;
using BH.oM.LifeCycleAssessment.MaterialFragments;
using BH.oM.LifeCycleAssessment.MaterialFragments.Construction;
using BH.oM.LifeCycleAssessment.MaterialFragments.Transport;
using BH.oM.LifeCycleAssessment.Results;
using BH.oM.Physical.Materials;
using BH.oM.Quantities;
using BH.oM.Quantities.Attributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace BH.Engine.LifeCycleAssessment
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Sets the value of a module in the indicators of the provided metric. The metric is updated in place.\n" +
            "Optionally removes any existing combination modules the module is part of, as well as all parts of the module if it is itself a combination, as these would no longer be consistent with the value set.\n" +
            "For example, setting A5_1 removes A5, and setting A1toA3 removes A1, A2 and A3.")]
        [Input("metric", "The metric to set the module value for. The indicators of the metric are modified.")]
        [Input("module", "The module to set the value for.")]
        [Input("value", "The value to set for the module.")]
        [Input("removeOverlaps", "If true, any existing combinations that the module is part of, and any existing parts of the module if it is a combination, are removed from the indicators.")]
        public static void SetModuleValue(this IEnvironmentalMetric metric, Module module, double value, bool removeOverlaps = true)
        {
            if (metric == null)
            {
                BH.Engine.Base.Compute.RecordError("Unable to set module value, metric is null.");
                return;
            }

            SetModuleValue(metric.Indicators, module, value, removeOverlaps);
        }

        /***************************************************/

        [Description("Sets the values of multiple modules in the indicators of the provided metric. The metric is updated in place.\n" +
            "Optionally removes any existing combination modules the modules are part of, as well as all parts of any of the modules that are combinations, as these would no longer be consistent with the values set.\n" +
            "Modules provided as inputs are never removed, so for example setting both C3 and C3toC4 results in both being present in the indicators.\n" +
            "The modules provided must be unique. If a module is provided more than once, an error is raised and no values are set.")]
        [Input("metric", "The metric to set the module values for. The indicators of the metric are modified.")]
        [Input("modules", "The modules to set the values for. All modules must be unique.")]
        [Input("values", "The values to set for the modules. Must be the same length as the list of modules.")]
        [Input("removeOverlaps", "If true, any existing combinations that any of the modules are part of, and any existing parts of any of the modules that are combinations, are removed from the indicators. Modules in the modules input are never removed.")]
        public static void SetModuleValues(this IEnvironmentalMetric metric, List<Module> modules, List<double> values, bool removeOverlaps = true)
        {
            if (metric == null)
            {
                BH.Engine.Base.Compute.RecordError("Unable to set module values, metric is null.");
                return;
            }

            SetModuleValues(metric.Indicators, modules, values, removeOverlaps);
        }

        /***************************************************/

        [Description("Sets the values of multiple modules for the metric of the provided metric type on the provided EPD. The EPD is updated in place.\n" +
            "If the EPD does not contain a metric of the provided metric type, a new metric of that type is created from the provided modules and values and added to the EPD. In that case there are no existing values to overlap with, so removeOverlaps has no effect.\n" +
            "Otherwise, the indicators of the existing metric are updated, and any existing combination modules the modules are part of, as well as all parts of any of the modules that are combinations, are optionally removed as these would no longer be consistent with the values set.\n" +
            "Modules provided as inputs are never removed, so for example setting both C3 and C3toC4 results in both being present in the indicators.\n" +
            "The modules provided must be unique. If a module is provided more than once, an error is raised and no values are set.")]
        [Input("epd", "The EPD to set the module values on. The indicators of the metric matching the metric type are modified, or a new metric is added to the EPD if none exists.")]
        [Input("metricType", "The type of the metric on the EPD to set the module values for. Must not be Undefined.")]
        [Input("modules", "The modules to set the values for. All modules must be unique.")]
        [Input("values", "The values to set for the modules. Must be the same length as the list of modules.")]
        [Input("removeOverlaps", "If true, any existing combinations that any of the modules are part of, and any existing parts of any of the modules that are combinations, are removed from the indicators. Modules in the modules input are never removed.")]
        public static void SetModuleValues(this EnvironmentalProductDeclaration epd, MetricType metricType, List<Module> modules, List<double> values, bool removeOverlaps = true)
        {
            if (epd == null)
            {
                BH.Engine.Base.Compute.RecordError("Unable to set module values, epd is null.");
                return;
            }

            if (epd.EnvironmentalMetrics == null)
                epd.EnvironmentalMetrics = new List<IEnvironmentalMetric>();


            if (metricType == MetricType.Undefined)
            {
                BH.Engine.Base.Compute.RecordError("Unable to set module values, metricType is undefined.");
                return;
            }

            IEnvironmentalMetric metric = epd.EnvironmentalMetrics.FirstOrDefault(x => x != null && x.IMetricType() == metricType);
            if (metric == null)
            {
                metric = Create.EnvironmentalMetric(metricType, modules, values);
                if (metric != null)
                {
                    epd.EnvironmentalMetrics.Add(metric);
                    BH.Engine.Base.Compute.RecordNote($"The epd did not contain a metric of type {metricType}, so a new metric was created and added to the epd.");
                }

                return;
            }

            SetModuleValues(metric, modules, values, removeOverlaps);
        }

        /***************************************************/

        [Description("Sets the value of a module in the provided dictionary of module values. The dictionary is updated in place.\n" +
            "Optionally removes any existing combination modules the module is part of, as well as all parts of the module if it is itself a combination, as these would no longer be consistent with the value set.\n" +
            "For example, setting A5_1 removes A5, and setting A1toA3 removes A1, A2 and A3.")]
        [Input("moduleValues", "The dictionary of module values to set the value in. The dictionary is modified.")]
        [Input("module", "The module to set the value for.")]
        [Input("value", "The value to set for the module.")]
        [Input("removeOverlaps", "If true, any existing combinations that the module is part of, and any existing parts of the module if it is a combination, are removed from the dictionary.")]
        public static void SetModuleValue(this Dictionary<Module, double> moduleValues, Module module, double value, bool removeOverlaps = true)
        {
            if (moduleValues == null)
            {
                BH.Engine.Base.Compute.RecordError("Unable to set module value, moduleValues is null.");
                return;
            }

            SetModuleValues(moduleValues, new List<Module> { module }, new List<double> { value }, removeOverlaps);
        }

        /***************************************************/

        [Description("Sets the values of multiple modules in the provided dictionary of module values. The dictionary is updated in place.\n" +
            "Optionally removes any existing combination modules the modules are part of, as well as all parts of any of the modules that are combinations, as these would no longer be consistent with the values set.\n" +
            "Modules provided as inputs are never removed, so for example setting both C3 and C3toC4 results in both being present in the dictionary.\n" +
            "The modules provided must be unique. If a module is provided more than once, an error is raised and no values are set.")]
        [Input("moduleValues", "The dictionary of module values to set the values in. The dictionary is modified.")]
        [Input("modules", "The modules to set the values for. All modules must be unique.")]
        [Input("values", "The values to set for the modules. Must be the same length as the list of modules.")]
        [Input("removeOverlaps", "If true, any existing combinations that any of the modules are part of, and any existing parts of any of the modules that are combinations, are removed from the dictionary. Modules in the modules input are never removed.")]
        public static void SetModuleValues(this Dictionary<Module, double> moduleValues, List<Module> modules, List<double> values, bool removeOverlaps = true)
        {
            if (moduleValues == null)
            {
                BH.Engine.Base.Compute.RecordError("Unable to set module values, moduleValues is null.");
                return;
            }

            if (modules == null || values == null)
            {
                BH.Engine.Base.Compute.RecordError("Unable to set module values, modules and values must both be provided.");
                return;
            }

            if (modules.Count != values.Count)
            {
                BH.Engine.Base.Compute.RecordError($"Unable to set module values, the number of modules ({modules.Count}) does not match the number of values ({values.Count}).");
                return;
            }

            List<Module> duplicates = modules.GroupBy(x => x).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
            if (duplicates.Count > 0)
            {
                BH.Engine.Base.Compute.RecordError($"Unable to set module values, the modules provided must be unique. Duplicate modules: {string.Join(", ", duplicates)}.");
                return;
            }

            //Set all values first, then remove any overlapping combinations and parts afterwards
            for (int i = 0; i < modules.Count; i++)
            {
                moduleValues[modules[i]] = values[i];
            }

            if (!removeOverlaps)
                return;

            //All modules that overlap with any of the modules provided, excluding the provided modues themselves
            //The overlapping modules are either modules that the provided modules are part of, or modules that are parts of the provided modules (if they are combinations themselves)
            List<Module> overlappingModules = Query.OverlappingModules(modules).Except(modules).ToList(); 

            foreach (Module module in overlappingModules)
            {
                //Remove any combination that includes the module, directly or indirectly, as its value could now be different than the sum of its parts,
                //as well as any parts of the module if it is a combination, as their values could now be different than the combination value.
                moduleValues.Remove(module);
            }
        }

        /***************************************************/
    }
}




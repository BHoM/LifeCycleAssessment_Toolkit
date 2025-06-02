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

using BH.oM.Base;
using BH.oM.Base.Attributes;
using BH.oM.LifeCycleAssessment;
using BH.oM.LifeCycleAssessment.Interfaces;
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

        [PreviousVersion("8.2", "BH.Engine.LifeCycleAssessment.Query.Total(BH.oM.LifeCycleAssessment.ILifeCycleAssessmentPhaseData)")]
        [Description("Gets the total sum of values from all phases with a set value (all values not NaN).")]
        [Input("moduleData", "LCA phase data object to get the total from.")]
        [Output("total", "The sum of all non-NaN properties on the provided phase data object.")]
        public static double Total(this ILifeCycleAssessmentModuleData<IDictionary<Module, double>> moduleData)
        {
            List<string> included = new List<string>();
            double total = ATotal(moduleData, included);
            total += BTotal(moduleData, included);
            total += CTotal(moduleData, included);
            if(moduleData.Indicators.ContainsKey(Module.D))
            { 
                total += moduleData.Indicators[Module.D];
                included.Add(nameof(Module.D));
            }
            IncludedMessage(included, nameof(Total));
            return total;
        }

        /***************************************************/

        [PreviousVersion("8.2", "BH.Engine.LifeCycleAssessment.Query.ATotal(BH.oM.LifeCycleAssessment.ILifeCycleAssessmentPhaseData)")]
        [Description("Gets the total sum of values from all A-phases (A1-A5) with a set value (all values not NaN).")]
        [Input("moduleData", "LCA phase data object to get the total from.")]
        [Output("Atotal", "The sum of all non-NaN properties on the provided phase data object.")]
        public static double ATotal(this ILifeCycleAssessmentModuleData<IDictionary<Module, double>> moduleData)
        {
            List<string> included = new List<string>();
            double total = ATotal(moduleData, included);
            IncludedMessage(included, nameof(ATotal));
            return total;
        }

        /***************************************************/

        [PreviousVersion("8.2", "BH.Engine.LifeCycleAssessment.Query.BTotal(BH.oM.LifeCycleAssessment.ILifeCycleAssessmentPhaseData)")]
        [Description("Gets the total sum of values from all B-phases (B1-B7) with a set value (all values not NaN).")]
        [Input("moduleData", "LCA phase data object to get the total from.")]
        [Output("BTotal", "The sum of all non-NaN properties on the provided phase data object.")]
        public static double BTotal(this ILifeCycleAssessmentModuleData<IDictionary<Module, double>> moduleData)
        {
            List<string> included = new List<string>();
            double total = BTotal(moduleData, included);
            IncludedMessage(included, nameof(BTotal));
            return total;
        }

        /***************************************************/

        [PreviousVersion("8.2", "BH.Engine.LifeCycleAssessment.Query.CTotal(BH.oM.LifeCycleAssessment.ILifeCycleAssessmentPhaseData)")]
        [Description("Gets the total sum of values from all C-phases (C1-C4) with a set value (all values not NaN).")]
        [Input("moduleData", "LCA phase data object to get the total from.")]
        [Output("CTotal", "The sum of all non-NaN properties on the provided phase data object.")]
        public static double CTotal(this ILifeCycleAssessmentModuleData<IDictionary<Module, double>> moduleData)
        {
            List<string> included = new List<string>();
            double total = CTotal(moduleData, included);
            IncludedMessage(included, nameof(CTotal));
            return total;
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        [Description("Raises a message corresponding to which phases that were included when computing the total.")]
        private static void IncludedMessage(List<string> included, string totalType)
        {
            if(included.Count == 0)
                Base.Compute.RecordWarning($"No phases contains any values for when evaluating {totalType}.");

            else
                Base.Compute.RecordNote($"Phases {string.Join(", ", included)} where included when evaluating {totalType}.");
        }

        /***************************************************/

        [Description("Gets out all A-phases, and stores the non-NaN properties in the included list.")]
        private static double ATotal(this ILifeCycleAssessmentModuleData<IDictionary<Module, double>> moduleData, List<string> included)
        {
            return SumModules(moduleData, m_AModules, included);
        }

        /***************************************************/

        [Description("Gets out all B-phases, and stores the non-NaN properties in the included list.")]
        private static double BTotal(this ILifeCycleAssessmentModuleData<IDictionary<Module, double>> moduleData, List<string> included)
        {
            return SumModules(moduleData, m_BModules, included);
        }

        /***************************************************/

        [Description("Gets out all C-phases, and stores the non-NaN properties in the included list.")]
        private static double CTotal(this ILifeCycleAssessmentModuleData<IDictionary<Module, double>> moduleData, List<string> included)
        {
            return SumModules(moduleData, m_CModules, included);
        }

        /***************************************************/

        private static double SumModules(this ILifeCycleAssessmentModuleData<IDictionary<Module, double>> moduleData, IEnumerable<Module> modulesToSum, List<string> included)
        {
            double total = 0;
            HashSet<Module> modulesLeftToHandle = new HashSet<Module>(modulesToSum);
            IReadOnlyDictionary<Module, IReadOnlyList<(Module, bool)>> combinationModules = CombinationModules();

            foreach (var combination in combinationModules.Reverse())
            {
                if (modulesLeftToHandle.Contains(combination.Key))
                {
                    if (moduleData.Indicators.TryGetValue(combination.Key, out double currentValue))
                    {
                        total += currentValue;
                        included.Add(combination.Key.ToString());
                        modulesLeftToHandle.RemoveModules(combinationModules, combination.Key);
                    }
                    else
                    {
                        modulesLeftToHandle.Remove(combination.Key);
                    }
                }
            }

            foreach (Module subItem in modulesLeftToHandle)
            {
                if (moduleData.Indicators.TryGetValue(subItem, out double subValue))
                {
                    total += subValue;
                    included.Add(subItem.ToString());
                }
            }

            return total;
        }

        /***************************************************/

        private static void RemoveModules(this HashSet<Module> modules, IReadOnlyDictionary<Module, IReadOnlyList<(Module, bool)>> combinationModules, Module moduleToRemove)
        {
            modules.Remove(moduleToRemove);
            if (combinationModules.TryGetValue(moduleToRemove, out IReadOnlyList<(Module, bool)> subModules))
            {
                foreach ((Module, bool) subModuleToRemove in subModules)
                {
                    RemoveModules(modules, combinationModules, subModuleToRemove.Item1);
                }
            }
        }

        /***************************************************/
        /**** Private Feilds                            ****/
        /***************************************************/

        private static List<Module> m_AModules = Enum.GetValues(typeof(Module)).Cast<Module>().Where(x => x.ToString().StartsWith("A")).ToList();
        private static List<Module> m_BModules = Enum.GetValues(typeof(Module)).Cast<Module>().Where(x => x.ToString().StartsWith("B")).ToList();
        private static List<Module> m_CModules = Enum.GetValues(typeof(Module)).Cast<Module>().Where(x => x.ToString().StartsWith("C")).ToList();

        /***************************************************/
    }
}



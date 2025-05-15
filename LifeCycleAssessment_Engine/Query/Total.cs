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
            double total = 0;

            if (!moduleData.Indicators.ContainsKey(Module.A1toA3))
            {
                if (moduleData.Indicators.ContainsKey(Module.A1))
                {
                    total += moduleData.Indicators[Module.A1];
                    included.Add(nameof(Module.A1));
                }

                if (moduleData.Indicators.ContainsKey(Module.A2))
                {
                    total += moduleData.Indicators[Module.A2];
                    included.Add(nameof(Module.A2));
                }

                if (moduleData.Indicators.ContainsKey(Module.A3))
                {
                    total += moduleData.Indicators[Module.A3];
                    included.Add(nameof(Module.A3));
                }
            }
            else
            {
                if (moduleData.Indicators.ContainsKey(Module.A1) && 
                    moduleData.Indicators.ContainsKey(Module.A2) && 
                    moduleData.Indicators.ContainsKey(Module.A3))
                {
                    total += moduleData.Indicators[Module.A1] + moduleData.Indicators[Module.A2] + moduleData.Indicators[Module.A3];
                    included.Add(nameof(Module.A1));
                    included.Add(nameof(Module.A2));
                    included.Add(nameof(Module.A3));
                }
                else
                {
                    total += moduleData.Indicators[Module.A1toA3];
                    included.Add(nameof(Module.A1toA3));
                }
            }

            if (moduleData.Indicators.ContainsKey(Module.A4))
            {
                total += moduleData.Indicators[Module.A4];
                included.Add(nameof(Module.A4));
            }

            if (moduleData.Indicators.ContainsKey(Module.A5))
            {
                total += moduleData.Indicators[Module.A5];
                included.Add(nameof(Module.A5));
            }

            return total;
        }

        /***************************************************/

        [Description("Gets out all B-phases, and stores the non-NaN properties in the included list.")]
        private static double BTotal(this ILifeCycleAssessmentModuleData<IDictionary<Module, double>> moduleData, List<string> included)
        {
            return SumModules(moduleData, m_BModules, included);
            double total = 0;

            if (moduleData.Indicators.ContainsKey(Module.B1toB7))
            {
                if (moduleData.Indicators.ContainsKey(Module.B1))
                {
                    total += moduleData.Indicators[Module.B1];
                    included.Add(nameof(Module.B1));
                }

                if (moduleData.Indicators.ContainsKey(Module.B2))
                {
                    total += moduleData.Indicators[Module.B2];
                    included.Add(nameof(Module.B2));
                }

                if (moduleData.Indicators.ContainsKey(Module.B3))
                {
                    total += moduleData.Indicators[Module.B3];
                    included.Add(nameof(Module.B3));
                }

                if (moduleData.Indicators.ContainsKey(Module.B4))
                {
                    total += moduleData.Indicators[Module.B4];
                    included.Add(nameof(Module.B4));
                }

                if (moduleData.Indicators.ContainsKey(Module.B5))
                {
                    total += moduleData.Indicators[Module.B5];
                    included.Add(nameof(Module.B5));
                }

                if (moduleData.Indicators.ContainsKey(Module.B6))
                {
                    total += moduleData.Indicators[Module.B6];
                    included.Add(nameof(Module.B6));
                }

                if (moduleData.Indicators.ContainsKey(Module.B7))
                {
                    total += moduleData.Indicators[Module.B7];
                    included.Add(nameof(Module.B7));
                }
            }
            else
            {
                if (moduleData.Indicators.ContainsKey(Module.B1) && 
                    !moduleData.Indicators.ContainsKey(Module.B2) && 
                    !moduleData.Indicators.ContainsKey(Module.B3) &&
                    !moduleData.Indicators.ContainsKey(Module.B4) &&
                    !moduleData.Indicators.ContainsKey(Module.B5) &&
                    !moduleData.Indicators.ContainsKey(Module.B6) &&
                    !moduleData.Indicators.ContainsKey(Module.B7))
                {
                    total += moduleData.Indicators[Module.B1] + moduleData.Indicators[Module.B2] + moduleData.Indicators[Module.B3] + moduleData.Indicators[Module.B4] + moduleData.Indicators[Module.B5] + moduleData.Indicators[Module.B6] + moduleData.Indicators[Module.B7];
                    included.Add(nameof(Module.B1));
                    included.Add(nameof(Module.B2));
                    included.Add(nameof(Module.B3));
                    included.Add(nameof(Module.B4));
                    included.Add(nameof(Module.B5));
                    included.Add(nameof(Module.B6));
                    included.Add(nameof(Module.B7));
                }
                else
                {
                    total += moduleData.Indicators[Module.B1toB7];
                    included.Add(nameof(Module.B1toB7));
                }
            }
            return total;
        }

        /***************************************************/

        [Description("Gets out all C-phases, and stores the non-NaN properties in the included list.")]
        private static double CTotal(this ILifeCycleAssessmentModuleData<IDictionary<Module, double>> moduleData, List<string> included)
        {
            return SumModules(moduleData, m_CModules, included);
            double total = 0;

            if (moduleData.Indicators.ContainsKey(Module.C1toC4))
            {
                if (moduleData.Indicators.ContainsKey(Module.C1))
                {
                    total += moduleData.Indicators[Module.C1];
                    included.Add(nameof(Module.C1));
                }

                if (moduleData.Indicators.ContainsKey(Module.C2))
                {
                    total += moduleData.Indicators[Module.C2];
                    included.Add(nameof(Module.C2));
                }

                if (moduleData.Indicators.ContainsKey(Module.C3))
                {
                    total += moduleData.Indicators[Module.C3];
                    included.Add(nameof(Module.C3));
                }

                if (moduleData.Indicators.ContainsKey(Module.C4))
                {
                    total += moduleData.Indicators[Module.C4];
                    included.Add(nameof(Module.C4));
                }
            }
            else
            {
                if (moduleData.Indicators.ContainsKey(Module.C1) &&
                    moduleData.Indicators.ContainsKey(Module.C2) &&
                    moduleData.Indicators.ContainsKey(Module.C3) &&
                    moduleData.Indicators.ContainsKey(Module.C4))
                {
                    total += moduleData.Indicators[Module.C1] + moduleData.Indicators[Module.C2] + moduleData.Indicators[Module.C3] + moduleData.Indicators[Module.C4];
                    included.Add(nameof(Module.C1));
                    included.Add(nameof(Module.C2));
                    included.Add(nameof(Module.C3));
                    included.Add(nameof(Module.C4));
                }
                else
                {
                    total += moduleData.Indicators[Module.C1toC4];
                    included.Add(nameof(Module.C1toC4));
                }
            }

            return total;
        }

        /***************************************************/

        private static double SumModules(this ILifeCycleAssessmentModuleData<IDictionary<Module, double>> moduleData, IEnumerable<Module> modulesToSum, List<string> included)
        {
            double total = 0;
            HashSet<Module> modulesLeftToHandle = new HashSet<Module>(modulesToSum);
            IReadOnlyDictionary<Module, IReadOnlyList<Module>> combinationModules = CombinationModules();

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

        private static void RemoveModules(this HashSet<Module> modules, IReadOnlyDictionary<Module, IReadOnlyList<Module>> combinationModules, Module moduleToRemove)
        {
            modules.Remove(moduleToRemove);
            if (combinationModules.TryGetValue(moduleToRemove, out IReadOnlyList<Module> subModules))
            {
                foreach (Module subModuleToRemove in subModules)
                {
                    RemoveModules(modules, combinationModules, moduleToRemove);
                }
            }
        }

        /***************************************************/

        /***************************************************/

        private static List<Module> m_AModules = Enum.GetValues(typeof(Module)).Cast<Module>().Where(x => x.ToString().StartsWith("A")).ToList();
        private static List<Module> m_BModules = Enum.GetValues(typeof(Module)).Cast<Module>().Where(x => x.ToString().StartsWith("B")).ToList();
        private static List<Module> m_CModules = Enum.GetValues(typeof(Module)).Cast<Module>().Where(x => x.ToString().StartsWith("C")).ToList();

        /***************************************************/
    }
}



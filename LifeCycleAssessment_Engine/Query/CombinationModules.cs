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

using BH.Engine.Base;
using BH.Engine.Matter;
using BH.oM.Base;
using BH.oM.Base.Attributes;
using BH.oM.Dimensional;
using BH.oM.LifeCycleAssessment;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace BH.Engine.LifeCycleAssessment
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Gets all the combined modules and the parts they consist of. All combinations that can be made up of other combinations are, e.g. B1toB5 is set to be a combination of B1toB3 and B4toB5.")]
        [Output("combinationModules", "All combinationModules and their parts.")]
        public static IReadOnlyDictionary<Module, IReadOnlyList<(Module, bool)>> CombinationModules()
        {
            if (m_CombinationModules == null)
            {
                // The combinations of modules and their parts are defined by the PartOf attribute on each module in the Module enum.
                // The bool indicates whether the part is required for the combination to be computed, i.e. true for required parts and false for optional parts.
                // Optional parts are added to the combination if present, but their absence does not prevent the combination from being computed.
                // The order is important:
                // - The parts of a combination are listed in the order they are defined in the Module enum.
                // - The combinations are listed in the order they are defined in the Module enum, except that a combination is always listed after all the parts it is made up of.
                //   Combinations lower down the list can hence be made up of combinations from above in the list.
                List<Module> allModules = Enum.GetValues(typeof(Module)).Cast<Module>().ToList();

                // Collect the parts of each combination. The enum is looped through in order, which gives the parts in the correct order.
                Dictionary<Module, List<(Module, bool)>> parts = new Dictionary<Module, List<(Module, bool)>>();
                foreach (Module module in allModules)
                {
                    PartOfAttribute partOf = System.Reflection.CustomAttributeExtensions.GetCustomAttribute<PartOfAttribute>(typeof(Module).GetField(module.ToString()));
                    if (partOf == null)
                        continue;

                    if (!parts.TryGetValue(partOf.PartOf, out List<(Module, bool)> combinationParts))
                    {
                        combinationParts = new List<(Module, bool)>();
                        parts[partOf.PartOf] = combinationParts;
                    }
                    combinationParts.Add((module, partOf.IsRequired));
                }

                // Add the combinations in the order of the enum, i.e. by the position of the combination itself, not by the position of its first part.
                // The latter would for example put B1toB3 before B1, as B1 is defined before B1_1.
                // For each combination, all parts that are combinations themselves are added first, so the result is correct also if the enum is not ordered that way.
                Dictionary<Module, IReadOnlyList<(Module, bool)>> modules = new Dictionary<Module, IReadOnlyList<(Module, bool)>>();
                HashSet<Module> inProgress = new HashSet<Module>();
                foreach (Module module in allModules)
                {
                    AddCombination(module, parts, modules, inProgress);
                }
                m_CombinationModules = modules;
            }

            return m_CombinationModules;
        }

        [Description("Gets all the combined modules and the parts they consist of. All combinations that can be made up of other combinations are, e.g. B1toB5 is set to be a combination of B1toB3 and B4toB5.")]
        [Output("combinationModules", "All combinationModules and their parts.")]
        public static IReadOnlyDictionary<Module, IReadOnlyList<Module>> PartOfCombinationModules()
        {
            if (m_PartOfCombinationModules == null)
            {
                
                IReadOnlyDictionary<Module, IReadOnlyList<(Module, bool)>> combinationModules = CombinationModules();
                Dictionary<Module, IReadOnlyList<Module>> partCombinations = new Dictionary<Module, IReadOnlyList<Module>>();
                foreach (Module module in Enum.GetValues(typeof(Module)))
                {
                    List<Module> parts = CollectParts(module, combinationModules);
                    if (parts.Any())
                        partCombinations[module] = parts;
                }
                m_PartOfCombinationModules = partCombinations;
            }

            return m_PartOfCombinationModules;
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static void AddCombination(Module module, Dictionary<Module, List<(Module, bool)>> parts, Dictionary<Module, IReadOnlyList<(Module, bool)>> result, HashSet<Module> inProgress)
        {
            //Not a combination, or already added
            if (!parts.TryGetValue(module, out List<(Module, bool)> combinationParts) || result.ContainsKey(module))
                return;

            if (!inProgress.Add(module))
            {
                BH.Engine.Base.Compute.RecordError($"The module {module} is, directly or indirectly, defined as a part of itself via the PartOf attribute. The combination is ignored.");
                return;
            }

            //Make sure all parts that are combinations themselves are added before this one
            foreach ((Module, bool) part in combinationParts)
            {
                AddCombination(part.Item1, parts, result, inProgress);
            }

            inProgress.Remove(module);
            result[module] = combinationParts;
        }

        /***************************************************/

        private static List<Module> CollectParts(Module module, IReadOnlyDictionary<Module, IReadOnlyList<(Module, bool)>> combinationModules)
        { 
            List<Module> parts = new List<Module>();

            foreach (var combinations in combinationModules)
            {
                if(combinations.Value.Any(x => x.Item1 == module))
                {
                    parts.Add(combinations.Key);
                    parts.AddRange(CollectParts(combinations.Key, combinationModules));
                }
            }

            return parts.Distinct().OrderBy(x => x).ToList();
        }

        /***************************************************/
        /**** Private Feilds                            ****/
        /***************************************************/

        private static IReadOnlyDictionary<Module, IReadOnlyList<(Module, bool)>> m_CombinationModules = null;
        private static IReadOnlyDictionary<Module, IReadOnlyList<Module>> m_PartOfCombinationModules = null;

        /***************************************************/
    }
}




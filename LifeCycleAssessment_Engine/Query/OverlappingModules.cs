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

        [Description("Gets all the modules that overlap with each module, i.e. all combinations the module is part of and, if the module is a combination, all the parts it consists of, both directly and indirectly.\n" +
            "For example, B1 overlaps with B1_1, B1_2, B1toB3, B1toB5 and B1toB7. Modules that overlap cannot be set at the same time without the values becoming inconsistent.")]
        [Output("overlappingModules", "For each module, all the modules that it overlaps with.")]
        public static IReadOnlyDictionary<Module, IReadOnlyList<Module>> OverlappingModules()
        {
            if (m_OverlappingModules == null)
            {
                Dictionary<Module, IReadOnlyList<Module>> overlappingModules = new Dictionary<Module, IReadOnlyList<Module>>();
                IReadOnlyDictionary<Module, IReadOnlyList<(Module, bool)>> combinationModules = CombinationModules();
                IReadOnlyDictionary<Module, IReadOnlyList<Module>> moduleParts = PartOfCombinationModules();


                foreach (Module module in Enum.GetValues(typeof(Module)).Cast<Module>())
                {
                    List<Module> overlapping = new List<Module>();

                    if(moduleParts.TryGetValue(module, out IReadOnlyList<Module> combinations))
                    {
                        overlapping.AddRange(combinations);
                    }
                    overlapping.AddRange(AllCombinationModules(module, combinationModules));

                    overlappingModules[module] = overlapping.Distinct().OrderBy(x => x).ToList();
                }

                m_OverlappingModules = overlappingModules;
            }

            return m_OverlappingModules;
        }


        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

       
        private static List<Module> AllCombinationModules(Module module, IReadOnlyDictionary<Module, IReadOnlyList<(Module, bool)>> combinationModules)
        {
            List<Module> allCombinationModules = new List<Module>();
            if (combinationModules.TryGetValue(module, out IReadOnlyList<(Module, bool)> parts))
            {
                foreach ((Module, bool) part in parts)
                {
                    Module partModule = part.Item1;
                    allCombinationModules.Add(partModule);
                    allCombinationModules.AddRange(AllCombinationModules(partModule, combinationModules));
                }
            }
            return allCombinationModules;
        }


        /***************************************************/
        /**** Private Feilds                            ****/
        /***************************************************/

        private static IReadOnlyDictionary<Module, IReadOnlyList<Module>> m_OverlappingModules = null;

        /***************************************************/
    }
}




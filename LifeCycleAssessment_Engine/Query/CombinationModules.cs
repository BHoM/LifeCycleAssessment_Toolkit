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

        [Description("Gets all the combined modules and the parts they consist of. All combinations that can be made up of other combinations are, e.g. B1toB5 is set to be a combination of B1toB3 and B4toB5.")]
        [Output("combinationModules", "All combinationModules and their parts.")]
        public static IReadOnlyDictionary<Module, IReadOnlyList<Module>> CombinationModules()
        {
            if (m_CombinationModules == null)
            {
                Dictionary<Module, IReadOnlyList<Module>> modules = new Dictionary<Module, IReadOnlyList<Module>>();
                modules[Module.A1toA3] = new List<Module>() { Module.A1, Module.A2, Module.A3 };
                modules[Module.A5] = new List<Module>() { Module.A5a, Module.A5w };
                modules[Module.B1toB3] = new List<Module>() { Module.B1, Module.B2, Module.B3 };
                modules[Module.B4toB5] = new List<Module>() { Module.B4, Module.B5 };
                modules[Module.B1toB5] = new List<Module>() { Module.B1toB3, Module.B4toB5 };
                modules[Module.B1toB7] = new List<Module>() { Module.B1toB5, Module.B6, Module.B7 };
                modules[Module.C3toC4] = new List<Module>() { Module.C3, Module.C4 };
                modules[Module.C1toC4] = new List<Module>() { Module.C1, Module.C2, Module.C3toC4 };
                m_CombinationModules = modules;
            }

            return m_CombinationModules;
        }

        /***************************************************/
        /**** Private Feilds                            ****/
        /***************************************************/

        private static IReadOnlyDictionary<Module, IReadOnlyList<Module>> m_CombinationModules = null;

        /***************************************************/
    }
}



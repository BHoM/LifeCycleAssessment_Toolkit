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
        public static IReadOnlyDictionary<Module, IReadOnlyList<(Module, bool)>> CombinationModules()
        {
            if (m_CombinationModules == null)
            {
                // Define the combinations of modules and their parts.
                // The bool indicates whether the module is a combination or not, i.e. true for combinations and false for parts.
                // The order of the parts is important, as it defines the order in which they are combined and caluculated. Parts lower down the lift can be made up of parts from above in the list
                Dictionary<Module, IReadOnlyList<(Module, bool)>> modules = new Dictionary<Module, IReadOnlyList<(Module, bool)>>();
                modules[Module.A1toA3] = new List<(Module, bool)>() { (Module.A1, true), (Module.A2,true), (Module.A3, true) };
                modules[Module.A5] = new List<(Module, bool)>() { (Module.A5_1, false), (Module.A5_2, true), (Module.A5_3, true), (Module.A5_4, false) };
                modules[Module.B1] = new List<(Module, bool)>() { (Module.B1_1, true), (Module.B1_2, true) };
                modules[Module.B1toB3] = new List<(Module, bool)>() { (Module.B1, true), (Module.B2, true), (Module.B3, true) };
                modules[Module.B4] = new List<(Module, bool)>() { (Module.B4_1, true), (Module.B4_2, true) };
                modules[Module.B4toB5] = new List<(Module, bool)>() { (Module.B4, true), (Module.B5, true) };
                modules[Module.B1toB5] = new List<(Module, bool)>() { (Module.B1toB3, true), (Module.B4toB5, true) };
                modules[Module.B7] = new List<(Module, bool)>() { (Module.B7_1, true), (Module.B7_2, true), (Module.B7_3, true) };
                modules[Module.B1toB7] = new List<(Module, bool)>() { (Module.B1toB5, true), (Module.B6, true), (Module.B7, true) };
                modules[Module.B8] = new List<(Module, bool)>() { (Module.B8_1, false), (Module.B8_2, false), (Module.B8_3, false) };
                modules[Module.C3toC4] = new List<(Module, bool)>() { (Module.C3, true), (Module.C4, true) };
                modules[Module.C1toC4] = new List<(Module, bool)>() { (Module.C1, true), (Module.C2, true), (Module.C3toC4, true) };
                modules[Module.D] = new List<(Module, bool)>() { (Module.D_1, true), (Module.D_2, true) };
                m_CombinationModules = modules;
            }

            return m_CombinationModules;
        }

        /***************************************************/
        /**** Private Feilds                            ****/
        /***************************************************/

        private static IReadOnlyDictionary<Module, IReadOnlyList<(Module, bool)>> m_CombinationModules = null;

        /***************************************************/
    }
}



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
using BH.oM.LifeCycleAssessment.Configs;
using BH.oM.LifeCycleAssessment.Fragments;
using BH.oM.LifeCycleAssessment.Interfaces;
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

        [Description("Gets the value of a module from the module data, taking into account any combination modules and their parts.")]
        [Input("moduleData", "The module data to get the value from.")]
        [Input("module", "The module to get the value for.")]
        [Input("requireAllRequiredSubParts", "If true, all required sub-parts must be present in the module data for the value to be calculated. If false, the value will be calculated from the available sub-parts, requiring at least one subpart to be present.")]
        [Output("value", "The value of the module.")]
        public static double ModuleValue(this ILifeCycleAssessmentModuleData<IDictionary<Module, double>> moduleData, Module module, bool requireAllRequiredSubParts = true)
        {
            if (moduleData == null)
                return double.NaN;

            return moduleData.Indicators.ModuleValue(module, requireAllRequiredSubParts);
        }

        /***************************************************/

        [Description("Gets the value of a module from the module data, taking into account any combination modules and their parts.")]
        [Input("moduleData", "The module data to get the value from.")]
        [Input("module", "The module to get the value for.")]
        [Input("requireAllRequiredSubParts", "If true, all required sub-parts must be present in the module data for the value to be calculated. If false, the value will be calculated from the available sub-parts, requiring at least one subpart to be present.")]
        [Output("value", "The value of the module.")]
        public static double ModuleValue(this IDictionary<Module, double> moduleData, Module module, bool requireAllRequiredSubParts = true)
        {
            if (moduleData == null)
                return double.NaN;

            if (moduleData.TryGetModuleValue(module, out double value, requireAllRequiredSubParts))
                return value;

            return double.NaN;
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        [Description("Tries to get the value of a module from the module data, taking into account any combination modules and their parts.")]
        [Input("moduleData", "The module data to get the value from.")]
        [Input("module", "The module to get the value for.")]
        [Input("requireAllRequiredSubParts", "If true, all required sub-parts must be present in the module data for the value to be calculated. If false, the value will be calculated from the available sub-parts, requiring at least one subpart to be present.")]
        [Output("value", "The value of the module.")]
        private static bool TryGetModuleValue(this IDictionary<Module, double> moduleData, Module module, out double value, bool requireAllRequiredSubParts = true)
        {
            if (moduleData == null)
            {
                value = double.NaN;
                return false;
            }

            if (moduleData.TryGetValue(module, out value))
                return true;

            if (Query.CombinationModules().TryGetValue(module, out IReadOnlyList<(Module, bool)> parts))
            {
                if (requireAllRequiredSubParts)
                {
                    // If all required sub-parts are needed, check if all are present
                    value = 0;
                    foreach (var part in parts)
                    {
                        if (part.Item2) // If the part is required
                        {
                            if (!moduleData.TryGetModuleValue(part.Item1, out double subValue))
                            {
                                value = double.NaN;
                                return false; // Required part is missing
                            }
                            value += subValue;
                        }
                        else
                        {
                            // If the part is not required, add its value if present
                            if (moduleData.TryGetModuleValue(part.Item1, out double subValue))
                                value += subValue;
                        }
                    }
                    return true;
                }
                else
                {
                    // If not all are required, return the sum of available parts
                    //Check that at least one part is present
                    value = 0;
                    bool atLeastOnePartPresent = false;
                    foreach (var part in parts)
                    {

                        // If the part is not required, add its value if present
                        if (moduleData.TryGetModuleValue(part.Item1, out double subValue))
                        {
                            value += subValue;
                            atLeastOnePartPresent = true;
                        }
                    }

                    return atLeastOnePartPresent;
                }
            }

            value = double.NaN;
            return false;
        }

        /***************************************************/
    }
}




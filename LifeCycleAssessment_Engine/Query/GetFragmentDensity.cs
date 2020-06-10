/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2020, the respective contributors. All rights reserved.
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using BH.oM.Base;
using BH.Engine.Base;
using BH.oM.Reflection.Attributes;
using BH.oM.LifeCycleAssessment;
using BH.oM.Quantities.Attributes;
using BH.Engine.Reflection;
using BH.oM.LifeCycleAssessment.MaterialFragments;

using BH.Engine.Matter;

namespace BH.Engine.LifeCycleAssessment
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Query a Scope object to see if it contains any data.")]
        [Input("epd", "The StructuresScope object used within your LCA to query.")]
        [Output("density", "True if the object contains data, False if the object does not contain data.")]
        public static double GetFragmentDensity(this IEnvironmentalProductDeclarationData epd)
        {
            if (epd == null || epd.Density == 0 || epd.Density == double.NaN)
            {
                BH.Engine.Reflection.Compute.RecordError("The input object does not contain a valid EnvironmentalProductDeclaration MaterialFragment.");
                return double.NaN;
            }
            else
            {
                object density = 0.0;
                density = System.Convert.ToDouble(epd.PropertyValue("Density"));

                return System.Convert.ToDouble(density);
            }
        }
    }
}

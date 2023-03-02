/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2022, the respective contributors. All rights reserved.
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

using System.ComponentModel;
using BH.oM.Base.Attributes;
using BH.Engine.Matter;
using BH.oM.Dimensional;
using BH.oM.LifeCycleAssessment.MaterialFragments;
using System.Collections.Generic;
using System.Linq;
using BH.oM.Physical.Materials;
using BH.Engine.LifeCycleAssessment.Objects;

namespace BH.Engine.LifeCycleAssessment
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Query the Environmental Product Declarations from any IElementM with a MaterialComposition composed of IEPD materials.")]
        [Input("elementM", "A IElementM from which to query the EPD.")]
        [Output("epd", "The EPD or EPDs used to define the material makeup of an object.")]
        public static List<EnvironmentalProductDeclaration> GetElementEpd(this IElementM elementM)
        {
            if (elementM == null)
            {
                BH.Engine.Base.Compute.RecordError("No IElementM was provided.");
            }

            MaterialComposition mc = elementM.IMaterialComposition();
            if (mc == null)
            {
                Base.Compute.RecordError("Material composition could not be assessed. Please add materials to your objects and try again.");
                return null;
            }

            return HelperMethods.GetElementEpd(elementM, mc);
        }

        /***************************************************/
    }
}


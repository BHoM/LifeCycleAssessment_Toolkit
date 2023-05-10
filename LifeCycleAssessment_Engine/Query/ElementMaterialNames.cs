/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2023, the respective contributors. All rights reserved.
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
using BH.oM.Dimensional;
using BH.oM.Base.Attributes;
using System.ComponentModel;
using BH.Engine.Matter;
using System.Linq;
using System.Collections.Generic;

namespace BH.Engine.LifeCycleAssessment
{
    public static partial class Query
    {
        /***************************************************/
        /****   Public Methods                          ****/
        /***************************************************/

        [PreviousVersion("6.2", "BH.Engine.LifeCycleAssessment.Query.GetElementMaterial(BH.oM.Dimensional.IElementM)")]
        [Description("Query the element's MaterialComposition to form a Material Hint to aid in EPD-Material Mapping.")]
        [Input("elementM", "The IElementM object from which to query the object's material type hint.")]
        [Output("materialHint", "The Material Names found within the MaterialComposition.")]
        public static List<string> ElementMaterialNames(this IElementM elementM)
        {
            List<string> mat = new List<string>();

            if (elementM == null)
                return null;

            return Matter.Query.IMaterialComposition(elementM).Materials.Select(x => x.Name).ToList();
        }

        /***************************************************/
    }
}



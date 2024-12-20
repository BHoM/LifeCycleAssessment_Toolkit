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

using BH.oM.LifeCycleAssessment;
using BH.oM.LifeCycleAssessment.MaterialFragments;
using BH.oM.Base.Attributes;
using System.ComponentModel;
using BH.oM.Dimensional;
using System.Collections.Generic;
using BH.Engine.Matter;
using System.Linq;

namespace BH.Engine.LifeCycleAssessment
{
    public static partial class Query
    {
        /***************************************************/
        /****   Public Methods                          ****/
        /***************************************************/

        [Description("Query the QuantityType values from any IElementM object's MaterialComposition.")]
        [Input("elementM", "The IElementM object from which to query the EPD's QuantityType values.")]
        [Output("quantityType", "The quantityType values from the IEnvironmentalProductDeclarationData objects found within the Element's MaterialComposition.")]
        public static List<QuantityType> QuantityTypes(this IElementM elementM)
        {
            List<QuantityType> qt = new List<QuantityType>();

            if (elementM == null)
            {
                Base.Compute.RecordError("Cannot get the QuantityType from a null element.");
                return new List<QuantityType> { oM.LifeCycleAssessment.QuantityType.Undefined };
            }

            return elementM.ElementEpds().Select(x => x == null ? oM.LifeCycleAssessment.QuantityType.Undefined : x.QuantityType).ToList();


        }

        /***************************************************/
    }
}





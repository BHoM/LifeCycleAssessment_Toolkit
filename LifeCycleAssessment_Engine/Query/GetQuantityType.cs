/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2021, the respective contributors. All rights reserved.
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

using BH.Engine.Matter;
using BH.oM.Dimensional;
using BH.oM.LifeCycleAssessment;
using BH.oM.LifeCycleAssessment.MaterialFragments;
using BH.oM.Reflection.Attributes;
using System.Collections.Generic;
using System.ComponentModel;
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
        [PreviousVersion("4.2", "BH.Engine.LifeCycleAssessment.Query.GetQuantityType(BH.oM.LifeCycleAssessment.MaterialFragment.IEnvironmentalProductDeclarationData)")]
        public static List<QuantityType> GetQuantityType(this IElementM elementM)
        {
            List<QuantityType> qt = new List<QuantityType>();

            if (elementM == null)
                return new List<QuantityType> { QuantityType.Undefined }; 

            qt = elementM.IMaterialComposition().Materials.Where(x => x != null).Select(x =>
            {
                var epd = x.Properties.Where(y => y is EnvironmentalProductDeclaration).FirstOrDefault() as EnvironmentalProductDeclaration;
                if (epd != null)
                    return epd.QuantityType;
                return QuantityType.Undefined;
            }).ToList();

            return qt;
        }

        /***************************************************/
    }
}

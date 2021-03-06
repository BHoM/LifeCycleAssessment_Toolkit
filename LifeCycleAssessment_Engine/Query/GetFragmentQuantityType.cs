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

using System.ComponentModel;
using System.Linq;
using BH.oM.Reflection.Attributes;
using BH.oM.Dimensional;
using BH.oM.LifeCycleAssessment;
using BH.oM.LifeCycleAssessment.MaterialFragments;
using BH.Engine.Matter;
using System.Collections.Generic;

namespace BH.Engine.LifeCycleAssessment
{
    public static partial class Query
    {
        /***************************************************/
        /****   Public Methods                          ****/
        /***************************************************/

        [Description("Query the QuantityType value from any IEnvironmentalProductDeclarationData object.")]
        [Input("epd", "IEnvironmentalProductDeclarationData object from which to query.")]
        [Output("quantityType", "The quantityType value from the provided IEPD.")]
        public static QuantityType GetFragmentQuantityType(this IEnvironmentalProductDeclarationData epd)
        {
            if (epd == null)
            {
                BH.Engine.Reflection.Compute.RecordError("The input object must have a Volume property for this method to work.");
            }
            return epd.QuantityType;
        }

        /***************************************************/
    }
}

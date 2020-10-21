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
using System.ComponentModel;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using BH.Engine.Base;
using BH.Engine.Reflection;
using BH.oM.Reflection.Attributes;
using BH.oM.Base;
using BH.oM.Dimensional;
using BH.oM.LifeCycleAssessment;
using BH.oM.LifeCycleAssessment.MaterialFragments;

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
        public static object GetFragmentQuantityType(this IEnvironmentalProductDeclarationData epd)
        {
            if (epd == null)
            {
                BH.Engine.Reflection.Compute.RecordError("The input object must have a Volume property for this method to work.");
            }
            return epd.QuantityType;
        }

        /***************************************************/

        [Description("Query the QuantityType value from any IElementM object's Fragments.")]
        [Input("elementM", "The IElementM object from which to query the EPD's QuantityType value.")]
        [Output("quantityType", "The quantityType value from the provided IEPD.")]
        public static QuantityType QuantityType(this IElementM elementM)
        {
            if (elementM == null)
                return oM.LifeCycleAssessment.QuantityType.Undefined;

            return ((IBHoMObject)elementM).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().QuantityType;
        }
    }
}
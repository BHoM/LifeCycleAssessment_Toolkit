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

using BH.Engine.Base;
using BH.oM.Base;
using BH.oM.Base.Attributes;
using BH.oM.Dimensional;
using BH.oM.LifeCycleAssessment;
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

        [PreviousVersion("6.2", "BH.Engine.LifeCycleAssessment.Query.GetElementScope(BH.oM.Dimensional.IElementM)")]
        [Description("Returns the enumerable type of the scope found on an element.")]
        [Input("elementM", "A IElementM from which to query the EPD.")]
        [Output("scopeType", "The type of scope the element is being set to within the evaluate EPD method.")]
        public static ScopeType ElementScope(this IElementM elementM)
        {
            if (elementM == null)
            {
                BH.Engine.Base.Compute.RecordError("No IElementM was provided.");
                return ScopeType.Undefined;
            }

            // get scope fragment
            Scope scope = ((IBHoMObject)elementM).FindFragment<Scope>();

            if (scope == null)
            {
                return ScopeType.Undefined;
            }

            ScopeType type = scope.ScopeType;

            return type;
        }

        /***************************************************/
    }
}



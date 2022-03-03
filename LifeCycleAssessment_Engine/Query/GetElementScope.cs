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

        [Description("Returns the enumerable type of the scope found on an element.")]
        [Input("elementM", "A IElementM from which to query the EPD.")]
        [Output("scopeType", "The type of scope the element is being set to within the evaluate EPD method.")]

        public static ScopeType GetElementScope(this IElementM elementM)
        {
            if (elementM == null)
            {
                BH.Engine.Base.Compute.RecordError("No IElementM was provided.");
            }

            // get all fragments
            List<IFragment> fragments = Base.Query.GetAllFragments((IBHoMObject)elementM);

            // return note that no fragments were found
            if (fragments.Count <= 0)
            {
                Base.Compute.RecordNote("No fragments were found on this element. Add a Scope fragment and try again.");
                return ScopeType.Undefined;
            }

            // get scope fragment
            Scope scope = fragments.Select(x => x).Where(y => y is Scope).FirstOrDefault() as Scope;

            ScopeType type = scope.ScopeType;

            if (scope == null)
            {
                BH.Engine.Base.Compute.RecordError("No scopes could be found within the objects. Add a scope fragment and try again.");
            }

            return type;
        }

        /***************************************************/
    }
}


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

using BH.oM.Dimensional;
using BH.oM.Reflection.Attributes;
using System.ComponentModel;
using BH.Engine.Base;
using BH.Engine.Reflection;
using BH.oM.Base;
using System.Collections.Generic;
using System.Reflection;

namespace BH.Engine.LifeCycleAssessment
{
    public static partial class Query
    {
        /***************************************************/
        /****   Public Methods                          ****/
        /***************************************************/

        [Description("Query the Density values from any IElementM object's MaterialComposition which hosts Environmental Product Declaration materials.")]
        [Input("elementM", "The IElementM object from which to query the object's material density.")]
        [Output("density", "The Density values from the Environmental Product Declarations found within the Element's MaterialComposition.")]
        public static string GetElementMaterial(this IElementM elementM)
        {
            string mat = "";
            string noMat = "No material name could be derived from the object.";

            if (elementM == null)
                return noMat;

            //List<IFragment> fragments = BH.Engine.Base.Query.GetAllFragments((IBHoMObject)elementM);
            //if (fragments.Count <= 0)
            //    return noMat;

            // Get Family Name
            System.Reflection.PropertyInfo familyName = Base.Query.GetAllFragments((IBHoMObject)elementM).GetType().GetProperty("FamilyType");

            // Get Family Type Name
            System.Reflection.PropertyInfo familyTypeName = Base.Query.GetAllFragments((IBHoMObject)elementM).GetType().GetProperty("FamilyTypeName");

            mat = familyName + " " + familyTypeName;

            return mat;
        }

        /***************************************************/
    }
}

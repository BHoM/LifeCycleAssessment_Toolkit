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

//using BH.oM.LifeCycleAssessment.MaterialFragments
using BH.oM.Reflection.Attributes;
using BH.oM.Quantities.Attributes;
using System.ComponentModel;
using System.Collections.Generic;
using System;
using BH.oM.Physical.Materials;
using BH.oM.LifeCycleAssessment;
using BH.Engine.Geometry;
using BH.Engine.Matter;
using BH.oM.Base;

namespace BH.Engine.LifeCycleAssessment
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Query a StructuresScope object to see if it contains any data.")]
        [Input("structuresScope", "The StructuresScope object used within your LCA to query.")]
        [Output("boolean", "True if the object contains data, False if the object does not contain data.")]
        public static bool isValid(this StructuresScope structuresScope)
        {
            int val = structuresScope.Beams.Count + structuresScope.Columns.Count + structuresScope.CoreWalls.Count + structuresScope.Slabs.Count;
            if(val > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        [Description("Query a FoundationsScope object to see if it contains any data.")]
        [Input("foundationsScope", "The FoundationsScope object used within your LCA to query.")]
        [Output("boolean", "True if the object contains data, False if the object does not contain data.")]
        public static bool isValid(this FoundationsScope foundationsScope)
        {
            int val = foundationsScope.Footings.Count + foundationsScope.Piles.Count + foundationsScope.Slabs.Count + foundationsScope.Walls.Count;
            if (val > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        [Description("Query a EnclosuresScope object to see if it contains any data.")]
        [Input("enclosuresScope", "The EnclosuresScope object used within your LCA to query.")]
        [Output("boolean", "True if the object contains data, False if the object does not contain data.")]
        public static bool isValid(this EnclosuresScope enclosuresScope)
        {
            int val = enclosuresScope.CurtainWalls.Count + enclosuresScope.Doors.Count + enclosuresScope.Walls.Count + enclosuresScope.Windows.Count;
            if (val > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        [Description("Query a MEPScope object to see if it contains any data.")]
        [Input("mepScope", "The MEPScope object used within your LCA to query.")]
        [Output("boolean", "True if the object contains data, False if the object does not contain data.")]
        public static bool isValid(this MEPScope mepScope)
        {
            int val = mepScope.Batteries.Count + mepScope.Conduit.Count + mepScope.Ductwork.Count + mepScope.Equipment.Count + mepScope.Generators.Count + mepScope.Lighting.Count + mepScope.Piping.Count + mepScope.Wiring.Count;
            if (val > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        [Description("Query a TenantImprovementScope object to see if it contains any data.")]
        [Input("tiScope", "The TenantImprovementScope object used within your LCA to query.")]
        [Output("boolean", "True if the object contains data, False if the object does not contain data.")]
        public static bool isValid(this TenantImprovementScope tiScope)
        {
            int val = tiScope.Ceiling.Count + tiScope.Finishes.Count + tiScope.Flooring.Count + tiScope.Furniture.Count + tiScope.InteriorDoors.Count + tiScope.InteriorGlazing.Count + tiScope.PartitionWalls.Count;
            if (val > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /***************************************************/

    }
}

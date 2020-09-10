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
        public static bool IsValid(this StructuresScope structuresScope)
        {
            if (structuresScope.Beams.Count == 0)
            {
                BH.Engine.Reflection.Compute.RecordNote("No objects within StructuresScope Beams.");
            }

            if (structuresScope.Slabs.Count == 0)
            {
                BH.Engine.Reflection.Compute.RecordNote("No objects within StructuresScope Slabs.");
            }

            if (structuresScope.Columns.Count == 0)
            {
                BH.Engine.Reflection.Compute.RecordNote("No objects within StructuresScope Columns.");
            }

            if (structuresScope.CoreWalls.Count == 0)
            {
                BH.Engine.Reflection.Compute.RecordNote("No objects within StructuresScope CoreWalls.");
            }

            if (structuresScope.Bracing.Count == 0)
            {
                BH.Engine.Reflection.Compute.RecordNote("No objects within StructuresScope Bracing.");
            }

            if (structuresScope.AdditionalObjects.Count > 0)
            {
                BH.Engine.Reflection.Compute.RecordNote("You are accounting for additional user objects, this will be noted in the results.");
            }

            int val = structuresScope.Beams.Count + structuresScope.Columns.Count + structuresScope.CoreWalls.Count + structuresScope.Slabs.Count + structuresScope.Bracing.Count;
            if(val > 0 || structuresScope.AdditionalObjects.Count > 0)
            {
                return true;
            }

            else
            {
                return false;
            }
        }

        /***************************************************/

        [Description("Query a FoundationsScope object to see if it contains any data.")]
        [Input("foundationsScope", "The FoundationsScope object used within your LCA to query.")]
        [Output("boolean", "True if the object contains data, False if the object does not contain data.")]
        public static bool IsValid(this FoundationsScope foundationsScope)
        {
            if(foundationsScope.Footings.Count == 0)
            {
                BH.Engine.Reflection.Compute.RecordNote("No objects within FoundationsScope Footings.");
            }

            if(foundationsScope.Piles.Count == 0)
            {
                BH.Engine.Reflection.Compute.RecordNote("No objects within FoundationScope Piles.");
            }

            if (foundationsScope.Slabs.Count == 0)
            {
                BH.Engine.Reflection.Compute.RecordNote("No objects within FoundationScope Slabs.");
            }

            if (foundationsScope.Walls.Count == 0)
            {
                BH.Engine.Reflection.Compute.RecordNote("No objects within FoundationScope Walls.");
            }

            if (foundationsScope.GradeBeams.Count == 0)
            {
                BH.Engine.Reflection.Compute.RecordNote("No objects within FoundationScope GradeBeams.");
            }

            if (foundationsScope.AdditionalObjects.Count > 0)
            {
                BH.Engine.Reflection.Compute.RecordNote("You are accounting for additional user objects, this will be noted in the results.");
            }

            int val = foundationsScope.Footings.Count + foundationsScope.Piles.Count + foundationsScope.Slabs.Count + foundationsScope.Walls.Count + foundationsScope.GradeBeams.Count;
            if (val > 0 || foundationsScope.AdditionalObjects.Count > 0)
            {
                return true;
            }

            else
            {
                return false;
            }
        }

        /***************************************************/

        [Description("Query a EnclosuresScope object to see if it contains any data.")]
        [Input("enclosuresScope", "The EnclosuresScope object used within your LCA to query.")]
        [Output("boolean", "True if the object contains data, False if the object does not contain data.")]
        public static bool IsValid(this EnclosuresScope enclosuresScope)
        {
            if (enclosuresScope.CurtainWalls.Count == 0)
            {
                BH.Engine.Reflection.Compute.RecordNote("No objects within EnclosuresScope CurtainWalls.");
            }

            if (enclosuresScope.Doors.Count == 0)
            {
                BH.Engine.Reflection.Compute.RecordNote("No objects within EnclosuresScope Doors.");
            }

            if (enclosuresScope.Walls.Count == 0)
            {
                BH.Engine.Reflection.Compute.RecordNote("No objects within EnclosuresScope Walls.");
            }

            if (enclosuresScope.Windows.Count == 0)
            {
                BH.Engine.Reflection.Compute.RecordNote("No objects within EnclosuresScope Windows.");
            }

            if (enclosuresScope.AdditionalObjects.Count > 0)
            {
                BH.Engine.Reflection.Compute.RecordNote("You are accounting for additional user objects, this will be noted in the results.");
            }

            int val = enclosuresScope.CurtainWalls.Count + enclosuresScope.Doors.Count + enclosuresScope.Walls.Count + enclosuresScope.Windows.Count;
            if (val > 0 || enclosuresScope.AdditionalObjects.Count > 0)
            {
                return true;
            }

            else
            {
                return false;
            }
        }

        /***************************************************/

        [Description("Query a MEPScope object to see if it contains any data.")]
        [Input("mepScope", "The MEPScope object used within your LCA to query.")]
        [Output("boolean", "True if the object contains data, False if the object does not contain data.")]
        public static bool IsValid(this MEPScope mepScope)
        {
            if (mepScope.Batteries.Count == 0)
            {
                BH.Engine.Reflection.Compute.RecordNote("No objects within MEPScope Batteries.");
            }

            if (mepScope.Conduit.Count == 0)
            {
                BH.Engine.Reflection.Compute.RecordNote("No objects within MEPScope Conduit.");
            }

            if (mepScope.Ductwork.Count == 0)
            {
                BH.Engine.Reflection.Compute.RecordNote("No objects within MEPScope Ductwork.");
            }

            if (mepScope.Equipment.Count == 0)
            {
                BH.Engine.Reflection.Compute.RecordNote("No objects within MEPScope Equipment.");
            }

            if (mepScope.Generators.Count == 0)
            {
                BH.Engine.Reflection.Compute.RecordNote("No objects within MEPScope Generators.");
            }

            if (mepScope.Lighting.Count == 0)
            {
                BH.Engine.Reflection.Compute.RecordNote("No objects within MEPScope Lighting.");
            }

            if (mepScope.Piping.Count == 0)
            {
                BH.Engine.Reflection.Compute.RecordNote("No objects within MEPScope Piping.");
            }

            if (mepScope.Wiring.Count == 0)
            {
                BH.Engine.Reflection.Compute.RecordNote("No objects within MEPScope Wiring.");
            }

            if (mepScope.AdditionalObjects.Count > 0)
            {
                BH.Engine.Reflection.Compute.RecordNote("You are accounting for additional user objects, this will be noted in the results.");
            }

            int val = mepScope.Batteries.Count + mepScope.Conduit.Count + mepScope.Ductwork.Count + mepScope.Equipment.Count + mepScope.Generators.Count + mepScope.Lighting.Count + mepScope.Piping.Count + mepScope.Wiring.Count;
            if (val > 0 || mepScope.AdditionalObjects.Count > 0)
            {
                return true;
            }

            else
            {
                return false;
            }
        }

        /***************************************************/

        [Description("Query a TenantImprovementScope object to see if it contains any data.")]
        [Input("tiScope", "The TenantImprovementScope object used within your LCA to query.")]
        [Output("boolean", "True if the object contains data, False if the object does not contain data.")]
        public static bool IsValid(this TenantImprovementScope tiScope)
        {
            if (tiScope.Ceiling.Count == 0)
            {
                BH.Engine.Reflection.Compute.RecordNote("No objects within TenantImprovementScope Ceiling.");
            }

            if (tiScope.Finishes.Count == 0)
            {
                BH.Engine.Reflection.Compute.RecordNote("No objects within TenantImprovementScope Finishes.");
            }

            if (tiScope.Flooring.Count == 0)
            {
                BH.Engine.Reflection.Compute.RecordNote("No objects within TenantImprovementScope Flooring.");
            }

            if (tiScope.Furniture.Count == 0)
            {
                BH.Engine.Reflection.Compute.RecordNote("No objects within TenantImprovementScope Furniture.");
            }

            if (tiScope.InteriorDoors.Count == 0)
            {
                BH.Engine.Reflection.Compute.RecordNote("No objects within TenantImprovementScope InteriorDoors.");
            }

            if (tiScope.InteriorGlazing.Count == 0)
            {
                BH.Engine.Reflection.Compute.RecordNote("No objects within TenantImprovementScope InteriorGlazing.");
            }

            if (tiScope.PartitionWalls.Count == 0)
            {
                BH.Engine.Reflection.Compute.RecordNote("No objects within TenantImprovementScope PartitionWalls.");
            }

            if (tiScope.AdditionalObjects.Count > 0)
            {
                BH.Engine.Reflection.Compute.RecordNote("You are accounting for additional user objects, this will be noted in the results.");
            }

            int val = tiScope.Ceiling.Count + tiScope.Finishes.Count + tiScope.Flooring.Count + tiScope.Furniture.Count + tiScope.InteriorDoors.Count + tiScope.InteriorGlazing.Count + tiScope.PartitionWalls.Count;
            if (val > 0 || tiScope.AdditionalObjects.Count > 0)
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

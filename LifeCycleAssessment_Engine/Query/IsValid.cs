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

using BH.oM.Reflection.Attributes;
using System.ComponentModel;
using BH.oM.LifeCycleAssessment;

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
        [Input("mepScope", "The MEPScope object to query.")]
        [Output("boolean", "True if the object contains any data, False if the object does not contain any data.")]
        public static bool IsValid(this MEPScope mepScope)
        {
            //MechanicalScope

            if (mepScope.MechanicalScope.AirTerminals.Count == 0)
            {
                BH.Engine.Reflection.Compute.RecordNote("No objects within MechanicalScope Air Terminals.");
            }

            if (mepScope.MechanicalScope.Dampers.Count == 0)
            {
                BH.Engine.Reflection.Compute.RecordNote("No objects within MechanicalScope Dampers.");
            }

            if (mepScope.MechanicalScope.Ducts.Count == 0)
            {
                BH.Engine.Reflection.Compute.RecordNote("No objects within MechanicalScope Ducts.");
            }

            if (mepScope.MechanicalScope.Equipment.Count == 0)
            {
                BH.Engine.Reflection.Compute.RecordNote("No objects within MechanicalScope Equipment.");
            }

            if (mepScope.MechanicalScope.Pipes.Count == 0)
            {
                BH.Engine.Reflection.Compute.RecordNote("No objects within MechanicalScope Pipes.");
            }

            if (mepScope.MechanicalScope.Refrigerants.Count == 0)
            {
                BH.Engine.Reflection.Compute.RecordNote("No objects within MechanicalScope Refrigerants.");
            }

            if (mepScope.MechanicalScope.Tanks.Count == 0)
            {
                BH.Engine.Reflection.Compute.RecordNote("No objects within MechanicalScope Tanks.");
            }

            if (mepScope.MechanicalScope.Valves.Count == 0)
            {
                BH.Engine.Reflection.Compute.RecordNote("No objects within MechanicalScope Valves.");
            }

            if (mepScope.MechanicalScope.AdditionalObjects.Count > 0)
            {
                BH.Engine.Reflection.Compute.RecordNote("You are accounting for additional user objects, this will be noted in the results.");
            }

            //ElectricalScope

            if (mepScope.ElectricalScope.Batteries.Count == 0)
            {
                BH.Engine.Reflection.Compute.RecordNote("No objects within ElectricalScope Batteries.");
            }

            if (mepScope.ElectricalScope.CableTrays.Count == 0)
            {
                BH.Engine.Reflection.Compute.RecordNote("No objects within ElectricalScope CableTrays.");
            }

            if (mepScope.ElectricalScope.Conduit.Count == 0)
            {
                BH.Engine.Reflection.Compute.RecordNote("No objects within ElectricalScope Conduit.");
            }

            if (mepScope.ElectricalScope.Equipment.Count == 0)
            {
                BH.Engine.Reflection.Compute.RecordNote("No objects within ElectricalScope Equipment.");
            }

            if (mepScope.ElectricalScope.FireAlarmDevices.Count == 0)
            {
                BH.Engine.Reflection.Compute.RecordNote("No objects within ElectricalScope FireAlarmDevices.");
            }

            if (mepScope.ElectricalScope.Generators.Count == 0)
            {
                BH.Engine.Reflection.Compute.RecordNote("No objects within ElectricalScope Generators.");
            }

            if (mepScope.ElectricalScope.InformationCommunicationDevices.Count == 0)
            {
                BH.Engine.Reflection.Compute.RecordNote("No objects within ElectricalScope InformationCommunicationDevices.");
            }

            if (mepScope.ElectricalScope.LightFixtures.Count == 0)
            {
                BH.Engine.Reflection.Compute.RecordNote("No objects within ElectricalScope LightFixtures.");
            }

            if (mepScope.ElectricalScope.LightingControls.Count == 0)
            {
                BH.Engine.Reflection.Compute.RecordNote("No objects within ElectricalScope LightingControls.");
            }

            if (mepScope.ElectricalScope.Meters.Count == 0)
            {
                BH.Engine.Reflection.Compute.RecordNote("No objects within ElectricalScope Meters.");
            }

            if (mepScope.ElectricalScope.SecurityDevices.Count == 0)
            {
                BH.Engine.Reflection.Compute.RecordNote("No objects within ElectricalScope SecurityDevices.");
            }

            if (mepScope.ElectricalScope.Sockets.Count == 0)
            {
                BH.Engine.Reflection.Compute.RecordNote("No objects within ElectricalScope Sockets.");
            }

            if (mepScope.ElectricalScope.SolarPanels.Count == 0)
            {
                BH.Engine.Reflection.Compute.RecordNote("No objects within ElectricalScope SolarPanels.");
            }

            if (mepScope.ElectricalScope.WireSegments.Count == 0)
            {
                BH.Engine.Reflection.Compute.RecordNote("No objects within ElectricalScope WireSegments.");
            }

            if (mepScope.ElectricalScope.AdditionalObjects.Count > 0)
            {
                BH.Engine.Reflection.Compute.RecordNote("You are accounting for additional user objects, this will be noted in the results.");
            }

            //PlumbingScope

            if (mepScope.PlumbingScope.Equipment.Count == 0)
            {
                BH.Engine.Reflection.Compute.RecordNote("No objects within PlumbingScope Equipment.");
            }

            if (mepScope.PlumbingScope.Pipes.Count == 0)
            {
                BH.Engine.Reflection.Compute.RecordNote("No objects within PlumbingScope Pipes.");
            }

            if (mepScope.PlumbingScope.PlumbingFixtures.Count == 0)
            {
                BH.Engine.Reflection.Compute.RecordNote("No objects within PlumbingScope PlumbingFixtures.");
            }

            if (mepScope.PlumbingScope.Tanks.Count == 0)
            {
                BH.Engine.Reflection.Compute.RecordNote("No objects within PlumbingScope Tanks.");
            }

            if (mepScope.PlumbingScope.Valves.Count == 0)
            {
                BH.Engine.Reflection.Compute.RecordNote("No objects within PlumbingScope Valves.");
            }

            if (mepScope.PlumbingScope.AdditionalObjects.Count > 0)
            {
                BH.Engine.Reflection.Compute.RecordNote("You are accounting for additional user objects, this will be noted in the results.");
            }

            //FireProtectionScope

            if (mepScope.FireProtectionScope.Equipment.Count == 0)
            {
                BH.Engine.Reflection.Compute.RecordNote("No objects within FireProtectionScope Equipment.");
            }

            if (mepScope.FireProtectionScope.Pipes.Count == 0)
            {
                BH.Engine.Reflection.Compute.RecordNote("No objects within FireProtectionScope Pipes.");
            }

            if (mepScope.FireProtectionScope.Sprinklers.Count == 0)
            {
                BH.Engine.Reflection.Compute.RecordNote("No objects within FireProtectionScope Sprinklers.");
            }

            if (mepScope.FireProtectionScope.Tanks.Count == 0)
            {
                BH.Engine.Reflection.Compute.RecordNote("No objects within FireProtectionScope Tanks.");
            }

            if (mepScope.FireProtectionScope.AdditionalObjects.Count > 0)
            {
                BH.Engine.Reflection.Compute.RecordNote("You are accounting for additional user objects, this will be noted in the results.");
            }

            int val =
                mepScope.MechanicalScope.AirTerminals.Count
                + mepScope.MechanicalScope.Dampers.Count
                + mepScope.MechanicalScope.Ducts.Count
                + mepScope.MechanicalScope.Equipment.Count 
                + mepScope.MechanicalScope.Pipes.Count
                + mepScope.MechanicalScope.Refrigerants.Count
                + mepScope.MechanicalScope.Tanks.Count
                + mepScope.MechanicalScope.Valves.Count
                + mepScope.ElectricalScope.Batteries.Count
                + mepScope.ElectricalScope.CableTrays.Count
                + mepScope.ElectricalScope.Conduit.Count 
                + mepScope.ElectricalScope.Equipment.Count
                + mepScope.ElectricalScope.FireAlarmDevices.Count
                + mepScope.ElectricalScope.Generators.Count
                + mepScope.ElectricalScope.InformationCommunicationDevices.Count
                + mepScope.ElectricalScope.LightFixtures.Count
                + mepScope.ElectricalScope.LightingControls.Count
                + mepScope.ElectricalScope.Meters.Count
                + mepScope.ElectricalScope.SecurityDevices.Count
                + mepScope.ElectricalScope.Sockets.Count
                + mepScope.ElectricalScope.SolarPanels.Count
                + mepScope.ElectricalScope.WireSegments.Count
                + mepScope.PlumbingScope.Equipment.Count
                + mepScope.PlumbingScope.Pipes.Count
                + mepScope.PlumbingScope.PlumbingFixtures.Count
                + mepScope.PlumbingScope.Tanks.Count
                + mepScope.PlumbingScope.Valves.Count
                + mepScope.FireProtectionScope.Equipment.Count
                + mepScope.FireProtectionScope.Pipes.Count
                + mepScope.FireProtectionScope.Sprinklers.Count
                + mepScope.FireProtectionScope.Tanks.Count;

            if (val > 0 || mepScope.MechanicalScope.AdditionalObjects.Count > 0 || mepScope.ElectricalScope.AdditionalObjects.Count > 0 || mepScope.PlumbingScope.AdditionalObjects.Count > 0 || mepScope.FireProtectionScope.AdditionalObjects.Count > 0)
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



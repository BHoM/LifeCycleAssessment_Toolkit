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

using System.Collections.Generic;
using System.ComponentModel;
using BH.oM.Base;

namespace BH.oM.LifeCycleAssessment
{
    [Description("The Life Cycle Assessment Scope object intends to provide a means of reporting all of the project criteria (name, area, type, location) as well as the objects that the study encompassed (structural slabs, foundation walls, etc) along with their properties for the Enviornmental Product Declarations they used (when using SetProperty), their densities and volumes. This object may be used for studies at any stage of development and can serve as a true means of 'apples to apples' comparison when catalogued.")]
    public class TenantImprovementScope : BHoMObject
    {
        /***************************************************/
        /**** Properties                                ****/
        /***************************************************/
        [Description("Tenant Improvement Ceiling is a material that creates an additional upper interior surface in a room")]
        public BHoMObject TenantImprovementsCeiling { get; set; } = null;
        [Description("Tenant Improvements Flooring  is inclusive of the flooring materials placed on top of the structural floor (eg carpet, tile)")]
        public BHoMObject TenantImprovementsFlooring { get; set; } = null;
        [Description("Tenant Improvements Finishes is inclusive of finishes (eg paint)")]
        public BHoMObject TenantImprovementsFinishes { get; set; } = null;
        [Description("Tenant Improvements Interior Glazing is inclusive of windows in the interior of the building")]
        public BHoMObject TenantImprovementsInteriorGlazing { get; set; } = null;
        [Description("Tenant Improvements Furniture includes furnishings (eg tables, chairs, desks)")]
        public BHoMObject TenantImprovementsFurniture { get; set; } = null;
        [Description("Tenant Improvements Interior Doors includes doors in the interior of the building")]
        public BHoMObject TenantImprovementsInteriorDoors { get; set; } = null;
        [Description("Tenant Improvements Partition Walls includes walls in the interior of the building")]
        public BHoMObject TenantImprovementsPartitionWalls { get; set; } = null;

        /***************************************************/
    }
}

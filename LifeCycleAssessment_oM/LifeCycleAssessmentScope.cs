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
    public class LifeCycleAssessmentScope : BHoMObject
    {
        /***************************************************/
        /**** Properties                                ****/
        /***************************************************/
        [Description ("The Project Name denotes the name of the project (eg Mercedes-Benz Stadium)")]
        public string ProjectName { get; set; } = "";
        [Description("The Contact Name denotes the person/people who performed the LCA study")]
        public string ContactName { get; set; } = "";
        [Description("The Actual Project Area denotes the more precise project area (m2) which will allow assessment of kgCO2eq/m2 metrics")]
        public double ActualProjectArea { get; set; } = double.NaN;
        [Description("Biogenic Carbon is a true/false that indicates that the project contains materials that originated from a biological source (trees, soil), these materials have the ability sequester/store carbon.")]
        public bool BiogenicCarbon { get; set; } = false;
        [Description("Zip Code is the means of tracking the project's location")]
        public int ZipCode { get; set; } = 00000;
        [Description("Primary dataset used within the Life Cycle Assessment")]
        public string Dataset { get; set; } = "";
        [Description("Additional notes should convey project design constraints (eg design for seismic activity) that could affect the overall embodied carbon")]
        public string AdditionalNotes { get; set; } = "";
        public StructuresScope StructuresScope { get; set; } = new StructuresScope();
        public EnclosuresScope EnclosuresScope { get; set; } = new EnclosuresScope();
        public FoundationsScope FoundationsScope { get; set; } = new FoundationsScope();
        public MEPScope MEPScope { get; set; } = new MEPScope();
        public TenantImprovementScope TenantImprovementScope { get; set; } = new TenantImprovementScope();

        [Description("Enclosure walls are inclusive of the opaque exterior wall assemblies of a building")]
        public BHoMObject EnclosuresWalls { get; set; } = null;
        [Description("Enclosure curtain walls are large sheets of transparent glazing on the building exterior")]
        public BHoMObject EnclosuresCurtainWalls { get; set; } = null;
        [Description("Enclosure windows are are openings in the building exterior, which consist of framing and glazing")]
        public BHoMObject EnclosuresWindows { get; set; } = null;
        [Description("Enclosure doors are are openings in the building exterior, which consist of framing and panels")]
        public BHoMObject EnclosuresDoors { get; set; } = null;
        [Description("Foundation footings (or pile caps) are mats below the buildings piles that help to distribute the load from the structure above")]
        public BHoMObject FoundationsFootings { get; set; } = null;
        [Description("Foundation piles are structural supports that are driven into the ground below a building to support the building structure")]
        public BHoMObject FoundationsPiles { get; set; } = null;
        [Description("Foundation walls are structural walls built below-grade")]
        public BHoMObject FoundationsWalls { get; set; } = null;
        [Description("Foundation slabs are structural slabs upon which the building is constructed. This category expects any type of slab, but assumes no construction properties.")]
        public BHoMObject FoundationsSlabs { get; set; } = null;
        [Description("MEP Equipment is a machine that processes mechanical, electrical or plumbing loads (eg Fan, Electrical Panel, Pump")]
        public BHoMObject MEPEquipment { get; set; } = null;
        [Description("MEP Ductwork is a material (eg sheet metal) that helps to convey airflow from heating, ventilation or cooling systems")]
        public BHoMObject MEPDuctwork { get; set; } = null;
        [Description("MEP Generators are devices that convert mechanical energy to electrical power")]
        public BHoMObject MEPGenerators { get; set; } = null;
        [Description("MEP Conduit is a tube used to route electrical wiring")]
        public BHoMObject MEPConduit { get; set; } = null;
        [Description("MEP Wiring is a flexible conductor of electricity")]
        public BHoMObject MEPWiring { get; set; } = null;
        [Description("MEP Lighting is inclusive of all light fixtures")]
        public BHoMObject MEPLighting { get; set; } = null;
        [Description("MEP Piping is a material (eg copper) that helps to convey fluids (eg water, waste) within a building")]
        public BHoMObject MEPPiping { get; set; } = null;
        [Description("MEP Batties are energy storage devices (eg photovoltaic panels)")]
        public BHoMObject MEPBatteries { get; set; } = null;
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

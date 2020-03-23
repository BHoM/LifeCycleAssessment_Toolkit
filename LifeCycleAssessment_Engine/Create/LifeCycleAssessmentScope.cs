using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using BH.oM.Base;

using BH.oM.Reflection.Attributes;
using BH.oM.LifeCycleAssessment;

namespace BH.Engine.LifeCycleAssessment
{
    public static partial class Create
    {
        [Description("The Life Cycle Assessment Scope object intends to provide a means of reporting all of the project criteria (name, area, type, location) as well as the objects that the study encompassed (structural slabs, foundation walls, etc) along with their properties for the Enviornmental Product Declarations they used (when using SetProperty), their densities and volumes. This object may be used for studies at any stage of development and can serve as a true means of 'apples to apples' comparison when catalogued.")]
        [Input("projectArea", "Enum - Enter description here!")]
        [Input("projectType", "Enum - Enter description here!")]
        [Input("levelOfDesign", "Enum - Enter description here!")]
        [Input("lifeCycleAssessmentPhases", "Enum - Enter description here!")]
        [Input("primaryStructuralMaterial", "Enum - Enter description here!")]
        [Input("projectName", "The Project Name denotes the name of the project (eg Mercedes-Benz Stadium)")]
        [Input("contactName", "The Contact Name denotes the person/people who performed the LCA study")]
        [Input("actualProjectArea", "The Actual Project Area denotes the more precise project area (m2) which will allow assessment of kgCO2eq/m2 metrics")]
        [Input("zipCode", "Zip Code is the means of tracking the project's location")]
        [Input("biogenicCarbon", "Biogenic Carbon is a true / false that indicates that the project contains materials that originated from a biological source(trees, soil), these materials have the ability sequester / store carbon.")]
        [Input("additionalNotes", "Additional notes should convey project design constraints (eg design for seismic activity) that could affect the overall embodied carbon")]
        [Input("strucutresSlabs", "Structural slabs are inclusive of the above-grade structural floors in a building")]
        [Input("strucutresCoreWalls", "Structural core walls are inclusive of the above-grade, structural-grade walls surrounding the core (elevators, building services)")]
        [Input("strucutresBeams", "Structural beams are typically horizontal elements that carry the load of floors, roofs, and ceilings")]
        [Input("strucutresColumns", "Structural columns are typically vertical elements that carry the load of floors, roofs, and ceilings")]
        [Input("enclosuresWalls", "Enclosure walls are inclusive of the opaque exterior wall assemblies of a building")]
        [Input("enclosuresCurtainWalls", "Enclosure curtain walls are large sheets of transparent glazing on the building exterior")]
        [Input("enclosuresWindows", "Enclosure windows are are openings in the building exterior, which consist of framing and glazing")]
        [Input("enclosuresDoors", "Enclosure doors are are openings in the building exterior, which consist of framing and panels")]
        [Input("foundationsFootings", "Foundation footings (or pile caps) are mats below the buildings piles that help to distribute the load from the structure above")]
        [Input("foundationsPiles", "Foundation piles are structural supports that are driven into the ground below a building to support the building structure")]
        [Input("foundationsWalls", "Foundation walls are structural walls built below-grade")]
        [Input("foundationsSlabs", "Foundation slabs are structural slabs upon which the building is constructed. This category expects any type of slab, but assumes no construction properties.")]
        [Input("mepEquipment", "MEP Equipment is a machine that processes mechanical, electrical or plumbing loads (eg Fan, Electrical Panel, Pump")]
        [Input("mepDuctwork", "MEP Ductwork is a material (eg sheet metal) that helps to convey airflow from heating, ventilation or cooling systems")]
        [Input("mepGenerators", "MEP Generators are devices that convert mechanical energy to electrical power")]
        [Input("mepConduit", "MEP Conduit is a tube used to route electrical wiring")]
        [Input("mepWiring", "MEP Wiring is a flexible conductor of electricity")]
        [Input("mepLighting", "MEP Lighting is inclusive of all light fixtures")]
        [Input("mepPiping", "MEP Piping is a material (eg copper) that helps to convey fluids (eg water, waste) within a building")]
        [Input("mepBatteries", "MEP Batties are energy storage devices (eg photovoltaic panels)")]
        [Input("tenantImprovementsCeiling", "Tenant Improvement Ceiling is a material that creates an additional upper interior surface in a room")]
        [Input("tenantImprovementsFlooring", "Tenant Improvements Flooring  is inclusive of the flooring materials placed on top of the structural floor (eg carpet, tile)")]
        [Input("tenantImprovementsFinishes", "Tenant Improvements Finishes is inclusive of finishes (eg paint)")]
        [Input("tenantImprovementsInteriorGlazing", "Tenant Improvements Interior Glazing is inclusive of windows in the interior of the building")]
        [Input("tenantImprovementsFurniture", "Tenant Improvements Furniture includes furnishings (eg tables, chairs, desks)")]
        [Input("tenantImprovementsInteriorDoors", "Tenant Improvements Interior Doors includes doors in the interior of the building")]
        [Input("tenantImprovementsPartitionWalls", "Tenant Improvements Partition Walls includes walls in the interior of the building")]
        [Output("lifeCycleAssessment", "A lifeCycleAssessment object for capturing and comparing additional studies. This object can be passed directly to a database for storage and further study.")]
        public static LifeCycleAssessmentScope LifeCycleAssessmentScope(ProjectArea projectArea, ProjectType projectType, LevelOfDevelopment levelOfDevelopment, LifeCycleAssessmentPhases lifeCycleAssessmentPhases, PrimaryStructuralMaterial primaryStructuralMaterial,
            string ProjectName = "",
            string ContactName = "",
            double ActualProjectArea = 0,
            bool BiogenicCarbon = false,
            int ZipCode = 00000,
            string AdditionalNotes = "",
            BHoMObject StructuresSlabs = null,
            BHoMObject StructuresCoreWalls = null,
            BHoMObject StructuresBeams = null,
            BHoMObject StructuresColumns = null,
            BHoMObject EnclosuresWalls = null,
            BHoMObject EnclosuresCurtainWalls = null,
            BHoMObject EnclosuresWindows = null,
            BHoMObject EnclosuresDoors = null,
            BHoMObject FoundationsFootings = null,
            BHoMObject FoundationsPiles = null,
            BHoMObject FoundationsWalls = null,
            BHoMObject FoundationsSlabs = null,
            BHoMObject MEPEquipment = null,
            BHoMObject MEPDuctwork = null,
            BHoMObject MEPGenerators = null,
            BHoMObject MEPConduit = null,
            BHoMObject MEPWiring = null,
            BHoMObject MEPLighting = null,
            BHoMObject MEPPiping = null,
            BHoMObject MEPBatteries = null,
            BHoMObject TenantImprovementsCeiling = null,
            BHoMObject TenantImprovementsFlooring = null,
            BHoMObject TenantImprovementsFinishes = null,
            BHoMObject TenantImprovementsInteriorGlazing = null,
            BHoMObject TenantImprovementsFurniture = null,
            BHoMObject TenantImprovementsInteriorDoors = null,
            BHoMObject TenantImprovementsPartitionWalls = null
        )
        {
            return new oM.LifeCycleAssessment.LifeCycleAssessmentScope
            {
                ProjectName = ProjectName,
                ContactName = ContactName,
                ActualProjectArea = ActualProjectArea,
                BiogenicCarbon = BiogenicCarbon,
                ZipCode = ZipCode,
                AdditionalNotes = AdditionalNotes,
                StructuresSlabs = StructuresSlabs,
                StructuresCoreWalls = StructuresCoreWalls,
                StructuresBeams = StructuresBeams,
                StructuresColumns = StructuresColumns,
                EnclosuresCurtainWalls = EnclosuresCurtainWalls,
                EnclosuresWalls = EnclosuresWalls,
                EnclosuresWindows = EnclosuresWindows,
                EnclosuresDoors = EnclosuresDoors,
                FoundationsFootings = FoundationsFootings,
                FoundationsPiles = FoundationsPiles,
                FoundationsSlabs = FoundationsSlabs,
                FoundationsWalls = FoundationsWalls,
                MEPEquipment = MEPEquipment,
                MEPDuctwork = MEPDuctwork, 
                MEPGenerators = MEPGenerators,
                MEPConduit = MEPConduit,
                MEPWiring = MEPWiring,
                MEPLighting = MEPLighting,
                MEPPiping = MEPPiping,
                MEPBatteries = MEPBatteries,
                TenantImprovementsCeiling = TenantImprovementsCeiling,
                TenantImprovementsFlooring = TenantImprovementsFlooring,
                TenantImprovementsFinishes = TenantImprovementsFinishes,
                TenantImprovementsInteriorGlazing =TenantImprovementsInteriorGlazing,
                TenantImprovementsFurniture = TenantImprovementsFurniture,
                TenantImprovementsInteriorDoors = TenantImprovementsInteriorDoors,
                TenantImprovementsPartitionWalls = TenantImprovementsPartitionWalls
            };
        }
    }
}
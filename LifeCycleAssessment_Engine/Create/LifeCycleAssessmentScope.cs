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
        [Description("A BH.oM.Climate.PointCondition object to define the non-climatic variables for a point in space and time being analysed. For example, the clothing or thermophsyical characteristics of a person, or local conditions such as ground reflectivity.")]
        [Input("projectArea", "Enum - Enter description here!")]
        [Input("projectType", "Enum - Enter description here!")]
        [Input("levelOfDesign", "Enum - Enter description here!")]
        [Input("lifeCycleAssessmentPhases", "Enum - Enter description here!")]
        [Input("primaryStructuralMaterial", "Enum - Enter description here!")]
        [Input("projectName", "Enter description here!")]
        [Input("contactName", "Enter description here!")]
        [Input("actualProjectArea", "Enter description here!")]
        [Input("zipCode", "Enter description here!")]
        [Input("additionalNotes", "Enter description here!")]
        [Input("strucutresSlabs", "Enter description here!")]
        [Input("strucutresCoreWalls", "Enter description here!")]
        [Input("strucutresBeams", "Enter description here!")]
        [Input("strucutresColumns", "Enter description here!")]
        [Input("enclosuresWalls", "Enter description here!")]
        [Input("enclosuresCurtainWalls", "Enter description here!")]
        [Input("enclosuresWindows", "Enter description here!")]
        [Input("enclosuresDoors", "Enter description here!")]
        [Input("foundationsFootings", "Enter description here!")]
        [Input("foundationsPiles", "Enter description here!")]
        [Input("foundationsWalls", "Enter description here!")]
        [Input("foundationsSlabs", "Enter description here!")]
        [Input("mepEquipment", "Enter description here!")]
        [Input("mepDuctwork", "Enter description here!")]
        [Input("mepGenerators", "Enter description here!")]
        [Input("mepConduit", "Enter description here!")]
        [Input("mepWiring", "Enter description here!")]
        [Input("mepLighting", "Enter description here!")]
        [Input("mepPiping", "Enter description here!")]
        [Input("mepBatteries", "Enter description here!")]
        [Input("tenantImprovementsCeiling", "Enter description here!")]
        [Input("tenantImprovementsFlooring", "Enter description here!")]
        [Input("tenantImprovementsFinishes", "Enter description here!")]
        [Input("tenantImprovementsInteriorGlazing", "Enter description here!")]
        [Input("tenantImprovementsFurniture", "Enter description here!")]
        [Input("tenantImprovementsInteriorDoors", "Enter description here!")]
        [Input("tenantImprovementsPartitionWalls", "Enter description here!")]
        [Output("scope", "A BH.oM.Climate.PointCondition object")]
        public static LifeCycleAssessmentScope LifeCycleAssessmentScope(ProjectArea projectArea, ProjectType projectType, LevelOfDesign levelOfDesign, LifeCycleAssessmentPhases lifeCycleAssessmentPhases, PrimaryStructuralMaterial primaryStructuralMaterial,
            string ProjectName = "",
            string ContactName = "",
            double ActualProjectArea = 0,
            bool BiogenicCarbon = false,
            int ZipCode = 00000,
            string AdditionalNotes = "",
            BHoMObject StructureSlabs = null,
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
                ActualProtjectArea = ActualProjectArea,
                BiogenicCarbon = BiogenicCarbon,
                ZipCode = ZipCode,
                AdditionalNotes = AdditionalNotes,
                StructuresSlabs = StructureSlabs,
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
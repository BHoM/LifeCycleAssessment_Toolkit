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
using BH.oM.Base;

namespace BH.oM.LifeCycleAssessment
{
    public class LifeCycleAssessmentScope : BHoMObject
    {
        /***************************************************/
        /**** Properties                                ****/
        /***************************************************/
        public string ProjectName { get; set; } = "";
        public string ContactName { get; set; } = "";
        public double ActualProtjectArea { get; set; } = double.NaN;
        //public enum ProjectArea { };
        //public enum ProjectType { };
        //public enum LevelOfDesign { };
        //public enum LifeCycleAssessmentPhases { };
        //public enum PrimaryStructuralMaterial { };
        public bool BiogenicCarbon { get; set; } = false;
        public int ZipCode { get; set; } = 00000;
        public string AdditionalNotes { get; set; } = "";
        public BHoMObject StructuresSlabs { get; set; } = null;
        public BHoMObject StructuresCoreWalls { get; set; } = null;
        public BHoMObject StructuresBeams { get; set; } = null;
        public BHoMObject StructuresColumns { get; set; } = null;
        public BHoMObject EnclosuresWalls { get; set; } = null;
        public BHoMObject EnclosuresCurtainWalls { get; set; } = null;
        public BHoMObject EnclosuresWindows { get; set; } = null;
        public BHoMObject EnclosuresDoors { get; set; } = null;
        public BHoMObject FoundationsFootings { get; set; } = null;
        public BHoMObject FoundationsPiles { get; set; } = null;
        public BHoMObject FoundationsWalls { get; set; } = null;
        public BHoMObject FoundationsSlabs { get; set; } = null;
        public BHoMObject MEPEquipment { get; set; } = null;
        public BHoMObject MEPDuctwork { get; set; } = null;
        public BHoMObject MEPGenerators { get; set; } = null;
        public BHoMObject MEPConduit { get; set; } = null;
        public BHoMObject MEPWiring { get; set; } = null;
        public BHoMObject MEPLighting { get; set; } = null;
        public BHoMObject MEPPiping { get; set; } = null;
        public BHoMObject MEPBatteries { get; set; } = null;
        public BHoMObject TenantImprovementsCeiling { get; set; } = null;
        public BHoMObject TenantImprovementsFlooring { get; set; } = null;
        public BHoMObject TenantImprovementsFinishes { get; set; } = null;
        public BHoMObject TenantImprovementsInteriorGlazing { get; set; } = null;
        public BHoMObject TenantImprovementsFurniture { get; set; } = null;
        public BHoMObject TenantImprovementsInteriorDoors { get; set; } = null;
        public BHoMObject TenantImprovementsPartitionWalls { get; set; } = null;

        /***************************************************/
    }
}

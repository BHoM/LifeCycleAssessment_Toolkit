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

using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using BH.oM.Base;
using BH.oM.Reflection.Attributes;
using BH.oM.LifeCycleAssessment;
using BH.oM.LifeCycleAssessment.Results;
using BH.oM.MEP.System;

namespace BH.Engine.LifeCycleAssessment
{
    public static partial class Compute
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/
        [Description("This method calls the appropriate compute method per object within the StructuresScope and returns results.")]
        [Input("structuresScope", "This is a life cycle assessment scope object which contains all specified objects along with their associated EPD MaterialFragment data.")]
        [Input("field", "This is the desired field you would like to evaluate. Notice that not all material datasets will contain information for all metrics. A holistic GWP evaluation is currently the only standard evaluation possible with all provided datasets.")]
        [Output("result", "The LifeCycleElementResult that contains structures scope specific data, total results, and results per element.")]
        public static List<LifeCycleAssessmentElementResult> EvaluateLifeCycleAssessmentScope(this StructuresScope structuresScope, EnvironmentalProductDeclarationField field)
        {
            if (structuresScope.IsValid() == false)
            {
                BH.Engine.Reflection.Compute.RecordWarning("Your StructuresScope is incomplete, please provide all types of objects where possible.");
            }

            List<LifeCycleAssessmentElementResult> results = new List<LifeCycleAssessmentElementResult>();

            //StructuresScope Beams
            List<LifeCycleAssessmentElementResult> beamResults = structuresScope.Beams.SelectMany(x => EvaluateEnvironmentalProductDeclarationPerObject(x, field).Collapse()).ToList();
            for (int x = 0; x < beamResults.Count; x++)
            {
                beamResults[x] = new GlobalWarmingPotentialResult(beamResults[x].ObjectId, beamResults[x].ResultCase, 0, ObjectScope.Structure, ObjectCategory.Beam, beamResults[x].EnvironmentalProductDeclaration, (beamResults[x] as GlobalWarmingPotentialResult).GlobalWarmingPotential);
            }
            results.AddRange(beamResults);

            //StructuresScope Columns
            List<LifeCycleAssessmentElementResult> columnsResults = structuresScope.Columns.SelectMany(x => EvaluateEnvironmentalProductDeclarationPerObject(x, field).Collapse()).ToList();
            for (int x = 0; x < columnsResults.Count; x++)
            {
                columnsResults[x] = new GlobalWarmingPotentialResult(columnsResults[x].ObjectId, columnsResults[x].ResultCase, 0, ObjectScope.Structure, ObjectCategory.Column, columnsResults[x].EnvironmentalProductDeclaration, (columnsResults[x] as GlobalWarmingPotentialResult).GlobalWarmingPotential);
            }
            results.AddRange(columnsResults);

            //StructuresScope Slabs
            List<LifeCycleAssessmentElementResult> slabsResults = structuresScope.Slabs.SelectMany(x => EvaluateEnvironmentalProductDeclarationPerObject(x, field).Collapse()).ToList();
            for (int x = 0; x < slabsResults.Count; x++)
            {
                slabsResults[x] = new GlobalWarmingPotentialResult(slabsResults[x].ObjectId, slabsResults[x].ResultCase, 0, ObjectScope.Structure, ObjectCategory.Slab, slabsResults[x].EnvironmentalProductDeclaration, (slabsResults[x] as GlobalWarmingPotentialResult).GlobalWarmingPotential);
            }
            results.AddRange(slabsResults);

            //StructuresScope Core Walls
            List<LifeCycleAssessmentElementResult> coreWallsResults = structuresScope.CoreWalls.SelectMany(x => EvaluateEnvironmentalProductDeclarationPerObject(x, field).Collapse()).ToList();
            for (int x = 0; x < coreWallsResults.Count; x++)
            {
                coreWallsResults[x] = new GlobalWarmingPotentialResult(coreWallsResults[x].ObjectId, coreWallsResults[x].ResultCase, 0, ObjectScope.Structure, ObjectCategory.Wall, coreWallsResults[x].EnvironmentalProductDeclaration, (coreWallsResults[x] as GlobalWarmingPotentialResult).GlobalWarmingPotential);
            }
            results.AddRange(coreWallsResults);

            //StructuresScope Bracing
            List<LifeCycleAssessmentElementResult> bracingResults = structuresScope.Bracing.SelectMany(x => EvaluateEnvironmentalProductDeclarationPerObject(x, field).Collapse()).ToList();
            for (int x = 0; x < bracingResults.Count; x++)
            {
                bracingResults[x] = new GlobalWarmingPotentialResult(bracingResults[x].ObjectId, bracingResults[x].ResultCase, 0, ObjectScope.Structure, ObjectCategory.Bracing, bracingResults[x].EnvironmentalProductDeclaration, (bracingResults[x] as GlobalWarmingPotentialResult).GlobalWarmingPotential);
            }
            results.AddRange(bracingResults);

            //StructuresScope AdditionalObjects
            List<LifeCycleAssessmentElementResult> additionalObjectsResults = structuresScope.AdditionalObjects.SelectMany(x => EvaluateEnvironmentalProductDeclarationPerObject(x, field).Collapse()).ToList();
            for (int x = 0; x < additionalObjectsResults.Count; x++)
            {
                additionalObjectsResults[x] = new GlobalWarmingPotentialResult(additionalObjectsResults[x].ObjectId, additionalObjectsResults[x].ResultCase, 0, ObjectScope.Structure, ObjectCategory.AdditionalObjects, additionalObjectsResults[x].EnvironmentalProductDeclaration, (additionalObjectsResults[x] as GlobalWarmingPotentialResult).GlobalWarmingPotential);
            }
            results.AddRange(additionalObjectsResults);

            return results;
        }

        /***************************************************/

        [Description("This method calls the appropriate compute method per object within the FoundationsScope and returns results.")]
        [Input("foundationsScope", "This is a life cycle assessment scope object which contains all specified objects along with their associated EPD MaterialFragment data.")]
        [Input("field", "This is the desired field you would like to evaluate. Notice that not all material datasets will contain information for all metrics. A holistic GWP evaluation is currently the only standard evaluation possible with all provided datasets.")]
        [Output("result", "The LifeCycleElementResult that contains foundations scope specific data, total results, and results per element.")]
        public static List<LifeCycleAssessmentElementResult> EvaluateLifeCycleAssessmentScope(this FoundationsScope foundationsScope, EnvironmentalProductDeclarationField field)
        {
            if (foundationsScope.IsValid() == false)
            {
                BH.Engine.Reflection.Compute.RecordWarning("Your FoundationsScope is incomplete, please provide all types of objects where possible.");
            }

            List<LifeCycleAssessmentElementResult> results = new List<LifeCycleAssessmentElementResult>();

            //FoundationsScope Footings
            List<LifeCycleAssessmentElementResult> footingsResults = foundationsScope.Footings.SelectMany(x => EvaluateEnvironmentalProductDeclarationPerObject(x, field).Collapse()).ToList();
            for (int x = 0; x < footingsResults.Count; x++)
            {
                footingsResults[x] = new GlobalWarmingPotentialResult(footingsResults[x].ObjectId, footingsResults[x].ResultCase, footingsResults[x].TimeStep, ObjectScope.Foundation, ObjectCategory.Footing, footingsResults[x].EnvironmentalProductDeclaration, (footingsResults[x] as GlobalWarmingPotentialResult).GlobalWarmingPotential);
            }
            results.AddRange(footingsResults);

            //FoundationsScope Piles
            List<LifeCycleAssessmentElementResult> pilesResults = foundationsScope.Piles.SelectMany(x => EvaluateEnvironmentalProductDeclarationPerObject(x, field).Collapse()).ToList();
            for (int x = 0; x < pilesResults.Count; x++)
            {
                pilesResults[x] = new GlobalWarmingPotentialResult(pilesResults[x].ObjectId, pilesResults[x].ResultCase, pilesResults[x].TimeStep, ObjectScope.Foundation, ObjectCategory.Pile, pilesResults[x].EnvironmentalProductDeclaration, (pilesResults[x] as GlobalWarmingPotentialResult).GlobalWarmingPotential);
            }
            results.AddRange(pilesResults);

            //FoundationsScope Walls
            List<LifeCycleAssessmentElementResult> wallsResults = foundationsScope.Walls.SelectMany(x => EvaluateEnvironmentalProductDeclarationPerObject(x, field).Collapse()).ToList();
            for (int x = 0; x < wallsResults.Count; x++)
            {
                wallsResults[x] = new GlobalWarmingPotentialResult(wallsResults[x].ObjectId, wallsResults[x].ResultCase, wallsResults[x].TimeStep, ObjectScope.Foundation, ObjectCategory.Wall, wallsResults[x].EnvironmentalProductDeclaration, (wallsResults[x] as GlobalWarmingPotentialResult).GlobalWarmingPotential);
            }
            results.AddRange(wallsResults);

            //FoundationsScope Slabs
            List<LifeCycleAssessmentElementResult> fndSlabsResults = foundationsScope.Slabs.SelectMany(x => EvaluateEnvironmentalProductDeclarationPerObject(x, field).Collapse()).ToList();
            for (int x = 0; x < fndSlabsResults.Count; x++)
            {
                fndSlabsResults[x] = new GlobalWarmingPotentialResult(fndSlabsResults[x].ObjectId, fndSlabsResults[x].ResultCase, fndSlabsResults[x].TimeStep, ObjectScope.Foundation, ObjectCategory.Slab, fndSlabsResults[x].EnvironmentalProductDeclaration, (fndSlabsResults[x] as GlobalWarmingPotentialResult).GlobalWarmingPotential);
            }
            results.AddRange(fndSlabsResults);

            //FoundationsScope GradeBeams
            List<LifeCycleAssessmentElementResult> gradeBeamsResults = foundationsScope.GradeBeams.SelectMany(x => EvaluateEnvironmentalProductDeclarationPerObject(x, field).Collapse()).ToList();
            for (int x = 0; x < gradeBeamsResults.Count; x++)
            {
                gradeBeamsResults[x] = new GlobalWarmingPotentialResult(gradeBeamsResults[x].ObjectId, gradeBeamsResults[x].ResultCase, gradeBeamsResults[x].TimeStep, ObjectScope.Foundation, ObjectCategory.GradeBeam, gradeBeamsResults[x].EnvironmentalProductDeclaration, (gradeBeamsResults[x] as GlobalWarmingPotentialResult).GlobalWarmingPotential);
            }
            results.AddRange(gradeBeamsResults);

            //FoundationScope AdditionalObjects
            List<LifeCycleAssessmentElementResult> additionalObjectsResults = foundationsScope.AdditionalObjects.SelectMany(x => EvaluateEnvironmentalProductDeclarationPerObject(x, field).Collapse()).ToList();
            for (int x = 0; x < additionalObjectsResults.Count; x++)
            {
                additionalObjectsResults[x] = new GlobalWarmingPotentialResult(additionalObjectsResults[x].ObjectId, additionalObjectsResults[x].ResultCase, additionalObjectsResults[x].TimeStep, ObjectScope.Foundation, ObjectCategory.AdditionalObjects, additionalObjectsResults[x].EnvironmentalProductDeclaration, (additionalObjectsResults[x] as GlobalWarmingPotentialResult).GlobalWarmingPotential);
            }
            results.AddRange(additionalObjectsResults);

            return results;
        }

        /***************************************************/

        [Description("This method calls the appropriate compute method per object within the EnclosuresScope and returns results.")]
        [Input("enclosuresScope", "This is a life cycle assessment scope object which contains all specified objects along with their associated EPD MaterialFragment data.")]
        [Input("field", "This is the desired field you would like to evaluate. Notice that not all material datasets will contain information for all metrics. A holistic GWP evaluation is currently the only standard evaluation possible with all provided datasets.")]
        [Output("result", "The LifeCycleElementResult that contains enclosures scope specific data, total results, and results per element.")]
        public static List<LifeCycleAssessmentElementResult> EvaluateLifeCycleAssessmentScope(this EnclosuresScope enclosuresScope, EnvironmentalProductDeclarationField field)
        {
            if (enclosuresScope.IsValid() == false)
            {
                BH.Engine.Reflection.Compute.RecordWarning("Your EnclosuresScope is incomplete, please provide all types of objects where possible.");
            }

            List<LifeCycleAssessmentElementResult> results = new List<LifeCycleAssessmentElementResult>();

            //EnclosuresScope Walls
            List<LifeCycleAssessmentElementResult> enclWallsResults = enclosuresScope.Walls.SelectMany(x => EvaluateEnvironmentalProductDeclarationPerObject(x, field).Collapse()).ToList();
            for (int x = 0; x < enclWallsResults.Count; x++)
            {
                enclWallsResults[x] = new GlobalWarmingPotentialResult(enclWallsResults[x].ObjectId, enclWallsResults[x].ResultCase, enclWallsResults[x].TimeStep, ObjectScope.Enclosure, ObjectCategory.Wall, enclWallsResults[x].EnvironmentalProductDeclaration, (enclWallsResults[x] as GlobalWarmingPotentialResult).GlobalWarmingPotential);
            }
            results.AddRange(enclWallsResults);

            //EnclosuresScope CurtainWalls
            List<LifeCycleAssessmentElementResult> curtainWallsResults = enclosuresScope.CurtainWalls.SelectMany(x => EvaluateEnvironmentalProductDeclarationPerObject(x, field).Collapse()).ToList();
            for (int x = 0; x < curtainWallsResults.Count; x++)
            {
                curtainWallsResults[x] = new GlobalWarmingPotentialResult(curtainWallsResults[x].ObjectId, curtainWallsResults[x].ResultCase, curtainWallsResults[x].TimeStep, ObjectScope.Enclosure, ObjectCategory.CurtainWall, curtainWallsResults[x].EnvironmentalProductDeclaration, (curtainWallsResults[x] as GlobalWarmingPotentialResult).GlobalWarmingPotential);
            }
            results.AddRange(curtainWallsResults);

            //EnclosuresScope Windows
            List<LifeCycleAssessmentElementResult> windowsResults = enclosuresScope.Windows.SelectMany(x => EvaluateEnvironmentalProductDeclarationPerObject(x, field).Collapse()).ToList();
            for (int x = 0; x < windowsResults.Count; x++)
            {
                windowsResults[x] = new GlobalWarmingPotentialResult(windowsResults[x].ObjectId, windowsResults[x].ResultCase, windowsResults[x].TimeStep, ObjectScope.Enclosure, ObjectCategory.Window, windowsResults[x].EnvironmentalProductDeclaration, (windowsResults[x] as GlobalWarmingPotentialResult).GlobalWarmingPotential);
            }
            results.AddRange(windowsResults);

            //EnclosuresScope Doors
            List<LifeCycleAssessmentElementResult> doorsResults = enclosuresScope.Doors.SelectMany(x => EvaluateEnvironmentalProductDeclarationPerObject(x, field).Collapse()).ToList();
            for (int x = 0; x < doorsResults.Count; x++)
            {
                doorsResults[x] = new GlobalWarmingPotentialResult(doorsResults[x].ObjectId, doorsResults[x].ResultCase, doorsResults[x].TimeStep, ObjectScope.Enclosure, ObjectCategory.Wall, doorsResults[x].EnvironmentalProductDeclaration, (doorsResults[x] as GlobalWarmingPotentialResult).GlobalWarmingPotential);
            }
            results.AddRange(doorsResults);

            //Enclosures AdditionalObjects
            List<LifeCycleAssessmentElementResult> additionalObjectsResults = enclosuresScope.AdditionalObjects.SelectMany(x => EvaluateEnvironmentalProductDeclarationPerObject(x, field).Collapse()).ToList();
            for (int x = 0; x < additionalObjectsResults.Count; x++)
            {
                additionalObjectsResults[x] = new GlobalWarmingPotentialResult(additionalObjectsResults[x].ObjectId, additionalObjectsResults[x].ResultCase, additionalObjectsResults[x].TimeStep, ObjectScope.Enclosure, ObjectCategory.AdditionalObjects, additionalObjectsResults[x].EnvironmentalProductDeclaration, (additionalObjectsResults[x] as GlobalWarmingPotentialResult).GlobalWarmingPotential);
            }
            results.AddRange(additionalObjectsResults);

            return results;
        }

        /***************************************************/

        [Description("This method calls the appropriate compute method per object within the MEPScope and returns results.")]
        [Input("mepScope", "This is a life cycle assessment scope object which contains all specified objects along with their associated EPD MaterialFragment data.")]
        [Input("field", "This is the desired field you would like to evaluate. Notice that not all material datasets will contain information for all metrics. A holistic GWP evaluation is currently the only standard evaluation possible with all provided datasets.")]
        [Output("result", "The LifeCycleElementResult that contains MEP scope specific data, total results, and results per element.")]
        public static List<LifeCycleAssessmentElementResult> EvaluateLifeCycleAssessmentScope(this MEPScope mepScope, EnvironmentalProductDeclarationField field)
        {
            if (mepScope.IsValid() == false)
            {
                BH.Engine.Reflection.Compute.RecordWarning("Your MEPScope is incomplete, please provide all types of objects where possible.");
            }

            List<LifeCycleAssessmentElementResult> results = new List<LifeCycleAssessmentElementResult>();

            //MechanicalScope

            //MechanicalScope AirTerminals
            List<LifeCycleAssessmentElementResult> airTerminalResults = mepScope.MechanicalScope.AirTerminals.SelectMany(x => EvaluateEnvironmentalProductDeclarationPerObject(x, field).Collapse()).ToList();
            for (int x = 0; x < airTerminalResults.Count; x++)
            {
                if (airTerminalResults[x] == null)
                {
                    BH.Engine.Reflection.Compute.RecordWarning("Mechanical object " + ((IBHoMObject)mepScope.MechanicalScope.AirTerminals[x]).BHoM_Guid + " does not contain a valid IEnvironmentalProductDeclaration MaterialFragment.");
                    continue;
                }
                airTerminalResults[x] = new GlobalWarmingPotentialResult(airTerminalResults[x].ObjectId, airTerminalResults[x].ResultCase, airTerminalResults[x].TimeStep, ObjectScope.Mechanical, ObjectCategory.AirTerminals, airTerminalResults[x].EnvironmentalProductDeclaration, (airTerminalResults[x] as GlobalWarmingPotentialResult).GlobalWarmingPotential);
            }
            results.AddRange(airTerminalResults);

            //MechanicalScope Dampers
            List<LifeCycleAssessmentElementResult> dampterResults = mepScope.MechanicalScope.Dampers.SelectMany(x => EvaluateEnvironmentalProductDeclarationPerObject(x, field).Collapse()).ToList();
            for (int x = 0; x < dampterResults.Count; x++)
            {
                if (dampterResults[x] == null)
                {
                    BH.Engine.Reflection.Compute.RecordWarning("Mechanical object " + ((IBHoMObject)mepScope.MechanicalScope.Dampers[x]).BHoM_Guid + " does not contain a valid IEnvironmentalProductDeclaration MaterialFragment.");
                    continue;
                }
                dampterResults[x] = new GlobalWarmingPotentialResult(dampterResults[x].ObjectId, dampterResults[x].ResultCase, dampterResults[x].TimeStep, ObjectScope.Mechanical, ObjectCategory.Dampers, dampterResults[x].EnvironmentalProductDeclaration, (dampterResults[x] as GlobalWarmingPotentialResult).GlobalWarmingPotential);
            }
            results.AddRange(dampterResults);

            //MechanicalScope Ducts
            List<LifeCycleAssessmentElementResult> ductResults = mepScope.MechanicalScope.Ducts.SelectMany(x => EvaluateEnvironmentalProductDeclarationPerObject(x, field).Collapse()).ToList();
            for (int x = 0; x < ductResults.Count; x++)
            {
                if (ductResults[x] == null)
                {
                    BH.Engine.Reflection.Compute.RecordWarning("Mechanical object " + ((IBHoMObject)mepScope.MechanicalScope.Ducts[x]).BHoM_Guid + " does not contain a valid IEnvironmentalProductDeclaration MaterialFragment.");
                    continue;
                }
                ductResults[x] = new GlobalWarmingPotentialResult(ductResults[x].ObjectId, ductResults[x].ResultCase, ductResults[x].TimeStep, ObjectScope.Mechanical, ObjectCategory.Ducts, ductResults[x].EnvironmentalProductDeclaration, (ductResults[x] as GlobalWarmingPotentialResult).GlobalWarmingPotential);
            }
            results.AddRange(ductResults);

            //MechanicalScope Equipment
            List<LifeCycleAssessmentElementResult> mechanicalEquipmentResults = mepScope.MechanicalScope.Equipment.SelectMany(x => EvaluateEnvironmentalProductDeclarationPerObject(x, field).Collapse()).ToList();
            for (int x = 0; x < mechanicalEquipmentResults.Count; x++)
            {
                if (mechanicalEquipmentResults[x] == null)
                {
                    BH.Engine.Reflection.Compute.RecordWarning("Mechanical object " + ((IBHoMObject)mepScope.MechanicalScope.Equipment[x]).BHoM_Guid + " does not contain a valid IEnvironmentalProductDeclaration MaterialFragment.");
                    continue;
                }
                mechanicalEquipmentResults[x] = new GlobalWarmingPotentialResult(mechanicalEquipmentResults[x].ObjectId, mechanicalEquipmentResults[x].ResultCase, mechanicalEquipmentResults[x].TimeStep, ObjectScope.Mechanical, ObjectCategory.Equipment, mechanicalEquipmentResults[x].EnvironmentalProductDeclaration, (mechanicalEquipmentResults[x] as GlobalWarmingPotentialResult).GlobalWarmingPotential);
            }
            results.AddRange(mechanicalEquipmentResults);

            //MechanicalScope Pipes
            List<LifeCycleAssessmentElementResult> pipingResults = mepScope.MechanicalScope.Pipes.SelectMany(x => EvaluateEnvironmentalProductDeclarationPerObject(x, field).Collapse()).ToList();
            for (int x = 0; x < pipingResults.Count; x++)
            {
                if (pipingResults[x] == null)
                {
                    BH.Engine.Reflection.Compute.RecordWarning("Mechanical object " + ((IBHoMObject)mepScope.MechanicalScope.Pipes[x]).BHoM_Guid + " does not contain a valid IEnvironmentalProductDeclaration MaterialFragment.");
                    continue;
                }
                pipingResults[x] = new GlobalWarmingPotentialResult(pipingResults[x].ObjectId, pipingResults[x].ResultCase, pipingResults[x].TimeStep, ObjectScope.Mechanical, ObjectCategory.Pipes, pipingResults[x].EnvironmentalProductDeclaration, (pipingResults[x] as GlobalWarmingPotentialResult).GlobalWarmingPotential);
            }
            results.AddRange(pipingResults);

            //MechanicalScope Refrigerants
            List<LifeCycleAssessmentElementResult> refrigerantsResults = mepScope.MechanicalScope.Refrigerants.SelectMany(x => EvaluateEnvironmentalProductDeclarationPerObject(x, field).Collapse()).ToList();
            for (int x = 0; x < refrigerantsResults.Count; x++)
            {
                if (refrigerantsResults[x] == null)
                {
                    BH.Engine.Reflection.Compute.RecordWarning("Mechanical object " + ((IBHoMObject)mepScope.MechanicalScope.Refrigerants[x]).BHoM_Guid + " does not contain a valid IEnvironmentalProductDeclaration MaterialFragment.");
                    continue;
                }
                refrigerantsResults[x] = new GlobalWarmingPotentialResult(refrigerantsResults[x].ObjectId, refrigerantsResults[x].ResultCase, refrigerantsResults[x].TimeStep, ObjectScope.Mechanical, ObjectCategory.Refrigerants, refrigerantsResults[x].EnvironmentalProductDeclaration, (refrigerantsResults[x] as GlobalWarmingPotentialResult).GlobalWarmingPotential);
            }
            results.AddRange(refrigerantsResults);

            //MechanicalScope Tanks
            List<LifeCycleAssessmentElementResult> tankResults = mepScope.MechanicalScope.Tanks.SelectMany(x => EvaluateEnvironmentalProductDeclarationPerObject(x, field).Collapse()).ToList();
            for (int x = 0; x < tankResults.Count; x++)
            {
                if (tankResults[x] == null)
                {
                    BH.Engine.Reflection.Compute.RecordWarning("Mechanical object " + ((IBHoMObject)mepScope.MechanicalScope.Tanks[x]).BHoM_Guid + " does not contain a valid IEnvironmentalProductDeclaration MaterialFragment.");
                    continue;
                }
                tankResults[x] = new GlobalWarmingPotentialResult(tankResults[x].ObjectId, tankResults[x].ResultCase, tankResults[x].TimeStep, ObjectScope.Mechanical, ObjectCategory.Tanks, tankResults[x].EnvironmentalProductDeclaration, (tankResults[x] as GlobalWarmingPotentialResult).GlobalWarmingPotential);
            }
            results.AddRange(tankResults);

            //MechanicalScope Valves
            List<LifeCycleAssessmentElementResult> valveResults = mepScope.MechanicalScope.Valves.SelectMany(x => EvaluateEnvironmentalProductDeclarationPerObject(x, field).Collapse()).ToList();
            for (int x = 0; x < valveResults.Count; x++)
            {
                if (valveResults[x] == null)
                {
                    BH.Engine.Reflection.Compute.RecordWarning("Mechanical object " + ((IBHoMObject)mepScope.MechanicalScope.Valves[x]).BHoM_Guid + " does not contain a valid IEnvironmentalProductDeclaration MaterialFragment.");
                    continue;
                }
                valveResults[x] = new GlobalWarmingPotentialResult(valveResults[x].ObjectId, valveResults[x].ResultCase, valveResults[x].TimeStep, ObjectScope.Mechanical, ObjectCategory.Valves, valveResults[x].EnvironmentalProductDeclaration, (valveResults[x] as GlobalWarmingPotentialResult).GlobalWarmingPotential);
            }
            results.AddRange(valveResults);

            //MechanicalScope AdditionalObjects
            List<LifeCycleAssessmentElementResult> additionalMechanicalObjectsResults = mepScope.MechanicalScope.AdditionalObjects.SelectMany(x => EvaluateEnvironmentalProductDeclarationPerObject(x, field).Collapse()).ToList();
            for (int x = 0; x < additionalMechanicalObjectsResults.Count; x++)
            {
                if (additionalMechanicalObjectsResults[x] == null)
                {
                    BH.Engine.Reflection.Compute.RecordWarning("Additional Mechanical object " + ((IBHoMObject)mepScope.MechanicalScope.AdditionalObjects[x]).BHoM_Guid + " within MechanicalScope does not contain a valid IEnvironmentalProductDeclaration MaterialFragment.");
                    continue;
                }
                additionalMechanicalObjectsResults[x] = new GlobalWarmingPotentialResult(additionalMechanicalObjectsResults[x].ObjectId, additionalMechanicalObjectsResults[x].ResultCase, additionalMechanicalObjectsResults[x].TimeStep, ObjectScope.Mechanical, ObjectCategory.AdditionalObjects, additionalMechanicalObjectsResults[x].EnvironmentalProductDeclaration, (additionalMechanicalObjectsResults[x] as GlobalWarmingPotentialResult).GlobalWarmingPotential);
            }
            results.AddRange(additionalMechanicalObjectsResults);

            //ElectricalScope

            //ElectricalScope Batteries
            List<LifeCycleAssessmentElementResult> batteriesResults = mepScope.ElectricalScope.Batteries.SelectMany(x => EvaluateEnvironmentalProductDeclarationPerObject(x, field).Collapse()).ToList();
            for (int x = 0; x < batteriesResults.Count; x++)
            {
                if (batteriesResults[x] == null)
                {
                    BH.Engine.Reflection.Compute.RecordWarning("Electrical object " + ((IBHoMObject)mepScope.ElectricalScope.Batteries[x]).BHoM_Guid + " does not contain a valid IEnvironmentalProductDeclaration MaterialFragment.");
                    continue;
                }
                batteriesResults[x] = new GlobalWarmingPotentialResult(batteriesResults[x].ObjectId, batteriesResults[x].ResultCase, batteriesResults[x].TimeStep, ObjectScope.Electrical, ObjectCategory.Battery, batteriesResults[x].EnvironmentalProductDeclaration, (batteriesResults[x] as GlobalWarmingPotentialResult).GlobalWarmingPotential);
            }
            results.AddRange(batteriesResults);

            //ElectricalScope CableTray
            List<LifeCycleAssessmentElementResult> cableTrayResults = mepScope.ElectricalScope.CableTrays.SelectMany(x => EvaluateEnvironmentalProductDeclarationPerObject(x, field).Collapse()).ToList();
            for (int x = 0; x < cableTrayResults.Count; x++)
            {
                if (cableTrayResults[x] == null)
                {
                    BH.Engine.Reflection.Compute.RecordWarning("Electrical object " + ((IBHoMObject)mepScope.ElectricalScope.CableTrays[x]).BHoM_Guid + " does not contain a valid IEnvironmentalProductDeclaration MaterialFragment.");
                    continue;
                }
                cableTrayResults[x] = new GlobalWarmingPotentialResult(cableTrayResults[x].ObjectId, cableTrayResults[x].ResultCase, cableTrayResults[x].TimeStep, ObjectScope.Electrical, ObjectCategory.CableTray, cableTrayResults[x].EnvironmentalProductDeclaration, (cableTrayResults[x] as GlobalWarmingPotentialResult).GlobalWarmingPotential);
            }
            results.AddRange(cableTrayResults);

            //ElectricalScope Conduit
            List<LifeCycleAssessmentElementResult> conduitResults = mepScope.ElectricalScope.Conduit.SelectMany(x => EvaluateEnvironmentalProductDeclarationPerObject(x, field).Collapse()).ToList();
            for (int x = 0; x < conduitResults.Count; x++)
            {
                if (conduitResults[x] == null)
                {
                    BH.Engine.Reflection.Compute.RecordWarning("Electrical object " + ((IBHoMObject)mepScope.ElectricalScope.Conduit[x]).BHoM_Guid + " does not contain a valid IEnvironmentalProductDeclaration MaterialFragment.");
                    continue;
                }
                conduitResults[x] = new GlobalWarmingPotentialResult(conduitResults[x].ObjectId, conduitResults[x].ResultCase, conduitResults[x].TimeStep, ObjectScope.Electrical, ObjectCategory.Conduit, conduitResults[x].EnvironmentalProductDeclaration, (conduitResults[x] as GlobalWarmingPotentialResult).GlobalWarmingPotential);
            }
            results.AddRange(conduitResults);

            //ElectricalScope Equipment
            List<LifeCycleAssessmentElementResult> electricalEquipmentResults = mepScope.ElectricalScope.Equipment.SelectMany(x => EvaluateEnvironmentalProductDeclarationPerObject(x, field).Collapse()).ToList();
            for (int x = 0; x < electricalEquipmentResults.Count; x++)
            {
                if (conduitResults[x] == null)
                {
                    BH.Engine.Reflection.Compute.RecordWarning("Electrical object " + ((IBHoMObject)mepScope.ElectricalScope.Equipment[x]).BHoM_Guid + " does not contain a valid IEnvironmentalProductDeclaration MaterialFragment.");
                    continue;
                }
                electricalEquipmentResults[x] = new GlobalWarmingPotentialResult(electricalEquipmentResults[x].ObjectId, electricalEquipmentResults[x].ResultCase, electricalEquipmentResults[x].TimeStep, ObjectScope.Electrical, ObjectCategory.Equipment, electricalEquipmentResults[x].EnvironmentalProductDeclaration, (electricalEquipmentResults[x] as GlobalWarmingPotentialResult).GlobalWarmingPotential);
            }
            results.AddRange(electricalEquipmentResults);

            //ElectricalScope FireAlarmDevices
            List<LifeCycleAssessmentElementResult> fireAlarmDeviceResults = mepScope.ElectricalScope.FireAlarmDevices.SelectMany(x => EvaluateEnvironmentalProductDeclarationPerObject(x, field).Collapse()).ToList();
            for (int x = 0; x < fireAlarmDeviceResults.Count; x++)
            {
                if (fireAlarmDeviceResults[x] == null)
                {
                    BH.Engine.Reflection.Compute.RecordWarning("Electrical object " + ((IBHoMObject)mepScope.ElectricalScope.Equipment[x]).BHoM_Guid + " does not contain a valid IEnvironmentalProductDeclaration MaterialFragment.");
                    continue;
                }
                fireAlarmDeviceResults[x] = new GlobalWarmingPotentialResult(fireAlarmDeviceResults[x].ObjectId, fireAlarmDeviceResults[x].ResultCase, fireAlarmDeviceResults[x].TimeStep, ObjectScope.Electrical, ObjectCategory.FireAlarmDevices, fireAlarmDeviceResults[x].EnvironmentalProductDeclaration, (fireAlarmDeviceResults[x] as GlobalWarmingPotentialResult).GlobalWarmingPotential);
            }
            results.AddRange(fireAlarmDeviceResults);

            //ElectricalScope Generator
            List<LifeCycleAssessmentElementResult> generatorResults = mepScope.ElectricalScope.Generators.SelectMany(x => EvaluateEnvironmentalProductDeclarationPerObject(x, field).Collapse()).ToList();
            for (int x = 0; x < generatorResults.Count; x++)
            {
                if (generatorResults[x] == null)
                {
                    BH.Engine.Reflection.Compute.RecordWarning("Electrical object " + ((IBHoMObject)mepScope.ElectricalScope.Generators[x]).BHoM_Guid + " does not contain a valid IEnvironmentalProductDeclaration MaterialFragment.");
                    continue;
                }
                generatorResults[x] = new GlobalWarmingPotentialResult(generatorResults[x].ObjectId, generatorResults[x].ResultCase, generatorResults[x].TimeStep, ObjectScope.Electrical, ObjectCategory.Generator, generatorResults[x].EnvironmentalProductDeclaration, (generatorResults[x] as GlobalWarmingPotentialResult).GlobalWarmingPotential);
            }
            results.AddRange(generatorResults);

            //ElectricalScope InformationCommunicationDevices
            List<LifeCycleAssessmentElementResult> infoCommsResults = mepScope.ElectricalScope.InformationCommunicationDevices.SelectMany(x => EvaluateEnvironmentalProductDeclarationPerObject(x, field).Collapse()).ToList();
            for (int x = 0; x < infoCommsResults.Count; x++)
            {
                if (infoCommsResults[x] == null)
                {
                    BH.Engine.Reflection.Compute.RecordWarning("Electrical object " + ((IBHoMObject)mepScope.ElectricalScope.InformationCommunicationDevices[x]).BHoM_Guid + " does not contain a valid IEnvironmentalProductDeclaration MaterialFragment.");
                    continue;
                }
                infoCommsResults[x] = new GlobalWarmingPotentialResult(infoCommsResults[x].ObjectId, infoCommsResults[x].ResultCase, infoCommsResults[x].TimeStep, ObjectScope.Electrical, ObjectCategory.InformationCommunicationDevices, infoCommsResults[x].EnvironmentalProductDeclaration, (infoCommsResults[x] as GlobalWarmingPotentialResult).GlobalWarmingPotential);
            }
            results.AddRange(infoCommsResults);

            //ElectricalScope Light Fixtures
            List<LifeCycleAssessmentElementResult> lightFixtureResults = mepScope.ElectricalScope.LightFixtures.SelectMany(x => EvaluateEnvironmentalProductDeclarationPerObject(x, field).Collapse()).ToList();
            for (int x = 0; x < lightFixtureResults.Count; x++)
            {
                if (lightFixtureResults[x] == null)
                {
                    BH.Engine.Reflection.Compute.RecordWarning("Electrical object" + ((IBHoMObject)mepScope.ElectricalScope.LightFixtures[x]).BHoM_Guid + " does not contain a valid IEnvironmentalProductDeclaration MaterialFragment.");
                    continue;
                }
                lightFixtureResults[x] = new GlobalWarmingPotentialResult(lightFixtureResults[x].ObjectId, lightFixtureResults[x].ResultCase, lightFixtureResults[x].TimeStep, ObjectScope.Electrical, ObjectCategory.LightFixtures, lightFixtureResults[x].EnvironmentalProductDeclaration, (lightFixtureResults[x] as GlobalWarmingPotentialResult).GlobalWarmingPotential);
            }
            results.AddRange(lightFixtureResults);

            //ElectricalScope Lighting Controls
            List<LifeCycleAssessmentElementResult> lightControlResults = mepScope.ElectricalScope.LightingControls.SelectMany(x => EvaluateEnvironmentalProductDeclarationPerObject(x, field).Collapse()).ToList();
            for (int x = 0; x < lightControlResults.Count; x++)
            {
                if (lightControlResults[x] == null)
                {
                    BH.Engine.Reflection.Compute.RecordWarning("Electrical object" + ((IBHoMObject)mepScope.ElectricalScope.LightingControls[x]).BHoM_Guid + " does not contain a valid IEnvironmentalProductDeclaration MaterialFragment.");
                    continue;
                }
                lightControlResults[x] = new GlobalWarmingPotentialResult(lightControlResults[x].ObjectId, lightControlResults[x].ResultCase, lightControlResults[x].TimeStep, ObjectScope.Electrical, ObjectCategory.LightingControls, lightControlResults[x].EnvironmentalProductDeclaration, (lightControlResults[x] as GlobalWarmingPotentialResult).GlobalWarmingPotential);
            }
            results.AddRange(lightControlResults);

            //ElectricalScope Meters
            List<LifeCycleAssessmentElementResult> meterResults = mepScope.ElectricalScope.Meters.SelectMany(x => EvaluateEnvironmentalProductDeclarationPerObject(x, field).Collapse()).ToList();
            for (int x = 0; x < meterResults.Count; x++)
            {
                if (meterResults[x] == null)
                {
                    BH.Engine.Reflection.Compute.RecordWarning("Electrical object" + ((IBHoMObject)mepScope.ElectricalScope.Meters[x]).BHoM_Guid + " does not contain a valid IEnvironmentalProductDeclaration MaterialFragment.");
                    continue;
                }
                meterResults[x] = new GlobalWarmingPotentialResult(meterResults[x].ObjectId, meterResults[x].ResultCase, meterResults[x].TimeStep, ObjectScope.Electrical, ObjectCategory.Meters, meterResults[x].EnvironmentalProductDeclaration, (meterResults[x] as GlobalWarmingPotentialResult).GlobalWarmingPotential);
            }
            results.AddRange(meterResults);

            //ElectricalScope Security Devices
            List<LifeCycleAssessmentElementResult> securityResults = mepScope.ElectricalScope.SecurityDevices.SelectMany(x => EvaluateEnvironmentalProductDeclarationPerObject(x, field).Collapse()).ToList();
            for (int x = 0; x < securityResults.Count; x++)
            {
                if (securityResults[x] == null)
                {
                    BH.Engine.Reflection.Compute.RecordWarning("Electrical object" + ((IBHoMObject)mepScope.ElectricalScope.SecurityDevices[x]).BHoM_Guid + " does not contain a valid IEnvironmentalProductDeclaration MaterialFragment.");
                    continue;
                }
                securityResults[x] = new GlobalWarmingPotentialResult(securityResults[x].ObjectId, securityResults[x].ResultCase, securityResults[x].TimeStep, ObjectScope.Electrical, ObjectCategory.SecurityDevices, securityResults[x].EnvironmentalProductDeclaration, (securityResults[x] as GlobalWarmingPotentialResult).GlobalWarmingPotential);
            }
            results.AddRange(securityResults);

            //ElectricalScope Sockets
            List<LifeCycleAssessmentElementResult> socketResults = mepScope.ElectricalScope.Sockets.SelectMany(x => EvaluateEnvironmentalProductDeclarationPerObject(x, field).Collapse()).ToList();
            for (int x = 0; x < socketResults.Count; x++)
            {
                if (socketResults[x] == null)
                {
                    BH.Engine.Reflection.Compute.RecordWarning("Electrical object" + ((IBHoMObject)mepScope.ElectricalScope.Sockets[x]).BHoM_Guid + " does not contain a valid IEnvironmentalProductDeclaration MaterialFragment.");
                    continue;
                }
                socketResults[x] = new GlobalWarmingPotentialResult(socketResults[x].ObjectId, socketResults[x].ResultCase, socketResults[x].TimeStep, ObjectScope.Electrical, ObjectCategory.Sockets, socketResults[x].EnvironmentalProductDeclaration, (socketResults[x] as GlobalWarmingPotentialResult).GlobalWarmingPotential);
            }
            results.AddRange(socketResults);

            //ElectricalScope Solar Panels
            List<LifeCycleAssessmentElementResult> solarPanelResults = mepScope.ElectricalScope.SolarPanels.SelectMany(x => EvaluateEnvironmentalProductDeclarationPerObject(x, field).Collapse()).ToList();
            for (int x = 0; x < solarPanelResults.Count; x++)
            {
                if (solarPanelResults[x] == null)
                {
                    BH.Engine.Reflection.Compute.RecordWarning("Electrical object" + ((IBHoMObject)mepScope.ElectricalScope.SolarPanels[x]).BHoM_Guid + " does not contain a valid IEnvironmentalProductDeclaration MaterialFragment.");
                    continue;
                }
                solarPanelResults[x] = new GlobalWarmingPotentialResult(solarPanelResults[x].ObjectId, solarPanelResults[x].ResultCase, solarPanelResults[x].TimeStep, ObjectScope.Electrical, ObjectCategory.SolarPanels, solarPanelResults[x].EnvironmentalProductDeclaration, (solarPanelResults[x] as GlobalWarmingPotentialResult).GlobalWarmingPotential);
            }
            results.AddRange(solarPanelResults);

            //ElectricalScope Wiring
            List<LifeCycleAssessmentElementResult> wiringResults = mepScope.ElectricalScope.WireSegments.SelectMany(x => EvaluateEnvironmentalProductDeclarationPerObject(x, field).Collapse()).ToList();
            for (int x = 0; x < wiringResults.Count; x++)
            {
                if (wiringResults[x] == null)
                {
                    BH.Engine.Reflection.Compute.RecordWarning("Electrical object " + ((IBHoMObject)mepScope.ElectricalScope.WireSegments[x]).BHoM_Guid + " does not contain a valid IEnvironmentalProductDeclaration MaterialFragment.");
                    continue;
                }
                wiringResults[x] = new GlobalWarmingPotentialResult(wiringResults[x].ObjectId, wiringResults[x].ResultCase, wiringResults[x].TimeStep, ObjectScope.Electrical, ObjectCategory.Wiring, wiringResults[x].EnvironmentalProductDeclaration, (wiringResults[x] as GlobalWarmingPotentialResult).GlobalWarmingPotential);
            }
            results.AddRange(wiringResults);

            //ElectricalScope AdditionalObjects
            List<LifeCycleAssessmentElementResult> additionalElectricalObjectsResults = mepScope.ElectricalScope.AdditionalObjects.SelectMany(x => EvaluateEnvironmentalProductDeclarationPerObject(x, field).Collapse()).ToList();
            for (int x = 0; x < additionalElectricalObjectsResults.Count; x++)
            {
                if (additionalElectricalObjectsResults[x] == null)
                {
                    BH.Engine.Reflection.Compute.RecordWarning("Electrical object " + ((IBHoMObject)mepScope.ElectricalScope.AdditionalObjects[x]).BHoM_Guid + " does not contain a valid IEnvironmentalProductDeclaration MaterialFragment.");
                    continue;
                }
                additionalElectricalObjectsResults[x] = new GlobalWarmingPotentialResult(additionalElectricalObjectsResults[x].ObjectId, additionalElectricalObjectsResults[x].ResultCase, additionalElectricalObjectsResults[x].TimeStep, ObjectScope.Electrical, ObjectCategory.AdditionalObjects, additionalElectricalObjectsResults[x].EnvironmentalProductDeclaration, (additionalElectricalObjectsResults[x] as GlobalWarmingPotentialResult).GlobalWarmingPotential);
            }
            results.AddRange(additionalElectricalObjectsResults);

            //PlumbingScope

            //PlumbingScope Equipment
            List<LifeCycleAssessmentElementResult> plumbingEquipmentResults = mepScope.PlumbingScope.Equipment.SelectMany(x => EvaluateEnvironmentalProductDeclarationPerObject(x, field).Collapse()).ToList();
            for (int x = 0; x < plumbingEquipmentResults.Count; x++)
            {
                if (plumbingEquipmentResults[x] == null)
                {
                    BH.Engine.Reflection.Compute.RecordWarning("Plumbing object " + ((IBHoMObject)mepScope.PlumbingScope.Equipment[x]).BHoM_Guid + " does not contain a valid IEnvironmentalProductDeclaration MaterialFragment.");
                    continue;
                }
                plumbingEquipmentResults[x] = new GlobalWarmingPotentialResult(plumbingEquipmentResults[x].ObjectId, plumbingEquipmentResults[x].ResultCase, plumbingEquipmentResults[x].TimeStep, ObjectScope.Plumbing, ObjectCategory.Equipment, plumbingEquipmentResults[x].EnvironmentalProductDeclaration, (plumbingEquipmentResults[x] as GlobalWarmingPotentialResult).GlobalWarmingPotential);
            }
            results.AddRange(plumbingEquipmentResults);

            //PlumbingScope Pipes
            List<LifeCycleAssessmentElementResult> plumbingPipingResults = mepScope.PlumbingScope.Pipes.SelectMany(x => EvaluateEnvironmentalProductDeclarationPerObject(x, field).Collapse()).ToList();
            for (int x = 0; x < plumbingPipingResults.Count; x++)
            {
                if (plumbingPipingResults[x] == null)
                {
                    BH.Engine.Reflection.Compute.RecordWarning("Plumbing object " + ((IBHoMObject)mepScope.PlumbingScope.Pipes[x]).BHoM_Guid + " does not contain a valid IEnvironmentalProductDeclaration MaterialFragment.");
                    continue;
                }
                plumbingPipingResults[x] = new GlobalWarmingPotentialResult(plumbingPipingResults[x].ObjectId, plumbingPipingResults[x].ResultCase, plumbingPipingResults[x].TimeStep, ObjectScope.Plumbing, ObjectCategory.Pipes, plumbingPipingResults[x].EnvironmentalProductDeclaration, (plumbingPipingResults[x] as GlobalWarmingPotentialResult).GlobalWarmingPotential);
            }
            results.AddRange(plumbingPipingResults);

            //PlumbingScope PlumbingFixtures
            List<LifeCycleAssessmentElementResult> plumbingFixtureResults = mepScope.PlumbingScope.PlumbingFixtures.SelectMany(x => EvaluateEnvironmentalProductDeclarationPerObject(x, field).Collapse()).ToList();
            for (int x = 0; x < plumbingFixtureResults.Count; x++)
            {
                if (plumbingFixtureResults[x] == null)
                {
                    BH.Engine.Reflection.Compute.RecordWarning("Plumbing object " + ((IBHoMObject)mepScope.PlumbingScope.PlumbingFixtures[x]).BHoM_Guid + " does not contain a valid IEnvironmentalProductDeclaration MaterialFragment.");
                    continue;
                }
                plumbingFixtureResults[x] = new GlobalWarmingPotentialResult(plumbingFixtureResults[x].ObjectId, plumbingFixtureResults[x].ResultCase, plumbingFixtureResults[x].TimeStep, ObjectScope.Plumbing, ObjectCategory.PlumbingFixtures, plumbingFixtureResults[x].EnvironmentalProductDeclaration, (plumbingFixtureResults[x] as GlobalWarmingPotentialResult).GlobalWarmingPotential);
            }
            results.AddRange(plumbingFixtureResults);

            //PlumbingScope Tanks
            List<LifeCycleAssessmentElementResult> plumbingTanksResults = mepScope.PlumbingScope.Tanks.SelectMany(x => EvaluateEnvironmentalProductDeclarationPerObject(x, field).Collapse()).ToList();
            for (int x = 0; x < plumbingTanksResults.Count; x++)
            {
                if (plumbingTanksResults[x] == null)
                {
                    BH.Engine.Reflection.Compute.RecordWarning("Plumbing object " + ((IBHoMObject)mepScope.PlumbingScope.Tanks[x]).BHoM_Guid + " does not contain a valid IEnvironmentalProductDeclaration MaterialFragment.");
                    continue;
                }
                plumbingTanksResults[x] = new GlobalWarmingPotentialResult(plumbingTanksResults[x].ObjectId, plumbingTanksResults[x].ResultCase, plumbingTanksResults[x].TimeStep, ObjectScope.Plumbing, ObjectCategory.Tanks, plumbingTanksResults[x].EnvironmentalProductDeclaration, (plumbingTanksResults[x] as GlobalWarmingPotentialResult).GlobalWarmingPotential);
            }
            results.AddRange(plumbingTanksResults);

            //PlumbingScope Valves
            List<LifeCycleAssessmentElementResult> plumbingValvesResults = mepScope.PlumbingScope.Valves.SelectMany(x => EvaluateEnvironmentalProductDeclarationPerObject(x, field).Collapse()).ToList();
            for (int x = 0; x < plumbingValvesResults.Count; x++)
            {
                if (plumbingValvesResults[x] == null)
                {
                    BH.Engine.Reflection.Compute.RecordWarning("Plumbing object " + ((IBHoMObject)mepScope.PlumbingScope.Valves[x]).BHoM_Guid + " does not contain a valid IEnvironmentalProductDeclaration MaterialFragment.");
                    continue;
                }
                plumbingValvesResults[x] = new GlobalWarmingPotentialResult(plumbingValvesResults[x].ObjectId, plumbingValvesResults[x].ResultCase, plumbingValvesResults[x].TimeStep, ObjectScope.Plumbing, ObjectCategory.Valves, plumbingValvesResults[x].EnvironmentalProductDeclaration, (plumbingValvesResults[x] as GlobalWarmingPotentialResult).GlobalWarmingPotential);
            }
            results.AddRange(plumbingValvesResults);

            //PlumbingScope AdditionalObjects
            List<LifeCycleAssessmentElementResult> additionalPlumbingObjectsResults = mepScope.PlumbingScope.AdditionalObjects.SelectMany(x => EvaluateEnvironmentalProductDeclarationPerObject(x, field).Collapse()).ToList();
            for (int x = 0; x < additionalPlumbingObjectsResults.Count; x++)
            {
                if (additionalPlumbingObjectsResults[x] == null)
                {
                    BH.Engine.Reflection.Compute.RecordWarning("Plumbing object " + ((IBHoMObject)mepScope.PlumbingScope.AdditionalObjects[x]).BHoM_Guid + " does not contain a valid IEnvironmentalProductDeclaration MaterialFragment.");
                    continue;
                }
                additionalPlumbingObjectsResults[x] = new GlobalWarmingPotentialResult(additionalPlumbingObjectsResults[x].ObjectId, additionalPlumbingObjectsResults[x].ResultCase, additionalPlumbingObjectsResults[x].TimeStep, ObjectScope.Plumbing, ObjectCategory.AdditionalObjects, additionalPlumbingObjectsResults[x].EnvironmentalProductDeclaration, (additionalPlumbingObjectsResults[x] as GlobalWarmingPotentialResult).GlobalWarmingPotential);
            }
            results.AddRange(additionalPlumbingObjectsResults);

            //FireProtectionScope

            //FireProtectionScope Equipment
            List<LifeCycleAssessmentElementResult> fireProtectionEquipmentResults = mepScope.FireProtectionScope.Equipment.SelectMany(x => EvaluateEnvironmentalProductDeclarationPerObject(x, field).Collapse()).ToList();
            for (int x = 0; x < fireProtectionEquipmentResults.Count; x++)
            {
                if (fireProtectionEquipmentResults[x] == null)
                {
                    BH.Engine.Reflection.Compute.RecordWarning("Fire Protection object " + ((IBHoMObject)mepScope.FireProtectionScope.Equipment[x]).BHoM_Guid + " does not contain a valid IEnvironmentalProductDeclaration MaterialFragment.");
                    continue;
                }
                fireProtectionEquipmentResults[x] = new GlobalWarmingPotentialResult(fireProtectionEquipmentResults[x].ObjectId, fireProtectionEquipmentResults[x].ResultCase, fireProtectionEquipmentResults[x].TimeStep, ObjectScope.FireProtection, ObjectCategory.Equipment, fireProtectionEquipmentResults[x].EnvironmentalProductDeclaration, (fireProtectionEquipmentResults[x] as GlobalWarmingPotentialResult).GlobalWarmingPotential);
            }
            results.AddRange(fireProtectionEquipmentResults);

            //FireProtectionScope Pipes
            List<LifeCycleAssessmentElementResult> fireProtectionPipesResults = mepScope.FireProtectionScope.Pipes.SelectMany(x => EvaluateEnvironmentalProductDeclarationPerObject(x, field).Collapse()).ToList();
            for (int x = 0; x < fireProtectionPipesResults.Count; x++)
            {
                if (fireProtectionPipesResults[x] == null)
                {
                    BH.Engine.Reflection.Compute.RecordWarning("Fire Protection object " + ((IBHoMObject)mepScope.FireProtectionScope.Pipes[x]).BHoM_Guid + " does not contain a valid IEnvironmentalProductDeclaration MaterialFragment.");
                    continue;
                }
                fireProtectionPipesResults[x] = new GlobalWarmingPotentialResult(fireProtectionPipesResults[x].ObjectId, fireProtectionPipesResults[x].ResultCase, fireProtectionPipesResults[x].TimeStep, ObjectScope.FireProtection, ObjectCategory.Pipes, fireProtectionPipesResults[x].EnvironmentalProductDeclaration, (fireProtectionPipesResults[x] as GlobalWarmingPotentialResult).GlobalWarmingPotential);
            }
            results.AddRange(fireProtectionPipesResults);

            //FireProtectionScope Sprinklers
            List<LifeCycleAssessmentElementResult> fireProtectionSprinklersResults = mepScope.FireProtectionScope.Sprinklers.SelectMany(x => EvaluateEnvironmentalProductDeclarationPerObject(x, field).Collapse()).ToList();
            for (int x = 0; x < fireProtectionSprinklersResults.Count; x++)
            {
                if (fireProtectionSprinklersResults[x] == null)
                {
                    BH.Engine.Reflection.Compute.RecordWarning("Fire Protection object " + ((IBHoMObject)mepScope.FireProtectionScope.Sprinklers[x]).BHoM_Guid + " does not contain a valid IEnvironmentalProductDeclaration MaterialFragment.");
                    continue;
                }
                fireProtectionSprinklersResults[x] = new GlobalWarmingPotentialResult(fireProtectionSprinklersResults[x].ObjectId, fireProtectionSprinklersResults[x].ResultCase, fireProtectionSprinklersResults[x].TimeStep, ObjectScope.FireProtection, ObjectCategory.Sprinklers, fireProtectionSprinklersResults[x].EnvironmentalProductDeclaration, (fireProtectionSprinklersResults[x] as GlobalWarmingPotentialResult).GlobalWarmingPotential);
            }
            results.AddRange(fireProtectionSprinklersResults);

            //FireProtectionScope Tanks
            List<LifeCycleAssessmentElementResult> fireProtectionTankResults = mepScope.FireProtectionScope.Tanks.SelectMany(x => EvaluateEnvironmentalProductDeclarationPerObject(x, field).Collapse()).ToList();
            for (int x = 0; x < fireProtectionTankResults.Count; x++)
            {
                if (fireProtectionTankResults[x] == null)
                {
                    BH.Engine.Reflection.Compute.RecordWarning("Fire Protection object " + ((IBHoMObject)mepScope.FireProtectionScope.Tanks[x]).BHoM_Guid + " does not contain a valid IEnvironmentalProductDeclaration MaterialFragment.");
                    continue;
                }
                fireProtectionTankResults[x] = new GlobalWarmingPotentialResult(fireProtectionTankResults[x].ObjectId, fireProtectionTankResults[x].ResultCase, fireProtectionTankResults[x].TimeStep, ObjectScope.FireProtection, ObjectCategory.Tanks, fireProtectionTankResults[x].EnvironmentalProductDeclaration, (fireProtectionTankResults[x] as GlobalWarmingPotentialResult).GlobalWarmingPotential);
            }
            results.AddRange(fireProtectionTankResults);

            //FireProtectionScope AdditionalObjects
            List<LifeCycleAssessmentElementResult> additionalFireProtectionObjectsResults = mepScope.FireProtectionScope.AdditionalObjects.SelectMany(x => EvaluateEnvironmentalProductDeclarationPerObject(x, field).Collapse()).ToList();
            for (int x = 0; x < additionalFireProtectionObjectsResults.Count; x++)
            {
                if (additionalFireProtectionObjectsResults[x] == null)
                {
                    BH.Engine.Reflection.Compute.RecordWarning("Fire Protection object " + ((IBHoMObject)mepScope.FireProtectionScope.AdditionalObjects[x]).BHoM_Guid + " does not contain a valid IEnvironmentalProductDeclaration MaterialFragment.");
                    continue;
                }
                additionalFireProtectionObjectsResults[x] = new GlobalWarmingPotentialResult(additionalFireProtectionObjectsResults[x].ObjectId, additionalFireProtectionObjectsResults[x].ResultCase, additionalFireProtectionObjectsResults[x].TimeStep, ObjectScope.FireProtection, ObjectCategory.AdditionalObjects, additionalFireProtectionObjectsResults[x].EnvironmentalProductDeclaration, (additionalFireProtectionObjectsResults[x] as GlobalWarmingPotentialResult).GlobalWarmingPotential);
            }
            results.AddRange(additionalFireProtectionObjectsResults);

            return results;
        }

        /***************************************************/

        [Description("This method calls the appropriate compute method per object within the TenantImprovementScope and returns results.")]
        [Input("tenantImprovementScope", "This is a life cycle assessment scope object which contains all specified objects along with their associated EPD MaterialFragment data.")]
        [Input("field", "This is the desired field you would like to evaluate. Notice that not all material datasets will contain information for all metrics. A holistic GWP evaluation is currently the only standard evaluation possible with all provided datasets.")]
        [Output("result", "The LifeCycleElementResult that contains Tenant Improvement scope specific data, total results, and results per element.")]
        public static List<LifeCycleAssessmentElementResult> EvaluateLifeCycleAssessmentScope(this TenantImprovementScope tenantImprovementScope, EnvironmentalProductDeclarationField field)
        {
            if (tenantImprovementScope.IsValid() == false)
            {
                BH.Engine.Reflection.Compute.RecordWarning("Your TenantImprovementScope is incomplete, please provide all types of objects where possible.");
            }

            List<LifeCycleAssessmentElementResult> results = new List<LifeCycleAssessmentElementResult>();

            //TI Ceilings
            List<LifeCycleAssessmentElementResult> ceilingResults = tenantImprovementScope.Ceiling.SelectMany(x => EvaluateEnvironmentalProductDeclarationPerObject(x, field).Collapse()).ToList();
            for (int x = 0; x < ceilingResults.Count; x++)
            {
                if (ceilingResults[x] == null)
                {
                    BH.Engine.Reflection.Compute.RecordWarning("Object " + ((IBHoMObject)tenantImprovementScope.Ceiling[x]).BHoM_Guid + " Within TentantImprovementScope Ceilings does not contain a valid IEnvironmentalProductDeclaration MaterialFragment.");
                    continue;
                }
                ceilingResults[x] = new GlobalWarmingPotentialResult(ceilingResults[x].ObjectId, ceilingResults[x].ResultCase, ceilingResults[x].TimeStep, ObjectScope.TenantImprovement, ObjectCategory.Ceiling, ceilingResults[x].EnvironmentalProductDeclaration, (ceilingResults[x] as GlobalWarmingPotentialResult).GlobalWarmingPotential);
            }
            results.AddRange(ceilingResults);

            //TI Finishes
            List<LifeCycleAssessmentElementResult> finishResults = tenantImprovementScope.Finishes.SelectMany(x => EvaluateEnvironmentalProductDeclarationPerObject(x, field).Collapse()).ToList();
            for (int x = 0; x < finishResults.Count; x++)
            {
                if (finishResults[x] == null)
                {
                    BH.Engine.Reflection.Compute.RecordWarning("Object " + ((IBHoMObject)tenantImprovementScope.Finishes[x]).BHoM_Guid + " Within TentantImprovementScope Finishes does not contain a valid IEnvironmentalProductDeclaration MaterialFragment.");
                    continue;
                }
                finishResults[x] = new GlobalWarmingPotentialResult(finishResults[x].ObjectId, finishResults[x].ResultCase, finishResults[x].TimeStep, ObjectScope.TenantImprovement, ObjectCategory.Finishes, finishResults[x].EnvironmentalProductDeclaration, (finishResults[x] as GlobalWarmingPotentialResult).GlobalWarmingPotential);
            }
            results.AddRange(finishResults);

            //TI Furniture
            List<LifeCycleAssessmentElementResult> furnitureResults = tenantImprovementScope.Furniture.SelectMany(x => EvaluateEnvironmentalProductDeclarationPerObject(x, field).Collapse()).ToList();
            for (int x = 0; x < furnitureResults.Count; x++)
            {
                if (furnitureResults[x] == null)
                {
                    BH.Engine.Reflection.Compute.RecordWarning("Object " + ((IBHoMObject)tenantImprovementScope.Furniture[x]).BHoM_Guid + " Within TentantImprovementScope Furniture does not contain a valid IEnvironmentalProductDeclaration MaterialFragment.");
                    continue;
                }
                furnitureResults[x] = new GlobalWarmingPotentialResult(furnitureResults[x].ObjectId, furnitureResults[x].ResultCase, furnitureResults[x].TimeStep, ObjectScope.TenantImprovement, ObjectCategory.Furniture, furnitureResults[x].EnvironmentalProductDeclaration, (furnitureResults[x] as GlobalWarmingPotentialResult).GlobalWarmingPotential);
            }
            results.AddRange(furnitureResults);

            //TI Interior Doors
            List<LifeCycleAssessmentElementResult> intDoorsResults = tenantImprovementScope.InteriorDoors.SelectMany(x => EvaluateEnvironmentalProductDeclarationPerObject(x, field).Collapse()).ToList();
            for (int x = 0; x < intDoorsResults.Count; x++)
            {
                if (intDoorsResults[x] == null)
                {
                    BH.Engine.Reflection.Compute.RecordWarning("Object " + ((IBHoMObject)tenantImprovementScope.InteriorDoors[x]).BHoM_Guid + " Within TentantImprovementScope InteriorDoors does not contain a valid IEnvironmentalProductDeclaration MaterialFragment.");
                    continue;
                }
                intDoorsResults[x] = new GlobalWarmingPotentialResult(intDoorsResults[x].ObjectId, intDoorsResults[x].ResultCase, intDoorsResults[x].TimeStep, ObjectScope.TenantImprovement, ObjectCategory.InteriorDoor, intDoorsResults[x].EnvironmentalProductDeclaration, (intDoorsResults[x] as GlobalWarmingPotentialResult).GlobalWarmingPotential);
            }
            results.AddRange(intDoorsResults);

            //TI Interior Glazing
            List<LifeCycleAssessmentElementResult> intGlazingResults = tenantImprovementScope.InteriorGlazing.SelectMany(x => EvaluateEnvironmentalProductDeclarationPerObject(x, field).Collapse()).ToList();
            for (int x = 0; x < intGlazingResults.Count; x++)
            {
                if (intGlazingResults[x] == null)
                {
                    BH.Engine.Reflection.Compute.RecordWarning("Object " + ((IBHoMObject)tenantImprovementScope.InteriorGlazing[x]).BHoM_Guid + " Within TentantImprovementScope InteriorGlazing does not contain a valid IEnvironmentalProductDeclaration MaterialFragment.");
                    continue;
                }
                intGlazingResults[x] = new GlobalWarmingPotentialResult(intGlazingResults[x].ObjectId, intGlazingResults[x].ResultCase, intGlazingResults[x].TimeStep, ObjectScope.TenantImprovement, ObjectCategory.InteriorGlazing, intGlazingResults[x].EnvironmentalProductDeclaration, (intGlazingResults[x] as GlobalWarmingPotentialResult).GlobalWarmingPotential);
            }
            results.AddRange(intGlazingResults);

            //TI Partition Walls
            List<LifeCycleAssessmentElementResult> partWallsResults = tenantImprovementScope.PartitionWalls.SelectMany(x => EvaluateEnvironmentalProductDeclarationPerObject(x, field).Collapse()).ToList();
            for (int x = 0; x < partWallsResults.Count; x++)
            {
                if (partWallsResults[x] == null)
                {
                    BH.Engine.Reflection.Compute.RecordWarning("Object " + ((IBHoMObject)tenantImprovementScope.PartitionWalls[x]).BHoM_Guid + " Within TentantImprovementScope PartitionWalls does not contain a valid IEnvironmentalProductDeclaration MaterialFragment.");
                    continue;
                }
                partWallsResults[x] = new GlobalWarmingPotentialResult(partWallsResults[x].ObjectId, partWallsResults[x].ResultCase, partWallsResults[x].TimeStep, ObjectScope.TenantImprovement, ObjectCategory.PartitionWall, partWallsResults[x].EnvironmentalProductDeclaration, (partWallsResults[x] as GlobalWarmingPotentialResult).GlobalWarmingPotential);
            }
            results.AddRange(partWallsResults);

            //TI AdditionalObjects
            List<LifeCycleAssessmentElementResult> additionalObjectsResults = tenantImprovementScope.AdditionalObjects.SelectMany(x => EvaluateEnvironmentalProductDeclarationPerObject(x, field).Collapse()).ToList();
            for (int x = 0; x < additionalObjectsResults.Count; x++)
            {
                if (additionalObjectsResults[x] == null)
                {
                    BH.Engine.Reflection.Compute.RecordWarning("Object " + ((IBHoMObject)tenantImprovementScope.AdditionalObjects[x]).BHoM_Guid + " Within TenantImprovementScope AdditionalObjects does not contain a valid IEnvironmentalProductDeclaration MaterialFragment.");
                    continue;
                }
                additionalObjectsResults[x] = new GlobalWarmingPotentialResult(additionalObjectsResults[x].ObjectId, additionalObjectsResults[x].ResultCase, additionalObjectsResults[x].TimeStep, ObjectScope.TenantImprovement, ObjectCategory.AdditionalObjects, additionalObjectsResults[x].EnvironmentalProductDeclaration, (additionalObjectsResults[x] as GlobalWarmingPotentialResult).GlobalWarmingPotential);
            }
            results.AddRange(additionalObjectsResults);

            return results;
        }

        /***************************************************/
    }
}

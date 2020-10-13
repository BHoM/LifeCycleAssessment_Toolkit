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

using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using BH.oM.Base;
using BH.oM.Reflection.Attributes;
using BH.oM.LifeCycleAssessment;
using BH.oM.LifeCycleAssessment.Results;
using BH.oM.MEP.Elements;

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
            List<LifeCycleAssessmentElementResult> beamResults = structuresScope.Beams.Select(x => EvaluateEnvironmentalProductDeclarationPerObject(x as IBHoMObject, field)).ToList();
            for (int x = 0; x < beamResults.Count; x++)
            {
                if (beamResults[x] == null)
                {
                    BH.Engine.Reflection.Compute.RecordWarning("Object " + structuresScope.Beams[x].BHoM_Guid + " Within StructuresScope Beams does not contain a valid IEnvironmentalProductDeclaration MaterialFragment.");
                    continue;
                }
                beamResults[x] = new GlobalWarmingPotentialResult(beamResults[x].ObjectId, beamResults[x].ResultCase, beamResults[x].TimeStep, ObjectScope.Structure, ObjectCategory.Beam, beamResults[x].EnvironmentalProductDeclaration, (beamResults[x] as GlobalWarmingPotentialResult).GlobalWarmingPotential);
            }
            results.AddRange(beamResults);

            //StructuresScope Columns
            List<LifeCycleAssessmentElementResult> columnsResults = structuresScope.Columns.Select(x => EvaluateEnvironmentalProductDeclarationPerObject(x as IBHoMObject, field)).ToList();
            for (int x = 0; x < columnsResults.Count; x++)
            {
                if (columnsResults[x] == null)
                {
                    BH.Engine.Reflection.Compute.RecordWarning("Object " + structuresScope.Columns[x].BHoM_Guid + " Within StructuresScope Columns does not contain a valid IEnvironmentalProductDeclaration MaterialFragment.");
                    continue;
                }
                columnsResults[x] = new GlobalWarmingPotentialResult(columnsResults[x].ObjectId, columnsResults[x].ResultCase, columnsResults[x].TimeStep, ObjectScope.Structure, ObjectCategory.Column, columnsResults[x].EnvironmentalProductDeclaration, (columnsResults[x] as GlobalWarmingPotentialResult).GlobalWarmingPotential);
            }
            results.AddRange(columnsResults);

            //StructuresScope Slabs
            List<LifeCycleAssessmentElementResult> slabsResults = structuresScope.Slabs.Select(x => EvaluateEnvironmentalProductDeclarationPerObject(x as IBHoMObject, field)).ToList();
            for (int x = 0; x < slabsResults.Count; x++)
            {
                if (slabsResults[x] == null)
                {
                    BH.Engine.Reflection.Compute.RecordWarning("Object " + structuresScope.Slabs[x].BHoM_Guid + " Within StructuresScope Slabs does not contain a valid IEnvironmentalProductDeclaration MaterialFragment.");
                    continue;
                }
                slabsResults[x] = new GlobalWarmingPotentialResult(slabsResults[x].ObjectId, slabsResults[x].ResultCase, slabsResults[x].TimeStep, ObjectScope.Structure, ObjectCategory.Slab, slabsResults[x].EnvironmentalProductDeclaration, (slabsResults[x] as GlobalWarmingPotentialResult).GlobalWarmingPotential);
            }
            results.AddRange(slabsResults);

            //StructuresScope Core Walls
            List<LifeCycleAssessmentElementResult> coreWallsResults = structuresScope.CoreWalls.Select(x => EvaluateEnvironmentalProductDeclarationPerObject(x as IBHoMObject, field)).ToList();
            for (int x = 0; x < coreWallsResults.Count; x++)
            {
                if (coreWallsResults[x] == null)
                {
                    BH.Engine.Reflection.Compute.RecordWarning("Object " + structuresScope.CoreWalls[x].BHoM_Guid + " Within StructuresScope CoreWalls does not contain a valid IEnvironmentalProductDeclaration MaterialFragment.");
                    continue;
                }
                coreWallsResults[x] = new GlobalWarmingPotentialResult(coreWallsResults[x].ObjectId, coreWallsResults[x].ResultCase, coreWallsResults[x].TimeStep, ObjectScope.Structure, ObjectCategory.Wall, coreWallsResults[x].EnvironmentalProductDeclaration, (coreWallsResults[x] as GlobalWarmingPotentialResult).GlobalWarmingPotential);
            }
            results.AddRange(coreWallsResults);

            //StructuresScope Bracing
            List<LifeCycleAssessmentElementResult> bracingResults = structuresScope.Bracing.Select(x => EvaluateEnvironmentalProductDeclarationPerObject(x as IBHoMObject, field)).ToList();
            for (int x = 0; x < bracingResults.Count; x++)
            {
                if (bracingResults[x] == null)
                {
                    BH.Engine.Reflection.Compute.RecordWarning("Object " + structuresScope.Bracing[x].BHoM_Guid + " Within StructuresScope Bracing does not contain a valid IEnvironmentalProductDeclaration MaterialFragment.");
                    continue;
                }
                bracingResults[x] = new GlobalWarmingPotentialResult(bracingResults[x].ObjectId, bracingResults[x].ResultCase, bracingResults[x].TimeStep, ObjectScope.Structure, ObjectCategory.Bracing, bracingResults[x].EnvironmentalProductDeclaration, (bracingResults[x] as GlobalWarmingPotentialResult).GlobalWarmingPotential);
            }
            results.AddRange(bracingResults);

            //StructuresScope AdditionalObjects
            List<LifeCycleAssessmentElementResult> additionalObjectsResults = structuresScope.AdditionalObjects.Select(x => EvaluateEnvironmentalProductDeclarationPerObject(x as IBHoMObject, field)).ToList();
            for (int x = 0; x < additionalObjectsResults.Count; x++)
            {
                if (additionalObjectsResults[x] == null)
                {
                    BH.Engine.Reflection.Compute.RecordWarning("Object " + structuresScope.AdditionalObjects[x].BHoM_Guid + " Within StructuresScope AdditionalObjects does not contain a valid IEnvironmentalProductDeclaration MaterialFragment.");
                    continue;
                }
                additionalObjectsResults[x] = new GlobalWarmingPotentialResult(additionalObjectsResults[x].ObjectId, additionalObjectsResults[x].ResultCase, additionalObjectsResults[x].TimeStep, ObjectScope.Structure, ObjectCategory.AdditionalObjects, additionalObjectsResults[x].EnvironmentalProductDeclaration, (additionalObjectsResults[x] as GlobalWarmingPotentialResult).GlobalWarmingPotential);
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
            List<LifeCycleAssessmentElementResult> footingsResults = foundationsScope.Footings.Select(x => EvaluateEnvironmentalProductDeclarationPerObject(x as IBHoMObject, field)).ToList();
            for (int x = 0; x < footingsResults.Count; x++)
            {
                if (footingsResults[x] == null)
                {
                    BH.Engine.Reflection.Compute.RecordWarning("Object " + foundationsScope.Footings[x].BHoM_Guid + " Within FoundationsScope Footings does not contain a valid IEnvironmentalProductDeclaration MaterialFragment.");
                    continue;
                }
                footingsResults[x] = new GlobalWarmingPotentialResult(footingsResults[x].ObjectId, footingsResults[x].ResultCase, footingsResults[x].TimeStep, ObjectScope.Foundation, ObjectCategory.Footing, footingsResults[x].EnvironmentalProductDeclaration, (footingsResults[x] as GlobalWarmingPotentialResult).GlobalWarmingPotential);
            }
            results.AddRange(footingsResults);

            //FoundationsScope Piles
            List<LifeCycleAssessmentElementResult> pilesResults = foundationsScope.Piles.Select(x => EvaluateEnvironmentalProductDeclarationPerObject(x as IBHoMObject, field)).ToList();
            for (int x = 0; x < pilesResults.Count; x++)
            {
                if (pilesResults[x] == null)
                {
                    BH.Engine.Reflection.Compute.RecordWarning("Object " + foundationsScope.Piles[x].BHoM_Guid + " Within FoundationsScope Piles does not contain a valid IEnvironmentalProductDeclaration MaterialFragment.");
                    continue;
                }
                pilesResults[x] = new GlobalWarmingPotentialResult(pilesResults[x].ObjectId, pilesResults[x].ResultCase, pilesResults[x].TimeStep, ObjectScope.Foundation, ObjectCategory.Pile, pilesResults[x].EnvironmentalProductDeclaration, (pilesResults[x] as GlobalWarmingPotentialResult).GlobalWarmingPotential);
            }
            results.AddRange(pilesResults);

            //FoundationsScope Walls
            List<LifeCycleAssessmentElementResult> wallsResults = foundationsScope.Walls.Select(x => EvaluateEnvironmentalProductDeclarationPerObject(x as IBHoMObject, field)).ToList();
            for (int x = 0; x < wallsResults.Count; x++)
            {
                if (wallsResults[x] == null)
                {
                    BH.Engine.Reflection.Compute.RecordWarning("Object " + foundationsScope.Walls[x].BHoM_Guid + " Within FoundationsScope Walls does not contain a valid IEnvironmentalProductDeclaration MaterialFragment.");
                    continue;
                }
                wallsResults[x] = new GlobalWarmingPotentialResult(wallsResults[x].ObjectId, wallsResults[x].ResultCase, wallsResults[x].TimeStep, ObjectScope.Foundation, ObjectCategory.Wall, wallsResults[x].EnvironmentalProductDeclaration, (wallsResults[x] as GlobalWarmingPotentialResult).GlobalWarmingPotential);
            }
            results.AddRange(wallsResults);

            //FoundationsScope Slabs
            List<LifeCycleAssessmentElementResult> fndSlabsResults = foundationsScope.Slabs.Select(x => EvaluateEnvironmentalProductDeclarationPerObject(x as IBHoMObject, field)).ToList();
            for (int x = 0; x < fndSlabsResults.Count; x++)
            {
                if (fndSlabsResults[x] == null)
                {
                    BH.Engine.Reflection.Compute.RecordWarning("Object " + foundationsScope.Slabs[x].BHoM_Guid + " Within FoundationsScope Slabs does not contain a valid IEnvironmentalProductDeclaration MaterialFragment.");
                    continue;
                }
                fndSlabsResults[x] = new GlobalWarmingPotentialResult(fndSlabsResults[x].ObjectId, fndSlabsResults[x].ResultCase, fndSlabsResults[x].TimeStep, ObjectScope.Foundation, ObjectCategory.Slab, fndSlabsResults[x].EnvironmentalProductDeclaration, (fndSlabsResults[x] as GlobalWarmingPotentialResult).GlobalWarmingPotential);
            }
            results.AddRange(fndSlabsResults);

            //FoundationsScope GradeBeams
            List<LifeCycleAssessmentElementResult> gradeBeamsResults = foundationsScope.GradeBeams.Select(x => EvaluateEnvironmentalProductDeclarationPerObject(x as IBHoMObject, field)).ToList();
            for (int x = 0; x < gradeBeamsResults.Count; x++)
            {
                if (gradeBeamsResults[x] == null)
                {
                    BH.Engine.Reflection.Compute.RecordWarning("Object " + foundationsScope.GradeBeams[x].BHoM_Guid + " Within FoundationsScope GradeBeams does not contain a valid IEnvironmentalProductDeclaration MaterialFragment.");
                    continue;
                }
                gradeBeamsResults[x] = new GlobalWarmingPotentialResult(gradeBeamsResults[x].ObjectId, gradeBeamsResults[x].ResultCase, gradeBeamsResults[x].TimeStep, ObjectScope.Foundation, ObjectCategory.GradeBeam, gradeBeamsResults[x].EnvironmentalProductDeclaration, (gradeBeamsResults[x] as GlobalWarmingPotentialResult).GlobalWarmingPotential);
            }
            results.AddRange(gradeBeamsResults);

            //FoundationScope AdditionalObjects
            List<LifeCycleAssessmentElementResult> additionalObjectsResults = foundationsScope.AdditionalObjects.Select(x => EvaluateEnvironmentalProductDeclarationPerObject(x as IBHoMObject, field)).ToList();
            for (int x = 0; x < additionalObjectsResults.Count; x++)
            {
                if (additionalObjectsResults[x] == null)
                {
                    BH.Engine.Reflection.Compute.RecordWarning("Object " + foundationsScope.AdditionalObjects[x].BHoM_Guid + " Within FoundationScope AdditionalObjects does not contain a valid IEnvironmentalProductDeclaration MaterialFragment.");
                    continue;
                }
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
            List<LifeCycleAssessmentElementResult> enclWallsResults = enclosuresScope.Walls.Select(x => EvaluateEnvironmentalProductDeclarationPerObject(x as IBHoMObject, field)).ToList();
            for (int x = 0; x < enclWallsResults.Count; x++)
            {
                if (enclWallsResults[x] == null)
                {
                    BH.Engine.Reflection.Compute.RecordWarning("Object " + enclosuresScope.Walls[x].BHoM_Guid + " Within EnclosuresScope Walls does not contain a valid IEnvironmentalProductDeclaration MaterialFragment.");
                    continue;
                }
                enclWallsResults[x] = new GlobalWarmingPotentialResult(enclWallsResults[x].ObjectId, enclWallsResults[x].ResultCase, enclWallsResults[x].TimeStep, ObjectScope.Enclosure, ObjectCategory.Wall, enclWallsResults[x].EnvironmentalProductDeclaration, (enclWallsResults[x] as GlobalWarmingPotentialResult).GlobalWarmingPotential);
            }
            results.AddRange(enclWallsResults);

            //EnclosuresScope CurtainWalls
            List<LifeCycleAssessmentElementResult> curtainWallsResults = enclosuresScope.CurtainWalls.Select(x => EvaluateEnvironmentalProductDeclarationPerObject(x as IBHoMObject, field)).ToList();
            for (int x = 0; x < curtainWallsResults.Count; x++)
            {
                if (curtainWallsResults[x] == null)
                {
                    BH.Engine.Reflection.Compute.RecordWarning("Object " + enclosuresScope.CurtainWalls[x].BHoM_Guid + " Within EnclosuresScope CurtainWalls does not contain a valid IEnvironmentalProductDeclaration MaterialFragment.");
                    continue;
                }
                curtainWallsResults[x] = new GlobalWarmingPotentialResult(curtainWallsResults[x].ObjectId, curtainWallsResults[x].ResultCase, curtainWallsResults[x].TimeStep, ObjectScope.Enclosure, ObjectCategory.CurtainWall, curtainWallsResults[x].EnvironmentalProductDeclaration, (curtainWallsResults[x] as GlobalWarmingPotentialResult).GlobalWarmingPotential);
            }
            results.AddRange(curtainWallsResults);

            //EnclosuresScope Windows
            List<LifeCycleAssessmentElementResult> windowsResults = enclosuresScope.Windows.Select(x => EvaluateEnvironmentalProductDeclarationPerObject(x as IBHoMObject, field)).ToList();
            for (int x = 0; x < windowsResults.Count; x++)
            {
                if (windowsResults[x] == null)
                {
                    BH.Engine.Reflection.Compute.RecordWarning("Object " + enclosuresScope.Windows[x].BHoM_Guid + " Within EnclosuresScope Windows does not contain a valid IEnvironmentalProductDeclaration MaterialFragment.");
                    continue;
                }
                windowsResults[x] = new GlobalWarmingPotentialResult(windowsResults[x].ObjectId, windowsResults[x].ResultCase, windowsResults[x].TimeStep, ObjectScope.Enclosure, ObjectCategory.Window, windowsResults[x].EnvironmentalProductDeclaration, (windowsResults[x] as GlobalWarmingPotentialResult).GlobalWarmingPotential);
            }
            results.AddRange(windowsResults);

            //EnclosuresScope Doors
            List<LifeCycleAssessmentElementResult> doorsResults = enclosuresScope.Doors.Select(x => EvaluateEnvironmentalProductDeclarationPerObject(x as IBHoMObject, field)).ToList();
            for (int x = 0; x < doorsResults.Count; x++)
            {
                if (doorsResults[x] == null)
                {
                    BH.Engine.Reflection.Compute.RecordWarning("Object " + enclosuresScope.Doors[x].BHoM_Guid + " Within EnclosuresScope Doors does not contain a valid IEnvironmentalProductDeclaration MaterialFragment.");
                    continue;
                }
                doorsResults[x] = new GlobalWarmingPotentialResult(doorsResults[x].ObjectId, doorsResults[x].ResultCase, doorsResults[x].TimeStep, ObjectScope.Enclosure, ObjectCategory.Wall, doorsResults[x].EnvironmentalProductDeclaration, (doorsResults[x] as GlobalWarmingPotentialResult).GlobalWarmingPotential);
            }
            results.AddRange(doorsResults);

            //Enclosures AdditionalObjects
            List<LifeCycleAssessmentElementResult> additionalObjectsResults = enclosuresScope.AdditionalObjects.Select(x => EvaluateEnvironmentalProductDeclarationPerObject(x as IBHoMObject, field)).ToList();
            for (int x = 0; x < additionalObjectsResults.Count; x++)
            {
                if (additionalObjectsResults[x] == null)
                {
                    BH.Engine.Reflection.Compute.RecordWarning("Object " + enclosuresScope.AdditionalObjects[x].BHoM_Guid + " Within EnclosuresScope AdditionalObjects does not contain a valid IEnvironmentalProductDeclaration MaterialFragment.");
                    continue;
                }
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

            //MechanicalScope Ducts
            List<LifeCycleAssessmentElementResult> ductResults = mepScope.MechanicalScope.Ducts.Select(x => EvaluateEnvironmentalProductDeclarationPerObject(x as Duct, field)).ToList();
            for (int x = 0; x < ductResults.Count; x++)
            {
                if (ductResults[x] == null)
                {
                    BH.Engine.Reflection.Compute.RecordWarning("Duct object " + mepScope.MechanicalScope.Ducts[x].BHoM_Guid + " does not contain a valid IEnvironmentalProductDeclaration MaterialFragment.");
                    continue;
                }
                ductResults[x] = new GlobalWarmingPotentialResult(ductResults[x].ObjectId, ductResults[x].ResultCase, ductResults[x].TimeStep, ObjectScope.Mechanical, ObjectCategory.Ducts, ductResults[x].EnvironmentalProductDeclaration, (ductResults[x] as GlobalWarmingPotentialResult).GlobalWarmingPotential);
            }
            results.AddRange(ductResults);

            //MechanicalScope Equipment
            List<LifeCycleAssessmentElementResult> mechanicalEquipmentResults = mepScope.MechanicalScope.Equipment.Select(x => EvaluateEnvironmentalProductDeclarationPerObject(x as IBHoMObject, field)).ToList();
            for (int x = 0; x < mechanicalEquipmentResults.Count; x++)
            {
                if (mechanicalEquipmentResults[x] == null)
                {
                    BH.Engine.Reflection.Compute.RecordWarning("Mechanical Equipment object " + mepScope.MechanicalScope.Equipment[x].BHoM_Guid + " does not contain a valid IEnvironmentalProductDeclaration MaterialFragment.");
                    continue;
                }
                mechanicalEquipmentResults[x] = new GlobalWarmingPotentialResult(mechanicalEquipmentResults[x].ObjectId, mechanicalEquipmentResults[x].ResultCase, mechanicalEquipmentResults[x].TimeStep, ObjectScope.Mechanical, ObjectCategory.Equipment, mechanicalEquipmentResults[x].EnvironmentalProductDeclaration, (mechanicalEquipmentResults[x] as GlobalWarmingPotentialResult).GlobalWarmingPotential);
            }
            results.AddRange(mechanicalEquipmentResults);

            //MechanicalScope Pipes
            List<LifeCycleAssessmentElementResult> pipingResults = mepScope.MechanicalScope.Pipes.Select(x => EvaluateEnvironmentalProductDeclarationPerObject(x as Pipe, field)).ToList();
            for (int x = 0; x < pipingResults.Count; x++)
            {
                if (pipingResults[x] == null)
                {
                    BH.Engine.Reflection.Compute.RecordWarning("Mechanical Pipe object " + mepScope.MechanicalScope.Pipes[x].BHoM_Guid + " does not contain a valid IEnvironmentalProductDeclaration MaterialFragment.");
                    continue;
                }
                pipingResults[x] = new GlobalWarmingPotentialResult(pipingResults[x].ObjectId, pipingResults[x].ResultCase, pipingResults[x].TimeStep, ObjectScope.Mechanical, ObjectCategory.Pipes, pipingResults[x].EnvironmentalProductDeclaration, (pipingResults[x] as GlobalWarmingPotentialResult).GlobalWarmingPotential);
            }
            results.AddRange(pipingResults);

            //MechanicalScope AdditionalObjects
            List<LifeCycleAssessmentElementResult> additionalMechanicalObjectsResults = mepScope.MechanicalScope.AdditionalObjects.Select(x => EvaluateEnvironmentalProductDeclarationPerObject(x as IBHoMObject, field)).ToList();
            for (int x = 0; x < additionalMechanicalObjectsResults.Count; x++)
            {
                if (additionalMechanicalObjectsResults[x] == null)
                {
                    BH.Engine.Reflection.Compute.RecordWarning("Additional Mechanical object " + mepScope.MechanicalScope.AdditionalObjects[x].BHoM_Guid + " within MechanicalScope does not contain a valid IEnvironmentalProductDeclaration MaterialFragment.");
                    continue;
                }
                additionalMechanicalObjectsResults[x] = new GlobalWarmingPotentialResult(additionalMechanicalObjectsResults[x].ObjectId, additionalMechanicalObjectsResults[x].ResultCase, additionalMechanicalObjectsResults[x].TimeStep, ObjectScope.Mechanical, ObjectCategory.AdditionalObjects, additionalMechanicalObjectsResults[x].EnvironmentalProductDeclaration, (additionalMechanicalObjectsResults[x] as GlobalWarmingPotentialResult).GlobalWarmingPotential);
            }
            results.AddRange(additionalMechanicalObjectsResults);

            //ElectricalScope

            //ElectricalScope Batteries
            List<LifeCycleAssessmentElementResult> batteriesResults = mepScope.ElectricalScope.Batteries.Select(x => EvaluateEnvironmentalProductDeclarationPerObject(x as IBHoMObject, field)).ToList();
            for (int x = 0; x < batteriesResults.Count; x++)
            {
                if (batteriesResults[x] == null)
                {
                    BH.Engine.Reflection.Compute.RecordWarning("Battery object " + mepScope.ElectricalScope.Batteries[x].BHoM_Guid + " does not contain a valid IEnvironmentalProductDeclaration MaterialFragment.");
                    continue;
                }
                batteriesResults[x] = new GlobalWarmingPotentialResult(batteriesResults[x].ObjectId, batteriesResults[x].ResultCase, batteriesResults[x].TimeStep, ObjectScope.Electrical, ObjectCategory.Battery, batteriesResults[x].EnvironmentalProductDeclaration, (batteriesResults[x] as GlobalWarmingPotentialResult).GlobalWarmingPotential);
            }
            results.AddRange(batteriesResults);

            //ElectricalScope CableTray
            List<LifeCycleAssessmentElementResult> cableTrayResults = mepScope.ElectricalScope.CableTrays.Select(x => EvaluateEnvironmentalProductDeclarationPerObject(x as IBHoMObject, field)).ToList();
            for (int x = 0; x < cableTrayResults.Count; x++)
            {
                if (cableTrayResults[x] == null)
                {
                    BH.Engine.Reflection.Compute.RecordWarning("CableTray object " + mepScope.ElectricalScope.CableTrays[x].BHoM_Guid + " does not contain a valid IEnvironmentalProductDeclaration MaterialFragment.");
                    continue;
                }
                cableTrayResults[x] = new GlobalWarmingPotentialResult(cableTrayResults[x].ObjectId, cableTrayResults[x].ResultCase, cableTrayResults[x].TimeStep, ObjectScope.Electrical, ObjectCategory.CableTray, cableTrayResults[x].EnvironmentalProductDeclaration, (cableTrayResults[x] as GlobalWarmingPotentialResult).GlobalWarmingPotential);
            }
            results.AddRange(cableTrayResults);

            //ElectricalScope Conduit
            List<LifeCycleAssessmentElementResult> conduitResults = mepScope.ElectricalScope.Conduit.Select(x => EvaluateEnvironmentalProductDeclarationPerObject(x as IBHoMObject, field)).ToList();
            for (int x = 0; x < conduitResults.Count; x++)
            {
                if (conduitResults[x] == null)
                {
                    BH.Engine.Reflection.Compute.RecordWarning("Conduit object " + mepScope.ElectricalScope.Conduit[x].BHoM_Guid + " does not contain a valid IEnvironmentalProductDeclaration MaterialFragment.");
                    continue;
                }
                conduitResults[x] = new GlobalWarmingPotentialResult(conduitResults[x].ObjectId, conduitResults[x].ResultCase, conduitResults[x].TimeStep, ObjectScope.Electrical, ObjectCategory.Conduit, conduitResults[x].EnvironmentalProductDeclaration, (conduitResults[x] as GlobalWarmingPotentialResult).GlobalWarmingPotential);
            }
            results.AddRange(conduitResults);

            //ElectricalScope Equipment
            List<LifeCycleAssessmentElementResult> electricalEquipmentResults = mepScope.ElectricalScope.Equipment.Select(x => EvaluateEnvironmentalProductDeclarationPerObject(x as IBHoMObject, field)).ToList();
            for (int x = 0; x < electricalEquipmentResults.Count; x++)
            {
                if (conduitResults[x] == null)
                {
                    BH.Engine.Reflection.Compute.RecordWarning("Electrical Equipment object " + mepScope.ElectricalScope.Equipment[x].BHoM_Guid + " does not contain a valid IEnvironmentalProductDeclaration MaterialFragment.");
                    continue;
                }
                electricalEquipmentResults[x] = new GlobalWarmingPotentialResult(electricalEquipmentResults[x].ObjectId, electricalEquipmentResults[x].ResultCase, electricalEquipmentResults[x].TimeStep, ObjectScope.Electrical, ObjectCategory.Equipment, electricalEquipmentResults[x].EnvironmentalProductDeclaration, (electricalEquipmentResults[x] as GlobalWarmingPotentialResult).GlobalWarmingPotential);
            }
            results.AddRange(electricalEquipmentResults);

            //ElectricalScope Generator
            List<LifeCycleAssessmentElementResult> generatorResults = mepScope.ElectricalScope.Generators.Select(x => EvaluateEnvironmentalProductDeclarationPerObject(x as IBHoMObject, field)).ToList();
            for (int x = 0; x < generatorResults.Count; x++)
            {
                if (generatorResults[x] == null)
                {
                    BH.Engine.Reflection.Compute.RecordWarning("Generator object " + mepScope.ElectricalScope.Generators[x].BHoM_Guid + " does not contain a valid IEnvironmentalProductDeclaration MaterialFragment.");
                    continue;
                }
                generatorResults[x] = new GlobalWarmingPotentialResult(generatorResults[x].ObjectId, generatorResults[x].ResultCase, generatorResults[x].TimeStep, ObjectScope.Electrical, ObjectCategory.Generator, generatorResults[x].EnvironmentalProductDeclaration, (generatorResults[x] as GlobalWarmingPotentialResult).GlobalWarmingPotential);
            }
            results.AddRange(generatorResults);

            //ElectricalScope Lighting
            List<LifeCycleAssessmentElementResult> lightingResults = mepScope.ElectricalScope.LightFixtures.Select(x => EvaluateEnvironmentalProductDeclarationPerObject(x as IBHoMObject, field)).ToList();
            for (int x = 0; x < lightingResults.Count; x++)
            {
                if (lightingResults[x] == null)
                {
                    BH.Engine.Reflection.Compute.RecordWarning("Light Fixture object" + mepScope.ElectricalScope.LightFixtures[x].BHoM_Guid + " does not contain a valid IEnvironmentalProductDeclaration MaterialFragment.");
                    continue;
                }
                lightingResults[x] = new GlobalWarmingPotentialResult(lightingResults[x].ObjectId, lightingResults[x].ResultCase, lightingResults[x].TimeStep, ObjectScope.Electrical, ObjectCategory.LightFixtures, lightingResults[x].EnvironmentalProductDeclaration, (lightingResults[x] as GlobalWarmingPotentialResult).GlobalWarmingPotential);
            }
            results.AddRange(lightingResults);

            //ElectricalScope Wiring
            List<LifeCycleAssessmentElementResult> wiringResults = mepScope.ElectricalScope.WireSegments.Select(x => EvaluateEnvironmentalProductDeclarationPerObject(x as WireSegment, field)).ToList();
            for (int x = 0; x < wiringResults.Count; x++)
            {
                if (wiringResults[x] == null)
                {
                    BH.Engine.Reflection.Compute.RecordWarning("Wire Segment object " + mepScope.ElectricalScope.WireSegments[x].BHoM_Guid + " does not contain a valid IEnvironmentalProductDeclaration MaterialFragment.");
                    continue;
                }
                wiringResults[x] = new GlobalWarmingPotentialResult(wiringResults[x].ObjectId, wiringResults[x].ResultCase, wiringResults[x].TimeStep, ObjectScope.Electrical, ObjectCategory.Wiring, wiringResults[x].EnvironmentalProductDeclaration, (wiringResults[x] as GlobalWarmingPotentialResult).GlobalWarmingPotential);
            }
            results.AddRange(wiringResults);

            //ElectricalScope AdditionalObjects
            List<LifeCycleAssessmentElementResult> additionalElectricalObjectsResults = mepScope.ElectricalScope.AdditionalObjects.Select(x => EvaluateEnvironmentalProductDeclarationPerObject(x as IBHoMObject, field)).ToList();
            for (int x = 0; x < additionalElectricalObjectsResults.Count; x++)
            {
                if (additionalElectricalObjectsResults[x] == null)
                {
                    BH.Engine.Reflection.Compute.RecordWarning("Additional Electrical object " + mepScope.ElectricalScope.AdditionalObjects[x].BHoM_Guid + " does not contain a valid IEnvironmentalProductDeclaration MaterialFragment.");
                    continue;
                }
                additionalElectricalObjectsResults[x] = new GlobalWarmingPotentialResult(additionalElectricalObjectsResults[x].ObjectId, additionalElectricalObjectsResults[x].ResultCase, additionalElectricalObjectsResults[x].TimeStep, ObjectScope.Electrical, ObjectCategory.AdditionalObjects, additionalElectricalObjectsResults[x].EnvironmentalProductDeclaration, (additionalElectricalObjectsResults[x] as GlobalWarmingPotentialResult).GlobalWarmingPotential);
            }
            results.AddRange(additionalElectricalObjectsResults);

            //PlumbingScope

            //PlumbingScope Equipment
            List<LifeCycleAssessmentElementResult> plumbingEquipmentResults = mepScope.PlumbingScope.Equipment.Select(x => EvaluateEnvironmentalProductDeclarationPerObject(x as IBHoMObject, field)).ToList();
            for (int x = 0; x < plumbingEquipmentResults.Count; x++)
            {
                if (plumbingEquipmentResults[x] == null)
                {
                    BH.Engine.Reflection.Compute.RecordWarning("Plumbing Equipment object " + mepScope.PlumbingScope.Equipment[x].BHoM_Guid + " does not contain a valid IEnvironmentalProductDeclaration MaterialFragment.");
                    continue;
                }
                plumbingEquipmentResults[x] = new GlobalWarmingPotentialResult(plumbingEquipmentResults[x].ObjectId, plumbingEquipmentResults[x].ResultCase, plumbingEquipmentResults[x].TimeStep, ObjectScope.Plumbing, ObjectCategory.Equipment, plumbingEquipmentResults[x].EnvironmentalProductDeclaration, (plumbingEquipmentResults[x] as GlobalWarmingPotentialResult).GlobalWarmingPotential);
            }
            results.AddRange(plumbingEquipmentResults);

            //PlumbingScope Pipes
            List<LifeCycleAssessmentElementResult> plumbingPipingResults = mepScope.PlumbingScope.Pipes.Select(x => EvaluateEnvironmentalProductDeclarationPerObject(x as Pipe, field)).ToList();
            for (int x = 0; x < plumbingPipingResults.Count; x++)
            {
                if (plumbingPipingResults[x] == null)
                {
                    BH.Engine.Reflection.Compute.RecordWarning("Plumbing Pipe object " + mepScope.PlumbingScope.Pipes[x].BHoM_Guid + " does not contain a valid IEnvironmentalProductDeclaration MaterialFragment.");
                    continue;
                }
                plumbingPipingResults[x] = new GlobalWarmingPotentialResult(plumbingPipingResults[x].ObjectId, plumbingPipingResults[x].ResultCase, plumbingPipingResults[x].TimeStep, ObjectScope.Plumbing, ObjectCategory.Pipes, plumbingPipingResults[x].EnvironmentalProductDeclaration, (plumbingPipingResults[x] as GlobalWarmingPotentialResult).GlobalWarmingPotential);
            }
            results.AddRange(plumbingPipingResults);

            //PlumbingScope AdditionalObjects
            List<LifeCycleAssessmentElementResult> additionalPlumbingObjectsResults = mepScope.PlumbingScope.AdditionalObjects.Select(x => EvaluateEnvironmentalProductDeclarationPerObject(x as IBHoMObject, field)).ToList();
            for (int x = 0; x < additionalPlumbingObjectsResults.Count; x++)
            {
                if (additionalPlumbingObjectsResults[x] == null)
                {
                    BH.Engine.Reflection.Compute.RecordWarning("Additional Plumbing object " + mepScope.PlumbingScope.AdditionalObjects[x].BHoM_Guid + " does not contain a valid IEnvironmentalProductDeclaration MaterialFragment.");
                    continue;
                }
                additionalPlumbingObjectsResults[x] = new GlobalWarmingPotentialResult(additionalPlumbingObjectsResults[x].ObjectId, additionalPlumbingObjectsResults[x].ResultCase, additionalPlumbingObjectsResults[x].TimeStep, ObjectScope.Plumbing, ObjectCategory.AdditionalObjects, additionalPlumbingObjectsResults[x].EnvironmentalProductDeclaration, (additionalPlumbingObjectsResults[x] as GlobalWarmingPotentialResult).GlobalWarmingPotential);
            }
            results.AddRange(additionalPlumbingObjectsResults);

            //FireProtectionScope

            //FireProtectionScope Equipment
            List<LifeCycleAssessmentElementResult> fireProtectionEquipmentResults = mepScope.FireProtectionScope.Equipment.Select(x => EvaluateEnvironmentalProductDeclarationPerObject(x as BHoMObject, field)).ToList();
            for (int x = 0; x < fireProtectionEquipmentResults.Count; x++)
            {
                if (fireProtectionEquipmentResults[x] == null)
                {
                    BH.Engine.Reflection.Compute.RecordWarning("Fire Protection Equipment object " + mepScope.FireProtectionScope.Equipment[x].BHoM_Guid + " does not contain a valid IEnvironmentalProductDeclaration MaterialFragment.");
                    continue;
                }
                fireProtectionEquipmentResults[x] = new GlobalWarmingPotentialResult(fireProtectionEquipmentResults[x].ObjectId, fireProtectionEquipmentResults[x].ResultCase, fireProtectionEquipmentResults[x].TimeStep, ObjectScope.FireProtection, ObjectCategory.Equipment, fireProtectionEquipmentResults[x].EnvironmentalProductDeclaration, (fireProtectionEquipmentResults[x] as GlobalWarmingPotentialResult).GlobalWarmingPotential);
            }
            results.AddRange(fireProtectionEquipmentResults);

            //FireProtectionScope Pipes
            List<LifeCycleAssessmentElementResult> fireProtectionPipesResults = mepScope.FireProtectionScope.Pipes.Select(x => EvaluateEnvironmentalProductDeclarationPerObject(x as Pipe, field)).ToList();
            for (int x = 0; x < fireProtectionPipesResults.Count; x++)
            {
                if (fireProtectionPipesResults[x] == null)
                {
                    BH.Engine.Reflection.Compute.RecordWarning("Fire Protection Pipe object " + mepScope.FireProtectionScope.Pipes[x].BHoM_Guid + " does not contain a valid IEnvironmentalProductDeclaration MaterialFragment.");
                    continue;
                }
                fireProtectionPipesResults[x] = new GlobalWarmingPotentialResult(fireProtectionPipesResults[x].ObjectId, fireProtectionPipesResults[x].ResultCase, fireProtectionPipesResults[x].TimeStep, ObjectScope.FireProtection, ObjectCategory.Pipes, fireProtectionPipesResults[x].EnvironmentalProductDeclaration, (fireProtectionPipesResults[x] as GlobalWarmingPotentialResult).GlobalWarmingPotential);
            }
            results.AddRange(fireProtectionPipesResults);

            //FireProtectionScope Sprinklers
            List<LifeCycleAssessmentElementResult> fireProtectionSprinklersResults = mepScope.FireProtectionScope.Sprinklers.Select(x => EvaluateEnvironmentalProductDeclarationPerObject(x as BHoMObject, field)).ToList();
            for (int x = 0; x < fireProtectionSprinklersResults.Count; x++)
            {
                if (fireProtectionSprinklersResults[x] == null)
                {
                    BH.Engine.Reflection.Compute.RecordWarning("Sprinkler object " + mepScope.FireProtectionScope.Sprinklers[x].BHoM_Guid + " does not contain a valid IEnvironmentalProductDeclaration MaterialFragment.");
                    continue;
                }
                fireProtectionSprinklersResults[x] = new GlobalWarmingPotentialResult(fireProtectionSprinklersResults[x].ObjectId, fireProtectionSprinklersResults[x].ResultCase, fireProtectionSprinklersResults[x].TimeStep, ObjectScope.FireProtection, ObjectCategory.Sprinklers, fireProtectionSprinklersResults[x].EnvironmentalProductDeclaration, (fireProtectionSprinklersResults[x] as GlobalWarmingPotentialResult).GlobalWarmingPotential);
            }
            results.AddRange(fireProtectionSprinklersResults);

            //FireProtectionScope AdditionalObjects
            List<LifeCycleAssessmentElementResult> additionalFireProtectionObjectsResults = mepScope.FireProtectionScope.AdditionalObjects.Select(x => EvaluateEnvironmentalProductDeclarationPerObject(x as IBHoMObject, field)).ToList();
            for (int x = 0; x < additionalFireProtectionObjectsResults.Count; x++)
            {
                if (additionalFireProtectionObjectsResults[x] == null)
                {
                    BH.Engine.Reflection.Compute.RecordWarning("Additional Fire Protection object " + mepScope.FireProtectionScope.AdditionalObjects[x].BHoM_Guid + " does not contain a valid IEnvironmentalProductDeclaration MaterialFragment.");
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
            List<LifeCycleAssessmentElementResult> ceilingResults = tenantImprovementScope.Ceiling.Select(x => EvaluateEnvironmentalProductDeclarationPerObject(x as IBHoMObject, field)).ToList();
            for (int x = 0; x < ceilingResults.Count; x++)
            {
                if (ceilingResults[x] == null)
                {
                    BH.Engine.Reflection.Compute.RecordWarning("Object " + tenantImprovementScope.Ceiling[x].BHoM_Guid + " Within TentantImprovementScope Ceilings does not contain a valid IEnvironmentalProductDeclaration MaterialFragment.");
                    continue;
                }
                ceilingResults[x] = new GlobalWarmingPotentialResult(ceilingResults[x].ObjectId, ceilingResults[x].ResultCase, ceilingResults[x].TimeStep, ObjectScope.TenantImprovement, ObjectCategory.Ceiling, ceilingResults[x].EnvironmentalProductDeclaration, (ceilingResults[x] as GlobalWarmingPotentialResult).GlobalWarmingPotential);
            }
            results.AddRange(ceilingResults);

            //TI Finishes
            List<LifeCycleAssessmentElementResult> finishResults = tenantImprovementScope.Finishes.Select(x => EvaluateEnvironmentalProductDeclarationPerObject(x as IBHoMObject, field)).ToList();
            for (int x = 0; x < finishResults.Count; x++)
            {
                if (finishResults[x] == null)
                {
                    BH.Engine.Reflection.Compute.RecordWarning("Object " + tenantImprovementScope.Finishes[x].BHoM_Guid + " Within TentantImprovementScope Finishes does not contain a valid IEnvironmentalProductDeclaration MaterialFragment.");
                    continue;
                }
                finishResults[x] = new GlobalWarmingPotentialResult(finishResults[x].ObjectId, finishResults[x].ResultCase, finishResults[x].TimeStep, ObjectScope.TenantImprovement, ObjectCategory.Finishes, finishResults[x].EnvironmentalProductDeclaration, (finishResults[x] as GlobalWarmingPotentialResult).GlobalWarmingPotential);
            }
            results.AddRange(finishResults);

            //TI Furniture
            List<LifeCycleAssessmentElementResult> furnitureResults = tenantImprovementScope.Furniture.Select(x => EvaluateEnvironmentalProductDeclarationPerObject(x as IBHoMObject, field)).ToList();
            for (int x = 0; x < furnitureResults.Count; x++)
            {
                if (furnitureResults[x] == null)
                {
                    BH.Engine.Reflection.Compute.RecordWarning("Object " + tenantImprovementScope.Furniture[x].BHoM_Guid + " Within TentantImprovementScope Furniture does not contain a valid IEnvironmentalProductDeclaration MaterialFragment.");
                    continue;
                }
                furnitureResults[x] = new GlobalWarmingPotentialResult(furnitureResults[x].ObjectId, furnitureResults[x].ResultCase, furnitureResults[x].TimeStep, ObjectScope.TenantImprovement, ObjectCategory.Furniture, furnitureResults[x].EnvironmentalProductDeclaration, (furnitureResults[x] as GlobalWarmingPotentialResult).GlobalWarmingPotential);
            }
            results.AddRange(furnitureResults);

            //TI Interior Doors
            List<LifeCycleAssessmentElementResult> intDoorsResults = tenantImprovementScope.InteriorDoors.Select(x => EvaluateEnvironmentalProductDeclarationPerObject(x as IBHoMObject, field)).ToList();
            for (int x = 0; x < intDoorsResults.Count; x++)
            {
                if (intDoorsResults[x] == null)
                {
                    BH.Engine.Reflection.Compute.RecordWarning("Object " + tenantImprovementScope.InteriorDoors[x].BHoM_Guid + " Within TentantImprovementScope InteriorDoors does not contain a valid IEnvironmentalProductDeclaration MaterialFragment.");
                    continue;
                }
                intDoorsResults[x] = new GlobalWarmingPotentialResult(intDoorsResults[x].ObjectId, intDoorsResults[x].ResultCase, intDoorsResults[x].TimeStep, ObjectScope.TenantImprovement, ObjectCategory.InteriorDoor, intDoorsResults[x].EnvironmentalProductDeclaration, (intDoorsResults[x] as GlobalWarmingPotentialResult).GlobalWarmingPotential);
            }
            results.AddRange(intDoorsResults);

            //TI Interior Glazing
            List<LifeCycleAssessmentElementResult> intGlazingResults = tenantImprovementScope.InteriorGlazing.Select(x => EvaluateEnvironmentalProductDeclarationPerObject(x as IBHoMObject, field)).ToList();
            for (int x = 0; x < intGlazingResults.Count; x++)
            {
                if (intGlazingResults[x] == null)
                {
                    BH.Engine.Reflection.Compute.RecordWarning("Object " + tenantImprovementScope.InteriorGlazing[x].BHoM_Guid + " Within TentantImprovementScope InteriorGlazing does not contain a valid IEnvironmentalProductDeclaration MaterialFragment.");
                    continue;
                }
                intGlazingResults[x] = new GlobalWarmingPotentialResult(intGlazingResults[x].ObjectId, intGlazingResults[x].ResultCase, intGlazingResults[x].TimeStep, ObjectScope.TenantImprovement, ObjectCategory.InteriorGlazing, intGlazingResults[x].EnvironmentalProductDeclaration, (intGlazingResults[x] as GlobalWarmingPotentialResult).GlobalWarmingPotential);
            }
            results.AddRange(intGlazingResults);

            //TI Partition Walls
            List<LifeCycleAssessmentElementResult> partWallsResults = tenantImprovementScope.PartitionWalls.Select(x => EvaluateEnvironmentalProductDeclarationPerObject(x as IBHoMObject, field)).ToList();
            for (int x = 0; x < partWallsResults.Count; x++)
            {
                if (partWallsResults[x] == null)
                {
                    BH.Engine.Reflection.Compute.RecordWarning("Object " + tenantImprovementScope.PartitionWalls[x].BHoM_Guid + " Within TentantImprovementScope PartitionWalls does not contain a valid IEnvironmentalProductDeclaration MaterialFragment.");
                    continue;
                }
                partWallsResults[x] = new GlobalWarmingPotentialResult(partWallsResults[x].ObjectId, partWallsResults[x].ResultCase, partWallsResults[x].TimeStep, ObjectScope.TenantImprovement, ObjectCategory.PartitionWall, partWallsResults[x].EnvironmentalProductDeclaration, (partWallsResults[x] as GlobalWarmingPotentialResult).GlobalWarmingPotential);
            }
            results.AddRange(partWallsResults);

            //TI AdditionalObjects
            List<LifeCycleAssessmentElementResult> additionalObjectsResults = tenantImprovementScope.AdditionalObjects.Select(x => EvaluateEnvironmentalProductDeclarationPerObject(x as IBHoMObject, field)).ToList();
            for (int x = 0; x < additionalObjectsResults.Count; x++)
            {
                if (additionalObjectsResults[x] == null)
                {
                    BH.Engine.Reflection.Compute.RecordWarning("Object " + tenantImprovementScope.AdditionalObjects[x].BHoM_Guid + " Within TenantImprovementScope AdditionalObjects does not contain a valid IEnvironmentalProductDeclaration MaterialFragment.");
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
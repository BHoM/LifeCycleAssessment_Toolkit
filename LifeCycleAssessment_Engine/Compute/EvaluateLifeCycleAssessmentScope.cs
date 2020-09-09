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
using BH.oM.Quantities.Attributes;
using BH.oM.LifeCycleAssessment.MaterialFragments;
using BH.oM.Physical.Elements;
using BH.oM.Dimensional;

using BH.Engine.Base;
using BH.Engine.Reflection;
using BH.Engine.Spatial;
using BH.Engine.Matter;
using BH.oM.LifeCycleAssessment.Results;
using BH.oM.Structure.Requests;
using System.Security.Cryptography.X509Certificates;
using System.Net;

namespace BH.Engine.LifeCycleAssessment
{
    public static partial class Compute
    {
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

            //MEPScope Batteries
            List<LifeCycleAssessmentElementResult> batteriesResults = mepScope.Batteries.Select(x => EvaluateEnvironmentalProductDeclarationPerObject(x as IBHoMObject, field)).ToList();
            for (int x = 0; x < batteriesResults.Count; x++)
            {
                if (batteriesResults[x] == null)
                {
                    BH.Engine.Reflection.Compute.RecordWarning("Object " + mepScope.Batteries[x].BHoM_Guid + " Within MEPScope Batteries does not contain a valid IEnvironmentalProductDeclaration MaterialFragment.");
                    continue;
                }
                batteriesResults[x] = new GlobalWarmingPotentialResult(batteriesResults[x].ObjectId, batteriesResults[x].ResultCase, batteriesResults[x].TimeStep, ObjectScope.MEP, ObjectCategory.Battery, batteriesResults[x].EnvironmentalProductDeclaration, (batteriesResults[x] as GlobalWarmingPotentialResult).GlobalWarmingPotential);
            }
            results.AddRange(batteriesResults);

            //MEPScope Conduit
            List<LifeCycleAssessmentElementResult> conduitResults = mepScope.Conduit.Select(x => EvaluateEnvironmentalProductDeclarationPerObject(x as IBHoMObject, field)).ToList();
            for (int x = 0; x < conduitResults.Count; x++)
            {
                if (conduitResults[x] == null)
                {
                    BH.Engine.Reflection.Compute.RecordWarning("Object " + mepScope.Conduit[x].BHoM_Guid + " Within MEPScope Conduit does not contain a valid IEnvironmentalProductDeclaration MaterialFragment.");
                    continue;
                }
                conduitResults[x] = new GlobalWarmingPotentialResult(conduitResults[x].ObjectId, conduitResults[x].ResultCase, conduitResults[x].TimeStep, ObjectScope.MEP, ObjectCategory.Conduit, conduitResults[x].EnvironmentalProductDeclaration, (conduitResults[x] as GlobalWarmingPotentialResult).GlobalWarmingPotential);
            }
            results.AddRange(conduitResults);

            //MEPScope Ductwork
            List<LifeCycleAssessmentElementResult> ductResults = mepScope.Ductwork.Select(x => EvaluateEnvironmentalProductDeclarationPerObject(x as IBHoMObject, field)).ToList();
            for (int x = 0; x < ductResults.Count; x++)
            {
                if (ductResults[x] == null)
                {
                    BH.Engine.Reflection.Compute.RecordWarning("Object " + mepScope.Ductwork[x].BHoM_Guid + " Within MEPScope Conduit does not contain a valid IEnvironmentalProductDeclaration MaterialFragment.");
                    continue;
                }
                ductResults[x] = new GlobalWarmingPotentialResult(ductResults[x].ObjectId, ductResults[x].ResultCase, ductResults[x].TimeStep, ObjectScope.MEP, ObjectCategory.Ductwork, ductResults[x].EnvironmentalProductDeclaration, (ductResults[x] as GlobalWarmingPotentialResult).GlobalWarmingPotential);
            }
            results.AddRange(ductResults);

            //MEPScope Equipment
            List<LifeCycleAssessmentElementResult> equipmentResults = mepScope.Equipment.Select(x => EvaluateEnvironmentalProductDeclarationPerObject(x as IBHoMObject, field)).ToList();
            for (int x = 0; x < equipmentResults.Count; x++)
            {
                if (equipmentResults[x] == null)
                {
                    BH.Engine.Reflection.Compute.RecordWarning("Object " + mepScope.Equipment[x].BHoM_Guid + " Within MEPScope Equipment does not contain a valid IEnvironmentalProductDeclaration MaterialFragment.");
                    continue;
                }
                equipmentResults[x] = new GlobalWarmingPotentialResult(equipmentResults[x].ObjectId, equipmentResults[x].ResultCase, equipmentResults[x].TimeStep, ObjectScope.MEP, ObjectCategory.Equipment, equipmentResults[x].EnvironmentalProductDeclaration, (equipmentResults[x] as GlobalWarmingPotentialResult).GlobalWarmingPotential);
            }
            results.AddRange(equipmentResults);

            //MEPScope Generator
            List<LifeCycleAssessmentElementResult> generatorResults = mepScope.Generators.Select(x => EvaluateEnvironmentalProductDeclarationPerObject(x as IBHoMObject, field)).ToList();
            for (int x = 0; x < generatorResults.Count; x++)
            {
                if (generatorResults[x] == null)
                {
                    BH.Engine.Reflection.Compute.RecordWarning("Object " + mepScope.Generators[x].BHoM_Guid + " Within MEPScope Generators does not contain a valid IEnvironmentalProductDeclaration MaterialFragment.");
                    continue;
                }
                generatorResults[x] = new GlobalWarmingPotentialResult(generatorResults[x].ObjectId, generatorResults[x].ResultCase, generatorResults[x].TimeStep, ObjectScope.MEP, ObjectCategory.Generator, generatorResults[x].EnvironmentalProductDeclaration, (generatorResults[x] as GlobalWarmingPotentialResult).GlobalWarmingPotential);
            }
            results.AddRange(generatorResults);

            //MEPScope Lighting
            List<LifeCycleAssessmentElementResult> lightingResults = mepScope.Lighting.Select(x => EvaluateEnvironmentalProductDeclarationPerObject(x as IBHoMObject, field)).ToList();
            for (int x = 0; x < lightingResults.Count; x++)
            {
                if (lightingResults[x] == null)
                {
                    BH.Engine.Reflection.Compute.RecordWarning("Object " + mepScope.Lighting[x].BHoM_Guid + " Within MEPScope Lighting does not contain a valid IEnvironmentalProductDeclaration MaterialFragment.");
                    continue;
                }
                lightingResults[x] = new GlobalWarmingPotentialResult(lightingResults[x].ObjectId, lightingResults[x].ResultCase, lightingResults[x].TimeStep, ObjectScope.MEP, ObjectCategory.Lighting, lightingResults[x].EnvironmentalProductDeclaration, (lightingResults[x] as GlobalWarmingPotentialResult).GlobalWarmingPotential);
            }
            results.AddRange(lightingResults);

            //MEPScope Piping
            List<LifeCycleAssessmentElementResult> pipingResults = mepScope.Piping.Select(x => EvaluateEnvironmentalProductDeclarationPerObject(x as IBHoMObject, field)).ToList();
            for (int x = 0; x < pipingResults.Count; x++)
            {
                if (pipingResults[x] == null)
                {
                    BH.Engine.Reflection.Compute.RecordWarning("Object " + mepScope.Piping[x].BHoM_Guid + " Within MEPScope Piping does not contain a valid IEnvironmentalProductDeclaration MaterialFragment.");
                    continue;
                }
                pipingResults[x] = new GlobalWarmingPotentialResult(pipingResults[x].ObjectId, pipingResults[x].ResultCase, pipingResults[x].TimeStep, ObjectScope.MEP, ObjectCategory.Piping, pipingResults[x].EnvironmentalProductDeclaration, (pipingResults[x] as GlobalWarmingPotentialResult).GlobalWarmingPotential);
            }
            results.AddRange(pipingResults);

            //MEPScope Wiring
            List<LifeCycleAssessmentElementResult> wiringResults = mepScope.Wiring.Select(x => EvaluateEnvironmentalProductDeclarationPerObject(x as IBHoMObject, field)).ToList();
            for (int x = 0; x < wiringResults.Count; x++)
            {
                if (wiringResults[x] == null)
                {
                    BH.Engine.Reflection.Compute.RecordWarning("Object " + mepScope.Wiring[x].BHoM_Guid + " Within MEPScope Wiring does not contain a valid IEnvironmentalProductDeclaration MaterialFragment.");
                    continue;
                }
                wiringResults[x] = new GlobalWarmingPotentialResult(wiringResults[x].ObjectId, wiringResults[x].ResultCase, wiringResults[x].TimeStep, ObjectScope.MEP, ObjectCategory.Wiring, wiringResults[x].EnvironmentalProductDeclaration, (wiringResults[x] as GlobalWarmingPotentialResult).GlobalWarmingPotential);
            }
            results.AddRange(wiringResults);

            //MEPScope AdditionalObjects
            List<LifeCycleAssessmentElementResult> additionalObjectsResults = mepScope.AdditionalObjects.Select(x => EvaluateEnvironmentalProductDeclarationPerObject(x as IBHoMObject, field)).ToList();
            for (int x = 0; x < additionalObjectsResults.Count; x++)
            {
                if (additionalObjectsResults[x] == null)
                {
                    BH.Engine.Reflection.Compute.RecordWarning("Object " + mepScope.AdditionalObjects[x].BHoM_Guid + " Within MEPScope AdditionalObjects does not contain a valid IEnvironmentalProductDeclaration MaterialFragment.");
                    continue;
                }
                additionalObjectsResults[x] = new GlobalWarmingPotentialResult(additionalObjectsResults[x].ObjectId, additionalObjectsResults[x].ResultCase, additionalObjectsResults[x].TimeStep, ObjectScope.MEP, ObjectCategory.AdditionalObjects, additionalObjectsResults[x].EnvironmentalProductDeclaration, (additionalObjectsResults[x] as GlobalWarmingPotentialResult).GlobalWarmingPotential);
            }
            results.AddRange(additionalObjectsResults);

            return results;
        }

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
    }
}
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

using BH.oM.Base;
using BH.oM.LifeCycleAssessment;
using BH.oM.LifeCycleAssessment.Results;
using BH.oM.Reflection.Attributes;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

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
        [Input("phases", "Provide phases of life you wish to evaluate. Phases of life must be documented within EPDs for this method to work.")]
        [Input("exactMatch", "If true, the evaluation method will force an exact LCA phase match to solve for.")]
        [Output("result", "The LifeCycleElementResult that contains structures scope specific data, total results, and results per element.")]
        [PreviousVersion("4.2", "BH.Engine.LifeCycleAssessment.Compute.EvaluateLifeCycleAssessmentScope(BH.oM.LifeCycleAssessment.StructuresScope, BH.oM.LifeCycleAssessment.EnvironmentalProductDeclarationField)")]
        public static List<LifeCycleAssessmentElementResult> EvaluateLifeCycleAssessmentScope(this StructuresScope structuresScope, EnvironmentalProductDeclarationField field, List<LifeCycleAssessmentPhases> phases, bool exactMatch = false)
        {
            if (structuresScope.IsValid() == false)
            {
                BH.Engine.Reflection.Compute.RecordWarning("Your StructuresScope is incomplete, please provide all types of objects where possible.");
            }

            List<LifeCycleAssessmentElementResult> results = new List<LifeCycleAssessmentElementResult>();

            //StructuresScope Beams
            List<LifeCycleAssessmentElementResult> beamResults = structuresScope.Beams.Select(x => EvaluateEnvironmentalProductDeclaration(x, phases, field, exactMatch)).ToList();

            for (int x = 0; x < beamResults.Count; x++)
            {
                if (beamResults[x] == null)
                {
                    BH.Engine.Reflection.Compute.RecordWarning("Structural object " + ((IBHoMObject)structuresScope.Beams[x]).BHoM_Guid + " does not contain a valid EPD.");
                    continue;
                }
                beamResults[x] = new EnvironmentalMetricResult(beamResults[x].ObjectId, beamResults[x].ResultCase, beamResults[x].TimeStep, ObjectScope.Structure, ObjectCategory.Beam, phases, beamResults[x].EnvironmentalProductDeclaration, (beamResults[x] as EnvironmentalMetricResult).Quantity, field);
            }
            results.AddRange(beamResults);

            //StructuresScope Columns
            List<LifeCycleAssessmentElementResult> columnsResults = structuresScope.Columns.Select(x => EvaluateEnvironmentalProductDeclaration(x, phases, field, exactMatch)).ToList();
            for (int x = 0; x < columnsResults.Count; x++)
            {
                if (columnsResults[x] == null)
                {
                    BH.Engine.Reflection.Compute.RecordWarning("Structural object " + ((IBHoMObject)structuresScope.Columns[x]).BHoM_Guid + " does not contain a valid EPD.");
                    continue;
                }
                columnsResults[x] = new EnvironmentalMetricResult(columnsResults[x].ObjectId, columnsResults[x].ResultCase, columnsResults[x].TimeStep, ObjectScope.Structure, ObjectCategory.Column, phases, columnsResults[x].EnvironmentalProductDeclaration, (columnsResults[x] as EnvironmentalMetricResult).Quantity, field);
            }
            results.AddRange(columnsResults);

            //StructuresScope Slabs
            List<LifeCycleAssessmentElementResult> slabsResults = structuresScope.Slabs.Select(x => EvaluateEnvironmentalProductDeclaration(x, phases, field, exactMatch)).ToList();
            for (int x = 0; x < slabsResults.Count; x++)
            {
                if (slabsResults[x] == null)
                {
                    BH.Engine.Reflection.Compute.RecordWarning("Structural object " + ((IBHoMObject)structuresScope.Slabs[x]).BHoM_Guid + " does not contain a valid EPD.");
                    continue;
                }

                slabsResults[x] = new EnvironmentalMetricResult(slabsResults[x].ObjectId, slabsResults[x].ResultCase, slabsResults[x].TimeStep, ObjectScope.Structure, ObjectCategory.Slab, phases, slabsResults[x].EnvironmentalProductDeclaration, (slabsResults[x] as EnvironmentalMetricResult).Quantity, field);
            }
            results.AddRange(slabsResults);

            //StructuresScope Core Walls
            List<LifeCycleAssessmentElementResult> coreWallsResults = structuresScope.CoreWalls.Select(x => EvaluateEnvironmentalProductDeclaration(x, phases, field, exactMatch)).ToList();
            for (int x = 0; x < coreWallsResults.Count; x++)
            {
                if (coreWallsResults[x] == null)
                {
                    BH.Engine.Reflection.Compute.RecordWarning("Structural object " + ((IBHoMObject)structuresScope.CoreWalls[x]).BHoM_Guid + " does not contain a valid EPD.");
                    continue;
                }
                coreWallsResults[x] = new EnvironmentalMetricResult(coreWallsResults[x].ObjectId, coreWallsResults[x].ResultCase, coreWallsResults[x].TimeStep, ObjectScope.Structure, ObjectCategory.Wall, phases, coreWallsResults[x].EnvironmentalProductDeclaration, (coreWallsResults[x] as EnvironmentalMetricResult).Quantity, field);
            }
            results.AddRange(coreWallsResults);

            //StructuresScope Bracing
            List<LifeCycleAssessmentElementResult> bracingResults = structuresScope.Bracing.Select(x => EvaluateEnvironmentalProductDeclaration(x, phases, field, exactMatch)).ToList();
            for (int x = 0; x < bracingResults.Count; x++)
            {
                if (bracingResults[x] == null)
                {
                    BH.Engine.Reflection.Compute.RecordWarning("Structural object " + ((IBHoMObject)structuresScope.Bracing[x]).BHoM_Guid + " does not contain a valid EPD.");
                    continue;
                }
                bracingResults[x] = new EnvironmentalMetricResult(bracingResults[x].ObjectId, bracingResults[x].ResultCase, bracingResults[x].TimeStep, ObjectScope.Structure, ObjectCategory.Bracing, phases, bracingResults[x].EnvironmentalProductDeclaration, (bracingResults[x] as EnvironmentalMetricResult).Quantity, field);
            }
            results.AddRange(bracingResults);

            //StructuresScope AdditionalObjects
            List<LifeCycleAssessmentElementResult> additionalObjectsResults = structuresScope.AdditionalObjects.Select(x => EvaluateEnvironmentalProductDeclaration(x, phases, field, exactMatch)).ToList();
            for (int x = 0; x < additionalObjectsResults.Count; x++)
            {
                if (additionalObjectsResults[x] == null)
                {
                    BH.Engine.Reflection.Compute.RecordWarning("Structural object " + ((IBHoMObject)structuresScope.AdditionalObjects[x]).BHoM_Guid + " does not contain a valid EPD.");
                    continue;
                }
                additionalObjectsResults[x] = new EnvironmentalMetricResult(additionalObjectsResults[x].ObjectId, additionalObjectsResults[x].ResultCase, additionalObjectsResults[x].TimeStep, ObjectScope.Structure, ObjectCategory.AdditionalObjects, phases, additionalObjectsResults[x].EnvironmentalProductDeclaration, (additionalObjectsResults[x] as EnvironmentalMetricResult).Quantity, field);
            }
            results.AddRange(additionalObjectsResults);

            return results;
        }

        /***************************************************/

        [Description("This method calls the appropriate compute method per object within the FoundationsScope and returns results.")]
        [Input("foundationsScope", "This is a life cycle assessment scope object which contains all specified objects along with their associated EPD MaterialFragment data.")]
        [Input("field", "This is the desired field you would like to evaluate. Notice that not all material datasets will contain information for all metrics. A holistic GWP evaluation is currently the only standard evaluation possible with all provided datasets.")]
        [Input("phases", "Provide phases of life you wish to evaluate. Phases of life must be documented within EPDs for this method to work.")]
        [Input("exactMatch", "If true, the evaluation method will force an exact LCA phase match to solve for.")]
        [Output("result", "The LifeCycleElementResult that contains foundations scope specific data, total results, and results per element.")]
        [PreviousVersion("4.2", "BH.Engine.LifeCycleAssessment.Compute.EvaluateLifeCycleAssessmentScope(BH.oM.LifeCycleAssessment.FoundationsScope, BH.oM.LifeCycleAssessment.EnvironmentalProductDeclarationField)")]
        public static List<LifeCycleAssessmentElementResult> EvaluateLifeCycleAssessmentScope(this FoundationsScope foundationsScope, EnvironmentalProductDeclarationField field, List<LifeCycleAssessmentPhases> phases, bool exactMatch = false)
        {
            if (foundationsScope.IsValid() == false)
            {
                BH.Engine.Reflection.Compute.RecordWarning("Your FoundationsScope is incomplete, please provide all types of objects where possible.");
            }

            List<LifeCycleAssessmentElementResult> results = new List<LifeCycleAssessmentElementResult>();

            //FoundationsScope Footings
            List<LifeCycleAssessmentElementResult> columnResults = foundationsScope.Columns.Select(x => EvaluateEnvironmentalProductDeclaration(x, phases, field, exactMatch)).ToList();
            for (int x = 0; x < columnResults.Count; x++)
            {
                if (columnResults[x] == null)
                {
                    BH.Engine.Reflection.Compute.RecordWarning("Foundations object " + ((IBHoMObject)foundationsScope.Columns[x]).BHoM_Guid + " does not contain a valid EPD.");
                    continue;
                }
                columnResults[x] = new EnvironmentalMetricResult(columnResults[x].ObjectId, columnResults[x].ResultCase, columnResults[x].TimeStep, ObjectScope.Foundation, ObjectCategory.Column, phases, columnResults[x].EnvironmentalProductDeclaration, (columnResults[x] as EnvironmentalMetricResult).Quantity, field);
            }
            results.AddRange(columnResults);

            //FoundationsScope Footings
            List<LifeCycleAssessmentElementResult> footingsResults = foundationsScope.Footings.Select(x => EvaluateEnvironmentalProductDeclaration(x, phases, field, exactMatch)).ToList();
            for (int x = 0; x < footingsResults.Count; x++)
            {
                if (footingsResults[x] == null)
                {
                    BH.Engine.Reflection.Compute.RecordWarning("Foundations object " + ((IBHoMObject)foundationsScope.Footings[x]).BHoM_Guid + " does not contain a valid EPD.");
                    continue;
                }
                footingsResults[x] = new EnvironmentalMetricResult(footingsResults[x].ObjectId, footingsResults[x].ResultCase, footingsResults[x].TimeStep, ObjectScope.Foundation, ObjectCategory.Footing, phases, footingsResults[x].EnvironmentalProductDeclaration, (footingsResults[x] as EnvironmentalMetricResult).Quantity, field);
            }
            results.AddRange(footingsResults);

            //FoundationsScope Piles
            List<LifeCycleAssessmentElementResult> pilesResults = foundationsScope.Piles.Select(x => EvaluateEnvironmentalProductDeclaration(x, phases, field, exactMatch)).ToList();
            for (int x = 0; x < pilesResults.Count; x++)
            {
                if (pilesResults[x] == null)
                {
                    BH.Engine.Reflection.Compute.RecordWarning("Foundations object " + ((IBHoMObject)foundationsScope.Piles[x]).BHoM_Guid + " does not contain a valid EPD.");
                    continue;
                }
                pilesResults[x] = new EnvironmentalMetricResult(pilesResults[x].ObjectId, pilesResults[x].ResultCase, pilesResults[x].TimeStep, ObjectScope.Foundation, ObjectCategory.Pile, phases, pilesResults[x].EnvironmentalProductDeclaration, (pilesResults[x] as EnvironmentalMetricResult).Quantity, field);
            }
            results.AddRange(pilesResults);

            //FoundationsScope Walls
            List<LifeCycleAssessmentElementResult> wallsResults = foundationsScope.Walls.Select(x => EvaluateEnvironmentalProductDeclaration(x, phases, field, exactMatch)).ToList();
            for (int x = 0; x < wallsResults.Count; x++)
            {
                if (wallsResults[x] == null)
                {
                    BH.Engine.Reflection.Compute.RecordWarning("Foundations object " + ((IBHoMObject)foundationsScope.Walls[x]).BHoM_Guid + " does not contain a valid EPD.");
                    continue;
                }
                wallsResults[x] = new EnvironmentalMetricResult(wallsResults[x].ObjectId, wallsResults[x].ResultCase, wallsResults[x].TimeStep, ObjectScope.Foundation, ObjectCategory.Wall, phases, wallsResults[x].EnvironmentalProductDeclaration, (wallsResults[x] as EnvironmentalMetricResult).Quantity, field);
            }
            results.AddRange(wallsResults);

            //FoundationsScope Slabs
            List<LifeCycleAssessmentElementResult> fndSlabsResults = foundationsScope.Slabs.Select(x => EvaluateEnvironmentalProductDeclaration(x, phases, field, exactMatch)).ToList();
            for (int x = 0; x < fndSlabsResults.Count; x++)
            {
                if (fndSlabsResults[x] == null)
                {
                    BH.Engine.Reflection.Compute.RecordWarning("Foundations object " + ((IBHoMObject)foundationsScope.Slabs[x]).BHoM_Guid + " does not contain a valid EPD.");
                    continue;
                }
                fndSlabsResults[x] = new EnvironmentalMetricResult(fndSlabsResults[x].ObjectId, fndSlabsResults[x].ResultCase, fndSlabsResults[x].TimeStep, ObjectScope.Foundation, ObjectCategory.Slab, phases, fndSlabsResults[x].EnvironmentalProductDeclaration, (fndSlabsResults[x] as EnvironmentalMetricResult).Quantity, field);
            }
            results.AddRange(fndSlabsResults);

            //FoundationsScope GradeBeams
            List<LifeCycleAssessmentElementResult> gradeBeamsResults = foundationsScope.GradeBeams.Select(x => EvaluateEnvironmentalProductDeclaration(x, phases, field, exactMatch)).ToList();
            for (int x = 0; x < gradeBeamsResults.Count; x++)
            {
                if (gradeBeamsResults[x] == null)
                {
                    BH.Engine.Reflection.Compute.RecordWarning("Foundations object " + ((IBHoMObject)foundationsScope.GradeBeams[x]).BHoM_Guid + " does not contain a valid EPD.");
                    continue;
                }
                gradeBeamsResults[x] = new EnvironmentalMetricResult(gradeBeamsResults[x].ObjectId, gradeBeamsResults[x].ResultCase, gradeBeamsResults[x].TimeStep, ObjectScope.Foundation, ObjectCategory.GradeBeam, phases, gradeBeamsResults[x].EnvironmentalProductDeclaration, (gradeBeamsResults[x] as EnvironmentalMetricResult).Quantity, field);
            }
            results.AddRange(gradeBeamsResults);

            //FoundationScope AdditionalObjects
            List<LifeCycleAssessmentElementResult> additionalObjectsResults = foundationsScope.AdditionalObjects.Select(x => EvaluateEnvironmentalProductDeclaration(x, phases, field, exactMatch)).ToList();
            for (int x = 0; x < additionalObjectsResults.Count; x++)
            {
                if (additionalObjectsResults[x] == null)
                {
                    BH.Engine.Reflection.Compute.RecordWarning("Foundations object " + ((IBHoMObject)foundationsScope.AdditionalObjects[x]).BHoM_Guid + " does not contain a valid EPD.");
                    continue;
                }
                additionalObjectsResults[x] = new EnvironmentalMetricResult(additionalObjectsResults[x].ObjectId, additionalObjectsResults[x].ResultCase, additionalObjectsResults[x].TimeStep, ObjectScope.Foundation, ObjectCategory.AdditionalObjects, phases, additionalObjectsResults[x].EnvironmentalProductDeclaration, (additionalObjectsResults[x] as EnvironmentalMetricResult).Quantity, field);
            }
            results.AddRange(additionalObjectsResults);

            return results;
        }

        /***************************************************/

        [Description("This method calls the appropriate compute method per object within the EnclosuresScope and returns results.")]
        [Input("enclosuresScope", "This is a life cycle assessment scope object which contains all specified objects along with their associated EPD MaterialFragment data.")]
        [Input("field", "This is the desired field you would like to evaluate. Notice that not all material datasets will contain information for all metrics. A holistic GWP evaluation is currently the only standard evaluation possible with all provided datasets.")]
        [Input("phases", "Provide phases of life you wish to evaluate. Phases of life must be documented within EPDs for this method to work.")]
        [Input("exactMatch", "If true, the evaluation method will force an exact LCA phase match to solve for.")]
        [Output("result", "The LifeCycleElementResult that contains enclosures scope specific data, total results, and results per element.")]
        [PreviousVersion("4.2", "BH.Engine.LifeCycleAssessment.Compute.EvaluateLifeCycleAssessmentScope(BH.oM.LifeCycleAssessment.EnclosuresScope, BH.oM.LifeCycleAssessment.EnvironmentalProductDeclarationField)")]
        public static List<LifeCycleAssessmentElementResult> EvaluateLifeCycleAssessmentScope(this EnclosuresScope enclosuresScope, EnvironmentalProductDeclarationField field, List<LifeCycleAssessmentPhases> phases, bool exactMatch = false)
        {
            if (enclosuresScope.IsValid() == false)
            {
                BH.Engine.Reflection.Compute.RecordWarning("Your EnclosuresScope is incomplete, please provide all types of objects where possible.");
            }

            List<LifeCycleAssessmentElementResult> results = new List<LifeCycleAssessmentElementResult>();

            //EnclosuresScope Walls
            List<LifeCycleAssessmentElementResult> enclWallsResults = enclosuresScope.Walls.Select(x => EvaluateEnvironmentalProductDeclaration(x, phases, field, exactMatch)).ToList();
            for (int x = 0; x < enclWallsResults.Count; x++)
            {
                if (enclWallsResults[x] == null)
                {
                    BH.Engine.Reflection.Compute.RecordWarning("Enclosures object " + ((IBHoMObject)enclosuresScope.Walls[x]).BHoM_Guid + " does not contain a valid EPD.");
                    continue;
                }
                enclWallsResults[x] = new EnvironmentalMetricResult(enclWallsResults[x].ObjectId, enclWallsResults[x].ResultCase, enclWallsResults[x].TimeStep, ObjectScope.Enclosure, ObjectCategory.Wall, phases, enclWallsResults[x].EnvironmentalProductDeclaration, (enclWallsResults[x] as EnvironmentalMetricResult).Quantity, field);
            }
            results.AddRange(enclWallsResults);

            //EnclosuresScope CurtainWalls
            List<LifeCycleAssessmentElementResult> curtainWallsResults = enclosuresScope.CurtainWalls.Select(x => EvaluateEnvironmentalProductDeclaration(x, phases, field, exactMatch)).ToList();
            for (int x = 0; x < curtainWallsResults.Count; x++)
            {
                if (curtainWallsResults[x] == null)
                {
                    BH.Engine.Reflection.Compute.RecordWarning("Enclosures object " + ((IBHoMObject)enclosuresScope.CurtainWalls[x]).BHoM_Guid + " does not contain a valid EPD.");
                    continue;
                }
                curtainWallsResults[x] = new EnvironmentalMetricResult(curtainWallsResults[x].ObjectId, curtainWallsResults[x].ResultCase, curtainWallsResults[x].TimeStep, ObjectScope.Enclosure, ObjectCategory.CurtainWall, phases, curtainWallsResults[x].EnvironmentalProductDeclaration, (curtainWallsResults[x] as EnvironmentalMetricResult).Quantity, field);
            }
            results.AddRange(curtainWallsResults);

            //EnclosuresScope Windows
            List<LifeCycleAssessmentElementResult> windowsResults = enclosuresScope.Windows.Select(x => EvaluateEnvironmentalProductDeclaration(x, phases, field, exactMatch)).ToList();
            for (int x = 0; x < windowsResults.Count; x++)
            {
                if (windowsResults[x] == null)
                {
                    BH.Engine.Reflection.Compute.RecordWarning("Enclosures object " + ((IBHoMObject)enclosuresScope.Windows[x]).BHoM_Guid + " does not contain a valid EPD.");
                    continue;
                }
                windowsResults[x] = new EnvironmentalMetricResult(windowsResults[x].ObjectId, windowsResults[x].ResultCase, windowsResults[x].TimeStep, ObjectScope.Enclosure, ObjectCategory.Window, phases, windowsResults[x].EnvironmentalProductDeclaration, (windowsResults[x] as EnvironmentalMetricResult).Quantity, field);
            }
            results.AddRange(windowsResults);

            //EnclosuresScope Doors
            List<LifeCycleAssessmentElementResult> doorsResults = enclosuresScope.Doors.Select(x => EvaluateEnvironmentalProductDeclaration(x, phases, field, exactMatch)).ToList();
            for (int x = 0; x < doorsResults.Count; x++)
            {
                if (doorsResults[x] == null)
                {
                    BH.Engine.Reflection.Compute.RecordWarning("Enclosures object " + ((IBHoMObject)enclosuresScope.Doors[x]).BHoM_Guid + " does not contain a valid EPD.");
                    continue;
                }
                doorsResults[x] = new EnvironmentalMetricResult(doorsResults[x].ObjectId, doorsResults[x].ResultCase, doorsResults[x].TimeStep, ObjectScope.Enclosure, ObjectCategory.Wall, phases, doorsResults[x].EnvironmentalProductDeclaration, (doorsResults[x] as EnvironmentalMetricResult).Quantity, field);
            }
            results.AddRange(doorsResults);

            //Enclosures AdditionalObjects
            List<LifeCycleAssessmentElementResult> additionalObjectsResults = enclosuresScope.AdditionalObjects.Select(x => EvaluateEnvironmentalProductDeclaration(x, phases, field, exactMatch)).ToList();
            for (int x = 0; x < additionalObjectsResults.Count; x++)
            {
                if (additionalObjectsResults[x] == null)
                {
                    BH.Engine.Reflection.Compute.RecordWarning("Enclosures object " + ((IBHoMObject)enclosuresScope.AdditionalObjects[x]).BHoM_Guid + " does not contain a valid EPD.");
                    continue;
                }
                additionalObjectsResults[x] = new EnvironmentalMetricResult(additionalObjectsResults[x].ObjectId, additionalObjectsResults[x].ResultCase, additionalObjectsResults[x].TimeStep, ObjectScope.Enclosure, ObjectCategory.AdditionalObjects, phases, additionalObjectsResults[x].EnvironmentalProductDeclaration, (additionalObjectsResults[x] as EnvironmentalMetricResult).Quantity, field);
            }
            results.AddRange(additionalObjectsResults);

            return results;
        }

        /***************************************************/

        [Description("This method calls the appropriate compute method per object within the MEPScope and returns results.")]
        [Input("mepScope", "This is a life cycle assessment scope object which contains all specified objects along with their associated EPD MaterialFragment data.")]
        [Input("field", "This is the desired field you would like to evaluate. Notice that not all material datasets will contain information for all metrics. A holistic GWP evaluation is currently the only standard evaluation possible with all provided datasets.")]
        [Input("phases", "Provide phases of life you wish to evaluate. Phases of life must be documented within EPDs for this method to work.")]
        [Input("exactMatch", "If true, the evaluation method will force an exact LCA phase match to solve for.")]
        [Output("result", "The LifeCycleElementResult that contains MEP scope specific data, total results, and results per element.")]
        [PreviousVersion("4.2", "BH.Engine.LifeCycleAssessment.Compute.EvaluateLifeCycleAssessmentScope(BH.oM.LifeCycleAssessment.MEPScope, BH.oM.LifeCycleAssessment.EnvironmentalProductDeclarationField)")]
        public static List<LifeCycleAssessmentElementResult> EvaluateLifeCycleAssessmentScope(this MEPScope mepScope, EnvironmentalProductDeclarationField field, List<LifeCycleAssessmentPhases> phases, bool exactMatch = false)
        {
            if (mepScope.IsValid() == false)
            {
                BH.Engine.Reflection.Compute.RecordWarning("Your MEPScope is incomplete, please provide all types of objects where possible.");
            }

            List<LifeCycleAssessmentElementResult> results = new List<LifeCycleAssessmentElementResult>();

            //MechanicalScope

            //MechanicalScope AirTerminals
            List<LifeCycleAssessmentElementResult> airTerminalResults = mepScope.MechanicalScope.AirTerminals.Select(x => EvaluateEnvironmentalProductDeclaration(x, phases, field, exactMatch)).ToList();
            for (int x = 0; x < airTerminalResults.Count; x++)
            {
                if (airTerminalResults[x] == null)
                {
                    BH.Engine.Reflection.Compute.RecordWarning("Mechanical object " + ((IBHoMObject)mepScope.MechanicalScope.AirTerminals[x]).BHoM_Guid + " does not contain a valid EPD.");
                    continue;
                }
                airTerminalResults[x] = new EnvironmentalMetricResult(airTerminalResults[x].ObjectId, airTerminalResults[x].ResultCase, airTerminalResults[x].TimeStep, ObjectScope.Mechanical, ObjectCategory.AirTerminals, phases, airTerminalResults[x].EnvironmentalProductDeclaration, (airTerminalResults[x] as EnvironmentalMetricResult).Quantity, field);
            }
            results.AddRange(airTerminalResults);

            //MechanicalScope Dampers
            List<LifeCycleAssessmentElementResult> dampterResults = mepScope.MechanicalScope.Dampers.Select(x => EvaluateEnvironmentalProductDeclaration(x, phases, field, exactMatch)).ToList();
            for (int x = 0; x < dampterResults.Count; x++)
            {
                if (dampterResults[x] == null)
                {
                    BH.Engine.Reflection.Compute.RecordWarning("Mechanical object " + ((IBHoMObject)mepScope.MechanicalScope.Dampers[x]).BHoM_Guid + " does not contain a valid EPD.");
                    continue;
                }
                dampterResults[x] = new EnvironmentalMetricResult(dampterResults[x].ObjectId, dampterResults[x].ResultCase, dampterResults[x].TimeStep, ObjectScope.Mechanical, ObjectCategory.Dampers, phases, dampterResults[x].EnvironmentalProductDeclaration, (dampterResults[x] as EnvironmentalMetricResult).Quantity, field);
            }
            results.AddRange(dampterResults);

            //MechanicalScope Ducts
            List<LifeCycleAssessmentElementResult> ductResults = mepScope.MechanicalScope.Ducts.Select(x => EvaluateEnvironmentalProductDeclaration(x, phases, field, exactMatch)).ToList();
            for (int x = 0; x < ductResults.Count; x++)
            {
                if (ductResults[x] == null)
                {
                    BH.Engine.Reflection.Compute.RecordWarning("Mechanical object " + ((IBHoMObject)mepScope.MechanicalScope.Ducts[x]).BHoM_Guid + " does not contain a valid EPD.");
                    continue;
                }
                ductResults[x] = new EnvironmentalMetricResult(ductResults[x].ObjectId, ductResults[x].ResultCase, ductResults[x].TimeStep, ObjectScope.Mechanical, ObjectCategory.Ducts, phases, ductResults[x].EnvironmentalProductDeclaration, (ductResults[x] as EnvironmentalMetricResult).Quantity, field);
            }
            results.AddRange(ductResults);

            //MechanicalScope Equipment
            List<LifeCycleAssessmentElementResult> mechanicalEquipmentResults = mepScope.MechanicalScope.Equipment.Select(x => EvaluateEnvironmentalProductDeclaration(x, phases, field, exactMatch)).ToList();
            for (int x = 0; x < mechanicalEquipmentResults.Count; x++)
            {
                if (mechanicalEquipmentResults[x] == null)
                {
                    BH.Engine.Reflection.Compute.RecordWarning("Mechanical object " + ((IBHoMObject)mepScope.MechanicalScope.Equipment[x]).BHoM_Guid + " does not contain a valid EPD.");
                    continue;
                }
                mechanicalEquipmentResults[x] = new EnvironmentalMetricResult(mechanicalEquipmentResults[x].ObjectId, mechanicalEquipmentResults[x].ResultCase, mechanicalEquipmentResults[x].TimeStep, ObjectScope.Mechanical, ObjectCategory.Equipment, phases, mechanicalEquipmentResults[x].EnvironmentalProductDeclaration, (mechanicalEquipmentResults[x] as EnvironmentalMetricResult).Quantity, field);
            }
            results.AddRange(mechanicalEquipmentResults);

            //MechanicalScope Pipes
            List<LifeCycleAssessmentElementResult> pipingResults = mepScope.MechanicalScope.Pipes.Select(x => EvaluateEnvironmentalProductDeclaration(x, phases, field, exactMatch)).ToList();
            for (int x = 0; x < pipingResults.Count; x++)
            {
                if (pipingResults[x] == null)
                {
                    BH.Engine.Reflection.Compute.RecordWarning("Mechanical object " + ((IBHoMObject)mepScope.MechanicalScope.Pipes[x]).BHoM_Guid + " does not contain a valid EPD.");
                    continue;
                }
                pipingResults[x] = new EnvironmentalMetricResult(pipingResults[x].ObjectId, pipingResults[x].ResultCase, pipingResults[x].TimeStep, ObjectScope.Mechanical, ObjectCategory.Pipes, phases, pipingResults[x].EnvironmentalProductDeclaration, (pipingResults[x] as EnvironmentalMetricResult).Quantity, field);
            }
            results.AddRange(pipingResults);

            //MechanicalScope Refrigerants
            List<LifeCycleAssessmentElementResult> refrigerantsResults = mepScope.MechanicalScope.Refrigerants.Select(x => EvaluateEnvironmentalProductDeclaration(x, phases, field, exactMatch)).ToList();
            for (int x = 0; x < refrigerantsResults.Count; x++)
            {
                if (refrigerantsResults[x] == null)
                {
                    BH.Engine.Reflection.Compute.RecordWarning("Mechanical object " + ((IBHoMObject)mepScope.MechanicalScope.Refrigerants[x]).BHoM_Guid + " does not contain a valid EPD.");
                    continue;
                }
                refrigerantsResults[x] = new EnvironmentalMetricResult(refrigerantsResults[x].ObjectId, refrigerantsResults[x].ResultCase, refrigerantsResults[x].TimeStep, ObjectScope.Mechanical, ObjectCategory.Refrigerants, phases, refrigerantsResults[x].EnvironmentalProductDeclaration, (refrigerantsResults[x] as EnvironmentalMetricResult).Quantity, field);
            }
            results.AddRange(refrigerantsResults);

            //MechanicalScope Tanks
            List<LifeCycleAssessmentElementResult> tankResults = mepScope.MechanicalScope.Tanks.Select(x => EvaluateEnvironmentalProductDeclaration(x, phases, field, exactMatch)).ToList();
            for (int x = 0; x < tankResults.Count; x++)
            {
                if (tankResults[x] == null)
                {
                    BH.Engine.Reflection.Compute.RecordWarning("Mechanical object " + ((IBHoMObject)mepScope.MechanicalScope.Tanks[x]).BHoM_Guid + " does not contain a valid EPD.");
                    continue;
                }
                tankResults[x] = new EnvironmentalMetricResult(tankResults[x].ObjectId, tankResults[x].ResultCase, tankResults[x].TimeStep, ObjectScope.Mechanical, ObjectCategory.Tanks, phases, tankResults[x].EnvironmentalProductDeclaration, (tankResults[x] as EnvironmentalMetricResult).Quantity, field);
            }
            results.AddRange(tankResults);

            //MechanicalScope Valves
            List<LifeCycleAssessmentElementResult> valveResults = mepScope.MechanicalScope.Valves.Select(x => EvaluateEnvironmentalProductDeclaration(x, phases, field, exactMatch)).ToList();
            for (int x = 0; x < valveResults.Count; x++)
            {
                if (valveResults[x] == null)
                {
                    BH.Engine.Reflection.Compute.RecordWarning("Mechanical object " + ((IBHoMObject)mepScope.MechanicalScope.Valves[x]).BHoM_Guid + " does not contain a valid EPD.");
                    continue;
                }
                valveResults[x] = new EnvironmentalMetricResult(valveResults[x].ObjectId, valveResults[x].ResultCase, valveResults[x].TimeStep, ObjectScope.Mechanical, ObjectCategory.Valves, phases, valveResults[x].EnvironmentalProductDeclaration, (valveResults[x] as EnvironmentalMetricResult).Quantity, field);
            }
            results.AddRange(valveResults);

            //MechanicalScope AdditionalObjects
            List<LifeCycleAssessmentElementResult> additionalMechanicalObjectsResults = mepScope.MechanicalScope.AdditionalObjects.Select(x => EvaluateEnvironmentalProductDeclaration(x, phases, field, exactMatch)).ToList();
            for (int x = 0; x < additionalMechanicalObjectsResults.Count; x++)
            {
                if (additionalMechanicalObjectsResults[x] == null)
                {
                    BH.Engine.Reflection.Compute.RecordWarning("Additional Mechanical object " + ((IBHoMObject)mepScope.MechanicalScope.AdditionalObjects[x]).BHoM_Guid + " within MechanicalScope does not contain a valid EPD.");
                    continue;
                }
                additionalMechanicalObjectsResults[x] = new EnvironmentalMetricResult(additionalMechanicalObjectsResults[x].ObjectId, additionalMechanicalObjectsResults[x].ResultCase, additionalMechanicalObjectsResults[x].TimeStep, ObjectScope.Mechanical, ObjectCategory.AdditionalObjects, phases, additionalMechanicalObjectsResults[x].EnvironmentalProductDeclaration, (additionalMechanicalObjectsResults[x] as EnvironmentalMetricResult).Quantity, field);
            }
            results.AddRange(additionalMechanicalObjectsResults);

            //ElectricalScope

            //ElectricalScope Batteries
            List<LifeCycleAssessmentElementResult> batteriesResults = mepScope.ElectricalScope.Batteries.Select(x => EvaluateEnvironmentalProductDeclaration(x, phases, field, exactMatch)).ToList();
            for (int x = 0; x < batteriesResults.Count; x++)
            {
                if (batteriesResults[x] == null)
                {
                    BH.Engine.Reflection.Compute.RecordWarning("Electrical object " + ((IBHoMObject)mepScope.ElectricalScope.Batteries[x]).BHoM_Guid + " does not contain a valid EPD.");
                    continue;
                }
                batteriesResults[x] = new EnvironmentalMetricResult(batteriesResults[x].ObjectId, batteriesResults[x].ResultCase, batteriesResults[x].TimeStep, ObjectScope.Electrical, ObjectCategory.Battery, phases, batteriesResults[x].EnvironmentalProductDeclaration, (batteriesResults[x] as EnvironmentalMetricResult).Quantity, field);
            }
            results.AddRange(batteriesResults);

            //ElectricalScope CableTray
            List<LifeCycleAssessmentElementResult> cableTrayResults = mepScope.ElectricalScope.CableTrays.Select(x => EvaluateEnvironmentalProductDeclaration(x, phases, field, exactMatch)).ToList();
            for (int x = 0; x < cableTrayResults.Count; x++)
            {
                if (cableTrayResults[x] == null)
                {
                    BH.Engine.Reflection.Compute.RecordWarning("Electrical object " + ((IBHoMObject)mepScope.ElectricalScope.CableTrays[x]).BHoM_Guid + " does not contain a valid EPD.");
                    continue;
                }
                cableTrayResults[x] = new EnvironmentalMetricResult(cableTrayResults[x].ObjectId, cableTrayResults[x].ResultCase, cableTrayResults[x].TimeStep, ObjectScope.Electrical, ObjectCategory.CableTray, phases, cableTrayResults[x].EnvironmentalProductDeclaration, (cableTrayResults[x] as EnvironmentalMetricResult).Quantity, field);
            }
            results.AddRange(cableTrayResults);

            //ElectricalScope Conduit
            List<LifeCycleAssessmentElementResult> conduitResults = mepScope.ElectricalScope.Conduit.Select(x => EvaluateEnvironmentalProductDeclaration(x, phases, field, exactMatch)).ToList();
            for (int x = 0; x < conduitResults.Count; x++)
            {
                if (conduitResults[x] == null)
                {
                    BH.Engine.Reflection.Compute.RecordWarning("Electrical object " + ((IBHoMObject)mepScope.ElectricalScope.Conduit[x]).BHoM_Guid + " does not contain a valid EPD.");
                    continue;
                }
                conduitResults[x] = new EnvironmentalMetricResult(conduitResults[x].ObjectId, conduitResults[x].ResultCase, conduitResults[x].TimeStep, ObjectScope.Electrical, ObjectCategory.Conduit, phases, conduitResults[x].EnvironmentalProductDeclaration, (conduitResults[x] as EnvironmentalMetricResult).Quantity, field);
            }
            results.AddRange(conduitResults);

            //ElectricalScope Equipment
            List<LifeCycleAssessmentElementResult> electricalEquipmentResults = mepScope.ElectricalScope.Equipment.Select(x => EvaluateEnvironmentalProductDeclaration(x, phases, field, exactMatch)).ToList();
            for (int x = 0; x < electricalEquipmentResults.Count; x++)
            {
                if (conduitResults[x] == null)
                {
                    BH.Engine.Reflection.Compute.RecordWarning("Electrical object " + ((IBHoMObject)mepScope.ElectricalScope.Equipment[x]).BHoM_Guid + " does not contain a valid EPD.");
                    continue;
                }
                electricalEquipmentResults[x] = new EnvironmentalMetricResult(electricalEquipmentResults[x].ObjectId, electricalEquipmentResults[x].ResultCase, electricalEquipmentResults[x].TimeStep, ObjectScope.Electrical, ObjectCategory.Equipment, phases, electricalEquipmentResults[x].EnvironmentalProductDeclaration, (electricalEquipmentResults[x] as EnvironmentalMetricResult).Quantity, field);
            }
            results.AddRange(electricalEquipmentResults);

            //ElectricalScope FireAlarmDevices
            List<LifeCycleAssessmentElementResult> fireAlarmDeviceResults = mepScope.ElectricalScope.FireAlarmDevices.Select(x => EvaluateEnvironmentalProductDeclaration(x, phases, field, exactMatch)).ToList();
            for (int x = 0; x < fireAlarmDeviceResults.Count; x++)
            {
                if (fireAlarmDeviceResults[x] == null)
                {
                    BH.Engine.Reflection.Compute.RecordWarning("Electrical object " + ((IBHoMObject)mepScope.ElectricalScope.Equipment[x]).BHoM_Guid + " does not contain a valid EPD.");
                    continue;
                }
                fireAlarmDeviceResults[x] = new EnvironmentalMetricResult(fireAlarmDeviceResults[x].ObjectId, fireAlarmDeviceResults[x].ResultCase, fireAlarmDeviceResults[x].TimeStep, ObjectScope.Electrical, ObjectCategory.FireAlarmDevices, phases, fireAlarmDeviceResults[x].EnvironmentalProductDeclaration, (fireAlarmDeviceResults[x] as EnvironmentalMetricResult).Quantity, field);
            }
            results.AddRange(fireAlarmDeviceResults);

            //ElectricalScope Generator
            List<LifeCycleAssessmentElementResult> generatorResults = mepScope.ElectricalScope.Generators.Select(x => EvaluateEnvironmentalProductDeclaration(x, phases, field, exactMatch)).ToList();
            for (int x = 0; x < generatorResults.Count; x++)
            {
                if (generatorResults[x] == null)
                {
                    BH.Engine.Reflection.Compute.RecordWarning("Electrical object " + ((IBHoMObject)mepScope.ElectricalScope.Generators[x]).BHoM_Guid + " does not contain a valid EPD.");
                    continue;
                }
                generatorResults[x] = new EnvironmentalMetricResult(generatorResults[x].ObjectId, generatorResults[x].ResultCase, generatorResults[x].TimeStep, ObjectScope.Electrical, ObjectCategory.Generator, phases, generatorResults[x].EnvironmentalProductDeclaration, (generatorResults[x] as EnvironmentalMetricResult).Quantity, field);
            }
            results.AddRange(generatorResults);

            //ElectricalScope InformationCommunicationDevices
            List<LifeCycleAssessmentElementResult> infoCommsResults = mepScope.ElectricalScope.InformationCommunicationDevices.Select(x => EvaluateEnvironmentalProductDeclaration(x, phases, field, exactMatch)).ToList();
            for (int x = 0; x < infoCommsResults.Count; x++)
            {
                if (infoCommsResults[x] == null)
                {
                    BH.Engine.Reflection.Compute.RecordWarning("Electrical object " + ((IBHoMObject)mepScope.ElectricalScope.InformationCommunicationDevices[x]).BHoM_Guid + " does not contain a valid EPD.");
                    continue;
                }
                infoCommsResults[x] = new EnvironmentalMetricResult(infoCommsResults[x].ObjectId, infoCommsResults[x].ResultCase, infoCommsResults[x].TimeStep, ObjectScope.Electrical, ObjectCategory.InformationCommunicationDevices, phases, infoCommsResults[x].EnvironmentalProductDeclaration, (infoCommsResults[x] as EnvironmentalMetricResult).Quantity, field);
            }
            results.AddRange(infoCommsResults);

            //ElectricalScope Light Fixtures
            List<LifeCycleAssessmentElementResult> lightFixtureResults = mepScope.ElectricalScope.LightFixtures.Select(x => EvaluateEnvironmentalProductDeclaration(x, phases, field, exactMatch)).ToList();
            for (int x = 0; x < lightFixtureResults.Count; x++)
            {
                if (lightFixtureResults[x] == null)
                {
                    BH.Engine.Reflection.Compute.RecordWarning("Electrical object" + ((IBHoMObject)mepScope.ElectricalScope.LightFixtures[x]).BHoM_Guid + " does not contain a valid EPD.");
                    continue;
                }
                lightFixtureResults[x] = new EnvironmentalMetricResult(lightFixtureResults[x].ObjectId, lightFixtureResults[x].ResultCase, lightFixtureResults[x].TimeStep, ObjectScope.Electrical, ObjectCategory.LightFixtures, phases, lightFixtureResults[x].EnvironmentalProductDeclaration, (lightFixtureResults[x] as EnvironmentalMetricResult).Quantity, field);
            }
            results.AddRange(lightFixtureResults);

            //ElectricalScope Lighting Controls
            List<LifeCycleAssessmentElementResult> lightControlResults = mepScope.ElectricalScope.LightingControls.Select(x => EvaluateEnvironmentalProductDeclaration(x, phases, field, exactMatch)).ToList();
            for (int x = 0; x < lightControlResults.Count; x++)
            {
                if (lightControlResults[x] == null)
                {
                    BH.Engine.Reflection.Compute.RecordWarning("Electrical object" + ((IBHoMObject)mepScope.ElectricalScope.LightingControls[x]).BHoM_Guid + " does not contain a valid EPD.");
                    continue;
                }
                lightControlResults[x] = new EnvironmentalMetricResult(lightControlResults[x].ObjectId, lightControlResults[x].ResultCase, lightControlResults[x].TimeStep, ObjectScope.Electrical, ObjectCategory.LightingControls, phases, lightControlResults[x].EnvironmentalProductDeclaration, (lightControlResults[x] as EnvironmentalMetricResult).Quantity, field);
            }
            results.AddRange(lightControlResults);

            //ElectricalScope Meters
            List<LifeCycleAssessmentElementResult> meterResults = mepScope.ElectricalScope.Meters.Select(x => EvaluateEnvironmentalProductDeclaration(x, phases, field, exactMatch)).ToList();
            for (int x = 0; x < meterResults.Count; x++)
            {
                if (meterResults[x] == null)
                {
                    BH.Engine.Reflection.Compute.RecordWarning("Electrical object" + ((IBHoMObject)mepScope.ElectricalScope.Meters[x]).BHoM_Guid + " does not contain a valid EPD.");
                    continue;
                }
                meterResults[x] = new EnvironmentalMetricResult(meterResults[x].ObjectId, meterResults[x].ResultCase, meterResults[x].TimeStep, ObjectScope.Electrical, ObjectCategory.Meters, phases, meterResults[x].EnvironmentalProductDeclaration, (meterResults[x] as EnvironmentalMetricResult).Quantity, field);
            }
            results.AddRange(meterResults);

            //ElectricalScope Security Devices
            List<LifeCycleAssessmentElementResult> securityResults = mepScope.ElectricalScope.SecurityDevices.Select(x => EvaluateEnvironmentalProductDeclaration(x, phases, field, exactMatch)).ToList();
            for (int x = 0; x < securityResults.Count; x++)
            {
                if (securityResults[x] == null)
                {
                    BH.Engine.Reflection.Compute.RecordWarning("Electrical object" + ((IBHoMObject)mepScope.ElectricalScope.SecurityDevices[x]).BHoM_Guid + " does not contain a valid EPD.");
                    continue;
                }
                securityResults[x] = new EnvironmentalMetricResult(securityResults[x].ObjectId, securityResults[x].ResultCase, securityResults[x].TimeStep, ObjectScope.Electrical, ObjectCategory.SecurityDevices, phases, securityResults[x].EnvironmentalProductDeclaration, (securityResults[x] as EnvironmentalMetricResult).Quantity, field);
            }
            results.AddRange(securityResults);

            //ElectricalScope Sockets
            List<LifeCycleAssessmentElementResult> socketResults = mepScope.ElectricalScope.Sockets.Select(x => EvaluateEnvironmentalProductDeclaration(x, phases, field, exactMatch)).ToList();
            for (int x = 0; x < socketResults.Count; x++)
            {
                if (socketResults[x] == null)
                {
                    BH.Engine.Reflection.Compute.RecordWarning("Electrical object" + ((IBHoMObject)mepScope.ElectricalScope.Sockets[x]).BHoM_Guid + " does not contain a valid EPD.");
                    continue;
                }
                socketResults[x] = new EnvironmentalMetricResult(socketResults[x].ObjectId, socketResults[x].ResultCase, socketResults[x].TimeStep, ObjectScope.Electrical, ObjectCategory.Sockets, phases, socketResults[x].EnvironmentalProductDeclaration, (socketResults[x] as EnvironmentalMetricResult).Quantity, field);
            }
            results.AddRange(socketResults);

            //ElectricalScope Solar Panels
            List<LifeCycleAssessmentElementResult> solarPanelResults = mepScope.ElectricalScope.SolarPanels.Select(x => EvaluateEnvironmentalProductDeclaration(x, phases, field, exactMatch)).ToList();
            for (int x = 0; x < solarPanelResults.Count; x++)
            {
                if (solarPanelResults[x] == null)
                {
                    BH.Engine.Reflection.Compute.RecordWarning("Electrical object" + ((IBHoMObject)mepScope.ElectricalScope.SolarPanels[x]).BHoM_Guid + " does not contain a valid EPD.");
                    continue;
                }
                solarPanelResults[x] = new EnvironmentalMetricResult(solarPanelResults[x].ObjectId, solarPanelResults[x].ResultCase, solarPanelResults[x].TimeStep, ObjectScope.Electrical, ObjectCategory.SolarPanels, phases, solarPanelResults[x].EnvironmentalProductDeclaration, (solarPanelResults[x] as EnvironmentalMetricResult).Quantity, field);
            }
            results.AddRange(solarPanelResults);

            //ElectricalScope Wiring
            List<LifeCycleAssessmentElementResult> wiringResults = mepScope.ElectricalScope.WireSegments.Select(x => EvaluateEnvironmentalProductDeclaration(x, phases, field, exactMatch)).ToList();
            for (int x = 0; x < wiringResults.Count; x++)
            {
                if (wiringResults[x] == null)
                {
                    BH.Engine.Reflection.Compute.RecordWarning("Electrical object " + ((IBHoMObject)mepScope.ElectricalScope.WireSegments[x]).BHoM_Guid + " does not contain a valid EPD.");
                    continue;
                }
                wiringResults[x] = new EnvironmentalMetricResult(wiringResults[x].ObjectId, wiringResults[x].ResultCase, wiringResults[x].TimeStep, ObjectScope.Electrical, ObjectCategory.Wiring, phases, wiringResults[x].EnvironmentalProductDeclaration, (wiringResults[x] as EnvironmentalMetricResult).Quantity, field);
            }
            results.AddRange(wiringResults);

            //ElectricalScope AdditionalObjects
            List<LifeCycleAssessmentElementResult> additionalElectricalObjectsResults = mepScope.ElectricalScope.AdditionalObjects.Select(x => EvaluateEnvironmentalProductDeclaration(x, phases, field, exactMatch)).ToList();
            for (int x = 0; x < additionalElectricalObjectsResults.Count; x++)
            {
                if (additionalElectricalObjectsResults[x] == null)
                {
                    BH.Engine.Reflection.Compute.RecordWarning("Electrical object " + ((IBHoMObject)mepScope.ElectricalScope.AdditionalObjects[x]).BHoM_Guid + " does not contain a valid EPD.");
                    continue;
                }
                additionalElectricalObjectsResults[x] = new EnvironmentalMetricResult(additionalElectricalObjectsResults[x].ObjectId, additionalElectricalObjectsResults[x].ResultCase, additionalElectricalObjectsResults[x].TimeStep, ObjectScope.Electrical, ObjectCategory.AdditionalObjects, phases, additionalElectricalObjectsResults[x].EnvironmentalProductDeclaration, (additionalElectricalObjectsResults[x] as EnvironmentalMetricResult).Quantity, field);
            }
            results.AddRange(additionalElectricalObjectsResults);

            //PlumbingScope

            //PlumbingScope Equipment
            List<LifeCycleAssessmentElementResult> plumbingEquipmentResults = mepScope.PlumbingScope.Equipment.Select(x => EvaluateEnvironmentalProductDeclaration(x, phases, field, exactMatch)).ToList();
            for (int x = 0; x < plumbingEquipmentResults.Count; x++)
            {
                if (plumbingEquipmentResults[x] == null)
                {
                    BH.Engine.Reflection.Compute.RecordWarning("Plumbing object " + ((IBHoMObject)mepScope.PlumbingScope.Equipment[x]).BHoM_Guid + " does not contain a valid EPD.");
                    continue;
                }
                plumbingEquipmentResults[x] = new EnvironmentalMetricResult(plumbingEquipmentResults[x].ObjectId, plumbingEquipmentResults[x].ResultCase, plumbingEquipmentResults[x].TimeStep, ObjectScope.Plumbing, ObjectCategory.Equipment, phases, plumbingEquipmentResults[x].EnvironmentalProductDeclaration, (plumbingEquipmentResults[x] as EnvironmentalMetricResult).Quantity, field);
            }
            results.AddRange(plumbingEquipmentResults);

            //PlumbingScope Pipes
            List<LifeCycleAssessmentElementResult> plumbingPipingResults = mepScope.PlumbingScope.Pipes.Select(x => EvaluateEnvironmentalProductDeclaration(x, phases, field, exactMatch)).ToList();
            for (int x = 0; x < plumbingPipingResults.Count; x++)
            {
                if (plumbingPipingResults[x] == null)
                {
                    BH.Engine.Reflection.Compute.RecordWarning("Plumbing object " + ((IBHoMObject)mepScope.PlumbingScope.Pipes[x]).BHoM_Guid + " does not contain a valid EPD.");
                    continue;
                }
                plumbingPipingResults[x] = new EnvironmentalMetricResult(plumbingPipingResults[x].ObjectId, plumbingPipingResults[x].ResultCase, plumbingPipingResults[x].TimeStep, ObjectScope.Plumbing, ObjectCategory.Pipes, phases, plumbingPipingResults[x].EnvironmentalProductDeclaration, (plumbingPipingResults[x] as EnvironmentalMetricResult).Quantity, field);
            }
            results.AddRange(plumbingPipingResults);

            //PlumbingScope PlumbingFixtures
            List<LifeCycleAssessmentElementResult> plumbingFixtureResults = mepScope.PlumbingScope.PlumbingFixtures.Select(x => EvaluateEnvironmentalProductDeclaration(x, phases, field, exactMatch)).ToList();
            for (int x = 0; x < plumbingFixtureResults.Count; x++)
            {
                if (plumbingFixtureResults[x] == null)
                {
                    BH.Engine.Reflection.Compute.RecordWarning("Plumbing object " + ((IBHoMObject)mepScope.PlumbingScope.PlumbingFixtures[x]).BHoM_Guid + " does not contain a valid EPD.");
                    continue;
                }
                plumbingFixtureResults[x] = new EnvironmentalMetricResult(plumbingFixtureResults[x].ObjectId, plumbingFixtureResults[x].ResultCase, plumbingFixtureResults[x].TimeStep, ObjectScope.Plumbing, ObjectCategory.PlumbingFixtures, phases, plumbingFixtureResults[x].EnvironmentalProductDeclaration, (plumbingFixtureResults[x] as EnvironmentalMetricResult).Quantity, field);
            }
            results.AddRange(plumbingFixtureResults);

            //PlumbingScope Tanks
            List<LifeCycleAssessmentElementResult> plumbingTanksResults = mepScope.PlumbingScope.Tanks.Select(x => EvaluateEnvironmentalProductDeclaration(x, phases, field, exactMatch)).ToList();
            for (int x = 0; x < plumbingTanksResults.Count; x++)
            {
                if (plumbingTanksResults[x] == null)
                {
                    BH.Engine.Reflection.Compute.RecordWarning("Plumbing object " + ((IBHoMObject)mepScope.PlumbingScope.Tanks[x]).BHoM_Guid + " does not contain a valid EPD.");
                    continue;
                }
                plumbingTanksResults[x] = new EnvironmentalMetricResult(plumbingTanksResults[x].ObjectId, plumbingTanksResults[x].ResultCase, plumbingTanksResults[x].TimeStep, ObjectScope.Plumbing, ObjectCategory.Tanks, phases, plumbingTanksResults[x].EnvironmentalProductDeclaration, (plumbingTanksResults[x] as EnvironmentalMetricResult).Quantity, field);
            }
            results.AddRange(plumbingTanksResults);

            //PlumbingScope Valves
            List<LifeCycleAssessmentElementResult> plumbingValvesResults = mepScope.PlumbingScope.Valves.Select(x => EvaluateEnvironmentalProductDeclaration(x, phases, field, exactMatch)).ToList();
            for (int x = 0; x < plumbingValvesResults.Count; x++)
            {
                if (plumbingValvesResults[x] == null)
                {
                    BH.Engine.Reflection.Compute.RecordWarning("Plumbing object " + ((IBHoMObject)mepScope.PlumbingScope.Valves[x]).BHoM_Guid + " does not contain a valid EPD.");
                    continue;
                }
                plumbingValvesResults[x] = new EnvironmentalMetricResult(plumbingValvesResults[x].ObjectId, plumbingValvesResults[x].ResultCase, plumbingValvesResults[x].TimeStep, ObjectScope.Plumbing, ObjectCategory.Valves, phases, plumbingValvesResults[x].EnvironmentalProductDeclaration, (plumbingValvesResults[x] as EnvironmentalMetricResult).Quantity, field);
            }
            results.AddRange(plumbingValvesResults);

            //PlumbingScope AdditionalObjects
            List<LifeCycleAssessmentElementResult> additionalPlumbingObjectsResults = mepScope.PlumbingScope.AdditionalObjects.Select(x => EvaluateEnvironmentalProductDeclaration(x, phases, field, exactMatch)).ToList();
            for (int x = 0; x < additionalPlumbingObjectsResults.Count; x++)
            {
                if (additionalPlumbingObjectsResults[x] == null)
                {
                    BH.Engine.Reflection.Compute.RecordWarning("Plumbing object " + ((IBHoMObject)mepScope.PlumbingScope.AdditionalObjects[x]).BHoM_Guid + " does not contain a valid EPD.");
                    continue;
                }
                additionalPlumbingObjectsResults[x] = new EnvironmentalMetricResult(additionalPlumbingObjectsResults[x].ObjectId, additionalPlumbingObjectsResults[x].ResultCase, additionalPlumbingObjectsResults[x].TimeStep, ObjectScope.Plumbing, ObjectCategory.AdditionalObjects, phases, additionalPlumbingObjectsResults[x].EnvironmentalProductDeclaration, (additionalPlumbingObjectsResults[x] as EnvironmentalMetricResult).Quantity, field);
            }
            results.AddRange(additionalPlumbingObjectsResults);

            //FireProtectionScope

            //FireProtectionScope Equipment
            List<LifeCycleAssessmentElementResult> fireProtectionEquipmentResults = mepScope.FireProtectionScope.Equipment.Select(x => EvaluateEnvironmentalProductDeclaration(x, phases, field, exactMatch)).ToList();
            for (int x = 0; x < fireProtectionEquipmentResults.Count; x++)
            {
                if (fireProtectionEquipmentResults[x] == null)
                {
                    BH.Engine.Reflection.Compute.RecordWarning("Fire Protection object " + ((IBHoMObject)mepScope.FireProtectionScope.Equipment[x]).BHoM_Guid + " does not contain a valid EPD.");
                    continue;
                }
                fireProtectionEquipmentResults[x] = new EnvironmentalMetricResult(fireProtectionEquipmentResults[x].ObjectId, fireProtectionEquipmentResults[x].ResultCase, fireProtectionEquipmentResults[x].TimeStep, ObjectScope.FireProtection, ObjectCategory.Equipment, phases, fireProtectionEquipmentResults[x].EnvironmentalProductDeclaration, (fireProtectionEquipmentResults[x] as EnvironmentalMetricResult).Quantity, field);
            }
            results.AddRange(fireProtectionEquipmentResults);

            //FireProtectionScope Pipes
            List<LifeCycleAssessmentElementResult> fireProtectionPipesResults = mepScope.FireProtectionScope.Pipes.Select(x => EvaluateEnvironmentalProductDeclaration(x, phases, field, exactMatch)).ToList();
            for (int x = 0; x < fireProtectionPipesResults.Count; x++)
            {
                if (fireProtectionPipesResults[x] == null)
                {
                    BH.Engine.Reflection.Compute.RecordWarning("Fire Protection object " + ((IBHoMObject)mepScope.FireProtectionScope.Pipes[x]).BHoM_Guid + " does not contain a valid EPD.");
                    continue;
                }
                fireProtectionPipesResults[x] = new EnvironmentalMetricResult(fireProtectionPipesResults[x].ObjectId, fireProtectionPipesResults[x].ResultCase, fireProtectionPipesResults[x].TimeStep, ObjectScope.FireProtection, ObjectCategory.Pipes, phases, fireProtectionPipesResults[x].EnvironmentalProductDeclaration, (fireProtectionPipesResults[x] as EnvironmentalMetricResult).Quantity, field);
            }
            results.AddRange(fireProtectionPipesResults);

            //FireProtectionScope Sprinklers
            List<LifeCycleAssessmentElementResult> fireProtectionSprinklersResults = mepScope.FireProtectionScope.Sprinklers.Select(x => EvaluateEnvironmentalProductDeclaration(x, phases, field, exactMatch)).ToList();
            for (int x = 0; x < fireProtectionSprinklersResults.Count; x++)
            {
                if (fireProtectionSprinklersResults[x] == null)
                {
                    BH.Engine.Reflection.Compute.RecordWarning("Fire Protection object " + ((IBHoMObject)mepScope.FireProtectionScope.Sprinklers[x]).BHoM_Guid + " does not contain a valid EPD.");
                    continue;
                }
                fireProtectionSprinklersResults[x] = new EnvironmentalMetricResult(fireProtectionSprinklersResults[x].ObjectId, fireProtectionSprinklersResults[x].ResultCase, fireProtectionSprinklersResults[x].TimeStep, ObjectScope.FireProtection, ObjectCategory.Sprinklers, phases, fireProtectionSprinklersResults[x].EnvironmentalProductDeclaration, (fireProtectionSprinklersResults[x] as EnvironmentalMetricResult).Quantity, field);
            }
            results.AddRange(fireProtectionSprinklersResults);

            //FireProtectionScope Tanks
            List<LifeCycleAssessmentElementResult> fireProtectionTankResults = mepScope.FireProtectionScope.Tanks.Select(x => EvaluateEnvironmentalProductDeclaration(x, phases, field, exactMatch)).ToList();
            for (int x = 0; x < fireProtectionTankResults.Count; x++)
            {
                if (fireProtectionTankResults[x] == null)
                {
                    BH.Engine.Reflection.Compute.RecordWarning("Fire Protection object " + ((IBHoMObject)mepScope.FireProtectionScope.Tanks[x]).BHoM_Guid + " does not contain a valid EPD.");
                    continue;
                }
                fireProtectionTankResults[x] = new EnvironmentalMetricResult(fireProtectionTankResults[x].ObjectId, fireProtectionTankResults[x].ResultCase, fireProtectionTankResults[x].TimeStep, ObjectScope.FireProtection, ObjectCategory.Tanks, phases, fireProtectionTankResults[x].EnvironmentalProductDeclaration, (fireProtectionTankResults[x] as EnvironmentalMetricResult).Quantity, field);
            }
            results.AddRange(fireProtectionTankResults);

            //FireProtectionScope AdditionalObjects
            List<LifeCycleAssessmentElementResult> additionalFireProtectionObjectsResults = mepScope.FireProtectionScope.AdditionalObjects.Select(x => EvaluateEnvironmentalProductDeclaration(x, phases, field, exactMatch)).ToList();
            for (int x = 0; x < additionalFireProtectionObjectsResults.Count; x++)
            {
                if (additionalFireProtectionObjectsResults[x] == null)
                {
                    BH.Engine.Reflection.Compute.RecordWarning("Fire Protection object " + ((IBHoMObject)mepScope.FireProtectionScope.AdditionalObjects[x]).BHoM_Guid + " does not contain a valid EPD.");
                    continue;
                }
                additionalFireProtectionObjectsResults[x] = new EnvironmentalMetricResult(additionalFireProtectionObjectsResults[x].ObjectId, additionalFireProtectionObjectsResults[x].ResultCase, additionalFireProtectionObjectsResults[x].TimeStep, ObjectScope.FireProtection, ObjectCategory.AdditionalObjects, phases, additionalFireProtectionObjectsResults[x].EnvironmentalProductDeclaration, (additionalFireProtectionObjectsResults[x] as EnvironmentalMetricResult).Quantity, field);
            }
            results.AddRange(additionalFireProtectionObjectsResults);

            return results;
        }

        /***************************************************/

        [Description("This method calls the appropriate compute method per object within the TenantImprovementScope and returns results.")]
        [Input("tenantImprovementScope", "This is a life cycle assessment scope object which contains all specified objects along with their associated EPD MaterialFragment data.")]
        [Input("field", "This is the desired field you would like to evaluate. Notice that not all material datasets will contain information for all metrics. A holistic GWP evaluation is currently the only standard evaluation possible with all provided datasets.")]
        [Input("phases", "Provide phases of life you wish to evaluate. Phases of life must be documented within EPDs for this method to work.")]
        [Input("exactMatch", "If true, the evaluation method will force an exact LCA phase match to solve for.")]
        [Output("result", "The LifeCycleElementResult that contains Tenant Improvement scope specific data, total results, and results per element.")]
        [PreviousVersion("4.2", "BH.Engine.LifeCycleAssessment.Compute.EvaluateLifeCycleAssessmentScope(BH.oM.LifeCycleAssessment.TenantImprovementScope, BH.oM.LifeCycleAssessment.EnvironmentalProductDeclarationField)")]
        public static List<LifeCycleAssessmentElementResult> EvaluateLifeCycleAssessmentScope(this TenantImprovementScope tenantImprovementScope, EnvironmentalProductDeclarationField field, List<LifeCycleAssessmentPhases> phases, bool exactMatch = false)
        {
            if (tenantImprovementScope.IsValid() == false)
            {
                BH.Engine.Reflection.Compute.RecordWarning("Your TenantImprovementScope is incomplete, please provide all types of objects where possible.");
            }

            List<LifeCycleAssessmentElementResult> results = new List<LifeCycleAssessmentElementResult>();

            //TI Ceilings
            List<LifeCycleAssessmentElementResult> ceilingResults = tenantImprovementScope.Ceiling.Select(x => EvaluateEnvironmentalProductDeclaration(x, phases, field, exactMatch)).ToList();
            for (int x = 0; x < ceilingResults.Count; x++)
            {
                if (ceilingResults[x] == null)
                {
                    BH.Engine.Reflection.Compute.RecordWarning("Object " + ((IBHoMObject)tenantImprovementScope.Ceiling[x]).BHoM_Guid + " Within TentantImprovementScope Ceilings does not contain a valid EPD.");
                    continue;
                }
                ceilingResults[x] = new EnvironmentalMetricResult(ceilingResults[x].ObjectId, ceilingResults[x].ResultCase, ceilingResults[x].TimeStep, ObjectScope.TenantImprovement, ObjectCategory.Ceiling, phases, ceilingResults[x].EnvironmentalProductDeclaration, (ceilingResults[x] as EnvironmentalMetricResult).Quantity, field);
            }
            results.AddRange(ceilingResults);

            //TI Finishes
            List<LifeCycleAssessmentElementResult> finishResults = tenantImprovementScope.Finishes.Select(x => EvaluateEnvironmentalProductDeclaration(x, phases, field, exactMatch)).ToList();
            for (int x = 0; x < finishResults.Count; x++)
            {
                if (finishResults[x] == null)
                {
                    BH.Engine.Reflection.Compute.RecordWarning("Object " + ((IBHoMObject)tenantImprovementScope.Finishes[x]).BHoM_Guid + " Within TentantImprovementScope Finishes does not contain a valid EPD.");
                    continue;
                }
                finishResults[x] = new EnvironmentalMetricResult(finishResults[x].ObjectId, finishResults[x].ResultCase, finishResults[x].TimeStep, ObjectScope.TenantImprovement, ObjectCategory.Finishes, phases, finishResults[x].EnvironmentalProductDeclaration, (finishResults[x] as EnvironmentalMetricResult).Quantity, field);
            }
            results.AddRange(finishResults);

            //TI Furniture
            List<LifeCycleAssessmentElementResult> furnitureResults = tenantImprovementScope.Furniture.Select(x => EvaluateEnvironmentalProductDeclaration(x, phases, field, exactMatch)).ToList();
            for (int x = 0; x < furnitureResults.Count; x++)
            {
                if (furnitureResults[x] == null)
                {
                    BH.Engine.Reflection.Compute.RecordWarning("Object " + ((IBHoMObject)tenantImprovementScope.Furniture[x]).BHoM_Guid + " Within TentantImprovementScope Furniture does not contain a valid EPD.");
                    continue;
                }
                furnitureResults[x] = new EnvironmentalMetricResult(furnitureResults[x].ObjectId, furnitureResults[x].ResultCase, furnitureResults[x].TimeStep, ObjectScope.TenantImprovement, ObjectCategory.Furniture, phases, furnitureResults[x].EnvironmentalProductDeclaration, (furnitureResults[x] as EnvironmentalMetricResult).Quantity, field);
            }
            results.AddRange(furnitureResults);

            //TI Interior Doors
            List<LifeCycleAssessmentElementResult> intDoorsResults = tenantImprovementScope.InteriorDoors.Select(x => EvaluateEnvironmentalProductDeclaration(x, phases, field, exactMatch)).ToList();
            for (int x = 0; x < intDoorsResults.Count; x++)
            {
                if (intDoorsResults[x] == null)
                {
                    BH.Engine.Reflection.Compute.RecordWarning("Object " + ((IBHoMObject)tenantImprovementScope.InteriorDoors[x]).BHoM_Guid + " Within TentantImprovementScope InteriorDoors does not contain a valid EPD.");
                    continue;
                }
                intDoorsResults[x] = new EnvironmentalMetricResult(intDoorsResults[x].ObjectId, intDoorsResults[x].ResultCase, intDoorsResults[x].TimeStep, ObjectScope.TenantImprovement, ObjectCategory.InteriorDoor, phases, intDoorsResults[x].EnvironmentalProductDeclaration, (intDoorsResults[x] as EnvironmentalMetricResult).Quantity, field);
            }
            results.AddRange(intDoorsResults);

            //TI Interior Glazing
            List<LifeCycleAssessmentElementResult> intGlazingResults = tenantImprovementScope.InteriorGlazing.Select(x => EvaluateEnvironmentalProductDeclaration(x, phases, field, exactMatch)).ToList();
            for (int x = 0; x < intGlazingResults.Count; x++)
            {
                if (intGlazingResults[x] == null)
                {
                    BH.Engine.Reflection.Compute.RecordWarning("Object " + ((IBHoMObject)tenantImprovementScope.InteriorGlazing[x]).BHoM_Guid + " Within TentantImprovementScope InteriorGlazing does not contain a valid EPD.");
                    continue;
                }
                intGlazingResults[x] = new EnvironmentalMetricResult(intGlazingResults[x].ObjectId, intGlazingResults[x].ResultCase, intGlazingResults[x].TimeStep, ObjectScope.TenantImprovement, ObjectCategory.InteriorGlazing, phases, intGlazingResults[x].EnvironmentalProductDeclaration, (intGlazingResults[x] as EnvironmentalMetricResult).Quantity, field);
            }
            results.AddRange(intGlazingResults);

            //TI Partition Walls
            List<LifeCycleAssessmentElementResult> partWallsResults = tenantImprovementScope.PartitionWalls.Select(x => EvaluateEnvironmentalProductDeclaration(x, phases, field, exactMatch)).ToList();
            for (int x = 0; x < partWallsResults.Count; x++)
            {
                if (partWallsResults[x] == null)
                {
                    BH.Engine.Reflection.Compute.RecordWarning("Object " + ((IBHoMObject)tenantImprovementScope.PartitionWalls[x]).BHoM_Guid + " Within TentantImprovementScope PartitionWalls does not contain a valid EPD.");
                    continue;
                }
                partWallsResults[x] = new EnvironmentalMetricResult(partWallsResults[x].ObjectId, partWallsResults[x].ResultCase, partWallsResults[x].TimeStep, ObjectScope.TenantImprovement, ObjectCategory.PartitionWall, phases, partWallsResults[x].EnvironmentalProductDeclaration, (partWallsResults[x] as EnvironmentalMetricResult).Quantity, field);
            }
            results.AddRange(partWallsResults);

            //TI AdditionalObjects
            List<LifeCycleAssessmentElementResult> additionalObjectsResults = tenantImprovementScope.AdditionalObjects.Select(x => EvaluateEnvironmentalProductDeclaration(x, phases, field, exactMatch)).ToList();
            for (int x = 0; x < additionalObjectsResults.Count; x++)
            {
                if (additionalObjectsResults[x] == null)
                {
                    BH.Engine.Reflection.Compute.RecordWarning("Object " + ((IBHoMObject)tenantImprovementScope.AdditionalObjects[x]).BHoM_Guid + " Within TenantImprovementScope AdditionalObjects does not contain a valid EPD.");
                    continue;
                }
                additionalObjectsResults[x] = new EnvironmentalMetricResult(additionalObjectsResults[x].ObjectId, additionalObjectsResults[x].ResultCase, additionalObjectsResults[x].TimeStep, ObjectScope.TenantImprovement, ObjectCategory.AdditionalObjects, phases, additionalObjectsResults[x].EnvironmentalProductDeclaration, (additionalObjectsResults[x] as EnvironmentalMetricResult).Quantity, field);
            }
            results.AddRange(additionalObjectsResults);

            return results;
        }

        /***************************************************/
    }
}

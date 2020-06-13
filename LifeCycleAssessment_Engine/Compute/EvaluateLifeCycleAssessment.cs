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

namespace BH.Engine.LifeCycleAssessment
{
    public static partial class Compute
    {
        /***************************************************/
        /****   Public Methods                          ****/
        /***************************************************/

        [Description("This method calls the appropriate compute method per object within a Life Cycle Assessment.")]
        [Input("lca", "This is a complete Life Cycle Assessment object with its appropriate nested scope objects for which the evaluation will occur.")]
        [Input("field", "This is the desired field you would like to evaluate. Notice that not all material datasets will contain information for all metrics. A holistic GWP evaluation is currently the only standard evaluation possible with all provided datasets.")]
        [Output("result", "The LifeCycleElementResult that contains project data, total results, and results per element.")]
        public static LifeCycleAssessmentResult EvaluateLifeCycleAssessment(ProjectLifeCycleAssessment lca, EnvironmentalProductDeclarationField field)
        {
            if (lca == null) return null;

            List<LifeCycleAssessmentElementResult> results = new List<LifeCycleAssessmentElementResult>();

            results.AddRange(lca.StructuresScope.EvaluateLifeCycleAssessmentScope(field));
            results.AddRange(lca.FoundationsScope.EvaluateLifeCycleAssessmentScope(field));
            results.AddRange(lca.EnclosuresScope.EvaluateLifeCycleAssessmentScope(field));
            results.AddRange(lca.MEPScope.EvaluateLifeCycleAssessmentScope(field));
            results.AddRange(lca.TenantImprovementScope.EvaluateLifeCycleAssessmentScope(field));

            return new LifeCycleAssessmentResult(lca.BHoM_Guid, "GWP", 0, lca.LifeCycleAssessmentScope, new System.Collections.ObjectModel.ReadOnlyCollection<LifeCycleAssessmentElementResult>(results), results.TotalGlobalWarmingPotential());
        }

        [Description("This method calls the appropriate compute method per object within the StructuresScope and returns results.")]
        [Input("structuresScope", "This is a life cycle assessment scope object which contains all specified objects along with their associated EPD MaterialFragment data.")]
        [Input("field", "This is the desired field you would like to evaluate. Notice that not all material datasets will contain information for all metrics. A holistic GWP evaluation is currently the only standard evaluation possible with all provided datasets.")]
        [Output("result", "The LifeCycleElementResult that contains structures scope specific data, total results, and results per element.")]
        public static List<LifeCycleAssessmentElementResult> EvaluateLifeCycleAssessmentScope(this StructuresScope structuresScope, EnvironmentalProductDeclarationField field)
        {
            List<LifeCycleAssessmentElementResult> results = new List<LifeCycleAssessmentElementResult>();

            List<LifeCycleAssessmentElementResult> beamResults = structuresScope.Beams.Select(x => EvaluateEnvironmentalProductDeclarationPerObject(x as IBHoMObject, field)).ToList();
            for (int x = 0; x < beamResults.Count; x++)
                beamResults[x] = new GlobalWarmingPotentialResult(beamResults[x].ObjectId, beamResults[x].ResultCase, beamResults[x].TimeStep, ObjectScope.Structure, ObjectCategory.Beam, beamResults[x].EnvironmentalProductDeclaration, (beamResults[x] as GlobalWarmingPotentialResult).GlobalWarmingPotential);
            results.AddRange(beamResults);

            List<LifeCycleAssessmentElementResult> columnsResults = structuresScope.Columns.Select(x => EvaluateEnvironmentalProductDeclarationPerObject(x as IBHoMObject, field)).ToList();
            for (int x = 0; x < columnsResults.Count; x++)
                columnsResults[x] = new GlobalWarmingPotentialResult(columnsResults[x].ObjectId, columnsResults[x].ResultCase, columnsResults[x].TimeStep, ObjectScope.Structure, ObjectCategory.Column, columnsResults[x].EnvironmentalProductDeclaration, (columnsResults[x] as GlobalWarmingPotentialResult).GlobalWarmingPotential);
            results.AddRange(columnsResults);

            List<LifeCycleAssessmentElementResult> slabsResults = structuresScope.Slabs.Select(x => EvaluateEnvironmentalProductDeclarationPerObject(x as IBHoMObject, field)).ToList();
            for (int x = 0; x < slabsResults.Count; x++)
                slabsResults[x] = new GlobalWarmingPotentialResult(slabsResults[x].ObjectId, slabsResults[x].ResultCase, slabsResults[x].TimeStep, ObjectScope.Structure, ObjectCategory.Slab, slabsResults[x].EnvironmentalProductDeclaration, (slabsResults[x] as GlobalWarmingPotentialResult).GlobalWarmingPotential);
            results.AddRange(slabsResults);

            List<LifeCycleAssessmentElementResult> coreWalls = structuresScope.CoreWalls.Select(x => EvaluateEnvironmentalProductDeclarationPerObject(x as IBHoMObject, field)).ToList();
            for (int x = 0; x < coreWalls.Count; x++)
                coreWalls[x] = new GlobalWarmingPotentialResult(coreWalls[x].ObjectId, coreWalls[x].ResultCase, coreWalls[x].TimeStep, ObjectScope.Structure, ObjectCategory.Wall, coreWalls[x].EnvironmentalProductDeclaration, (coreWalls[x] as GlobalWarmingPotentialResult).GlobalWarmingPotential);
            results.AddRange(coreWalls);

            return results;
        }

        [Description("This method calls the appropriate compute method per object within the FoundationsScope and returns results.")]
        [Input("foundationsScope", "This is a life cycle assessment scope object which contains all specified objects along with their associated EPD MaterialFragment data.")]
        [Input("field", "This is the desired field you would like to evaluate. Notice that not all material datasets will contain information for all metrics. A holistic GWP evaluation is currently the only standard evaluation possible with all provided datasets.")]
        [Output("result", "The LifeCycleElementResult that contains foundations scope specific data, total results, and results per element.")]
        public static List<LifeCycleAssessmentElementResult> EvaluateLifeCycleAssessmentScope(this FoundationsScope foundationsScope, EnvironmentalProductDeclarationField field)
        {
            List<LifeCycleAssessmentElementResult> results = new List<LifeCycleAssessmentElementResult>();

            List<LifeCycleAssessmentElementResult> footingsResults = foundationsScope.Footings.Select(x => EvaluateEnvironmentalProductDeclarationPerObject(x as IBHoMObject, field)).ToList();
            for (int x = 0; x < footingsResults.Count; x++)
                footingsResults[x] = new GlobalWarmingPotentialResult(footingsResults[x].ObjectId, footingsResults[x].ResultCase, footingsResults[x].TimeStep, ObjectScope.Foundation, ObjectCategory.Footing, footingsResults[x].EnvironmentalProductDeclaration, (footingsResults[x] as GlobalWarmingPotentialResult).GlobalWarmingPotential);
            results.AddRange(footingsResults);

            List<LifeCycleAssessmentElementResult> pilesResults = foundationsScope.Piles.Select(x => EvaluateEnvironmentalProductDeclarationPerObject(x as IBHoMObject, field)).ToList();
            for (int x = 0; x < pilesResults.Count; x++)
                pilesResults[x] = new GlobalWarmingPotentialResult(pilesResults[x].ObjectId, pilesResults[x].ResultCase, pilesResults[x].TimeStep, ObjectScope.Foundation, ObjectCategory.Pile, pilesResults[x].EnvironmentalProductDeclaration, (pilesResults[x] as GlobalWarmingPotentialResult).GlobalWarmingPotential);
            results.AddRange(pilesResults);

            List<LifeCycleAssessmentElementResult> wallsResults = foundationsScope.Walls.Select(x => EvaluateEnvironmentalProductDeclarationPerObject(x as IBHoMObject, field)).ToList();
            for (int x = 0; x < wallsResults.Count; x++)
                wallsResults[x] = new GlobalWarmingPotentialResult(wallsResults[x].ObjectId, wallsResults[x].ResultCase, wallsResults[x].TimeStep, ObjectScope.Foundation, ObjectCategory.Wall, wallsResults[x].EnvironmentalProductDeclaration, (wallsResults[x] as GlobalWarmingPotentialResult).GlobalWarmingPotential);
            results.AddRange(wallsResults);

            List<LifeCycleAssessmentElementResult> fndSlabsResults = foundationsScope.Slabs.Select(x => EvaluateEnvironmentalProductDeclarationPerObject(x as IBHoMObject, field)).ToList();
            for (int x = 0; x < fndSlabsResults.Count; x++)
                fndSlabsResults[x] = new GlobalWarmingPotentialResult(fndSlabsResults[x].ObjectId, fndSlabsResults[x].ResultCase, fndSlabsResults[x].TimeStep, ObjectScope.Foundation, ObjectCategory.Slab, fndSlabsResults[x].EnvironmentalProductDeclaration, (fndSlabsResults[x] as GlobalWarmingPotentialResult).GlobalWarmingPotential);
            results.AddRange(fndSlabsResults);

            return results;
        }

        [Description("This method calls the appropriate compute method per object within the EnclosuresScope and returns results.")]
        [Input("enclosuresScope", "This is a life cycle assessment scope object which contains all specified objects along with their associated EPD MaterialFragment data.")]
        [Input("field", "This is the desired field you would like to evaluate. Notice that not all material datasets will contain information for all metrics. A holistic GWP evaluation is currently the only standard evaluation possible with all provided datasets.")]
        [Output("result", "The LifeCycleElementResult that contains enclosures scope specific data, total results, and results per element.")]
        public static List<LifeCycleAssessmentElementResult> EvaluateLifeCycleAssessmentScope(this EnclosuresScope enclosuresScope, EnvironmentalProductDeclarationField field)
        {
            List<LifeCycleAssessmentElementResult> results = new List<LifeCycleAssessmentElementResult>();

            List<LifeCycleAssessmentElementResult> enclWallsResults = enclosuresScope.Walls.Select(x => EvaluateEnvironmentalProductDeclarationPerObject(x as IBHoMObject, field)).ToList();
            for (int x = 0; x < enclWallsResults.Count; x++)
                enclWallsResults[x] = new GlobalWarmingPotentialResult(enclWallsResults[x].ObjectId, enclWallsResults[x].ResultCase, enclWallsResults[x].TimeStep, ObjectScope.Enclosure, ObjectCategory.Wall, enclWallsResults[x].EnvironmentalProductDeclaration, (enclWallsResults[x] as GlobalWarmingPotentialResult).GlobalWarmingPotential);
            results.AddRange(enclWallsResults);

            List<LifeCycleAssessmentElementResult> curtainWallsResults = enclosuresScope.CurtainWalls.Select(x => EvaluateEnvironmentalProductDeclarationPerObject(x as IBHoMObject, field)).ToList();
            for (int x = 0; x < curtainWallsResults.Count; x++)
                curtainWallsResults[x] = new GlobalWarmingPotentialResult(curtainWallsResults[x].ObjectId, curtainWallsResults[x].ResultCase, curtainWallsResults[x].TimeStep, ObjectScope.Enclosure, ObjectCategory.CurtainWall, curtainWallsResults[x].EnvironmentalProductDeclaration, (curtainWallsResults[x] as GlobalWarmingPotentialResult).GlobalWarmingPotential);
            results.AddRange(curtainWallsResults);

            List<LifeCycleAssessmentElementResult> windowsResults = enclosuresScope.Windows.Select(x => EvaluateEnvironmentalProductDeclarationPerObject(x as IBHoMObject, field)).ToList();
            for (int x = 0; x < windowsResults.Count; x++)
                windowsResults[x] = new GlobalWarmingPotentialResult(windowsResults[x].ObjectId, windowsResults[x].ResultCase, windowsResults[x].TimeStep, ObjectScope.Enclosure, ObjectCategory.Window, windowsResults[x].EnvironmentalProductDeclaration, (windowsResults[x] as GlobalWarmingPotentialResult).GlobalWarmingPotential);
            results.AddRange(windowsResults);

            List<LifeCycleAssessmentElementResult> doorsResults = enclosuresScope.Doors.Select(x => EvaluateEnvironmentalProductDeclarationPerObject(x as IBHoMObject, field)).ToList();
            for (int x = 0; x < doorsResults.Count; x++)
                doorsResults[x] = new GlobalWarmingPotentialResult(doorsResults[x].ObjectId, doorsResults[x].ResultCase, doorsResults[x].TimeStep, ObjectScope.Enclosure, ObjectCategory.Wall, doorsResults[x].EnvironmentalProductDeclaration, (doorsResults[x] as GlobalWarmingPotentialResult).GlobalWarmingPotential);
            results.AddRange(doorsResults);

            return results;
        }

        [Description("This method calls the appropriate compute method per object within the MEPScope and returns results.")]
        [Input("mepScope", "This is a life cycle assessment scope object which contains all specified objects along with their associated EPD MaterialFragment data.")]
        [Input("field", "This is the desired field you would like to evaluate. Notice that not all material datasets will contain information for all metrics. A holistic GWP evaluation is currently the only standard evaluation possible with all provided datasets.")]
        [Output("result", "The LifeCycleElementResult that contains MEP scope specific data, total results, and results per element.")]
        public static List<LifeCycleAssessmentElementResult> EvaluateLifeCycleAssessmentScope(this MEPScope mepScope, EnvironmentalProductDeclarationField field)
        {
            List<LifeCycleAssessmentElementResult> results = new List<LifeCycleAssessmentElementResult>();

            List<LifeCycleAssessmentElementResult> batteriesResults = mepScope.Batteries.Select(x => EvaluateEnvironmentalProductDeclarationPerObject(x as IBHoMObject, field)).ToList();
            for (int x = 0; x < batteriesResults.Count; x++)
                batteriesResults[x] = new GlobalWarmingPotentialResult(batteriesResults[x].ObjectId, batteriesResults[x].ResultCase, batteriesResults[x].TimeStep, ObjectScope.MEP, ObjectCategory.Battery, batteriesResults[x].EnvironmentalProductDeclaration, (batteriesResults[x] as GlobalWarmingPotentialResult).GlobalWarmingPotential);

            List<LifeCycleAssessmentElementResult> conduitResults = mepScope.Conduit.Select(x => EvaluateEnvironmentalProductDeclarationPerObject(x as IBHoMObject, field)).ToList();
            for (int x = 0; x < conduitResults.Count; x++)
                conduitResults[x] = new GlobalWarmingPotentialResult(conduitResults[x].ObjectId, conduitResults[x].ResultCase, conduitResults[x].TimeStep, ObjectScope.MEP, ObjectCategory.Conduit, conduitResults[x].EnvironmentalProductDeclaration, (conduitResults[x] as GlobalWarmingPotentialResult).GlobalWarmingPotential);

            List<LifeCycleAssessmentElementResult> ductResults = mepScope.Ductwork.Select(x => EvaluateEnvironmentalProductDeclarationPerObject(x as IBHoMObject, field)).ToList();
            for (int x = 0; x < ductResults.Count; x++)
                ductResults[x] = new GlobalWarmingPotentialResult(ductResults[x].ObjectId, ductResults[x].ResultCase, ductResults[x].TimeStep, ObjectScope.MEP, ObjectCategory.Ductwork, ductResults[x].EnvironmentalProductDeclaration, (ductResults[x] as GlobalWarmingPotentialResult).GlobalWarmingPotential);

            List<LifeCycleAssessmentElementResult> equipmentResults = mepScope.Equipment.Select(x => EvaluateEnvironmentalProductDeclarationPerObject(x as IBHoMObject, field)).ToList();
            for (int x = 0; x < equipmentResults.Count; x++)
                equipmentResults[x] = new GlobalWarmingPotentialResult(equipmentResults[x].ObjectId, equipmentResults[x].ResultCase, equipmentResults[x].TimeStep, ObjectScope.MEP, ObjectCategory.Equipment, equipmentResults[x].EnvironmentalProductDeclaration, (equipmentResults[x] as GlobalWarmingPotentialResult).GlobalWarmingPotential);

            List<LifeCycleAssessmentElementResult> generatorResults = mepScope.Generators.Select(x => EvaluateEnvironmentalProductDeclarationPerObject(x as IBHoMObject, field)).ToList();
            for (int x = 0; x < generatorResults.Count; x++)
                generatorResults[x] = new GlobalWarmingPotentialResult(generatorResults[x].ObjectId, generatorResults[x].ResultCase, generatorResults[x].TimeStep, ObjectScope.MEP, ObjectCategory.Generator, generatorResults[x].EnvironmentalProductDeclaration, (generatorResults[x] as GlobalWarmingPotentialResult).GlobalWarmingPotential);

            List<LifeCycleAssessmentElementResult> lightingResults = mepScope.Lighting.Select(x => EvaluateEnvironmentalProductDeclarationPerObject(x as IBHoMObject, field)).ToList();
            for (int x = 0; x < lightingResults.Count; x++)
                lightingResults[x] = new GlobalWarmingPotentialResult(lightingResults[x].ObjectId, lightingResults[x].ResultCase, lightingResults[x].TimeStep, ObjectScope.MEP, ObjectCategory.Lighting, lightingResults[x].EnvironmentalProductDeclaration, (lightingResults[x] as GlobalWarmingPotentialResult).GlobalWarmingPotential);

            List<LifeCycleAssessmentElementResult> pipingResults = mepScope.Piping.Select(x => EvaluateEnvironmentalProductDeclarationPerObject(x as IBHoMObject, field)).ToList();
            for (int x = 0; x < pipingResults.Count; x++)
                pipingResults[x] = new GlobalWarmingPotentialResult(pipingResults[x].ObjectId, pipingResults[x].ResultCase, pipingResults[x].TimeStep, ObjectScope.MEP, ObjectCategory.Piping, pipingResults[x].EnvironmentalProductDeclaration, (pipingResults[x] as GlobalWarmingPotentialResult).GlobalWarmingPotential);

            List<LifeCycleAssessmentElementResult> wiringResults = mepScope.Wiring.Select(x => EvaluateEnvironmentalProductDeclarationPerObject(x as IBHoMObject, field)).ToList();
            for (int x = 0; x < wiringResults.Count; x++)
                wiringResults[x] = new GlobalWarmingPotentialResult(wiringResults[x].ObjectId, wiringResults[x].ResultCase, wiringResults[x].TimeStep, ObjectScope.MEP, ObjectCategory.Wiring, wiringResults[x].EnvironmentalProductDeclaration, (wiringResults[x] as GlobalWarmingPotentialResult).GlobalWarmingPotential);

            return results;
        }

        [Description("This method calls the appropriate compute method per object within the TenantImprovementScope and returns results.")]
        [Input("tenantImprovementScope", "This is a life cycle assessment scope object which contains all specified objects along with their associated EPD MaterialFragment data.")]
        [Input("field", "This is the desired field you would like to evaluate. Notice that not all material datasets will contain information for all metrics. A holistic GWP evaluation is currently the only standard evaluation possible with all provided datasets.")]
        [Output("result", "The LifeCycleElementResult that contains Tenant Improvement scope specific data, total results, and results per element.")]
        public static List<LifeCycleAssessmentElementResult> EvaluateLifeCycleAssessmentScope(this TenantImprovementScope tenantImprovementScope, EnvironmentalProductDeclarationField field)
        {
            List<LifeCycleAssessmentElementResult> results = new List<LifeCycleAssessmentElementResult>();

            List<LifeCycleAssessmentElementResult> ceilingResults = tenantImprovementScope.Ceiling.Select(x => EvaluateEnvironmentalProductDeclarationPerObject(x as IBHoMObject, field)).ToList();
            for (int x = 0; x < ceilingResults.Count; x++)
                ceilingResults[x] = new GlobalWarmingPotentialResult(ceilingResults[x].ObjectId, ceilingResults[x].ResultCase, ceilingResults[x].TimeStep, ObjectScope.TenantImprovement, ObjectCategory.Ceiling, ceilingResults[x].EnvironmentalProductDeclaration, (ceilingResults[x] as GlobalWarmingPotentialResult).GlobalWarmingPotential);

            List<LifeCycleAssessmentElementResult> finishResults = tenantImprovementScope.Finishes.Select(x => EvaluateEnvironmentalProductDeclarationPerObject(x as IBHoMObject, field)).ToList();
            for (int x = 0; x < finishResults.Count; x++)
                finishResults[x] = new GlobalWarmingPotentialResult(finishResults[x].ObjectId, finishResults[x].ResultCase, finishResults[x].TimeStep, ObjectScope.TenantImprovement, ObjectCategory.Finishes, finishResults[x].EnvironmentalProductDeclaration, (finishResults[x] as GlobalWarmingPotentialResult).GlobalWarmingPotential);

            List<LifeCycleAssessmentElementResult> furnitureResults = tenantImprovementScope.Furniture.Select(x => EvaluateEnvironmentalProductDeclarationPerObject(x as IBHoMObject, field)).ToList();
            for (int x = 0; x < furnitureResults.Count; x++)
                furnitureResults[x] = new GlobalWarmingPotentialResult(furnitureResults[x].ObjectId, furnitureResults[x].ResultCase, furnitureResults[x].TimeStep, ObjectScope.TenantImprovement, ObjectCategory.Furniture, furnitureResults[x].EnvironmentalProductDeclaration, (furnitureResults[x] as GlobalWarmingPotentialResult).GlobalWarmingPotential);

            List<LifeCycleAssessmentElementResult> intDoorsResults = tenantImprovementScope.InteriorDoors.Select(x => EvaluateEnvironmentalProductDeclarationPerObject(x as IBHoMObject, field)).ToList();
            for (int x = 0; x < intDoorsResults.Count; x++)
                intDoorsResults[x] = new GlobalWarmingPotentialResult(intDoorsResults[x].ObjectId, intDoorsResults[x].ResultCase, intDoorsResults[x].TimeStep, ObjectScope.TenantImprovement, ObjectCategory.InteriorDoor, intDoorsResults[x].EnvironmentalProductDeclaration, (intDoorsResults[x] as GlobalWarmingPotentialResult).GlobalWarmingPotential);

            List<LifeCycleAssessmentElementResult> intGlazingResults = tenantImprovementScope.InteriorGlazing.Select(x => EvaluateEnvironmentalProductDeclarationPerObject(x as IBHoMObject, field)).ToList();
            for (int x = 0; x < intGlazingResults.Count; x++)
                intGlazingResults[x] = new GlobalWarmingPotentialResult(intGlazingResults[x].ObjectId, intGlazingResults[x].ResultCase, intGlazingResults[x].TimeStep, ObjectScope.TenantImprovement, ObjectCategory.InteriorGlazing, intGlazingResults[x].EnvironmentalProductDeclaration, (intGlazingResults[x] as GlobalWarmingPotentialResult).GlobalWarmingPotential);

            List<LifeCycleAssessmentElementResult> partWallsResults = tenantImprovementScope.PartitionWalls.Select(x => EvaluateEnvironmentalProductDeclarationPerObject(x as IBHoMObject, field)).ToList();
            for (int x = 0; x < partWallsResults.Count; x++)
                partWallsResults[x] = new GlobalWarmingPotentialResult(partWallsResults[x].ObjectId, partWallsResults[x].ResultCase, partWallsResults[x].TimeStep, ObjectScope.TenantImprovement, ObjectCategory.PartitionWall, partWallsResults[x].EnvironmentalProductDeclaration, (partWallsResults[x] as GlobalWarmingPotentialResult).GlobalWarmingPotential);

            return results;
        }
    }
}
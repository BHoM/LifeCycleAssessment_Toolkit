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
        [Output("quantity", "The quantity of the desired metric provided by the EnvironmentalProductDeclarationField. This is an enum.")]

        public static double EvaluateLifeCycleAssessment(ProjectLifeCycleAssessment lca, EnvironmentalProductDeclarationField field)
        {
            //Check for object validity
            if (lca != null)
            {
                //Structures Scope Objects
                double beams = lca.StructuresScope.Beams.Select(x => EvaluateEnvironmentalProductDeclarationPerObject(x as IBHoMObject, field)).Sum();
                double columns = lca.StructuresScope.Columns.Select(x => EvaluateEnvironmentalProductDeclarationPerObject(x as IBHoMObject, field)).Sum();
                double coreWalls = lca.StructuresScope.CoreWalls.Select(x => EvaluateEnvironmentalProductDeclarationPerObject(x as IBHoMObject, field)).Sum();
                double slabs = lca.StructuresScope.Slabs.Select(x => EvaluateEnvironmentalProductDeclarationPerObject(x as IBHoMObject, field)).Sum();
                double structures = beams + columns + coreWalls + slabs;
                //create new results objects

                //Foundation Scope Objects
                double footings = lca.FoundationsScope.Footings.Select(x => EvaluateEnvironmentalProductDeclarationPerObject(x as IBHoMObject, field)).Sum();
                double piles = lca.FoundationsScope.Piles.Select(x => EvaluateEnvironmentalProductDeclarationPerObject(x as IBHoMObject, field)).Sum();
                double walls = lca.FoundationsScope.Walls.Select(x => EvaluateEnvironmentalProductDeclarationPerObject(x as IBHoMObject, field)).Sum();
                double fndSlabs = lca.FoundationsScope.Slabs.Select(x => EvaluateEnvironmentalProductDeclarationPerObject(x as IBHoMObject, field)).Sum();
                double foundations = footings + piles + walls + fndSlabs;
                //create new results objects

                //Enclosure Scope Objects
                double encWalls = lca.EnclosuresScope.Walls.Select(x => EvaluateEnvironmentalProductDeclarationPerObject(x as IBHoMObject, field)).Sum();
                double curtainWalls = lca.EnclosuresScope.CurtainWalls.Select(x => EvaluateEnvironmentalProductDeclarationPerObject(x as IBHoMObject, field)).Sum();
                double windows = lca.EnclosuresScope.Windows.Select(x => EvaluateEnvironmentalProductDeclarationPerObject(x as IBHoMObject, field)).Sum();
                double doors = lca.EnclosuresScope.Doors.Select(x => EvaluateEnvironmentalProductDeclarationPerObject(x as IBHoMObject, field)).Sum();
                double enclosures = encWalls + curtainWalls + windows + doors;
                //create new results objects

                //MEP Scope Objects
                double batteries = lca.MEPScope.Batteries.Select(x => EvaluateEnvironmentalProductDeclarationPerObject(x as IBHoMObject, field)).Sum();
                double conduit = lca.MEPScope.Conduit.Select(x => EvaluateEnvironmentalProductDeclarationPerObject(x as IBHoMObject, field)).Sum();
                double ductwork = lca.MEPScope.Ductwork.Select(x => EvaluateEnvironmentalProductDeclarationPerObject(x as IBHoMObject, field)).Sum();
                double equipment = lca.MEPScope.Equipment.Select(x => EvaluateEnvironmentalProductDeclarationPerObject(x as IBHoMObject, field)).Sum();
                double generators = lca.MEPScope.Generators.Select(x => EvaluateEnvironmentalProductDeclarationPerObject(x as IBHoMObject, field)).Sum();
                double lighting = lca.MEPScope.Lighting.Select(x => EvaluateEnvironmentalProductDeclarationPerObject(x as IBHoMObject, field)).Sum();
                double piping = lca.MEPScope.Piping.Select(x => EvaluateEnvironmentalProductDeclarationPerObject(x as IBHoMObject, field)).Sum();
                double wiring = lca.MEPScope.Wiring.Select(x => EvaluateEnvironmentalProductDeclarationPerObject(x as IBHoMObject, field)).Sum();
                double mep = batteries + conduit + ductwork + equipment + generators + lighting + piping + wiring;
                //create new results objects

                //Tenant Improvement Scope Objects
                double ceiling = lca.TenantImprovementScope.Ceiling.Select(x => EvaluateEnvironmentalProductDeclarationPerObject(x as IBHoMObject, field)).Sum();
                double finishes = lca.TenantImprovementScope.Finishes.Select(x => EvaluateEnvironmentalProductDeclarationPerObject(x as IBHoMObject, field)).Sum();
                double flooring = lca.TenantImprovementScope.Flooring.Select(x => EvaluateEnvironmentalProductDeclarationPerObject(x as IBHoMObject, field)).Sum();
                double furniture = lca.TenantImprovementScope.Furniture.Select(x => EvaluateEnvironmentalProductDeclarationPerObject(x as IBHoMObject, field)).Sum();
                double intDoors = lca.TenantImprovementScope.InteriorDoors.Select(x => EvaluateEnvironmentalProductDeclarationPerObject(x as IBHoMObject, field)).Sum();
                double intGlazing = lca.TenantImprovementScope.InteriorGlazing.Select(x => EvaluateEnvironmentalProductDeclarationPerObject(x as IBHoMObject, field)).Sum();
                double partitionWalls = lca.TenantImprovementScope.PartitionWalls.Select(x => EvaluateEnvironmentalProductDeclarationPerObject(x as IBHoMObject, field)).Sum();
                double ti = ceiling + finishes + flooring + furniture + intDoors + intGlazing + partitionWalls;
                //create new results objects

                return structures + foundations + enclosures + mep + ti;
            }
            return double.NaN;
        }
    }
}
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
        [Output("results", "The quantity of the desired metric provided by the EnvironmentalProductDeclarationField. This is an enum.")]

        public static LifeCycleAssessmentResult EvaluateLifeCycleAssessment(ProjectLifeCycleAssessment lca, EnvironmentalProductDeclarationField field)
        {

            //Check for object validity
            if (lca != null)
            {
                ////Structures Scope Objects
                //object beams = System.Convert.ToDouble(lca.StructuresScope.Beams.Select(x => EvaluateEnvironmentalProductDeclarationPerObject(x as IBHoMObject, field)));
                //object columns = System.Convert.ToDouble(lca.StructuresScope.Columns.Select(x => EvaluateEnvironmentalProductDeclarationPerObject(x as IBHoMObject, field)));
                //object coreWalls = System.Convert.ToDouble(lca.StructuresScope.CoreWalls.Select(x => EvaluateEnvironmentalProductDeclarationPerObject(x as IBHoMObject, field)));
                //object slabs = System.Convert.ToDouble(lca.StructuresScope.Slabs.Select(x => EvaluateEnvironmentalProductDeclarationPerObject(x as IBHoMObject, field)));
                //double structures = System.Convert.ToDouble(beams) + System.Convert.ToDouble(columns) + System.Convert.ToDouble(coreWalls) + System.Convert.ToDouble(slabs);

                ////Foundation Scope Objects
                //object footings = lca.FoundationsScope.Footings.Select(x => EvaluateEnvironmentalProductDeclarationPerObject(x as IBHoMObject, field));
                //object piles = lca.FoundationsScope.Piles.Select(x => EvaluateEnvironmentalProductDeclarationPerObject(x as IBHoMObject, field));
                //object walls = lca.FoundationsScope.Walls.Select(x => EvaluateEnvironmentalProductDeclarationPerObject(x as IBHoMObject, field));
                //object fndSlabs = lca.FoundationsScope.Slabs.Select(x => EvaluateEnvironmentalProductDeclarationPerObject(x as IBHoMObject, field));
                //double foundations = System.Convert.ToDouble(footings) + System.Convert.ToDouble(piles) + System.Convert.ToDouble(walls) + System.Convert.ToDouble(fndSlabs);

                ////Enclosure Scope Objects
                //object encWalls = lca.EnclosuresScope.Walls.Select(x => EvaluateEnvironmentalProductDeclarationPerObject(x as IBHoMObject, field));
                //object curtainWalls = lca.EnclosuresScope.CurtainWalls.Select(x => EvaluateEnvironmentalProductDeclarationPerObject(x as IBHoMObject, field));
                //object windows = lca.EnclosuresScope.Windows.Select(x => EvaluateEnvironmentalProductDeclarationPerObject(x as IBHoMObject, field));
                //object doors = lca.EnclosuresScope.Doors.Select(x => EvaluateEnvironmentalProductDeclarationPerObject(x as IBHoMObject, field));
                //double enclosures = System.Convert.ToDouble(encWalls) + System.Convert.ToDouble(curtainWalls) + System.Convert.ToDouble(windows) + System.Convert.ToDouble(doors);

                ////MEP Scope Objects
                //object batteries = lca.MEPScope.Batteries.Select(x => EvaluateEnvironmentalProductDeclarationPerObject(x as IBHoMObject, field));
                //object conduit = lca.MEPScope.Conduit.Select(x => EvaluateEnvironmentalProductDeclarationPerObject(x as IBHoMObject, field));
                //object ductwork = lca.MEPScope.Ductwork.Select(x => EvaluateEnvironmentalProductDeclarationPerObject(x as IBHoMObject, field));
                //object equipment = lca.MEPScope.Equipment.Select(x => EvaluateEnvironmentalProductDeclarationPerObject(x as IBHoMObject, field));
                //object generators = lca.MEPScope.Generators.Select(x => EvaluateEnvironmentalProductDeclarationPerObject(x as IBHoMObject, field));
                //object lighting = lca.MEPScope.Lighting.Select(x => EvaluateEnvironmentalProductDeclarationPerObject(x as IBHoMObject, field));
                //object piping = lca.MEPScope.Piping.Select(x => EvaluateEnvironmentalProductDeclarationPerObject(x as IBHoMObject, field));
                //object wiring = lca.MEPScope.Wiring.Select(x => EvaluateEnvironmentalProductDeclarationPerObject(x as IBHoMObject, field));
                //double mep = System.Convert.ToDouble(batteries) + System.Convert.ToDouble(conduit) + System.Convert.ToDouble(ductwork) + System.Convert.ToDouble(equipment) + System.Convert.ToDouble(generators) + System.Convert.ToDouble(lighting) + System.Convert.ToDouble(piping) + System.Convert.ToDouble(wiring);

                ////Tenant Improvement Scope Objects
                //object ceiling = lca.TenantImprovementScope.Ceiling.Select(x => EvaluateEnvironmentalProductDeclarationPerObject(x as IBHoMObject, field));
                //object finishes = lca.TenantImprovementScope.Finishes.Select(x => EvaluateEnvironmentalProductDeclarationPerObject(x as IBHoMObject, field));
                //object flooring = lca.TenantImprovementScope.Flooring.Select(x => EvaluateEnvironmentalProductDeclarationPerObject(x as IBHoMObject, field));
                //object furniture = lca.TenantImprovementScope.Furniture.Select(x => EvaluateEnvironmentalProductDeclarationPerObject(x as IBHoMObject, field));
                //object intDoors = lca.TenantImprovementScope.InteriorDoors.Select(x => EvaluateEnvironmentalProductDeclarationPerObject(x as IBHoMObject, field));
                //object intGlazing = lca.TenantImprovementScope.InteriorGlazing.Select(x => EvaluateEnvironmentalProductDeclarationPerObject(x as IBHoMObject, field));
                //object partitionWalls = lca.TenantImprovementScope.PartitionWalls.Select(x => EvaluateEnvironmentalProductDeclarationPerObject(x as IBHoMObject, field));
                //double ti = System.Convert.ToDouble(ceiling) + System.Convert.ToDouble(finishes) + System.Convert.ToDouble(flooring) + System.Convert.ToDouble(furniture) + System.Convert.ToDouble(intDoors) + System.Convert.ToDouble(intGlazing) + System.Convert.ToDouble(partitionWalls);

                //return structures + foundations + enclosures + mep + ti;
            }
            return null;
        }
    }
}
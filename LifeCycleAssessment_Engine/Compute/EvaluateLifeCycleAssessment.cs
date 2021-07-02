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

using System.Collections.Generic;
using System.ComponentModel;
using BH.oM.Reflection.Attributes;
using BH.oM.LifeCycleAssessment;
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
        [Input("phases", "Provide phases of life you wish to evaluate. Phases of life must be documented within EPDs for this method to work.")]
        [Input("exactMatch", "If true, the evaluation method will force an exact LCA phase match to solve for.")]
        [Output("result", "The LifeCycleElementResult that contains project data, total results, and results per element.")]
        public static LifeCycleAssessmentResult EvaluateLifeCycleAssessment(ProjectLifeCycleAssessment lca, EnvironmentalProductDeclarationField field, List<LifeCycleAssessmentPhases> phases, bool exactMatch = false)
        {
            if (lca == null) return null;

            List<LifeCycleAssessmentElementResult> results = new List<LifeCycleAssessmentElementResult>();

            results.AddRange(lca.StructuresScope.EvaluateLifeCycleAssessmentScope(field, phases, exactMatch));
            results.AddRange(lca.FoundationsScope.EvaluateLifeCycleAssessmentScope(field, phases, exactMatch));
            results.AddRange(lca.EnclosuresScope.EvaluateLifeCycleAssessmentScope(field, phases, exactMatch));
            results.AddRange(lca.MEPScope.EvaluateLifeCycleAssessmentScope(field, phases, exactMatch));
            results.AddRange(lca.TenantImprovementScope.EvaluateLifeCycleAssessmentScope(field, phases, exactMatch));

            return new LifeCycleAssessmentResult(lca.BHoM_Guid, field, 0, lca.LifeCycleAssessmentScope, new System.Collections.ObjectModel.ReadOnlyCollection<LifeCycleAssessmentElementResult>(results), results.TotalFieldQuantity());
        }
        /***************************************************/
    }
}

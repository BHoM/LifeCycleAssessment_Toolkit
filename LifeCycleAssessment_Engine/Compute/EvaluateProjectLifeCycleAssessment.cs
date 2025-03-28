/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2025, the respective contributors. All rights reserved.
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

using BH.oM.Dimensional;
using BH.oM.LifeCycleAssessment;
using BH.oM.LifeCycleAssessment.Results;
using BH.oM.Base.Attributes;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System;
using BH.oM.Physical.Materials;

namespace BH.Engine.LifeCycleAssessment
{
    public static partial class Compute
    {
        /***************************************************/
        /****   Public Methods                          ****/
        /***************************************************/

        [Description("This method calculates the total field quantity specified for an entire project or a collection of elements. Metadata included on the ProjectLCA object is not accounted for within this calculation.")]
        [Input("projectLCA", "Project LCA can be used to collect all objects used in an evaluation along with the project's specific metatdata for tracking within a database.")]
        [Input("field", "Filter the provided EnvironmentalProductDeclaration by selecting one of the provided metrics for calculation. This method also accepts multiple fields simultaneously.")]
        [Output("quantity", "The total quantity of the specified metric.")]
        public static double EvaluateProjectLifeCycleAssessment(ProjectLifeCycleAssessment projectLCA, EnvironmentalMetrics field, List<Material> templateMaterials = null, bool prioritiseTemplate = true)
        {
            if(projectLCA == null)
            {
                Base.Compute.RecordError("No Project LCA was provided.");
                return double.NaN;
            }
            
            List<IElementM> elements = projectLCA.Elements;

            if (elements.Count <=0)
            {
                Base.Compute.RecordError("No elements were found in the Project LCA.");
                return double.NaN;
            }

            var results = projectLCA.Elements.SelectMany(x => Query.EnvironmentalResults(x, templateMaterials, prioritiseTemplate, new List<EnvironmentalMetrics> { field })).ToList();

            return results.Sum(x => x.Total());
        }
        /***************************************************/


    }
}





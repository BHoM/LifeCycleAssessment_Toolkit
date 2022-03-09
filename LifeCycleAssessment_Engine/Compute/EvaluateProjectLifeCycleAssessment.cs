/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2022, the respective contributors. All rights reserved.
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

namespace BH.Engine.LifeCycleAssessment
{
    public static partial class Compute
    {
        /***************************************************/
        /****   Public Methods                          ****/
        /***************************************************/

        [Description("This method calculates the total field quantity specified for an entire project or a collection of elements. Metadata included on the ProjectLCA object is not accounted for within this calculation.")]
        [Input("projectLCA", "Project LCA can be used to collect all objects used in an evaluation along with the project's specific metatdata for tracking within a database.")]
        [Input("phases", "Provide phases of life you wish to evaluate. Phases of life must be documented within EPDs for this method to work.")]
        [Input("field", "Filter the provided EnvironmentalProductDeclaration by selecting one of the provided metrics for calculation. This method also accepts multiple fields simultaneously.")]
        [Input("exactMatch", "If true, the evaluation method will force an exact LCA phase match to solve for.")]
        [Output("quantity", "The total quantity of the specified metric.")]
        [PreviousVersion("5.1", "BH.Engine.Compute.EvaluateLifeCycleAssessment(BH.oM.LifeCycleAssessment.ProjectLifeCycleAssessment, BH.oM.EnvironmentalProductDeclarationField, System.Collections.Generic.List<BH.oM.LifeCycleAssessmentPhases>, System.Boolean)")]
        public static double EvaluateProjectLifeCycleAssessment(ProjectLifeCycleAssessment projectLCA, List<LifeCycleAssessmentPhases> phases, EnvironmentalProductDeclarationField field = EnvironmentalProductDeclarationField.GlobalWarmingPotential, bool exactMatch = false)
        {
            List<IElementM> elements = projectLCA.Elements;

            if (elements.Count <=0)
            {
                Base.Compute.RecordError("No elements were found in the Project LCA.");
                return double.NaN;
            }

            List<LifeCycleAssessmentElementResult> elementResults = new List<LifeCycleAssessmentElementResult>();

            for (int i=0; i<elements.Count(); i++)
            {
                IElementM element = elements[i];
                LifeCycleAssessmentElementResult evalElement = EvaluateEnvironmentalProductDeclaration(element, phases, field, exactMatch);
                elementResults.Add(evalElement);         
            }

            double quantity = Query.TotalFieldQuantity(elementResults);

            return quantity;
        }
        /***************************************************/


    }
}


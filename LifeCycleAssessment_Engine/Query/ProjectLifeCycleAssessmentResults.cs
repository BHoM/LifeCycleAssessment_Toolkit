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

using BH.oM.LifeCycleAssessment.Results;
using BH.oM.Reflection.Attributes;
using System;
using System.ComponentModel;
using System.Linq;
using BH.oM.Environment.Climate;

namespace BH.Engine.LifeCycleAssessment
{
    public static partial class Query
    {
        /***************************************************/
        /****   Public Methods                          ****/
        /***************************************************/

        [Description("Query the standard project results from a complete ProjectLifeCycleAssessment evaluation for pushing to datasets.")]
        [Input("lcaResult", "Supply a complete LifeCycleAssessmentResult to query.")]
        [Output("projectResults", "The combined amount of kgCO2e of the objects provided.")]
        public static ProjectLifeCycleAssessmentResult ProjectLifeCycleAssessmentResults(this LifeCycleAssessmentResult lcaResult)
        {
            double area = lcaResult.LifeCycleAssessmentScope.ProjectArea;
            if (area <= 0)
            {
                BH.Engine.Reflection.Compute.RecordNote("No building area was defined in the LCA Scope. Returning zero.");
                area = 0;
            }

            double buildingLifespan = lcaResult.LifeCycleAssessmentScope.BuildingLifespan;
            if (double.IsNaN(buildingLifespan))
            {
                BH.Engine.Reflection.Compute.RecordNote("No building lifespan has been provided for this entry. Using zero as building lifespan.");
                buildingLifespan = 0;
            }

            string constructionScope = "";
            if (lcaResult.LifeCycleAssessmentScope.ConstructionScopeNew)
                constructionScope = "New Construction";
            else
                constructionScope = "Renovation";

            string contactName = lcaResult.LifeCycleAssessmentScope.ContactName;
            if (contactName == "")
            {
                BH.Engine.Reflection.Compute.RecordNote("Please enter your contact information within the LifeCycleAssessmentScope.");
                contactName = "Undefined";
            }

            string elementScope = "";
            if(lcaResult.Results.Count() > 0)
            {
                foreach (var b in lcaResult.Results.Select(x => x.Scope).Distinct().ToList())
                    elementScope += b.ToString() + ",";
                    
                elementScope = elementScope.Remove(elementScope.LastIndexOf(','));
            }
            else
            {
                BH.Engine.Reflection.Compute.RecordWarning("No objects have been provided for evaluation within the discipline-specific scope objects.");
            }

            string gravitySys = System.Convert.ToString(lcaResult.LifeCycleAssessmentScope.GravityStructuralMaterial);
            if(gravitySys == "Undefined")
                BH.Engine.Reflection.Compute.RecordNote("Please enter your primary gravity structural system within the LifeCycleAssessmentScope.");

            double gwp = lcaResult.TotalQuantity;
            if (gwp <= 0)
            {
                BH.Engine.Reflection.Compute.RecordError("No GWP amount was accounted for. Please ensure you have provided objects with EPDs for evaluation before submitting to the central database.");
                return null;
            }

            double projectArea = lcaResult.LifeCycleAssessmentScope.ProjectArea;
            if (projectArea <= 0)
            {
                BH.Engine.Reflection.Compute.RecordWarning("Please enter your project's total area within the LifeCycleAssessmentScope.");
                return null;
            }

            double gwpPerArea = 0;

            gwpPerArea = gwp / projectArea;

            if (gwpPerArea <= 0)
                BH.Engine.Reflection.Compute.RecordWarning("Insufficient data was provided to calculate GWP per area. Please provide a project area and valid objects.");

            string lateralSys = System.Convert.ToString(lcaResult.LifeCycleAssessmentScope.LateralStructuralMaterial);
            if (lateralSys == "Undefined")
                BH.Engine.Reflection.Compute.RecordNote("Please enter your primary lateral structural system within the LifeCycleAssessmentScope.");

            string lcaPhases = "";
            if(lcaResult.LifeCycleAssessmentScope.LifeCycleAssessmentPhases.Count() >= 1)
            {
                foreach (var c in lcaResult.LifeCycleAssessmentScope.LifeCycleAssessmentPhases)
                    lcaPhases += c.ToString() + ",";
                    
                lcaPhases = lcaPhases.Remove(lcaPhases.LastIndexOf(','));
            }
            else
            {
                BH.Engine.Reflection.Compute.RecordWarning("Please enter the specific LCA phases you are reporting within the LifeCycleAssessmentScope.");
                lcaPhases = "Undefined";
            }

            string lod = System.Convert.ToString(lcaResult.LifeCycleAssessmentScope.LevelOfDevelopment);
            if (lod == "Undefined")
                BH.Engine.Reflection.Compute.RecordNote("Please enter your project's level of development upon reporting within the LifeCycleAssessmentScope.");

            string projectId = lcaResult.LifeCycleAssessmentScope.AdditionalNotes;
            if(lcaResult.LifeCycleAssessmentScope.AdditionalNotes == "")
            {
                BH.Engine.Reflection.Compute.RecordNote("Please enter your project's identifier the in LifeCycleAssessmentScope AdditionalNotes.");
                projectId = "Undefined";
            }           

            IComparable objectId = lcaResult.ObjectId;
            IComparable resultCase = lcaResult.ResultCase;
            DateTime timeStamp = lcaResult.TimeStep;
            DateTime date = DateTime.Now;
              
            return new ProjectLifeCycleAssessmentResult(area, buildingLifespan, constructionScope, contactName, date, elementScope, gravitySys, gwp, gwpPerArea, lateralSys, lod, objectId, lcaPhases, projectId, resultCase, timeStamp);
        }
        /***************************************************/
    }
}

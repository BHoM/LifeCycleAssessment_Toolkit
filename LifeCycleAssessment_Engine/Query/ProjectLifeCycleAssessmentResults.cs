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
        public static ProjectLifeCycleAssessmentResult ProjectLifeCycleAssessmentResult(this LifeCycleAssessmentResult lcaResult)
        {
            double buildingLifespan = lcaResult.LifeCycleAssessmentScope.BuildingLifespan;
            if (double.IsNaN(buildingLifespan))
            {
                BH.Engine.Reflection.Compute.RecordNote("No building lifespan has been provided for this entry. Returning zero.");
                buildingLifespan = 0;
            }

            string constructionScope = "";
            if (lcaResult.LifeCycleAssessmentScope.ConstructionScopeNew == true)
                constructionScope = "New Construction";
            else constructionScope = "Renovation";

            string contact = lcaResult.LifeCycleAssessmentScope.ContactName;
            if (contact == "")
            {
                BH.Engine.Reflection.Compute.RecordNote("Please enter your contact information within the LifeCycleAssessmentScope.");
                contact = "Undefined";
            }

            string elementScope = "";
            if(lcaResult.Results.Select(x => x.Scope).Count() > 0)
            {
                foreach (var b in lcaResult.Results.ToList())
                    elementScope += b.Scope.ToString() + ",";
                elementScope = elementScope.Remove(elementScope.LastIndexOf(','));
            }
            else
            {
                BH.Engine.Reflection.Compute.RecordWarning("No objects have been provided for evaluation within the discipline-specific scope objects.");
            }

            string gravitySys = System.Convert.ToString(lcaResult.LifeCycleAssessmentScope.GravityStructuralMaterial);
            if(gravitySys == "Undefined")
                BH.Engine.Reflection.Compute.RecordNote("Please enter your primary gravity structural system within the LifeCycleAssessmentScope.");

            double gwp = lcaResult.TotalGlobalWarmingPotential;
            if (gwp <= 0)
            {
                BH.Engine.Reflection.Compute.RecordError("No GWP amount was accounted for. Please ensure you have provided objects with EPDs for evaluation before submitting to the central database.");
                return null;
            }

            double projectArea = lcaResult.LifeCycleAssessmentScope.ProjectArea;
            if (projectArea <= 0)
            {
                BH.Engine.Reflection.Compute.RecordWarning("Please enter your project's total area within the LifeCycleAssessmentScope.");
                projectArea = double.NaN;
            }

            double gwpPerArea = 0;
            if (double.IsNaN(projectArea))
            {
                BH.Engine.Reflection.Compute.RecordError("No area was recorded for this project. The key metric of GWP per area cannot be calculated.");
                return null;
            }

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
                BH.Engine.Reflection.Compute.RecordNote("Please enter your project's identifier the LifeCycleAssessmentScope AdditionalNotes.");
                projectId = "Undefined";
            }           

            object location = lcaResult.LifeCycleAssessmentScope.Location;
            string projectLocation = "";
            if (location != null)
            {
                string latitude = System.Convert.ToString(lcaResult.LifeCycleAssessmentScope.Location.Latitude);
                string longitude = System.Convert.ToString(lcaResult.LifeCycleAssessmentScope.Location.Longitude);
                projectLocation = String.Concat(latitude, longitude + ",");
            }
            else
            {
                projectLocation = "Undefined";
                BH.Engine.Reflection.Compute.RecordWarning("Please enter your project's location in Lat Long format within the LifeCycleAssessmentScope.");
            }

            string projectName = lcaResult.LifeCycleAssessmentScope.ProjectName;
            if (projectName == "")
            {
                BH.Engine.Reflection.Compute.RecordWarning("Please enter your project name within the LifeCycleAssessmentScope.");
                projectName = "Undefined";
            }

            string projectType = System.Convert.ToString(lcaResult.LifeCycleAssessmentScope.ProjectType);
            if (projectType == "Undefined")
                BH.Engine.Reflection.Compute.RecordWarning("Please enter your project's type within the LifeCycleAssessmentScope.");

            IComparable objectId = lcaResult.ObjectId;
            IComparable resultCase = lcaResult.ResultCase;
            double timeStamp = lcaResult.TimeStep;
            string date = System.Convert.ToString(System.DateTime.Now);
              
            return new ProjectLifeCycleAssessmentResult(buildingLifespan, constructionScope, contact, elementScope, gravitySys, gwp, gwpPerArea, lateralSys, lcaPhases, lod, projectArea, projectId, projectLocation, projectName, projectType, objectId, resultCase, timeStamp, date);
        }
        /***************************************************/
    }
}


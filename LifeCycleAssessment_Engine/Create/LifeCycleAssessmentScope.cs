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
using System.Text;
using System.ComponentModel;
using BH.oM.Base;

using BH.oM.Reflection.Attributes;
using BH.oM.LifeCycleAssessment;

namespace BH.Engine.LifeCycleAssessment
{
    public static partial class Create
    {
        [Description("The Life Cycle Assessment Scope object intends to provide a means of reporting all of the project criteria (name, area, type, location) as well as the objects that the study encompassed (structural slabs, foundation walls, etc) along with their properties for the Enviornmental Product Declarations they used (when using SetProperty), their densities and volumes. This object may be used for studies at any stage of development and can serve as a true means of 'apples to apples' comparison when catalogued.")]
        [Input("projectArea", "This is an Enum of project areas to help categorize your project Life Cycle Assessment.")]
        [Input("projectType", "This is an Enum listing the overall type of functional program that takes place throughout the project.")]
        [Input("levelOfDevelopment", "This is an Enum listing the level of development of your project. This is commonly refered to as LOD100, LOD200 etc. Also known as project phase.")]
        [Input("lifeCycleAssessmentPhases", "This is an Enum listing the phases encorporated within your Life Cycle Assessment.")]
        [Input("primaryStructuralMaterial", "This is an Enum listing the primary structural material for the project.")]
        [Input("projectName", "The Project Name denotes the name of the project (eg Mercedes-Benz Stadium)")]
        [Input("contactName", "The Contact Name denotes the person/people who performed the LCA study")]
        [Input("actualProjectArea", "The Actual Project Area denotes the more precise project area (m2) which will allow assessment of kgCO2eq/m2 metrics")]
        [Input("zipCode", "Zip Code is the means of tracking the project's location")]
        [Input("biogenicCarbon", "Biogenic Carbon is a true / false that indicates that the project contains materials that originated from a biological source(trees, soil), these materials have the ability sequester / store carbon.")]
        [Input("additionalNotes", "Additional notes should convey project design constraints (eg design for seismic activity) that could affect the overall embodied carbon")]
        [Output("lifeCycleAssessment", "A lifeCycleAssessment object for capturing and comparing additional studies. This object can be passed directly to a database for storage and further study.")]
        public static LifeCycleAssessmentScope LifeCycleAssessmentScope(ProjectArea projectArea, ProjectType projectType, LevelOfDevelopment levelOfDevelopment, List<LifeCycleAssessmentPhases> lifeCycleAssessmentPhases, PrimaryStructuralMaterial primaryStructuralMaterial,
            
            string projectName = "",
            string contactName = "",
            double actualProjectArea = 0,
            bool biogenicCarbon = false,
            int zipCode = 00000,
            string additionalNotes = ""
        )
        {
            return new oM.LifeCycleAssessment.LifeCycleAssessmentScope
            {
                ProjectName = projectName,
                ContactName = contactName,
                ActualProjectArea = actualProjectArea,
                BiogenicCarbon = biogenicCarbon,
                ZipCode = zipCode,
                AdditionalNotes = additionalNotes,
                //
                //match order of inputs and add all properties. 
                //list of LCA phases
            };
        }
    }
}
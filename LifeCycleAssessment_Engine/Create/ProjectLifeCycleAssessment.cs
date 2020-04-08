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
        [Description("The Project Life Cycle Assessment object intends to provide a means of reporting all of the project criteria (name, area, type, location) as well as the objects that the study encompassed (structural slabs, foundation walls, etc) along with their properties for the Enviornmental Product Declarations they used, their densities and volumes. This object may be used for studies at any stage of development and can serve as a true means of 'apples to apples' comparison when catalogued.")]
        [Input("lifeCycleAssessmentScope", "The Life Cycle Assessment Scope object collects all details of the intended Life Cycle Assessment study for accurate comparison of assumptions and results. Please provide as many details about your project as possible to ensure the most accurate study.")]
        [Input("structuresScope", "The Structures Scope object collects all structural objects being evaluated within any Life Cycle Assessment.")]
        [Input("enclosuresScope", "The Enclosures Scope object collects all enclosures objects being evaluated within any Life Cycle Assessment.")]
        [Input("foundationsScope", "The Foundations Scope object collects all foundations objects being evaluated within any Life Cycle Assessment.")]
        [Input("MEPScope", "The MEP Scope object collects all MEP objects being evaluated within any Life Cycle Assessment.")]
        [Input("tenantImprovementScope", "The Tenant Improvement Scope object collects all Tenant Improvement objects being evaluated within any Life Cycle Assessment.")]
        [Output("lifeCycleAssessment", "A collection of objects organized and prepared for Life Cycle Assessment evaluation.")]
        public static ProjectLifeCycleAssessment LifeCycleAssessment(LifeCycleAssessmentScope lifeCycleAssessmentScope,
            StructuresScope structuresScope = null,
            EnclosuresScope enclosuresScope = null,
            FoundationsScope foundationsScope = null,
            MEPScope mepScope = null,
            TenantImprovementScope tenantImprovementScope = null
        )
        {
            return new oM.LifeCycleAssessment.ProjectLifeCycleAssessment
            {
                LifeCycleAssessmentScope = lifeCycleAssessmentScope,
                StructuresScope = structuresScope,
                EnclosuresScope = enclosuresScope,
                FoundationsScope = foundationsScope,
                MEPScope = mepScope,
                TenantImprovementScope = tenantImprovementScope,
            };
        }
    }
}
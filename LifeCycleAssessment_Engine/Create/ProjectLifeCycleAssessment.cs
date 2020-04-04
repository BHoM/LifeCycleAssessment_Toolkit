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
        [Input("lifeCycleAssessmentScope", "PLACEHOLDER DESCRIPTION.")]
        [Input("structuresScope", "PLACEHOLDER DESCRIPTION.")]
        [Input("enclosuresScope", "PLACEHOLDER DESCRIPTION.")]
        [Input("foundationsScope", "PLACEHOLDER DESCRIPTION.")]
        [Input("MEPScope", "PLACEHOLDER DESCRIPTION.")]
        [Input("tenantImprovementScope", "PLACEHOLDER DESCRIPTION.")]
        [Output("lifeCycleAssessment", "A lifeCycleAssessment object for capturing and comparing additional studies. This object can be passed directly to a database for storage and further study.")]
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
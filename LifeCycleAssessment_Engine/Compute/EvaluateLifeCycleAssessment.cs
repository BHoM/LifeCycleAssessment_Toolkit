﻿/*
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
using BH.Engine.Reflection;
using BH.oM.Reflection;
using BH.Engine.Matter;

namespace BH.Engine.LifeCycleAssessment
{
    public static partial class Compute
    {
        /***************************************************/
        /****   Public Methods                          ****/
        /***************************************************/

        [Description("This evaluation method will extract environmental product declaration data (EPD) from all provided geometries within each scope object. Please review the report output for a better understanding of what you have provided and what areas you can improve for the most accurate life cycle assessment..")]
        [Input("projectLifeCycleAssessment", "A complete Project Life Cycle Assessment. ")]
        [Input("environmentalProductDeclarationField", "Filter the provided EnvironmentalProductDeclaration by selecting one of the provided metrics for calculation. This method also accepts multiple fields simultaneously.")]
        [MultiOutput(0, "quantity", "this is the quantity.")]
        [MultiOutput(1, "report", "this is a detailed report of your life cycle assessment.")]

        public static Output<double, string> EvaluateLifeCycleAssessment(ProjectLifeCycleAssessment projectLifeCycleAssessment, EnvironmentalProductDeclarationField environmentalProductDeclarationField = EnvironmentalProductDeclarationField.GlobalWarmingPotential) //add sort method
        {
            Output<double, string> report = new Output<double, string>
            {
                Item1 = projectLifeCycleAssessment.StructuresScope.GetEvaluationValue(environmentalProductDeclarationField) + projectLifeCycleAssessment.EnclosuresScope.GetEvaluationValue(environmentalProductDeclarationField) + projectLifeCycleAssessment.FoundationsScope.GetEvaluationValue(environmentalProductDeclarationField) + projectLifeCycleAssessment.MEPScope.GetEvaluationValue(environmentalProductDeclarationField) + projectLifeCycleAssessment.TenantImprovementScope.GetEvaluationValue(environmentalProductDeclarationField) * (projectLifeCycleAssessment.StructuresScope.StructuresSlabs.Slabs.ISolidVolume()),
                Item2 = "My Report"
            };
            //field * volume * density 
            return report;

        }
    }
}
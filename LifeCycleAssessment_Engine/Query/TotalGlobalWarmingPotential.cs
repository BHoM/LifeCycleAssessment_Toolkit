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

using BH.oM.LifeCycleAssessment;
using BH.oM.LifeCycleAssessment.Results;
using BH.oM.Reflection.Attributes;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.LifeCycleAssessment
{
    public static partial class Query
    {
        [Description("Query the total global warming potential, or GWP from any LifeCycleAssessmentElementResult data. This is typically evaluated by kgCO2e or kg of Carbon Dioxide equivalent.")]
        [Input("gwpResults", "Supply a valid GlobalWarmingPotentialResult from a scope evaluation.")]
        [Output("gwp", "The combined amount of kgCO2e of the objects provided.")]
        public static double TotalGlobalWarmingPotential(this List<LifeCycleAssessmentElementResult> gwpResults)
        {
            return gwpResults.Where(x => x is GlobalWarmingPotentialResult).Select(x => (x as GlobalWarmingPotentialResult).GlobalWarmingPotential).Sum();
        }
    }
}

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

using BH.oM.LifeCycleAssessment.Results;
using BH.oM.Base.Attributes;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace BH.Engine.LifeCycleAssessment
{
    public static partial class Query
    {
        [Description("Query the total quantity from any LifeCycleAssessmentElementResult data. Be mindful of the specific unit specified in the Field enum.")]
        [Input("results", "Supply a valid LifeCycleAssessmentElementResult from a scope evaluation.")]
        [Output("totalFieldQuantity", "The combined amount of kgCO2e of the objects provided.")]
        public static double TotalFieldQuantity(this List<LifeCycleAssessmentElementResult> results)
        {
            return results.Where(x => x is EnvironmentalMetricResult).Select(x => (x as EnvironmentalMetricResult).Quantity).Sum();
        }
    }
}



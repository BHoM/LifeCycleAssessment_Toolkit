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

using BH.oM.Dimensional;
using BH.oM.LifeCycleAssessment;
using BH.oM.LifeCycleAssessment.MaterialFragments;
using BH.oM.Reflection.Attributes;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace BH.Engine.LifeCycleAssessment
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Query an Environmental Product Declaration MaterialFragment to return it's LCA Phase property value where any exists.")]
        [Input("epd", "The EPD object to query.")]
        [Output("phases", "A list of all phases used within the EPD.")]
        public static List<List<LifeCycleAssessmentPhases>> GetEPDPhases(this EnvironmentalProductDeclaration epd)
        {
            // EPD null check
            if (epd == null)
            {
                BH.Engine.Reflection.Compute.RecordError("No EPD was provided.");
                return new List<List<LifeCycleAssessmentPhases>>();
            }

            // Get list of all EPD EnvironmentalMetrics
            List<EnvironmentalMetric> metrics = epd.EnvironmentalMetric;
            List<List<LifeCycleAssessmentPhases>> phases = metrics.Select(x => x.Phases).Distinct().ToList();

            if (phases.Count <= 0)
            {
                BH.Engine.Reflection.Compute.RecordError("No Phases have been found within the EPD.");
                return new List<List<LifeCycleAssessmentPhases>>();
            }

            return phases;
        }

        /***************************************************/

        [Description("Query an IElementM to return it's LCA Phase property value where any exists within applied Environmental Product Declarations.")]
        [Input("elementM", "The IElementM object to query.")]
        [Output("phases", "A list of all phases used within the EPD.")]
        public static List<List<LifeCycleAssessmentPhases>> GetEPDPhases(this IElementM elementM)
        {
            // Element null check
            if (elementM == null)
            {
                BH.Engine.Reflection.Compute.RecordError("No IElementM was provided.");
                return new List<List<LifeCycleAssessmentPhases>>();
            }

            // Get all the epds from the elements 
            List<EnvironmentalProductDeclaration> epd = GetElementEpd(elementM);

            if (epd == null)
            {
                BH.Engine.Reflection.Compute.RecordError($"No EPDs could be found within element {elementM.GetType()}.");
                return new List<List<LifeCycleAssessmentPhases>>();
            }

            // Get list of all EPD EnvironmentalMetrics
            List<EnvironmentalMetric> metrics = (List<EnvironmentalMetric>)epd.Select(x => x.EnvironmentalMetric);
            if (metrics.Count() <= 0)
            {
                BH.Engine.Reflection.Compute.RecordError($"No environmental metrics could be found within element {elementM.GetType()}.");
                return new List<List<LifeCycleAssessmentPhases>>();
            }

            // Get list of all Phases
            List<List<LifeCycleAssessmentPhases>> phases = metrics.Select(x => x.Phases).Distinct().ToList();

            if (phases.Count <= 0)
            {
                BH.Engine.Reflection.Compute.RecordError("No Phases have been found within the EPD.");
                return new List<List<LifeCycleAssessmentPhases>>();
            }

            return phases;
        }
    }
}


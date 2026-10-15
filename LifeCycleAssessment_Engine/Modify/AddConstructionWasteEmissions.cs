/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2026, the respective contributors. All rights reserved.
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


using BH.oM.Base.Attributes;
using BH.oM.Geometry;
using BH.oM.LifeCycleAssessment;
using BH.oM.LifeCycleAssessment.MaterialFragments;
using BH.oM.LifeCycleAssessment.MaterialFragments.Construction;
using BH.oM.LifeCycleAssessment.MaterialFragments.Transport;
using BH.oM.LifeCycleAssessment.Results;
using BH.oM.Physical.Materials;
using BH.oM.Quantities;
using BH.oM.Quantities.Attributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace BH.Engine.LifeCycleAssessment
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Calculates and adds construction waste emissions (A5_3 module) to the resulting module values dictionary based on construction emissions parameters. The method computes waste factors according to IStructE guidance and adds the A5_3 module representing construction waste impacts.")]
        [Input("resultingValues", "Dictionary of module values to which the construction emissions will be added. Must contain required modules (A1toA3 or A1+A2+A3, A4, C3toC4 or C3 or C4, and optionally C2 if not reused on site).")]
        [Input("constructionEmissions", "Construction emissions parameters including waste rate and whether materials are reused on site.")]
        [Input("metricType", "The metric type being evaluated, used for error reporting when required modules are missing.")]
        [Output("resultingValues", "The input dictionary is modified in place with the A5_3 module added, representing construction waste emissions calculated as: (cradle-to-gate + transport + disposal) × waste factor.")]
        public static void AddConstructionWasteEmissions(this Dictionary<Module, double> resultingValues, ConstructionWasteEmissions constructionEmissions, MetricType metricType)
        {
            double a5_3 = resultingValues.ConstructionWasteEmissions(constructionEmissions, metricType);
            if (double.IsNaN(a5_3))
                return;

            resultingValues[Module.A5_3] = a5_3;

            if(Query.PartOfCombinationModules().TryGetValue(Module.A5_3, out var parentCombinations))
            {
                foreach(var parentCombination in parentCombinations)
                {
                    resultingValues.Remove(parentCombination);  //Remove parent combinations to force recalculation when material results are created
                }
            }
        }

        /***************************************************/

    }
}




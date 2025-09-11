/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2025, the respective contributors. All rights reserved.
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
            List<Module> requiredWasteModules = new List<Module>() { Module.A1toA3, Module.A4 };


            List<Module> disposalModules = new List<Module>() { Module.C3toC4, Module.C3, Module.C4 };

            double total = 0;
            List<Module> missingModules = new List<Module>();

            //Get out the cradle to gate metrics
            if (resultingValues.TryGetValue(Module.A1toA3, out double a1a3))
                total += a1a3;
            else if(resultingValues.TryGetValue(Module.A1, out double a1) && resultingValues.TryGetValue(Module.A2, out double a2) && resultingValues.TryGetValue(Module.A3, out double a3))
                total += a1 + a2 + a3;
            else
                missingModules.Add(Module.A1toA3);

            if(resultingValues.TryGetValue(Module.A4, out double a4))
                total += a4;
            else
                missingModules.Add(Module.A4);

            if (!constructionEmissions.ResuedOnSite)
            {
                if (resultingValues.TryGetValue(Module.C2, out double c2))
                    total += c2; //Add C2 if not re-used on site
                else
                    missingModules.Add(Module.C2);
            }

            if (resultingValues.TryGetValue(Module.C3toC4, out double c3c4))
                total += c3c4;
            else
            {
                //For some EPDs it might be that only one of C3 or C4 is reported, so check for both and add whichever is present
                //Quite common for EPDs to only report C3 or C4
                //However if both are present, then both should be used.
                bool hasAtLeastOne = false;
                if (resultingValues.TryGetValue(Module.C3, out double c3))
                {
                    hasAtLeastOne = true;
                    total += c3;
                }

                if (resultingValues.TryGetValue(Module.C4, out double c4))
                {
                    hasAtLeastOne = true;
                    total += c4;
                }
                
                if(!hasAtLeastOne)
                    missingModules.Add(Module.C3toC4);
            }

            if (missingModules.Any())
            {
                string modulesMissingNames = string.Join(", ", missingModules);

                string message = $"Missing modules for waste computation of waste factors (A5.3) for metric of type {metricType}: {string.Join(", ", missingModules)} (or their subparts).";
                BH.Engine.Base.Compute.RecordError(message);
                return;
            }

            //From IStructE guidance
            //The waste factor WFi is calculated by converting the waste rate WRi (a percentage of the quantity of materials brought
            //to the site that are wasted) to the quantity of materials wasted on site as a percentage of the material quantities used
            //in the final asset(Eqn. 2.5):
            double wasteFactor = (1 / (1 - constructionEmissions.WasteRate.Rate)) - 1;
            resultingValues[Module.A5_3] = total * wasteFactor;

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



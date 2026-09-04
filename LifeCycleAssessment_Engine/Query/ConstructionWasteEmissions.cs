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
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Calculates construction waste emissions (A5_3 module) based on construction emissions parameters. The method computes waste factors according to IStructE guidance and returns the A5_3 module representing construction waste impacts.")]
        [Input("resultingValues", "Dictionary of module values to which the construction emissions will be added. Must contain required modules (A1toA3 or A1+A2+A3, A4, C3toC4 or C3 or C4, and optionally C2 if not reused on site).")]
        [Input("constructionEmissions", "Construction emissions parameters including waste rate and whether materials are reused on site.")]
        [Input("metricType", "The metric type being evaluated, used for error reporting when required modules are missing.")]
        [Output("constructionWasteEmissions", "The calculated construction waste emissions as a double value.")]
        public static double ConstructionWasteEmissions(this Dictionary<Module, double> resultingValues, ConstructionWasteEmissions constructionEmissions, MetricType metricType)
        {

            if(resultingValues == null || constructionEmissions?.WasteRate == null)
                return double.NaN;

            double total = 0;
            List<Module> missingModules = new List<Module>();

            //Get out the cradle to gate metrics
            if (resultingValues.TryGetModuleValue(Module.A1toA3, out double a1a3))
                total += a1a3;
            else
                missingModules.Add(Module.A1toA3);

            //Get out the transport metrics
            if(resultingValues.TryGetModuleValue(Module.A4, out double a4))
                total += a4;
            else
                missingModules.Add(Module.A4);

            //Get out the disposal metrics
            if (!constructionEmissions.ResuedOnSite)
            {
                if (resultingValues.TryGetModuleValue(Module.C2, out double c2))
                    total += c2; //Add C2 if not re-used on site
                else
                    missingModules.Add(Module.C2);
            }

            //Get out the disposal metrics
            if (resultingValues.TryGetModuleValue(Module.C3toC4, out double c3c4, false))
                total += c3c4;
            else
                missingModules.Add(Module.C3toC4);

            if (missingModules.Any())
            {
                string message = $"Missing modules for waste computation of waste factors (A5.3) for metric of type {metricType}: {string.Join(", ", missingModules)} (or their subparts).";
                BH.Engine.Base.Compute.RecordError(message);
                return double.NaN;
            }

            return total * constructionEmissions.WasteFactor();

        }

        /***************************************************/

    }
}




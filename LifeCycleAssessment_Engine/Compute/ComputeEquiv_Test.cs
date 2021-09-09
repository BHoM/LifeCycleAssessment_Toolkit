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

using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using BH.oM.Base;
using BH.oM.Reflection.Attributes;
using BH.oM.Structure.MaterialFragments;
using BH.oM.LifeCycleAssessment;
using BH.oM.LifeCycleAssessment.Results;

namespace BH.Engine.LifeCycleAssessment
{
    public static partial class Compute
    {
        /***************************************************/
        /****   Public Methods                          ****/
        /***************************************************/

        [Description("Returns EPA equivalency.")]
        [Input("kgCO2", "LCA result for Global Warming Potential")]
        [Input("equivalencyType", "Choose from enum options - specify which enum object?")]
        [Output("totalSomething", "A description to help here")]
        public static double ComputeEquivalency(double kgCO2, EquivalencyType equivalencyType)
        {
            double equivalencyVal = 0;

            switch (equivalencyType)
            {
                case EquivalencyType.Undefined:
                    BH.Engine.Reflection.Compute.RecordError("You have not provided an equivalency type.");
                    return 0;

                case EquivalencyType.GallonsOfGasoline:
                    BH.Engine.Reflection.Compute.RecordNote("You selected Gallons of Gasoline and the method is using formula.");
                    var evalByGallons = 651.68; // Shivanie to add specific formulas from excel example.
                    equivalencyVal = evalByGallons;
                    break;

                case EquivalencyType.MilesDriven:
                    BH.Engine.Reflection.Compute.RecordNote("You selected miles driven and the method is using a constant of 65161.");
                    var evalByMiles = 200; // Shivanie to add specific formulas from excel example.
                    equivalencyVal = evalByMiles;
                    break;

                default:
                    BH.Engine.Reflection.Compute.RecordWarning("Check your inputs and try again. dum");
                    return 0;
            }
            // results 
            return kgCO2 * equivalencyVal;
        }

        /***************************************************/

        [Description("Returns EPA equivalency.")]
        [Input("result", "LCA result for Global Warming Potential")]
        [Input("equivalencyType", "Choose from enum options - specify which enum object?")]
        [Output("totalSomething", "A description to help here")]
        public static double ComputeEquivalency(List<LifeCycleAssessmentElementResult> result, EquivalencyType equivalencyType)
        {
            double kgCO2 = Query.TotalFieldQuantity(result);

            double equivalencyVal = 0;

            switch (equivalencyType)
            {
                case EquivalencyType.Undefined:
                    BH.Engine.Reflection.Compute.RecordError("You have not provided an equivalency type.");
                    return 0;

                case EquivalencyType.GallonsOfGasoline:
                    BH.Engine.Reflection.Compute.RecordNote("You selected Gallons of Gasoline and the method is using formula.");
                    var evalByGallons = 651.68; // Shivanie to add specific formulas from excel example.
                    equivalencyVal = evalByGallons;
                    break;

                case EquivalencyType.MilesDriven:
                    BH.Engine.Reflection.Compute.RecordNote("You selected miles driven and the method is using a constant of 65161.");
                    var evalByMiles = 200; // Shivanie to add specific formulas from excel example.
                    equivalencyVal = evalByMiles;
                    break;

                default:
                    BH.Engine.Reflection.Compute.RecordWarning("Check your inputs and try again. dum");
                    return 0;
            }
            // results 
            return kgCO2 * equivalencyVal;
        }
        /***************************************************/
    }
}

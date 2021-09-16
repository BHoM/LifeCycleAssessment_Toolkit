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

        [Description("Returns EPA equivalency using a U.S.A. specific dataset")]
        [Input("kgCO2", "LCA result for Global Warming Potential")]
        [Input("equivalencyType", "Resultant unit for equivalency conversion")]
        [Output("quantity", "The numerical value for the resultant unit")]
        public static double EmbodiedCarbonEquivalency(double kgCO2, EquivalencyType equivalencyType)
        {
            double equivalencyVal = 0;
            string message = $"You selected {equivalencyType} and the method is using a constant of";

            switch (equivalencyType)
            {
                case EquivalencyType.Undefined:
                    BH.Engine.Reflection.Compute.RecordError("You have not provided an equivalency type.");
                    return 0;

                case EquivalencyType.MilesDriven:
                    var evalByMiles = 3.98E-4 / 1000;
                    equivalencyVal = evalByMiles;
                    BH.Engine.Reflection.Compute.RecordNote($"{message} {evalByMiles} kg CO2 per mile in a year.");
                    break;

                case EquivalencyType.PassengerVehiclesDriven:
                    var evalByVehicles = 4.6 / 1000;
                    equivalencyVal = evalByVehicles;
                    BH.Engine.Reflection.Compute.RecordNote($"{message} {evalByVehicles} kg CO2 per vehicle in a year.");
                    break;

                case EquivalencyType.HomesEnergyUse:
                    var evalByHomes = 8.3 / 1000;
                    equivalencyVal = evalByHomes;
                    BH.Engine.Reflection.Compute.RecordNote($"{message} {evalByHomes} kg CO2 per home electricity use in a year.");
                    break;

                case EquivalencyType.GallonsOfGasoline:
                    var evalByGallons = 8.887E-3 / 1000;
                    equivalencyVal = evalByGallons;
                    BH.Engine.Reflection.Compute.RecordNote($"{message} {evalByGallons} kg CO2 per gallon.");
                    break;

                case EquivalencyType.PoundsOfCoal:
                    var evalByCoalPounds = 9.05E-4 / 1000;
                    equivalencyVal = evalByCoalPounds;
                    BH.Engine.Reflection.Compute.RecordNote($"{message} {evalByCoalPounds} kg CO2 per pound of coal.");
                    break;

                case EquivalencyType.AcresOfForest:
                    var evalByAcres = 9.05E-4 / 1000;
                    equivalencyVal = evalByAcres;
                    BH.Engine.Reflection.Compute.RecordNote($"{message} {evalByAcres} kg CO2 sequestered per acre of forest in a year.");
                    break;

                default:
                    BH.Engine.Reflection.Compute.RecordWarning("Check your inputs and try again.");
                    return 0;
            }
            // results 


            return kgCO2 * equivalencyVal;
        }

        /***************************************************/

        [Description("Returns EPA equivalency using a U.S.A. specific dataset with a Result input.")]
        [Input("result", "LCA result for Global Warming Potential")]
        [Input("equivalencyType", "Resultant unit for equivalency conversion")]
        [Output("quantity", "The numerical value for the resultant unit")]
        public static double EmbodiedCarbonEquivalency(List<LifeCycleAssessmentElementResult> result, EquivalencyType equivalencyType)
        {
            double kgCO2 = Query.TotalFieldQuantity(result);

            double equivalencyVal = 0;
            string message = $"You selected {equivalencyType} and the method is using a constant of";

            switch (equivalencyType)
            {
                case EquivalencyType.Undefined:
                    BH.Engine.Reflection.Compute.RecordError("You have not provided an equivalency type.");
                    return 0;

                case EquivalencyType.MilesDriven:
                    var evalByMiles = 3.98E-4 / 1000;
                    equivalencyVal = evalByMiles;
                    BH.Engine.Reflection.Compute.RecordNote($"{message} {evalByMiles} kg CO2 per mile in a year.");
                    break;

                case EquivalencyType.PassengerVehiclesDriven:
                    var evalByVehicles = 4.6/1000;
                    equivalencyVal = evalByVehicles;
                    BH.Engine.Reflection.Compute.RecordNote($"{message} {evalByVehicles} kg CO2 per vehicle in a year.");
                    break;

                case EquivalencyType.HomesEnergyUse:
                    var evalByHomes = 8.3/1000;
                    equivalencyVal = evalByHomes;
                    BH.Engine.Reflection.Compute.RecordNote($"{message} {evalByHomes} kg CO2 per home electricity use in a year.");
                    break;

                case EquivalencyType.GallonsOfGasoline:
                    var evalByGallons = 8.887E-3 / 1000;
                    equivalencyVal = evalByGallons;
                    BH.Engine.Reflection.Compute.RecordNote($"{message} {evalByGallons} kg CO2 per gallon.");
                    break;

                case EquivalencyType.PoundsOfCoal:
                    var evalByCoalPounds = 9.05E-4 / 1000;
                    equivalencyVal = evalByCoalPounds;
                    BH.Engine.Reflection.Compute.RecordNote($"{message} {evalByCoalPounds} kg CO2 per pound of coal.");
                    break;

                case EquivalencyType.AcresOfForest:
                    var evalByAcres = 9.05E-4 / 1000;
                    equivalencyVal = evalByAcres;
                    BH.Engine.Reflection.Compute.RecordNote($"{message} {evalByAcres} kg CO2 sequestered per acre of forest in a year.");
                    break;

                default:
                    BH.Engine.Reflection.Compute.RecordWarning("Check your inputs and try again.");
                    return 0;
            }
            // results 
            return kgCO2 / equivalencyVal;
        }
        /***************************************************/
    }
}

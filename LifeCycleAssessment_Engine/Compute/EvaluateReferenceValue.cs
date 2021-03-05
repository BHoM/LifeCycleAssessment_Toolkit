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

using BH.oM.LifeCycleAssessment;
using BH.oM.LifeCycleAssessment.MaterialFragments;
using BH.oM.Reflection.Attributes;
using System.ComponentModel;

namespace BH.Engine.LifeCycleAssessment
{
    public static partial class Compute
    {
        /***************************************************/
        /****   Public Methods                          ****/
        /***************************************************/

        [Description("This is a simple calculation method for EPD QuantityTypes that are not yet fully supported. \n" +
            "This calculation is performed by multiplying the quantity by the selected field metric found within the EPD. \n" +
            "This method relys upon user input and is therefore at the discression of the user to verify all results.")]
        [Input("quantity", "The amount, quantity, or value to evaluate against any Environmental Product Declaration.")]
        [Input("epd", "The Environmental Product Declaration to evaluate against the quantity.")]
        [Input("field", "The Environmental indicator to evaluate by. This value is queried from the EPD.")]
        [Output("totalQuantity", "The total quantity of the desired metric based on the EnvironmentalProductDeclarationField.")]
        public static double EvaluateReferenceValue(double quantity, IEnvironmentalProductDeclarationData epd, EnvironmentalProductDeclarationField field)
        {
            if (epd == null)
            {
                BH.Engine.Reflection.Compute.RecordError("No EPD provided. Please provide a reference EPD.");
            }

            double epdValue = Query.GetEvaluationValue(epd, field);

            if (quantity <= 0)
            {
                BH.Engine.Reflection.Compute.RecordError("No evaluation value was found within the EPD. Please try another.");
            }

            string qt = System.Convert.ToString(Query.GetFragmentQuantityType(epd));

            BH.Engine.Reflection.Compute.RecordNote($"TotalQuantity is created using {qt} QuantityType extracted from " + epd.Name + ".");

            double totalQuantity = quantity * epdValue;

            return totalQuantity;
        }
        /***************************************************/

    }
}
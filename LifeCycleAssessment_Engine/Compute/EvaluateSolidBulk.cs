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

using System.ComponentModel;
using BH.oM.Reflection.Attributes;
using BH.oM.LifeCycleAssessment;
using BH.oM.LifeCycleAssessment.Results;
using BH.oM.Physical.Elements;

namespace BH.Engine.LifeCycleAssessment
{
    public static partial class Compute
    {
        /***************************************************/
        /****   Public Methods                          ****/
        /***************************************************/

        [Description("This method calculates the results of any selected metric within an Environmental Product Declaration for a SolidBulk.\n" +
            "This compute method will determine whether to solve by Volume or by Mass depending on the EPD QuantityType value.\n" +
            "If Volume QuantityType is used, the SolidVolume of the geometry will be utilised in the calculation.\n" +
            "If Mass QuantityType is used, the Element's mass will be calculated based on the MaterialComposition's density values.\n" +
            "All values can be assessed at any point by using the appropriate element query methods (i.e. Mass(SolidBulk), or SolidVolume(SolidBulk)).")]
        [Input("solidBulk", "SolidBulks are any object not currently formatted as a standard BHoM object (i.e. bars, floors, or panels), but can host a MaterialComposition for material based analysis.")]
        [Input("field", "Filter the provided EnvironmentalProductDeclaration by selecting one of the provided metrics for calculation. This method also accepts multiple fields simultaneously.")]
        [Output("result", "A LifeCycleElementResult that contains the LifeCycleAssessment data for the input object.")]
        public static LifeCycleAssessmentElementResult EvaluateSolidBulk(SolidBulk solidBulk, EnvironmentalProductDeclarationField field = EnvironmentalProductDeclarationField.GlobalWarmingPotential)
        {
            QuantityType qt = solidBulk.GetFragmentQuantityType();

            switch (qt)
            {
                case QuantityType.Undefined:
                    BH.Engine.Reflection.Compute.RecordError("The object's EPD QuantityType is Undefined and cannot be evaluated.");
                    return null;
                case QuantityType.Area:
                    BH.Engine.Reflection.Compute.RecordError("EvaluateSolidBulk currently only supports EPD evaluation based on Volume or Mass QuantityTypes.");
                    return null;
                case QuantityType.Item:
                    BH.Engine.Reflection.Compute.RecordError("EvaluateSolidBulk currently only supports EPD evaluation based on Volume or Mass QuantityTypes.");
                    return null;
                case QuantityType.Length:
                    BH.Engine.Reflection.Compute.RecordError("EvaluateSolidBulk currently only supports EPD evaluation based on Volume or Mass QuantityTypes.");
                    return null;
                case QuantityType.Mass:
                    BH.Engine.Reflection.Compute.RecordNote("Evaluating object based on EPD Mass QuantityType.");
                    return EvaluateSolidBulkByMass(solidBulk, field);
                case QuantityType.Volume:
                    BH.Engine.Reflection.Compute.RecordNote("Evaluating object based on EPD Volume QuantityType.");
                    return EvaluateSolidBulkByVolume(solidBulk, field);
                default:
                    BH.Engine.Reflection.Compute.RecordWarning("The object you have provided does not contain an EPD Material Fragment.");
                    return null;
            }
        }
        /***************************************************/


    }
}
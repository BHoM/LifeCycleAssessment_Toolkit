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

using System.Linq;
using System.ComponentModel;
using BH.oM.Base;
using BH.oM.Reflection.Attributes;
using BH.oM.LifeCycleAssessment;
using BH.oM.LifeCycleAssessment.MaterialFragments;
using BH.oM.LifeCycleAssessment.Results;
using BH.Engine.Base;
using BH.Engine.Matter;
using BH.oM.Physical.Elements;
using BH.oM.Dimensional;

namespace BH.Engine.LifeCycleAssessment
{
    public static partial class Compute
    {
        /***************************************************/
        /****   Public Methods                          ****/
        /***************************************************/

        [Description("This method calculates the total value of the selected environmental impact metric.\n" +
            "This compute method functions by multiplying the object's Mass by the EPD environmental impact metric.\n" +
            "Mass is derived from the materialComposition's density values.")]
        [Input("bulkSolids", "BulkSolids are any object not currently formatted as a standard BHoM object (i.e. bars, floors, or panels), but can host a MaterialComposition for material based analysis.")]
        [Input("field", "Filter the provided EnvironmentalProductDeclaration by selecting one of the provided metrics for calculation.")]
        [Output("quantity", "The total quantity of the desired metric based on the EnvironmentalProductDeclarationField")]
        public static GlobalWarmingPotentialResult EvaluateExplicitBulkByMass(ExplicitBulk explicitBulk = null, EnvironmentalProductDeclarationField field = EnvironmentalProductDeclarationField.GlobalWarmingPotential)
        {
            if (explicitBulk.GetFragmentQuantityType() != QuantityType.Mass)
            {
                BH.Engine.Reflection.Compute.RecordError("This EnvironmentalProductDeclaration's QuantityType is not Mass. Please supply a Mass-based EPD or try a different method.");
                return null;
            }
            else
            {
                double epdVal = ((IElementM)explicitBulk).GetEvaluationValue(field);
                double mass = explicitBulk.Mass();
                double quantityTypeValue = explicitBulk.GetQuantityTypeValue();

                if (epdVal <= 0 || epdVal == double.NaN)
                {
                    BH.Engine.Reflection.Compute.RecordError($"No value for {field} can be found within the supplied EPD."); 
                    return null;
                }

                if (mass <= 0 || mass == double.NaN)
                {
                    BH.Engine.Reflection.Compute.RecordError("Mass cannot be calculated from object " + ((IBHoMObject)explicitBulk).BHoM_Guid);
                    return null;
                }

                if (quantityTypeValue != 1)
                {
                    BH.Engine.Reflection.Compute.RecordNote($"Using QuantityTypeValue of {quantityTypeValue} taken from EPD.");
                    epdVal /= quantityTypeValue;
                }

                double quantity = mass * epdVal;

                return new GlobalWarmingPotentialResult(((IBHoMObject)explicitBulk).BHoM_Guid, field, 0, ObjectScope.Undefined, ObjectCategory.Undefined, ((IBHoMObject)explicitBulk).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault(), quantity);
            }
        }

        /***************************************************/
    }
}
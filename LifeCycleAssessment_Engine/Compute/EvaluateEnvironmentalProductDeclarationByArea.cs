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
using BH.oM.Dimensional;
using BH.Engine.Base;
using BH.Engine.Spatial;
using BH.oM.Physical.Elements;

namespace BH.Engine.LifeCycleAssessment
{
    public static partial class Compute
    {
        /***************************************************/
        /****   Public Methods                          ****/
        /***************************************************/

        [Description("This method calculates the quantity of a supplied metric by querying Environmental Impact Metrics from the EPD materialFragment and the object's area.")]
        [Input("elementM", "An IElementM object used to calculate EPD metric.")]
        [Input("field", "Filter the provided EnvironmentalProductDeclaration by selecting one of the provided metrics for calculation.")]
        [Output("quantity", "The total quantity of the desired metric based on the EnvironmentalProductDeclarationField")]
        public static GlobalWarmingPotentialResult EvaluateEnvironmentalProductDeclarationByArea(IElementM elementM = null, EnvironmentalProductDeclarationField field = EnvironmentalProductDeclarationField.GlobalWarmingPotential)
        {
            if (elementM.GetFragmentQuantityType() != QuantityType.Area)
            {
                BH.Engine.Reflection.Compute.RecordError("This EnvironmentalProductDeclaration's QuantityType is not Area. Please supply an Area-based EPD or try a different method.");
                return null;
            }
            else
            {
                if (elementM is SolidBulk || elementM is ExplicitBulk)
                {
                    BH.Engine.Reflection.Compute.RecordError("Element of type: " + elementM.GetType() + " does not support area-based evaluations, which is required based on your current EPD assignment.\n Please review the EPD QuantityType and confirm that your objects are compatible.");
                    return null;
                }

                double area = (elementM as IElement2D).Area();
                double epdVal = elementM.GetEvaluationValue(field);
                double quantityTypeValue = elementM.GetQuantityTypeValue();              

                if (epdVal <= 0 || epdVal == double.NaN)
                {
                    BH.Engine.Reflection.Compute.RecordError($"No value for {field} can be found within the supplied EPD.");
                    return null;
                }

                if (area <= 0 || area == double.NaN)
                {
                    BH.Engine.Reflection.Compute.RecordError("Area cannot be calculated from object " + ((IBHoMObject)elementM).BHoM_Guid);
                    return null;
                }

                if (quantityTypeValue != 1)
                {
                    BH.Engine.Reflection.Compute.RecordNote($"Using QuantityTypeValue of {quantityTypeValue} taken from EPD.");
                    epdVal /= quantityTypeValue;
                }

                double quantity = area * epdVal;

                return new GlobalWarmingPotentialResult(((IBHoMObject)elementM).BHoM_Guid, field, 0, ObjectScope.Undefined, ObjectCategory.Undefined, ((IBHoMObject)elementM).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault(), quantity);
            }
        }

        /***************************************************/
    }
}
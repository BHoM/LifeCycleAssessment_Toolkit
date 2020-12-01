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
using BH.oM.Physical.Elements;

namespace BH.Engine.LifeCycleAssessment
{
    public static partial class Compute
    {
        /***************************************************/
        /****   Public Methods                          ****/
        /***************************************************/

        [Description("This method calculates the total value of the selected environmental impact metric./n" +
            "This compute method functions by multiplying the object's SolidVolume by the EPD environmental impact metric.")]
        [Input("bulkSolids", "BulkSolids are any object not currently formatted as a standard BHoM object (i.e. bars, floors, or panels), but can host a MaterialComposition for material based analysis.")]
        [Input("field", "Filter the provided EnvironmentalProductDeclaration by selecting one of the provided metrics for calculation.")]
        [Output("quantity", "The total quantity of the desired metric based on the EnvironmentalProductDeclarationField")]
        public static GlobalWarmingPotentialResult EvaluateBulkSolidsByVolume(SolidBulk solidBulk = null, EnvironmentalProductDeclarationField field = EnvironmentalProductDeclarationField.GlobalWarmingPotential)
        {
            if (solidBulk.GetFragmentQuantityType() != QuantityType.Volume)
            {
                BH.Engine.Reflection.Compute.RecordError("This EnvironmentalProductDeclaration's declared unit type is not Volume. Please supply a Volume-based EPD or try a different method.");
                return null;
            }
            else
            {
                double epdVal = (solidBulk as IBHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetEvaluationValue(field);

                if (epdVal <= 0 || epdVal == double.NaN)
                {
                    BH.Engine.Reflection.Compute.RecordError($"No value for {field} can be found within the supplied EPD.");
                    return null;
                }
                //double volume = BH.Engine.Geometry.Query.Volume((dynamic)bulkSolids.Geometry);
                double volume = BH.Engine.Geometry.Query.IVolume((oM.Geometry.ISolid)solidBulk.Geometry);

                if (volume <= 0)
                {
                    BH.Engine.Reflection.Compute.RecordError("Volume cannot be calculated from object " + ((IBHoMObject)solidBulk).BHoM_Guid);
                    return null;
                }

                double quantity = volume * epdVal;

                return new GlobalWarmingPotentialResult(((IBHoMObject)solidBulk).BHoM_Guid, "GWP", 0, ObjectScope.Undefined, ObjectCategory.Undefined, ((IBHoMObject)solidBulk).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault(), quantity);
            }
        }
        /***************************************************/
    }
}
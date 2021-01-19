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

using System.Linq;
using System.ComponentModel;
using System.Collections.Generic;
using BH.oM.Base;
using BH.oM.Reflection.Attributes;
using BH.oM.LifeCycleAssessment;
using BH.oM.LifeCycleAssessment.MaterialFragments;
using BH.oM.LifeCycleAssessment.Results;
using BH.oM.Dimensional;
using BH.Engine.Base;
using BH.Engine.Matter;

namespace BH.Engine.LifeCycleAssessment
{
    public static partial class Compute
    {
        /***************************************************/
        /****   Public Methods                          ****/
        /***************************************************/

        [Description("This method calculates the quantity of a supplied metric by querying Environmental Impact Metrics from the EPD materialFragment and the object's volume.")]
        [Input("elementM", "An IElementM object used to calculate EPD metric.")]
        [Input("field", "Filter the provided EnvironmentalProductDeclaration by selecting one of the provided metrics for calculation.")]
        [Output("quantity", "The total quantity of the desired metric based on the EnvironmentalProductDeclarationField.")]
        private static GlobalWarmingPotentialResult EvaluateEnvironmentalProductDeclarationByVolume(IElementM elementM = null, EnvironmentalProductDeclarationField field = EnvironmentalProductDeclarationField.GlobalWarmingPotential)
        {
            List<double> epdVal = elementM.GetEvaluationValue(field);
            double volume = elementM.ISolidVolume();
            List<double> volumeByRatio = elementM.IMaterialComposition().Ratios.Select(x => volume * x).ToList();
            List<double> gwpByMaterial = new List<double>();

            for (int x = 0; x < epdVal.Count; x++)
                gwpByMaterial.Add(epdVal[x] * volumeByRatio[x]);

            if (epdVal.Sum() <= 0 || epdVal == null)
            {
                BH.Engine.Reflection.Compute.RecordError($"No value for {field} can be found within the supplied EPD.");
                return null;
            }

            if (volume <= 0)
            {
                BH.Engine.Reflection.Compute.RecordError("Volume cannot be calculated from object " + ((IBHoMObject)elementM).BHoM_Guid);
                return null;
            }

            double quantity = gwpByMaterial.Sum();

            return new GlobalWarmingPotentialResult(((IBHoMObject)elementM).BHoM_Guid, field, 0, ObjectScope.Undefined, ObjectCategory.Undefined, ((IBHoMObject)elementM).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault(), quantity);
        }

        /***************************************************/
    }
}

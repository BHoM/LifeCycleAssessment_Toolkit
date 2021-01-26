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

        [Description("This method calculates the quantity of a supplied metric by querying Environmental Impact Metrics from the EPD materialFragment and the object's mass.")]
        [Input("elementM", "An IElementM object used to calculate EPD metric.")]
        [Input("field", "Filter the provided EnvironmentalProductDeclaration by selecting one of the provided metrics for calculation.")]
        [Output("quantity", "The total quantity of the desired metric based on the EnvironmentalProductDeclarationField.")]
        private static GlobalWarmingPotentialResult EvaluateEnvironmentalProductDeclarationByMass(IElementM elementM = null, EnvironmentalProductDeclarationField field = EnvironmentalProductDeclarationField.GlobalWarmingPotential)
        {
            double volume = elementM.ISolidVolume();
            List<double> epdVal = elementM.GetEvaluationValue(field, QuantityType.Mass);
            List<double> gwpByMaterial = new List<double>();
            List<double> volumeByRatio = elementM.IMaterialComposition().Ratios.Select(x => volume * x).ToList();
            List<double> densityOfMassEpd = Query.GetElementDensity(elementM);
            List<double> massOfObj = new List<double>();

            if (densityOfMassEpd == null)
            {
                BH.Engine.Reflection.Compute.RecordError("Density could not be found. Material density is required for all objects using Mass-based evaluations.");
                return null;
            }

            for (int x = 0; x < volumeByRatio.Count; x++)
            {
                massOfObj.Add(volumeByRatio[x] * densityOfMassEpd[x]);
            }

            for (int x = 0; x < epdVal.Count; x++)
            {
                if (double.IsNaN(epdVal[x]))
                    gwpByMaterial.Add(double.NaN);
                else
                {
                    gwpByMaterial.Add(epdVal[x] * massOfObj[x]);
                }
            }

            if (epdVal == null || epdVal.Where(x => !double.IsNaN(x)).Sum() <= 0)
            {
                BH.Engine.Reflection.Compute.RecordError($"No value for {field} can be found within the supplied EPD.");
                return null;
            }

            double quantity = gwpByMaterial.Where(x => !double.IsNaN(x)).Sum();

            return new GlobalWarmingPotentialResult(((IBHoMObject)elementM).BHoM_Guid, field, 0, ObjectScope.Undefined, ObjectCategory.Undefined, ((IBHoMObject)elementM).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault(), quantity);
        }

        /***************************************************/
    }
}

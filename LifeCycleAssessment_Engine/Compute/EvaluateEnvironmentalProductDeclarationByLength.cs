/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2022, the respective contributors. All rights reserved.
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

using BH.Engine.Spatial;
using BH.oM.Base;
using BH.oM.Dimensional;
using BH.oM.LifeCycleAssessment;
using BH.oM.LifeCycleAssessment.Results;
using BH.oM.Reflection.Attributes;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace BH.Engine.LifeCycleAssessment
{
    public static partial class Compute
    {
        /***************************************************/
        /****   Public Methods                          ****/
        /***************************************************/

        [Description("This method calculates the quantity of a supplied metric by querying Environmental Impact Metrics from the EPD materialFragment and the object's area.")]
        [Input("elementM", "An IElementM object used to calculate EPD metric.")]
        [Input("phases", "Provide phases of life you wish to evaluate. Phases of life must be documented within EPDs for this method to work.")]
        [Input("field", "Filter the provided EnvironmentalProductDeclaration by selecting one of the provided metrics for calculation.")]
        [Input("exactMatch", "If true, the evaluation method will force an exact LCA phase match to solve for.")]
        [Output("quantity", "The total quantity of the desired metric based on the EnvironmentalProductDeclarationField.")]
        private static EnvironmentalMetricResult EvaluateEnvironmentalProductDeclarationByLength(IElementM elementM, List<LifeCycleAssessmentPhases> phases, EnvironmentalProductDeclarationField field = EnvironmentalProductDeclarationField.GlobalWarmingPotential, bool exactMatch = false)
        {
            if (elementM is IElement1D)
            {
               
                double length = (elementM as IElement1D).Length();
                List<double> epdVal = elementM.GetEvaluationValue(field, phases ,QuantityType.Length, exactMatch);
                List<double> gwpByMaterial = new List<double>();

                for (int x = 0; x < epdVal.Count; x++)
                {
                    if (double.IsNaN(epdVal[x]))
                        gwpByMaterial.Add(double.NaN);
                    else
                        gwpByMaterial.Add(epdVal[x] * length);
                }

                if (epdVal == null || epdVal.Where(x => !double.IsNaN(x)).Sum() <= 0)
                {
                    BH.Engine.Reflection.Compute.RecordError($"No value for {field} can be found within the supplied EPD.");
                    return null;
                }

                if (length <= 0 || length == double.NaN)
                {
                    BH.Engine.Reflection.Compute.RecordError("Length cannot be calculated from object " + ((IBHoMObject)elementM).BHoM_Guid);
                    return null;
                }

                double quantity = gwpByMaterial.Where(x => !double.IsNaN(x)).Sum();

                return new EnvironmentalMetricResult(((IBHoMObject)elementM).BHoM_Guid, field, 0, ObjectScope.Undefined, ObjectCategory.Undefined, phases, Query.GetElementEpd(elementM), quantity, field);
            }
            else
            {
                BH.Engine.Reflection.Compute.RecordError("Length-based evaluations are not supported for objects of type: " + elementM.GetType() + ".");
                return null;
            }
        }

        /***************************************************/
    }
}


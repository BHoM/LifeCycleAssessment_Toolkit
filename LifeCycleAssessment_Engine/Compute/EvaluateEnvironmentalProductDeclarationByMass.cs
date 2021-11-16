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

using BH.Engine.Matter;
using BH.oM.Base;
using BH.oM.Dimensional;
using BH.oM.LifeCycleAssessment;
using BH.oM.LifeCycleAssessment.MaterialFragments;
using BH.oM.LifeCycleAssessment.Results;
using BH.oM.Reflection.Attributes;
using BH.oM.Physical.Materials;
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

        [Description("This method calculates the quantity of a supplied metric by querying Environmental Impact Metrics from the EPD materialFragment and the object's mass.")]
        [Input("elementM", "An IElementM object used to calculate EPD metric.")]
        [Input("phases", "Provide phases of life you wish to evaluate. Phases of life must be documented within EPDs for this method to work.")]
        [Input("field", "Filter the provided EnvironmentalProductDeclaration by selecting one of the provided metrics for calculation.")]
        [Input("exactMatch", "If true, the evaluation method will force an exact LCA phase match to solve for.")]
        [Output("quantity", "The total quantity of the desired metric based on the EnvironmentalProductDeclarationField.")]
        private static EnvironmentalMetricResult EvaluateEnvironmentalProductDeclarationByMass(IElementM elementM, List<LifeCycleAssessmentPhases> phases, EnvironmentalProductDeclarationField field = EnvironmentalProductDeclarationField.GlobalWarmingPotential, bool exactMatch = false)
        {
            double volume = elementM.ISolidVolume();
            List<double> gwpByMaterial = new List<double>();
            MaterialComposition mc = elementM.IMaterialComposition();
            List<Material> matList = mc.Materials.ToList();

            List<double> epdVals = elementM.GetEvaluationValue(field, phases, QuantityType.Mass, exactMatch);
            if (epdVals == null || epdVals.Where(x => !double.IsNaN(x)).Sum() <= 0)
            {
                BH.Engine.Reflection.Compute.RecordError($"No value for {field} can be found within the supplied EPD.");
                return null;
            }

            for (int i = 0; i < matList.Count(); i++)
            {
                double density;
                List<EnvironmentalProductDeclaration> materialEPDs = matList[i].Properties.OfType<EnvironmentalProductDeclaration>().ToList();
                if (materialEPDs.Any(x => x.QuantityType == QuantityType.Mass))
                {
                    double volumeOfMaterial = mc.Ratios[i] * volume;
                    List<double> densityOfMassEpd = materialEPDs.Where(x => x.QuantityType == QuantityType.Mass).First().GetEPDDensity();
                    if (densityOfMassEpd == null || densityOfMassEpd.Count() == 0)
                    {
                        BH.Engine.Reflection.Compute.RecordError("Density could not be found. Add DensityFragment for all objects using Mass-based QuantityType EPDs.");
                        return null;
                    }
                    else
                        density = densityOfMassEpd[0];

                    double massOfObj = volumeOfMaterial * density;
                    if (double.IsNaN(epdVals[i]))
                        gwpByMaterial.Add(double.NaN);
                    else
                        gwpByMaterial.Add(epdVals[i] * massOfObj);
                }
            }
            double quantity = gwpByMaterial.Where(x => !double.IsNaN(x)).Sum();
            return new EnvironmentalMetricResult(((IBHoMObject)elementM).BHoM_Guid, field, 0, ObjectScope.Undefined, ObjectCategory.Undefined, phases, Query.GetElementEpd(elementM), quantity, field);
        }
        /***************************************************/
    }
}

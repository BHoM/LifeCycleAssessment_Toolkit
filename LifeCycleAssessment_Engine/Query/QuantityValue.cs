/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2025, the respective contributors. All rights reserved.
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

using BH.Engine.Base;
using BH.oM.Base;
using BH.oM.Base.Attributes;
using BH.oM.LifeCycleAssessment;
using BH.oM.LifeCycleAssessment.Fragments;
using BH.oM.LifeCycleAssessment.MaterialFragments;
using BH.oM.LifeCycleAssessment.MaterialFragments.Transport;
using BH.oM.Physical.Materials;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace BH.Engine.LifeCycleAssessment
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Gets the amount of a particular quantity from the takeof item given the provided quantity type.")]
        [Input("materialTakeoffItem", "The takeof item to extract the quantity from.")]
        [Input("quantityType", "The type of quantity to extract.")]
        [Output("quantityValue", "The amout of the provided quantity type on the material takeof item. Unit will be SI unit corresponding to the quantity type.")]
        public static double QuantityValue(this TakeoffItem materialTakeoffItem, QuantityType quantityType)
        {
            double quantityValue;
            switch (quantityType)
            {
                case QuantityType.Volume:
                    quantityValue = materialTakeoffItem.Volume;
                    break;
                case QuantityType.Mass:
                    quantityValue = materialTakeoffItem.Mass;
                    if (double.IsNaN(quantityValue) || quantityValue == 0)
                    {
                        double vol = materialTakeoffItem.Volume;
                        if (vol == 0)
                            quantityValue = 0;
                        else if (!double.IsNaN(vol))
                        {
                            double density = materialTakeoffItem.Material.Density;
                            if (double.IsNaN(density) || density == 0)
                            {
                                //Keeping support for backwards compability for old workflows relying epd density assigned as fragments to EPDS
                                //Generally not recomended to work with this fragment
                                EPDDensity epdDensity = materialTakeoffItem.Material.Properties.OfType<IEnvironmentalFactorsProvider>().Select(x => x.FindFragment<EPDDensity>()).Where(x => x != null).FirstOrDefault();
                                if (epdDensity != null)
                                    density = epdDensity.Density;
                            }
                            if (!double.IsNaN(density))
                                quantityValue = vol * density;
                        }
                    }
                    break;
                case QuantityType.Length:
                    quantityValue = materialTakeoffItem.Length;
                    break;
                case QuantityType.Area:
                    quantityValue = materialTakeoffItem.Area;
                    break;
                case QuantityType.Item:
                    quantityValue = materialTakeoffItem.NumberItem;
                    break;
                case QuantityType.Ampere:
                    quantityValue = materialTakeoffItem.ElectricCurrent;
                    break;
                case QuantityType.VoltAmps:
                case QuantityType.Watt:
                    quantityValue = materialTakeoffItem.Power;
                    break;
                case QuantityType.VolumetricFlowRate:
                    quantityValue = materialTakeoffItem.VolumetricFlowRate;
                    break;
                case QuantityType.Energy:
                    quantityValue = materialTakeoffItem.Energy;
                    break;
                case QuantityType.Undefined:
                default:
                    Base.Compute.RecordError($"{quantityType} QuantityType is currently not supported.");
                    quantityValue = double.NaN;
                    break;
            }

            return quantityValue;

        }

        /***************************************************/

    }
}

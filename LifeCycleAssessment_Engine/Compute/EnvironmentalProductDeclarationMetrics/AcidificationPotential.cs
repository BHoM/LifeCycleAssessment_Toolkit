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

using System.ComponentModel;
using BH.oM.Base;
using BH.oM.Reflection.Attributes;
using BH.oM.LifeCycleAssessment.MaterialFragments;
using BH.oM.Dimensional;
using BH.Engine.Matter;
using BH.oM.LifeCycleAssessment;

namespace BH.Engine.LifeCycleAssessment
{
    public static partial class Compute
    {
        /***************************************************/
        /****   Public Methods                          ****/
        /***************************************************/

        [Description("Calculates the AcidificationPotential of a BHoM Object based on a Mass-based QuantityType Environmental Product Declaration dataset.")]
        [Input("elementM", "The IElementM Object to calculate the acidification potential.")]
        [Input("epd", "A BHoM Environmental Product Declaration object with a valid value for acidification potential.")]
        [Output("acidificationPotential", "Acidification, measured in kgSO2e, refers to emissions which increase the H+ ions in the environment causing a decrease in pH. Potential effects include fish mortality, forest decline, and the deterioration of building materials.")]
        public static double AcidificationPotential(IElementM elementM, IEnvironmentalProductDeclarationData epd)
        {
            QuantityType qt = epd.QuantityType;

            if(qt != QuantityType.Mass)
            {
                Reflection.Compute.RecordError("This method only works with Mass-based QuantityType EPDs. Please provide a different EPD.");
                return double.NaN;
            }
            else
            {
                double volume = elementM.ISolidVolume();
                double density = epd.Density;
                double acidificationPotential = System.Convert.ToDouble(epd.AcidificationPotential);

                if (volume <= 0 || volume == double.NaN)
                {
                    Reflection.Compute.RecordError("Volume cannot be calculated from object " + ((IBHoMObject)elementM).BHoM_Guid);
                    return double.NaN;
                }

                if (density <= 0 || density == double.NaN)
                {
                    Reflection.Compute.RecordError("EPD does not contain a value for Density");
                    return double.NaN;
                }

                if (acidificationPotential <= 0 || acidificationPotential == double.NaN)
                {
                    Reflection.Compute.RecordError("EPD does not contain a value for AcidificationPotential");
                    return double.NaN;
                }

                return volume * density * acidificationPotential;
            }
        }
        /***************************************************/
    }
}


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

using System;
using System.Collections.Generic;
using System.Linq;
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

        [Description("Calculates the OzoneDepletionPotential of a BHoM Object based on a Mass-based QuantityType Environmental Product Declaration dataset.")]
        [Input("elementM", "The IElementM Object to calculate the OzoneDepletionPotential.")]
        [Input("epd", "A BHoM Environmental Product Declaration object with a valid value for OzoneDepletionPotential.")]
        [Output("ozoneDepletionPotential", "Ozone Depletion Potential, measured in kg CFC-11 equivalents, refers to emissions which contribute to the depletion of the stratospheric ozone layer.")]
        [PreviousVersion("4.0", "BH.Engine.LifeCycleAssessment.Compute.OzoneDepletionPotential(BH.oM.Base.BHoMObject, BH.oM.Base.CustomObject)")]
        public static double OzoneDepletionPotential(IElementM elementM, IEnvironmentalProductDeclarationData epd)
        {
            QuantityType qt = epd.QuantityType;

            if (qt != QuantityType.Mass)
            {
                Reflection.Compute.RecordError("This method only works with Mass-based QuantityType EPDs. Please provide a different EPD.");
                return double.NaN;
            }
            else
            {
                double volume = elementM.ISolidVolume();
                double density = epd.Density;
                double ozoneDepletionPotential = System.Convert.ToDouble(epd.OzoneDepletionPotential);

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

                if (ozoneDepletionPotential <= 0 || ozoneDepletionPotential == double.NaN)
                {
                    Reflection.Compute.RecordError("EPD does not contain a value for OzoneDepletionPotential");
                    return double.NaN;
                }

                return volume * density * ozoneDepletionPotential;
            }
        }
        /***************************************************/
    }
}


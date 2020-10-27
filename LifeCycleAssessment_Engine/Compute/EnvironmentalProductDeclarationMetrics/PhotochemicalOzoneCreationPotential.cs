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

using System.ComponentModel;
using BH.oM.Base;
using BH.oM.Reflection.Attributes;
using BH.oM.LifeCycleAssessment;
using BH.oM.LifeCycleAssessment.MaterialFragments;
using BH.oM.Dimensional;
using BH.Engine.Matter;

namespace BH.Engine.LifeCycleAssessment
{
    public static partial class Compute
    {
        /***************************************************/
        /****   Public Methods                          ****/
        /***************************************************/

        [Description("Calculates the PhotochemicalOzoneCreationPotential of a BHoM Object based on a Mass-based QuantityType Environmental Product Declaration dataset.")]
        [Input("elementM", "The IElementM Object to calculate the PhotochemicalOzoneCreationPotential.")]
        [Input("epd", "BHoM Data object with a valid value for PhotochemicalOzoneCreationPotential.")]
        [Output("photochemicalOzoneCreationPotential", "Photochemical Ozone Creation Potential, measured in kgO3 equivalents, refers to emissions which contribute to the formation of ground-level smog.")]
        [PreviousVersion("4.0", "BH.Engine.LifeCycleAssessment.Compute.PhotochemicalOzoneCreationPotential(BH.oM.Base.BHoMObject, BH.oM.Base.CustomObject)")]
        public static double PhotochemicalOzoneCreationPotential(IElementM elementM, IEnvironmentalProductDeclarationData epd)
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
                double photochemicalOzoneCreationPotential = System.Convert.ToDouble(epd.PhotochemicalOzoneCreationPotential);

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

                if (photochemicalOzoneCreationPotential <= 0 || photochemicalOzoneCreationPotential == double.NaN)
                {
                    Reflection.Compute.RecordError("EPD does not contain a value for PhotochemicalOzoneCreationPotential");
                    return double.NaN;
                }

                return volume * density * photochemicalOzoneCreationPotential;
            }
        }
        /***************************************************/
    }
}

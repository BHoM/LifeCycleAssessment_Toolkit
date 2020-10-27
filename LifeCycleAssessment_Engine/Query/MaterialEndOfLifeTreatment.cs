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
using BH.oM.Reflection.Attributes;
using BH.oM.LifeCycleAssessment.MaterialFragments;

namespace BH.Engine.LifeCycleAssessment
{
    public static partial class Query
    {
        /***************************************************/
        /****   Public Methods                          ****/
        /***************************************************/

        [Description("Returns End of Life processing information contained within an EPD dataset.")]
        [Input("epd", "Environmental Product Declaration of a specific material from an EPD Dataset.")]
        [Output("materialEndOfLifeTreatment", "End of Life treatment per material. This includes all data collected for LCA stages C1-C4 within a provided EPD dataset.")]
        [PreviousVersion("4.0", "BH.Engine.LifeCycleAssessment.Compute.MaterialEndOfLife(BH.oM.Base.CustomObject)")]
        public static string MaterialEndOfLifeTreatment(IEnvironmentalProductDeclarationData epd)
        {
            if (epd.EndOfLifeTreatment == null)
            {
                BH.Engine.Reflection.Compute.RecordError("The EPD does not contain any EndOfLife data.");
                return null;
            }
            else
            {
                return epd.EndOfLifeTreatment;
            }
        }
        /***************************************************/

    }
}

﻿/*
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using BH.oM.Base;
using BH.oM.Reflection.Attributes;

namespace BH.Engine.LifeCycleAssessment
{
    public static partial class Compute
    {
        /***************************************************/
        /****   Public Methods                          ****/
        /***************************************************/

        [Description("Returns End of Life processing information contained within an EPD dataset.")]
        [Input("epdData", "Environmental Product Declaration of a specific material from an EPD Dataset.")]
        [Output("materialEndOfLifeTreatment", "End of Life treatment per material. This includes all data collected for LCA stages C1-C4 within a provided EPD dataset.")]
        public static string MaterialEndOfLifeTreatment(CustomObject epdData)
        {
            if (epdData.CustomData.ContainsKey("TreatmentEOL"))
            {
                return (string)epdData.CustomData["TreatmentEOL"];
            }
            else
            {
                BH.Engine.Reflection.Compute.RecordError("The EPDDataset must have a valid value for Treatment EOL under a 'TreatmentEOL' key.");
                return "No data to record. Please provide missing data within 'TreatmentEOL' key.";
            }
        }
        /***************************************************/

    }
}
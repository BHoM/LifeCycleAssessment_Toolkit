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

using System.ComponentModel;
using BH.oM.Base.Attributes;
using BH.oM.LifeCycleAssessment.MaterialFragments;
using BH.oM.LifeCycleAssessment.Fragments;
using BH.Engine.Base;
using System.Linq;
using BH.oM.Base;

namespace BH.Engine.LifeCycleAssessment
{
    public static partial class Query
    {
        /***************************************************/
        /****   Public Methods                          ****/
        /***************************************************/

        [PreviousVersion("8.2", "BH.Engine.LifeCycleAssessment.Query.MaterialEndOfLifeTreatment(BH.oM.LifeCycleAssessment.MaterialFragments.EnvironmentalProductDeclaration)")]
        [PreviousInputNames("factorsProvider", "epd")]
        [Description("Returns End of Life processing information contained within an EPD dataset.")]
        [Input("factorsProvider", "The EnvironmentalProductDeclaration of a specific material from an EPD Dataset.")]
        [Output("materialEndOfLifeTreatment", "End of Life treatment per material. This includes all data collected for LCA stages C1-C4 within a provided EPD dataset.")]
        public static string MaterialEndOfLifeTreatment(this IEnvironmentalFactorsProvider factorsProvider)
        {
            // EPD null check
            if (factorsProvider == null)
            {
                BH.Engine.Base.Compute.RecordError("No EPD has been provided.");
                return null;
            }

            // AdditionalEPDData fragment
            AdditionalEPDData dataFragment = factorsProvider.FindFragment<AdditionalEPDData>();

            // AdditionalEPDData fragment null check
            if (dataFragment == null)
            {
                BH.Engine.Base.Compute.RecordError("No AdditionalEPDData fragment has been found. Have you tried AddFragment on the selected EPD?");
                return null;
            }

            string endOfLife = dataFragment.EndOfLifeTreatment;

            return endOfLife;
        }
        /***************************************************/

    }
}






/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2026, the respective contributors. All rights reserved.
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


using BH.oM.Base.Attributes;
using BH.oM.Geometry;
using BH.oM.LifeCycleAssessment;
using BH.oM.LifeCycleAssessment.MaterialFragments;
using BH.oM.LifeCycleAssessment.MaterialFragments.Construction;
using BH.oM.LifeCycleAssessment.MaterialFragments.Transport;
using BH.oM.LifeCycleAssessment.Results;
using BH.oM.Physical.Materials;
using BH.oM.Quantities;
using BH.oM.Quantities.Attributes;
using System;
using System.Collections;
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

        [Description("The waste factor WFi is calculated by converting the waste rate WRi (a percentage of the quantity of materials brought to the site that are wasted)" +
                     " to the quantity of materials wasted on site as a percentage of the material quantities used in the final asset (Eqn. 2.5 from IStructE guidance).")]
        [Input("constructionWasteEmissions", "The construction waste emissions parameters including the waste rate to be converted into a waste factor.")]
        [Output("wasteFactor", "Percentage of the quantity of materials wasted on site as a percentage of the material quantities used in the final asset.")]
        public static double WasteFactor(this ConstructionWasteEmissions constructionWasteEmissions)
        {
            return (constructionWasteEmissions?.WasteRate).WasteFactor();
        }

        /***************************************************/

        [Description("The waste factor WFi is calculated by converting the waste rate WRi (a percentage of the quantity of materials brought to the site that are wasted)" +
                     " to the quantity of materials wasted on site as a percentage of the material quantities used in the final asset (Eqn. 2.5 from IStructE guidance).")]
        [Input("wasteRate", "The WasteRate object storing the waste rate - a percentage of the quantity of materials brought to the site that are wasted. Should be a value between 0 and 1.")]
        [Output("wasteFactor", "Percentage of the quantity of materials wasted on site as a percentage of the material quantities used in the final asset.")]
        public static double WasteFactor(this WasteRate wasteRate)
        {
            if(wasteRate == null)
                return double.NaN;

            return WasteFactor(wasteRate.Rate);

        }

        /***************************************************/

        [Description("The waste factor WFi is calculated by converting the waste rate WRi (a percentage of the quantity of materials brought to the site that are wasted)" +
                     " to the quantity of materials wasted on site as a percentage of the material quantities used in the final asset (Eqn. 2.5 from IStructE guidance).")]
        [Input("wasteRate", "The waste rate WRi - a percentage of the quantity of materials brought to the site that are wasted. Should be a value between 0 and 1.")]
        [Output("wasteFactor", "Percentage of the quantity of materials wasted on site as a percentage of the material quantities used in the final asset.")]
        public static double WasteFactor(double wasteRate)
        {
            return (1 / (1 - wasteRate)) - 1;
        }

        /***************************************************/
    }
}




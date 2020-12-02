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
using System.Collections.Generic;
using BH.oM.Reflection.Attributes;
using BH.oM.LifeCycleAssessment.MaterialFragments;
using BH.oM.Dimensional;
using BH.Engine.Matter;
using System.Linq;

namespace BH.Engine.LifeCycleAssessment
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Query the QuantityTypeValue from any Environmental Product Declaration MaterialFragmment.")]
        [Input("epd", "The EPD Object to query.")]
        [Output("quantityTypeValue", "The quantityTypeValue property from the EPD.")]
        public static double GetQuantityTypeValue(this IEnvironmentalProductDeclarationData epd)
        {
            if (epd == null)
            {
                BH.Engine.Reflection.Compute.RecordWarning("The Environmental Product Declaration QuantityTypeValue could not be assessed. Returning default value of 1.");
                return 1;
            }
            else
            {
                double qtv = epd.QuantityTypeValue;

                return qtv;
            }
        }

        /***************************************************/

        [Description("Query the QuantityTypeValue from any object with a valid construction with Environmental Product Declaration MaterialFragmments.")]
        [Input("elementM", "The IElementM Object to query.")]
        [Output("quantityTypeValue", "The quantityTypeValue property from the IElementM.")]
        public static List<double> GetQuantityTypeValue(this IElementM elementM)
        {
            if (elementM == null)
                return new List<double>();

            List<double> qtv = elementM.IMaterialComposition().Materials.Select(x =>
            {
                var epd = x.Properties.Where(y => y is IEnvironmentalProductDeclarationData).FirstOrDefault() as IEnvironmentalProductDeclarationData;
                if (epd != null)
                    return epd.QuantityTypeValue;
                return 1;
            }).Where(x => x != null).ToList();

            return qtv;
        }

        /***************************************************/
    }
}

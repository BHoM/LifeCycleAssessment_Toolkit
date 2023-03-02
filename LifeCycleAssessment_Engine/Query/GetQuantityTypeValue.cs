/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2022, the respective contributors. All rights reserved.
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
using BH.oM.Base.Attributes;
using BH.oM.LifeCycleAssessment.MaterialFragments;
using BH.oM.Dimensional;
using BH.Engine.Matter;
using System.Linq;
using BH.oM.LifeCycleAssessment;
using BH.oM.Physical.Constructions;
using BH.oM.Physical.Materials;
using BH.Engine.LifeCycleAssessment.Objects;

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
        public static double GetQuantityTypeValue(this EnvironmentalProductDeclaration epd)
        {
            if (epd == null)
            {
                BH.Engine.Base.Compute.RecordWarning("The Environmental Product Declaration QuantityTypeValue could not be assessed. Returning default value of 1.");
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
        [Input("type", "The quantityType to query.")]
        [Output("quantityTypeValue", "The quantityTypeValue property from the IElementM.")]
        public static List<double> GetQuantityTypeValue(this IElementM elementM, QuantityType type)
        {
            MaterialComposition mc = elementM.IMaterialComposition();
            if (mc == null)
            {
                Base.Compute.RecordError("Material composition could not be assessed. Please add materials to your objects and try again.");
                return null;
            }

            return HelperMethods.GetQuantityTypeValue(elementM, type, mc);
        }

        /***************************************************/

        [Description("Query the QuantityTypeValue from any object with a valid construction.")]
        [Input("construction", "The physical construction to query.")]
        [Input("type", "The quantityType to query.")]
        [Output("quantityTypeValue", "The quantityTypeValue property from the IElementM.")]
        public static List<double> GetQuantityTypeValue(this Construction construction, QuantityType type)
        {
            if (construction == null)
                return new List<double>();

            List<double> qtv = construction.Layers.Select(x =>
            {
                var s = x.Material.Properties.Where(y => y is EnvironmentalProductDeclaration).FirstOrDefault() as EnvironmentalProductDeclaration;
                if (s != null && s.QuantityType == type)
                    return s.QuantityTypeValue;
                return 1;
            }).Where(x => x != null).ToList();

            return qtv;
        }

        /***************************************************/
    }
}


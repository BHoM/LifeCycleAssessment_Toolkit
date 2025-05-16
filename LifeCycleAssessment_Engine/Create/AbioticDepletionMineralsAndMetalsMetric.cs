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

using BH.oM.Base;
using BH.oM.Base.Attributes;
using BH.oM.LifeCycleAssessment.MaterialFragments;
using BH.oM.Quantities.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace BH.Engine.LifeCycleAssessment
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Creates a AbioticDepletionMineralsAndMetalsMetric to be applied as part of an EnvironmentalProductDeclaration. If no values are provided (NaN), the module will not be added to the created metric.")]
        [Input("a1", "Raw Material Supply module in the Product stage.", typeof(AbioticDepletionMineralsAndMetalsPerQuantity))]
        [Input("a2", "Transport module in the Product stage.", typeof(AbioticDepletionMineralsAndMetalsPerQuantity))]
        [Input("a3", "Manufacturing module in the Product stage.", typeof(AbioticDepletionMineralsAndMetalsPerQuantity))]
        [Input("a4", "Transport module in the Construction Process stage.", typeof(AbioticDepletionMineralsAndMetalsPerQuantity))]
        [Input("a5", "Construction Installation Process module in the Construction Process stage.", typeof(AbioticDepletionMineralsAndMetalsPerQuantity))]
        [Input("b1", "Use module in the Use stage.", typeof(AbioticDepletionMineralsAndMetalsPerQuantity))]
        [Input("b2", "Maintenance module in the Use stage.", typeof(AbioticDepletionMineralsAndMetalsPerQuantity))]
        [Input("b3", "Repair module in the Use stage.", typeof(AbioticDepletionMineralsAndMetalsPerQuantity))]
        [Input("b4", "Replacement module in the Use stage.", typeof(AbioticDepletionMineralsAndMetalsPerQuantity))]
        [Input("b5", "Refurbishment module in the Use stage.", typeof(AbioticDepletionMineralsAndMetalsPerQuantity))]
        [Input("b6", "Operational Energy Use module in the Use stage.", typeof(AbioticDepletionMineralsAndMetalsPerQuantity))]
        [Input("b7", "Operational Water Use module in the Use stage.", typeof(AbioticDepletionMineralsAndMetalsPerQuantity))]
        [Input("c1", "De-construction Demolition module in the End of Life stage.", typeof(AbioticDepletionMineralsAndMetalsPerQuantity))]
        [Input("c2", "Transport module in the End of Life stage.", typeof(AbioticDepletionMineralsAndMetalsPerQuantity))]
        [Input("c3", "Waste Processing module in the End of Life stage.", typeof(AbioticDepletionMineralsAndMetalsPerQuantity))]
        [Input("c4", "Disposal module in the End of Life stage.", typeof(AbioticDepletionMineralsAndMetalsPerQuantity))]
        [Input("d", "Benefits and loads beyond the system boundary.", typeof(AbioticDepletionMineralsAndMetalsPerQuantity))]
        [Output("metric", "Created AbioticDepletionMineralsAndMetalsMetric.", typeof(AbioticDepletionMineralsAndMetalsPerQuantity))]
        public static AbioticDepletionMineralsAndMetalsMetric AbioticDepletionMineralsAndMetalsMetric(
            double a1 = double.NaN,
            double a2 = double.NaN,
            double a3 = double.NaN,
            double a4 = double.NaN,
            double a5 = double.NaN,
            double b1 = double.NaN,
            double b2 = double.NaN,
            double b3 = double.NaN,
            double b4 = double.NaN,
            double b5 = double.NaN,
            double b6 = double.NaN,
            double b7 = double.NaN,
            double c1 = double.NaN,
            double c2 = double.NaN,
            double c3 = double.NaN,
            double c4 = double.NaN,
            double d = double.NaN)
        {
            return new AbioticDepletionMineralsAndMetalsMetric() { Indicators = FactorsDictionary(a1, a2, a3, a4, a5, b1, b2, b3, b4, b5, b6, b7, c1, c2, c3, c4, d) };
        }

        /***************************************************/

        [Description("Creates a AbioticDepletionMineralsAndMetalsMetric to be applied as part of an EnvironmentalProductDeclaration. Create method to be used when no discrete values for A1, A2 and A3 are available, but only a total value for those 3 phases. If no values are provided (NaN), the module will not be added to the created metric.")]
        [Input("a1toa3", "Full Product stage.", typeof(AbioticDepletionMineralsAndMetalsPerQuantity))]
        [Input("a4", "Transport module in the Construction Process stage.", typeof(AbioticDepletionMineralsAndMetalsPerQuantity))]
        [Input("a5", "Construction Installation Process module in the Construction Process stage.", typeof(AbioticDepletionMineralsAndMetalsPerQuantity))]
        [Input("b1", "Use module in the Use stage.", typeof(AbioticDepletionMineralsAndMetalsPerQuantity))]
        [Input("b2", "Maintenance module in the Use stage.", typeof(AbioticDepletionMineralsAndMetalsPerQuantity))]
        [Input("b3", "Repair module in the Use stage.", typeof(AbioticDepletionMineralsAndMetalsPerQuantity))]
        [Input("b4", "Replacement module in the Use stage.", typeof(AbioticDepletionMineralsAndMetalsPerQuantity))]
        [Input("b5", "Refurbishment module in the Use stage.", typeof(AbioticDepletionMineralsAndMetalsPerQuantity))]
        [Input("b6", "Operational Energy Use module in the Use stage.", typeof(AbioticDepletionMineralsAndMetalsPerQuantity))]
        [Input("b7", "Operational Water Use module in the Use stage.", typeof(AbioticDepletionMineralsAndMetalsPerQuantity))]
        [Input("c1", "De-construction Demolition module in the End of Life stage.", typeof(AbioticDepletionMineralsAndMetalsPerQuantity))]
        [Input("c2", "Transport module in the End of Life stage.", typeof(AbioticDepletionMineralsAndMetalsPerQuantity))]
        [Input("c3", "Waste Processing module in the End of Life stage.", typeof(AbioticDepletionMineralsAndMetalsPerQuantity))]
        [Input("c4", "Disposal module in the End of Life stage.", typeof(AbioticDepletionMineralsAndMetalsPerQuantity))]
        [Input("d", "Benefits and loads beyond the system boundary.", typeof(AbioticDepletionMineralsAndMetalsPerQuantity))]
        [Output("metric", "Created AbioticDepletionMineralsAndMetalsMetric.")]
        public static AbioticDepletionMineralsAndMetalsMetric AbioticDepletionMineralsAndMetalsMetric(
                double a1toa3 = double.NaN,
                double a4 = double.NaN,
                double a5 = double.NaN,
                double b1 = double.NaN,
                double b2 = double.NaN,
                double b3 = double.NaN,
                double b4 = double.NaN,
                double b5 = double.NaN,
                double b6 = double.NaN,
                double b7 = double.NaN,
                double c1 = double.NaN,
                double c2 = double.NaN,
                double c3 = double.NaN,
                double c4 = double.NaN,
                double d = double.NaN)
        {
            return new AbioticDepletionMineralsAndMetalsMetric() { Indicators = FactorsDictionary(a1toa3, a4, a5, b1, b2, b3, b4, b5, b6, b7, c1, c2, c3, c4, d) };
        }

        /***************************************************/

        [Description("Creates a AbioticDepletionMineralsAndMetalsMetric to be applied as part of an EnvironmentalProductDeclaration. Create method to be used when no discrete values for the phases in the Product stage (A1 - A3), use stage (B1-B7) or end of life stage (C1-C4) is given, but only the total value for the phases in those stages are available. If no values are provided (NaN), the module will not be added to the created metric.")]
        [Input("a1toa3", "Full Product stage.", typeof(AbioticDepletionMineralsAndMetalsPerQuantity))]
        [Input("a4", "Transport module in the Construction Process stage.", typeof(AbioticDepletionMineralsAndMetalsPerQuantity))]
        [Input("a5", "Construction Installation Process module in the Construction Process stage.", typeof(AbioticDepletionMineralsAndMetalsPerQuantity))]
        [Input("b1tob7", "Full Use Stage.", typeof(AbioticDepletionMineralsAndMetalsPerQuantity))]
        [Input("c1toc4", "Full End of Life stage.", typeof(AbioticDepletionMineralsAndMetalsPerQuantity))]
        [Input("d", "Benefits and loads beyond the system boundary.", typeof(AbioticDepletionMineralsAndMetalsPerQuantity))]
        [Output("metric", "Created AbioticDepletionMineralsAndMetalsMetric.")]
        public static AbioticDepletionMineralsAndMetalsMetric AbioticDepletionMineralsAndMetalsMetric(
                double a1toa3 = double.NaN,
                double a4 = double.NaN,
                double a5 = double.NaN,
                double b1tob7 = double.NaN,
                double c1toc4 = double.NaN,
                double d = double.NaN)
        {

            return new AbioticDepletionMineralsAndMetalsMetric() { Indicators = FactorsDictionary(a1toa3, a4, a5, b1tob7, c1toc4, d) };
        }

        /***************************************************/
    }
}


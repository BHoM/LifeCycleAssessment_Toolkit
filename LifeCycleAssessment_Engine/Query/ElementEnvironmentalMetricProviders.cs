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
using BH.Engine.Matter;
using BH.oM.Dimensional;
using BH.oM.LifeCycleAssessment.MaterialFragments;
using System.Collections.Generic;
using System.Linq;
using BH.oM.Physical.Materials;

namespace BH.Engine.LifeCycleAssessment
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [PreviousVersion("8.1", "BH.Engine.LifeCycleAssessment.Query.ElementEpds(BH.oM.Dimensional.IElementM)")]
        [Description("Query the EnvironmentalProductDeclaration or CalculatedMaterialLifeCycleEnvironmentalImpactFactors from any IElementM with a MaterialComposition composed of IEPD materials.")]
        [Input("elementM", "A IElementM from which to query the EPD.")]
        [Output("epd", "The EnvironmentalProductDeclarations or CalculatedMaterialLifeCycleEnvironmentalImpactFactors used to define the material makeup of an object.")]
        public static List<IEnvironmentalFactorsProvider> ElementEnvironmentalMetricProviders(this IElementM elementM)
        {
            if (elementM == null)
            {
                BH.Engine.Base.Compute.RecordError("No IElementM was provided.");
            }

            if (elementM.IMaterialComposition() == null)
            {
                BH.Engine.Base.Compute.RecordError("The provided element does not have a MaterialComposition.");
            }
            //Prioritise CalculatedMaterialLifeCycleEnvironmentalImpactFactors over EnvironmentalProductDeclaration
            return elementM.IMaterialComposition().Materials.Select(x => x.Properties.OfType<IEnvironmentalFactorsProvider>().OrderBy(e => e.GetType() == typeof(CalculatedMaterialLifeCycleEnvironmentalImpactFactors) ? 1 : 2).FirstOrDefault()).ToList();
        }

        /***************************************************/
    }
}





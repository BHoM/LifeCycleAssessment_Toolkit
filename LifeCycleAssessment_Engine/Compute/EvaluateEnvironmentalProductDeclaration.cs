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

using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using BH.oM.Base;
using BH.oM.Reflection.Attributes;
using BH.oM.LifeCycleAssessment;
using BH.oM.Quantities.Attributes;
using BH.oM.LifeCycleAssessment.MaterialFragments;
using BH.oM.LifeCycleAssessment.Results;
using BH.oM.Physical.Elements;
using BH.oM.Dimensional;
using BH.Engine.Physical;
using BH.Engine.Base;
using BH.Engine.Reflection;
using BH.Engine.Spatial;
using BH.Engine.Matter;

namespace BH.Engine.LifeCycleAssessment
{
    public static partial class Compute
    {
        /***************************************************/
        /****   Public Methods                          ****/
        /***************************************************/

        [Description("This method calculates the results of any selected metric within an Environmental Product Declaration. For example for an EPD of QuantityType Volume, results will reflect the objects volume * EPD Field metric.")]
        [Input("elementM", "This is a BHoM object used to calculate EPD metric. This obj must have an EPD MaterialFragment applied to the object.")]
        [Input("field", "Filter the provided EnvironmentalProductDeclaration by selecting one of the provided metrics for calculation. This method also accepts multiple fields simultaneously.")]
        [Output("result", "A LifeCycleElementResult that contains the LifeCycleAssessment data for the input object.")]
        [PreviousVersion("4.0", "BH.Engine.LifeCycleAssessment.Compute.EvaluateEnvironmentalProductDeclarationPerObject(BH.oM.Base.IBHoMObject, BH.oM.LifeCycleAssessment.EnvironmentalProductDeclarationField)")]
        public static LifeCycleAssessmentElementResult EvaluateEnvironmentalProductDeclarationPerObject(IElementM elementM, EnvironmentalProductDeclarationField field = EnvironmentalProductDeclarationField.GlobalWarmingPotential)
        {
            if(elementM is BulkSolids)
            {
                if (elementM.IMaterialComposition() == null)
                {
                    Engine.Reflection.Compute.RecordError("The BulkSolids does not contain a materialComposition. The objects must have a materialComposition if you wish to evaluate material properties.");
                    return null;
                }
                //QuantityType qt = (BulkSolids)elementM.IMaterialComposition().Materials.Select(x => BH.Engine.Physical.Query.MaterialComposition);
            }
            
            QuantityType qt = elementM.QuantityType();
            
            switch (qt)
            {
                case QuantityType.Area:
                    return EvaluateEnvironmentalProductDeclarationByArea(elementM, field);
                case QuantityType.Volume:
                    return EvaluateEnvironmentalProductDeclarationByVolume(elementM, field);
                case QuantityType.Mass:
                    return EvaluateEnvironmentalProductDeclarationByMass(elementM, field);
                default:        
                    BH.Engine.Reflection.Compute.RecordWarning("The object you have provided does not contain an EPD Material Fragment.");
                    return null;
            }
        }
        /***************************************************/


    }
}
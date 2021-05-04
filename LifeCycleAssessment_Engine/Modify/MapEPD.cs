/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2021, the respective contributors. All rights reserved.
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

using BH.Engine.Reflection;
using BH.oM.Dimensional;
using BH.oM.MEP.System;
using BH.oM.Physical.Materials;
using System.Collections.Generic;
using BH.oM.LifeCycleAssessment.MaterialFragments;

namespace BH.Engine.LifeCycleAssessment
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static IElementM MapEPD(IElementM element, IEnvironmentalProductDeclarationData elementEPD, IEnvironmentalProductDeclarationData insulationEPD, IEnvironmentalProductDeclarationData liningEPD)
        {
            if(element == null)
            {
                Engine.Reflection.Compute.RecordError("No valid element has been provided, returning null.");
                return null;
            }

            Material elementEPDMaterial = new Material
            {
                Name = elementEPD.Name,
                Properties = new List<IMaterialProperties>() { elementEPD },
            };

            Material insulationEPDMaterial = new Material
            {
                Name = insulationEPD.Name,
                Properties = new List<IMaterialProperties>() { insulationEPD },
            };

            Material liningEPDMaterial = new Material
            {
                Name = liningEPD.Name,
                Properties = new List<IMaterialProperties>() { liningEPD },
            };

            if (element is Duct)
            {               
                element.SetPropertyValue("SectionProperty.DuctMaterial", elementEPDMaterial);
                element.SetPropertyValue("SectionProperty.InsulationMaterial", insulationEPDMaterial);
                element.SetPropertyValue("SectionProperty.LiningMaterial", liningEPDMaterial);

                return element;
            }
            else
            {
                Engine.Reflection.Compute.RecordError("This is not a duct.");
                return null;
            }
        } 
    }
}

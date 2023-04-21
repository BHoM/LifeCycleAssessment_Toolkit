/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2023, the respective contributors. All rights reserved.
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
using BH.oM.LifeCycleAssessment;
using BH.oM.Base;
using BH.oM.Geometry;
using BH.oM.Physical.FramingProperties;
using BH.oM.Spatial.ShapeProfiles;
using BH.oM.Facade.Elements;
using BH.oM.Facade.Fragments;
using BH.oM.Facade.Results;
using BH.oM.Facade.SectionProperties;
using System;
using System.Runtime.CompilerServices;
using BH.oM.Facade;
using BH.oM.Physical.Materials;
using BH.Engine.Base;
using BH.oM.Quantities.Attributes;

namespace BH.Engine.LifeCycleAssessment
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Changes the materials of section profiles to EPD based material.")]
        [Input("frameEdgeProp", "FrameEdgeProperty with section profiles to apply EPD material to.")]
        [Input("targetMatl", "Name of material to replace with EPD material.")]
        [Output("epd", "Environmental Product Declaration to apply to section profiles as material.")]
        public static FrameEdgeProperty AssignEPDFrameEdgeProperty(this FrameEdgeProperty frameEdgeProp, string targetMatl, EnvironmentalProductDeclaration epd)
        {
            if (frameEdgeProp == null || frameEdgeProp.SectionProperties == null || epd == null)
            {
                return null;
            }
            if (targetMatl == null)
            {
                targetMatl = "Aluminum";
                BH.Engine.Base.Compute.RecordError($"No target material entered. Target material set to Aluminum by default.");
            }

            List<IFragment> epdDensity = epd.GetAllFragments(typeof(Density));
            if (epdDensity.Count > 1)
            {
                BH.Engine.Base.Compute.RecordError($"EPD has more than one density fragment assigned.");
                return null;
            }
            
            Material epdMatl = BH.Engine.Physical.Create.Material(epd);
            if (epdDensity.Count == 1)
            {
                epdMatl.SetPropertyValue("Density", epdDensity[0]);
            }

            Double matchCount = 0;
            //List<ConstantFramingProperty> secProps = frameEdgeProp.SectionProperties;
            foreach (ConstantFramingProperty prop in frameEdgeProp.SectionProperties)
            {
                if (prop.Material.Name.Contains(targetMatl))
                {
                    prop.Material = epdMatl;
                    matchCount ++;
                }
                else
                {
                    frameEdgeProp.SectionProperties.Remove(prop);
                }
            }
            //frameEdgeProp.SectionProperties = secProps;

            if (matchCount == 0)
            {
                BH.Engine.Base.Compute.RecordError($"No SectionProperties found that matched target material.");
                return null;
            }
            return frameEdgeProp;
        }
    }
}
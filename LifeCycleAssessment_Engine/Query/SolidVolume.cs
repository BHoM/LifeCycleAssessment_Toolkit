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

using BH.oM.Reflection.Attributes;
using BH.oM.Quantities.Attributes;
using System.ComponentModel;
using System.Collections.Generic;
using System;
using BH.oM.Physical.Materials;
using BH.oM.LifeCycleAssessment;
using BH.Engine.Geometry;
using BH.Engine.Matter;
using BH.oM.Base;
using BH.Engine.Reflection;
using System.Linq;

namespace BH.Engine.LifeCycleAssessment
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Returns a scope object's solid volume.")]
        [Input("structuresScope", "The object to get the volume from.")]
        [Output("volume", "The object's solid material volume.", typeof(Volume))]
        public static double SolidVolume(this StructuresScope structuresScope)
        {
            double structuresVol = structuresScope.Beams.Select(x => x.ISolidVolume()).Sum() +
                            structuresScope.Columns.Select(x => x.ISolidVolume()).Sum() +
                            structuresScope.CoreWalls.Select(x => x.ISolidVolume()).Sum() +
                            structuresScope.Slabs.Select(x => x.ISolidVolume()).Sum();

            return structuresVol;
        }

        /***************************************************/

    }
}

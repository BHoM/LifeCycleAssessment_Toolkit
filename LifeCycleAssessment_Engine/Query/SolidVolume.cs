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

//using BH.oM.LifeCycleAssessment.MaterialFragments
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

namespace BH.Engine.LifeCycleAssessment
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Returns an object's solid volume.")]
        [Input("object", "The object to get the volume from.")]
        [Output("volume", "The object's solid material volume.", typeof(Volume))]
        public static double SolidVolume(this IBHoMObject obj) // this is where the try catch takes place for finding volume of objects. 
        {
            return SolidVolume(obj as dynamic);
        }

        public static double SolidVolume(this BHoMObject obj)
        {
            double val = 0;

            return val;
        }

        public static double SolidVolume(this StructuresScope structuresScope)
        {
            return SolidVolume(structuresScope.StructuresSlabs); //oM connections where possible and IBHoMObject where not possible --- volume methods should query customData for key "Volume" to start.
        }

        /***************************************************/

    }
}
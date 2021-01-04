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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Base;
using BH.oM.LifeCycleAssessment;
using BH.Engine.Reflection;

namespace BH.Engine.LifeCycleAssessment
{
    public static partial class Convert
    {
        /***************************************************/
        /****           Public Methods                  ****/
        /***************************************************/

        public static BuildingBenchmarkingData ToBuildingBenchmarkingData(this CustomObject obj)
        {
            BuildingBenchmarkingData benchmark = new BuildingBenchmarkingData
            {
                High = obj.PropertyValue("High") != null ? System.Convert.ToDouble(obj.PropertyValue("High")) : double.NaN,
                Average = obj.PropertyValue("Average") != null ? System.Convert.ToDouble(obj.PropertyValue("Average")) : double.NaN,
                Low = obj.PropertyValue("Low") != null ? System.Convert.ToDouble(obj.PropertyValue("Low")) : double.NaN,
            };
            return benchmark;
        }
    }
}


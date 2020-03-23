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
using System.Text;
using System.Threading.Tasks;

using BH.oM.LifeCycleAssessment;

namespace BH.Engine.LifeCycleAssessment
{
    public static partial class Query
    {
        public static string DeclaredUnitType(this IEnvironmentalProductDeclarationData epd)
        {
            //Temporary method to be replaced by epd.DeclaredUnit convert to volume or mass using IMatter
            if (epd.DeclaredUnit != null)
            {
                switch (epd.DeclaredUnit)
                {
                    case "1 kg":
                    case "kg":
                    case "1 lbs":
                    case "1 t":
                        return "Mass";
                    case "1 m3":
                    case "1 yd3":
                        return "Volume";
                    case "1 m2":
                    case "1 ft2":
                    case "1 sqft":
                    case "1000 ft2":
                    case "1000 sqft":
                        return "Area";
                    case "1 m":
                    case "1 ft":
                        return "Length";
                    default:
                        return "Unrecognized";
                }
            }
            return null;        
        }
    }
}

/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2019, the respective contributors. All rights reserved.
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
using System.Collections.Generic;
using BH.oM.LifeCycleAnalysis;

namespace BH.Engine.LifeCycleAnalysis
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public  Method                            ****/
        /***************************************************/

        /***************************************************/

        public static CarbonQueryDatabaseRequest CarbonQueryDatabaseRequest(string uri, string username)
        {
            return new CarbonQueryDatabaseRequest
            {
                Uri = "-H " + "Authorization: Bearer " + username + uri,
            };
        }

        /***************************************************/

        public static CarbonQueryDatabaseRequest CarbonQueryDatabaseRequest0(string uri, string username, string password)
        {
            return new CarbonQueryDatabaseRequest
            {
                Uri = "curl - X POST " + "https://etl-api.cqd.io/api/rest-auth/login" + " - H " + "accept: application/json" + " - H " + "Content - Type: application / json" + " - d " + "{\"username\":\"" + username + "\",\"password\":\"" + password + "\"}",
            };
        }

        /***************************************************/

        public static CarbonQueryDatabaseRequest CarbonQueryDatabaseRequest(string uri, CustomObject headers = null, CustomObject parameters = null)
        {
            return new CarbonQueryDatabaseRequest
            {
                Uri = uri,
                Headers = headers?.CustomData,
                Parameters = parameters?.CustomData
            };
        }

        /***************************************************/
    }
}

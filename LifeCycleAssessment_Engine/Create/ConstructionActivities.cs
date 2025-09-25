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
using BH.oM.LifeCycleAssessment.MaterialFragments.Construction;
using BH.oM.LifeCycleAssessment.MaterialFragments.Transport;
using BH.oM.Quantities.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;

namespace BH.Engine.LifeCycleAssessment
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Creates a ConstructionActivities object, controlling emissions on project scale. To be used with the GlobalEmissionFactors config.")]
        [InputFromProperty("constructedFloorArea")]
        [Input("emmissionsFactor", "Emissions factor for climate change impact per constructed area. Will impact the A5.2 module. Impact will be spread across materials and elements based on their ratio of total mass.", typeof(ClimateChangePerQuantity))]
        [InputFromProperty("name")]
        [Output("transportScenario", "The created full transport scenario.")]
        public static ConstructionActivities ConstructionActivities(double constructedFloorArea, double emmissionsFactor = 40, string name = "")
        {
            return new ConstructionActivities
            {
                Name = name,
                ConstructedFloorArea = constructedFloorArea,
                EnvironmentalFactors = new List<IEnvironmentalFactor>
                {
                    new ClimateChangeFossilFactor{ Value = emmissionsFactor },
                    new ClimateChangeTotalFactor{ Value = emmissionsFactor },
                    new ClimateChangeTotalNoBiogenicFactor{ Value = emmissionsFactor },
                    new ClimateChangeBiogenicFactor{ Value = 0 },
                    new ClimateChangeLandUseFactor{ Value = 0 },
                }
            };
        }

        /***************************************************/
    }
}

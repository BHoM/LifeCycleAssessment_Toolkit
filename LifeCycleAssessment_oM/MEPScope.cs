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

using System.Collections.Generic;
using System.ComponentModel;
using BH.oM.Base;

namespace BH.oM.LifeCycleAssessment
{
    [Description("The Life Cycle Assessment Scope object intends to provide a means of reporting all of the project criteria (name, area, type, location) as well as the objects that the study encompassed (structural slabs, foundation walls, etc) along with their properties for the Enviornmental Product Declarations they used (when using SetProperty), their densities and volumes. This object may be used for studies at any stage of development and can serve as a true means of 'apples to apples' comparison when catalogued.")]
    public class MEPScope : BHoMObject
    {
        /***************************************************/
        /**** Properties                                ****/
        /***************************************************/
        [Description("MEP Equipment is a machine that processes mechanical, electrical or plumbing loads (eg Fan, Electrical Panel, Pump")]
        public MEPEquipment MEPEquipment { get; set; } = null;
        
        [Description("MEP Ductwork is a material (eg sheet metal) that helps to convey airflow from heating, ventilation or cooling systems")]
        public MEPDuctwork MEPDuctwork { get; set; } = null;
        
        [Description("MEP Generators are devices that convert mechanical energy to electrical power")]
        public MEPGenerators MEPGenerators { get; set; } = null;
        
        [Description("MEP Conduit is a tube used to route electrical wiring")]
        public MEPConduit MEPConduit { get; set; } = null;
        
        [Description("MEP Wiring is a flexible conductor of electricity")]
        public MEPWiring MEPWiring { get; set; } = null;
        
        [Description("MEP Lighting is inclusive of all light fixtures")]
        public MEPLighting MEPLighting { get; set; } = null;
        
        [Description("MEP Piping is a material (eg copper) that helps to convey fluids (eg water, waste) within a building")]
        public MEPPiping MEPPiping { get; set; } = null;
        
        [Description("MEP Batties are energy storage devices (eg photovoltaic panels)")]
        public MEPBatteries MEPBatteries { get; set; } = null;

        /***************************************************/
    }
}

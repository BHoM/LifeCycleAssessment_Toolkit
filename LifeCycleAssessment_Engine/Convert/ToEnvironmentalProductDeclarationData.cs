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
using BH.oM.LifeCycleAssessment.MaterialFragments;


namespace BH.Engine.LifeCycleAssessment
{
    public static partial class Convert
    {
        /***************************************************/
        /****           Public Methods                  ****/
        /***************************************************/

        public static EnvironmentalProductDeclaration ToEnvironmentalProductDeclarationData(this CustomObject obj)
        {
            EnvironmentalProductDeclaration epd = new EnvironmentalProductDeclaration
            {
                Id = obj.PropertyValue("_id")?.ToString() ?? "",
                Name = obj.PropertyValue("Name")?.ToString() ?? "",
                DepletionOfAbioticResourcesFossilFuels = obj.PropertyValue("PrimaryEnergyDemand") != null ? System.Convert.ToDouble(obj.PropertyValue("PrimaryEnergyDemand")) : double.NaN,
                EutrophicationPotential = obj.PropertyValue("EutrophicationPotential") != null ? System.Convert.ToDouble(obj.PropertyValue("EutrophicationPotential")) : double.NaN,
                AcidificationPotential = obj.PropertyValue("AcidificationPotential") != null ? System.Convert.ToDouble(obj.PropertyValue("AcidificationPotential")) : double.NaN,
                PhotochemicalOzoneCreationPotential = obj.PropertyValue("SmogPotential") != null ? System.Convert.ToDouble(obj.PropertyValue("SmogPotential")) : double.NaN,
                OzoneDepletionPotential = obj.PropertyValue("OzoneDepletionPotential") != null ? System.Convert.ToDouble(obj.PropertyValue("OzoneDepletionPotential")) : double.NaN,
                GlobalWarmingPotential = obj.PropertyValue("GlobalWarmingPotential") != null ? System.Convert.ToDouble(obj.PropertyValue("GlobalWarmingPotential")) : double.NaN,
                Description = obj.PropertyValue("Description")?.ToString() ?? "",
                BiogenicCarbon = obj.PropertyValue("BiogenicCarbon") != null ? System.Convert.ToDouble(obj.PropertyValue("BiogenicCarbon")) : double.NaN,
                Density = obj.PropertyValue("Density") != null ? System.Convert.ToDouble(obj.PropertyValue("Density")) : 0,
                EndOfLifeTreatment = obj.PropertyValue("EolTreatment")?.ToString() ?? "",
            };
            return epd;
        }
    }
}


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

using System.ComponentModel;
using System.Collections.Generic;
using BH.oM.Base.Attributes;
using BH.oM.LifeCycleAssessment;
using BH.oM.Environment;
using BH.oM.Environment.Climate;

namespace BH.Engine.LifeCycleAssessment
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/
        [Description("Creates a Life Cycle Assessment that quantifies the carbon emissions and other environmental impacts of a project based on building elements and life cycle stages of the study.")]
        [Input("additionalNotes", "Additional notes should convey project design constraints (eg design for seismic activity) that could affect the overall embodied carbon.")]
        [Input("biogenicCarbon", "Biogenic Carbon is a true/false that indicates that the project contains materials that originated from a biological source (trees, soil), these materials have the ability sequester/store carbon.")]
        [Input("buildingLifespan", "The assumed lifespan of the building being evaluated.  These values are for categorisation purposes only and will not effect the overall results.")]
        [Input("constructionScopeNew", "Identifies the overall construction scope for the project. Set True if New Construction exists within your project.")]
        [Input("constructionScopeRenovation", "Identifies the overall construction scope for the project. Set True if Renovation exists within your project.")]
        [Input("contactName", "The Contact Name denotes the person/people who performed the LCA study.")]
        [Input("gravityStructuralMaterial", "The primary structural system providing gravity support for the building.")]
        [Input("lateralStructuralMaterial", "The primary structural system providing lateral support for the building.")]
        [Input("levelOfDevelopment", "Typically a term utilised in BIM practices to clearly identify the scope of work being account for. Equivalents for LOD classifications can offen times be linked to design and construction phases common to the projects locale.")]
        [Input("modules", "This is a list of life cycle assessment modules to be accounted for within this assessment. These values are for categorisation purposes only and will not effect the overall results.")]
        [Input("projectType", "A general classification of the buildings primary function. This value is for categorisation purposes only and will not effect the overall results.")]
        [Input("projectName", "The Project Name denotes the name of the project for reporting purposes.")]
        [Input("projectArea", "The Project Area (m2) denotes the more precise project area which will allow assessment of kgCO2eq/m2 metrics.")]
        [Input("seismicDesignCategory", "Seismic Design Category is a classification assigned to a structure based on it's occupancy category, and the severity of the design earthquake ground motion. These categories are currently in reference to ASCE 7-05.")]
        [Input("windSpeedCategory", "Wind speed category is in reference to the Beaufort Scale of wind speeds. Values are arranged from 0-12 equivalent, 0 being Calm and 12 being Hurricane, and are used only to represent an average site-based, wind condition. Specific informaion on categorisation can be found at https://www.spc.noaa.gov/faq/tornado/beaufort.html")]
        [Input("location", "Provide the projects geographic location for database organisation purposes. This value is for categorisation purposes only and will not effect the overall results.")]
        [Output("lifeCycleAssessmentScope", "The user-specified project metadata used to define the overall scope of the LCA being complete.")]
        public static LifeCycleAssessmentScope LifeCycleAssessmentScope(
            string additionalNotes = "", 
            bool biogenicCarbon = false, 
            int buildingLifespan = 20, 
            bool constructionScopeNew = true, 
            bool constructionScopeRenovation = false, 
            string contactName = "", 
            GravityStructuralMaterial gravityStructuralMaterial = GravityStructuralMaterial.Undefined, 
            LateralStructuralMaterial lateralStructuralMaterial = LateralStructuralMaterial.Undefined, 
            LevelOfDevelopment levelOfDevelopment = LevelOfDevelopment.Undefined, 
            List<Module> modules = null, 
            ProjectType projectType = ProjectType.Undefined, 
            string projectName = "", 
            double projectArea = 0, 
            SeismicDesignCategory seismicDesignCategory = SeismicDesignCategory.Undefined, 
            WindSpeedCategory windSpeedCategory = WindSpeedCategory.Undefined, 
            Location location = null)
        {
            return new LifeCycleAssessmentScope
            {
                AdditionalNotes = additionalNotes,
                BiogenicCarbon = biogenicCarbon,
                BuildingLifespan = buildingLifespan, 
                ConstructionScopeNew = constructionScopeNew,
                ConstructionScopeRenovation = constructionScopeRenovation,
                ContactName = contactName,
                GravityStructuralMaterial = gravityStructuralMaterial,
                LateralStructuralMaterial = lateralStructuralMaterial,
                LevelOfDevelopment = levelOfDevelopment,
                LifeCycleAssessmentPhases = modules,
                ProjectType = projectType,
                ProjectName = projectName,
                ProjectArea = projectArea,
                SeismicDesignCategory = seismicDesignCategory,
                WindSpeedCategory = windSpeedCategory,
                Location = location
            };
        }
        /***************************************************/
    }
}






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

using BH.Engine.Reflection;
using BH.oM.LifeCycleAssessment.MaterialFragments;
using BH.oM.Quantities.Attributes;
using BH.oM.Reflection.Attributes;
using System.ComponentModel;
using BH.Engine.Base;
using System.Collections.Generic;
using BH.oM.LifeCycleAssessment.Fragments;
using BH.oM.Base;
using System.Linq;
using BH.oM.Dimensional;

namespace BH.Engine.LifeCycleAssessment
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Query an Environmental Product Declaration MaterialFragment to return it's Density property value where any exists.")]
        [Input("epd", "The EPD object to query.")]
        [Output("density", "Density value queried from the EPD MaterialFragment.", typeof(Density))]
        [PreviousVersion("4.2", "BH.Engine.LifeCycleAssessment.Query.GetFragmentDensity(BH.oM.LifeCycleAssessment.MaterialFragment.EnvironmentalProductDeclaration)")]
        public static List<double> GetEPDDensity(this EnvironmentalProductDeclaration epd)
        {
            // EPD null check
            if (epd == null)
            {
                BH.Engine.Reflection.Compute.RecordError("No EPD was provided. Returning NaN.");
                return new List<double>();
            }        

            // Get list of all EPD Density Fragments
            List<IFragment> densityFragment = epd.GetAllFragments().Where(x => typeof(EPDDensity).IsAssignableFrom(x.GetType())).ToList();

            if (densityFragment.Count <= 0)
            {
                BH.Engine.Reflection.Compute.RecordError("No EPDDensity Fragment could be found within the EPD. AddFragment and try again.");
                return new List<double>();
            }

            List<double> density = epd.GetAllFragments().Where(x => typeof(EPDDensity).IsAssignableFrom(x.GetType())).Select(y => (y as EPDDensity).Density).ToList();

            if(density.Count <= 0)
            {
                BH.Engine.Reflection.Compute.RecordError("No density values could be found within the EPDDensity Fragments. Please check these values and try again.");
                return new List<double>();
            }

            return density;
        }

        /***************************************************/

        [Description("Query an Environmental Product Declaration MaterialFragment to return it's Density property value where any exists.")]
        [Input("elementM", "The EPD object to query.")]
        [Output("density", "Density value queried from the EPD MaterialFragment.", typeof(Density))]
        public static List<double> GetEPDDensity(this IElementM elementM)
        {
            // Element null check
            if (elementM == null)
            {
                BH.Engine.Reflection.Compute.RecordError("No element was provided. Returning NaN.");
                return new List<double>();
            }

            // EPD Fragment null check
            List<EnvironmentalProductDeclaration> elementEpd = GetElementEpd(elementM);
            if(elementEpd.Count() <= 0)
            {
                BH.Engine.Reflection.Compute.RecordError("No EPDs could be found within any elements. Returning NaN. \n" + "Have you tried MapEPD to set your desired EPD?");
                return new List<double>();
            }

            // Get list of all EPD Fragments -- Cast to IBHoMObject fails
            List<EPDDensity> densityFragment = elementEpd.SelectMany(a => Base.Query.GetAllFragments(a, typeof(EPDDensity)).Cast<EPDDensity>()).ToList();
            if (densityFragment.Count() <= 0)
            {
                BH.Engine.Reflection.Compute.RecordError("No Density fragments could be found on the provided EPD. Have you tried adding an EPDDensity fragment?");
                return new List<double>();
            }

            // Get list of all EPD Density Values from fragments
            List<double> density = densityFragment.Select(x => x).Select(y => y.Density).ToList();
            if(density == null)
            {
                BH.Engine.Reflection.Compute.RecordWarning("No density data could be found. Please review any EPDDensity fragments used on the EPD.");
                return new List<double>();
            }

            return density;
        }
    }
}


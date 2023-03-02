/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2022, the respective contributors. All rights reserved.
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

using BH.Engine.Matter;
using BH.oM.Base.Attributes;
using BH.oM.Dimensional;
using BH.oM.LifeCycleAssessment;
using BH.oM.LifeCycleAssessment.Fragments;
using BH.oM.LifeCycleAssessment.MaterialFragments;
using BH.oM.Physical.Materials;
using BH.oM.Quantities.Attributes;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace BH.Engine.LifeCycleAssessment.Objects
{
    public static class HelperMethods 
    {
        [Description("Returns the Environmental Impact metric value for the specified field input from the Environmental Product Declaration found within the MaterialComposition of an object.")]
        [Input("elementM", "An IElementM object with a MaterialProperty from which to query the desired metric.")]
        [Input("field", "Specific metric to query from provided EPD.")]
        [Input("phases", "Provide phases of life you wish to evaluate. Phases of life must be documented within EPDs for this method to work.")]
        [Input("type", "The quantityType to query.")]
        [Input("materialComposition", "The material composition of the element using physical materials.")]
        [Input("exactMatch", "If true, the evaluation method will force an exact LCA phase match to solve for.")]
        [Output("evaluationValue", "The Environmental Impact metric value for the specified field and quantityType.")]
        public static List<double> GetEvaluationValue(this IElementM elementM, EnvironmentalProductDeclarationField field, List<LifeCycleAssessmentPhases> phases, QuantityType type, MaterialComposition materialComposition, bool exactMatch = false)
        {
            if (materialComposition == null)
            {
                BH.Engine.Base.Compute.RecordError("The material composition of the element could not be returned.");
                return new List<double>();
            }

            if (elementM == null)
                return new List<double>();

            List<double> quantityTypeValue = elementM.GetQuantityTypeValue(type, materialComposition);

            List<double> epdVal = materialComposition.Materials.Select(x =>
            {
                var epd = x.Properties.Where(y => y is EnvironmentalProductDeclaration).FirstOrDefault() as EnvironmentalProductDeclaration;

                if (epd.QuantityType == type && (epd.EnvironmentalMetric.Where(z => z.Phases.Where(a => phases.Contains(a)).Count() != 0).FirstOrDefault() != null))
                    return Query.GetEvaluationValue(epd, field, phases, exactMatch);
                else
                    return double.NaN;

            }).ToList();

            //Division of GWP constant by QTV
            List<double> normalisedEpdVal = new List<double>();

            for (int x = 0; x < epdVal.Count; x++)
            {
                if (double.IsNaN(epdVal[x]))
                    normalisedEpdVal.Add(double.NaN);
                else
                    normalisedEpdVal.Add(epdVal[x] / quantityTypeValue[x]);
            }

            return normalisedEpdVal;
        }

        /***************************************************/

        [Description("Query the QuantityTypeValue from any object with a valid construction with Environmental Product Declaration MaterialFragmments.")]
        [Input("elementM", "The IElementM Object to query.")]
        [Input("type", "The quantityType to query.")]
        [Input("materialComposition", "The material composition of the element using physical materials.")]
        [Output("quantityTypeValue", "The quantityTypeValue property from the IElementM.")]
        public static List<double> GetQuantityTypeValue(this IElementM elementM, QuantityType type, MaterialComposition materialComposition)
        {
            if (elementM == null)
                return new List<double>();

            List<double> qtv = materialComposition.Materials.Select(x =>
            {
                var epd = x.Properties.Where(y => y is EnvironmentalProductDeclaration).FirstOrDefault() as EnvironmentalProductDeclaration;
                if (epd != null && epd.QuantityType == type)
                    return epd.QuantityTypeValue;
                return 1;
            }).Where(x => x != null).ToList();

            return qtv;
        }

        /***************************************************/

        [Description("Query the QuantityType values from any IElementM object's MaterialComposition.")]
        [Input("elementM", "The IElementM object from which to query the EPD's QuantityType values.")]
        [Input("materialComposition", "The IElementM object from which to query the EPD's QuantityType values.")]
        [Output("quantityType", "The quantityType values from the IEnvironmentalProductDeclarationData objects found within the Element's MaterialComposition.")]
        public static List<QuantityType> GetQuantityType(this IElementM elementM, MaterialComposition materialComposition)
        {
            List<QuantityType> qt = new List<QuantityType>();

            materialComposition = elementM.IMaterialComposition();

            if (elementM == null)
                return new List<QuantityType> { QuantityType.Undefined };

            qt = materialComposition.Materials.Where(x => x != null).Select(x =>
            {
                var epd = x.Properties.Where(y => y is EnvironmentalProductDeclaration).FirstOrDefault() as EnvironmentalProductDeclaration;
                if (epd != null)
                    return epd.QuantityType;
                return QuantityType.Undefined;
            }).ToList();

            return qt;
        }

        /***************************************************/

        [Description("Query the Environmental Product Declarations from any IElementM with a MaterialComposition composed of IEPD materials.")]
        [Input("elementM", "A IElementM from which to query the EPD.")]
        [Input("materialComposition", "The material composition of the element using physical materials.")]
        [Output("epd", "The EPD or EPDs used to define the material makeup of an object.")]
        public static List<EnvironmentalProductDeclaration> GetElementEpd(this IElementM elementM, MaterialComposition materialComposition)
        {
            if (elementM == null)
            {
                BH.Engine.Base.Compute.RecordError("No IElementM was provided.");
                return null;
            }

            if (materialComposition == null)
            {
                BH.Engine.Base.Compute.RecordError("The provided element does not have a MaterialComposition.");
                return null;
            }

            List<EnvironmentalProductDeclaration> epd = materialComposition.Materials.Select(x => x.Properties.Where(y => y is EnvironmentalProductDeclaration).FirstOrDefault() as EnvironmentalProductDeclaration).ToList();

            if (epd == null)
            {
                BH.Engine.Base.Compute.RecordError("No EPD Material was found within the object's MaterialComposition.");
            }

            return epd;
        }

        /***************************************************/

        [Description("Query an Environmental Product Declaration MaterialFragment to return it's Density property value where any exists.")]
        [Input("elementM", "The EPD object to query.")]
        [Input("materialComposition", "The material composition of the element using physical materials.")]
        [Output("density", "Density value queried from the EPD MaterialFragment.", typeof(Density))]
        public static List<double> GetEPDDensity(this IElementM elementM, MaterialComposition materialComposition)
        {
            // Element null check
            if (elementM == null)
            {
                BH.Engine.Base.Compute.RecordError("No element was provided. Returning NaN.");
                return new List<double>();
            }

            // EPD Fragment null check
            List<EnvironmentalProductDeclaration> elementEpd = GetElementEpd(elementM, materialComposition);
            if (elementEpd.Count() <= 0)
            {
                BH.Engine.Base.Compute.RecordError("No EPDs could be found within any elements. Returning NaN. \n" + "Have you tried MapEPD to set your desired EPD?");
                return new List<double>();
            }

            // Get list of all EPD Fragments -- Cast to IBHoMObject fails
            List<EPDDensity> densityFragment = elementEpd.SelectMany(a => Base.Query.GetAllFragments(a, typeof(EPDDensity)).Cast<EPDDensity>()).ToList();
            if (densityFragment.Count() <= 0)
            {
                BH.Engine.Base.Compute.RecordError("No Density fragments could be found on the provided EPD. Have you tried adding an EPDDensity fragment?");
                return new List<double>();
            }

            // Get list of all EPD Density Values from fragments
            List<double> density = densityFragment.Select(x => x).Select(y => y.Density).ToList();
            if (density == null)
            {
                BH.Engine.Base.Compute.RecordWarning("No density data could be found. Please review any EPDDensity fragments used on the EPD.");
                return new List<double>();
            }
            return density;
        }
    }
}

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
using BH.oM.Dimensional;
using BH.oM.LifeCycleAssessment;
using BH.oM.LifeCycleAssessment.MaterialFragments;
using BH.oM.MEP.System;
using BH.oM.Physical.Elements;
using BH.oM.Physical.Materials;
using BH.oM.Reflection.Attributes;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace BH.Engine.LifeCycleAssessment
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Map Environmental Product Declarations to a duct, duct Insulation, and duct Lining SectionProperties for use in LCA workflows.")]
        [Input("duct", "The duct element to modify.")]
        [Input("elementEPD", "The EPD to be applied to the duct.")]
        [Input("insulationEPD", "The EPD to be applied to the duct's insulation.")]
        [Input("liningEPD", "The EPD to be applied to the duct's lining.")]
        [Output("duct", "The duct with EPDs applied to the SectionProperty.")]
        public static IElementM MapEPD(this Duct duct, IEnvironmentalProductDeclarationData elementEPD, IEnvironmentalProductDeclarationData insulationEPD, IEnvironmentalProductDeclarationData liningEPD)
        {
            if(duct == null)
            {
                Engine.Reflection.Compute.RecordError("No valid element has been provided, returning null.");
                return null;
            }

            if(elementEPD == null || insulationEPD == null || liningEPD == null)
            {
                Engine.Reflection.Compute.RecordError("You must provide an EPD for each input for mapping to function.");
                return null;
            }

            // Create new materials for each type depending on the element
            Material elementEPDMaterial = new Material
            {
                Name = elementEPD.Name,
                Properties = new List<IMaterialProperties>() { elementEPD },
            };

            Material insulationEPDMaterial = new Material
            {
                Name = insulationEPD.Name,
                Properties = new List<IMaterialProperties>() { insulationEPD },
            };

            Material liningEPDMaterial = new Material
            {
                Name = liningEPD.Name,
                Properties = new List<IMaterialProperties>() { liningEPD },
            };

            // Set the materials with properties
            duct.SetPropertyValue("SectionProperty.DuctMaterial", elementEPDMaterial);
            duct.SetPropertyValue("SectionProperty.InsulationMaterial", insulationEPDMaterial);
            duct.SetPropertyValue("SectionProperty.LiningMaterial", liningEPDMaterial);

            // User feedback if EPD is Mass-based and no density value is provided
            List<QuantityType> qts = duct.GetQuantityType();
            qts = qts.Distinct().ToList();

            if (qts.Contains(QuantityType.Mass))
            {
                if(Query.GetFragmentDensity(elementEPD) <= 0 && Query.GetFragmentQuantityType(elementEPD) == QuantityType.Mass || Query.GetFragmentDensity(insulationEPD) <= 0 && Query.GetFragmentQuantityType(insulationEPD) == QuantityType.Mass || Query.GetFragmentDensity(liningEPD) <= 0 && Query.GetFragmentQuantityType(liningEPD) == QuantityType.Mass)
                {
                    Reflection.Compute.RecordError("You have provided an EPD with Mass QuantityType, but the EPD itself does not contain a density value. This object will not compute the desired metric until a Density value is set on the EPD using SetProperty prior to MapEPD.");
                    return null;
                }
            }
            return duct;
        }

        /***************************************************/

        [Description("Map Environmental Product Declarations to a pipe, and pipe Insulation SectionProperties for use in LCA workflows.")]
        [Input("pipe", "The pipe element to modify.")]
        [Input("elementEPD", "The EPD to be applied to the pipe.")]
        [Input("insulationEPD", "The EPD to be applied to the pipe's insulation.")]
        [Output("pipe", "The pipe with EPDs applied to the SectionProperty.")]
        public static IElementM MapEPD(this Pipe pipe, IEnvironmentalProductDeclarationData elementEPD, IEnvironmentalProductDeclarationData insulationEPD)
        {
            if (pipe == null)
            {
                Engine.Reflection.Compute.RecordError("No valid element has been provided, returning null.");
                return null;
            }

            // Create new materials for each type depending on the element
            Material elementEPDMaterial = new Material
            {
                Name = elementEPD.Name,
                Properties = new List<IMaterialProperties>() { elementEPD },
            };

            Material insulationEPDMaterial = new Material
            {
                Name = insulationEPD.Name,
                Properties = new List<IMaterialProperties>() { insulationEPD },
            };

            // Set the materials with properties
            pipe.SetPropertyValue("SectionProperty.PipeMaterial", elementEPDMaterial);
            pipe.SetPropertyValue("SectionProperty.InsulationMaterial", insulationEPDMaterial);

            // User feedback if EPD is Mass-based and no density value is provided
            List<QuantityType> qts = pipe.GetQuantityType();
            qts = qts.Distinct().ToList();

            if (qts.Contains(QuantityType.Mass))
            {
                if (Query.GetFragmentDensity(elementEPD) <= 0 && Query.GetFragmentQuantityType(elementEPD) == QuantityType.Mass || Query.GetFragmentDensity(insulationEPD) <= 0 && Query.GetFragmentQuantityType(insulationEPD) == QuantityType.Mass)
                {
                    Reflection.Compute.RecordError("You have provided an EPD with Mass QuantityType, but the EPD itself does not contain a density value. This object will not compute the desired metric until a Density value is set on the EPD using SetProperty prior to MapEPD.");
                    return null;
                }
            }
            return pipe;
        }

        /***************************************************/

        [Description("Map Environmental Product Declarations to a cableTray for use in LCA workflows.")]
        [Input("cableTray", "The cableTray element to modify.")]
        [Input("elementEPD", "The EPD to be applied to the cableTray.")]
        [Output("cableTray", "The cableTray with EPDs applied to the SectionProperty.")]
        public static IElementM MapEPD(this CableTray cableTray, IEnvironmentalProductDeclarationData elementEPD)
        {
            if (cableTray == null)
            {
                Engine.Reflection.Compute.RecordError("No valid element has been provided, returning null.");
                return null;
            }

            Material elementEPDMaterial = new Material
            {
                Name = elementEPD.Name,
                Properties = new List<IMaterialProperties>() { elementEPD },
            };

            cableTray.SetPropertyValue("SectionProperty.Material", elementEPDMaterial);

            // User feedback if EPD is Mass-based and no density value is provided
            List<QuantityType> qts = cableTray.GetQuantityType();
            qts = qts.Distinct().ToList();

            if (qts.Contains(QuantityType.Mass))
            {
                if (Query.GetFragmentDensity(elementEPD) <= 0 && Query.GetFragmentQuantityType(elementEPD) == QuantityType.Mass)
                {
                    Reflection.Compute.RecordError("You have provided an EPD with Mass QuantityType, but the EPD itself does not contain a density value. This object will not compute the desired metric until a Density value is set on the EPD using SetProperty prior to MapEPD.");
                    return null;
                }
            }
            return cableTray;
        }

        /***************************************************/

        [Description("Map Environmental Product Declarations to a framingElement for use in LCA workflows.")]
        [Input("framingElement", "The framingElement element to modify.")]
        [Input("elementEPD", "The EPD to be applied to the framingElement.")]
        [Output("framingElement", "The framingElement with EPDs applied to the element's material composition.")]
        public static IElementM MapEPD(this IFramingElement framingElement, IEnvironmentalProductDeclarationData elementEPD)
        {
            if (framingElement == null)
            {
                Engine.Reflection.Compute.RecordError("No valid element has been provided, returning null.");
                return null;
            }

            Material elementEPDMaterial = new Material
            {
                Name = elementEPD.Name,
                Properties = new List<IMaterialProperties>() { elementEPD },
            };

            framingElement.SetPropertyValue("Property.Material", elementEPDMaterial);

            // User feedback if EPD is Mass-based and no density value is provided
            List<QuantityType> qts = framingElement.GetQuantityType();
            qts = qts.Distinct().ToList();

            if (qts.Contains(QuantityType.Mass))
            {
                if (Query.GetFragmentDensity(elementEPD) <= 0 && Query.GetFragmentQuantityType(elementEPD) == QuantityType.Mass)
                {
                    Reflection.Compute.RecordError("You have provided an EPD with Mass QuantityType, but the EPD itself does not contain a density value. This object will not compute the desired metric until a Density value is set on the EPD using SetProperty prior to MapEPD.");
                    return null;
                }
            }
            return framingElement;
        }

        /***************************************************/

        [Description("Map Environmental Product Declarations to a surfaceElement for use in LCA workflows.")]
        [Input("surfaceElement", "The surfaceElement element to modify.")]
        [Input("elementEPD", "The EPD to be applied to the surfaceElement.")]
        [Output("surfaceElement", "The surfaceElement with EPDs applied to the element's material composition.")]
        public static IElementM MapEPD(this ISurface surfaceElement, IEnvironmentalProductDeclarationData elementEPD)
        {
            if (surfaceElement == null)
            {
                Engine.Reflection.Compute.RecordError("No valid element has been provided, returning null.");
                return null;
            }

            Material elementEPDMaterial = new Material
            {
                Name = elementEPD.Name,
                Properties = new List<IMaterialProperties>() { elementEPD },
            };

            surfaceElement.SetPropertyValue("Construction.Layers.Material", elementEPDMaterial);

            // User feedback if EPD is Mass-based and no density value is provided
            List<QuantityType> qts = surfaceElement.GetQuantityType();
            qts = qts.Distinct().ToList();

            if (qts.Contains(QuantityType.Mass))
            {
                if (Query.GetFragmentDensity(elementEPD) <= 0 && Query.GetFragmentQuantityType(elementEPD) == QuantityType.Mass)
                {
                    Reflection.Compute.RecordError("You have provided an EPD with Mass QuantityType, but the EPD itself does not contain a density value. This object will not compute the desired metric until a Density value is set on the EPD using SetProperty prior to MapEPD.");
                    return null;
                }
            }
            return surfaceElement;
        }

        /***************************************************/

        [Description("Map Environmental Product Declarations to a openingElement for use in LCA workflows.")]
        [Input("openingElement", "The openingElement element to modify.")]
        [Input("elementEPD", "The EPD to be applied to the openingElement.")]
        [Output("openingElement", "The openingElement with EPDs applied to the element's material composition.")]
        public static IElementM MapEPD(this IOpening openingElement, IEnvironmentalProductDeclarationData elementEPD)
        {
            if (openingElement == null)
            {
                Engine.Reflection.Compute.RecordError("No valid element has been provided, returning null.");
                return null;
            }

            Material elementEPDMaterial = new Material
            {
                Name = elementEPD.Name,
                Properties = new List<IMaterialProperties>() { elementEPD },
            };

            if(openingElement is Door)
            {
                Door door = (Door)openingElement;
                openingElement.SetPropertyValue("Construction.Layers.Material", elementEPDMaterial);

                // User feedback if EPD is Mass-based and no density value is provided
                List<QuantityType> qts = door.GetQuantityType();
                qts = qts.Distinct().ToList();

                if (qts.Contains(QuantityType.Mass))
                {
                    if (Query.GetFragmentDensity(elementEPD) <= 0 && Query.GetFragmentQuantityType(elementEPD) == QuantityType.Mass)
                    {
                        Reflection.Compute.RecordError("You have provided an EPD with Mass QuantityType, but the EPD itself does not contain a density value. This object will not compute the desired metric until a Density value is set on the EPD using SetProperty prior to MapEPD.");
                        return null;
                    }
                }
                return door;
            }
            if(openingElement is Window)
            {
                Window window = (Window)openingElement;
                openingElement.SetPropertyValue("Construction.Layers.Material", elementEPDMaterial);

                // User feedback if EPD is Mass-based and no density value is provided
                List<QuantityType> qts = window.GetQuantityType();
                qts = qts.Distinct().ToList();

                if (qts.Contains(QuantityType.Mass))
                {
                    if (Query.GetFragmentDensity(elementEPD) <= 0 && Query.GetFragmentQuantityType(elementEPD) == QuantityType.Mass)
                    {
                        Reflection.Compute.RecordError("You have provided an EPD with Mass QuantityType, but the EPD itself does not contain a density value. This object will not compute the desired metric until a Density value is set on the EPD using SetProperty prior to MapEPD.");
                        return null;
                    }
                }
                return window;
            }
            Reflection.Compute.RecordError($"MapEPD is not supported for element of type {openingElement.GetType()}");

            return null;
        }

        /***************************************************/
    }
}

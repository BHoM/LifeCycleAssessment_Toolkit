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
//using BH.oM.Structure; <--This was used for IMass calc. Remove when IelementM is implemented.
using System.ComponentModel;
using BH.oM.Base;
using BH.oM.Reflection.Attributes;
using BH.oM.LifeCycleAssessment;

namespace BH.Engine.LifeCycleAssessment
{
    public static partial class Compute
    {
        /***************************************************/
        /****   Public Methods                          ****/
        /***************************************************/

        [Description("This method calculates the quantity of any selected metric within an Environmental Product Declaration by extracting the declared unit of the selected material and multiplying the objects Volume * Density * EnvironmentalProductDeclarationField criteria. Please view the EnvironmentalProductDeclarationField Enum to explore current evaluation metric options.")]
        [Input("BHoM CustomObject", "This is a BHoM CustomObject made with CreateCustom. The method expects Volume and Density properties. Density will be extracted from the dataset if possible prior to extracting from the object itself.")]
        [Input("environmentalProductDeclaration", "This is LifeCycleAssessment.EnvironmentalProductDeclaration data. Please select your desired dataset and supply your material choice to the corresponding BHoM objects.")]
        [Input("environmentalProductDeclarationField", "Filter the provided EnvironmentalProductDeclaration by selecting one of the provided metrics for calculation. This method also accepts multiple fields simultaneously.")]
        [Output("quantity", "The quantity of the desired metric provided by the EnvironmentalProductDeclarationField")]
        public static double EvaluateEnvironmentalProductDeclaration(CustomObject obj = null, EnvironmentalProductDeclaration environmentalProductDeclaration = null, EnvironmentalProductDeclarationField environmentalProductDeclarationField = EnvironmentalProductDeclarationField.GlobalWarmingPotential) //default to globalWarmingPotential evaluation
        {
            if (obj != null)
            {
                //if declared unit from EnvironmentalProductDeclaration is kg (standard unit).
                if (environmentalProductDeclaration.DeclaredUnit != null && environmentalProductDeclaration.DeclaredUnit.Equals("kg", StringComparison.InvariantCultureIgnoreCase))
                {
                    try
                    {
                        double volume = System.Convert.ToDouble(obj.CustomData["Volume"]);
                        double constant = environmentalProductDeclaration.queryEnvironmentalProductDeclaration(environmentalProductDeclarationField);
                        try
                        {
                            if (System.Convert.ToDouble(environmentalProductDeclaration.Density) != 0)
                            {
                                BH.Engine.Reflection.Compute.RecordNote("This method is using Density supplied by the EnvironmentalProductDeclaration to calculate Mass.");
                                double density = System.Convert.ToDouble(environmentalProductDeclaration.Density);
                                return volume * density * constant;
                            }
                        }
                        catch
                        {
                            BH.Engine.Reflection.Compute.RecordNote("Unable to extract Density from the EnvironmentalProductDeclaration to calculate Mass. The method will attempt to extract the Density value from the object.");
                        } //Try to extract density from the dataset.
                        try
                        {
                            BH.Engine.Reflection.Compute.RecordNote("This method is using Density supplied by the CustomObject to calculate Mass. Please verify that you are supplying accurate values for Density.");
                            double density = System.Convert.ToDouble(obj.CustomData["Density"]);
                            return volume * density * constant;
                        }
                        catch
                        {
                            BH.Engine.Reflection.Compute.RecordError("This method requires an object to function. Please provide a BHoM CustomObject with Volume and Density properties stored in CustomData");
                        } //Try to extract the density from the object.
                    }
                    catch
                    {
                        BH.Engine.Reflection.Compute.RecordError("The CustomObject must have Volume and Density properties within CustomData for this method to work.");
                    } //Extract Density and Volume properties from available sources. 
                }
                else
                {
                    BH.Engine.Reflection.Compute.RecordError("This method requires a BHoM CustomObject with Volume and Density properties to function.Please verify these values and SetProperty to resume calculation.");
                }

                //if declared unit from EnvironmentalProductDeclaration is t (tonne).
                if (environmentalProductDeclaration.DeclaredUnit != null && environmentalProductDeclaration.DeclaredUnit.Equals("t", StringComparison.InvariantCultureIgnoreCase))
                {
                    BH.Engine.Reflection.Compute.RecordError("The declared unit of the EnvironmentalProductDeclaration is t (tonne). Because this is not standard practice, you should verify all units and data to ensure consistant results. Units will be converted from kgCO2/tonne to kgCO2/kg.");
                    try
                    {
                        double volume = System.Convert.ToDouble(obj.CustomData["Volume"]);
                        double constant = environmentalProductDeclaration.queryEnvironmentalProductDeclaration(environmentalProductDeclarationField) / 1000; //<---convert to kg. this convert is temporary until iElementM is functional.
                        try
                        {
                            if (System.Convert.ToDouble(environmentalProductDeclaration.Density) != 0)
                            {
                                BH.Engine.Reflection.Compute.RecordNote("This method is using Density supplied by the EnvironmentalProductDeclaration to calculate Mass.");
                                double density = System.Convert.ToDouble(environmentalProductDeclaration.Density);
                                return volume * density * constant;
                            }
                        }
                        catch
                        {
                            BH.Engine.Reflection.Compute.RecordNote("Unable to extract Density from the EnvironmentalProductDeclaration to calculate Mass. The method will attempt to extract the Density value from the object.");
                        } //Try to extract density from the dataset.
                        try
                        {
                            BH.Engine.Reflection.Compute.RecordNote("This method is using Density supplied by the CustomObject to calculate Mass. Please verify that you are supplying accurate values for Density.");
                            double density = System.Convert.ToDouble(obj.CustomData["Density"]);
                            return volume * density * constant;
                        }
                        catch
                        {
                            BH.Engine.Reflection.Compute.RecordError("This method requires an object to function. Please provide a BHoM CustomObject with Volume and Density properties stored in CustomData");
                        } //Try to extract the density from the object.
                    }
                    catch
                    {
                        BH.Engine.Reflection.Compute.RecordError("The CustomObject must have Volume and Density properties within CustomData for this method to work.");
                    } //Extract Density and Volume properties from available sources. 
                }
                else
                {
                    BH.Engine.Reflection.Compute.RecordError("This method requires a BHoM CustomObject with Volume and Density properties to function.Please verify these values and SetProperty to resume calculation.");
                }

                //if declared unit from EnvironmentalProductDeclaration is m2 (surface area).
                if (environmentalProductDeclaration.DeclaredUnit != null && environmentalProductDeclaration.DeclaredUnit.Equals("m2", StringComparison.InvariantCultureIgnoreCase))
                {
                    BH.Engine.Reflection.Compute.RecordError("The declared unit of the EnvironmentalProductDeclaration is area based (m2). You must therefore multiply the result by the surface area of the material you wish to evaluate.");
                    try
                    {
                        double volume = System.Convert.ToDouble(obj.CustomData["Volume"]);
                        double constant = environmentalProductDeclaration.queryEnvironmentalProductDeclaration(environmentalProductDeclarationField); //Prompt the user to multiply the value by the m2 of material being evaluated.
                        try
                        {
                            if (System.Convert.ToDouble(environmentalProductDeclaration.Density) != 0)
                            {
                                BH.Engine.Reflection.Compute.RecordNote("This method is using Density supplied by the EnvironmentalProductDeclaration to calculate Mass.");
                                double density = System.Convert.ToDouble(environmentalProductDeclaration.Density);
                                return volume * density * constant;
                            }
                        }
                        catch
                        {
                            BH.Engine.Reflection.Compute.RecordNote("Unable to extract Density from the EnvironmentalProductDeclaration to calculate Mass. The method will attempt to extract the Density value from the object.");
                        } //Try to extract density from the dataset.
                        try
                        {
                            BH.Engine.Reflection.Compute.RecordNote("This method is using Density supplied by the CustomObject to calculate Mass. Please verify that you are supplying accurate values for Density.");
                            double density = System.Convert.ToDouble(obj.CustomData["Density"]);
                            return volume * density * constant;
                        }
                        catch
                        {
                            BH.Engine.Reflection.Compute.RecordError("This method requires an object to function. Please provide a BHoM CustomObject with Volume and Density properties stored in CustomData");
                        } //Try to extract the density from the object.
                    }
                    catch
                    {
                        BH.Engine.Reflection.Compute.RecordError("The CustomObject must have Volume and Density properties within CustomData for this method to work.");
                    } //Extract Density and Volume properties from available sources. 
                }
                else
                {
                    BH.Engine.Reflection.Compute.RecordError("This method requires a BHoM CustomObject with Volume and Density properties to function.Please verify these values and SetProperty to resume calculation.");
                }
            }
            else 
            {
                BH.Engine.Reflection.Compute.RecordError("This method requires a BHoM CustomObject and mass to function. Mass = Volume * Density: Please provide both Volume and Density properties within CustomData to resume calculation.");
            }
            return 0;
        }

        /***************************************************/

        //<-----Additional method in progress to work with any geometry----->//

        /*[Description("Calculates the acidification potential of a BHoM Object based on explicitly defined volume and Environmental Product Declaration dataset.")]
        [Input("volume", "Volume in m^2 as a double. This method does not extract the Volume of an object and should be provided manually. The property may be extracted from an EPDData Object.")]
        [Input("density", "Density in kg/m^3 as a double. This method does not extract the Density of an object and should be provided manually. The property may be extracted from an EPDData Object.")]
        [Input("epdData", "BHoM EPDData object containing values for typical metrics within Environmental Product Declarations.")]
        [Input("environmentalProductDeclarationField", "BHoM environmentalProductDeclarationField Enum to select Environmental Product Declaration metric for evaluation.")]
        [Output("quantity", "The quantity of the specified EPD metric within a given geometry.")]
        public static double EvaluateEPD(double volume = 0, double density = 0, EPDData epdData = null, environmentalProductDeclarationField environmentalProductDeclarationField = environmentalProductDeclarationField.GlobalWarmingPotential)
        {
            double calcDensity = double.NaN;

            if (density != 0)
            {
                calcDensity = density;
            }
            else
            {
                BH.Engine.Reflection.Compute.RecordNote("Density value is either zero or is being derived from the epdData. Please verify the correct value within the dataset before continuing by using the explode component.");

                if (System.Convert.ToDouble(epdData.Density) != 0)
                {
                    try
                    {
                        calcDensity = System.Convert.ToDouble(epdData.Density);
                    }
                    catch (Exception e)
                    {
                        BH.Engine.Reflection.Compute.RecordError("An error occurred in converting the custom data key Density to a valid number (double) - error was: " + e.ToString());
                    }
                }
                if ((calcDensity == 0 || double.IsNaN(calcDensity)) && (density == 0 || double.IsNaN(density)))
                {
                    BH.Engine.Reflection.Compute.RecordError($"Results cannot be calculated. Please check your input values for Density, Volume, and {environmentalProductDeclarationField}");
                    return double.NaN;
                }
            }
            return (volume * calcDensity) * CompileEPD(epdData, environmentalProductDeclarationField);
        }*/
    }
}
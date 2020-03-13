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
using BH.oM.Quantities.Attributes;
using BH.Engine.Reflection;

namespace BH.Engine.LifeCycleAssessment
{
    public static partial class Compute
    {
        /***************************************************/
        /****   Public Methods                          ****/
        /***************************************************/

        [Description("This method calculates the quantity of any selected metric within an Environmental Product Declaration by extracting the declared unit of the selected material and multiplying the objects Volume * Density * EnvironmentalProductDeclarationField criteria. Please view the EnvironmentalProductDeclarationField Enum to explore current evaluation metric options.")]
        [Input("BHoM Object", "This is a BHoM Object to calculate EPD off of. The method requires a volume property on the BHoM Object. Density is required if the chosen EPD is on a per mass basis, and will be extracted from the dataset if possible prior to extracting from the object itself.")]
        [Input("environmentalProductDeclaration", "This is LifeCycleAssessment.EnvironmentalProductDeclaration data. Please select your desired dataset and supply your material choice to the corresponding BHoM objects.")]
        [Input("environmentalProductDeclarationField", "Filter the provided EnvironmentalProductDeclaration by selecting one of the provided metrics for calculation. This method also accepts multiple fields simultaneously.")]
        [Output("quantity", "The quantity of the desired metric provided by the EnvironmentalProductDeclarationField")]
        public static double EvaluateEnvironmentalProductDeclarationPerObject(IBHoMObject obj, EnvironmentalProductDeclaration environmentalProductDeclaration, EnvironmentalProductDeclarationField environmentalProductDeclarationField = EnvironmentalProductDeclarationField.GlobalWarmingPotential) //default to globalWarmingPotential evaluation
        {
            if (obj != null)
            {
                //First check validity of inputs
                object vol = obj.PropertyValue("Volume");
                if (vol == null)
                {
                    BH.Engine.Reflection.Compute.RecordError("The input object must have a Volume property for this method to work.");
                    return 0;
                }
                double volume = System.Convert.ToDouble(vol);
                if (volume == 0)
                {
                    BH.Engine.Reflection.Compute.RecordError("The input object's Volume value is invalid. Volume should be in m3 in numerical format.");
                    return 0;
                }

                //Check the epd declared unit type to determine whether to calculate by mass, volume, or neither
                string declaredUnitType = environmentalProductDeclaration.DeclaredUnitType();

                if (declaredUnitType == "Volume")
                {
                    return EvaluateEnvironmentalProductDeclarationByVolume(environmentalProductDeclaration, environmentalProductDeclarationField, volume);
                }

                else if (declaredUnitType == "Mass")
                {
                    double density = 0;
                    string epdDensity = environmentalProductDeclaration.Density.ToString() ?? "";
                    double densityVal = System.Convert.ToDouble(epdDensity.Substring(0, epdDensity.IndexOf(" ")));
                    if (System.Convert.ToDouble(densityVal) != 0)
                    {
                        density = densityVal;
                        BH.Engine.Reflection.Compute.RecordNote(String.Format("This method is using a density of {0} supplied by the EnvironmentalProductDeclaration to calculate Mass.", density));
                    }
                    else if (obj.PropertyValue("Density") == null)
                    {
                        BH.Engine.Reflection.Compute.RecordNote("The EnvironmentalProductDeclaration and input object do not have density information. The epd is mass-based. Please add density data to the input object.");
                        return 0;
                    }
                    else if (System.Convert.ToDouble(obj.PropertyValue("Density")) == 0)
                    {
                        BH.Engine.Reflection.Compute.RecordNote("The input object's Density value is invalid. Density should be in kg/m3 in numerical format.");
                        return 0;
                    }
                    else
                    {
                        density = System.Convert.ToDouble(obj.PropertyValue("Density"));
                    }
                    double mass = density * volume;

                    return EvaluateEnvironmentalProductDeclarationByMass(environmentalProductDeclaration, environmentalProductDeclarationField, mass);
                }

                else if (declaredUnitType == "Area")
                {
                    object ar = obj.PropertyValue("Area");
                    if (ar == null)
                    {
                        BH.Engine.Reflection.Compute.RecordError("The EnvironmentalProductDeclaration supplied uses an area based declared unit, so the input object requires an Area property. ");
                        return 0;
                    }
                    double area = System.Convert.ToDouble(ar);
                    if (area == 0)
                    {
                        BH.Engine.Reflection.Compute.RecordError("The input object's Area value is invalid. Area should be in m2 in numerical format.");
                        return 0;
                    }
                    return EvaluateEnvironmentalProductDeclarationByArea(environmentalProductDeclaration, environmentalProductDeclarationField, area);
                }
                else
                {
                    BH.Engine.Reflection.Compute.RecordError("This EnvironmentalProductDeclaration's declared unit type is not supported.");
                    return 0;
                }
            }
            else
            {
                return 0;
            }
        }

        /***************************************************/

        [Description("This method calculates the quantity of a supplied metric per a supplied EPD and mass.")]
        [Input("environmentalProductDeclaration", "This is a LifeCycleAssessment.EnvironmentalProductDeclaration with a mass-based declared unit. Please select your desired dataset and supply your material choice to the corresponding BHoM objects.")]
        [Input("environmentalProductDeclarationField", "The metric to calculate total quantity for.")]
        [Input("mass", "The total mass to calculate the total quantity of the input metric for.",typeof(Mass))]
        [Output("quantity", "The total quantity of the desired metric based on the EnvironmentalProductDeclarationField")]
        public static double EvaluateEnvironmentalProductDeclarationByMass(EnvironmentalProductDeclaration environmentalProductDeclaration = null, EnvironmentalProductDeclarationField environmentalProductDeclarationField = EnvironmentalProductDeclarationField.GlobalWarmingPotential, double mass = 0) //default to globalWarmingPotential evaluation
        {
            if (environmentalProductDeclaration.DeclaredUnitType() != "Mass")
            {
                BH.Engine.Reflection.Compute.RecordError("This EnvironmentalProductDeclaration's declared unit type is not Mass. Please supply a Mass-based epd or try a different method.");
                return 0;
            }
            else
            {
                double declaredUnit = environmentalProductDeclaration.DeclaredUnitValueInSI();
                return mass * environmentalProductDeclaration.EnvironmentalProductDeclaration(environmentalProductDeclarationField) / declaredUnit;
            }
        }

        /***************************************************/

        [Description("This method calculates the quantity of a supplied metric per a supplied EPD and volume.")]
        [Input("environmentalProductDeclaration", "This is a LifeCycleAssessment.EnvironmentalProductDeclaration with a volume-based declared unit. Please select your desired dataset and supply your material choice to the corresponding BHoM objects.")]
        [Input("environmentalProductDeclarationField", "The metric to calculate total quantity for.")]
        [Input("volume", "The total volume to calculate the total quantity of the input metric for.", typeof(Volume))]
        [Output("quantity", "The total quantity of the desired metric based on the EnvironmentalProductDeclarationField")]
        public static double EvaluateEnvironmentalProductDeclarationByVolume(EnvironmentalProductDeclaration environmentalProductDeclaration = null, EnvironmentalProductDeclarationField environmentalProductDeclarationField = EnvironmentalProductDeclarationField.GlobalWarmingPotential, double volume = 0)
        {
            if (environmentalProductDeclaration.DeclaredUnitType() != "Volume")
            {
                BH.Engine.Reflection.Compute.RecordError("This EnvironmentalProductDeclaration's declared unit type is not Volume. Please supply a Volume-based epd or try a different method.");
                return 0;
            }
            else
            {
                double declaredUnit = environmentalProductDeclaration.DeclaredUnitValueInSI();
                return volume * environmentalProductDeclaration.EnvironmentalProductDeclaration(environmentalProductDeclarationField) / declaredUnit;
            }
        }

        /***************************************************/

        [Description("This method calculates the quantity of a supplied metric per a supplied EPD and area.")]
        [Input("environmentalProductDeclaration", "This is a LifeCycleAssessment.EnvironmentalProductDeclaration with an area-based declared unit. Please select your desired dataset and supply your material choice to the corresponding BHoM objects.")]
        [Input("environmentalProductDeclarationField", "The metric to calculate total quantity for.")]
        [Input("area", "The total area to calculate the total quantity of the input metric for.", typeof(Area))]
        [Output("quantity", "The total quantity of the desired metric based on the EnvironmentalProductDeclarationField")]
        public static double EvaluateEnvironmentalProductDeclarationByArea(EnvironmentalProductDeclaration environmentalProductDeclaration = null, EnvironmentalProductDeclarationField environmentalProductDeclarationField = EnvironmentalProductDeclarationField.GlobalWarmingPotential, double area = 0)
        {
            if (environmentalProductDeclaration.DeclaredUnitType() != "Area")
            {
                BH.Engine.Reflection.Compute.RecordError("This EnvironmentalProductDeclaration's declared unit type is not Area. Please supply an Area-based epd or try a different method.");
                return 0;
            }
            else
            {
                double declaredUnit = environmentalProductDeclaration.DeclaredUnitValueInSI();
                return area * environmentalProductDeclaration.EnvironmentalProductDeclaration(environmentalProductDeclarationField) / declaredUnit;
            }
        }

    }
}
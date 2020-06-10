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
using System.ComponentModel;
using BH.oM.Base;
using BH.oM.Reflection.Attributes;
using BH.oM.LifeCycleAssessment;
using BH.oM.Quantities.Attributes;
using BH.oM.LifeCycleAssessment.MaterialFragments;
using BH.oM.Physical.Elements;
using BH.oM.Dimensional;

using BH.Engine.Base;
using BH.Engine.Reflection;
using BH.Engine.Spatial;
using BH.Engine.Matter;

namespace BH.Engine.LifeCycleAssessment
{
    public static partial class Compute
    {
        /***************************************************/
        /****   Public Methods                          ****/
        /***************************************************/

        [Description("This method calculates the quantity of any selected metric within an Environmental Product Declaration by extracting the declared unit of the selected material and multiplying the objects Volume * Density * EnvironmentalProductDeclarationField criteria. Please view the EnvironmentalProductDeclarationField Enum to explore current evaluation metric options.")]
        [Input("obj", "This is a BHoM Object to calculate EPD off of. The method requires a volume property on the BHoM Object. Density is required if the chosen EPD is on a per mass basis, and will be extracted from the dataset if possible prior to extracting from the object itself.")]
        [Input("environmentalProductDeclaration", "This is LifeCycleAssessment.EnvironmentalProductDeclaration data. Please select your desired dataset and supply your material choice to the corresponding BHoM objects.")]
        [Input("environmentalProductDeclarationField", "Filter the provided EnvironmentalProductDeclaration by selecting one of the provided metrics for calculation. This method also accepts multiple fields simultaneously.")]
        [Output("quantity", "The quantity of the desired metric provided by the EnvironmentalProductDeclarationField")]
        public static double EvaluateEnvironmentalProductDeclarationPerObject(IElementM obj, EnvironmentalProductDeclarationField field = EnvironmentalProductDeclarationField.GlobalWarmingPotential)
        {
            //Check validity of inputs
            if (obj != null)
            {
                //IElementM SolidVolume() if not, throw error.

                if(obj.ISolidVolume() != 0)
                {
                    //SolidVolume for objects
                    double volume = obj.ISolidVolume();
                    if (volume == 0)
                    {
                        BH.Engine.Reflection.Compute.RecordError("The object does not contain a value for solid volume. Please provide an IElementM with the expected properties to calcuate SolidVolume.");
                        return 0;
                    }

                    //If QuantityType = Volume, return EvalByVolume()
                    if ((obj as IBHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().QuantityType == QuantityType.Volume)
                    {
                        return EvaluateEnvironmentalProductDeclarationByVolume(obj, field, volume);
                    }

                    //elif QuantityType = Mass, return EvalByMass()
                    else if ((obj as IBHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().QuantityType == QuantityType.Mass)
                    {
                        //get Density from IEPD MaterialFragment
                        double density = (obj as IBHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetFragmentDensity();

                        double mass = density * volume;

                        return EvaluateEnvironmentalProductDeclarationByMass(obj, field, mass);
                    }                    
                    //elif QuantityType = Area, return EvalByArea()
                    else if ((obj as IBHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().QuantityType == QuantityType.Area)
                    {
                        object ar = (obj as IElement2D).Area();
                        if (ar == null)
                        {
                            BH.Engine.Reflection.Compute.RecordError("No area values can be calculated from the provided objects. Because the object's material fragment requires an Area based calculation, you must supply an object with an area.");
                            return 0;
                        }

                        double area = (obj as IElement2D).Area();
                        if (area <= 0)
                        {
                            BH.Engine.Reflection.Compute.RecordError("The input object's Area value is invalid. Area should be in m2 in numerical format.");
                            return 0;
                        }
                        return EvaluateEnvironmentalProductDeclarationByArea(obj, field, area);
                    }      
                    //throw error when declared unit type is not supported
                    else
                    {
                        BH.Engine.Reflection.Compute.RecordError("This EnvironmentalProductDeclaration's declared unit type is not supported.");
                        return 0;
                    }
                }
            }
            return 0;
        }

        /***************************************************/

        [Description("This method calculates the quantity of a supplied metric per a supplied EPD and mass.")]
        [Input("environmentalProductDeclaration", "This is a LifeCycleAssessment.EnvironmentalProductDeclaration with a mass-based declared unit. Please select your desired dataset and supply your material choice to the corresponding BHoM objects.")]
        [Input("environmentalProductDeclarationField", "The metric to calculate total quantity for.")]
        [Input("mass", "The total mass to calculate the total quantity of the input metric for.", typeof(Mass))]
        [Output("quantity", "The total quantity of the desired metric based on the EnvironmentalProductDeclarationField")]
        public static double EvaluateEnvironmentalProductDeclarationByMass(IElementM obj = null, EnvironmentalProductDeclarationField field = EnvironmentalProductDeclarationField.GlobalWarmingPotential, double mass = 0) //default to globalWarmingPotential evaluation
        {
            if ((obj as IBHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().QuantityType != QuantityType.Mass)
            {
                BH.Engine.Reflection.Compute.RecordError("This EnvironmentalProductDeclaration's declared unit type is not Mass. Please supply a Mass-based EPD or try a different method.");
                return 0;
            }
            else
            {
                double epdVal = (obj as IBHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetEvaluationValue(field);
                return mass * epdVal;
            }
        }

        /***************************************************/

        [Description("This method calculates the quantity of a supplied metric per a supplied EPD and volume.")]
        [Input("environmentalProductDeclaration", "This is a LifeCycleAssessment.EnvironmentalProductDeclaration with a volume-based declared unit. Please select your desired dataset and supply your material choice to the corresponding BHoM objects.")]
        [Input("environmentalProductDeclarationField", "The metric to calculate total quantity for.")]
        [Input("volume", "The total volume to calculate the total quantity of the input metric for.", typeof(Volume))]
        [Output("quantity", "The total quantity of the desired metric based on the EnvironmentalProductDeclarationField")]
        public static double EvaluateEnvironmentalProductDeclarationByVolume(IElementM obj = null, EnvironmentalProductDeclarationField field = EnvironmentalProductDeclarationField.GlobalWarmingPotential, double volume = 0)
        {
            if ((obj as IBHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().QuantityType != QuantityType.Volume)
            {
                BH.Engine.Reflection.Compute.RecordError("This EnvironmentalProductDeclaration's declared unit type is not Volume. Please supply a Volume-based EPD or try a different method.");
                return 0;
            }
            else
            {
                double epdVal = (obj as IBHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetEvaluationValue(field);
                return volume * epdVal;
            }
        }

        /***************************************************/

        [Description("This method calculates the quantity of a supplied metric per a supplied EPD and area.")]
        [Input("environmentalProductDeclaration", "This is a LifeCycleAssessment.EnvironmentalProductDeclaration with an area-based declared unit. Please select your desired dataset and supply your material choice to the corresponding BHoM objects.")]
        [Input("environmentalProductDeclarationField", "The metric to calculate total quantity for.")]
        [Input("area", "The total area to calculate the total quantity of the input metric for.", typeof(Area))]
        [Output("quantity", "The total quantity of the desired metric based on the EnvironmentalProductDeclarationField")]
        public static double EvaluateEnvironmentalProductDeclarationByArea(IElementM obj = null, EnvironmentalProductDeclarationField field = EnvironmentalProductDeclarationField.GlobalWarmingPotential, double area = 0)
        {
            if ((obj as IBHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().QuantityType != QuantityType.Area)
            {
                BH.Engine.Reflection.Compute.RecordError("This EnvironmentalProductDeclaration's declared unit type is not Area. Please supply an Area-based EPD or try a different method.");
                return 0;
            }
            else
            {
                double epdVal = (obj as IBHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetEvaluationValue(field);
                return area * epdVal;
            }
        }
    }
}
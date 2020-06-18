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
using BH.oM.LifeCycleAssessment.Results;
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

        [Description("This method calculates the results of any selected metric within an Environmental Product Declaration. For example for an EPD of QuantityType Volume, results will reflect the objects volume * EPD Field metric.")]
        [Input("obj", "This is a BHoM object used to calculate EPD metric. This obj must have an EPD MaterialFragment applied to the object.")]
        [Input("field", "Filter the provided EnvironmentalProductDeclaration by selecting one of the provided metrics for calculation. This method also accepts multiple fields simultaneously.")]
        [Output("result", "A LifeCycleElementResult that contains the LifeCycleAssessment data for the input object.")]
        public static LifeCycleAssessmentElementResult EvaluateEnvironmentalProductDeclarationPerObject(IBHoMObject obj, EnvironmentalProductDeclarationField field = EnvironmentalProductDeclarationField.GlobalWarmingPotential)
        {
            //if (obj.GetAllFragments() == null || obj.GetAllFragments().Count() <= 0)
            //{
            //    BH.Engine.Reflection.Compute.RecordWarning("Object " + obj.BHoM_Guid + " " + obj.GetType().ToString() + " does not contain a valid IEnvironmentalProductDeclaration MaterialFragment.");
            //    return null;
            //}
            for (int x = 0; x < obj.GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Count();)
            {
                //if obj != null and Fragment !=, EvalPerObject()
                if (obj != null && obj.GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault() != null)
                {
                    //if QuantityType = Area, return EvalByArea()
                    if (obj.GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().QuantityType == QuantityType.Area)
                    {
                        object ar = obj as IElement2D;
                        if (ar == null)
                        {
                            //Temporary fix to check for area property value if it can not cast to IElement2d and calculate its own area. This will be phased out once IElement2D and IElementM are fully implemented in the next milestone.
                            object areaProp = obj.PropertyValue("Area");
                            double areaVal = System.Convert.ToDouble(areaProp);
                            if (areaProp == null)
                            {
                                BH.Engine.Reflection.Compute.RecordError("No area values can be calculated for object " + obj.BHoM_Guid + " " + ". Because the object's material fragment requires an Area based calculation, you must supply an object with an area.");
                                return null;
                            }
                            else
                            {
                                BH.Engine.Reflection.Compute.RecordWarning("Object " + obj.BHoM_Guid + "is not an IElement2D. Its value is being calculated based on the assigned Area property value of " + areaVal.ToString() + ". Please confirm this area value is accurate.");
                                return EvaluateEnvironmentalProductDeclarationByArea(obj, field, areaVal);
                            }

                        }
                        double area = (obj as IElement2D).Area();
                        if (area <= 0)
                        {
                            BH.Engine.Reflection.Compute.RecordError("No area values can be calculated for object " + obj.BHoM_Guid + ". Because the object's material fragment requires an Area based calculation, you must supply an object with an area.");
                            return null;
                        }
                        return EvaluateEnvironmentalProductDeclarationByArea(obj, field, area);
                    }

                    //If QuantityType = Volume, return EvalByVolume()
                    if ((obj as IBHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().QuantityType == QuantityType.Volume)
                    {
                        //Check if obj is IElementM, if not then check for volume property. This is a temporary fix that will be phased out to only use IElemenM's once IElementM is fully implemented on footings, piles, etc.
                        try
                        {
                            IElementM elem = obj as IElementM;
                            //SolidVolume for objects
                            double volume = elem.ISolidVolume();
                            if (volume == 0)
                            {
                                BH.Engine.Reflection.Compute.RecordError("No volume can be calculated for object " + obj.BHoM_Guid + ".Because the object's material fragment requires a volume-based calculation, you must supply an object with a volume.");
                                return null;
                            }
                            return EvaluateEnvironmentalProductDeclarationByVolume(obj, field, volume);
                        }
                        catch
                        {
                            object vol = obj.PropertyValue("Volume");
                            double volVal = System.Convert.ToDouble(vol);
                            if (vol == null)
                            {
                                BH.Engine.Reflection.Compute.RecordError("No volume can be calculated for object " + obj.BHoM_Guid + ".Because the object's material fragment requires a volume-based calculation, you must supply an object with a volume.");
                                return null;
                            }
                            else
                            {
                                BH.Engine.Reflection.Compute.RecordWarning("Object " + obj.BHoM_Guid + " " + "is not an IElementM. Its value is being calculated based on the assigned Volume property value of " + volVal.ToString() + ". This method of evaluation will not be supported passed BHoM Version 3.3.");
                                return EvaluateEnvironmentalProductDeclarationByVolume(obj, field, volVal);
                            }
                        }
                    }

                    //elif QuantityType = Mass, return EvalByMass()
                    else if ((obj as IBHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().QuantityType == QuantityType.Mass)
                    {
                        //Check if obj is IElementM, if not then check for volume property. This is a temporary fix that will be phased out to only use IElemenM's once IElementM is fully implemented on footings, piles, etc.
                        double volume = 0;
                        try
                        {
                            IElementM elem = obj as IElementM;
                            volume = elem.ISolidVolume();
                            if (volume == 0)
                            {
                                BH.Engine.Reflection.Compute.RecordError("No volume can be calculated for object " + obj.BHoM_Guid + ".Because the object's material fragment requires a volume-based calculation, you must supply an object with a volume.");
                                return null;
                            }
                        }
                        catch
                        {
                            object vol = obj.PropertyValue("Volume");
                            volume = System.Convert.ToDouble(vol);
                            if (vol == null)
                            {
                                BH.Engine.Reflection.Compute.RecordError("No volume can be calculated from the provided object. Because the object's material fragment requires a mass-based calculation, you must supply an object with a volume.");
                                return null;
                            }
                            else
                            {
                                BH.Engine.Reflection.Compute.RecordWarning("No volume can be calculated for object " + obj.BHoM_Guid + ".Because the object's material fragment requires a volume-based calculation, you must supply an object with a volume.");
                            }
                        }

                        //get Density from IEPD MaterialFragment
                        double density = (obj as IBHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetFragmentDensity();

                        double mass = density * volume;

                        return EvaluateEnvironmentalProductDeclarationByMass(obj, field, mass);
                    }

                    else
                    {
                        BH.Engine.Reflection.Compute.RecordError("This EnvironmentalProductDeclaration's declared unit type is not supported.");
                        return null;
                    }
                }
                BH.Engine.Reflection.Compute.RecordWarning("HEY! Object equals null, or missing EPD Fragment.");
            }
            return null;
        }

        /***************************************************/

        [Description("This method calculates the quantity of a supplied metric per a supplied EPD and mass.")]
        [Input("obj", "This is an IElementM object used to calculate EPD metric. The method requires a BHoM Object that inherits properties of IElementM in order to run mass-based calculations. Density will be extracted from the EPD MaterialFragment, therefore insure that you have an accurate density value prior to running calculations.")]
        [Input("field", "Filter the provided EnvironmentalProductDeclaration by selecting one of the provided metrics for calculation. This method also accepts multiple fields simultaneously.")]
        [Input("mass", "The total mass to calculate the total quantity of the input metric for.", typeof(Mass))]
        [Output("quantity", "The total quantity of the desired metric based on the EnvironmentalProductDeclarationField")]
        public static GlobalWarmingPotentialResult EvaluateEnvironmentalProductDeclarationByMass(IBHoMObject obj = null, EnvironmentalProductDeclarationField field = EnvironmentalProductDeclarationField.GlobalWarmingPotential, double mass = 0) //default to globalWarmingPotential evaluation
        {
            if (obj.GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().QuantityType != QuantityType.Mass)
            {
                BH.Engine.Reflection.Compute.RecordError("This EnvironmentalProductDeclaration's declared unit type is not Mass. Please supply a Mass-based EPD or try a different method.");
                return null;
            }
            else
            {
                double epdVal = (obj as IBHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetEvaluationValue(field);
                double quantity = mass * epdVal;

                //if epdfield = gwp return that else return error of not yet implemented and return null. 
                return new GlobalWarmingPotentialResult(obj.BHoM_Guid, "GWP", 0, ObjectScope.Undefined, ObjectCategory.Undefined, obj.GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault(), quantity);
            }
        }

        /***************************************************/

        [Description("This method calculates the quantity of a supplied metric per a supplied EPD and volume.")]
        [Input("obj", "This is an IElementM object used to calculate EPD metric. The method requires a BHoM Object that inherits properties of IElementM in order to run mass-based calculations. Density will be extracted from the EPD MaterialFragment, therefore insure that you have an accurate density value prior to running calculations.")]
        [Input("field", "Filter the provided EnvironmentalProductDeclaration by selecting one of the provided metrics for calculation. This method also accepts multiple fields simultaneously.")]
        [Input("volume", "The total volume to calculate the total quantity of the input metric for.", typeof(Volume))]
        [Output("quantity", "The total quantity of the desired metric based on the EnvironmentalProductDeclarationField")]
        public static GlobalWarmingPotentialResult EvaluateEnvironmentalProductDeclarationByVolume(IBHoMObject obj = null, EnvironmentalProductDeclarationField field = EnvironmentalProductDeclarationField.GlobalWarmingPotential, double volume = 0)
        {
            if (obj.GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().QuantityType != QuantityType.Volume)
            {
                BH.Engine.Reflection.Compute.RecordError("This EnvironmentalProductDeclaration's declared unit type is not Volume. Please supply a Volume-based EPD or try a different method.");
                return null;
            }
            else
            {
                double epdVal = (obj as IBHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetEvaluationValue(field);
                double quantity = volume * epdVal;
                return new GlobalWarmingPotentialResult(obj.BHoM_Guid, "GWP", 0, ObjectScope.Undefined, ObjectCategory.Undefined, obj.GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault(), quantity);
            }
        }

        /***************************************************/

        [Description("This method calculates the quantity of a supplied metric per a supplied EPD and area.")]
        [Input("obj", "This is an IElementM object used to calculate EPD metric. The method requires a BHoM Object that inherits properties of IElementM in order to run mass-based calculations. Density will be extracted from the EPD MaterialFragment, therefore insure that you have an accurate density value prior to running calculations.")]
        [Input("field", "Filter the provided EnvironmentalProductDeclaration by selecting one of the provided metrics for calculation. This method also accepts multiple fields simultaneously.")]
        [Input("area", "The total area to calculate the total quantity of the input metric for.", typeof(Area))]
        [Output("quantity", "The total quantity of the desired metric based on the EnvironmentalProductDeclarationField")]
        public static GlobalWarmingPotentialResult EvaluateEnvironmentalProductDeclarationByArea(IBHoMObject obj = null, EnvironmentalProductDeclarationField field = EnvironmentalProductDeclarationField.GlobalWarmingPotential, double area = 0)
        {
            if (obj.GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().QuantityType != QuantityType.Area)
            {
                BH.Engine.Reflection.Compute.RecordError("This EnvironmentalProductDeclaration's declared unit type is not Area. Please supply an Area-based EPD or try a different method.");
                return null;
            }
            else
            {
                double epdVal = (obj as IBHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetEvaluationValue(field);
                double quantity = area * epdVal;
                return new GlobalWarmingPotentialResult(obj.BHoM_Guid, "GWP", 0, ObjectScope.Undefined, ObjectCategory.Undefined, obj.GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault(), quantity);
            }
        }
    }
}
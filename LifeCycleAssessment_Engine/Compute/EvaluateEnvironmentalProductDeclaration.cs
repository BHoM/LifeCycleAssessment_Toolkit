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
using BH.Engine.Base;
using BH.oM.Reflection.Attributes;
using BH.oM.LifeCycleAssessment;
using BH.oM.Quantities.Attributes;
using BH.Engine.Reflection;
using BH.oM.LifeCycleAssessment.MaterialFragments;

using BH.Engine.Matter;
using BH.oM.Physical.Elements;

namespace BH.Engine.LifeCycleAssessment
{
    public static partial class Compute
    {
        /***************************************************/
        /****   Public Methods                          ****/
        /***************************************************/

        [Description("This method calls the appropriate compute method per object within a Life Cycle Assessment.")]
        [Input("lifeCycleAssessment", "This is a complete Life Cycle Assessment object for which the evaluation will occur.")]
        [Input("field", "This is the desired field you would like to evaluate. Notice that not all material datasets will contain information for all metrics. A holistic GWP evaluation is currently the only standard evaluation possible with all provided datasets.")]
        [Output("quantity", "The quantity of the desired metric provided by the EnvironmentalProductDeclarationField. This is an enum.")]

        public static double EvaluateLifeCycleAssessment(ProjectLifeCycleAssessment lca, EnvironmentalProductDeclarationField field)
        {
            //Check for object validity
            if(lca != null)
            {
                //Structures Scope Objects
                double beams = lca.StructuresScope.Beams.Select(x => EvaluateEnvironmentalProductDeclarationPerObject((IBHoMObject)x, field)).Sum();
                double columns = lca.StructuresScope.Columns.Select(x => EvaluateEnvironmentalProductDeclarationPerObject((IBHoMObject)x, field)).Sum();
                double coreWalls = lca.StructuresScope.CoreWalls.Select(x => EvaluateEnvironmentalProductDeclarationPerObject((IBHoMObject)x, field)).Sum();
                double slabs = lca.StructuresScope.Slabs.Select(x => EvaluateEnvironmentalProductDeclarationPerObject((IBHoMObject)x, field)).Sum();
                double structures = beams + columns + coreWalls + slabs;

                //Foundation Scope Objects
                double footings = lca.FoundationsScope.Footings.Select(x => EvaluateEnvironmentalProductDeclarationPerObject((IBHoMObject)x, field)).Sum();
                double piles = lca.FoundationsScope.Piles.Select(x => EvaluateEnvironmentalProductDeclarationPerObject((IBHoMObject)x, field)).Sum();
                double walls = lca.FoundationsScope.Walls.Select(x => EvaluateEnvironmentalProductDeclarationPerObject((IBHoMObject)x, field)).Sum();
                double fndslabs = lca.FoundationsScope.Slabs.Select(x => EvaluateEnvironmentalProductDeclarationPerObject((IBHoMObject)x, field)).Sum();
                double foundations = footings + piles + walls + fndslabs;

                //Enclosure Scope Objects
                double encWalls = lca.EnclosuresScope.Walls.Select(x => EvaluateEnvironmentalProductDeclarationPerObject((IBHoMObject)x, field)).Sum();
                double curtainWalls = lca.EnclosuresScope.CurtainWalls.Select(x => EvaluateEnvironmentalProductDeclarationPerObject((IBHoMObject)x, field)).Sum();
                double windows = lca.EnclosuresScope.Windows.Select(x => EvaluateEnvironmentalProductDeclarationPerObject((IBHoMObject)x, field)).Sum();
                double doors = lca.EnclosuresScope.Doors.Select(x => EvaluateEnvironmentalProductDeclarationPerObject((IBHoMObject)x, field)).Sum();
                double enclosures = encWalls + curtainWalls + windows + doors;

                //MEP Scope Objects
                double batteries = lca.MEPScope.Batteries.Select(x => EvaluateEnvironmentalProductDeclarationPerObject((IBHoMObject)x, field)).Sum();
                double conduit = lca.MEPScope.Conduit.Select(x => EvaluateEnvironmentalProductDeclarationPerObject((IBHoMObject)x, field)).Sum();
                double ductwork = lca.MEPScope.Ductwork.Select(x => EvaluateEnvironmentalProductDeclarationPerObject((IBHoMObject)x, field)).Sum();
                double equipment = lca.MEPScope.Equipment.Select(x => EvaluateEnvironmentalProductDeclarationPerObject((IBHoMObject)x, field)).Sum();
                double generators = lca.MEPScope.Generators.Select(x => EvaluateEnvironmentalProductDeclarationPerObject((IBHoMObject)x, field)).Sum();
                double lighting = lca.MEPScope.Lighting.Select(x => EvaluateEnvironmentalProductDeclarationPerObject((IBHoMObject)x, field)).Sum();
                double piping = lca.MEPScope.Piping.Select(x => EvaluateEnvironmentalProductDeclarationPerObject((IBHoMObject)x, field)).Sum();
                double wiring = lca.MEPScope.Wiring.Select(x => EvaluateEnvironmentalProductDeclarationPerObject((IBHoMObject)x, field)).Sum();
                double mep = batteries + conduit + ductwork + equipment + generators + lighting + piping + wiring; 

                //Tenant Improvement Scope Objects
                double ceiling = lca.TenantImprovementScope.Ceiling.Select(x => EvaluateEnvironmentalProductDeclarationPerObject((IBHoMObject)x, field)).Sum();
                double finishes = lca.TenantImprovementScope.Finishes.Select(x => EvaluateEnvironmentalProductDeclarationPerObject((IBHoMObject)x, field)).Sum();
                double flooring = lca.TenantImprovementScope.Flooring.Select(x => EvaluateEnvironmentalProductDeclarationPerObject((IBHoMObject)x, field)).Sum();
                double furniture = lca.TenantImprovementScope.Furniture.Select(x => EvaluateEnvironmentalProductDeclarationPerObject((IBHoMObject)x, field)).Sum();
                double intDoors = lca.TenantImprovementScope.InteriorDoors.Select(x => EvaluateEnvironmentalProductDeclarationPerObject((IBHoMObject)x, field)).Sum();
                double intGlazing = lca.TenantImprovementScope.InteriorGlazing.Select(x => EvaluateEnvironmentalProductDeclarationPerObject((IBHoMObject)x, field)).Sum();
                double partitionWalls = lca.TenantImprovementScope.PartitionWalls.Select(x => EvaluateEnvironmentalProductDeclarationPerObject((IBHoMObject)x, field)).Sum();
                double ti = ceiling + finishes + flooring + furniture + intDoors + intGlazing + partitionWalls;

                return structures + foundations + enclosures + mep + ti;
            }
            return double.NaN;
        }

        [Description("This method calculates the quantity of any selected metric within an Environmental Product Declaration by extracting the declared unit of the selected material and multiplying the objects Volume * Density * EnvironmentalProductDeclarationField criteria. Please view the EnvironmentalProductDeclarationField Enum to explore current evaluation metric options.")]
        [Input("obj", "This is a BHoM Object to calculate EPD off of. The method requires a volume property on the BHoM Object. Density is required if the chosen EPD is on a per mass basis, and will be extracted from the dataset if possible prior to extracting from the object itself.")]
        [Input("environmentalProductDeclaration", "This is LifeCycleAssessment.EnvironmentalProductDeclaration data. Please select your desired dataset and supply your material choice to the corresponding BHoM objects.")]
        [Input("environmentalProductDeclarationField", "Filter the provided EnvironmentalProductDeclaration by selecting one of the provided metrics for calculation. This method also accepts multiple fields simultaneously.")]
        [Output("quantity", "The quantity of the desired metric provided by the EnvironmentalProductDeclarationField")]
        public static double EvaluateEnvironmentalProductDeclarationPerObject(IBHoMObject obj, EnvironmentalProductDeclarationField field = EnvironmentalProductDeclarationField.GlobalWarmingPotential)
        {
            //Check validity of inputs
            if (obj != null)
            {
                //Check for IEPD Material Fragment
                if(obj.GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault() != null)
                {
                    //Call SolidVolume()
                    //double vol = StructuresScope.Beams.Select(x => x.ISolidVolume()).Sum();
                    object vol = obj.PropertyValue("Volume");
                    if (vol == null)
                    {
                        BH.Engine.Reflection.Compute.RecordError("The input object must have a Volume property for this method to work.");
                        return 0;
                    }
                    double volume = System.Convert.ToDouble(vol);
                    if (volume <= 0)
                    {
                        BH.Engine.Reflection.Compute.RecordError("The input object's Volume value is invalid or negative. Volume should be in m3 in numerical format.");
                        return 0;
                    }
                    //If QuantityType = Volume, return EvalByVolume()
                    if (obj.GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().QuantityType == QuantityType.Volume)
                    {
                        return EvaluateEnvironmentalProductDeclarationByVolume(obj, field, volume);
                    }
                    //elif QuantityType = Mass, return EvalByMass()
                    else if (obj.GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().QuantityType == QuantityType.Mass)
                    {
                        //get Density from IEPD MaterialFragment
                        double density = obj.GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetFragmentDensity();

                        if (density != 0 && density != double.NaN)
                        {
                            BH.Engine.Reflection.Compute.RecordNote(String.Format("This method is using a density of {0} supplied by the EnvironmentalProductDeclaration to calculate Mass.", density));
                        }
                        else if (obj.PropertyValue("Density") == null)
                        {
                            BH.Engine.Reflection.Compute.RecordNote("The EnvironmentalProductDeclaration and input object do not have density information. The EPD is mass-based. Please add density data to the input object.");
                            return 0;
                        }
                        else if (System.Convert.ToDouble(obj.PropertyValue("Density")) <= 0)
                        {
                            BH.Engine.Reflection.Compute.RecordNote("The input object's Density value is invalid. Density should be in kg/m3 in numerical format.");
                            return 0;
                        }
                        else
                        {
                            //check for density in property
                            density = System.Convert.ToDouble(obj.PropertyValue("Density"));
                        }

                        double mass = density * volume;

                        return EvaluateEnvironmentalProductDeclarationByMass(obj, field, mass);
                    }                    
                    //elif QuantityType = Area, return EvalByArea()
                    else if (obj.GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().QuantityType == QuantityType.Area)
                    {
                        object ar = obj.PropertyValue("Area");
                        if (ar == null)
                        {
                            BH.Engine.Reflection.Compute.RecordError("The EnvironmentalProductDeclaration supplied uses an area based declared unit, so the input object requires an Area property.");
                            return 0;
                        }

                        double area = System.Convert.ToDouble(ar);
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
        public static double EvaluateEnvironmentalProductDeclarationByMass(IBHoMObject obj = null, EnvironmentalProductDeclarationField field = EnvironmentalProductDeclarationField.GlobalWarmingPotential, double mass = 0) //default to globalWarmingPotential evaluation
        {
            if (obj.GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().QuantityType != QuantityType.Mass)
            {
                BH.Engine.Reflection.Compute.RecordError("This EnvironmentalProductDeclaration's declared unit type is not Mass. Please supply a Mass-based EPD or try a different method.");
                return 0;
            }
            else
            {
                return mass * obj.GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetEvaluationValue(field);
            }
        }

        /***************************************************/

        [Description("This method calculates the quantity of a supplied metric per a supplied EPD and volume.")]
        [Input("environmentalProductDeclaration", "This is a LifeCycleAssessment.EnvironmentalProductDeclaration with a volume-based declared unit. Please select your desired dataset and supply your material choice to the corresponding BHoM objects.")]
        [Input("environmentalProductDeclarationField", "The metric to calculate total quantity for.")]
        [Input("volume", "The total volume to calculate the total quantity of the input metric for.", typeof(Volume))]
        [Output("quantity", "The total quantity of the desired metric based on the EnvironmentalProductDeclarationField")]
        public static double EvaluateEnvironmentalProductDeclarationByVolume(IBHoMObject obj = null, EnvironmentalProductDeclarationField field = EnvironmentalProductDeclarationField.GlobalWarmingPotential, double volume = 0)
        {
            if (obj.GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().QuantityType != QuantityType.Volume)
            {
                BH.Engine.Reflection.Compute.RecordError("This EnvironmentalProductDeclaration's declared unit type is not Volume. Please supply a Volume-based EPD or try a different method.");
                return 0;
            }
            else
            {
                return volume * obj.GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetEvaluationValue(field);
            }
        }

        /***************************************************/

        [Description("This method calculates the quantity of a supplied metric per a supplied EPD and area.")]
        [Input("environmentalProductDeclaration", "This is a LifeCycleAssessment.EnvironmentalProductDeclaration with an area-based declared unit. Please select your desired dataset and supply your material choice to the corresponding BHoM objects.")]
        [Input("environmentalProductDeclarationField", "The metric to calculate total quantity for.")]
        [Input("area", "The total area to calculate the total quantity of the input metric for.", typeof(Area))]
        [Output("quantity", "The total quantity of the desired metric based on the EnvironmentalProductDeclarationField")]
        public static double EvaluateEnvironmentalProductDeclarationByArea(IBHoMObject obj = null, EnvironmentalProductDeclarationField field = EnvironmentalProductDeclarationField.GlobalWarmingPotential, double area = 0)
        {
            if (obj.GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().QuantityType != QuantityType.Area)
            {
                BH.Engine.Reflection.Compute.RecordError("This EnvironmentalProductDeclaration's declared unit type is not Area. Please supply an Area-based EPD or try a different method.");
                return 0;
            }
            else
            {
                return area * obj.GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetEvaluationValue(field);
            }
        }
    }
}
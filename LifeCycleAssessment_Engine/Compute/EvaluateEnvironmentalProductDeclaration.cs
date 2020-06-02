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

namespace BH.Engine.LifeCycleAssessment
{
    public static partial class Compute
    {
        /***************************************************/
        /****   Public Methods                          ****/
        /***************************************************/

            // Questions for work session
            //1. general structure of method is getting complex
            //2. checking for QuantityType property and calling the correct method
            //3. error handling 

        public static double EvaluateLifeCycleAssessment(ProjectLifeCycleAssessment lca, EnvironmentalProductDeclarationField field)
        {
            //Check for object validity
            if(lca != null)
            {
                //StructuresScope EPD Constant Values
                double structuresEpd = lca.StructuresScope.Beams.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetEvaluationValue(field)).Sum() +
                    lca.StructuresScope.Columns.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetEvaluationValue(field)).Sum() +
                    lca.StructuresScope.CoreWalls.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetEvaluationValue(field)).Sum() +
                    lca.StructuresScope.Slabs.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetEvaluationValue(field)).Sum();

                //FoundationsScope EPD Constant Values
                double foundationsEpd = lca.FoundationsScope.Footings.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetEvaluationValue(field)).Sum() +
                    lca.FoundationsScope.Piles.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetEvaluationValue(field)).Sum() +
                    lca.FoundationsScope.Slabs.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetEvaluationValue(field)).Sum() +
                    lca.FoundationsScope.Walls.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetEvaluationValue(field)).Sum();

                //EnclosuresScope EPD Constant Values
                double enclosuresEpd = lca.EnclosuresScope.CurtainWalls.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetEvaluationValue(field)).Sum() +
                    lca.EnclosuresScope.Doors.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetEvaluationValue(field)).Sum() +
                    lca.EnclosuresScope.Walls.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetEvaluationValue(field)).Sum() +
                    lca.EnclosuresScope.Windows.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetEvaluationValue(field)).Sum();

                //MEPScope EPD Constant Values
                double mepEpd = lca.MEPScope.Batteries.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetEvaluationValue(field)).Sum() +
                    lca.MEPScope.Conduit.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetEvaluationValue(field)).Sum() +
                    lca.MEPScope.Ductwork.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetEvaluationValue(field)).Sum() +
                    lca.MEPScope.Equipment.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetEvaluationValue(field)).Sum() +
                    lca.MEPScope.Generators.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetEvaluationValue(field)).Sum() +
                    lca.MEPScope.Lighting.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetEvaluationValue(field)).Sum() +
                    lca.MEPScope.Piping.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetEvaluationValue(field)).Sum() +
                    lca.MEPScope.Wiring.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetEvaluationValue(field)).Sum();

                //TentantImprovements EPD Constant Values
                double tiEpd = lca.TenantImprovementScope.Ceiling.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetEvaluationValue(field)).Sum() +
                    lca.TenantImprovementScope.Finishes.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetEvaluationValue(field)).Sum() +
                    lca.TenantImprovementScope.Flooring.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetEvaluationValue(field)).Sum() +
                    lca.TenantImprovementScope.Furniture.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetEvaluationValue(field)).Sum() +
                    lca.TenantImprovementScope.InteriorDoors.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetEvaluationValue(field)).Sum() +
                    lca.TenantImprovementScope.InteriorGlazing.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetEvaluationValue(field)).Sum() +
                    lca.TenantImprovementScope.PartitionWalls.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetEvaluationValue(field)).Sum();

                double epdVal = structuresEpd + foundationsEpd + enclosuresEpd + mepEpd + tiEpd;

                //Get Density from MaterialFragment

                //StructuresScope Density
                double structuresDensity = lca.StructuresScope.Beams.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetFragmentDensity()).Sum() +
                    lca.StructuresScope.Columns.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetFragmentDensity()).Sum() +
                    lca.StructuresScope.CoreWalls.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetFragmentDensity()).Sum() +
                    lca.StructuresScope.Slabs.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetFragmentDensity()).Sum();

                //FoundationsScope Density
                double foundationsDensity = lca.FoundationsScope.Footings.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetFragmentDensity()).Sum() +
                    lca.FoundationsScope.Piles.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetFragmentDensity()).Sum() +
                    lca.FoundationsScope.Slabs.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetFragmentDensity()).Sum() +
                    lca.FoundationsScope.Walls.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetFragmentDensity()).Sum();

                //EnclosuresScope Density
                double enclosuresDensity = lca.EnclosuresScope.CurtainWalls.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetFragmentDensity()).Sum() +
                    lca.EnclosuresScope.Doors.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetFragmentDensity()).Sum() +
                    lca.EnclosuresScope.Walls.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetFragmentDensity()).Sum() +
                    lca.EnclosuresScope.Windows.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetFragmentDensity()).Sum();

                double mepDensity = lca.MEPScope.Batteries.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetFragmentDensity()).Sum() +
                    lca.MEPScope.Conduit.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetFragmentDensity()).Sum() +
                    lca.MEPScope.Ductwork.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetFragmentDensity()).Sum() +
                    lca.MEPScope.Equipment.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetFragmentDensity()).Sum() +
                    lca.MEPScope.Generators.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetFragmentDensity()).Sum() +
                    lca.MEPScope.Lighting.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetFragmentDensity()).Sum() +
                    lca.MEPScope.Piping.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetFragmentDensity()).Sum() +
                    lca.MEPScope.Wiring.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetFragmentDensity()).Sum();

                //TentantImprovement Density
                double tiDensity = lca.TenantImprovementScope.Ceiling.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetFragmentDensity()).Sum() +
                    lca.TenantImprovementScope.Finishes.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetFragmentDensity()).Sum() +
                    lca.TenantImprovementScope.Flooring.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetFragmentDensity()).Sum() +
                    lca.TenantImprovementScope.Furniture.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetFragmentDensity()).Sum() +
                    lca.TenantImprovementScope.InteriorDoors.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetFragmentDensity()).Sum() +
                    lca.TenantImprovementScope.InteriorGlazing.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetFragmentDensity()).Sum() +
                    lca.TenantImprovementScope.PartitionWalls.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetFragmentDensity()).Sum();

                double densityVal = structuresDensity + foundationsDensity + enclosuresDensity + mepDensity + tiDensity;

                //Get Volumes from Objects

                //StructuresScope Volumes
                double structuresVol = lca.StructuresScope.Beams.Select(x => x.ISolidVolume()).Sum() +
                    lca.StructuresScope.Columns.Select(x => x.ISolidVolume()).Sum() +
                    lca.StructuresScope.CoreWalls.Select(x => x.ISolidVolume()).Sum() +
                    lca.StructuresScope.Slabs.Select(x => x.ISolidVolume()).Sum();

                //FoundationsScope Volumes
                double foundationsVol = lca.FoundationsScope.Piles.Select(x => x.ISolidVolume()).Sum() +
                    lca.FoundationsScope.Footings.Select(x => x.ISolidVolume()).Sum() +
                    lca.FoundationsScope.Slabs.Select(x => x.ISolidVolume()).Sum() +
                    lca.FoundationsScope.Walls.Select(x => x.ISolidVolume()).Sum();

                //EnclosuresScope Volumes
                double enclosuresVol = lca.EnclosuresScope.CurtainWalls.Select(x => x.ISolidVolume()).Sum() +
                    lca.EnclosuresScope.Doors.Select(x => x.ISolidVolume()).Sum() +
                    lca.EnclosuresScope.Walls.Select(x => x.ISolidVolume()).Sum() +
                    lca.EnclosuresScope.Windows.Select(x => x.ISolidVolume()).Sum();

                //MEPScope Volumes
                double mepVol = lca.MEPScope.Batteries.Select(x => x.ISolidVolume()).Sum() +
                    lca.MEPScope.Conduit.Select(x => x.ISolidVolume()).Sum() +
                    lca.MEPScope.Ductwork.Select(x => x.ISolidVolume()).Sum() +
                    lca.MEPScope.Equipment.Select(x => x.ISolidVolume()).Sum() +
                    lca.MEPScope.Generators.Select(x => x.ISolidVolume()).Sum() +
                    lca.MEPScope.Lighting.Select(x => x.ISolidVolume()).Sum() +
                    lca.MEPScope.Piping.Select(x => x.ISolidVolume()).Sum() +
                    lca.MEPScope.Wiring.Select(x => x.ISolidVolume()).Sum();

                //TentantImprovements Volumes
                double tiVol = lca.TenantImprovementScope.Ceiling.Select(x => x.ISolidVolume()).Sum() +
                    lca.TenantImprovementScope.Finishes.Select(x => x.ISolidVolume()).Sum() +
                    lca.TenantImprovementScope.Flooring.Select(x => x.ISolidVolume()).Sum() +
                    lca.TenantImprovementScope.Furniture.Select(x => x.ISolidVolume()).Sum() +
                    lca.TenantImprovementScope.InteriorDoors.Select(x => x.ISolidVolume()).Sum() +
                    lca.TenantImprovementScope.InteriorGlazing.Select(x => x.ISolidVolume()).Sum() +
                    lca.TenantImprovementScope.PartitionWalls.Select(x => x.ISolidVolume()).Sum();

                double volumeVal = structuresVol + foundationsVol + enclosuresVol + mepVol + tiVol;

                //Check for materialFragment Quantity type and call method -- switch case method for quantity type? 

                //Check for materialFragment Density and define value

                //IEnvironmentalProductDeclarationData spaceIsHard = lca.StructuresScope.Slabs.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault()).FirstOrDefault();
                //double evaluatedSomething = 0;
                //if (spaceIsHard.QuantityType == QuantityType.Volume)
                //    evaluatedSomething = EvaluateEnvironmentalProductDeclarationByVolume(spaceIsHard, field, structureVol);
                //else if (spaceIsHard.QuantityType == QuantityType.Mass)
                //    evaluatedSomething = EvaluateEnvironmentalProductDeclarationByMass(spaceIsHard, field, structureMass);
                //else if (spaceIsHard.QuantityType == QuantityType.Area)
                //    evaluatedSomething = EvaluateEnvironmentalProductDeclarationByArea(spaceIsHard, field, structureArea);


                //return newVal * volume;

                return epdVal * densityVal * volumeVal;
            }
            return double.NaN;
        }

        [Description("This method calculates the quantity of any selected metric within an Environmental Product Declaration by extracting the declared unit of the selected material and multiplying the objects Volume * Density * EnvironmentalProductDeclarationField criteria. Please view the EnvironmentalProductDeclarationField Enum to explore current evaluation metric options.")]
        [Input("obj", "This is a BHoM Object to calculate EPD off of. The method requires a volume property on the BHoM Object. Density is required if the chosen EPD is on a per mass basis, and will be extracted from the dataset if possible prior to extracting from the object itself.")]
        [Input("environmentalProductDeclaration", "This is LifeCycleAssessment.EnvironmentalProductDeclaration data. Please select your desired dataset and supply your material choice to the corresponding BHoM objects.")]
        [Input("environmentalProductDeclarationField", "Filter the provided EnvironmentalProductDeclaration by selecting one of the provided metrics for calculation. This method also accepts multiple fields simultaneously.")]
        [Output("quantity", "The quantity of the desired metric provided by the EnvironmentalProductDeclarationField")]
        public static double EvaluateEnvironmentalProductDeclarationPerObject(IBHoMObject obj, IEnvironmentalProductDeclarationData environmentalProductDeclaration, EnvironmentalProductDeclarationField environmentalProductDeclarationField = EnvironmentalProductDeclarationField.GlobalWarmingPotential) //default to globalWarmingPotential evaluation
        {
            if (obj != null)
            {
                //First check validity of inputs - 1. look for fragment on the objects extract IEPD. 
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

                if (environmentalProductDeclaration.QuantityType == QuantityType.Volume)
                {
                    return EvaluateEnvironmentalProductDeclarationByVolume(environmentalProductDeclaration, environmentalProductDeclarationField, volume);
                }
                else if (environmentalProductDeclaration.QuantityType == QuantityType.Mass)
                    {

                    double density = environmentalProductDeclaration.Density;

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
                        density = System.Convert.ToDouble(obj.PropertyValue("Density"));
                    }

                    double mass = density * volume;

                    return EvaluateEnvironmentalProductDeclarationByMass(environmentalProductDeclaration, environmentalProductDeclarationField, mass);
                }
                else if (environmentalProductDeclaration.QuantityType == QuantityType.Area)
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
        [Input("mass", "The total mass to calculate the total quantity of the input metric for.", typeof(Mass))]
        [Output("quantity", "The total quantity of the desired metric based on the EnvironmentalProductDeclarationField")]
        public static double EvaluateEnvironmentalProductDeclarationByMass(IEnvironmentalProductDeclarationData environmentalProductDeclaration = null, EnvironmentalProductDeclarationField environmentalProductDeclarationField = EnvironmentalProductDeclarationField.GlobalWarmingPotential, double mass = 0) //default to globalWarmingPotential evaluation
        {
            if (environmentalProductDeclaration.QuantityType != QuantityType.Mass)
            {
                BH.Engine.Reflection.Compute.RecordError("This EnvironmentalProductDeclaration's declared unit type is not Mass. Please supply a Mass-based EPD or try a different method.");
                return 0;
            }
            else
            {
                return mass * environmentalProductDeclaration.EnvironmentalProductDeclaration(environmentalProductDeclarationField);
            }
        }

        /***************************************************/

        [Description("This method calculates the quantity of a supplied metric per a supplied EPD and volume.")]
        [Input("environmentalProductDeclaration", "This is a LifeCycleAssessment.EnvironmentalProductDeclaration with a volume-based declared unit. Please select your desired dataset and supply your material choice to the corresponding BHoM objects.")]
        [Input("environmentalProductDeclarationField", "The metric to calculate total quantity for.")]
        [Input("volume", "The total volume to calculate the total quantity of the input metric for.", typeof(Volume))]
        [Output("quantity", "The total quantity of the desired metric based on the EnvironmentalProductDeclarationField")]
        public static double EvaluateEnvironmentalProductDeclarationByVolume(IEnvironmentalProductDeclarationData environmentalProductDeclaration = null, EnvironmentalProductDeclarationField environmentalProductDeclarationField = EnvironmentalProductDeclarationField.GlobalWarmingPotential, double volume = 0)
        {
            if (environmentalProductDeclaration.QuantityType != QuantityType.Volume)
            {
                BH.Engine.Reflection.Compute.RecordError("This EnvironmentalProductDeclaration's declared unit type is not Volume. Please supply a Volume-based EPD or try a different method.");
                return 0;
            }
            else
            {
                return volume * environmentalProductDeclaration.EnvironmentalProductDeclaration(environmentalProductDeclarationField);
            }
        }

        /***************************************************/

        [Description("This method calculates the quantity of a supplied metric per a supplied EPD and area.")]
        [Input("environmentalProductDeclaration", "This is a LifeCycleAssessment.EnvironmentalProductDeclaration with an area-based declared unit. Please select your desired dataset and supply your material choice to the corresponding BHoM objects.")]
        [Input("environmentalProductDeclarationField", "The metric to calculate total quantity for.")]
        [Input("area", "The total area to calculate the total quantity of the input metric for.", typeof(Area))]
        [Output("quantity", "The total quantity of the desired metric based on the EnvironmentalProductDeclarationField")]
        public static double EvaluateEnvironmentalProductDeclarationByArea(IEnvironmentalProductDeclarationData environmentalProductDeclaration = null, EnvironmentalProductDeclarationField environmentalProductDeclarationField = EnvironmentalProductDeclarationField.GlobalWarmingPotential, double area = 0)
        {
            if (environmentalProductDeclaration.QuantityType != QuantityType.Area)
            {
                BH.Engine.Reflection.Compute.RecordError("This EnvironmentalProductDeclaration's declared unit type is not Area. Please supply an Area-based EPD or try a different method.");
                return 0;
            }
            else
            {
                return area * environmentalProductDeclaration.EnvironmentalProductDeclaration(environmentalProductDeclarationField);
            }
        }

    }
}
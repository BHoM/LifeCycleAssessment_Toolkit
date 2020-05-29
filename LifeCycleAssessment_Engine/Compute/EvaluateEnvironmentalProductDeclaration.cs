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

namespace BH.Engine.LifeCycleAssessment
{
    public static partial class Compute
    {
        /***************************************************/
        /****   Public Methods                          ****/
        /***************************************************/

        public static double EvaluateLifeCycleAssessment(ProjectLifeCycleAssessment lca, EnvironmentalProductDeclarationField field)
        {
            //Check for object materialFragments
            if(lca != null)
            {
                //Find fragments on objects and extract
                double structuresVal = 0;
                double structuresDensity = 0;
                double foundationsVal = 0;
                double foundationsDensity = 0;
                double enclosuresVal = 0;
                double enclosuresDensity = 0;
                double mepVal = 0;
                double mepDensity = 0;
                double tiVal = 0;
                double tiDensity = 0;

                //Structures Scope MaterialFragment Values
                if (lca.StructuresScope == null)
                {
                    structuresVal = double.NaN;
                    BH.Engine.Reflection.Compute.RecordError("Scope objects do not contain any information to evaluate. Please add relevant objects with their corresponding materialFragments.");
                    return structuresVal;
                }
                if (lca.StructuresScope != null)
                {
                    //Structural Slabs
                    structuresVal += lca.StructuresScope.StructuresSlabs.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetEvaluationValue(field)).Sum();
                    structuresDensity += lca.StructuresScope.StructuresSlabs.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetFragmentDensity()).Sum();
                    
                    //Structural Beams
                    structuresVal += lca.StructuresScope.StructuresBeams.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetEvaluationValue(field)).Sum();
                    structuresDensity += lca.StructuresScope.StructuresBeams.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetFragmentDensity()).Sum();

                    //Structural Columns
                    structuresVal += lca.StructuresScope.StructuresColumns.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetEvaluationValue(field)).Sum();
                    structuresDensity += lca.StructuresScope.StructuresColumns.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetFragmentDensity()).Sum();

                    //Structural Core Walls
                    structuresVal += lca.StructuresScope.StructuresCoreWalls.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetEvaluationValue(field)).Sum();
                    structuresDensity += lca.StructuresScope.StructuresCoreWalls.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetFragmentDensity()).Sum();

                    return structuresVal * structuresDensity;
                }
                
                //Foundations Scope MaterialFragment Values
                if(lca.FoundationsScope == null)
                {
                    foundationsVal = double.NaN;
                    BH.Engine.Reflection.Compute.RecordError("FoundationsScope Object does not contain any information to evaluate. Please add relevant objects with their corresponding materialFragments.");
                    return foundationsVal;
                }
                if(lca.FoundationsScope != null)
                {
                    //Foundations Footings
                    foundationsVal += lca.FoundationsScope.FoundationsFootings.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetEvaluationValue(field)).Sum();
                    foundationsDensity += lca.FoundationsScope.FoundationsFootings.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetFragmentDensity()).Sum();

                    //Foundations Piles
                    foundationsVal += lca.FoundationsScope.FoundationsPiles.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetEvaluationValue(field)).Sum();
                    foundationsDensity += lca.FoundationsScope.FoundationsPiles.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetFragmentDensity()).Sum();

                    //Foundations Slabs
                    foundationsVal += lca.FoundationsScope.FoundationsSlabs.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetEvaluationValue(field)).Sum();
                    foundationsDensity += lca.FoundationsScope.FoundationsSlabs.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetFragmentDensity()).Sum();

                    //Foundations Walls
                    foundationsVal += lca.FoundationsScope.FoundationsWalls.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetEvaluationValue(field)).Sum();
                    foundationsDensity += lca.FoundationsScope.FoundationsWalls.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetFragmentDensity()).Sum();

                    return foundationsVal * foundationsDensity;
                }

                //Enclosures Scope MaterialFragment Values
                if (lca.EnclosuresScope == null)
                {
                    enclosuresVal = double.NaN;
                    BH.Engine.Reflection.Compute.RecordError("EnclosureScope object does not contain any information to evaluate. Please add relevant objects with their corresponding materialFragments.");
                    return enclosuresVal;
                }
                if (lca.EnclosuresScope != null)
                {
                    //Enclosures Curtain Walls
                    enclosuresVal += lca.EnclosuresScope.EnclosuresCurtainWalls.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetEvaluationValue(field)).Sum();
                    enclosuresDensity += lca.EnclosuresScope.EnclosuresCurtainWalls.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetFragmentDensity()).Sum();

                    //Enclosures Doors
                    enclosuresVal += lca.EnclosuresScope.EnclosuresDoors.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetEvaluationValue(field)).Sum();
                    enclosuresDensity += lca.EnclosuresScope.EnclosuresDoors.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetFragmentDensity()).Sum();

                    //Enclosures Walls
                    enclosuresVal += lca.EnclosuresScope.EnclosuresWalls.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetEvaluationValue(field)).Sum();
                    enclosuresDensity += lca.EnclosuresScope.EnclosuresWalls.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetFragmentDensity()).Sum();

                    //Enclosures Windows
                    enclosuresVal += lca.EnclosuresScope.EnclosuresWindows.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetEvaluationValue(field)).Sum();
                    enclosuresDensity += lca.EnclosuresScope.EnclosuresWindows.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetFragmentDensity()).Sum();

                    return enclosuresVal * enclosuresDensity;
                }

                //MEP Scope MaterialFragment Values
                if (lca.MEPScope == null)
                {
                    mepVal = double.NaN;
                    BH.Engine.Reflection.Compute.RecordError("MEPScope object does not contain any information to evaluate. Please add relevant objects with their corresponding materialFragments.");
                    return mepVal;
                }
                if (lca.MEPScope != null)
                {
                    //MEP Batteries
                    mepVal += lca.MEPScope.MEPBatteries.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetEvaluationValue(field)).Sum();
                    mepDensity += lca.MEPScope.MEPBatteries.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetFragmentDensity()).Sum();

                    //MEP Conduit
                    mepVal += lca.MEPScope.MEPConduit.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetEvaluationValue(field)).Sum();
                    mepDensity += lca.MEPScope.MEPConduit.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetFragmentDensity()).Sum();

                    //MEP Ductwork
                    mepVal += lca.MEPScope.MEPDuctwork.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetEvaluationValue(field)).Sum();
                    mepDensity += lca.MEPScope.MEPDuctwork.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetFragmentDensity()).Sum();

                    //MEP Equipment
                    mepVal += lca.MEPScope.MEPEquipment.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetEvaluationValue(field)).Sum();
                    mepDensity += lca.MEPScope.MEPEquipment.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetFragmentDensity()).Sum();

                    //MEP Generators
                    mepVal += lca.MEPScope.MEPGenerators.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetEvaluationValue(field)).Sum();
                    mepDensity += lca.MEPScope.MEPGenerators.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetFragmentDensity()).Sum();

                    //MEP Lighting
                    mepVal += lca.MEPScope.MEPLighting.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetEvaluationValue(field)).Sum();
                    mepDensity += lca.MEPScope.MEPLighting.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetFragmentDensity()).Sum();

                    //MEP Piping
                    mepVal += lca.MEPScope.MEPPiping.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetEvaluationValue(field)).Sum();
                    mepDensity += lca.MEPScope.MEPPiping.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetFragmentDensity()).Sum();

                    //MEP Wiring
                    mepVal += lca.MEPScope.MEPWiring.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetEvaluationValue(field)).Sum();
                    mepDensity += lca.MEPScope.MEPWiring.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetFragmentDensity()).Sum();

                    return mepVal * mepDensity;
                }

                //Tenant Improvement Scope MaterialFragment Values
                if (lca.TenantImprovementScope == null)
                {
                    tiVal = double.NaN;
                    BH.Engine.Reflection.Compute.RecordError("TenantImprovementScope object does not contain any information to evaluate. Please add relevant objects with their corresponding materialFragments.");
                    return tiVal;
                }
                if (lca.TenantImprovementScope != null)
                {
                    //Tenant Improvements Ceiling 
                    tiVal += lca.TenantImprovementScope.TenantImprovementsCeiling.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetEvaluationValue(field)).Sum();
                    tiDensity += lca.TenantImprovementScope.TenantImprovementsCeiling.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetFragmentDensity()).Sum();
                    
                    //Tenant Improvements Finishes
                    tiVal += lca.TenantImprovementScope.TenantImprovementsFinishes.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetEvaluationValue(field)).Sum();
                    tiDensity += lca.TenantImprovementScope.TenantImprovementsFinishes.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetFragmentDensity()).Sum();

                    //Tenant Improvements Flooring
                    tiVal += lca.TenantImprovementScope.TenantImprovementsFlooring.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetEvaluationValue(field)).Sum();
                    tiDensity += lca.TenantImprovementScope.TenantImprovementsFlooring.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetFragmentDensity()).Sum();

                    //Tenant Improvements Furniture
                    tiVal += lca.TenantImprovementScope.TenantImprovementsFurniture.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetEvaluationValue(field)).Sum();
                    tiDensity += lca.TenantImprovementScope.TenantImprovementsFurniture.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetFragmentDensity()).Sum();

                    //Tenant Improvements Interior Doors
                    tiVal += lca.TenantImprovementScope.TenantImprovementsInteriorDoors.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetEvaluationValue(field)).Sum();
                    tiDensity += lca.TenantImprovementScope.TenantImprovementsInteriorDoors.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetFragmentDensity()).Sum();

                    //Tenant Improvements Interior Glazing
                    tiVal += lca.TenantImprovementScope.TenantImprovementsInteriorGlazing.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetEvaluationValue(field)).Sum();
                    tiDensity += lca.TenantImprovementScope.TenantImprovementsInteriorGlazing.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetFragmentDensity()).Sum();

                    //Tenant Improvements Partition Walls
                    tiVal += lca.TenantImprovementScope.TenantImprovementsPartitionWalls.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetEvaluationValue(field)).Sum();
                    tiDensity += lca.TenantImprovementScope.TenantImprovementsPartitionWalls.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetFragmentDensity()).Sum();


                    return tiVal * tiDensity;
                }

                //Combined MaterialFragment value
                double epdMetric = (structuresVal + foundationsVal + enclosuresVal + mepVal + tiVal);

                //Get Volumes from Objects

                //StructuresScope Volumes

                object StructureVol = System.Convert.ToDouble(lca.StructuresScope.StructuresSlabs.PropertyValue("Volume")) 
                    + System.Convert.ToDouble(lca.StructuresScope.StructuresCoreWalls.PropertyValue("Volume")) 
                    + System.Convert.ToDouble(lca.StructuresScope.StructuresColumns.PropertyValue("Volume")) 
                    + System.Convert.ToDouble(lca.StructuresScope.StructuresBeams.PropertyValue("Volume"));

                if (StructureVol == null)
                {
                    BH.Engine.Reflection.Compute.RecordError("The objects used within the StructuresScope must have a Volume property and value for this method to work.");
                    return 0;
                }
                double volume = System.Convert.ToDouble(StructureVol);
                if (volume <= 0)
                {
                    BH.Engine.Reflection.Compute.RecordError("The input object's Volume value is invalid or negative. Volume should be in m3 in numerical format.");
                    return 0;
                }

                //get quantityType property from MaterialFragment -- See switch case in getEvaluationValue
                if (lca.StructuresScope == null)
                {
                    structuresVolume += lca.StructuresScope.StructuresSlabs.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetFragmentVolume()).Sum();
                }

                //Check for materialFragment Quantity type and call method

                //Validity of Density
                //Check for materialFragment Density and define value

                double lifeCycleAssessmentValue = epdMetric * volume;

                return lifeCycleAssessmentValue;
            }

            //Find volume on objects and extract
            //Find density on objects and extract
            //return constant * volume * density
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
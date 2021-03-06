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

using System.ComponentModel;
using System.Linq;
using BH.Engine.Base;
using BH.oM.Base;
using BH.oM.Dimensional;
using BH.oM.LifeCycleAssessment;
using BH.oM.LifeCycleAssessment.MaterialFragments;
using BH.oM.MEP.System;
using BH.oM.Reflection.Attributes;
using BH.Engine.Matter;
using System.Collections.Generic;

namespace BH.Engine.LifeCycleAssessment
{
    /***************************************************/
    /****   Public Methods                          ****/
    /***************************************************/
    public static partial class Query
    {
        [Description("Return a sum of all Material Fragment values from a specified EnvironmentalProductDeclarationField within any EPD object.")]
        [Input("epd", "Environmental Product Declaration to query the field value from.")]
        [Input("field", "Specific metric to query from provided Environmental Product Declarations.")]
        [Output("evaluationValue", "The Environmental Impact metric value for the specified field.")]
        public static double GetEvaluationValue(this IEnvironmentalProductDeclarationData epd, EnvironmentalProductDeclarationField field)
        {
            if (epd == null)
                return double.NaN;

            switch(field)
            {
                case EnvironmentalProductDeclarationField.AcidificationPotential:
                    return epd.AcidificationPotential;
                case EnvironmentalProductDeclarationField.DepletionOfAbioticResourcesFossilFuels:
                    return epd.DepletionOfAbioticResourcesFossilFuels;
                case EnvironmentalProductDeclarationField.EutrophicationPotential:
                    return epd.EutrophicationPotential;
                case EnvironmentalProductDeclarationField.GlobalWarmingPotential:
                    return epd.GlobalWarmingPotential;
                case EnvironmentalProductDeclarationField.OzoneDepletionPotential:
                    return epd.OzoneDepletionPotential;
                case EnvironmentalProductDeclarationField.PhotochemicalOzoneCreationPotential:
                    return epd.PhotochemicalOzoneCreationPotential;
                default:
                    return double.NaN;
            }
        }

        /***************************************************/

        [Description("Returns the Environmental Impact metric value for the specified field input from the Environmental Product Declaration found within the MaterialComposition of an object.")]
        [Input("elementM", "An IElementM object with a MaterialProperty from which to query the desired metric.")]
        [Input("field", "Specific metric to query from provided Environmental Product Declarations.")]
        [Output("evaluationValue", "The Environmental Impact metric value for the specified field.")]
        public static List<double> GetEvaluationValue(this IElementM elementM, EnvironmentalProductDeclarationField field, QuantityType type)
        {
            if (elementM == null)
                return new List<double>();

            List<double> quantityTypeValue = elementM.GetQuantityTypeValue(type);

            List<double> epdVal = elementM.IMaterialComposition().Materials.Select(x =>
            {
                var epd = x.Properties.Where(y => y is IEnvironmentalProductDeclarationData).FirstOrDefault() as IEnvironmentalProductDeclarationData;
                if (epd.QuantityType == type)
                    return GetEvaluationValue(epd, field);
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

        [Description("Return a sum of all Material Fragment values from a specified EnvironmentalProductDeclarationField within any Life Cycle Assessment Scope object to be used within a ProjectLifeCycleAssessment.")]
        [Input("lcaScope", "Scope to extract Environmental Product Declaration field value from.")]
        [Input("field", "Specific metric to query from provided Environmental Product Declarations.")]
        [Output("evaluationValue", "The Environmental Impact metric value for the specified field.")]
        public static double GetEvaluationValue(this IBHoMObject lcaScope, EnvironmentalProductDeclarationField field)
        {
            return lcaScope.GetEvaluationValue(field);
        }

        /***************************************************/

        [Description("Return a sum of all Material Fragment values from a specified EnvironmentalProductDeclarationField within any StructuresScope object to be used within a ProjectLifeCycleAssessment.")]
        [Input("structuresScope", "Scope to extract Environmental Product Declaration field value from.")]
        [Input("field", "Specific metric to query from provided Environmental Product Declarations.")]
        [Output("evaluationValue", "The Environmental Impact metric value for the specified field.")]
        public static double GetEvaluationValueStructuresScope(this StructuresScope structuresScope, EnvironmentalProductDeclarationField field)
        {
            double val = 0;

            val += structuresScope.Slabs.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetEvaluationValue(field)).Sum();
            val += structuresScope.Beams.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetEvaluationValue(field)).Sum();
            val += structuresScope.Columns.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetEvaluationValue(field)).Sum();
            val += structuresScope.CoreWalls.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetEvaluationValue(field)).Sum();
            val += structuresScope.Bracing.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetEvaluationValue(field)).Sum();
            val += structuresScope.AdditionalObjects.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetEvaluationValue(field)).Sum();

            return val;
        }

        /***************************************************/

        [Description("Return a sum of all Material Fragment values from a specified EnvironmentalProductDeclarationField within any FoundationsScope object to be used within a ProjectLifeCycleAssessment.")]
        [Input("foundationsScope", "Scope to extract Environmental Product Declaration field value from.")]
        [Input("field", "Specific metric to query from provided Environmental Product Declarations.")]
        [Output("evaluationValue", "The Environmental Impact metric value for the specified field.")]
        public static double GetEvaluationValueFoundationsScope(this FoundationsScope foundationsScope, EnvironmentalProductDeclarationField field)
        {
            double val = 0;

            val += foundationsScope.Footings.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetEvaluationValue(field)).Sum();
            val += foundationsScope.Piles.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetEvaluationValue(field)).Sum();
            val += foundationsScope.Slabs.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetEvaluationValue(field)).Sum();
            val += foundationsScope.Walls.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetEvaluationValue(field)).Sum();
            val += foundationsScope.GradeBeams.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetEvaluationValue(field)).Sum();
            val += foundationsScope.AdditionalObjects.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetEvaluationValue(field)).Sum();

            return val;
        }

        /***************************************************/

        [Description("Return a sum of all Material Fragment values from a specified EnvironmentalProductDeclarationField within any EnclosuresScope object to be used within a ProjectLifeCycleAssessment.")]
        [Input("enclosuresScope", "Scope to extract Environmental Product Declaration field value from.")]
        [Input("field", "Specific metric to query from provided Environmental Product Declarations.")]
        [Output("evaluationValue", "The Environmental Impact metric value for the specified field.")]
        public static double GetEvaluationValueEnclosuresScope(this EnclosuresScope enclosuresScope, EnvironmentalProductDeclarationField field)
        {
            double val = 0;

            val += enclosuresScope.CurtainWalls.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetEvaluationValue(field)).Sum();
            val += enclosuresScope.Doors.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetEvaluationValue(field)).Sum();
            val += enclosuresScope.Walls.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetEvaluationValue(field)).Sum();
            val += enclosuresScope.Windows.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetEvaluationValue(field)).Sum();
            val += enclosuresScope.AdditionalObjects.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetEvaluationValue(field)).Sum();

            return val;
        }

        /***************************************************/

        [Description("Return a sum of all Material Fragment values from a specified EnvironmentalProductDeclarationField within any MEPScope object to be used within a ProjectLifeCycleAssessment.")]
        [Input("mepScope", "Scope to extract Environmental Product Declaration field value from.")]
        [Input("field", "Specific metric to query from provided Environmental Product Declarations.")]
        [Output("evaluationValue", "The Environmental Impact metric value for the specified field.")]
        public static double GetEvaluationValueMEPScope(this MEPScope mepScope, EnvironmentalProductDeclarationField field)
        {
            double val = 0;

            //MechanicalScope
            val += mepScope.MechanicalScope.AirTerminals.Select(x => (x as Duct).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetEvaluationValue(field)).Sum();
            val += mepScope.MechanicalScope.Dampers.Select(x => (x as Duct).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetEvaluationValue(field)).Sum();
            val += mepScope.MechanicalScope.Ducts.Select(x => (x as Duct).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetEvaluationValue(field)).Sum();
            val += mepScope.MechanicalScope.Equipment.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetEvaluationValue(field)).Sum();
            val += mepScope.MechanicalScope.Pipes.Select(x => (x as Pipe).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetEvaluationValue(field)).Sum();
            val += mepScope.MechanicalScope.Refrigerants.Select(x => (x as Duct).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetEvaluationValue(field)).Sum();
            val += mepScope.MechanicalScope.Tanks.Select(x => (x as Duct).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetEvaluationValue(field)).Sum();
            val += mepScope.MechanicalScope.Valves.Select(x => (x as Duct).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetEvaluationValue(field)).Sum();
            val += mepScope.MechanicalScope.AdditionalObjects.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetEvaluationValue(field)).Sum();

            //ElectricalScope
            val += mepScope.ElectricalScope.Batteries.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetEvaluationValue(field)).Sum();
            val += mepScope.ElectricalScope.CableTrays.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetEvaluationValue(field)).Sum();
            val += mepScope.ElectricalScope.Conduit.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetEvaluationValue(field)).Sum();
            val += mepScope.ElectricalScope.Equipment.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetEvaluationValue(field)).Sum();
            val += mepScope.ElectricalScope.FireAlarmDevices.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetEvaluationValue(field)).Sum();
            val += mepScope.ElectricalScope.Generators.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetEvaluationValue(field)).Sum();
            val += mepScope.ElectricalScope.InformationCommunicationDevices.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetEvaluationValue(field)).Sum();
            val += mepScope.ElectricalScope.LightFixtures.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetEvaluationValue(field)).Sum();
            val += mepScope.ElectricalScope.LightingControls.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetEvaluationValue(field)).Sum();
            val += mepScope.ElectricalScope.Meters.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetEvaluationValue(field)).Sum();
            val += mepScope.ElectricalScope.SecurityDevices.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetEvaluationValue(field)).Sum();
            val += mepScope.ElectricalScope.Sockets.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetEvaluationValue(field)).Sum();
            val += mepScope.ElectricalScope.SolarPanels.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetEvaluationValue(field)).Sum();
            val += mepScope.ElectricalScope.WireSegments.Select(x => (x as WireSegment).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetEvaluationValue(field)).Sum();
            val += mepScope.ElectricalScope.AdditionalObjects.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetEvaluationValue(field)).Sum();

            //PlumbingScope
            val += mepScope.PlumbingScope.Equipment.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetEvaluationValue(field)).Sum();
            val += mepScope.PlumbingScope.Pipes.Select(x => (x as Pipe).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetEvaluationValue(field)).Sum();
            val += mepScope.PlumbingScope.PlumbingFixtures.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetEvaluationValue(field)).Sum();
            val += mepScope.PlumbingScope.Tanks.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetEvaluationValue(field)).Sum();
            val += mepScope.PlumbingScope.Valves.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetEvaluationValue(field)).Sum();
            val += mepScope.PlumbingScope.AdditionalObjects.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetEvaluationValue(field)).Sum();

            //FireProtectionScope
            val += mepScope.FireProtectionScope.Equipment.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetEvaluationValue(field)).Sum();
            val += mepScope.FireProtectionScope.Pipes.Select(x => (x as Pipe).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetEvaluationValue(field)).Sum();
            val += mepScope.FireProtectionScope.Sprinklers.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetEvaluationValue(field)).Sum();
            val += mepScope.FireProtectionScope.Tanks.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetEvaluationValue(field)).Sum();
            val += mepScope.FireProtectionScope.AdditionalObjects.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetEvaluationValue(field)).Sum();

            return val;
        }

        /***************************************************/

        [Description("Return a sum of all Material Fragment values from a specified EnvironmentalProductDeclarationField within any TenantImprovementScope object to be used within a ProjectLifeCycleAssessment.")]
        [Input("tenantImprovementScope", "Scope to extract Environmental Product Declaration field value from.")]
        [Input("field", "Specific metric to query from provided Environmental Product Declarations.")]
        [Output("evaluationValue", "The Environmental Impact metric value for the specified field.")]
        public static double GetEvaluationValueTenantImprovementScope(this TenantImprovementScope tenantImprovementScope, EnvironmentalProductDeclarationField field)
        {
            double val = 0;

            val += tenantImprovementScope.Ceiling.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetEvaluationValue(field)).Sum();
            val += tenantImprovementScope.Finishes.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetEvaluationValue(field)).Sum();
            val += tenantImprovementScope.Flooring.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetEvaluationValue(field)).Sum();
            val += tenantImprovementScope.Furniture.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetEvaluationValue(field)).Sum();
            val += tenantImprovementScope.InteriorDoors.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetEvaluationValue(field)).Sum();
            val += tenantImprovementScope.InteriorGlazing.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetEvaluationValue(field)).Sum();
            val += tenantImprovementScope.PartitionWalls.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetEvaluationValue(field)).Sum();
            val += tenantImprovementScope.AdditionalObjects.Select(x => (x as BHoMObject).GetAllFragments().Where(y => typeof(IEnvironmentalProductDeclarationData).IsAssignableFrom(y.GetType())).Select(z => z as IEnvironmentalProductDeclarationData).FirstOrDefault().GetEvaluationValue(field)).Sum();

            return val;
        }
        /***************************************************/
    }
}


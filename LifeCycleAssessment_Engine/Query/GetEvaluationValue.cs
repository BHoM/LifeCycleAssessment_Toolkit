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

using BH.Engine.Matter;
using BH.oM.Dimensional;
using BH.oM.LifeCycleAssessment;
using BH.oM.LifeCycleAssessment.MaterialFragments;
using BH.oM.Reflection.Attributes;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

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

            switch (field)
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

        public static double IGetEvaluationValue(IScope scope, EnvironmentalProductDeclarationField field)
        {
            return GetEvaluationValue(scope as dynamic, field);
        }

        /***************************************************/
        /****          Private Methods                  ****/
        /***************************************************/

        private static double GetEvaluationValue(object obj, EnvironmentalProductDeclarationField field)
        {
            BH.Engine.Reflection.Compute.RecordError($"Object of type {obj.GetType()} does not have an EvaluationValue method.");
            return 0;
        }

        /***************************************************/

        private static double GetEvaluationValue(StructuresScope scope, EnvironmentalProductDeclarationField field)
        {
            double val = 0;

            val += scope.Slabs.Select(x => GetEvaluationValue(x, field).Sum()).Sum();
            val += scope.CoreWalls.Select(x => GetEvaluationValue(x, field).Sum()).Sum();
            val += scope.Beams.Select(x => GetEvaluationValue(x, field).Sum()).Sum();
            val += scope.Columns.Select(x => GetEvaluationValue(x, field).Sum()).Sum();
            val += scope.Bracing.Select(x => GetEvaluationValue(x, field).Sum()).Sum();
            val += scope.AdditionalObjects.Select(x => GetEvaluationValue(x, field).Sum()).Sum();

            return val;
        }

        /***************************************************/

        private static double GetEvaluationValue(FoundationsScope scope, EnvironmentalProductDeclarationField field)
        {
            double val = 0;

            val += scope.Columns.Select(x => GetEvaluationValue(x, field).Sum()).Sum();
            val += scope.Footings.Select(x => GetEvaluationValue(x, field).Sum()).Sum();
            val += scope.Piles.Select(x => GetEvaluationValue(x, field).Sum()).Sum();
            val += scope.Walls.Select(x => GetEvaluationValue(x, field).Sum()).Sum();
            val += scope.Slabs.Select(x => GetEvaluationValue(x, field).Sum()).Sum();
            val += scope.GradeBeams.Select(x => GetEvaluationValue(x, field).Sum()).Sum();
            val += scope.AdditionalObjects.Select(x => GetEvaluationValue(x, field).Sum()).Sum();

            return val;
        }

        /***************************************************/
        private static double GetEvaluationValue(EnclosuresScope scope, EnvironmentalProductDeclarationField field)
        {
            double val = 0;

            val += scope.Walls.Select(x => GetEvaluationValue(x, field).Sum()).Sum();
            val += scope.CurtainWalls.Select(x => GetEvaluationValue(x, field).Sum()).Sum();
            val += scope.Windows.Select(x => GetEvaluationValue(x, field).Sum()).Sum();
            val += scope.Doors.Select(x => GetEvaluationValue(x, field).Sum()).Sum();
            val += scope.AdditionalObjects.Select(x => GetEvaluationValue(x, field).Sum()).Sum();

            return val;
        }

        /***************************************************/
        private static double GetEvaluationValue(MechanicalScope scope, EnvironmentalProductDeclarationField field)
        {
            double val = 0;

            val += scope.AirTerminals.Select(x => GetEvaluationValue(x, field).Sum()).Sum();
            val += scope.Dampers.Select(x => GetEvaluationValue(x, field).Sum()).Sum();
            val += scope.Ducts.Select(x => GetEvaluationValue(x, field).Sum()).Sum();
            val += scope.Equipment.Select(x => GetEvaluationValue(x, field).Sum()).Sum();
            val += scope.Pipes.Select(x => GetEvaluationValue(x, field).Sum()).Sum();
            val += scope.Refrigerants.Select(x => GetEvaluationValue(x, field).Sum()).Sum();
            val += scope.Tanks.Select(x => GetEvaluationValue(x, field).Sum()).Sum();
            val += scope.Valves.Select(x => GetEvaluationValue(x, field).Sum()).Sum();
            val += scope.AdditionalObjects.Select(x => GetEvaluationValue(x, field).Sum()).Sum();

            return val;
        }

        /***************************************************/
        private static double GetEvaluationValue(ElectricalScope scope, EnvironmentalProductDeclarationField field)
        {
            double val = 0;

            val += scope.Batteries.Select(x => GetEvaluationValue(x, field).Sum()).Sum();
            val += scope.CableTrays.Select(x => GetEvaluationValue(x, field).Sum()).Sum();
            val += scope.Conduit.Select(x => GetEvaluationValue(x, field).Sum()).Sum();
            val += scope.Equipment.Select(x => GetEvaluationValue(x, field).Sum()).Sum();
            val += scope.FireAlarmDevices.Select(x => GetEvaluationValue(x, field).Sum()).Sum();
            val += scope.Generators.Select(x => GetEvaluationValue(x, field).Sum()).Sum();
            val += scope.InformationCommunicationDevices.Select(x => GetEvaluationValue(x, field).Sum()).Sum();
            val += scope.LightFixtures.Select(x => GetEvaluationValue(x, field).Sum()).Sum();
            val += scope.LightingControls.Select(x => GetEvaluationValue(x, field).Sum()).Sum();
            val += scope.Meters.Select(x => GetEvaluationValue(x, field).Sum()).Sum();
            val += scope.SecurityDevices.Select(x => GetEvaluationValue(x, field).Sum()).Sum();
            val += scope.SolarPanels.Select(x => GetEvaluationValue(x, field).Sum()).Sum();
            val += scope.WireSegments.Select(x => GetEvaluationValue(x, field).Sum()).Sum();
            val += scope.AdditionalObjects.Select(x => GetEvaluationValue(x, field).Sum()).Sum();

            return val;
        }

        /***************************************************/
        private static double GetEvaluationValue(PlumbingScope scope, EnvironmentalProductDeclarationField field)
        {
            double val = 0;

            val += scope.Equipment.Select(x => GetEvaluationValue(x, field).Sum()).Sum();
            val += scope.Pipes.Select(x => GetEvaluationValue(x, field).Sum()).Sum();
            val += scope.PlumbingFixtures.Select(x => GetEvaluationValue(x, field).Sum()).Sum();
            val += scope.Tanks.Select(x => GetEvaluationValue(x, field).Sum()).Sum();
            val += scope.Valves.Select(x => GetEvaluationValue(x, field).Sum()).Sum();
            val += scope.AdditionalObjects.Select(x => GetEvaluationValue(x, field).Sum()).Sum();

            return val;
        }

        /***************************************************/
        private static double GetEvaluationValue(FireProtectionScope scope, EnvironmentalProductDeclarationField field)
        {
            double val = 0;

            val += scope.Equipment.Select(x => GetEvaluationValue(x, field).Sum()).Sum();
            val += scope.Pipes.Select(x => GetEvaluationValue(x, field).Sum()).Sum();
            val += scope.Sprinklers.Select(x => GetEvaluationValue(x, field).Sum()).Sum();
            val += scope.Tanks.Select(x => GetEvaluationValue(x, field).Sum()).Sum();
            val += scope.AdditionalObjects.Select(x => GetEvaluationValue(x, field).Sum()).Sum();

            return val;
        }

        /***************************************************/
        private static double GetEvaluationValue(TenantImprovementScope scope, EnvironmentalProductDeclarationField field)
        {
            double val = 0;

            val += scope.Ceiling.Select(x => GetEvaluationValue(x, field).Sum()).Sum();
            val += scope.Flooring.Select(x => GetEvaluationValue(x, field).Sum()).Sum();
            val += scope.Finishes.Select(x => GetEvaluationValue(x, field).Sum()).Sum();
            val += scope.InteriorGlazing.Select(x => GetEvaluationValue(x, field).Sum()).Sum();
            val += scope.Furniture.Select(x => GetEvaluationValue(x, field).Sum()).Sum();
            val += scope.InteriorDoors.Select(x => GetEvaluationValue(x, field).Sum()).Sum();
            val += scope.PartitionWalls.Select(x => GetEvaluationValue(x, field).Sum()).Sum();
            val += scope.AdditionalObjects.Select(x => GetEvaluationValue(x, field).Sum()).Sum();

            return val;
        }

        /***************************************************/
    }
}


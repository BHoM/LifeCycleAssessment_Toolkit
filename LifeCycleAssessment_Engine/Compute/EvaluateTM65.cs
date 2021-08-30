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

using System;
using BH.oM.Reflection.Attributes;
using System.ComponentModel;
using BH.oM.LifeCycleAssessment;
using BH.oM.LifeCycleAssessment.MaterialFragments;
using BH.oM.Physical.Materials;
using System.Collections.Generic;
using BH.Engine.Matter;

namespace BH.Engine.LifeCycleAssessment
{
    public static partial class Compute
    {
        /***************************************************/
        /****   Public Methods                          ****/
        /***************************************************/

        [Description("Returns reinforcement volume.")]
        [Input("name", "Name of the equipment type being evaluated. This name will be used to identify the complexity multiplier.")]
        [Input("weight", "Weight of the equipment in kg.")]
        [Input("quantity", "The amount of determined QuantityType.")]
        [Input("materialComposition", "The material composition of the object being evaluated. This is a breakdown of materials by ratio.")]
        [Output("epd", "An Environmental Product Declaration.")]
        public static EnvironmentalProductDeclaration EvaluateTM65 (string name, double weight, double quantity, QuantityType quantityType, MaterialComposition materialComposition) //add optional refrigerant input after prototype is working.
        {
            // TM65 calculation 
            // TotalWeight x ExplicitBulkByMass x 1.1 (ReplacementWeightIncrease) x Scale Up Factor x 1.3 (Buffer Factor) + RefrigerantTotalFromImageBelow

            // Force input to lower case
            name = name.ToLower();

            double replacementWeightIncrease = 1.1; // is this constant or does this need to be variable? 
            double bufferFactor = 1.3; // is this constant or does this need to be variable?
            // Refrigerant to be added later
            
            // Define complexity lists
            // Make this an input 
            EPDComplexity scaleUpFactor = EPDComplexity.Undefined;

            // Remove lists and add as description to epdComplexity enum

            // Class 1 objects > scaleUpFactor == 1.1
            List<string> class1 = new List<string>() { 
                "pipe",
                "cables",
                "ducts",
                "valves",
                "fire alarm",
                "access control",
                "cable containment",
                "electrical outlets",
                "busbars"
            };

            // Class 2 objects > scaleUpFactor == 1.2
            List<string> class2 = new List<string>() {
                "pumps",
                "luminaires",
                "radiators",
                "control panels",
                "lighting control devices",
                "sensors",
                "thermal store"
            };

            // Class 3 objects > scaleUpFactor == 1.3
            List<string> class3 = new List<string>() {
                "air handling units",
                "heat pumps",
                "boilers",
                "heat interface units",
                "chillers",
                "generator",
                "MVHR",
                "switchgear",
                "UPS"
            };

            if (class1.Contains(name))
            {
                scaleUpFactor = EPDComplexity.Class1;
            }
            else if (class2.Contains(name))
            {
                scaleUpFactor = EPDComplexity.Class2;
            }
            else if (class3.Contains(name))
            {
                scaleUpFactor = EPDComplexity.Class3;
            }

            double scaleFactor = 1;

            switch (scaleUpFactor)
            {
                case EPDComplexity.Class1:
                    scaleFactor = 1.1;
                    break;

                case EPDComplexity.Class2:
                    scaleFactor = 1.2;
                    break;

                case EPDComplexity.Class3:
                    scaleFactor = 1.3;
                    break;
            }

            // This is the calculation 
            // Need Mass = Density * Volume for all material composition inputs 
            // Density => All EPDDensity values 
            // Volume => ???

            // TODO Only use kg based EPDs and remove the Mass query -> give error message if QT is not kg

            double totalQuantity = weight * /*BH.Engine.Matter.Query.Mass(materialComposition)*/ replacementWeightIncrease * scaleFactor * bufferFactor;

            // inputs required to construct an Environmental Metric
            List<LifeCycleAssessmentPhases> phases = new List<LifeCycleAssessmentPhases>() { LifeCycleAssessmentPhases.A1, LifeCycleAssessmentPhases.A2, LifeCycleAssessmentPhases.A3}; // This is hard coded to A1-A3, not sure if this is desired. But it should definitely be stated when using the method. 
            EnvironmentalProductDeclarationField field = EnvironmentalProductDeclarationField.GlobalWarmingPotential;

            // Create a new environmental metric using the TotalQuantity produced from the method calc
            EnvironmentalMetric metric = new EnvironmentalMetric(field, phases, totalQuantity);

            // construct an new EPD
            BH.oM.LifeCycleAssessment.MaterialFragments.EnvironmentalProductDeclaration epd = new oM.LifeCycleAssessment.MaterialFragments.EnvironmentalProductDeclaration { Type = EPDType.Product, EnvironmentalMetric = new List<EnvironmentalMetric> { metric }, QuantityType = quantityType, QuantityTypeValue = quantity};

            return epd;
        }
        /***************************************************/

    }
}

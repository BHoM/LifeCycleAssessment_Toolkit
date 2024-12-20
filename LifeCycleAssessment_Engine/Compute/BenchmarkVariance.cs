/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2025, the respective contributors. All rights reserved.
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
using BH.oM.Base.Attributes;
using BH.oM.LifeCycleAssessment;

namespace BH.Engine.LifeCycleAssessment
{
    public static partial class Compute
    {
        /***************************************************/
        /****   Public Methods                          ****/
        /***************************************************/

        [Description("Calculates the percentage change of kgCO2 between a proposed, user-created building and a baseline building based on a defined building type.")]
        [Input("projectEmbodiedCarbon", "Combined kgCO2 per building.")]
        [Input("projectArea", "Total area of the building, including all floor surface area in m2.")]
        [Input("embodiedCarbonBenchmarkTypeDataset", "Benchmark kgCO2/m2 per building type based on program benchmark dataset.")]
        [Input("embodiedCarbonBenchmarkStructureDataset", "Benchmark kgCO2/m2 per building structure based on structural benchmark dataset.")]
        [Input("typeWeighting", "Subjective weighting of program type benchmark results. Used in LCA to prioritize different CO2 dataset assumptions.")]
        [Input("structureWeighting", "Subjective weighting of structure type benchmark results. Used in LCA to prioritize different CO2 dataset assumptions.")]
        [Output("embodiedCarbonPercentageVariance", "Percentage variance between current project being evaluated and benchmark datasets, e.g. -30% for a building that is 30% less than the benchmark.")]
        public static double BenchmarkVariance(double projectEmbodiedCarbon, double projectArea, BuildingBenchmarkingData embodiedCarbonBenchmarkTypeDataset, BuildingBenchmarkingData embodiedCarbonBenchmarkStructureDataset, double typeWeighting, double structureWeighting)
        {

            double typeBenchmark = embodiedCarbonBenchmarkTypeDataset.Average;

            double structureBenchmark = embodiedCarbonBenchmarkStructureDataset.Average;

            double weightedAverage = (((typeBenchmark * typeWeighting) + (structureBenchmark * structureWeighting)) / (typeWeighting + structureWeighting));

            return ((((projectEmbodiedCarbon / projectArea) - weightedAverage) / weightedAverage) * 100);
        }
        /***************************************************/
    }
}






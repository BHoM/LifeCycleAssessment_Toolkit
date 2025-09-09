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


using BH.oM.Base.Attributes;
using BH.oM.LifeCycleAssessment;
using BH.oM.LifeCycleAssessment.MaterialFragments;
using BH.oM.LifeCycleAssessment.MaterialFragments.EndOfLife;
using BH.oM.Physical.Materials;
using BH.oM.Quantities.Attributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace BH.Engine.LifeCycleAssessment
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Calculates waste and disposal results for climate change metrics based on waste disposal factors. The method computes fossil, biogenic, land use, and total climate change impacts for end-of-life disposal phases (C3-C4 modules), with optional biogenic carbon cancellation.")]
        [Input("disposalFactors", "Waste and disposal factors containing fossil waste factor and configuration for biogenic carbon cancellation and EPD value override settings.")]
        [Input("mass", "Mass of the material, used to calculate fossil climate change impacts based on the disposal factor per unit mass.", typeof(Mass))]
        [Input("quantityValue", "Quantity value used for biogenic carbon cancellation calculations when applicable (typically the same value used for cradle-to-gate calculations).")]
        [Input("metrics", "List of environmental metrics from the EPD, used to extract biogenic carbon values for cancellation calculations and to check for existing disposal values.")]
        [Output("results", "Dictionary mapping MetricType to calculated disposal impact values for ClimateChangeFossil, ClimateChangeBiogenic, ClimateChangeLandUse, ClimateChangeTotal, and ClimateChangeTotalNoBiogenic. Returns null if disposal factors are invalid or empty dictionary if no computation is needed.")]
        public static Dictionary<MetricType, double> WasteAndDisposalResults(this WasteAndDisposalFactors disposalFactors, double mass, double quantityValue, List<IEnvironmentalMetric> metrics)
        {
            if(disposalFactors == null || disposalFactors.FossilWasteFactor == null)
                return null;

            Dictionary<MetricType, double> results = new Dictionary<MetricType, double>();

            HashSet<Module> disposalModules = new HashSet<Module> { Module.C3, Module.C4, Module.C3toC4 };
            HashSet<MetricType> applicableTypes = new HashSet<MetricType>
            {
                MetricType.ClimateChangeFossil,
                MetricType.ClimateChangeBiogenic,
                MetricType.ClimateChangeLandUse,
                MetricType.ClimateChangeTotal,
                MetricType.ClimateChangeTotalNoBiogenic
            };

            //Check if the values should be overriden or not.
            //If present even in one of the submetrics and with override set to false, then do not override.
            //This is to avoid getting incorrect sum values for the two total metrics (Total and TotalNoBiogenic)
            //when some of the submetrics have disposal values assigned in the EPD and some do
            bool computeValues = disposalFactors.OverrideEpdValue || !metrics.Where(x => applicableTypes.Any(t => t == x.IMetricType())).Any(x => disposalModules.Any(y => x.Indicators.ContainsKey(y)));

            // If no need to compute values, return empty results
            if (!computeValues)
            {
                BH.Engine.Base.Compute.RecordNote("Some of the disposal values are already present in the EPD and override is set to false. WasteAndDisposalResults will not compute any values based on WasteAndDisposalFactors.");
                return results;
            }

            //Fossil Climate Change - Use C3toC4 factor as it includes both transport to disposal and disposal impacts
            double fossilFactor = disposalFactors.FossilWasteFactor.C3toC4 * mass;
            results[MetricType.ClimateChangeFossil] = fossilFactor;

            //Land use - 
            double landUseFactor = 0;
            results[MetricType.ClimateChangeLandUse] = landUseFactor;

            //Biogenic
            double biogenicFactor = 0;
            IEnvironmentalMetric biogenic = metrics.Where(x => x.IMetricType() == MetricType.ClimateChangeBiogenic).FirstOrDefault();
            if (biogenic != null && disposalFactors.CancelOutBiogenicCarbon)
            {
                //If biogenic metric is present, then first check if it has a A1 value (raw material extraction), if not then check if it has A1-A3 value (cradle to gate)
                if (biogenic.Indicators.TryGetValue(Module.A1, out double a1Biogenic))
                    biogenicFactor = - a1Biogenic * quantityValue;
                else if (biogenic.Indicators.TryGetValue(Module.A1toA3, out double a1a3Biogenic))
                    biogenicFactor = - a1a3Biogenic * quantityValue;
            }

            results[MetricType.ClimateChangeBiogenic] = biogenicFactor;

            //Total
            results[MetricType.ClimateChangeTotal] = fossilFactor + landUseFactor + biogenicFactor;
            results[MetricType.ClimateChangeTotalNoBiogenic] = fossilFactor + landUseFactor;
            return results;
        }

        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

    }
}



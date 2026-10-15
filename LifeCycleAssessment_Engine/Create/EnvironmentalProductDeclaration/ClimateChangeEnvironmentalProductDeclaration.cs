/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2026, the respective contributors. All rights reserved.
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
using BH.oM.Quantities.Attributes;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace BH.Engine.LifeCycleAssessment
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Creates an EnvironmentalProductDeclaration containing the climate change metrics, given the fossil, biogenic and land use and land use change (LULUC) factors per module. " +
                     "The ClimateChangeTotal (fossil + biogenic + LULUC) and ClimateChangeTotalNoBiogenic (fossil + LULUC) metrics are computed per module from the provided factors for each module.")]
        [Input("modules", "The modules the provided factors correspond to. The factor lists need to be of the same length as this list, with the factor at each index corresponding to the module at the same index.")]
        [Input("fossil", "Climate change fossil factors per module, in the same order as the provided modules. Values given should be per quantity set by the quantityType.", typeof(ClimateChangePerQuantity))]
        [Input("biogenic", "Climate change biogenic factors per module, in the same order as the provided modules. Values given should be per quantity set by the quantityType.", typeof(ClimateChangePerQuantity))]
        [Input("luluc", "Climate change land use and land use change (LULUC) factors per module, in the same order as the provided modules. Values given should be per quantity set by the quantityType.", typeof(ClimateChangePerQuantity))]
        [Input("quantityType", "The quantity type the factors are given per, for example Mass for factors given per kg.")]
        [Input("epdType", "The type of the EnvironmentalProductDeclaration.")]
        [Input("name", "Name of the created EnvironmentalProductDeclaration.")]
        [Output("epd", "The created EnvironmentalProductDeclaration containing the fossil, biogenic, LULUC, total and total without biogenic climate change metrics.")]
        public static EnvironmentalProductDeclaration ClimateChangeEnvironmentalProductDeclaration(List<Module> modules, List<double> fossil, List<double> biogenic, List<double> luluc, QuantityType quantityType, EPDType epdType = EPDType.Undefined, string name = "")
        {
            if (modules == null || fossil == null || biogenic == null || luluc == null)
            {
                Base.Compute.RecordError("Cannot create a climate change EnvironmentalProductDeclaration from null modules or factors.");
                return null;
            }

            if (fossil.Count != modules.Count || biogenic.Count != modules.Count || luluc.Count != modules.Count)
            {
                Base.Compute.RecordError($"The number of fossil ({fossil.Count}), biogenic ({biogenic.Count}) and LULUC ({luluc.Count}) factors need to match the number of modules ({modules.Count}).");
                return null;
            }

            if (modules.Distinct().Count() != modules.Count)
            {
                Base.Compute.RecordError("The provided modules contain duplicates. Each module can only be provided once.");
                return null;
            }

            if (fossil.Concat(biogenic).Concat(luluc).Any(double.IsNaN))
            {
                Base.Compute.RecordError("The fossil, biogenic and LULUC factors need to be provided for all modules. NaN values are not allowed.");
                return null;
            }

            Dictionary<Module, double> fossilFactors = new Dictionary<Module, double>();
            Dictionary<Module, double> biogenicFactors = new Dictionary<Module, double>();
            Dictionary<Module, double> lulucFactors = new Dictionary<Module, double>();
            Dictionary<Module, double> totalFactors = new Dictionary<Module, double>();
            Dictionary<Module, double> totalNoBiogenicFactors = new Dictionary<Module, double>();

            for (int i = 0; i < modules.Count; i++)
            {
                Module module = modules[i];
                fossilFactors[module] = fossil[i];
                biogenicFactors[module] = biogenic[i];
                lulucFactors[module] = luluc[i];
                totalFactors[module] = fossil[i] + biogenic[i] + luluc[i];
                totalNoBiogenicFactors[module] = fossil[i] + luluc[i];
            }

            EnvironmentalProductDeclaration epd = new EnvironmentalProductDeclaration()
            {
                Name = name,
                Type = epdType,
                QuantityType = quantityType,
                EnvironmentalMetrics = new List<IEnvironmentalMetric>()
                {
                    new ClimateChangeFossilMetric() { Indicators = fossilFactors },
                    new ClimateChangeBiogenicMetric() { Indicators = biogenicFactors },
                    new ClimateChangeLandUseMetric() { Indicators = lulucFactors },
                    new ClimateChangeTotalMetric() { Indicators = totalFactors },
                    new ClimateChangeTotalNoBiogenicMetric() { Indicators = totalNoBiogenicFactors }
                }
            };

            return epd;
        }

        /***************************************************/
    }
}

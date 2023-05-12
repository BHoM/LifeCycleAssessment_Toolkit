/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2023, the respective contributors. All rights reserved.
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

using BH.oM.Base;
using BH.oM.Base.Attributes;
using BH.oM.LifeCycleAssessment;
using BH.oM.LifeCycleAssessment.Results;
using System;
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

        [Description("Gets the total sum of values from all phases with a set value (all values not NaN).")]
        [Input("phaseData", "LCA phase data object to get the total from.")]
        [Output("total", "The sum of all non-NaN properties on the provided phase data object.")]
        public static double Total(this ILifeCycleAssessmentPhaseData phaseData)
        {
            List<string> included = new List<string>();
            double total = ATotal(phaseData, included);
            total += BTotal(phaseData, included);
            total += CTotal(phaseData, included);
            if (!double.IsNaN(phaseData.D))
            {
                total += phaseData.D;
                included.Add(nameof(phaseData.D));
            }
            total += IAdditional(phaseData, included);
            IncludedMessage(included, nameof(Total));
            return total;
        }

        /***************************************************/

        [Description("Gets the total sum of values from all A-phases (A1-A5) with a set value (all values not NaN).")]
        [Input("phaseData", "LCA phase data object to get the total from.")]
        [Output("Atotal", "The sum of all non-NaN properties on the provided phase data object.")]
        public static double ATotal(this ILifeCycleAssessmentPhaseData phaseData)
        {
            List<string> included = new List<string>();
            double total = ATotal(phaseData, included);
            IncludedMessage(included, nameof(ATotal));
            return total;
        }

        /***************************************************/

        [Description("Gets the total sum of values from all B-phases (B1-B7) with a set value (all values not NaN).")]
        [Input("phaseData", "LCA phase data object to get the total from.")]
        [Output("BTotal", "The sum of all non-NaN properties on the provided phase data object.")]
        public static double BTotal(this ILifeCycleAssessmentPhaseData phaseData)
        {
            List<string> included = new List<string>();
            double total = BTotal(phaseData, included);
            IncludedMessage(included, nameof(BTotal));
            return total;
        }

        /***************************************************/

        [Description("Gets the total sum of values from all C-phases (C1-C4) with a set value (all values not NaN).")]
        [Input("phaseData", "LCA phase data object to get the total from.")]
        [Output("CTotal", "The sum of all non-NaN properties on the provided phase data object.")]
        public static double CTotal(this ILifeCycleAssessmentPhaseData phaseData)
        {
            List<string> included = new List<string>();
            double total = CTotal(phaseData, included);
            IncludedMessage(included, nameof(CTotal));
            return total;
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        [Description("Additional data point to be inlcuded.")]
        private static double IAdditional(this ILifeCycleAssessmentPhaseData phaseData, List<string> included)
        {
            return Additional(phaseData as dynamic, included);
        }

        /***************************************************/

        [Description("Additional data point for general case (nothing).")]
        private static double Additional(this ILifeCycleAssessmentPhaseData phaseData, List<string> included)
        {
            return 0;
        }

        /***************************************************/

        [Description("Additional data point for GWP.")]
        private static double Additional(this IGlobalWarmingPotentialPhaseData phaseData, List<string> included)
        {
            if (!double.IsNaN(phaseData.BiogenicCarbon))
            {
                included.Add(nameof(phaseData.BiogenicCarbon));
                return phaseData.BiogenicCarbon;
            }
            return 0;
        }

        /***************************************************/

        [Description("Raises a message corresponding to which phases that were included when computing the total.")]
        private static void IncludedMessage(List<string> included, string totalType)
        {
            if(included.Count == 0)
                Base.Compute.RecordWarning($"No phases contains any values for when evaluating {totalType}.");

            else
                Base.Compute.RecordNote($"Phases {string.Join(", ", included)} where included when evaluating {totalType}.");
        }

        /***************************************************/

        [Description("Gets out all A-phases, and stores the non-NaN properties in the included list.")]
        private static double ATotal(this ILifeCycleAssessmentPhaseData phaseData, List<string> included)
        {
            double total = 0;

            if (double.IsNaN(phaseData.A1toA3))
            {
                if (!double.IsNaN(phaseData.A1))
                {
                    total += phaseData.A1;
                    included.Add(nameof(phaseData.A1));
                }

                if (!double.IsNaN(phaseData.A2))
                {
                    total += phaseData.A2;
                    included.Add(nameof(phaseData.A2));
                }

                if (!double.IsNaN(phaseData.A3))
                {
                    total += phaseData.A3;
                    included.Add(nameof(phaseData.A3));
                }
            }
            else
            {
                if (!double.IsNaN(phaseData.A1) && !double.IsNaN(phaseData.A2) && !double.IsNaN(phaseData.A3))
                {
                    total += phaseData.A1 + phaseData.A2 + phaseData.A3;
                    included.Add(nameof(phaseData.A1));
                    included.Add(nameof(phaseData.A2));
                    included.Add(nameof(phaseData.A3));
                }
                else
                {
                    total += phaseData.A1toA3;
                    included.Add(nameof(phaseData.A1toA3));
                }
            }

            if (!double.IsNaN(phaseData.A4))
            {
                total += phaseData.A4;
                included.Add(nameof(phaseData.A4));
            }

            if (!double.IsNaN(phaseData.A5))
            {
                total += phaseData.A5;
                included.Add(nameof(phaseData.A5));
            }

            return total;
        }

        /***************************************************/

        [Description("Gets out all B-phases, and stores the non-NaN properties in the included list.")]
        private static double BTotal(this ILifeCycleAssessmentPhaseData phaseData, List<string> included)
        {
            double total = 0;


            if (!double.IsNaN(phaseData.B1))
            {
                total += phaseData.B1;
                included.Add(nameof(phaseData.B1));
            }

            if (!double.IsNaN(phaseData.B2))
            {
                total += phaseData.B2;
                included.Add(nameof(phaseData.B2));
            }

            if (!double.IsNaN(phaseData.B3))
            {
                total += phaseData.B3;
                included.Add(nameof(phaseData.B3));
            }

            if (!double.IsNaN(phaseData.B4))
            {
                total += phaseData.B4;
                included.Add(nameof(phaseData.B4));
            }

            if (!double.IsNaN(phaseData.B5))
            {
                total += phaseData.B5;
                included.Add(nameof(phaseData.B5));
            }

            if (!double.IsNaN(phaseData.B6))
            {
                total += phaseData.B6;
                included.Add(nameof(phaseData.B6));
            }

            if (!double.IsNaN(phaseData.B7))
            {
                total += phaseData.B7;
                included.Add(nameof(phaseData.B7));
            }

            return total;
        }

        /***************************************************/

        [Description("Gets out all C-phases, and stores the non-NaN properties in the included list.")]
        private static double CTotal(this ILifeCycleAssessmentPhaseData phaseData, List<string> included)
        {
            double total = 0;


            if (!double.IsNaN(phaseData.C1))
            {
                total += phaseData.C1;
                included.Add(nameof(phaseData.C1));
            }

            if (!double.IsNaN(phaseData.C2))
            {
                total += phaseData.C2;
                included.Add(nameof(phaseData.C2));
            }

            if (!double.IsNaN(phaseData.C3))
            {
                total += phaseData.C3;
                included.Add(nameof(phaseData.C3));
            }

            if (!double.IsNaN(phaseData.C4))
            {
                total += phaseData.C4;
                included.Add(nameof(phaseData.C4));
            }

            return total;
        }

        /***************************************************/
    }
}

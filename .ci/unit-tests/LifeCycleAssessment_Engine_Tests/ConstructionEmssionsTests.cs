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

using BH.Engine.LifeCycleAssessment;
using BH.oM.LifeCycleAssessment;
using BH.oM.LifeCycleAssessment.MaterialFragments.Construction;
using BH.oM.Test.NUnit;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Tests.Engine.LifeCycleAssessment
{
    public class ConstructionEmssionsTests : NUnitTest
    {

        [TestCaseSource(nameof(AddConstructionWasteEmissionsTestCases))]
        [Description("Tests the AddConstructionWasteEmissions extension method by validating that construction waste emissions (A5_3 module) are correctly calculated based on waste rates and reuse scenarios. Tests both normal cases and edge cases where required modules are missing.")]
        public void TestAddConstructionWasteEmissions(Dictionary<Module, double> resultingValues, ConstructionWasteEmissions constructionEmissions, double assumedA5_3Value)
        {
            BH.Engine.Base.Compute.ClearCurrentEvents(); //Make sure events raised by previous tests can not satisfy the assertions below
            resultingValues.AddConstructionWasteEmissions(constructionEmissions, MetricType.ClimateChangeTotal);
            if(assumedA5_3Value == 0)
            {
                Assert.That(!resultingValues.ContainsKey(Module.A5_3));
                Assert.That(BH.Engine.Base.Query.CurrentEvents().Any(e => e.Type == oM.Base.Debugging.EventType.Error && e.Message.Contains("Missing modules for waste computation")), Is.True);
                return;
            }
            Assert.That(resultingValues.ContainsKey(Module.A5_3));
            Assert.That(resultingValues[Module.A5_3], Is.EqualTo(assumedA5_3Value).Within(1e-9));
        }

        private static IEnumerable<TestCaseData> AddConstructionWasteEmissionsTestCases()
        {
            // Normal cases
            yield return Case("A1toA3 with all required modules, not reused on site, C2 included",
                new Dictionary<Module, double> { { Module.A1toA3, 100 }, { Module.A4, 20 }, { Module.C2, 10 }, { Module.C3, 10 } },
                new ConstructionWasteEmissions { WasteRate = new WasteRate { Rate = 0.5 }, ReusedOnSite = false }, 140);

            yield return Case("A1toA3 with all required modules, reused on site, C2 excluded",
                new Dictionary<Module, double> { { Module.A1toA3, 100 }, { Module.A4, 20 }, { Module.C2, 10 }, { Module.C3, 10 } },
                new ConstructionWasteEmissions { WasteRate = new WasteRate { Rate = 0.5 }, ReusedOnSite = true }, 130);

            yield return Case("Separate A1, A2 and A3, not reused on site, C2 included",
                new Dictionary<Module, double> { { Module.A1, 40 }, { Module.A2, 30 }, { Module.A3, 30 }, { Module.A4, 20 }, { Module.C2, 10 }, { Module.C3, 10 } },
                new ConstructionWasteEmissions { WasteRate = new WasteRate { Rate = 0.5 }, ReusedOnSite = false }, 140);

            yield return Case("Separate A1, A2 and A3, reused on site, C2 excluded",
                new Dictionary<Module, double> { { Module.A1, 40 }, { Module.A2, 30 }, { Module.A3, 30 }, { Module.A4, 20 }, { Module.C2, 10 }, { Module.C3, 10 } },
                new ConstructionWasteEmissions { WasteRate = new WasteRate { Rate = 0.5 }, ReusedOnSite = true }, 130);

            yield return Case("Waste rate of 0.1 gives waste factor of 1/9 of the summed modules",
                new Dictionary<Module, double> { { Module.A1, 100 }, { Module.A2, 20 }, { Module.A3, 30 }, { Module.A4, 20 }, { Module.C2, 10 }, { Module.C3, 30 } },
                new ConstructionWasteEmissions { WasteRate = new WasteRate { Rate = 0.1 }, ReusedOnSite = false }, 23.3333333333333);

            yield return Case("C4 used as disposal module in place of C3",
                new Dictionary<Module, double> { { Module.A1, 40 }, { Module.A2, 30 }, { Module.A3, 30 }, { Module.A4, 20 }, { Module.C2, 10 }, { Module.C4, 10 } },
                new ConstructionWasteEmissions { WasteRate = new WasteRate { Rate = 0.5 }, ReusedOnSite = true }, 130);

            yield return Case("C3toC4 used as disposal module in place of C3",
                new Dictionary<Module, double> { { Module.A1, 40 }, { Module.A2, 30 }, { Module.A3, 30 }, { Module.A4, 20 }, { Module.C2, 10 }, { Module.C3toC4, 10 } },
                new ConstructionWasteEmissions { WasteRate = new WasteRate { Rate = 0.5 }, ReusedOnSite = true }, 130);

            yield return Case("C2 not required when reused on site",
                new Dictionary<Module, double> { { Module.A1toA3, 100 }, { Module.A4, 20 }, { Module.C3, 10 } },
                new ConstructionWasteEmissions { WasteRate = new WasteRate { Rate = 0.5 }, ReusedOnSite = true }, 130);

            // Missing modules cases
            yield return Case("Missing A1toA3 gives error and no A5_3",
                new Dictionary<Module, double> { { Module.A4, 20 }, { Module.C2, 10 }, { Module.C3, 10 } },
                new ConstructionWasteEmissions { WasteRate = new WasteRate { Rate = 0.5 }, ReusedOnSite = false }, 0);

            yield return Case("Missing A4 gives error and no A5_3",
                new Dictionary<Module, double> { { Module.A1toA3, 100 }, { Module.C2, 10 }, { Module.C3, 10 } },
                new ConstructionWasteEmissions { WasteRate = new WasteRate { Rate = 0.5 }, ReusedOnSite = false }, 0);

            yield return Case("Missing C2 when not reused on site gives error and no A5_3",
                new Dictionary<Module, double> { { Module.A1toA3, 100 }, { Module.A4, 20 }, { Module.C3, 10 } },
                new ConstructionWasteEmissions { WasteRate = new WasteRate { Rate = 0.5 }, ReusedOnSite = false }, 0);

            yield return Case("Missing disposal (C3, C4 or C3toC4) gives error and no A5_3",
                new Dictionary<Module, double> { { Module.A1toA3, 100 }, { Module.A4, 20 }, { Module.C2, 10 } },
                new ConstructionWasteEmissions { WasteRate = new WasteRate { Rate = 0.5 }, ReusedOnSite = false }, 0);

            yield return Case("Missing A1 when A2 and A3 are provided gives error and no A5_3",
                new Dictionary<Module, double> { { Module.A2, 20 }, { Module.A3, 30 }, { Module.A4, 20 }, { Module.C2, 10 }, { Module.C3, 30 } },
                new ConstructionWasteEmissions { WasteRate = new WasteRate { Rate = 0.1 }, ReusedOnSite = false }, 0);

            yield return Case("Missing A2 when A1 and A3 are provided gives error and no A5_3",
                new Dictionary<Module, double> { { Module.A1, 100 }, { Module.A3, 30 }, { Module.A4, 20 }, { Module.C2, 10 }, { Module.C3, 30 } },
                new ConstructionWasteEmissions { WasteRate = new WasteRate { Rate = 0.1 }, ReusedOnSite = false }, 0);

            yield return Case("Missing A3 when A1 and A2 are provided gives error and no A5_3",
                new Dictionary<Module, double> { { Module.A1, 100 }, { Module.A2, 20 }, { Module.A4, 20 }, { Module.C2, 10 }, { Module.C3, 30 } },
                new ConstructionWasteEmissions { WasteRate = new WasteRate { Rate = 0.1 }, ReusedOnSite = false }, 0);
        }

        /***************************************************/

        private static TestCaseData Case(string name, Dictionary<Module, double> resultingValues, ConstructionWasteEmissions constructionEmissions, double assumedA5_3Value)
        {
            return new TestCaseData(resultingValues, constructionEmissions, assumedA5_3Value).SetArgDisplayNames(name);
        }
    }
}


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

using BH.oM.LifeCycleAssessment;
using BH.oM.LifeCycleAssessment.MaterialFragments.Construction;
using BH.Engine.LifeCycleAssessment;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Tests.Engine.LifeCycleAssessment
{
    public class ConstructionEmssionsTests
    {

        [TestCaseSource(nameof(AddConstructionWasteEmissionsTestCases))]
        [Description("Tests the AddConstructionWasteEmissions extension method by validating that construction waste emissions (A5_3 module) are correctly calculated based on waste rates and reuse scenarios. Tests both normal cases and edge cases where required modules are missing.")]
        public void TestAddConstructionWasteEmissions(Dictionary<Module, double> resultingValues, ConstructionWasteEmissions constructionEmissions, double assumedA5_3Value)
        { 
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

        private static IEnumerable<object[]> AddConstructionWasteEmissionsTestCases()
        {
            // Normal cases
            yield return new object[] { new Dictionary<Module, double> { { Module.A1toA3, 100 }, { Module.A4, 20 }, { Module.C2, 10 }, { Module.C3, 10 } }, new ConstructionWasteEmissions { WasteRate = new WasteRate { Rate = 0.5 }, ResuedOnSite = false }, 140  };
            yield return new object[] { new Dictionary<Module, double> { { Module.A1toA3, 100 }, { Module.A4, 20 }, { Module.C2, 10 }, { Module.C3, 10 } }, new ConstructionWasteEmissions { WasteRate = new WasteRate { Rate = 0.5 }, ResuedOnSite = true }, 130 };
            yield return new object[] { new Dictionary<Module, double> { { Module.A1, 40 }, { Module.A2, 30 }, { Module.A3, 30 }, { Module.A4, 20 }, { Module.C2, 10 }, { Module.C3, 10 } }, new ConstructionWasteEmissions { WasteRate = new WasteRate { Rate = 0.5 }, ResuedOnSite = false }, 140 };
            yield return new object[] { new Dictionary<Module, double> { { Module.A1, 40 }, { Module.A2, 30 }, { Module.A3, 30 }, { Module.A4, 20 }, { Module.C2, 10 }, { Module.C3, 10 } }, new ConstructionWasteEmissions { WasteRate = new WasteRate { Rate = 0.5 }, ResuedOnSite = true }, 130 };
            yield return new object[] { new Dictionary<Module, double> { { Module.A1, 100 }, { Module.A2, 20 }, { Module.A3, 30 }, { Module.A4, 20 }, { Module.C2, 10 }, { Module.C3, 30 } }, new ConstructionWasteEmissions { WasteRate = new WasteRate { Rate = 0.1 }, ResuedOnSite = false }, 23.3333333333333 };
            yield return new object[] { new Dictionary<Module, double> { { Module.A1, 40 }, { Module.A2, 30 }, { Module.A3, 30 }, { Module.A4, 20 }, { Module.C2, 10 }, { Module.C4, 10 } }, new ConstructionWasteEmissions { WasteRate = new WasteRate { Rate = 0.5 }, ResuedOnSite = true }, 130 };
            yield return new object[] { new Dictionary<Module, double> { { Module.A1, 40 }, { Module.A2, 30 }, { Module.A3, 30 }, { Module.A4, 20 }, { Module.C2, 10 }, { Module.C3toC4, 10 } }, new ConstructionWasteEmissions { WasteRate = new WasteRate { Rate = 0.5 }, ResuedOnSite = true }, 130 };
            yield return new object[] { new Dictionary<Module, double> { { Module.A1toA3, 100 }, { Module.A4, 20 }, { Module.C3, 10 } }, new ConstructionWasteEmissions { WasteRate = new WasteRate { Rate = 0.5 }, ResuedOnSite = true }, 130 };

            // Missing modules cases
            yield return new object[] { new Dictionary<Module, double> { { Module.A4, 20 }, { Module.C2, 10 }, { Module.C3, 10 } }, new ConstructionWasteEmissions { WasteRate = new WasteRate { Rate = 0.5 }, ResuedOnSite = false }, 0 };
            yield return new object[] { new Dictionary<Module, double> { { Module.A1toA3, 100 }, { Module.C2, 10 }, { Module.C3, 10 } }, new ConstructionWasteEmissions { WasteRate = new WasteRate { Rate = 0.5 }, ResuedOnSite = false }, 0 };
            yield return new object[] { new Dictionary<Module, double> { { Module.A1toA3, 100 }, { Module.A4, 20 }, { Module.C3, 10 } }, new ConstructionWasteEmissions { WasteRate = new WasteRate { Rate = 0.5 }, ResuedOnSite = false }, 0 };
            yield return new object[] { new Dictionary<Module, double> { { Module.A1toA3, 100 }, { Module.A4, 20 }, { Module.C2, 10 } }, new ConstructionWasteEmissions { WasteRate = new WasteRate { Rate = 0.5 }, ResuedOnSite = false }, 0 };
            yield return new object[] { new Dictionary<Module, double> { { Module.A2, 20 }, { Module.A3, 30 }, { Module.A4, 20 }, { Module.C2, 10 }, { Module.C3, 30 } }, new ConstructionWasteEmissions { WasteRate = new WasteRate { Rate = 0.1 }, ResuedOnSite = false }, 0 };
            yield return new object[] { new Dictionary<Module, double> { { Module.A1, 100 }, { Module.A3, 30 }, { Module.A4, 20 }, { Module.C2, 10 }, { Module.C3, 30 } }, new ConstructionWasteEmissions { WasteRate = new WasteRate { Rate = 0.1 }, ResuedOnSite = false }, 0 };
            yield return new object[] { new Dictionary<Module, double> { { Module.A1, 100 }, { Module.A2, 20 },  { Module.A4, 20 }, { Module.C2, 10 }, { Module.C3, 30 } }, new ConstructionWasteEmissions { WasteRate = new WasteRate { Rate = 0.1 }, ResuedOnSite = false }, 0 };

        }
    }
}

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

using AutoBogus;
using BH.Engine.Base;
using BH.Engine.LifeCycleAssessment;
using BH.oM.LifeCycleAssessment;
using BH.oM.LifeCycleAssessment.Fragments;
using BH.oM.LifeCycleAssessment.MaterialFragments;
using BH.oM.LifeCycleAssessment.Results;
using BH.oM.Physical.Materials;
using BH.oM.Test.NUnit;
using FluentAssertions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BH.Tests.Engine.LifeCycleAssessment
{
    public class QuantityValueTests : NUnitTest
    {
        /***************************************************/
        /****   Public Methods                          ****/
        /***************************************************/

        [TestCaseSource(nameof(DummyQuantityValues))]
        public void QuantityValue(TakeoffItem item, QuantityType quantityType, double expected)
        {
            Assert.That(item.QuantityValue(quantityType), Is.EqualTo(expected));
        }

        /***************************************************/

        private static IEnumerable<object[]> DummyQuantityValues()
        { 
            Material mat = new Material();

            yield return new object[] { new TakeoffItem { Material = mat, Area = 2 }, QuantityType.Area, 2 };
            yield return new object[] { new TakeoffItem { Material = mat, ElectricCurrent = 2 }, QuantityType.Ampere, 2 };
            yield return new object[] { new TakeoffItem { Material = mat, Energy = 2 }, QuantityType.Energy, 2 };
            yield return new object[] { new TakeoffItem { Material = mat, NumberItem = 2 }, QuantityType.Item, 2 };
            yield return new object[] { new TakeoffItem { Material = mat, Length = 2 }, QuantityType.Length, 2 };
            yield return new object[] { new TakeoffItem { Material = mat, Mass = 2 }, QuantityType.Mass, 2 };
            yield return new object[] { new TakeoffItem { Material = mat, Area = 2 }, QuantityType.Undefined, double.NaN };
            yield return new object[] { new TakeoffItem { Material = mat, Power = 2 }, QuantityType.VoltAmps, 2 };
            yield return new object[] { new TakeoffItem { Material = mat, Volume = 2 }, QuantityType.Volume, 2 };
            yield return new object[] { new TakeoffItem { Material = mat, VolumetricFlowRate = 2 }, QuantityType.VolumetricFlowRate, 2 };
            yield return new object[] { new TakeoffItem { Material = mat, Power = 2 }, QuantityType.Watt, 2 };

            yield return new object[] { new TakeoffItem { Material = new Material { Density = 2 }, Volume = 2 }, QuantityType.Mass, 4 };
            yield return new object[] { new TakeoffItem { Material = new Material { Density = 2 }, Volume = 0 }, QuantityType.Mass, 0 };
            yield return new object[] { new TakeoffItem { Material = new Material { }, Volume = 0 }, QuantityType.Mass, 0 };

            Material matWithEpdDensity = new Material();
            EnvironmentalProductDeclaration epd = new EnvironmentalProductDeclaration();
            epd.Fragments.Add(new EPDDensity { Density = 2 });
            matWithEpdDensity.Properties.Add(epd);
            yield return new object[] { new TakeoffItem { Material = matWithEpdDensity, Volume = 2 }, QuantityType.Mass, 4 };



        }

        /***************************************************/
    }
}



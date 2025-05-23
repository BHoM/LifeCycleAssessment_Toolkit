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

using BH.oM.Structure.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.Engine.Serialiser;
using NUnit.Framework;
using System.Diagnostics;
using Bogus.Bson;
using BH.oM.Base;
using BH.oM.Graphics;
using FluentAssertions;
using BH.oM.LifeCycleAssessment.MaterialFragments;
using BH.oM.Versioning;

namespace BH.Tests.Engine.LifeCycleAssessment
{
    public class Datasets
    {
        [TestCaseSource(nameof(DatasetFilePaths))]
        public void DatasetsAllDeserialiseing(string f)
        {
            BH.Engine.Base.Compute.ClearCurrentEvents();
            string json = System.IO.File.ReadAllText(f);
            object back = BH.Engine.Serialiser.Convert.FromJson(json);
            BH.oM.Data.Library.Dataset dataset = back as BH.oM.Data.Library.Dataset;

            Assert.That(dataset, Is.Not.Null);
            Assert.That(dataset.Data, Is.Not.Null);
            Assert.That(dataset.Data, Has.All.Not.Null);
            Assert.That(dataset.Data, Has.All.Not.TypeOf<CustomObject>());

            List<VersioningEvent> versioningEvents = BH.Engine.Base.Query.CurrentEvents().OfType<VersioningEvent>().ToList();

            Warn.Unless(versioningEvents, Is.Empty, "Verisoning required");

        }

        private static IEnumerable<string> DatasetFilePaths()
        {
            EnvironmentalProductDeclaration epd = new EnvironmentalProductDeclaration();
            string folder = @"C:\ProgramData\BHoM\Datasets\LifeCycleAssessment\";

            return Directory.GetFiles(folder, "*.json", SearchOption.AllDirectories);
        }
    }
}




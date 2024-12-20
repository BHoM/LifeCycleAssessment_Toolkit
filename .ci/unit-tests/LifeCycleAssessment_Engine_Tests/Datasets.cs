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

namespace BH.Tests.Engine.LifeCycleAssessment
{
    public class Datasets
    {
        [Test]
        public void DatasetsAllDeserialiseing()
        {
            string folder = @"C:\ProgramData\BHoM\Datasets\LifeCycleAssessment\";

            string[] files = Directory.GetFiles(folder, "*.json", SearchOption.AllDirectories);

            List<string> failures = new List<string>();
            List<string> successes = new List<string>();

            EnvironmentalProductDeclaration epd = new EnvironmentalProductDeclaration();

            foreach (string f in files)
            {
                try
                {
                    string json = System.IO.File.ReadAllText(f);
                    object back = BH.Engine.Serialiser.Convert.FromJson(json);
                    BH.oM.Data.Library.Dataset dataset = back as BH.oM.Data.Library.Dataset;
                    if (dataset == null || dataset.Data.Any(x => x == null || x is CustomObject))
                        failures.Add(f);
                    else
                        successes.Add(f);
                }
                catch (Exception)
                {
                    failures.Add(f);
                }
            }
            Console.WriteLine("Fail upgrade:");
            foreach (string f in failures)
            {
                Console.WriteLine(f);
            }

            Console.WriteLine("");
            Console.WriteLine("Success upgrade:");
            foreach (string s in successes)
            {
                Console.WriteLine(s);
            }

            failures.Should().BeEmpty();
        }
    }
}




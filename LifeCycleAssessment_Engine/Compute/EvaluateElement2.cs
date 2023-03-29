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
using BH.oM.Dimensional;
using BH.oM.LifeCycleAssessment.Results;
using BH.oM.LifeCycleAssessment;
using BH.oM.Physical.Materials;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using BH.Engine.Matter;
using BH.oM.LifeCycleAssessment.MaterialFragments;
using BH.Engine.Spatial;
using System.Collections.Concurrent;
using System.Reflection;
using BH.Engine.Base;

namespace BH.Engine.LifeCycleAssessment
{
    public static partial class Compute
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Evaluates the environmental metrics found within a given EPD. \n" +
                   "This method is only fit for the evaluation of phases A1-A3 (cradle to gate) at present. \n" +
                   "To view a list of all possible metric evaluations, please view the EPDField enum. Note that not all fields can be evaluated. \n" +
                   "The provided Template Materials allow each material within your object to have an associated Environmental Product Declaration.")]
        [Input("elementM", "This is a BHoM object used to calculate EPD metric. This obj must have an EPD MaterialFragment applied to the object.")]
        [Input("phases", "Provide phases of life you wish to evaluate. Phases of life must be documented within EPDs for this method to work.\n" +
                   "Note that only phases A1-A3 combined are possible evaluations at present.")]
        [Input("field", "Filter the provided EnvironmentalProductDeclaration by selecting one of the provided metrics for calculation. This method also accepts multiple fields simultaneously.")]
        [Input("exactMatch", "If true, the evaluation method will force an exact LCA phase match to solve for.")]
        [Input("templateMaterials", "Template materials to match to and assign properties from onto the model materials. Should generally have unique names. EPDs should be assigned to these materials and will be mapped over to the materials on the element with the same name and used in the evaluation.")]
        [Input("prioritiseTemplate", "Controls if main material or map material should be prioritised when conflicting information is found on both in terms of Density and/or Properties. If true, map is prioritised, if false, main material is prioritised.")]
        [Output("result", "An ElementResult that contains the LifeCycleAssessment data for the input object(s).")]
        public static List<IElementResult<MaterialResult2>> EvaluateElement(IElementM elementM, List<Material> templateMaterials = null, bool prioritiseTemplate = true)
        {
            if (elementM == null)
            {
                return null;
            }

            VolumetricMaterialTakeoff takeoff = elementM.MappedVolumetricMaterialTakeoff(templateMaterials, true, prioritiseTemplate);

            if (takeoff == null)
            {
                return null;
            }

            if (takeoff.Materials.Count == 0)
                return null;

            //Predefining parameters to be used for some item types in loop
            double area = double.NaN;
            double length = double.NaN;

            List<MaterialResult2> materialResults = new List<MaterialResult2>();

            for (int i = 0; i < takeoff.Materials.Count; i++)
            {
                Material material = takeoff.Materials[i];
                EnvironmentalProductDeclaration2 epd = material.Properties.OfType<EnvironmentalProductDeclaration2>().FirstOrDefault();

                if (epd == null)
                {
                    Base.Compute.RecordError($"EPD not set to material {material.Name}. Unable to evaluate element.");
                    return null;
                }

                double quantityValue;
                switch (epd.QuantityType)
                {
                    case QuantityType.Volume:
                        quantityValue = takeoff.Volumes[i];
                        break;
                    case QuantityType.Mass:
                        if (double.IsNaN(material.Density))
                        {
                            Base.Compute.RecordError($"Density is not set for material {material.Name}. Cannot evaluate mass based EPD.");
                            return null;
                        }
                        if (material.Density == 0)
                        {
                            Base.Compute.RecordWarning($"Density of materials {material.Name} is 0 and will give no contribution for evaluating mass based EPD.");
                        }
                        quantityValue = takeoff.Volumes[i] * material.Density;
                        break;
                    case QuantityType.Area:
                        if (double.IsNaN(area))
                        {
                            IElement2D element2D = elementM as IElement2D;
                            if (element2D == null)
                            {
                                Base.Compute.RecordError($"Can only evaluate Area based epds on elements of a {nameof(IElement2D)} type.");
                                return null;
                            }
                            area = element2D.Area();
                        }
                        quantityValue = area;
                        break;
                    case QuantityType.Length:
                        if (double.IsNaN(length))
                        {
                            IElement1D element1D = elementM as IElement1D;
                            if (element1D == null)
                            {
                                Base.Compute.RecordError($"Can only evaluate Area based epds on elements of a {nameof(IElement1D)} type.");
                                return null;
                            }
                            length = element1D.Length();
                        }
                        quantityValue = length;
                        break;
                    case QuantityType.Undefined:
                    case QuantityType.Item:
                    case QuantityType.Ampere:
                    case QuantityType.VoltAmps:
                    case QuantityType.VolumetricFlowRate:
                    case QuantityType.Watt:
                    case QuantityType.Energy:
                    default:
                        Base.Compute.RecordError($"{epd.QuantityType} QuantityType is currently not supported.");
                        return null;
                }

                materialResults.AddRange(EvaluateEnvironmentalProductDeclaration5(epd, material.Name, quantityValue));
            }

            IComparable objectId = "";
            if (elementM is IBHoMObject bhObj)
                objectId = bhObj.BHoM_Guid;

            return GroupAndSumResults(materialResults, objectId, elementM.GetElementScope(), ObjectCategory.Undefined);
        }

        /***************************************************/

        private static List<IElementResult<MaterialResult2>> GroupAndSumResults(List<MaterialResult2> materialResults, IComparable objectId, ScopeType scope, ObjectCategory category)
        {
            List<IElementResult<MaterialResult2>> elementResults = new List<IElementResult<MaterialResult2>>();
            foreach (var group in materialResults.GroupBy(x => x.GetType()))
            {
                elementResults.Add(SumMaterialResultToElementResult2(group.First() as dynamic, group, objectId, scope, category));
            }
            return elementResults;
        }

        /***************************************************/

        private static IElementResult<MaterialResult2> SumMaterialResultToElementResult2<T>(T item, IEnumerable<MaterialResult2> materialResults, IComparable objectId, ScopeType scope, ObjectCategory category) where T : MaterialResult2
        {
            List<T> castResults = materialResults.Cast<T>().ToList();

            Func<object[], IElementResult<MaterialResult2>> cst = typeof(T).ElementResultConstructor();//GetElementResultConstructor<T>();

            List<object> parameters = new List<object> { objectId, scope, category, new ReadOnlyCollection<T>(castResults) };
            parameters.AddRange(castResults.SumPhaseDataValues2().Cast<object>());

            return cst(parameters.ToArray());
        }

        /***************************************************/

        private static IElementResult<MaterialResult2> SumMaterialResultToElementResult<T>(T item, IEnumerable<MaterialResult2> materialResults, IComparable objectId, ScopeType scope, ObjectCategory category) where T : MaterialResult2
        {
            List<object> parameters = new List<object> { objectId, scope, category, new ReadOnlyCollection<T>(materialResults.Cast<T>().ToList()) };
            Output<List<Func<MaterialResult2, double>>, Func<object[], IElementResult<MaterialResult2>>> constAndParameters = GetElementResultConstructorAndFuncs<T>();
            List<Func<MaterialResult2, double>> metricFuncs = constAndParameters.Item1;
            Func<object[], IElementResult<MaterialResult2>> cst = constAndParameters.Item2;

            foreach (Func<MaterialResult2, double> f in metricFuncs)
            {
                parameters.Add(materialResults.Select(f).Sum());
            }
            return cst(parameters.ToArray());
        }

        /***************************************************/

        private static Func<object[], IElementResult<MaterialResult2>> GetElementResultConstructor<T>() where T : MaterialResult2
        {
            Type t = typeof(T);

            Func<object[], IElementResult<MaterialResult2>> cstFunc;

            if (!m_ElementResultConstructors.TryGetValue(t, out cstFunc))
            {
                ConstructorInfo constructor = GetElementResultConstructorInfo<T>();
                Func<object[], object> genericFunc = constructor.ToFunc();
                cstFunc = x => (IElementResult<MaterialResult2>)genericFunc(x);
                m_ElementResultConstructors[t] = cstFunc;
            }

            return cstFunc;
        }

        /***************************************************/

        private static Output<List<Func<MaterialResult2, double>>, Func<object[], IElementResult<MaterialResult2>>> GetElementResultConstructorAndFuncs<T>() where T : MaterialResult2
        {
            Type t = typeof(T);

            Func<object[], IElementResult<MaterialResult2>> cstFunc;
            ConstructorInfo constructor = null;


            if (!m_ElementResultConstructors.TryGetValue(t, out cstFunc))
            {
                constructor = GetElementResultConstructorInfo<T>();
                Func<object[], object> genericFunc = constructor.ToFunc();
                cstFunc = x => (IElementResult<MaterialResult2>)genericFunc(x);
                m_ElementResultConstructors[t] = cstFunc;
            }

            List<Func<MaterialResult2, double>> funcs;

            if (!m_MaterialResultProperties.TryGetValue(t, out funcs))
            {
                if (constructor == null)
                    constructor = GetElementResultConstructorInfo<T>();

                Dictionary<string, PropertyInfo> props = t.GetProperties(BindingFlags.Instance | BindingFlags.Public).Where(x => x.PropertyType == typeof(double)).ToDictionary(x => x.Name, StringComparer.OrdinalIgnoreCase);

                funcs = new List<Func<MaterialResult2, double>>();

                foreach (ParameterInfo parameter in constructor.GetParameters())
                {
                    if (parameter.ParameterType != typeof(double))
                        continue;

                    PropertyInfo prop;
                    if (props.TryGetValue(parameter.Name, out prop))
                    {
                        Func<T, double> func = (Func<T, double>)Delegate.CreateDelegate(typeof(Func<T, double>), prop.GetGetMethod());
                        funcs.Add(x => func((T)x));
                    }
                }
                m_MaterialResultProperties[t] = funcs;
            }
            return new Output<List<Func<MaterialResult2, double>>, Func<object[], IElementResult<MaterialResult2>>> { Item1 = funcs, Item2 = cstFunc };
        }

        /***************************************************/

        private static ConstructorInfo GetElementResultConstructorInfo<T>() where T : MaterialResult2
        {
            Type materialResultType = GetMaterialElementResultType<T>();

            return materialResultType.GetConstructors().OrderByDescending(x => x.GetParameters().Count()).First();
        }

        /***************************************************/

        private static Type GetMaterialElementResultType<T>() where T : MaterialResult2
        {
            Type materialResultType = BH.Engine.Base.Query.BHoMTypeList()
                .First(x => typeof(IElementResult<T>).IsAssignableFrom(x));
            return materialResultType;
        }

        /***************************************************/

        private static ConcurrentDictionary<Type, Func<object[], IElementResult<MaterialResult2>>> m_ElementResultConstructors = new ConcurrentDictionary<Type, Func<object[], IElementResult<MaterialResult2>>>();
        private static ConcurrentDictionary<Type, List<Func<MaterialResult2, double>>> m_MaterialResultProperties = new ConcurrentDictionary<Type, List<Func<MaterialResult2, double>>>();


        /***************************************************/
    }
}

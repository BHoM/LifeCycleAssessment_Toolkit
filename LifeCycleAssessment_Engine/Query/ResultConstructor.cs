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

using BH.Engine.Base;
using BH.oM.Base;
using BH.oM.Base.Attributes;
using BH.oM.LifeCycleAssessment.MaterialFragments;
using BH.oM.LifeCycleAssessment.Results;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace BH.Engine.LifeCycleAssessment
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Gets a function corresponding to the constructor for a Element result corresponding to the provided type.\n" +
                     "If a type of ElementResult is provided, then the returned constructor will be corresponding to the provided type.\n" +
                     "If a type of MaterialResult is provided, the constructor will correspond to the ElementResult able to store this type.\n" +
                     "For all other types, null is returned.")]
        [Input("t", "The type to find a matching constructor for. Should be a type of ElementResult or a type of MaterialResult.")]
        [Output("cstFunc", "The function correpsonding to the constructor of the ElementResult related to the type.")]
        public static Func<object[], IElementResult<MaterialResult2>> ElementResultConstructor(this Type t)
        {
            Func<object[], IElementResult<MaterialResult2>> cstFunc;

            if (!m_ElementResultConstructors.TryGetValue(t, out cstFunc))
            {
                ConstructorInfo constructor = GetElementResultConstructorInfo(t);
                if (constructor != null)
                {
                    Func<object[], object> genericFunc = constructor.ToFunc();
                    cstFunc = x => (IElementResult<MaterialResult2>)genericFunc(x);
                }
                else
                    cstFunc = null;

                m_ElementResultConstructors[t] = cstFunc;
            }

            if (cstFunc == null)
                Base.Compute.RecordError($"Unable to find a constructor for a type of {nameof(IElementResult<MaterialResult2>)} based on provided type {t}");

            return cstFunc;
        }

        /***************************************************/

        [Description("Gets a function corresponding to the constructor for a Material result corresponding to the provided type.\n" +
                     "If a type of MaterialResult is provided, then the returned constructor will be corresponding to the provided type.\n" +
                     "If a type of EnvironmentalMetric is provided, the constructor will correspond to the MaterialResult corresponding to this type.\n" +
                     "For all other types, null is returned.")]
        [Input("t", "The type to find a matching constructor for. Should be a type of ElementResult or a type of MaterialResult.")]
        [Output("cstFunc", "The function correpsonding to the constructor of the ElementResult related to the type.")]
        public static Func<object[], MaterialResult2> MaterialResultConstructor(this Type t)
        {
            Func<object[], MaterialResult2> cstFunc;

            if (!m_MaterialResultConstructors.TryGetValue(t, out cstFunc))
            {
                ConstructorInfo constructor = GetElementResultConstructorInfo(t);
                if (constructor != null)
                {
                    Func<object[], object> genericFunc = constructor.ToFunc();
                    cstFunc = x => (MaterialResult2)genericFunc(x);
                }
                else
                    cstFunc = null;

                m_MaterialResultConstructors[t] = cstFunc;
            }

            if (cstFunc == null)
                Base.Compute.RecordError($"Unable to find a constructor for a type of {nameof(IElementResult<MaterialResult2>)} based on provided type {t}");

            return cstFunc;
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static ConstructorInfo GetElementResultConstructorInfo(Type t)
        {
            Type elementResultType = null;
            if (typeof(IElementResult<MaterialResult2>).IsAssignableFrom(t))
                elementResultType = t;
            else if (typeof(MaterialResult2).IsAssignableFrom(t))
                elementResultType = typeof(Query).GetMethod("GetMaterialElementResultType", BindingFlags.Static | BindingFlags.NonPublic).MakeGenericMethod(t).Invoke(null, new object[] { }) as Type;
            else
                return null;
            return elementResultType.GetConstructors().OrderByDescending(x => x.GetParameters().Count()).First();
        }

        /***************************************************/

        private static Type GetMaterialElementResultType<T>() where T : MaterialResult2
        {
            Type materialResultType = BH.Engine.Base.Query.BHoMTypeList()
                .First(x => typeof(IElementResult<T>).IsAssignableFrom(x));
            return materialResultType;
        }

        private static ConstructorInfo GetMaterialResultConstructorInfo(Type t)
        {
            Type materialResultType = null;
            if (typeof(MaterialResult2).IsAssignableFrom(t))
                materialResultType = t;
            else if(typeof(IEnvironmentalMetric).IsAssignableFrom(t))
                materialResultType = GetMaterialMaterialResultType(t);

            return materialResultType.GetConstructors().OrderByDescending(x => x.GetParameters().Count()).First();
        }

        /***************************************************/

        private static Type GetMaterialMaterialResultType(Type metricType)
        {
            string metric = metricType.Name.Replace("Metrics", "");
            Type materialResultType = BH.Engine.Base.Query.BHoMTypeList().Where(x => typeof(MaterialResult2).IsAssignableFrom(x)).First(x => x.Name.StartsWith(metric));
            return materialResultType;
        }

        /***************************************************/
        /**** Private Fields                            ****/
        /***************************************************/

        private static ConcurrentDictionary<Type, Func<object[], IElementResult<MaterialResult2>>> m_ElementResultConstructors = new ConcurrentDictionary<Type, Func<object[], IElementResult<MaterialResult2>>>();
        private static ConcurrentDictionary<Type, Func<object[], MaterialResult2>> m_MaterialResultConstructors = new ConcurrentDictionary<Type, Func<object[], MaterialResult2>>();

        /***************************************************/
    }
}

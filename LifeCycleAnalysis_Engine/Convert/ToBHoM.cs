using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.ComponentModel;
using BH.oM.Reflection.Attributes;
using BH.oM.Base;
using BH.oM.LifeCycleAnalysis;
using BH.Engine.Reflection;

namespace BH.Engine.LifeCycleAnalysis
{
    public static partial class Convert
    {
        /***************************************************/
        /****           Public Methods                  ****/
        /***************************************************/

        public static EPDData ToBHoMObject(this CustomObject obj)
        {
            int result = 0;

            EPDData data = new EPDData
            {
                Id = obj.PropertyValue("id")?.ToString() ?? "",

                Name = obj.PropertyValue("name")?.ToString() ?? "",

                Manufacturer = obj.PropertyValue("manufacturer.name")?.ToString() ?? "",

                Plant = obj.PropertyValue("plant.name")?.ToString() ?? "",

                PostalCode = int.TryParse(obj.PropertyValue("plant.postal_code")?.ToString() ?? "", out result) ? result : 0,

                Density = obj.PropertyValue("density")?.ToString() ?? "",

                GwpPerKG = obj.PropertyValue("gwp_per_kg")?.ToString() ?? "",
            };
            return data;
        }
    }
}

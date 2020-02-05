using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using BH.oM.Reflection.Attributes;
using BH.oM.Structure.Elements;
using BH.oM.Physical.Elements;
using BH.oM.Geometry;
using BH.oM.Physical.Constructions;
using BH.oM.LifeCycleAnalysis;


namespace BH.Engine.LifeCycleAnalysis
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Returns a structural composite assembly object needed to complete a life cycle analysis")]
        [Input("beams", "A collection of structural beams as BHoM objects")]
        [Input("columns", "A collection of structural columns as BHoM objects")]
        [Input("slabs", "A collection of structural slabs as BHoM objects")]
        [Output("structureAssembly", "A structural composite assembly object required for life cycle analysis")]

        public static StructuresAssembly structuresAssembly(Beam beams, Column columns, Floor slabs, Bracing bracing)
        {
            return null;
        }

    }
}

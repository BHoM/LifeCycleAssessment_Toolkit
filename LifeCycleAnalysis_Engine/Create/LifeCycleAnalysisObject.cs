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

        [Description("Returns a complete a life cycle analysis")]
        [Input("projectScope", "description here")]
        [Input("structuresAssembly", "description here")]
        [Input("foundationsAssembly", "description here")]
        [Input("enclosureAssembly", "description here")]
        [Input("mepAssembly", "description here")]
        [Input("interiorsAssembly", "description here")]
        [Output("lifeCycleAnalysis", "A structural composite assembly object required for life cycle analysis")]

        public static LifeCycleAnalysisObject lifeCycleAnalysisObject(ProjectScope projectScope, StructuresAssembly structuresAssembly, FoundationsAssembly foundationsAssembly, EnclosureAssembly enclosureAssembly, MEPAssembly mepAssembly, InteriorsAssembly interiorsAssembly)
        {
            return new LifeCycleAnalysisObject
            {


            };
        }

    }
}
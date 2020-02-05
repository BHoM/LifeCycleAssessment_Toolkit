using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Base;
using BH.oM.Physical.Constructions;
using BH.oM.Geometry;

namespace BH.oM.LifeCycleAnalysis
{
    public class LifeCycleAnalysisObject : BHoMObject, IBHoMGroup
    {
        /***************************************************/
        /**** Properties                                ****/
        /***************************************************/

        public LifeCycleAnalysisObject ProjectScope { get; set; } = null;
        public StructuresAssembly Structures { get; set; } = null;
        public FoundationsAssembly Foundations { get; set; } = null;
        public EnclosureAssembly Enclosures { get; set; } = null;
        public MEPAssembly MEP { get; set; } = null;
        public InteriorsAssembly Interiors { get; set; } = null;
        
    }
}

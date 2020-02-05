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
    public class StructuresAssembly : BHoMObject, IBHoMGroup
    {
        /***************************************************/
        /**** Properties                                ****/
        /***************************************************/

        public Construction Beams { get; set; } = null;
        public Construction Columns { get; set; } = null;
        public Construction Slabs { get; set; } = null;
        public Construction Bracing { get; set; } = null; 
    }
}

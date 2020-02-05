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
    public class FoundationsAssembly : BHoMObject, IBHoMGroup
    {
        /***************************************************/
        /**** Properties                                ****/
        /***************************************************/

        public Construction Walls { get; set; } = null;
        public Construction Footings { get; set; } = null;
        public Construction Piles { get; set; } = null;
    }
}

using System.Collections.Generic;
using BH.oM.Base;

namespace BH.oM.LifeCycleAnalysis
{
    public class SectorEPD : EPDData
    {
        /***************************************************/
        /**** Properties                                ****/
        /***************************************************/

        public List<string> Publisher { get; set; } = new List<string>();
        public List<string> Jurisdiction { get; set; } = new List<string>();

        /***************************************************/
    }
}

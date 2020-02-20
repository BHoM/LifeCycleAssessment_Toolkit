using System.Collections.Generic;
using BH.oM.Base;

namespace BH.oM.LifeCycleAnalysis
{
    public class EPDData : BHoMObject
    {
        /***************************************************/
        /**** Properties                                ****/
        /***************************************************/

        public string Id { get; set; } = "";
        public string Density { get; set; } = "";
        public string GwpPerKG { get; set; } = "";
        public double BiogenicEmbodiedCarbon { get; set; } = double.NaN;
        public string DeclaredUnit { get; set; } = "";
        public string Description { get; set; } = "";

        /***************************************************/
    }
}

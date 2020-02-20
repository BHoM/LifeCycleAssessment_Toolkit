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
        public string Name { get; set; } = "";
        public string Manufacturer { get; set; } = "";
        public string Plant { get; set; } = "";
        public int PostalCode { get; set; } = 0;
        public string Density { get; set; } = "";
        public string GwpPerKG { get; set; } = "";
        public double BiogenicEmbodiedCarbon { get; set; } = double.NaN;
        public string DeclaredUnit { get; set; } = "";
        public string Description { get; set; } = "";
        public List<string> IndustryStandards { get; set; } = new List<string>();

        /***************************************************/
    }
}

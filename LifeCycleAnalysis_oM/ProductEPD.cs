using System.Collections.Generic;
using BH.oM.Base;

namespace BH.oM.LifeCycleAnalysis
{
    public class ProductEPD : EPDData
    {
        /***************************************************/
        /**** Properties                                ****/
        /***************************************************/

        public string Manufacturer { get; set; } = "";
        public string Plant { get; set; } = "";
        public int PostalCode { get; set; } = 0;
        public List<string> IndustryStandards { get; set; } = new List<string>(); //returns blank lists

        /***************************************************/
    }
}

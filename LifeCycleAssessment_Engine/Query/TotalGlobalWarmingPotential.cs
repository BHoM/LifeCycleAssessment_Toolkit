using BH.oM.LifeCycleAssessment;
using BH.oM.LifeCycleAssessment.Results;
using BH.oM.Reflection.Attributes;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.LifeCycleAssessment
{
    public static partial class Query
    {
        [Description("Query the total global warming potential, or GWP from any LifeCycleAssessmentElementResult data. This is typically evaluated by kgCO2e or kg of Carbon Dioxide equivalent.")]
        [Input("results", "Supply a valid GlobalWarmingPotentialResult from a scope evaluation.")]
        [Output("gwp", "The combined amount of kgCO2e of the objects provided.")]
        public static double TotalGlobalWarmingPotential(this List<LifeCycleAssessmentElementResult> gwpResults)
        {
            return gwpResults.Where(x => x is GlobalWarmingPotentialResult).Select(x => (x as GlobalWarmingPotentialResult).GlobalWarmingPotential).Sum();
        }
    }
}

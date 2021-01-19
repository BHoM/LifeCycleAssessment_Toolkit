using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BH.oM.LifeCycleAssessment.Results;
using BH.oM.Reflection;

namespace BH.Engine.LifeCycleAssessment
{
    public static partial class Modify
    {
        public static List<LifeCycleAssessmentElementResult> Collapse(this Output<LifeCycleAssessmentElementResult, LifeCycleAssessmentElementResult, LifeCycleAssessmentElementResult> metric)
        {
            List<LifeCycleAssessmentElementResult> results =  new List<LifeCycleAssessmentElementResult>
            {
                metric.Item1,
                metric.Item2,
                metric.Item3,
            };

            return results.Where(x => x != null).ToList();
        }
    }
}

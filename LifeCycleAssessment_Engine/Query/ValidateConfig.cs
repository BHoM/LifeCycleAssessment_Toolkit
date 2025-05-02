using BH.oM.LifeCycleAssessment.Configs;
using BH.oM.LifeCycleAssessment.MaterialFragments;
using BH.oM.LifeCycleAssessment;
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using BH.oM.Base.Attributes;

namespace BH.Engine.LifeCycleAssessment
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Validates the config against the factors provider to ensure the two are compatible to be used together. If no config is provided (null) true will be returned as Environmental results can be computed without a config.")]
        [Input("config", "The config to evaluate against the provider.")]
        [Input("factorsProvider", "The provider to validate the config against.")]
        [Output("isValid", "Returns true if the config is valid to be used with the provider - false if not.")]
        public static bool IValidateConfig(IEvaluationConfig config, IEnvironmentalFactorsProvider factorsProvider)
        {
            if (config == null) //Null config is valid, as default case of evaluation is assumed for provided null config.
                return true;

            return ValidateConfig(config, factorsProvider);
        }

        /***************************************************/

        [Description("Validates the config against the metrics provider to ensure the two are compatible to be used together. An IStructEEvaluationConfig required the QuantityType on the factorsProvider to be of type Mass. If no config is provided (null) true will be returned as Environmental results can be computed without a config.")]
        [Input("config", "The config to evaluate against the provider.")]
        [Input("factorsProvider", "The provider to validate the config against.")]
        [Output("isValid", "Returns true if the config is valid to be used with the provider - false if not.")]
        public static bool ValidateConfig(IStructEEvaluationConfig config, IEnvironmentalFactorsProvider factorsProvider)
        {
            if(config == null) 
                return true;

            bool valid;
            if (factorsProvider is IBaseLevelEnvironalmentalFactorsProvider basicEnvironmentalfactorsProvider)
                valid = basicEnvironmentalfactorsProvider.QuantityType == QuantityType.Mass;
            else
                valid = false;

            if (!valid)
                BH.Engine.Base.Compute.RecordError($"{nameof(IStructEEvaluationConfig)} is only valid to be used with {factorsProvider.GetType().Name} with quantity type {QuantityType.Mass}.");
            return valid;
        }

        /***************************************************/
        /**** Private Methods - Validation - Fallback   ****/
        /***************************************************/

        private static bool ValidateConfig(IEvaluationConfig config, IEnvironmentalFactorsProvider factorsProvider)
        {
            return true;    //Default to true for fallback
        }

        /***************************************************/
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using Vts.MonteCarlo.PhaseFunctionInputs;
using Vts.MonteCarlo.PhaseFunctions;
using Vts.MonteCarlo.Tissues;

namespace Vts.MonteCarlo.Factories
{
    /// <summary>
    /// Instantiates appropriate ITissue given ITissueInput.
    /// </summary>
    public static class TissueFactory
    {
        // todo: revisit to make signatures here and in Tissue/TissueInput class signatures strongly typed
        /// <summary>
        /// Method to return ITissue given inputs
        /// </summary>
        /// <param name="ti">ITissueInput</param>
        /// <param name="awt">AbsorptionWeightingType enum</param>
        /// <param name="pft">PhaseFunctionType enum</param>
        /// <param name="russianRouletteWeightThreshold">Russian Roulette weight threshold</param>
        /// <returns>ITissue</returns>
        public static ITissue GetTissue(ITissueInput ti, AbsorptionWeightingType awt, IDictionary<string, IPhaseFunctionInput> regionPhaseFunctionInputs, double russianRouletteWeightThreshold)
        {
            ITissue t = ti.CreateTissue(awt, regionPhaseFunctionInputs, russianRouletteWeightThreshold);

            if (t == null)
                throw new ArgumentException(
                    "Problem generating ITissue instance. Check that TissueInput, ti, has a matching ITissue definition.");

            return t;
        }
    }
}

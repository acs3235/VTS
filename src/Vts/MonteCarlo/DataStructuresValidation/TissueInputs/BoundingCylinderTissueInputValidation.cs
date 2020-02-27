using System.Collections.Generic;
using System.Linq;
using Vts.MonteCarlo.DataStructuresValidation;
using Vts.MonteCarlo.Extensions;
using Vts.MonteCarlo.Tissues;

namespace Vts.MonteCarlo
{
    /// <summary>
    /// This verifies that the bounding cylinder is the same height as the tissue,
    /// and that the refractive index of the tissue layer and ellipsoid match.
    /// </summary>
    public class BoundingCylinderTissueInputValidation
    {
        /// <summary>
        /// Main validation method for BoundingCylinderTissueInput.
        /// </summary>
        /// <param name="input">tissue input defined in SimulationInput</param>
        /// <returns>ValidationResult</returns>
        public static ValidationResult ValidateInput(ITissueInput input)
        {
            var layers = ((BoundingCylinderTissueInput)input).LayerRegions.Select(region => (LayerTissueRegion)region).ToArray();
            var boundingCylinder = (CaplessCylinderTissueRegion)((BoundingCylinderTissueInput)input).CylinderRegion;
            ValidationResult tempResult;
            tempResult = ValidateGeometry(layers, boundingCylinder);
            if (!tempResult.IsValid)
            {
                return tempResult;
            }
            tempResult = ValidateRefractiveIndexMatch(layers, boundingCylinder);
            if (!tempResult.IsValid)
            {
                return tempResult;
            }
            return tempResult;
        }
        /// <summary>
        /// Method to validate that the geometry of tissue layers and bounding cylinder agree with capabilities
        /// of code.
        /// </summary>
        /// <param name="layers">list of LayerTissueRegion</param>
        /// <param name="boundingCylinder">CylinderTissueRegion</param>
        /// <returns>ValidationResult</returns>
        private static ValidationResult ValidateGeometry(IList<LayerTissueRegion> layers, 
            CaplessCylinderTissueRegion boundingCylinder)
        {            
            // check that layer definition is valid
            var tempResult = MultiLayerTissueInputValidation.ValidateLayers(layers);

            if (!tempResult.IsValid){ return tempResult; }

            // test for air layers and eliminate from list
            var tissueLayers = layers.Where(layer => !layer.IsAir());

            var layersHeight = tissueLayers.Sum(layer => layer.ZRange.Delta);

            if (boundingCylinder.Height != layersHeight)
            {
                tempResult = new ValidationResult(
                    false,
                    "BoundingCylinderTissueInput: bounding cylinder must have same height as tissue",
                    "BoundingCylinderTissueInput: make sure cylinder Height = depth of tissue");
            }

            if (!tempResult.IsValid) { return tempResult; }

            // check that there is at least one layer of tissue 
            if (!tissueLayers.Any())
            {
                tempResult = new ValidationResult(
                    false,
                    "BoundingCylinderTissueInput: tissue layer is assumed to be at least a single layer with air layer above and below",
                    "BoundingCylinderTissueInput: redefine tissue definition to contain at least a single layer of tissue");
            }

            if (!tempResult.IsValid) { return tempResult; }

            return new ValidationResult(
                true,
                "BoundingCylinderTissueInput: geometry and refractive index settings validated");
        }

        /// <summary>
        /// Method to verify refractive index of tissue layer and bounding cylinder match.
        /// Code does not yet include reflecting/refracting off bounding cylinder surface.
        /// </summary>
        /// <param name="layers">list of LayerTissueRegion</param>
        /// <param name="boundingCylinder">CylinderTissueRegion></param>
        /// <returns>ValidationResult</returns>
        private static ValidationResult ValidateRefractiveIndexMatch(
            IList<LayerTissueRegion> layers, CaplessCylinderTissueRegion boundingCylinder)
        {
            // for all tissue layers
            for (int i = 1; i < layers.Count - 2; i++)
            {
                if (layers[i].RegionOP.N != boundingCylinder.RegionOP.N)
                {
                    return new ValidationResult(
                        false,
                        "BoundingCylinderTissueInput: refractive index of tissue layer must match that of bounding cylinder",
                        "Change N of bounding cylinder to match tissue layer N");
                }
            }
            return new ValidationResult(
                true,
                "BoundingCylinderTissueInput: refractive index of tissue and bounding cylinder match");
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Vts.Common;
using Vts.MonteCarlo.Tissues;

namespace Vts.MonteCarlo
{
    /// <summary>
    /// Implements ITissueInput.  Defines input to MultiLayerTissue class.
    /// </summary>
    [KnownType(typeof(LayerRegion))]
    [KnownType(typeof(OpticalProperties))]
    [KnownType(typeof(List<OpticalProperties>))]
    [KnownType(typeof(List<LayerRegion>))]
    [KnownType(typeof(List<ITissueRegion>))]
    public class MultiLayerTissueInput : ITissueInput
    {
        private IList<ITissueRegion> _regions;

        /// <summary>
        /// MultiLayeredTissueInput constructor 
        /// </summary>
        public MultiLayerTissueInput(IList<ITissueRegion> regions)
        {
            _regions = regions;
            ValidateInput(regions);
        }

        /// <summary>
        /// MultiLayerTissue default constructor provides homogeneous tissue
        /// </summary>
        public MultiLayerTissueInput()
            : this(
                new List<ITissueRegion> 
                { 
                    new LayerRegion(
                        new DoubleRange(double.NegativeInfinity, 0.0),
                        new OpticalProperties(0.0, 1e-10, 0.0, 1.0)),
                    new LayerRegion(
                        new DoubleRange(0.0, 100.0),
                        new OpticalProperties(0.0, 1.0, 0.8, 1.4)),
                    new LayerRegion(
                        new DoubleRange(100.0, double.PositiveInfinity),
                        new OpticalProperties(0.0, 1e-10, 0.0, 1.0))
                })
        {
        }

        public IList<ITissueRegion> Regions { get { return _regions; } set { _regions = value; } }
     
        /// <summary>
        /// This verifies that the layers do not overlap.  It assumes that the layers are
        /// adjacent and defined in order.
        /// </summary>
        /// <param name="layers"></param>
        private void ValidateInput(IList<ITissueRegion> layers)
        {
            for (int i = 0; i < layers.Count() - 1; i++)
            {
                var thisLayer = (LayerRegion)layers[i];
                var nextLayer = (LayerRegion)layers[i + 1];
                if (thisLayer.ZRange.Stop != nextLayer.ZRange.Start)
                {
                    throw new ArgumentException("MultiLayerTissueInput: layers start/stop definition in error.");
                }
            }
        }
    }
}
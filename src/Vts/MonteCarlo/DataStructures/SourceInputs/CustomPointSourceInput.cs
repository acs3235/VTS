using Vts.Common;

namespace Vts.MonteCarlo
{   
    /// <summary>
    /// Implements ISourceInput.  Defines input data for PointSource implementation
    /// including position, direction and range of theta and phi.
    /// </summary>
    public class CustomPointSourceInput : ISourceInput
    {
        // this handles point
        public CustomPointSourceInput( 
            Position pointLocation,
            Direction solidAngleAxis,
            DoubleRange thetaRange, 
            DoubleRange phiRange,
            int startingRegionIndex) 
        {
            PointLocation = pointLocation;
            SolidAngleAxis = solidAngleAxis;
            ThetaRange = thetaRange;
            PhiRange = phiRange;
            StartingRegionIndex = startingRegionIndex;
        }
        public CustomPointSourceInput()
            : this(
                new Position (0, 0, 0),
                new Direction(0, 0, 1),
                new DoubleRange(0.0, 0, 1),
                new DoubleRange(0.0, 0, 1),
                0) { }

        public Position PointLocation { get; set; }
        public Direction SolidAngleAxis { get; set; }
        public DoubleRange ThetaRange { get; set; }
        public DoubleRange PhiRange { get; set; }
        public int StartingRegionIndex { get; set; }
    }
}
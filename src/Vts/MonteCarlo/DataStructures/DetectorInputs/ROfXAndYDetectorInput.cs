using System;
using System.Collections.Generic;
using Vts.Common;

namespace Vts.MonteCarlo
{
    /// <summary>
    /// DetectorInput for R(x,y)
    /// </summary>
    public class ROfXAndYDetectorInput : IDetectorInput
    {
        /// <summary>
        /// constructor for reflectance as a function of x and y detector input
        /// </summary>
        /// <param name="x">x binning</param>
        /// <param name="y">y binning</param>
        /// <param name="name">detector name</param>
        public ROfXAndYDetectorInput(DoubleRange x, DoubleRange y, String name)
        {
            TallyType = "ROfXAndY";
            Name = name;
            X = x;
            Y = y;
        }
        /// <summary>
        /// constructor uses TallyType for name
        /// </summary>
        /// <param name="x">x binning</param>
        /// <param name="y">y binning</param>
        public ROfXAndYDetectorInput(DoubleRange x, DoubleRange y) 
            : this (x, y, "ROfXAndY") {}

        /// <summary>
        /// Default constructor uses default x and y bins
        /// </summary>
        public ROfXAndYDetectorInput() 
            : this (new DoubleRange(-200.0, 200.0, 401), 
                    new DoubleRange(-200.0, 200.0, 401), 
                    "ROfXAndY") {}

        /// <summary>
        /// detector tally identifier
        /// </summary>
        public string TallyType { get; set; }
        /// <summary>
        /// detector name, defaults to TallyType.ToString() but can be user specified
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// x-axis binning 
        /// </summary>
        public DoubleRange X { get; set; }
        /// <summary>
        /// y-axis binning
        /// </summary>
        public DoubleRange Y { get; set; }
    }
}

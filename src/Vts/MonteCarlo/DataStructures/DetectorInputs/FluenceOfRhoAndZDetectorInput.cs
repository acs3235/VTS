using System;
using System.Collections.Generic;
using Vts.Common;

namespace Vts.MonteCarlo
{
    /// <summary>
    /// DetectorInput for Flu(r,z)
    /// </summary>
    public class FluenceOfRhoAndZDetectorInput : IDetectorInput
    {
        /// <summary>
        /// constructor for fluence as a function of rho and z detector input
        /// </summary>
        /// <param name="rho">rho binning</param>
        /// <param name="z">z binning</param>
        /// <param name="name">detector name</param>
        public FluenceOfRhoAndZDetectorInput(DoubleRange rho, DoubleRange z, String name)
        {
            TallyType ="FluenceOfRhoAndZ";
            Name = name;
            Rho = rho;
            Z = z;
        }
        /// <summary>
        /// constructor that uses TallyType for name
        /// </summary>
        /// <param name="rho">rho binning</param>
        /// <param name="z">z binning</param>
        public FluenceOfRhoAndZDetectorInput(DoubleRange rho, DoubleRange z) 
            : this (rho, z, "FluenceOfRhoAndZ") { }

        /// <summary>
        /// Default constructor uses default rho and z bins
        /// </summary>
        public FluenceOfRhoAndZDetectorInput()
            : this(
                new DoubleRange(0.0, 10.0, 101), //rho
                new DoubleRange(0.0, 10.0, 101), // z
                "FluenceOfRhoAndZ") {}

        /// <summary>
        /// detector identifier
        /// </summary>
        public string TallyType { get; set; }
        /// <summary>
        /// detector name, defaults to TallyType.ToString() but can be user specified
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// detector rho binning
        /// </summary>
        public DoubleRange Rho { get; set; }
        /// <summary>
        /// detector z binning
        /// </summary>
        public DoubleRange Z { get; set; }
    }
}

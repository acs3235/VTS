﻿using System;
using Vts.Common;

namespace Vts.MonteCarlo.Sources
{
    /// <summary>
    /// Implements ISourceInput. Defines input data for FluorescenceEmissionAOfRhoAndZSource 
    /// implementation.  This source reads the Cartesian coordinate absorbed energy results of a
    /// prior simulation and uses it to generate an emission source.
    /// </summary>
    public class FluorescenceEmissionAOfRhoAndZSourceInput : ISourceInput
    {
        /// <summary>
        /// Initializes a new instance of FluorescenceEmissionAOfRhoAndZSourceInput class
        /// </summary>
        /// <param name="inputFolder">Folder where AOfRhoAndZ resides</param>
        /// <param name="infile">Infile for simulation that generated AOfRhoAndZ</param>
        /// <param name="initialTissueRegionIndex">Tissue region of fluorescence</param>
        /// <param name="samplingMethod">sample initial position: CDF from AE or Uniform</param>
        public FluorescenceEmissionAOfRhoAndZSourceInput(
            string inputFolder, string infile, int initialTissueRegionIndex, 
            SourcePositionSamplingType samplingMethod)
        {
            SourceType = "FluorescenceEmissionAOfRhoAndZ";
            InputFolder = inputFolder;
            Infile = infile;
            InitialTissueRegionIndex = initialTissueRegionIndex;
            SamplingMethod = samplingMethod;
        }

        /// <summary>
        /// Initializes the default constructor of FluorescenceEmissionAOfRhoAndZSourceInput class
        /// </summary>
        public FluorescenceEmissionAOfRhoAndZSourceInput()
            : this("", "",  0, SourcePositionSamplingType.CDF) { }

        /// <summary>
        ///  fluorescence emission source type
        /// </summary>
        public string SourceType { get; set; }
        /// <summary>
        /// Input folder where AE(x,y,z) resides
        /// </summary>
        public string InputFolder { get; set; }
        /// <summary>
        /// Infile that generated AOfRhoAndZ
        /// </summary>
        public string Infile { get; set; }
        /// <summary>
        /// Initial tissue region index = tissue region index of fluorescence
        /// </summary>
        public int InitialTissueRegionIndex { get; set; }
        /// <summary>
        /// Sampling method for location and associated weight
        /// </summary>
        public SourcePositionSamplingType SamplingMethod { get; set; }

        /// <summary>
        /// Required code to create a source based on the input values
        /// </summary>
        /// <param name="rng"></param>
        /// <returns></returns>
        public ISource CreateSource(Random rng = null)
        {
            rng = rng ?? new Random();

            return new FluorescenceEmissionAOfRhoAndZSource(
                this.InputFolder,
                this.Infile,
                this.InitialTissueRegionIndex,
                this.SamplingMethod) { Rng = rng };
        }
    }

    /// <summary>
    /// Implements FluorescenceEmissionAOfRhoAndZSource with AOfRhoAndZ created from prior
    /// simulation and initial tissue region index.
    /// </summary>
    public class FluorescenceEmissionAOfRhoAndZSource : FluorescenceEmissionSourceBase
    {
        double _totalWeight = 0.0;
        /// <summary>
        /// class that holds all Source arrays for proper initiation
        /// </summary>
        public AOfRhoAndZLoader Loader { get; set; }
        /// <summary>
        /// Sampling method flag
        /// </summary>
        public SourcePositionSamplingType SamplingMethod { get; set; }

        /// <summary>
        /// key into dictionary of indices
        /// </summary>
        public static int IndexCount = 0;

        /// <summary>
        /// Returns an instance of  Fluorescence Emission AOfRhoAndZ Source with
        /// a Lambertian angular distribution.
        /// </summary>
        /// <param name="initialTissueRegionIndex">Initial tissue region index</param>
        public FluorescenceEmissionAOfRhoAndZSource(
            string inputFolder,
            string infile,
            int initialTissueRegionIndex,
            SourcePositionSamplingType samplingMethod)
            : base(
                inputFolder,
                infile,
                initialTissueRegionIndex)
        {
            SamplingMethod = samplingMethod;
            Loader = new AOfRhoAndZLoader(inputFolder, infile, initialTissueRegionIndex);
        }

        protected override Position GetFinalPositionAndWeight(Random rng, out double weight)
        {
            Position finalPosition = null;
            double xMidpoint, yMidpoint, zMidpoint;
            switch (SamplingMethod)
            {
                case SourcePositionSamplingType.CDF:
                    // determine position from CDF determined in AOfRhoAndZLoader
                    // due to ordering of indices CDF will be increasing with each increment
                    double rho = rng.NextDouble();
                    for (int i = 0; i < Loader.Rho.Count - 1; i++)
                    {
                        for (int k = 0; k < Loader.Z.Count - 1; k++)
                        {
                            if (Loader.MapOfRhoAndZ[i, k] == 1)
                            {
                                if (rho < Loader.CDFOfRhoAndZ[i, k])
                                {
                                    // SHOULD I SAMPLE THIS W CYLINDRICAL (Y=0) OR CARTESIAN COORD?
                                    xMidpoint = Loader.Rho.Start + i * Loader.Rho.Delta + Loader.Rho.Delta / 2;
                                    yMidpoint = 0.0;
                                    zMidpoint = Loader.Z.Start + k * Loader.Z.Delta + Loader.Z.Delta / 2;
                                    // the following outputs initial positions so that a plot can show distribution
                                    //Console.WriteLine(xMidpoint.ToString("") + " " +
                                    //                  yMidpoint.ToString("") + " " +
                                    //                  zMidpoint.ToString(""));
                                    weight = 1.0;
                                    return new Position(xMidpoint, yMidpoint, zMidpoint);
                                }
                            }
                        }
                        
                    }
                    weight = 1.0;
                    return finalPosition;
                case SourcePositionSamplingType.Uniform:
                    // rotate through indices by starting over if reached end
                    if (IndexCount > Loader.FluorescentRegionIndicesInOrder.Count - 1)
                    {
                        IndexCount = 0;
                        // the following output is to verify after each cycle through voxels total AE correct
                        //Console.WriteLine("totalWeight = " + _totalWeight.ToString(""));
                    }
                    var indices = Loader.FluorescentRegionIndicesInOrder[IndexCount].ToArray();
                    var irho = indices[0];
                    var iz = indices[1];
                    xMidpoint = Loader.Rho.Start + irho * Loader.Rho.Delta + Loader.Rho.Delta / 2;
                    yMidpoint = 0.0;
                    zMidpoint = Loader.Z.Start + iz * Loader.Z.Delta + Loader.Z.Delta / 2;
                    // undo normalization performed when AOfRhoAndZDetector saved
                    var normalizationFactor = 2.0 * Math.PI * Loader.Rho.Delta * Loader.Z.Delta;
                    var rhozNorm = (Loader.Rho.Start + (irho + 0.5) * Loader.Rho.Delta) * normalizationFactor;
                    weight = Loader.AOfRhoAndZ[irho, iz] * rhozNorm;
                    _totalWeight = _totalWeight + weight;
                    IndexCount = IndexCount + 1;
                    return new Position(xMidpoint, yMidpoint, zMidpoint);
            }
            weight = 1.0;
            return finalPosition;
        }

    }

}

﻿using System.Linq;
using NUnit.Framework;
using Vts.Common;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Detectors;
using Vts.MonteCarlo.Factories;

// using the following for user-defined ROfXDetector implementation
using System.Runtime.Serialization;
using Vts.IO;
using Vts.MonteCarlo.Helpers;
using Vts.MonteCarlo.IO;
using Vts.MonteCarlo.PhotonData;

namespace Vts.Test.MonteCarlo.Factories
{
    /// <summary>
    /// Unit tests for DetectorFactory
    /// </summary>
    [TestFixture]
    public class DetectorFactoryTests
    {
        /// <summary>
        /// Simulate basic usage of DetectorFactory
        /// </summary>
        [Test]
        public void demonstrate_basic_mc_creation()
        {
            var detectorInput = new ROfRhoDetectorInput
                {
                    TallyType =  "ROfRho", 
                    Name = "My First Detector", 
                    TallySecondMoment = false,
                    Rho = new DoubleRange(0, 1, 5),
                };

            var simInput = new SimulationInput { DetectorInputs = new[] { detectorInput } };

            var sim = simInput.CreateSimulation();

            var results = sim.Run();

            IDetector detector;

            var rOfRhoDetector = results.ResultsDictionary.TryGetValue(detectorInput.Name, out detector);

            Assert.NotNull(rOfRhoDetector);
        }

        /// <summary>
        /// Simulate fluent usage of DetectorFactory
        /// </summary>
        [Test]
        public void demonstrate_fluent_mc_creation()
        {
            var rOfRhoDetector = new SimulationInput
                {
                    DetectorInputs = new[]
                    {
                        new ROfRhoDetectorInput
                        {
                            TallyType = "ROfRho",
                            Name = "My First Detector",
                            TallySecondMoment = false,
                            Rho = new DoubleRange(0, 1, 5),
                        }
                    }
                }
                .CreateSimulation()
                .Run()
                .GetDetector("My First Detector");

            Assert.NotNull(rOfRhoDetector);
        }
        
        /// <summary>
        /// Simulate usage of user-defined types with DetectorFactory.RegisterDetector method
        /// </summary>
        [Test]
        public void demonstrate_user_defined_detector_usage()
        {
            DetectorFactory.RegisterDetector(typeof (ROfXDetectorInput), typeof (ROfXDetector));

            var detectorInput = new ROfXDetectorInput
            {
                TallyType = "ROfX",
                Name = "My First R(x) Detector",
                TallySecondMoment = true,
                X = new DoubleRange(0, 10, 101),
            };

            var simInput = new SimulationInput
                {
                    DetectorInputs = new[] { detectorInput },
                    N = 100,
                };

            var sim = simInput.CreateSimulation();

            var results = sim.Run();

            IDetector detector;

            var detectorExists = results.ResultsDictionary.TryGetValue(detectorInput.Name, out detector);
            
            Assert.IsTrue(detectorExists);

            var firstValue = ((ROfXDetector)detector).Mean.FirstOrDefault();

            Assert.IsTrue(firstValue != 0);

            DetectorIO.WriteDetectorToFile(detector, "user_defined_detector");

            var deserializedDetector = DetectorIO.ReadDetectorFromFile("user_defined_detector", "");
        }

        #region User-defined ROfXDetector
        /// <summary>
        /// DetectorInput for R(r)
        /// </summary>
        public class ROfXDetectorInput : DetectorInput, IDetectorInput
        {
            /// <summary>
            /// constructor for reflectance as a function of x detector input
            /// </summary>
            public ROfXDetectorInput()
            {
                TallyType = "ROfX";
                Name = "My R(x) Detector";
                X = new DoubleRange(0.0, 10, 101);

                // modify base class TallyDetails to take advantage of built-in validation capabilities (error-checking)
                TallyDetails.IsReflectanceTally = true;
            }

            /// <summary>
            /// detector x binning
            /// </summary>
            public DoubleRange X { get; set; }

            public IDetector CreateDetector()
            {
                return new ROfXDetector
                {
                    // required properties (part of DetectorInput/Detector base classes)
                    TallyType = this.TallyType,
                    Name = this.Name,
                    TallySecondMoment = this.TallySecondMoment,
                    TallyDetails = this.TallyDetails,

                    // optional/custom detector-specific properties
                    X = this.X
                };
            }
        }

        /// <summary>
        /// Implements IDetector.  Tally for reflectance as a function  of X.
        /// This implementation works for Analog, DAW and CAW processing.
        /// </summary>
        public class ROfXDetector : Detector, IDetector
        {
            /* ==== Place optional/user-defined input properties here. They will be saved in text (JSON) format ==== */
            /* ==== Note: make sure to copy over all optional/user-defined inputs from corresponding input class ==== */
            /// <summary>
            /// x binning
            /// </summary>
            public DoubleRange X { get; set; }

            /* ==== Place user-defined output arrays here. They should be prepended with "[IgnoreDataMember]" attribute ==== */
            /* ==== Then, GetBinaryArrays() should be implemented to save them separately in binary format ==== */
            /// <summary>
            /// detector mean
            /// </summary>
            [IgnoreDataMember]
            public double[] Mean { get; set; }
            /// <summary>
            /// detector second moment
            /// </summary>
            [IgnoreDataMember]
            public double[] SecondMoment { get; set; }

            /* ==== Place optional/user-defined output properties here. They will be saved in text (JSON) format ==== */
            /// <summary>
            /// number of times detector gets tallied to
            /// </summary>
            public long TallyCount { get; set; }

            public void Initialize(ITissue tissue)
            {
                // assign any user-defined outputs (except arrays...we'll make those on-demand)
                TallyCount = 0;

                // if the data arrays are null, create them (only create second moment if TallySecondMoment is true)
                Mean = Mean ?? new double[X.Count - 1];
                SecondMoment = SecondMoment ?? (TallySecondMoment ? new double[X.Count - 1] : null);

                // intialize any other necessary class fields here
            }

            /// <summary>
            /// method to tally to detector
            /// </summary>
            /// <param name="photon">photon data needed to tally</param>
            public void Tally(Photon photon)
            {
                var ir = DetectorBinning.WhichBin(photon.DP.Position.X, X.Count - 1, X.Delta, X.Start);

                Mean[ir] += photon.DP.Weight;
                if (TallySecondMoment)
                {
                    SecondMoment[ir] += photon.DP.Weight * photon.DP.Weight;
                }
                TallyCount++;
            }

            /// <summary>
            /// method to normalize detector tally results
            /// </summary>
            /// <param name="numPhotons">number of photons launched</param>
            public void Normalize(long numPhotons)
            {
                // normalization accounts for Rho.Start != 0
                var normalizationFactor = X.Delta;
                for (int ir = 0; ir < X.Count - 1; ir++)
                {
                    Mean[ir] /= normalizationFactor * numPhotons;
                    if (TallySecondMoment)
                    {
                        SecondMoment[ir] /= normalizationFactor * normalizationFactor * numPhotons;
                    }
                }
            }

            // this is to allow saving of large arrays separately as a binary file
            public BinaryArraySerializer[] GetBinarySerializers()
            {
                return new[] {
                new BinaryArraySerializer {
                    DataArray = Mean,
                    Name = "Mean",
                    FileTag = "",
                    WriteData = binaryWriter => {
                        for (int i = 0; i < X.Count - 1; i++) {
                            binaryWriter.Write(Mean[i]);
                        }
                    },
                    ReadData = binaryReader => {
                        Mean = Mean ?? new double[ X.Count - 1];
                        for (int i = 0; i <  X.Count - 1; i++) {
                            Mean[i] = binaryReader.ReadDouble();
                        }
                    }
                },
                // return a null serializer, if we're not serializing the second moment
                !TallySecondMoment ? null :  new BinaryArraySerializer {
                    DataArray = SecondMoment,
                    Name = "SecondMoment",
                    FileTag = "_2",
                    WriteData = binaryWriter => {
                        if (!TallySecondMoment || SecondMoment == null) return;
                        for (int i = 0; i < X.Count - 1; i++) {
                            binaryWriter.Write(SecondMoment[i]);
                        }
                    },
                    ReadData = binaryReader => {
                        if (!TallySecondMoment || SecondMoment == null) return;
                        SecondMoment = new double[ X.Count - 1];
                        for (int i = 0; i < X.Count - 1; i++) {
                            SecondMoment[i] = binaryReader.ReadDouble();
                        }
                    },
                },
            };
            }

            /// <summary>
            /// Method to determine if photon is within detector
            /// </summary>
            /// <param name="dp">photon data point</param>
            /// <returns>method always returns true</returns>
            public bool ContainsPoint(PhotonDataPoint dp)
            {
                return true; // or, possibly test for NA or confined position, etc
                //return (dp.StateFlag.Has(PhotonStateType.PseudoTransmissionDomainTopBoundary));
            }

        }
        #endregion
    }
}

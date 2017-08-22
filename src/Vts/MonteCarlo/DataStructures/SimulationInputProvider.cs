using System;
using System.Collections.Generic;
using Vts.Common;
using Vts.MonteCarlo.Detectors;
using Vts.MonteCarlo.Helpers;
using Vts.MonteCarlo.PhaseFunctionInputs;
using Vts.MonteCarlo.Sources;
using Vts.MonteCarlo.Sources.SourceProfiles;
using Vts.MonteCarlo.Tissues;

namespace Vts.MonteCarlo
{
    /// <summary>
    /// Implements various commonly used SimulationInput classes for various tissue types.
    /// </summary>
    public class SimulationInputProvider : SimulationInput
    {
        /// <summary>
        /// Method that provides instances of all inputs in this class.
        /// </summary>
        /// <returns>a list of the SimulationInputs generated</returns>
        public static IList<SimulationInput> GenerateAllSimulationInputs()
        {
            return new List<SimulationInput>()
            {
                PointSourceOneLayerTissueAllDetectors(),
                PointSourceOneLayerTissueROfRhoAndFluenceOfRhoAndZDetectors(),
                PointSourceOneLayerTissueRadianceOfRhoAndZAndAngleDetector(),
                PointSourceTwoLayerTissueROfRhoDetector(),
                PointSourceTwoLayerTissueROfRhoDetectorWithPhotonDatabase(),
                PointSourceSingleEllipsoidTissueFluenceOfRhoAndZDetector(),
                PointSourceOneLayerTissueLUTPhaseFunctionROfRhoDetector(),
                pMCPointSourceOneLayerTissueROfRhoDAW(),
                GaussianSourceOneLayerTissueROfRhoDetector(),
                FlatSourceOneLayerTissueROfRhoDetector(),
                PointSourceMultiLayerMomentumTransferDetectors(),
                PointSourceSingleVoxelTissueROfXAndYAndFluenceOfXAndYAndZDetector(),
                PointSourceThreeLayerReflectedTimeOfRhoAndSubregionHistDetector()
            };
        }

        #region point source one layer tissue all detectors
        /// <summary>
        /// Point source, single tissue layer definition, all detectors included
        /// </summary>
        public static SimulationInput PointSourceOneLayerTissueAllDetectors()
        {
            MultiLayerTissueInput ti = new MultiLayerTissueInput(
                    new ITissueRegion[]
                    { 
                        new LayerTissueRegion(
                            new DoubleRange(double.NegativeInfinity, 0.0),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0),
                        "HenyeyGreensteinKey1"),
                        new LayerTissueRegion(
                            new DoubleRange(0.0, 100.0),
                            new OpticalProperties(0.01, 1.0, 0.8, 1.4),
                        "HenyeyGreensteinKey2"),
                        new LayerTissueRegion(
                            new DoubleRange(100.0, double.PositiveInfinity),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0),
                        "HenyeyGreensteinKey3")
                    }
                );
            ti.RegionPhaseFunctionInputs.Add("HenyeyGreensteinKey1", new HenyeyGreensteinPhaseFunctionInput());
            ti.RegionPhaseFunctionInputs.Add("HenyeyGreensteinKey2", new HenyeyGreensteinPhaseFunctionInput());
            ti.RegionPhaseFunctionInputs.Add("HenyeyGreensteinKey3", new HenyeyGreensteinPhaseFunctionInput());
            return new SimulationInput(
                100,
                "one_layer_all_detectors",
                new SimulationOptions(
                    0, // random number generator seed, -1=random seed, 0=fixed seed
                    RandomNumberGeneratorType.MersenneTwister,
                    AbsorptionWeightingType.Discrete,
                    new List<DatabaseType>() { }, // databases to be written
                    false, // track statistics
                    0.0, // RR threshold -> no RR performed
                    0),
                new DirectionalPointSourceInput(
                    new Position(0.0, 0.0, 0.0),
                    new Direction(0.0, 0.0, 1.0),
                    0), // 0=start in air, 1=start in tissue
                ti,
                new List<IDetectorInput>()
                {
                    // units space[mm], time[ns], temporal-freq[GHz], abs./scat. coeff[/mm]    
                    new AOfRhoAndZDetectorInput() {Rho=new DoubleRange(0.0, 10, 101),Z=new DoubleRange(0.0, 10, 101)},
                    new AOfXAndYAndZDetectorInput(){X=new DoubleRange(-10, 10, 201),Y=new DoubleRange(-10, 10, 2),Z=new DoubleRange(0, 10, 101)},                
                    new ATotalDetectorInput(),      
                    new FluenceOfRhoAndZAndTimeDetectorInput(){Rho=new DoubleRange(0.0, 10, 101),Z=new DoubleRange(0.0, 10, 101),Time= new DoubleRange(0.0, 10, 101)},              
                    new FluenceOfRhoAndZDetectorInput() {Rho=new DoubleRange(0.0, 10, 101),Z=new DoubleRange(0.0, 10, 101)},
                    new FluenceOfXAndYAndZDetectorInput(){X=new DoubleRange(-10, 10, 201),Y=new DoubleRange(-10, 10, 2),Z=new DoubleRange(0, 10, 101)},
                    new FluenceOfXAndYAndZAndOmegaDetectorInput(){X=new DoubleRange(-10, 10, 201),Y=new DoubleRange(-10, 10, 2),Z=new DoubleRange(0, 10, 101),Omega=new DoubleRange(0.0, 1, 21)},
                    new RadianceOfRhoAndZAndAngleDetectorInput(){Rho=new DoubleRange(0.0, 10, 101),Z=new DoubleRange(0.0, 10, 101),Angle=new DoubleRange(0, Math.PI, 3)},
                    new RadianceOfFxAndZAndAngleDetectorInput(){Fx=new DoubleRange(0.0, 0.5, 51),Z=new DoubleRange(0.0, 10, 101),Angle=new DoubleRange(0, Math.PI, 3)},
                    new RadianceOfXAndYAndZAndThetaAndPhiDetectorInput(){
                        X=new DoubleRange(-10.0, 10.0, 101),
                        Y= new DoubleRange(-10.0, 10.0, 101),
                        Z= new DoubleRange(0.0, 10.0, 101), 
                        Theta=new DoubleRange(0.0, Math.PI, 5), // theta (polar angle)
                        Phi=new DoubleRange(-Math.PI, Math.PI, 5)}, // phi (azimuthal angle)
                    new RadianceOfRhoAtZDetectorInput() {Rho = new DoubleRange(0.0, 10, 101), ZDepth = 3},
                    new RDiffuseDetectorInput(),
                    new ROfAngleDetectorInput() {Angle=new DoubleRange(Math.PI / 2 , Math.PI, 5)},
                    new ROfFxAndTimeDetectorInput() {Fx = new DoubleRange(0.0, 0.5, 51), Time= new DoubleRange(0.0, 10, 101)},
                    new ROfFxDetectorInput() {Fx = new DoubleRange(0.0, 0.5, 51)},
                    new ROfRhoAndAngleDetectorInput() {Rho=new DoubleRange(0.0, 10, 101),Angle=new DoubleRange(Math.PI / 2 , Math.PI, 5)},             
                    new ROfRhoAndOmegaDetectorInput() {Rho=new DoubleRange(0.0, 10, 101),Omega=new DoubleRange(0.0, 1, 21)}, // GHz
                    new ROfRhoAndTimeDetectorInput() {Rho= new DoubleRange(0.0, 10, 101),Time=new DoubleRange(0.0, 10, 101)},       
                    new ROfRhoDetectorInput() {Rho =new DoubleRange(0.0, 10, 101)},
                    new ROfXAndYDetectorInput() {X=new DoubleRange(-100.0, 100.0, 21), Y= new DoubleRange(-100.0, 100.0, 21)}, 
                    new RSpecularDetectorInput(),
                    new TDiffuseDetectorInput(),
                    new TOfAngleDetectorInput() {Angle=new DoubleRange(0.0, Math.PI / 2, 5)},
                    new TOfRhoAndAngleDetectorInput() {Rho=new DoubleRange(0.0, 10, 101),Angle=new DoubleRange(0.0, Math.PI / 2, 5)},
                    new TOfRhoDetectorInput() {Rho=new DoubleRange(0.0, 10, 101)},
                    new TOfXAndYDetectorInput() {X=new DoubleRange(-100.0, 100.0, 21), Y= new DoubleRange(-100.0, 100.0, 21)}, 
                }
            );
        }
        #endregion

        #region point source one layer R(rho) and Fluence(rho) (for lab exercises)
        /// <summary>
        /// Point source, single tissue layer definition, only ROfRho detector included
        /// </summary>
        public static SimulationInput PointSourceOneLayerTissueROfRhoAndFluenceOfRhoAndZDetectors()
        {
            MultiLayerTissueInput ti = new MultiLayerTissueInput(
                    new ITissueRegion[]
                    { 
                        new LayerTissueRegion(
                            new DoubleRange(double.NegativeInfinity, 0.0),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0),
                        "HenyeyGreensteinKey1"),
                        new LayerTissueRegion(
                            new DoubleRange(0.0, 100.0),
                            new OpticalProperties(0.01, 1.0, 0.8, 1.4),
                        "HenyeyGreensteinKey2"),
                        new LayerTissueRegion(
                            new DoubleRange(100.0, double.PositiveInfinity),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0),
                        "HenyeyGreensteinKey3")
                    }
                );
            ti.RegionPhaseFunctionInputs.Add("HenyeyGreensteinKey1", new HenyeyGreensteinPhaseFunctionInput());
            ti.RegionPhaseFunctionInputs.Add("HenyeyGreensteinKey2", new HenyeyGreensteinPhaseFunctionInput());
            ti.RegionPhaseFunctionInputs.Add("HenyeyGreensteinKey3", new HenyeyGreensteinPhaseFunctionInput());
            return new SimulationInput(
                100,
                "one_layer_ROfRho_FluenceOfRhoAndZ",
                new SimulationOptions(
                    0, // random number generator seed, -1=random seed, 0=fixed seed
                    RandomNumberGeneratorType.MersenneTwister,
                    AbsorptionWeightingType.Discrete,
                    new List<DatabaseType>() { }, // databases to be written
                    false, // track statistics
                    0.0, // RR threshold -> no RR performed
                    0),
                new DirectionalPointSourceInput(
                    new Position(0.0, 0.0, 0.0),
                    new Direction(0.0, 0.0, 1.0),
                    0), // 0=start in air, 1=start in tissue
                ti,
                new List<IDetectorInput>()
                {
                    new ROfRhoDetectorInput() { Rho =new DoubleRange(0.0, 10, 101)},
                    new FluenceOfRhoAndZDetectorInput() {Rho=new DoubleRange(0.0, 10, 101),Z=new DoubleRange(0.0, 10, 101)}
                }
             );
        }
        #endregion

        #region point source one layer Fluence(rho, z) and Radiance(rho, z, angle) (for lab exercises)
        // THIS IS USED FOR THE VP LABS, PLEASE DO NOT DELETE
        /// <summary>
        /// Point source, single tissue layer definition, Radiance included
        /// </summary>
        public static SimulationInput PointSourceOneLayerTissueRadianceOfRhoAndZAndAngleDetector()
        {
            MultiLayerTissueInput ti = new MultiLayerTissueInput(
                    new ITissueRegion[]
                    { 
                        new LayerTissueRegion(
                            new DoubleRange(double.NegativeInfinity, 0.0),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0),
                        "HenyeyGreensteinKey1"),
                        new LayerTissueRegion(
                            new DoubleRange(0.0, 100.0),
                            new OpticalProperties(0.01, 1.0, 0.8, 1.4),
                        "HenyeyGreensteinKey2"),
                        new LayerTissueRegion(
                            new DoubleRange(100.0, double.PositiveInfinity),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0),
                        "HenyeyGreensteinKey3")
                    }
                );
            ti.RegionPhaseFunctionInputs.Add("HenyeyGreensteinKey1", new HenyeyGreensteinPhaseFunctionInput());
            ti.RegionPhaseFunctionInputs.Add("HenyeyGreensteinKey2", new HenyeyGreensteinPhaseFunctionInput());
            ti.RegionPhaseFunctionInputs.Add("HenyeyGreensteinKey3", new HenyeyGreensteinPhaseFunctionInput());
            return new SimulationInput(
                100,  // set to 10000 for lab exercises
                "one_layer_FluenceOfRhoAndZ_RadianceOfRhoAndZAndAngle",
                new SimulationOptions(
                    0, // random number generator seed, -1=random seed, 0=fixed seed
                    RandomNumberGeneratorType.MersenneTwister,
                    AbsorptionWeightingType.Discrete,
                    new List<DatabaseType>() { }, // databases to be written
                    false, // track statistics
                    0.0, // RR threshold -> no RR performed
                    0),
                new DirectionalPointSourceInput(
                    new Position(0.0, 0.0, 0.0),
                    new Direction(0.0, 0.0, 1.0),
                    0), // 0=start in air, 1=start in tissue
                ti,
                new List<IDetectorInput>()
                {
                    new FluenceOfRhoAndZDetectorInput() {Rho=new DoubleRange(0.0, 10, 101),Z= new DoubleRange(0.0, 10, 101)},
                    new RadianceOfRhoAndZAndAngleDetectorInput()
                    {
                        Rho=new DoubleRange(0.0, 10, 101),
                        Z=new DoubleRange(0.0, 10, 101),
                        Angle= new DoubleRange(0, Math.PI, 3)
                    }
                }
             );
        }
        #endregion

        #region point source two layer R(rho) (for lab exercises)
        /// <summary>
        /// Point source, two-layer tissue definition, only ROfRho detector included
        /// </summary>
        public static SimulationInput PointSourceTwoLayerTissueROfRhoDetector()
        {
            MultiLayerTissueInput ti = new MultiLayerTissueInput(
                    new ITissueRegion[]
                    { 
                        new LayerTissueRegion(
                            new DoubleRange(double.NegativeInfinity, 0.0),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0),
                        "HenyeyGreensteinKey1"),
                        new LayerTissueRegion(
                            new DoubleRange(0.0, 1.5),
                            new OpticalProperties(0.01, 1.0, 0.8, 1.4),
                        "HenyeyGreensteinKey2"),
                        new LayerTissueRegion(
                            new DoubleRange(1.5, 100.0),
                            new OpticalProperties(0.01, 1.0, 0.8, 1.4),
                        "HenyeyGreensteinKey3"),
                        new LayerTissueRegion(
                            new DoubleRange(100.0, double.PositiveInfinity),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0),
                        "HenyeyGreensteinKey4")
                    }
                );
            ti.RegionPhaseFunctionInputs.Add("HenyeyGreensteinKey1", new HenyeyGreensteinPhaseFunctionInput());
            ti.RegionPhaseFunctionInputs.Add("HenyeyGreensteinKey2", new HenyeyGreensteinPhaseFunctionInput());
            ti.RegionPhaseFunctionInputs.Add("HenyeyGreensteinKey3", new HenyeyGreensteinPhaseFunctionInput());
            ti.RegionPhaseFunctionInputs.Add("HenyeyGreensteinKey4", new HenyeyGreensteinPhaseFunctionInput());
            return new SimulationInput(
                100,
                "two_layer_ROfRho",
                new SimulationOptions(
                    0, // random number generator seed, -1=random seed, 0=fixed seed
                    RandomNumberGeneratorType.MersenneTwister,
                    AbsorptionWeightingType.Discrete,
                    new List<DatabaseType>() { }, // databases to be written
                    false, // track statistics
                    0.0, // RR threshold -> no RR performed
                    0),
                new DirectionalPointSourceInput(
                    new Position(0.0, 0.0, 0.0),
                    new Direction(0.0, 0.0, 1.0),
                    0), // 0=start in air, 1=start in tissue
                ti,
                new List<IDetectorInput>()
                {
                    new ROfRhoDetectorInput
                    {
                        Rho =new DoubleRange(0.0, 10, 101)
                    },
                }
            );
        }
        #endregion

        #region point source two layer R(rho) with photon database
        /// <summary>
        /// Point source, two-layer tissue definition, only ROfRho detector included
        /// </summary>
        public static SimulationInput PointSourceTwoLayerTissueROfRhoDetectorWithPhotonDatabase()
        {
            MultiLayerTissueInput ti = new MultiLayerTissueInput(
                    new ITissueRegion[]
                    { 
                        new LayerTissueRegion(
                            new DoubleRange(double.NegativeInfinity, 0.0),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0),
                        "HenyeyGreensteinKey1"),
                        new LayerTissueRegion(
                            new DoubleRange(0.0, 1.5),
                            new OpticalProperties(0.01, 1.0, 0.8, 1.4),
                        "HenyeyGreensteinKey2"),
                        new LayerTissueRegion(
                            new DoubleRange(1.5, 100.0),
                            new OpticalProperties(0.01, 1.0, 0.8, 1.4),
                        "HenyeyGreensteinKey3"),
                        new LayerTissueRegion(
                            new DoubleRange(100.0, double.PositiveInfinity),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0),
                        "HenyeyGreensteinKey4")
                    }
                );
            ti.RegionPhaseFunctionInputs.Add("HenyeyGreensteinKey1", new HenyeyGreensteinPhaseFunctionInput());
            ti.RegionPhaseFunctionInputs.Add("HenyeyGreensteinKey2", new HenyeyGreensteinPhaseFunctionInput());
            ti.RegionPhaseFunctionInputs.Add("HenyeyGreensteinKey3", new HenyeyGreensteinPhaseFunctionInput());
            ti.RegionPhaseFunctionInputs.Add("HenyeyGreensteinKey4", new HenyeyGreensteinPhaseFunctionInput());
            return new SimulationInput(
                100,
                "two_layer_ROfRho_with_db",
                new SimulationOptions(
                    0, // random number generator seed, -1=random seed, 0=fixed seed
                    RandomNumberGeneratorType.MersenneTwister,
                    AbsorptionWeightingType.Discrete,
                    new[] { DatabaseType.DiffuseReflectance }, // databases to be written
                    false, // track statistics
                    0.0, // RR threshold -> no RR performed
                    0),
                new DirectionalPointSourceInput(
                    new Position(0.0, 0.0, 0.0),
                    new Direction(0.0, 0.0, 1.0),
                    0), // 0=start in air, 1=start in tissue
                ti,
                new List<IDetectorInput>()
                {
                    new ROfRhoDetectorInput
                    {
                        Rho =new DoubleRange(0.0, 10, 101)
                    },
                }
            );
        }
        #endregion

        #region point source single ellipsoid Fluence(rho)
        /// <summary>
        /// Point source, single ellipsoid tissue definition, only ROfRho detector included
        /// </summary>
        public static SimulationInput PointSourceSingleEllipsoidTissueFluenceOfRhoAndZDetector()
        {
            SingleEllipsoidTissueInput ti = new SingleEllipsoidTissueInput(
                    new EllipsoidTissueRegion(
                        new Position(0, 0, 1),
                        0.5,
                        0.5,
                        0.5,
                        new OpticalProperties(0.05, 1.0, 0.8, 1.4),
                        "HenyeyGreensteinKey1"
                    ),
                    new ITissueRegion[]
                    { 
                        new LayerTissueRegion(
                            new DoubleRange(double.NegativeInfinity, 0.0),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0),
                        "HenyeyGreensteinKey2"),
                        new LayerTissueRegion(
                            new DoubleRange(0.0, 100.0),
                            new OpticalProperties(0.01, 1.0, 0.8, 1.4),
                        "HenyeyGreensteinKey3"),
                        new LayerTissueRegion(
                            new DoubleRange(100.0, double.PositiveInfinity),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0),
                        "HenyeyGreensteinKey4")
                    }
                );
            ti.RegionPhaseFunctionInputs.Add("HenyeyGreensteinKey1", new HenyeyGreensteinPhaseFunctionInput());
            ti.RegionPhaseFunctionInputs.Add("HenyeyGreensteinKey2", new HenyeyGreensteinPhaseFunctionInput());
            ti.RegionPhaseFunctionInputs.Add("HenyeyGreensteinKey3", new HenyeyGreensteinPhaseFunctionInput());
            ti.RegionPhaseFunctionInputs.Add("HenyeyGreensteinKey4", new HenyeyGreensteinPhaseFunctionInput());
            return new SimulationInput(
                100,
                "ellip_FluenceOfRhoAndZ",
                new SimulationOptions(
                    0, // random number generator seed, -1=random seed, 0=fixed seed
                    RandomNumberGeneratorType.MersenneTwister,
                    AbsorptionWeightingType.Discrete,
                    new List<DatabaseType>() { }, // databases to be written
                    false, // track statistics
                    0.0, // RR threshold -> no RR performed
                    0),
                new DirectionalPointSourceInput(
                    new Position(0.0, 0.0, 0.0),
                    new Direction(0.0, 0.0, 1.0),
                    0), // 0=start in air, 1=start in tissue
                ti,
                new List<IDetectorInput>()
                {
                    new FluenceOfRhoAndZDetectorInput( )                           
                        {Rho = new DoubleRange(0.0, 10, 101),
                        Z = new DoubleRange(0.0, 10, 101)}
                }
            );
        }
        #endregion

        #region point source one layer LUT Phase function R(rho)
        /// <summary>
        /// Point source, one layer tissue definition with LUT phase function, only ROfRho detector included
        /// </summary>
        public static SimulationInput PointSourceOneLayerTissueLUTPhaseFunctionROfRhoDetector()
        {
            MultiLayerTissueInput ti = new MultiLayerTissueInput(
                    new ITissueRegion[]
                    { 
                        new LayerTissueRegion(
                            new DoubleRange(double.NegativeInfinity, 0.0),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0),
                        "HenyeyGreensteinKey1"),
                        new LayerTissueRegion(
                            new DoubleRange(0.0, 100.0),
                            new OpticalProperties(0.01, 1.0, 0.8, 1.4),
                        "LUT1"),
                        new LayerTissueRegion(
                            new DoubleRange(100.0, double.PositiveInfinity),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0),
                        "HenyeyGreensteinKey2")
                    }
                );
            ti.RegionPhaseFunctionInputs.Add("HenyeyGreensteinKey1", new HenyeyGreensteinPhaseFunctionInput());
            ti.RegionPhaseFunctionInputs.Add("LUT1", new LookupTablePhaseFunctionInput(
                //new PolarLookupTablePhaseFunctionData
                //{
                //    LutAngles = new[] { 0, Math.PI/6, Math.PI/3, Math.PI/2, 2*Math.PI/3, Math.PI*5/6, Math.PI },
                //    LutPdf = new[] { 0.5, 0.5, 0.5, 0.5, 0.5, 0.5, 0.5 },
                //    LutCdf = new[] { 0, 0.5 * (1 - Math.Sqrt(3) / 2), 0.25 , 0.5, 0.75 , 0.5 * (1 + Math.Sqrt(3) / 2), 1},
                //}
            ));
            ti.RegionPhaseFunctionInputs.Add("HenyeyGreensteinKey2", new HenyeyGreensteinPhaseFunctionInput());
            return new SimulationInput(
                100,
                "one_layer_LUT_ROfRho",
                new SimulationOptions(
                    0, // random number generator seed, -1=random seed, 0=fixed seed
                    RandomNumberGeneratorType.MersenneTwister,
                    AbsorptionWeightingType.Discrete, 
                    new List<DatabaseType>() { }, // databases to be written
                    false, // track statistics
                    0.0, // RR threshold -> no RR performed
                    0),
                new DirectionalPointSourceInput(
                    new Position(0.0, 0.0, 0.0),
                    new Direction(0.0, 0.0, 1.0),
                    0), // 0=start in air, 1=start in tissue
                ti,
                new List<IDetectorInput>()
                {
                    new ROfRhoDetectorInput { Rho =new DoubleRange(0.0, 10, 101) },
                }
            );
        }
        #endregion

        #region pMC point source one layer tissue R(rho) DAW
        /// <summary>
        /// Perturbation MC point source, single tissue layer definition, R(rho) included
        /// </summary>
        public static SimulationInput pMCPointSourceOneLayerTissueROfRhoDAW()
        {
            MultiLayerTissueInput ti = new MultiLayerTissueInput(
                    new ITissueRegion[]
                    { 
                        new LayerTissueRegion(
                            new DoubleRange(double.NegativeInfinity, 0.0),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0),
                        "HenyeyGreensteinKey1"),
                        new LayerTissueRegion(
                            new DoubleRange(0.0, 100.0),
                            new OpticalProperties(0.01, 1.0, 0.8, 1.4),
                        "HenyeyGreensteinKey2"),
                        new LayerTissueRegion(
                            new DoubleRange(100.0, double.PositiveInfinity),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0),
                        "HenyeyGreensteinKey3")
                    }
                );
            ti.RegionPhaseFunctionInputs.Add("HenyeyGreensteinKey1", new HenyeyGreensteinPhaseFunctionInput());
            ti.RegionPhaseFunctionInputs.Add("HenyeyGreensteinKey2", new HenyeyGreensteinPhaseFunctionInput());
            ti.RegionPhaseFunctionInputs.Add("HenyeyGreensteinKey3", new HenyeyGreensteinPhaseFunctionInput());
            return new SimulationInput(
                100,
                "pMC_one_layer_ROfRho_DAW",
                new SimulationOptions(
                    0, // random number generator seed, -1=random seed, 0=fixed seed
                    RandomNumberGeneratorType.MersenneTwister,
                    AbsorptionWeightingType.Discrete,
                //new HenyeyGreensteinPhaseFunctionInput(),
                    new List<DatabaseType>() { DatabaseType.pMCDiffuseReflectance }, // databases to be written
                    false, // track statistics
                    0.0, // RR threshold -> 0 = no RR performed
                    0),
                new DirectionalPointSourceInput(
                    new Position(0.0, 0.0, 0.0),
                    new Direction(0.0, 0.0, 1.0),
                    0), // 0=start in air, 1=start in tissue
                ti,
                new List<IDetectorInput>()
                {
                    new ROfRhoDetectorInput
                    {
                        Rho =new DoubleRange(0.0, 10, 101)
                    },
                }
            );
        }
        #endregion

        #region Gaussian source one layer R(rho)
        /// <summary>
        /// Gaussian source, single tissue layer definition, only ROfRho detector included
        /// </summary>
        public static SimulationInput GaussianSourceOneLayerTissueROfRhoDetector()
        {
            MultiLayerTissueInput ti = new MultiLayerTissueInput(
                    new ITissueRegion[]
                    { 
                        new LayerTissueRegion(
                            new DoubleRange(double.NegativeInfinity, 0.0),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0),
                        "HenyeyGreensteinKey1"),
                        new LayerTissueRegion(
                            new DoubleRange(0.0, 100.0),
                            new OpticalProperties(0.01, 1.0, 0.8, 1.4),
                        "HenyeyGreensteinKey2"),
                        new LayerTissueRegion(
                            new DoubleRange(100.0, double.PositiveInfinity),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0),
                        "HenyeyGreensteinKey3")
                    }
                );
            ti.RegionPhaseFunctionInputs.Add("HenyeyGreensteinKey1", new HenyeyGreensteinPhaseFunctionInput());
            ti.RegionPhaseFunctionInputs.Add("HenyeyGreensteinKey2", new HenyeyGreensteinPhaseFunctionInput());
            ti.RegionPhaseFunctionInputs.Add("HenyeyGreensteinKey3", new HenyeyGreensteinPhaseFunctionInput());
            return new SimulationInput(
                100,
                "Gaussian_source_one_layer_ROfRho",
                new SimulationOptions(
                    0, // random number generator seed, -1=random seed, 0=fixed seed
                    RandomNumberGeneratorType.MersenneTwister,
                    AbsorptionWeightingType.Discrete,
                //new HenyeyGreensteinPhaseFunctionInput(),
                    new List<DatabaseType>() { }, // databases to be written
                    false, // track statistics
                    0.0, // RR threshold -> 0 = no RR performed
                    0),
                new CustomCircularSourceInput(
                    3.0, // outer radius
                    0.0, // inner radius
                    new GaussianSourceProfile(1.0), // fwhm
                    new DoubleRange(0.0, 0.0), // polar angle emission range
                    new DoubleRange(0.0, 0.0), // azimuthal angle emmision range
                    new Direction(0, 0, 1), // normal to tissue
                    new Position(0, 0, 0), // center of beam on surface
                    new PolarAzimuthalAngles(0, 0), // no beam rotation         
                    0), // 0=start in air, 1=start in tissue
                ti,
                new List<IDetectorInput>()
                {
                    new ROfRhoDetectorInput
                    {
                        Rho =new DoubleRange(0.0, 10, 101)
                    },
                }
             );
        }
        #endregion

        #region Flat source one layer R(rho)
        /// <summary>
        /// Flat source, single tissue layer definition, only ROfRho detector included
        /// </summary>
        public static SimulationInput FlatSourceOneLayerTissueROfRhoDetector()
        {
            var ti = new MultiLayerTissueInput(
                new ITissueRegion[]
                {
                    new LayerTissueRegion(
                        new DoubleRange(double.NegativeInfinity, 0.0),
                        new OpticalProperties(0.0, 1e-10, 1.0, 1.0),
                        "HenyeyGreensteinKey1"),
                    new LayerTissueRegion(
                        new DoubleRange(0.0, 100.0),
                        new OpticalProperties(0.01, 1.0, 0.8, 1.4),
                        "HenyeyGreensteinKey2"),
                    new LayerTissueRegion(
                        new DoubleRange(100.0, double.PositiveInfinity),
                        new OpticalProperties(0.0, 1e-10, 1.0, 1.0),
                        "HenyeyGreensteinKey3")
                }
                );
            ti.RegionPhaseFunctionInputs.Add("HenyeyGreensteinKey1", new HenyeyGreensteinPhaseFunctionInput());
            ti.RegionPhaseFunctionInputs.Add("HenyeyGreensteinKey2", new HenyeyGreensteinPhaseFunctionInput());
            ti.RegionPhaseFunctionInputs.Add("HenyeyGreensteinKey3", new HenyeyGreensteinPhaseFunctionInput());
            return new SimulationInput(
                100,
                "Flat_source_one_layer_ROfRho",
                new SimulationOptions(
                    0, // random number generator seed, -1=random seed, 0=fixed seed
                    RandomNumberGeneratorType.MersenneTwister,
                    AbsorptionWeightingType.Discrete,
                    //PhaseFunctionType.HenyeyGreenstein,
                    new List<DatabaseType>() { }, // databases to be written
                    false, // track statistics
                    0.0, // RR threshold -> 0 = no RR performed
                    0),
                new CustomCircularSourceInput(
                    3.0, // outer radius
                    0.0, // inner radius
                    new FlatSourceProfile(), 
                    new DoubleRange(0.0, 0.0), // polar angle emission range
                    new DoubleRange(0.0, 0.0), // azimuthal angle emmision range
                    new Direction(0, 0, 1), // normal to tissue
                    new Position(0, 0, 0), // center of beam on surface
                    new PolarAzimuthalAngles(0, 0), // no beam rotation         
                    0), // 0=start in air, 1=start in tissue
                ti,
                new List<IDetectorInput>()
                {
                    new ROfRhoDetectorInput
                    {
                        Rho =new DoubleRange(0.0, 10, 101)
                    },
                }
             );
        }
        #endregion

        #region point source multilayer momentum transfer
        /// <summary>
        /// Point source, multi-layer tissue definition, all momentum detectors detectors included
        /// </summary>
        public static SimulationInput PointSourceMultiLayerMomentumTransferDetectors()
        {
            MultiLayerTissueInput ti = new MultiLayerTissueInput(
                    new ITissueRegion[]
                    { 
                        new LayerTissueRegion(
                            new DoubleRange(double.NegativeInfinity, 0.0),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0),
                        "HenyeyGreensteinKey1"),
                        new LayerTissueRegion(
                            new DoubleRange(0.0, 1.0), // upper layer 1mm
                            new OpticalProperties(0.01, 1.0, 0.7, 1.33),
                        "HenyeyGreensteinKey2"), // Tyler's data
                        new LayerTissueRegion(
                            new DoubleRange(1.0, 100.0),
                            new OpticalProperties(0.01, 1.0, 0.7, 1.33),
                        "HenyeyGreensteinKey3"), 
                        new LayerTissueRegion(
                            new DoubleRange(100.0, double.PositiveInfinity),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0),
                        "HenyeyGreensteinKey4")
                    }
                );
            ti.RegionPhaseFunctionInputs.Add("HenyeyGreensteinKey1", new HenyeyGreensteinPhaseFunctionInput());
            ti.RegionPhaseFunctionInputs.Add("HenyeyGreensteinKey2", new HenyeyGreensteinPhaseFunctionInput());
            ti.RegionPhaseFunctionInputs.Add("HenyeyGreensteinKey3", new HenyeyGreensteinPhaseFunctionInput());
            ti.RegionPhaseFunctionInputs.Add("HenyeyGreensteinKey4", new HenyeyGreensteinPhaseFunctionInput());
            return new SimulationInput(
                100,
                "two_layer_momentum_transfer_detectors",
                new SimulationOptions(
                    0, // random number generator seed, -1=random seed, 0=fixed seed
                    RandomNumberGeneratorType.MersenneTwister,
                    AbsorptionWeightingType.Continuous,
                //new HenyeyGreensteinPhaseFunctionInput(),
                    new List<DatabaseType>() { }, // databases to be written
                    true, // track statistics
                    0.00, // RR threshold -> no RR performed
                    0),
                new DirectionalPointSourceInput(
                    new Position(0.0, 0.0, 0.0),
                    new Direction(0.0, 0.0, 1.0),
                    0), // 0=start in air, 1=start in tissue, start in tissue so no MT tally at tissue crossing in air
                ti,
                new List<IDetectorInput>()
                {
                    // detectors with cylindrical symmetry
                    new ROfRhoDetectorInput{Rho =new DoubleRange(0.0, 10, 101) },
                    new ReflectedMTOfRhoAndSubregionHistDetectorInput(){
                        Rho=new DoubleRange(0.0, 10.0, 101), // rho bins
                        MTBins=new DoubleRange(0.0, 500.0, 51), // MT bins
                        FractionalMTBins = new DoubleRange(0.0, 1.0, 11)}, // fractional MT bins
                    new TOfRhoDetectorInput {Rho =new DoubleRange(0.0, 10, 101)},
                    new TransmittedMTOfRhoAndSubregionHistDetectorInput(){
                        Rho=new DoubleRange(0.0, 10.0, 101), // rho bins
                        MTBins=new DoubleRange(0.0, 500.0, 51), // MT bins
                        FractionalMTBins = new DoubleRange(0.0, 1.0, 11)}, // fractional MT bins
                    // detectors with Cartesian coordinates                      
                    new ROfXAndYDetectorInput() {X=new DoubleRange(-100.0, 100.0, 21), Y= new DoubleRange(-100.0, 100.0, 21)},
                    new ReflectedMTOfXAndYAndSubregionHistDetectorInput(){
                        X=new DoubleRange(-100.0, 100.0, 21), 
                        Y= new DoubleRange(-100.0, 100.0, 21),
                        MTBins=new DoubleRange(0.0, 500.0, 51), // MT bins
                        FractionalMTBins = new DoubleRange(0.0, 1.0, 11)}, // fractional MT bins
                    new TOfXAndYDetectorInput() {X=new DoubleRange(-100.0, 100.0, 21), Y= new DoubleRange(-100.0, 100.0, 21)}, 
                    new TransmittedMTOfXAndYAndSubregionHistDetectorInput(){
                        X=new DoubleRange(-100.0, 100.0, 21), 
                        Y= new DoubleRange(-100.0, 100.0, 21),
                        MTBins=new DoubleRange(0.0, 500.0, 51), // MT bins
                        FractionalMTBins = new DoubleRange(0.0, 1.0, 11)}, // fractional MT bins
                    // DYNAMIC MT detectors
                    // detectors with cylindrical symmetry
                    new ReflectedDynamicMTOfRhoAndSubregionHistDetectorInput(){
                        Rho=new DoubleRange(0.0, 10.0, 101), // rho bins                
                        Z= new DoubleRange(0.0, 10.0, 11),
                        MTBins=new DoubleRange(0.0, 500.0, 51), // MT bins
                        FractionalMTBins = new DoubleRange(0.0, 1.0, 11), // fractional MT bins                        
                        BloodVolumeFraction = new List<double>() { 0, 0.5, 0.5, 0 },
                        TallySecondMoment = true},
                    new TransmittedDynamicMTOfRhoAndSubregionHistDetectorInput(){
                        Rho=new DoubleRange(0.0, 10.0, 101), // rho bins                
                        Z= new DoubleRange(0.0, 10.0, 11),
                        MTBins=new DoubleRange(0.0, 500.0, 51), // MT bins
                        FractionalMTBins = new DoubleRange(0.0, 1.0, 11), // fractional MT bins
                        BloodVolumeFraction = new List<double>() { 0, 0.5, 0.5, 0 },
                        TallySecondMoment = true},
                    // detectors with Cartesian coordinates                      
                    new ReflectedDynamicMTOfXAndYAndSubregionHistDetectorInput(){
                        X=new DoubleRange(-10.0, 10.0, 21), 
                        Y= new DoubleRange(-10.0, 10.0, 21),                 
                        Z= new DoubleRange(0.0, 10.0, 11),
                        MTBins=new DoubleRange(0.0, 500.0, 51), // MT bins
                        FractionalMTBins = new DoubleRange(0.0, 1.0, 11), // fractional MT bins
                        BloodVolumeFraction = new List<double>() { 0, 0.5, 0.5, 0 },
                        TallySecondMoment = true},
                    new TransmittedDynamicMTOfXAndYAndSubregionHistDetectorInput(){
                        X=new DoubleRange(-10.0, 10.0, 21), 
                        Y= new DoubleRange(-10.0, 10.0, 21),             
                        Z= new DoubleRange(0.0, 10.0, 11),
                        MTBins=new DoubleRange(0.0, 500.0, 51), // MT bins
                        FractionalMTBins = new DoubleRange(0.0, 1.0, 11), // fractional MT bins
                        BloodVolumeFraction = new List<double>() { 0, 0.5, 0.5, 0 },
                        TallySecondMoment = true},
                }
            );
        }
        #endregion

        #region point source single voxel Fluence(x,y,z)
        /// <summary>
        /// Point source, single voxel tissue definition, only FluenceOfXAndYAndZ detector included
        /// </summary>
        public static SimulationInput PointSourceSingleVoxelTissueROfXAndYAndFluenceOfXAndYAndZDetector()
        {
            var ti = new SingleVoxelTissueInput(
                new VoxelTissueRegion(
                    new DoubleRange(-1, 1),
                    new DoubleRange(-1, 1),
                    new DoubleRange(1, 2),
                    new OpticalProperties(0.05, 1.0, 0.8, 1.4),
                    "HenyeyGreensteinKey1"
                    ),
                new ITissueRegion[]
                {
                    new LayerTissueRegion(
                        new DoubleRange(double.NegativeInfinity, 0.0),
                        new OpticalProperties(0.0, 1e-10, 1.0, 1.0),
                        "HenyeyGreensteinKey2"),
                    new LayerTissueRegion(
                        new DoubleRange(0.0, 100.0),
                        new OpticalProperties(0.01, 1.0, 0.8, 1.4),
                        "HenyeyGreensteinKey3"),
                    new LayerTissueRegion(
                        new DoubleRange(100.0, double.PositiveInfinity),
                        new OpticalProperties(0.0, 1e-10, 1.0, 1.0),
                        "HenyeyGreensteinKey4")
                }
                );
            ti.RegionPhaseFunctionInputs.Add("HenyeyGreensteinKey1", new HenyeyGreensteinPhaseFunctionInput());
            ti.RegionPhaseFunctionInputs.Add("HenyeyGreensteinKey2", new HenyeyGreensteinPhaseFunctionInput());
            ti.RegionPhaseFunctionInputs.Add("HenyeyGreensteinKey3", new HenyeyGreensteinPhaseFunctionInput());
            ti.RegionPhaseFunctionInputs.Add("HenyeyGreensteinKey4", new HenyeyGreensteinPhaseFunctionInput());
 
            return new SimulationInput(
                100,
                "voxel_ROfXAndY_FluenceOfXAndYAndZ",
                new SimulationOptions(
                    0, // random number generator seed, -1=random seed, 0=fixed seed
                    RandomNumberGeneratorType.MersenneTwister,
                    AbsorptionWeightingType.Discrete,
                    new List<DatabaseType>() { }, // databases to be written
                    false, // track statistics
                    0.0, // RR threshold -> no RR performed
                    0),
                new DirectionalPointSourceInput(
                    new Position(0.0, 0.0, 0.0),
                    new Direction(0.0, 0.0, 1.0),
                    0), // 0=start in air, 1=start in tissue
                ti,
                new List<IDetectorInput>()
                { 
                    new ROfXAndYDetectorInput() {X=new DoubleRange(-100.0, 100.0, 21), Y= new DoubleRange(-100.0, 100.0, 21)}, 
                    new FluenceOfXAndYAndZDetectorInput(){X=new DoubleRange(-10, 10, 101),
                        Y=new DoubleRange(-10, 10, 101), Z= new DoubleRange(0.0, 10, 101)}
                }
            );
        }
        #endregion

        #region point source three layer SubRegion Time
        /// <summary>
        /// Point source, three-layer tissue definition, with R(rho,time) and
        /// ReflectedTimeOfRhoAndSubregionHistDetector detector included
        /// </summary>
        public static SimulationInput PointSourceThreeLayerReflectedTimeOfRhoAndSubregionHistDetector()
        {
            var ti = new MultiLayerTissueInput(
                new ITissueRegion[]
                {
                    new LayerTissueRegion(
                        new DoubleRange(double.NegativeInfinity, 0.0),
                        new OpticalProperties(0.0, 1e-10, 1.0, 1.0),
                        "HenyeyGreensteinKey1"),
                    new LayerTissueRegion(
                        new DoubleRange(0.0, 5.0),
                        new OpticalProperties(0.01, 1.0, 0.9, 1.4),
                        "HenyeyGreensteinKey2"),
                    new LayerTissueRegion(
                        new DoubleRange(5.0, 10.0),
                        new OpticalProperties(0.01, 1.0, 0.9, 1.4),
                        "HenyeyGreensteinKey3"),
                    new LayerTissueRegion(
                        new DoubleRange(10.0, 100.0),
                        new OpticalProperties(0.01, 1.0, 0.9, 1.4),
                        "HenyeyGreensteinKey4"),
                    new LayerTissueRegion(
                        new DoubleRange(100.0, double.PositiveInfinity),
                        new OpticalProperties(0.0, 1e-10, 1.0, 1.0),
                        "HenyeyGreensteinKey5")
                }
                );
            ti.RegionPhaseFunctionInputs.Add("HenyeyGreensteinKey1", new HenyeyGreensteinPhaseFunctionInput());
            ti.RegionPhaseFunctionInputs.Add("HenyeyGreensteinKey2", new HenyeyGreensteinPhaseFunctionInput());
            ti.RegionPhaseFunctionInputs.Add("HenyeyGreensteinKey3", new HenyeyGreensteinPhaseFunctionInput());
            ti.RegionPhaseFunctionInputs.Add("HenyeyGreensteinKey4", new HenyeyGreensteinPhaseFunctionInput());
            ti.RegionPhaseFunctionInputs.Add("HenyeyGreensteinKey5", new HenyeyGreensteinPhaseFunctionInput());
            return new SimulationInput(
                100,
                "three_layer_ReflectedTimeOfRhoAndSubregionHist",
                new SimulationOptions(
                    0, // random number generator seed, -1=random seed, 0=fixed seed
                    RandomNumberGeneratorType.MersenneTwister,
                    AbsorptionWeightingType.Continuous,
                //PhaseFunctionType.HenyeyGreenstein,
                    new List<DatabaseType>() { }, // databases to be written
                    true, // track statistics
                    0.0, // RR threshold -> no RR performed
                    0),
                new DirectionalPointSourceInput(
                    new Position(0.0, 0.0, 0.0),
                    new Direction(0.0, 0.0, 1.0),
                    0), // 0=start in air, 1=start in tissue, start in tissue so no MT tally at tissue crossing in air
                ti,
                new List<IDetectorInput>()
                {
                    new ROfRhoAndTimeDetectorInput(){
                          Rho=new DoubleRange(0.0, 10.0, 21), // rho bins
                          Time=new DoubleRange(0.0, 1.0, 11)},  // time bins
                    new ReflectedTimeOfRhoAndSubregionHistDetectorInput(){
                          Rho=new DoubleRange(0.0, 10.0, 21), // rho bins
                          Time=new DoubleRange(0.0, 1.0, 11)} // time bins
                }
            );
        }
        #endregion
    }
}

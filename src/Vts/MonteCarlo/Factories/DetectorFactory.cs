using System.Collections.Generic;
using System.Linq;
using Vts.MonteCarlo.Detectors;

namespace Vts.MonteCarlo.Factories
{
    /// <summary>
    /// Instantiates appropriate detector tally given TallyType.
    /// </summary>
    public class DetectorFactory
    {
        //private static Dictionary<string, IProvider<IDetector>> _providers;

        //// todo: look up ways of using MEF to register 3rd party plug-ins at runtime
        //public static void RegisterProvider(IProvider<IDetector> provider)
        //{
        //    var typeString = provider.TargetType.ToString();
        //    if(_providers.ContainsKey(typeString))
        //    {
        //        _providers.Remove(typeString);
        //    }
        //    _providers.Add(typeString, provider);
        //}
        
        //public static void RegisterProviders(IEnumerable<IProvider<IDetector>> providers)
        //{
        //    foreach (var provider in providers)
        //    {
        //        RegisterProvider(provider);
        //    }
        //}

        /// <summary>
        /// Method to instantiate all detectors given list of IDetectorInputs.  This method calls
        /// the method below that instantiates a single detector.
        /// </summary>
        /// <param name="detectorInputs">IEnumerable of IDetectorInput</param>
        /// <param name="tissue">ITissue</param>
        /// <param name="tallySecondMoment">flag indicating whether to tally second moment or not</param>
        /// <returns>List of IDetector</returns>
        public static IList<IDetector> GetDetectors(IEnumerable<IDetectorInput> detectorInputs, ITissue tissue)
        {
            if (detectorInputs == null)
            {
                return null;
            }
            
            var detectors = detectorInputs.Select(detectorInput => GetDetector(detectorInput, tissue)).ToList();

            return detectors;
        }
        
        /// <summary>
        /// Method to instantiate all detectors given list of IDetectorInputs.  This method calls
        /// the method below that instantiates a single detector.
        /// </summary>
        /// <param name="detectorInputs">IEnumerable of IDetectorInput</param>
        /// <param name="tissue">ITissue</param>
        /// <param name="tallySecondMoment">flag indicating whether to tally second moment or not</param>
        /// <returns>List of IDetector</returns>
        public static IDetector GetDetector(IDetectorInput detectorInput, ITissue tissue)
        {
            if (detectorInput == null)
            {
                return null;
            }

            var detector = detectorInput.CreateDetector();
            detector.Initialize(tissue);
            return detector;
        }
        ///// <summary>
        ///// Method that instantiates the correct detector class given a IDetectorInput
        ///// </summary>
        ///// <param name="detectorInput">IDetectorInput</param>
        ///// <param name="tissue">ITissue</param>
        ///// <param name="tallySecondMoment">flag indicating whether to tally second moment or not</param>
        ///// <returns>IDetector</returns>
        //public static IDetectorOld GetDetector(
        //    IDetectorInput detectorInput,
        //    ITissue tissue,
        //    bool tallySecondMoment)
        //{
        //    switch (detectorInput.TallyType)
        //    {
        //        // IDetector(s):
        //        case "RDiffuse":
        //            var rdinput = (RDiffuseDetectorInput)detectorInput;
        //            //return new RDiffuseDetector(tallySecondMoment, rdinput.Name);
        //            return new RDiffuseDetector(tallySecondMoment, rdinput.Name);
        //        case "RSpecular":
        //            var rsinput = (RSpecularDetectorInput)detectorInput;
        //            return new RSpecularDetector(tallySecondMoment, rsinput.Name);
        //        case "ROfRho":
        //            var rrinput = (ROfRhoDetectorInput)detectorInput;
        //            return new ROfRhoDetector(rrinput.Rho, tallySecondMoment, rrinput.Name);
        //        case "ROfAngle":
        //            var rainput = (ROfAngleDetectorInput)detectorInput;
        //            return new ROfAngleDetector(rainput.Angle, tallySecondMoment, rainput.Name);
        //        case "ROfRhoAndTime":
        //            var rrtinput = (ROfRhoAndTimeDetectorInput)detectorInput;
        //            return new ROfRhoAndTimeDetector(rrtinput.Rho, rrtinput.Time, tallySecondMoment, rrtinput.Name);
        //        case "ROfRhoAndAngle":
        //            var rrainput = (ROfRhoAndAngleDetectorInput)detectorInput;
        //            return new ROfRhoAndAngleDetector(rrainput.Rho, rrainput.Angle, tallySecondMoment, rrainput.Name);
        //        case "ROfXAndY":
        //            var rxyinput = (ROfXAndYDetectorInput)detectorInput;
        //            return new ROfXAndYDetector(rxyinput.X, rxyinput.Y, tallySecondMoment, rxyinput.Name);
        //        case "ROfRhoAndOmega":
        //            var rroinput = (ROfRhoAndOmegaDetectorInput)detectorInput;
        //            return new ROfRhoAndOmegaDetector(rroinput.Rho, rroinput.Omega, tallySecondMoment, rroinput.Name);
        //        case "ROfFx":
        //            var rfxinput = (ROfFxDetectorInput)detectorInput;
        //            return new ROfFxDetector(rfxinput.Fx, tallySecondMoment, rfxinput.Name);
        //        case "ROfFxAndTime":
        //            var rfxtinput = (ROfFxAndTimeDetectorInput)detectorInput;
        //            return new ROfFxAndTimeDetector(rfxtinput.Fx, rfxtinput.Time, tallySecondMoment, rfxtinput.Name);
        //        case "TDiffuse":
        //            var tdinput = (TDiffuseDetectorInput)detectorInput;
        //            return new TDiffuseDetector(tallySecondMoment, tdinput.Name);
        //        case "TOfAngle":
        //            var tainput = (TOfAngleDetectorInput)detectorInput;
        //            return new TOfAngleDetector(tainput.Angle, tallySecondMoment, tainput.Name);
        //        case "TOfRho":
        //            var trinput = (TOfRhoDetectorInput)detectorInput;
        //            return new TOfRhoDetector(trinput.Rho, tallySecondMoment, trinput.Name);
        //        case "TOfRhoAndAngle":
        //            var trainput = (TOfRhoAndAngleDetectorInput)detectorInput;
        //            return new TOfRhoAndAngleDetector(trainput.Rho, trainput.Angle, tallySecondMoment, trainput.Name);
        //        case "RadianceOfRho":
        //            var drinput = (RadianceOfRhoDetectorInput)detectorInput;
        //            return new RadianceOfRhoDetector(drinput.ZDepth, drinput.Rho, tallySecondMoment, drinput.Name);

        //        // IHistoryDetector(s):
        //        case "FluenceOfRhoAndZ":
        //            var frzinput = (FluenceOfRhoAndZDetectorInput)detectorInput;
        //            return new FluenceOfRhoAndZDetector(frzinput.Rho, frzinput.Z, tissue, tallySecondMoment, frzinput.Name);
        //        case "FluenceOfRhoAndZAndTime":
        //            var frztinput = (FluenceOfRhoAndZAndTimeDetectorInput)detectorInput;
        //            return new FluenceOfRhoAndZAndTimeDetector(frztinput.Rho, frztinput.Z, frztinput.Time, tissue, tallySecondMoment, frztinput.Name);
        //        case "FluenceOfXAndYAndZ":
        //            var fxyzinput = (FluenceOfXAndYAndZDetectorInput)detectorInput;
        //            return new FluenceOfXAndYAndZDetector(fxyzinput.X, fxyzinput.Y, fxyzinput.Z, tissue, tallySecondMoment, fxyzinput.Name);
        //        case "AOfRhoAndZ":
        //            var arzinput = (AOfRhoAndZDetectorInput)detectorInput;
        //            return new AOfRhoAndZDetector(arzinput.Rho, arzinput.Z, tissue, tallySecondMoment, arzinput.Name);
        //        case "ATotal":
        //            var ainput = (ATotalDetectorInput)detectorInput;
        //            return new ATotalDetector(tissue, tallySecondMoment, ainput.Name);
        //        case "RadianceOfRhoAndZAndAngle":
        //            var rrzainput = (RadianceOfRhoAndZAndAngleDetectorInput)detectorInput;
        //            return new RadianceOfRhoAndZAndAngleDetector(rrzainput.Rho, rrzainput.Z, rrzainput.Angle, tissue, tallySecondMoment, rrzainput.Name);
        //        case "RadianceOfXAndYAndZAndThetaAndPhi":
        //            var rxyztpinput = (RadianceOfXAndYAndZAndThetaAndPhiDetectorInput)detectorInput;
        //            return new RadianceOfXAndYAndZAndThetaAndPhiDetector(rxyztpinput.X, rxyztpinput.Y, rxyztpinput.Z, rxyztpinput.Theta, rxyztpinput.Phi, tissue, tallySecondMoment, rxyztpinput.Name);
        //        case "ReflectedMTOfRhoAndSubregionHist":
        //            var rmtrsinput = (ReflectedMTOfRhoAndSubregionHistDetectorInput)detectorInput;
        //            return new ReflectedMTOfRhoAndSubregionHistDetector(rmtrsinput.Rho, rmtrsinput.MTBins, rmtrsinput.FractionalMTBins, tissue, tallySecondMoment, rmtrsinput.Name);
        //        case "ReflectedTimeOfRhoAndSubregionHist":
        //            var rtrsinput = (ReflectedTimeOfRhoAndSubregionHistDetectorInput)detectorInput;
        //            return new ReflectedTimeOfRhoAndSubregionHistDetector(rtrsinput.Rho, rtrsinput.Time, tissue, tallySecondMoment, rtrsinput.Name);

        //        // pMC Detector(s):
        //        case "pMCROfRhoAndTime":
        //            var prrtinput = (pMCROfRhoAndTimeDetectorInput)detectorInput;
        //            return new pMCROfRhoAndTimeDetector(
        //                prrtinput.Rho,
        //                prrtinput.Time,
        //                tissue,
        //                prrtinput.PerturbedOps,
        //                prrtinput.PerturbedRegionsIndices,
        //                tallySecondMoment,
        //                prrtinput.Name);
        //        case "pMCROfRho":
        //            var prrinput = (pMCROfRhoDetectorInput)detectorInput;
        //            return new pMCROfRhoDetector(
        //                prrinput.Rho,
        //                tissue,
        //                prrinput.PerturbedOps,
        //                prrinput.PerturbedRegionsIndices,
        //                tallySecondMoment,
        //                prrinput.Name
        //                );
        //        case "pMCROfFx":
        //            var prfxinput = (pMCROfFxDetectorInput)detectorInput;
        //            return new pMCROfFxDetector(
        //                prfxinput.Fx,
        //                tissue,
        //                prfxinput.PerturbedOps.ToArray(), // todo: temp...make everything arrays (and deal w/ any pre/post serialization issues)
        //                prfxinput.PerturbedRegionsIndices.ToArray(),// todo: temp...make everything arrays (and deal w/ any pre/post serialization issues)
        //                tallySecondMoment,
        //                prfxinput.Name
        //                );
        //        case "pMCROfFxAndTime":
        //            var prfxtinput = (pMCROfFxAndTimeDetectorInput)detectorInput;
        //            return new pMCROfFxAndTimeDetector(
        //                prfxtinput.Fx,
        //                prfxtinput.Time,
        //                tissue,
        //                prfxtinput.PerturbedOps.ToArray(),// todo: temp...make everything arrays (and deal w/ any pre/post serialization issues)
        //                prfxtinput.PerturbedRegionsIndices.ToArray(),// todo: temp...make everything arrays (and deal w/ any pre/post serialization issues)
        //                tallySecondMoment,
        //                prfxtinput.Name
        //                );
        //        case "dMCdROfRhodMua":
        //            var pdrrainput = (dMCdROfRhodMuaDetectorInput)detectorInput;
        //            return new dMCdROfRhodMuaDetector(
        //                pdrrainput.Rho,
        //                tissue,
        //                pdrrainput.PerturbedOps,
        //                pdrrainput.PerturbedRegionsIndices,
        //                tallySecondMoment,
        //                pdrrainput.Name
        //                );
        //        case "dMCdROfRhodMus":
        //            var pdrrsinput = (dMCdROfRhodMusDetectorInput)detectorInput;
        //            return new dMCdROfRhodMusDetector(
        //                pdrrsinput.Rho,
        //                tissue,
        //                pdrrsinput.PerturbedOps,
        //                pdrrsinput.PerturbedRegionsIndices,
        //                tallySecondMoment,
        //                pdrrsinput.Name
        //                );
        //        default:
        //            return null;
        //    }
        //}
    }
}

﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using Vts.IO;
using Vts.MonteCarlo.Detectors;
using System.Runtime.Serialization;
using System.IO.IsolatedStorage;

namespace Vts.MonteCarlo.IO
{
    /// <summary>
    /// Class that handles IO for IDetectors.
    /// </summary>
    public static class DetectorIO
    {
        /// <summary>
        /// Writes Detector xml for scalar detectors, writes Detector xml and 
        /// binary for 1D and larger detectors.  Detector.Name is used for filename.
        /// </summary>
        /// <param name="detector">IDetector being written.</param>
        /// <param name="folderPath">location of written file.</param>
        public static void WriteDetectorToFile(IDetector detector, string folderPath)
        {
            try
            {
                // allow null folderPath in case writing to isolated storage
                string filePath = folderPath;
                if (folderPath == "")
                {
                    filePath = detector.Name;
                }
                else
                {
                    filePath = folderPath + @"/" + detector.Name;

                    // uses isolated storage for Silverlight, desktop folder otherwise
                    FileIO.CreateDirectory(folderPath);
                }

                if (detector is IDetector<double>)
                {
                    //var d = detector as IDetector<double>;
                    //FileIO.WriteToXML((RDiffuseDetector)d, filePath + ".xml");
                }
                if (detector is IDetector<double[]>)
                {
                    var d = detector as IDetector<double[]>;
                    //FileIO.WriteToXML(d, filePath + ".xml");
                    FileIO.WriteArrayToBinary<double>(d.Mean, filePath, false);
                    // output of 2nd moment 
                    FileIO.WriteArrayToBinary<double>(d.SecondMoment, filePath + "_2", false);
                }
                if (detector is IDetector<double[,]>)
                {
                    var d = detector as IDetector<double[,]>;
                    //FileIO.WriteToXML(d, filePath + ".xml");
                    FileIO.WriteArrayToBinary<double>(d.Mean, filePath, false);
                    FileIO.WriteArrayToBinary<double>(d.SecondMoment, filePath + "_2", false);
                }
                if (detector is IDetector<double[, ,]>)
                {
                    var d = detector as IDetector<double[, ,]>;
                    //FileIO.WriteToXML(d, filePath + ".xml");
                    FileIO.WriteArrayToBinary<double>(d.Mean, filePath, false);
                    FileIO.WriteArrayToBinary<double>(d.SecondMoment, filePath + "_2", false);
                }
                if (detector is IDetector<double[, , ,]>)
                {
                    var d = detector as IDetector<double[, , ,]>;
                    //FileIO.WriteToXML(d, filePath + ".xml");
                    FileIO.WriteArrayToBinary<double>(d.Mean, filePath, false);
                    FileIO.WriteArrayToBinary<double>(d.SecondMoment, filePath + "_2", false);
                }
                if (detector is IDetector<double[, , , ,]>)
                {
                    var d = detector as IDetector<double[, , , ,]>;
                    //FileIO.WriteToXML(d, filePath + ".xml");
                    FileIO.WriteArrayToBinary<double>(d.Mean, filePath, false);
                    FileIO.WriteArrayToBinary<double>(d.SecondMoment, filePath + "_2", false);
                }
                if (detector is IDetector<Complex>)
                {
                    //var d = detector as IDetector<Complex>;
                    //FileIO.WriteToXML(d, filePath + ".xml");
                }
                if (detector is IDetector<Complex[]>)
                {
                    var d = detector as IDetector<Complex[]>;
                    //FileIO.WriteToXML(d, filePath + ".xml");
                    FileIO.WriteArrayToBinary<Complex>(d.Mean, filePath, false);
                    FileIO.WriteArrayToBinary<Complex>(d.SecondMoment, filePath + "_2", false);
                }
                if (detector is IDetector<Complex[,]>)
                {
                    var d = detector as IDetector<Complex[,]>;
                    //FileIO.WriteToXML(d, filePath + ".xml");
                    FileIO.WriteArrayToBinary<Complex>(d.Mean, filePath, false);
                    FileIO.WriteArrayToBinary<Complex>(d.SecondMoment, filePath + "_2", false);
                }
                if (detector is IDetector<Complex[, ,]>)
                {
                    var d = detector as IDetector<Complex[, ,]>;
                    //FileIO.WriteToXML(d, filePath + ".xml");
                    FileIO.WriteArrayToBinary<Complex>(d.Mean, filePath, false);
                    FileIO.WriteArrayToBinary<Complex>(d.SecondMoment, filePath + "_2", false);
                }
                if (detector is IDetector<Complex[, , ,]>)
                {
                    var d = detector as IDetector<Complex[, , ,]>;
                    //FileIO.WriteToXML(d, filePath + ".xml");
                    FileIO.WriteArrayToBinary<Complex>(d.Mean, filePath, false);
                    FileIO.WriteArrayToBinary<Complex>(d.SecondMoment, filePath + "_2", false);
                }
                if (detector is IDetector<Complex[, , , ,]>)
                {
                    var d = detector as IDetector<Complex[, , , ,]>;
                    //FileIO.WriteToXML(d, filePath + ".xml");
                    FileIO.WriteArrayToBinary<Complex>(d.Mean, filePath, false);
                    FileIO.WriteArrayToBinary<Complex>(d.SecondMoment, filePath + "_2", false);
                }
				
				switch (detector.TallyType) {
					case TallyType.RDiffuse:					
	                    FileIO.WriteToXML((RDiffuseDetector)detector, filePath + ".xml");
						break;
					case TallyType.TDiffuse:
	                    FileIO.WriteToXML((TDiffuseDetector)detector, filePath + ".xml");
						break;

                    case TallyType.ATotal:
	                    FileIO.WriteToXML((ATotalDetector)detector, filePath + ".xml");
						break;

                    // "1D" detectors
                    case TallyType.ROfRho:
	                    FileIO.WriteToXML((ROfRhoDetector)detector, filePath + ".xml");
						break;

                    case TallyType.pMCROfRho:
	                    FileIO.WriteToXML((pMCROfRhoDetector)detector, filePath + ".xml");
						break;

                    case TallyType.dMCdROfRhodMua:
                        FileIO.WriteToXML((dMCdROfRhodMuaDetector)detector, filePath + ".xml");
                        break;

                    case TallyType.dMCdROfRhodMus:
                        FileIO.WriteToXML((dMCdROfRhodMusDetector)detector, filePath + ".xml");
                        break;

                    case TallyType.TOfRho:
	                    FileIO.WriteToXML((TOfRhoDetector)detector, filePath + ".xml");
						break;

                    case TallyType.ROfAngle:
	                    FileIO.WriteToXML((ROfAngleDetector)detector, filePath + ".xml");
						break;

                    case TallyType.ROfFx:
	                    FileIO.WriteToXML((ROfFxDetector)detector, filePath + ".xml");
						break;

                    case TallyType.pMCROfFx:
	                    FileIO.WriteToXML((pMCROfFxDetector)detector, filePath + ".xml");
						break;

                    case TallyType.TOfAngle:
	                    FileIO.WriteToXML((TOfAngleDetector)detector, filePath + ".xml");
						break;

                    case TallyType.FluenceOfTetrahedralMesh:
                        FileIO.WriteToXML((FluenceOfTetrahedralMeshDetector)detector, filePath + ".xml");
                        break;

                    // "2D" detectors
                    case TallyType.ROfRhoAndTime:
	                    FileIO.WriteToXML((ROfRhoAndTimeDetector)detector, filePath + ".xml");
						break;

                    case TallyType.pMCROfRhoAndTime:
	                    FileIO.WriteToXML((pMCROfRhoAndTimeDetector)detector, filePath + ".xml");
						break;

                    case TallyType.pMCROfFxAndTime:
	                    FileIO.WriteToXML((pMCROfFxAndTimeDetector)detector, filePath + ".xml");
						break;

                    case TallyType.ROfRhoAndAngle:
	                    FileIO.WriteToXML((ROfRhoAndAngleDetector)detector, filePath + ".xml");
						break;

                    case TallyType.TOfRhoAndAngle:
	                    FileIO.WriteToXML((TOfRhoAndAngleDetector)detector, filePath + ".xml");
						break;

                    case TallyType.ROfRhoAndOmega:
	                    FileIO.WriteToXML((ROfRhoAndOmegaDetector)detector, filePath + ".xml");
						break;
					
                    case TallyType.ROfFxAndTime:
	                    FileIO.WriteToXML((ROfFxAndTimeDetector)detector, filePath + ".xml");
						break;

                    case TallyType.ROfXAndY:
	                    FileIO.WriteToXML((ROfXAndYDetector)detector, filePath + ".xml");
						break;

                    case TallyType.FluenceOfRhoAndZ:
	                    FileIO.WriteToXML((FluenceOfRhoAndZDetector)detector, filePath + ".xml");
						break;

                    case TallyType.FluenceOfXAndYAndZ:
	                    FileIO.WriteToXML((FluenceOfXAndYAndZDetector)detector, filePath + ".xml");
						break;

                    case TallyType.AOfRhoAndZ:
	                    FileIO.WriteToXML((AOfRhoAndZDetector)detector, filePath + ".xml");
						break;


                    // "3D" detectors
                    case TallyType.FluenceOfRhoAndZAndTime:
	                    FileIO.WriteToXML((FluenceOfRhoAndZAndTimeDetector)detector, filePath + ".xml");
						break;

                    case TallyType.ReflectedMTOfRhoAndSubregionHist:
	                    FileIO.WriteToXML((ReflectedMTOfRhoAndSubregionHistDetector)detector, filePath + ".xml");
                        var dmt = detector as ReflectedMTOfRhoAndSubregionHistDetector;
                        FileIO.WriteArrayToBinary<double>(dmt.FractionalMT, filePath + "_FractionalMT", false);
						break;

                    case TallyType.ReflectedTimeOfRhoAndSubregionHist:
	                    FileIO.WriteToXML((ReflectedTimeOfRhoAndSubregionHistDetector)detector, filePath + ".xml");
                        var d = detector as ReflectedTimeOfRhoAndSubregionHistDetector;
                        FileIO.WriteArrayToBinary<double>(d.FractionalTime, filePath + "_FractionalTime", false);
						break;

                    // "5D" detectors
                    case TallyType.RadianceOfXAndYAndZAndThetaAndPhi:
	                    FileIO.WriteToXML((RadianceOfXAndYAndZAndThetaAndPhiDetector)detector, filePath + ".xml");
						break;
					default:
	                    FileIO.WriteToXML(detector, filePath + ".xml");
						break;
					}
            }
            catch (Exception e)
            {
                Console.WriteLine("Problem writing detector information to file.\n\nDetails:\n\n" + e + "\n");
            }
        }
        /// <summary>
        /// Reads Detector from File with given fileName.
        /// </summary>
        /// <param name="tallyType">TallyType of IDetector being read</param>
        /// <param name="fileName">filename string of file to be read</param>
        /// <param name="folderPath">path string where file resides</param>
        /// <returns>IDetector</returns>
        public static IDetector ReadDetectorFromFile(TallyType tallyType, string fileName, string folderPath)
        {
            try
            {
                // allow null filePaths in case writing to isolated storage
                string filePath;
                if (folderPath == "")
                {
                    filePath = fileName;
                }
                else
                {
                    filePath = folderPath + @"/" + fileName;
                }
                switch (tallyType)
                {
                    // "0D" detectors
                    case TallyType.RDiffuse:
                        return FileIO.ReadFromXML<RDiffuseDetector>(filePath + ".xml");

                    case TallyType.TDiffuse:
                        return FileIO.ReadFromXML<TDiffuseDetector>(filePath + ".xml");

                    case TallyType.ATotal:
                        return FileIO.ReadFromXML<ATotalDetector>(filePath + ".xml");

                    // "1D" detectors
                    case TallyType.ROfRho:
                        var rOfRhoDetector = FileIO.ReadFromXML<ROfRhoDetector>(filePath + ".xml");
                        var rOfRhoDetectorDims = new int[] { rOfRhoDetector.Rho.Count - 1 };
                        rOfRhoDetector.Mean = (double[])FileIO.ReadArrayFromBinary<double>(filePath, rOfRhoDetectorDims);
                        return rOfRhoDetector;

                    case TallyType.pMCROfRho:
                        var pMCROfRhoDetector = FileIO.ReadFromXML<pMCROfRhoDetector>(filePath + ".xml");
                        var pMCROfRhoDetectorDims = new int[] { pMCROfRhoDetector.Rho.Count - 1 };
                        pMCROfRhoDetector.Mean = (double[])FileIO.ReadArrayFromBinary<double>(filePath, pMCROfRhoDetectorDims);
                        return pMCROfRhoDetector;

                    case TallyType.dMCdROfRhodMua:
                        var dMCdROfRhodMuaDetector = FileIO.ReadFromXML<dMCdROfRhodMuaDetector>(filePath + ".xml");
                        var dMCdROfRhodMuaDetectorDims = new int[] { dMCdROfRhodMuaDetector.Rho.Count - 1 };
                        dMCdROfRhodMuaDetector.Mean = (double[])FileIO.ReadArrayFromBinary<double>(filePath, dMCdROfRhodMuaDetectorDims);
                        return dMCdROfRhodMuaDetector;

                    case TallyType.dMCdROfRhodMus:
                        var dMCdROfRhodMusDetector = FileIO.ReadFromXML<dMCdROfRhodMusDetector>(filePath + ".xml");
                        var dMCdROfRhodMusDetectorDims = new int[] { dMCdROfRhodMusDetector.Rho.Count - 1 };
                        dMCdROfRhodMusDetector.Mean = (double[])FileIO.ReadArrayFromBinary<double>(filePath, dMCdROfRhodMusDetectorDims);
                        return dMCdROfRhodMusDetector;

                    case TallyType.TOfRho:
                        var tOfRhoDetector = FileIO.ReadFromXML<TOfRhoDetector>(filePath + ".xml");
                        var tOfRhoDetectorDims = new int[] { tOfRhoDetector.Rho.Count - 1 };
                        tOfRhoDetector.Mean = (double[])FileIO.ReadArrayFromBinary<double>(filePath, tOfRhoDetectorDims);
                        return tOfRhoDetector;

                    case TallyType.ROfAngle:
                        var rOfAngleDetector = FileIO.ReadFromXML<ROfAngleDetector>(filePath + ".xml");
                        var rOfAngleDetectorDims = new int[] { rOfAngleDetector.Angle.Count - 1 };
                        rOfAngleDetector.Mean = (double[])FileIO.ReadArrayFromBinary<double>(filePath, rOfAngleDetectorDims);
                        return rOfAngleDetector;

                    case TallyType.ROfFx:
                        var rOfFxDetector = FileIO.ReadFromXML<ROfFxDetector>(filePath + ".xml");
                        var rOfFxDetectorDims = new int[] { rOfFxDetector.Fx.Count };
                        rOfFxDetector.Mean = (Complex[])FileIO.ReadArrayFromBinary<Complex>(filePath, rOfFxDetectorDims);
                        return rOfFxDetector;

                    case TallyType.pMCROfFx:
                        var pMCROfFxDetector = FileIO.ReadFromXML<pMCROfFxDetector>(filePath + ".xml");
                        var pMCROfFxDetectorDims = new int[] { pMCROfFxDetector.Fx.Count };
                        pMCROfFxDetector.Mean = (Complex[])FileIO.ReadArrayFromBinary<Complex>(filePath, pMCROfFxDetectorDims);
                        return pMCROfFxDetector;

                    case TallyType.TOfAngle:
                        var tOfAngleDetector = FileIO.ReadFromXML<TOfAngleDetector>(filePath + ".xml");
                        var tOfAngleDetectorDims = new int[] { tOfAngleDetector.Angle.Count - 1 };
                        tOfAngleDetector.Mean = (double[])FileIO.ReadArrayFromBinary<double>(filePath, tOfAngleDetectorDims);
                        return tOfAngleDetector;

                    case TallyType.FluenceOfTetrahedralMesh:
                        var fluenceOfTetrahedralMeshDetector = FileIO.ReadFromXML<FluenceOfTetrahedralMeshDetector>(filePath + ".xml");
                        var fluenceOfTetrahedralMeshDetectorDims = new int[] { fluenceOfTetrahedralMeshDetector.Mean.Length - 1 };
                        fluenceOfTetrahedralMeshDetector.Mean = (double[])FileIO.ReadArrayFromBinary<double>(filePath, fluenceOfTetrahedralMeshDetectorDims);
                        return fluenceOfTetrahedralMeshDetector;

                    // "2D" detectors
                    case TallyType.ROfRhoAndTime:
                        var rOfRhoAndTimeDetector = FileIO.ReadFromXML<ROfRhoAndTimeDetector>(filePath + ".xml");
                        var rOfRhoAndTimeDetectorDims = new int[] { rOfRhoAndTimeDetector.Rho.Count - 1, rOfRhoAndTimeDetector.Time.Count - 1 };
                        rOfRhoAndTimeDetector.Mean = (double[,])FileIO.ReadArrayFromBinary<double>(filePath, rOfRhoAndTimeDetectorDims);
                        return rOfRhoAndTimeDetector;

                    case TallyType.pMCROfRhoAndTime:
                        var pMCROfRhoAndTimeDetector =
                            FileIO.ReadFromXML<pMCROfRhoAndTimeDetector>(filePath + ".xml");
                        var pMCROfRhoAndTimeDetectorDims = new int[] { pMCROfRhoAndTimeDetector.Rho.Count - 1, pMCROfRhoAndTimeDetector.Time.Count - 1 };
                        pMCROfRhoAndTimeDetector.Mean = (double[,])FileIO.ReadArrayFromBinary<double>(filePath, pMCROfRhoAndTimeDetectorDims);
                        return pMCROfRhoAndTimeDetector;

                    case TallyType.pMCROfFxAndTime:
                        var pMCROfFxAndTimeDetector =
                            FileIO.ReadFromXML<pMCROfFxAndTimeDetector>(filePath + ".xml");
                        var pMCROfFxAndTimeDetectorDims = new int[] { pMCROfFxAndTimeDetector.Fx.Count - 1, pMCROfFxAndTimeDetector.Time.Count - 1 };
                        pMCROfFxAndTimeDetector.Mean = (Complex[,])FileIO.ReadArrayFromBinary<Complex>(filePath, pMCROfFxAndTimeDetectorDims);
                        return pMCROfFxAndTimeDetector;

                    case TallyType.ROfRhoAndAngle:
                        var rOfRhoAndAngleDetector = FileIO.ReadFromXML<ROfRhoAndAngleDetector>(filePath + ".xml");
                        var rOfRhoAndAngleDetectorDims = new int[] { rOfRhoAndAngleDetector.Rho.Count - 1, rOfRhoAndAngleDetector.Angle.Count - 1 };
                        rOfRhoAndAngleDetector.Mean = (double[,])FileIO.ReadArrayFromBinary<double>(filePath, rOfRhoAndAngleDetectorDims);
                        return rOfRhoAndAngleDetector;

                    case TallyType.TOfRhoAndAngle:
                        var tOfRhoAndAngleDetector = FileIO.ReadFromXML<TOfRhoAndAngleDetector>(filePath + ".xml");
                        var tOfRhoAndAngleDetectorDims = new int[] { tOfRhoAndAngleDetector.Rho.Count - 1, tOfRhoAndAngleDetector.Angle.Count - 1 };
                        tOfRhoAndAngleDetector.Mean = (double[,])FileIO.ReadArrayFromBinary<double>(filePath, tOfRhoAndAngleDetectorDims);
                        return tOfRhoAndAngleDetector;

                    case TallyType.ROfRhoAndOmega:
                        var rOfRhoAndOmegaDetector = FileIO.ReadFromXML<ROfRhoAndOmegaDetector>(filePath + ".xml");
                        var rOfRhoAndOmegaDetectorDims = new int[] { rOfRhoAndOmegaDetector.Rho.Count - 1, rOfRhoAndOmegaDetector.Omega.Count };
                        rOfRhoAndOmegaDetector.Mean = (Complex[,])FileIO.ReadArrayFromBinary<Complex>(filePath, rOfRhoAndOmegaDetectorDims);
                        return rOfRhoAndOmegaDetector;

                    case TallyType.ROfFxAndTime:
                        var rOfFxAndTimeDetector = FileIO.ReadFromXML<ROfFxAndTimeDetector>(filePath + ".xml");
                        var rOfFxAndTimeDetectorDims = new int[] { rOfFxAndTimeDetector.Fx.Count, rOfFxAndTimeDetector.Time.Count - 1 };
                        rOfFxAndTimeDetector.Mean = (Complex[,])FileIO.ReadArrayFromBinary<Complex>(filePath, rOfFxAndTimeDetectorDims);
                        return rOfFxAndTimeDetector;

                    case TallyType.ROfXAndY:
                        var rOfXAndYDetector = FileIO.ReadFromXML<ROfXAndYDetector>(filePath + ".xml");
                        var rOfXAndYDetectorDims = new int[] { rOfXAndYDetector.X.Count - 1, rOfXAndYDetector.Y.Count - 1 };
                        rOfXAndYDetector.Mean = (double[,])FileIO.ReadArrayFromBinary<double>(filePath, rOfXAndYDetectorDims);
                        return rOfXAndYDetector;

                    case TallyType.FluenceOfRhoAndZ:
                        var fluenceOfRhoAndZDetector = FileIO.ReadFromXML<FluenceOfRhoAndZDetector>(filePath + ".xml");
                        var fluenceOfRhoAndZDetectorDims = new int[] { fluenceOfRhoAndZDetector.Rho.Count - 1, fluenceOfRhoAndZDetector.Z.Count - 1 };
                        fluenceOfRhoAndZDetector.Mean = (double[,])FileIO.ReadArrayFromBinary<double>(filePath, fluenceOfRhoAndZDetectorDims);
                        return fluenceOfRhoAndZDetector;

                    case TallyType.FluenceOfXAndYAndZ:
                        var fluenceOfXAndYAndZDetector = FileIO.ReadFromXML<FluenceOfXAndYAndZDetector>(filePath + ".xml");
                        var fluenceOfXAndYAndZDetectorDims = new int[] { fluenceOfXAndYAndZDetector.X.Count - 1, fluenceOfXAndYAndZDetector.Y.Count - 1, fluenceOfXAndYAndZDetector.Z.Count -1 };
                        fluenceOfXAndYAndZDetector.Mean = (double[,,])FileIO.ReadArrayFromBinary<double>(filePath, fluenceOfXAndYAndZDetectorDims);
                        return fluenceOfXAndYAndZDetector;

                    case TallyType.AOfRhoAndZ:
                        var aOfRhoAndZDetector = FileIO.ReadFromXML<AOfRhoAndZDetector>(filePath + ".xml");
                        var aOfRhoAndZDetectorDims = new int[] { aOfRhoAndZDetector.Rho.Count - 1, aOfRhoAndZDetector.Z.Count - 1 };
                        aOfRhoAndZDetector.Mean = (double[,])FileIO.ReadArrayFromBinary<double>(filePath, aOfRhoAndZDetectorDims);
                        return aOfRhoAndZDetector;


                    // "3D" detectors
                    case TallyType.FluenceOfRhoAndZAndTime:
                        var fluenceOfRhoAndZAndTimeDetector =
                            FileIO.ReadFromXML<FluenceOfRhoAndZAndTimeDetector>(filePath + ".xml");
                        var fluenceOfRhoAndZAndTimeDetectorDims = new int[] { 
                            fluenceOfRhoAndZAndTimeDetector.Rho.Count - 1, 
                            fluenceOfRhoAndZAndTimeDetector.Z.Count - 1,
                            fluenceOfRhoAndZAndTimeDetector.Time.Count - 1 };
                        fluenceOfRhoAndZAndTimeDetector.Mean = (double[, ,])FileIO.ReadArrayFromBinary<double>(filePath, fluenceOfRhoAndZAndTimeDetectorDims);
                        return fluenceOfRhoAndZAndTimeDetector;

                    case TallyType.ReflectedMTOfRhoAndSubregionHist:
                        var reflectedMTOfRhoAndSubregionHistDetector =
                            FileIO.ReadFromXML<ReflectedMTOfRhoAndSubregionHistDetector>(filePath + ".xml");
                        var reflectedMTOfRhoAndSubregionHistDetectorDims = new int[] {
                            reflectedMTOfRhoAndSubregionHistDetector.Rho.Count - 1, 
                            reflectedMTOfRhoAndSubregionHistDetector.MTBins.Count - 1 };                                                 
                        reflectedMTOfRhoAndSubregionHistDetector.Mean =
                            (double[,])FileIO.ReadArrayFromBinary<double>(filePath, reflectedMTOfRhoAndSubregionHistDetectorDims);
                        return reflectedMTOfRhoAndSubregionHistDetector;

                    case TallyType.ReflectedTimeOfRhoAndSubregionHist:
                        var reflectedTimeOfRhoAndSubregionHistDetector =
                            FileIO.ReadFromXML<ReflectedTimeOfRhoAndSubregionHistDetector>(filePath + ".xml");
                        var reflectedTimeOfRhoAndSubregionHistDetectorDims = new int[] {
                            reflectedTimeOfRhoAndSubregionHistDetector.Rho.Count - 1, 
                            reflectedTimeOfRhoAndSubregionHistDetector.SubregionIndices.Count,
                            reflectedTimeOfRhoAndSubregionHistDetector.Time.Count - 1 };
                        reflectedTimeOfRhoAndSubregionHistDetector.Mean =
                            (double[, ,])FileIO.ReadArrayFromBinary<double>(filePath, reflectedTimeOfRhoAndSubregionHistDetectorDims);
                        return reflectedTimeOfRhoAndSubregionHistDetector;

                    // "5D" detectors
                    case TallyType.RadianceOfXAndYAndZAndThetaAndPhi:
                        var radianceOfXAndYAndZAndThetaAndPhiDetector =
                            FileIO.ReadFromXML<RadianceOfXAndYAndZAndThetaAndPhiDetector>(filePath + ".xml");
                        var radianceOfXAndYAndZAndThetaAndPhiDims = new int[] { 
                            radianceOfXAndYAndZAndThetaAndPhiDetector.X.Count - 1, 
                            radianceOfXAndYAndZAndThetaAndPhiDetector.Y.Count - 1,
                            radianceOfXAndYAndZAndThetaAndPhiDetector.Z.Count - 1,
                            radianceOfXAndYAndZAndThetaAndPhiDetector.Theta.Count - 1,
                            radianceOfXAndYAndZAndThetaAndPhiDetector.Phi.Count - 1};
                        radianceOfXAndYAndZAndThetaAndPhiDetector.Mean = (double[,,,,])FileIO.ReadArrayFromBinary<double>(filePath, radianceOfXAndYAndZAndThetaAndPhiDims);
                        return radianceOfXAndYAndZAndThetaAndPhiDetector;

                    default:
                        throw new ArgumentOutOfRangeException("tallyType");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Problem reading detector information from file.\n\nDetails:\n\n" + e + "\n");
            }

            return null;
        }
        /// <summary>
        /// Reads Detector from file with default fileName (TallyType.ToString).
        /// </summary>
        /// <param name="tallyType">TallyType of IDetector being read</param>
        /// <param name="folderPath">path string of folder where file to be read resides</param>
        /// <returns>IDetector</returns>
        public static IDetector ReadDetectorFromFile(TallyType tallyType, string folderPath)
        {
            return ReadDetectorFromFile(tallyType, tallyType.ToString(), folderPath);
        }
        /// <summary>
        /// Reads Detector from a file in resources using given fileName.
        /// </summary>
        /// <param name="tallyType">TallyType of IDetector being read</param>
        /// <param name="fileName">filename string of file to be read</param>
        /// <param name="folderPath">path string of folder where file to be read resides</param>
        /// <param name="projectName">project name string where file resides in resources</param>
        /// <returns>IDetector</returns>
        public static IDetector ReadDetectorFromFileInResources(TallyType tallyType, string fileName, string folderPath, string projectName)
        {
            try
            {
                string filePath = folderPath + fileName;
                switch (tallyType)
                {
                    // "0D" detectors
                    case TallyType.RDiffuse:
                        return FileIO.ReadFromXMLInResources<RDiffuseDetector>(filePath + ".xml", projectName);

                    case TallyType.TDiffuse:
                        return FileIO.ReadFromXMLInResources<TDiffuseDetector>(filePath + ".xml", projectName);

                    case TallyType.ATotal:
                        return FileIO.ReadFromXMLInResources<ATotalDetector>(filePath + ".xml", projectName);

                    // "1D" detectors
                    case TallyType.ROfRho:
                        var rOfRhoDetector = FileIO.ReadFromXMLInResources<ROfRhoDetector>(filePath + ".xml", projectName);
                        var rOfRhoDetectorDims = new int[] { rOfRhoDetector.Rho.Count - 1 };
                        rOfRhoDetector.Mean = (double[])FileIO.ReadArrayFromBinaryInResources<double>(filePath, projectName, rOfRhoDetectorDims);
                        return rOfRhoDetector;

                    case TallyType.pMCROfRho:
                        var pMuaMusROfRhoDetector = FileIO.ReadFromXMLInResources<pMCROfRhoDetector>(filePath + ".xml", projectName);
                        var pMCROfRhoDetectorDims = new int[] { pMuaMusROfRhoDetector.Rho.Count - 1 };
                        pMuaMusROfRhoDetector.Mean = (double[])FileIO.ReadArrayFromBinaryInResources<double>(filePath, projectName, pMCROfRhoDetectorDims);
                        return pMuaMusROfRhoDetector;

                    case TallyType.dMCdROfRhodMua:
                        var dMCdROfRhodMuaDetector = FileIO.ReadFromXMLInResources<dMCdROfRhodMuaDetector>(filePath + ".xml", projectName);
                        var dMCdROfRhodMuaDetectorDims = new int[] { dMCdROfRhodMuaDetector.Rho.Count - 1 };
                        dMCdROfRhodMuaDetector.Mean = (double[])FileIO.ReadArrayFromBinaryInResources<double>(filePath, projectName, dMCdROfRhodMuaDetectorDims);
                        return dMCdROfRhodMuaDetector;

                    case TallyType.dMCdROfRhodMus:
                        var dMCdROfRhodMusDetector = FileIO.ReadFromXMLInResources<dMCdROfRhodMusDetector>(filePath + ".xml", projectName);
                        var dMCdROfRhodMusDetectorDims = new int[] { dMCdROfRhodMusDetector.Rho.Count - 1 };
                        dMCdROfRhodMusDetector.Mean = (double[])FileIO.ReadArrayFromBinaryInResources<double>(filePath, projectName, dMCdROfRhodMusDetectorDims);
                        return dMCdROfRhodMusDetector;

                    case TallyType.TOfRho:
                        var tOfRhoDetector = FileIO.ReadFromXMLInResources<TOfRhoDetector>(filePath + ".xml", projectName);
                        tOfRhoDetector.Mean = (double[])FileIO.ReadArrayFromBinaryInResources<double>(filePath, projectName);
                        return tOfRhoDetector;

                    case TallyType.ROfAngle:
                        var rOfAngleDetector = FileIO.ReadFromXMLInResources<ROfAngleDetector>(filePath + ".xml", projectName);
                        var rOfAngleDetectorDims = new int[] { rOfAngleDetector.Angle.Count - 1 };
                        rOfAngleDetector.Mean = (double[])FileIO.ReadArrayFromBinaryInResources<double>(filePath, projectName, rOfAngleDetectorDims);
                        return rOfAngleDetector;

                    case TallyType.ROfFx:
                        var rOfFxDetector = FileIO.ReadFromXMLInResources<ROfFxDetector>(filePath + ".xml", projectName);
                        var rOfFxDetectorDims = new int[] { rOfFxDetector.Fx.Count };
                        rOfFxDetector.Mean = (Complex[])FileIO.ReadArrayFromBinaryInResources<Complex>(filePath, projectName, rOfFxDetectorDims);
                        return rOfFxDetector;

                    case TallyType.pMCROfFx:
                        var pMuaMusROfFxDetector = FileIO.ReadFromXMLInResources<pMCROfFxDetector>(filePath + ".xml", projectName);
                        var pMCROfFxDetectorDims = new int[] { pMuaMusROfFxDetector.Fx.Count };
                        pMuaMusROfFxDetector.Mean = (Complex[])FileIO.ReadArrayFromBinaryInResources<Complex>(filePath, projectName, pMCROfFxDetectorDims);
                        return pMuaMusROfFxDetector;

                    case TallyType.TOfAngle:
                        var tOfAngleDetector = FileIO.ReadFromXMLInResources<TOfAngleDetector>(filePath + ".xml", projectName);
                        var tOfAngleDetectorDims = new int[] { tOfAngleDetector.Angle.Count - 1 };
                        tOfAngleDetector.Mean = (double[])FileIO.ReadArrayFromBinaryInResources<double>(filePath, projectName, tOfAngleDetectorDims);
                        return tOfAngleDetector;

                    case TallyType.FluenceOfTetrahedralMesh:
                        var fluenceOfTetrahedralMeshDetector = FileIO.ReadFromXMLInResources<FluenceOfTetrahedralMeshDetector>(filePath + ".xml", projectName);
                        var fluenceOfTetrahedralMeshDetectorDims = new int[] { fluenceOfTetrahedralMeshDetector.Mean.Length - 1 };
                        fluenceOfTetrahedralMeshDetector.Mean = (double[])FileIO.ReadArrayFromBinaryInResources<double>(filePath, projectName, fluenceOfTetrahedralMeshDetectorDims);
                        return fluenceOfTetrahedralMeshDetector;
                    // "2D" detectors
                    case TallyType.ROfRhoAndTime:
                        var rOfRhoAndTimeDetector = FileIO.ReadFromXMLInResources<ROfRhoAndTimeDetector>(filePath + ".xml", projectName);
                        var rOfRhoAndTimeDetectorDims = new int[] { rOfRhoAndTimeDetector.Rho.Count, rOfRhoAndTimeDetector.Time.Count - 1 };
                        rOfRhoAndTimeDetector.Mean = (double[,])FileIO.ReadArrayFromBinaryInResources<double>(filePath, projectName, rOfRhoAndTimeDetectorDims);
                        return rOfRhoAndTimeDetector;

                    case TallyType.pMCROfRhoAndTime:
                        var pMuaMusROfRhoAndTimeDetector = FileIO.ReadFromXMLInResources<pMCROfRhoAndTimeDetector>(filePath + ".xml", projectName);
                        var pMCROfRhoAndTimeDetectorDims = new int[] { pMuaMusROfRhoAndTimeDetector.Rho.Count, pMuaMusROfRhoAndTimeDetector.Time.Count - 1 };
                        pMuaMusROfRhoAndTimeDetector.Mean = (double[,])FileIO.ReadArrayFromBinaryInResources<double>(filePath, projectName, pMCROfRhoAndTimeDetectorDims);
                        return pMuaMusROfRhoAndTimeDetector;

                    case TallyType.pMCROfFxAndTime:
                        var pMuaMusROfFxAndTimeDetector = FileIO.ReadFromXMLInResources<pMCROfFxAndTimeDetector>(filePath + ".xml", projectName);
                        var pMCROfFxAndTimeDetectorDims = new int[] { pMuaMusROfFxAndTimeDetector.Fx.Count, pMuaMusROfFxAndTimeDetector.Time.Count - 1 };
                        pMuaMusROfFxAndTimeDetector.Mean = (Complex[,])FileIO.ReadArrayFromBinaryInResources<Complex>(filePath, projectName, pMCROfFxAndTimeDetectorDims);
                        return pMuaMusROfFxAndTimeDetector;

                    case TallyType.ROfRhoAndAngle:
                        var rOfRhoAndAngleDetector =
                            FileIO.ReadFromXMLInResources<ROfRhoAndAngleDetector>(filePath + ".xml", projectName);
                        var rOfRhoAndAngleDetectorDims = new int[] { rOfRhoAndAngleDetector.Rho.Count - 1, rOfRhoAndAngleDetector.Angle.Count - 1 };
                        rOfRhoAndAngleDetector.Mean =
                            (double[,])FileIO.ReadArrayFromBinaryInResources<double>(filePath, projectName, rOfRhoAndAngleDetectorDims);
                        return rOfRhoAndAngleDetector;

                    case TallyType.TOfRhoAndAngle:
                        var tOfRhoAndAngleDetector =
                            FileIO.ReadFromXMLInResources<TOfRhoAndAngleDetector>(filePath + ".xml", projectName);
                        var tOfRhoAndAngleDetectorDims = new int[] { tOfRhoAndAngleDetector.Rho.Count - 1, tOfRhoAndAngleDetector.Angle.Count - 1 };
                        tOfRhoAndAngleDetector.Mean =
                            (double[,])FileIO.ReadArrayFromBinaryInResources<double>(filePath, projectName, tOfRhoAndAngleDetectorDims);
                        return tOfRhoAndAngleDetector;

                    case TallyType.ROfRhoAndOmega:
                        var rOfRhoAndOmegaDetector =
                            FileIO.ReadFromXMLInResources<ROfRhoAndOmegaDetector>(filePath + ".xml", projectName);
                        var rOfRhoAndOmegaDetectorDims = new int[] { rOfRhoAndOmegaDetector.Rho.Count - 1, rOfRhoAndOmegaDetector.Omega.Count};
                        rOfRhoAndOmegaDetector.Mean =
                            (Complex[,])FileIO.ReadArrayFromBinaryInResources<Complex>(filePath, projectName, rOfRhoAndOmegaDetectorDims);
                        return rOfRhoAndOmegaDetector;

                    case TallyType.ROfFxAndTime:
                        var rOfFxAndTimeDetector = FileIO.ReadFromXMLInResources<ROfFxAndTimeDetector>(filePath + ".xml", projectName);
                        var rOfFxAndTimeDetectorDims = new int[] { rOfFxAndTimeDetector.Fx.Count, rOfFxAndTimeDetector.Time.Count - 1 };
                        rOfFxAndTimeDetector.Mean = (Complex[,])FileIO.ReadArrayFromBinaryInResources<Complex>(filePath, projectName, rOfFxAndTimeDetectorDims);
                        return rOfFxAndTimeDetector;

                    case TallyType.ROfXAndY:
                        var rOfXAndYDetector = FileIO.ReadFromXMLInResources<ROfXAndYDetector>(filePath + ".xml",
                                                                                               projectName);
                        var rOfXAndYDetectorDims = new int[] { rOfXAndYDetector.X.Count, rOfXAndYDetector.Y.Count };
                        rOfXAndYDetector.Mean =
                            (double[,])FileIO.ReadArrayFromBinaryInResources<double>(filePath, projectName, rOfXAndYDetectorDims);
                        return rOfXAndYDetector;

                    case TallyType.FluenceOfRhoAndZ:
                        var fluenceOfRhoAndZDetector =
                            FileIO.ReadFromXMLInResources<FluenceOfRhoAndZDetector>(filePath + ".xml", projectName);
                        var fluenceOfRhoAndZDetectorDims = new int[] { fluenceOfRhoAndZDetector.Rho.Count, fluenceOfRhoAndZDetector.Z.Count };
                        fluenceOfRhoAndZDetector.Mean =
                            (double[,])FileIO.ReadArrayFromBinaryInResources<double>(filePath, projectName, fluenceOfRhoAndZDetectorDims);
                        return fluenceOfRhoAndZDetector;

                    case TallyType.AOfRhoAndZ:
                        var aOfRhoAndZDetector = FileIO.ReadFromXMLInResources<AOfRhoAndZDetector>(filePath + ".xml",
                                                                                                   projectName);
                        var aOfRhoAndZDetectorDims = new int[] { aOfRhoAndZDetector.Rho.Count, aOfRhoAndZDetector.Z.Count };
                        aOfRhoAndZDetector.Mean =
                            (double[,])FileIO.ReadArrayFromBinaryInResources<double>(filePath, projectName, aOfRhoAndZDetectorDims);
                        return aOfRhoAndZDetector;


                    // "3D" detectors
                    case TallyType.FluenceOfRhoAndZAndTime:
                        var fluenceOfRhoAndZAndTimeDetector =
                            FileIO.ReadFromXMLInResources<FluenceOfRhoAndZAndTimeDetector>(filePath + ".xml",  projectName);
                        var fluenceOfRhoAndZAndTimeDetectorDims =
                            new int[] { fluenceOfRhoAndZAndTimeDetector.Rho.Count, 
                                        fluenceOfRhoAndZAndTimeDetector.Z.Count,
                                        fluenceOfRhoAndZAndTimeDetector.Time.Count };
                        fluenceOfRhoAndZAndTimeDetector.Mean =
                            (double[, ,])FileIO.ReadArrayFromBinaryInResources<double>(filePath, projectName, fluenceOfRhoAndZAndTimeDetectorDims);
                        return fluenceOfRhoAndZAndTimeDetector;
                    case TallyType.ReflectedMTOfRhoAndSubregionHist:
                        var reflectedMTOfRhoAndSubRegionHistDetector =
                            FileIO.ReadFromXMLInResources<ReflectedMTOfRhoAndSubregionHistDetector>(filePath + ".xml", projectName);
                        var ReflectedMTOfRhoAndSubregionHistDims =
                            new int[] { reflectedMTOfRhoAndSubRegionHistDetector.Rho.Count,  
                                        reflectedMTOfRhoAndSubRegionHistDetector.MTBins.Count };
                        reflectedMTOfRhoAndSubRegionHistDetector.Mean =
                            (double[,])FileIO.ReadArrayFromBinaryInResources<double>(filePath, projectName, ReflectedMTOfRhoAndSubregionHistDims);
                        return reflectedMTOfRhoAndSubRegionHistDetector;
                    case TallyType.ReflectedTimeOfRhoAndSubregionHist:
                        var reflectedTimeOfRhoAndSubregionHistDetector =
                            FileIO.ReadFromXMLInResources<ReflectedTimeOfRhoAndSubregionHistDetector>(filePath + ".xml", projectName);
                        var reflectedTimeOfRhoAndSubregionHistDims =
                            new int[] { reflectedTimeOfRhoAndSubregionHistDetector.Rho.Count, 
                                        reflectedTimeOfRhoAndSubregionHistDetector.SubregionIndices.Count, 
                                        reflectedTimeOfRhoAndSubregionHistDetector.Time.Count };
                        reflectedTimeOfRhoAndSubregionHistDetector.Mean =
                            (double[, ,])FileIO.ReadArrayFromBinaryInResources<double>(filePath, projectName, reflectedTimeOfRhoAndSubregionHistDims);
                        return reflectedTimeOfRhoAndSubregionHistDetector;

                    default:
                        throw new ArgumentOutOfRangeException("tallyType");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Problem reading detector information from resource file.\n\nDetails:\n\n" + e + "\n");
            }

            return null;
        }
        /// <summary>
        /// Reads Detector from file in resources using default name (TallyType.ToString).
        /// </summary>
        /// <param name="tallyType">TallyType of IDetector to be read</param>
        /// <param name="folderPath">path string of folder where file to be read resides</param>
        /// <param name="projectName">project name string where the file resides in resources</param>
        /// <returns>IDetector</returns>
        public static IDetector ReadDetectorFromFileInResources(TallyType tallyType, string folderPath, string projectName)
        {
            return ReadDetectorFromFileInResources(tallyType, tallyType.ToString(), folderPath, projectName);
        }
    }
}

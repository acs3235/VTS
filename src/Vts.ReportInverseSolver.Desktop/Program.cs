﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using Vts.IO;
using Vts.Factories;
using Vts.Modeling.ForwardSolvers;
using Vts.Modeling.Optimizers;
using Vts.Extensions;
using Vts.Common;

namespace Vts.ReportInverseSolver.Desktop
{
    class Program
    {
        static void Main(string[] args)
        {
            //resources path definition, common for all the reports:
            var projectName = "Vts.ReportInverseSolver.Desktop";
            var inputPath = @"..\..\Resources\";
            string currentAssemblyDirectoryName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            inputPath = currentAssemblyDirectoryName + "\\" + inputPath;
            
            //R(rho)
            //ChooseDataAndReportRofRho(projectName, inputPath);
            //R(rho,t)
            ChooseDataAndReportRofRhoAndT(projectName, inputPath);

        }

        private static void ChooseDataAndReportRofRho(string projectName, string inputPath)
        {
            //choose drho
            double[] drhos = { 0.25 };//mm
            // ratio detectors
            int[] rDs = { 1, 2, 4, 8 };
            // noise percentage
            double[] nPs = {0.0, 0.5, 1.0, 2.0, 4.0, 8.0, 10.0};
            //choose rho ranges
            var rhoRanges = new double[][]{
                                           new double[] {0.625,21.875},
                                           new double[] {0.625, 4.875},
                                           new double[] {5.125, 12.875},
                                           new double[] {13.125, 21.875},
                                          };
            //IFT
            InverseFitType[] IFTs = { InverseFitType.MuaMusp, InverseFitType.Mua, InverseFitType.Musp };//recovered op

            //fs definition
            var forwardSolverTypes = new ForwardSolverType[]
                      {
                          ForwardSolverType.Nurbs,
                          ForwardSolverType.PointSourceSDA,
                          //ForwardSolverType.DistributedPointSourceSDA,
                          //ForwardSolverType.MonteCarlo,
                          //ForwardSolverType.DistributedPointSDA,
                          //ForwardSolverType.DistributedGaussianSDA,
                          //ForwardSolverType.DeltaPOne,
                      };
            //optimizer definition
            var optimizerTypes = new OptimizerType[]
                      {
                          OptimizerType.MPFitLevenbergMarquardt,
                      };

            //optical properties definition
            var g = 0.8;
            var n = 1.4;
            //guess
            var guessMuas = new double[] { 0.0042, 0.0173, 0.0721 };//[mm-1]
            var guessMusps = new double[] { 0.875, 1.25, 1.625};//[mm-1]
            var guessOps =
                      from musp in guessMusps
                      from mua in guessMuas
                      select new OpticalProperties(mua, musp, g, n);
            //real
            var realMuas = new double[] { 0.001, 0.01, 0.03, 0.1, 0.3 };//[mm-1]
            var realMusps = new double[] { 0.5, 0.7, 1.0, 1.2, 1.5, 2.0 };//[mm-1]
            var realOps =
                      from musp in realMusps
                      from mua in realMuas
                      select new OpticalProperties(mua, musp, g, n);

            // console visualization
            bool stepByStep = false;//boolean variable used to proceed 'step by step' during the report, to view the output on the console window

            //execute
            foreach (var drho in drhos)
            {
                foreach (var rD in rDs)
                {
                    foreach (var nP in nPs)
                    {
                        foreach (var rhoRange in rhoRanges)
                        {
                            foreach (var IFT in IFTs)
                            {
                                ReportInverseSolverRofRho(drho, rhoRange, IFT, projectName, inputPath,
                                    forwardSolverTypes, optimizerTypes, guessOps, realOps, rD,nP, stepByStep);
                            }
                        }
                    }
                }
            }
            Console.WriteLine(" -------------- THE END for RofRho --------------");
            if (stepByStep) { Console.ReadLine(); }
        }

        private static void ChooseDataAndReportRofRhoAndT(string projectName,
                                                  string inputPath)
        {
            // noise percentage
            double[] nPs = { 0.0, 0.5, 1.0, 2.0, 4.0, 8.0, 10.0 };
            //choose data & run report for R(r,t)
            double[] dts = { 0.001, 0.005, 0.025 };//ps
            double[] riseMarkers = { 80.0, 50.0 };// % peak value
            double[] tailMarkers = { 20.0, 1.0, 0.1 };// % peak value
            string stDevMode = "A";// U = Uniform, A = Absolute, R = Relative //can be removed
            InverseFitType[] IFTs = { InverseFitType.MuaMusp, InverseFitType.Mua, InverseFitType.Musp };//recovered op
            bool stepByStep = false;//boolean variable used to proceed 'step by step' during the report, to view the output on the console window

            //fs definition
            //fs definition
            var forwardSolverTypes = new ForwardSolverType[]
                      {
                          ForwardSolverType.Nurbs,
                          ForwardSolverType.PointSourceSDA,
                          //ForwardSolverType.DistributedPointSourceSDA,
                          //ForwardSolverType.MonteCarlo,
                          //ForwardSolverType.DistributedPointSDA,
                          //ForwardSolverType.DistributedGaussianSDA,
                          //ForwardSolverType.DeltaPOne,
                      };
            //optimizer definition
            var optimizerTypes = new OptimizerType[]
                      {
                          OptimizerType.MPFitLevenbergMarquardt,
                      };
            //optical properties definition
            var g = 0.8;
            var n = 1.4;
            //guess
            var guessMuas = new double[] { 0.0042, 0.0173, 0.0721 };//[mm-1]
            var guessMusps = new double[] { 0.875, 1.25, 1.625 };//[mm-1]
            var guessOps =
                      from musp in guessMusps
                      from mua in guessMuas
                      select new OpticalProperties(mua, musp, g, n);
            //real
            var realMuas = new double[] { 0.001, 0.01, 0.03, 0.1, 0.3 };//[mm-1]
            var realMusps = new double[] { 0.5, 0.7, 1.0, 1.2, 1.5, 2.0 };//[mm-1]
            var realOps =
                      from musp in realMusps
                      from mua in realMuas
                      select new OpticalProperties(mua, musp, g, n);

            // s-d separations, match folders
            double[] rhos = new double[] { 0.375, 1.125, 2.125, 4.875, 9.875, 14.875, 19.875 };//[mm]

            foreach (var nP in nPs)
            {
                foreach (var dt in dts)
                {
                    foreach (var riseMarker in riseMarkers)
                    {
                        foreach (var tailMarker in tailMarkers)
                        {
                            foreach (var IFT in IFTs)
                            {
                                ReportInverseSolverRofRhoAndT(dt, riseMarker, tailMarker,
                                    stDevMode, IFT, projectName, inputPath, forwardSolverTypes,
                                    optimizerTypes, guessOps, realOps, rhos, nP, stepByStep);
                            }
                        }
                    }
                }
            }
            Console.WriteLine(" -------------- THE END for RofRhoAndT --------------");
            if (stepByStep) { Console.ReadLine(); }
        }

        private static void ReportInverseSolverRofRho(double drho,
                                                      double[] rhoRange,
                                                      InverseFitType IFT,
                                                      string projectName,
                                                      string inputPath,
                                                      ForwardSolverType[] forwardSolverTypes,
                                                      OptimizerType[] optimizerTypes,
                                                      IEnumerable<OpticalProperties> guessOps,
                                                      IEnumerable<OpticalProperties> realOps,
                                                      int ratioDetectors,
                                                      double noisePercentage,
                                                      bool stepByStep)
        {
            Console.WriteLine("#############################################");
            Console.WriteLine("####### REPORT INVERSE SOLVER: RofRho #######");
            Console.WriteLine("#############################################");
            //path definition
            string spaceDomainFolder = "Real";
            string timeDomainFolder = "SteadyState";
            string problemFolder = "drho" + drho.ToString() + "/" + "ratioD" + ratioDetectors.ToString() + "/" +
                                   "noise" + noisePercentage.ToString() + "/" + rhoRange[0].ToString() + "_" + rhoRange[1].ToString();
            problemFolder = problemFolder.Replace(".", "p");
            //rhos based on range
            int numberOfPoints = Convert.ToInt32((rhoRange[1] - rhoRange[0]) / drho) + 1;
            var rhos = new DoubleRange(rhoRange[0], rhoRange[1], numberOfPoints).AsEnumerable().ToArray();
            double[] R = new double[numberOfPoints];
            double[] S = new double[numberOfPoints];
            int firstInd = Convert.ToInt32((rhoRange[0] + drho / 2.0) / drho) - 1;
            int lastInd = Convert.ToInt32((rhoRange[1] + drho / 2) / drho) - 1;

            foreach (var fST in forwardSolverTypes)
            {
                Console.WriteLine("Forward Solver Type: {0}", fST.ToString());
                foreach (var oT in optimizerTypes)
                {
                    Console.WriteLine("Optimizer Type: {0}", oT.ToString());
                    if (stepByStep) { Console.WriteLine("Press enter to continue"); }
                    Console.WriteLine("=================================================");
                    if (stepByStep) { Console.ReadLine(); }

                    //double[] constantVals;

                    foreach (var rOp in realOps)
                    {
                        //output 
                        double bestMua = 0.0;
                        double meanMua = 0.0;
                        double guessBestMua = 0.0;
                        double bestMusp = 0.0;
                        double meanMusp = 0.0;
                        double guessBestMusp = 0.0;
                        double bestChiSquared = 10000000000000.0;//initialize very large to avoid if first
                        double meanChiSquared = 0.0;
                        DateTime start = new DateTime();//processing start time
                        DateTime end = new DateTime();//processing finish time
                        double elapsedSeconds;//processing time

                        //set filename based on real optical properties
                        var filename = "musp" + rOp.Musp.ToString() + "mua" + rOp.Mua.ToString();
                        filename = filename.Replace(".", "p");
                        Console.WriteLine("Looking for file {0}", filename);

                        if (File.Exists(inputPath + spaceDomainFolder + "/" + timeDomainFolder + "/" + filename + "R"))
                        {
                            Console.WriteLine("The file has been found");
                            //read binary files
                            var Rtot = (IEnumerable<double>)FileIO.ReadArrayFromBinaryInResources<double>
                                                  ("Resources/" + spaceDomainFolder + "/" + timeDomainFolder + "/" + filename + "R", projectName, 88);
                            var Stot = (IEnumerable<double>)FileIO.ReadArrayFromBinaryInResources<double>
                                                  ("Resources/" + spaceDomainFolder + "/" + timeDomainFolder + "/" + filename + "S", projectName, 88);
                            for (int i = firstInd; i <= lastInd; i++)
                            {   
                                R[i - firstInd] = Rtot.ToArray()[i];
                                S[i - firstInd] = Stot.ToArray()[i];
                            }
                            if (ratioDetectors != 1)
                            {
                                rhos = FilterArray(rhos, ratioDetectors);
                                R = FilterArray(R, ratioDetectors);
                                S = FilterArray(S, ratioDetectors);
                            }
                            if (noisePercentage != 0.0)
                            {
                                //why the extension for double[] is void?
                                R = (((IEnumerable<double>)R).AddNoise(noisePercentage)).ToArray();
                            }
                            start = DateTime.Now;
                            int covergedCounter = 0;
                            foreach (var gOp in guessOps)
                            {
                                bool converged;
                                //if fitting only one parameter change the guess to the true value
                                if (IFT == InverseFitType.Mua) { gOp.Musp = rOp.Musp; }
                                if (IFT == InverseFitType.Musp) { gOp.Mua = rOp.Mua; }
                                //solve inverse problem
                                double[] fit = ComputationFactory.ConstructAndExecuteVectorizedOptimizer(
                                                               fST, oT, SolutionDomainType.RofRho,
                                                               IndependentVariableAxis.Rho, rhos, R, S, gOp, IFT);
                                if (fit[0] != 0 && fit[1] != 0)
                                {
                                    converged = true;
                                }
                                else
                                {
                                    converged = false;
                                }
                                // fitted op
                                if (converged)
                                {
                                    OpticalProperties fOp = new OpticalProperties(fit[0], fit[1], gOp.G, gOp.N);
                                    //calculate chi squared and change values if it improved
                                    double chiSquared = EvaluateChiSquared(R, SolverFactory.GetForwardSolver(fST).RofRho(fOp.AsEnumerable(), rhos).ToArray(), S);
                                    if (chiSquared < bestChiSquared)
                                    {
                                        guessBestMua = gOp.Mua;
                                        bestMua = fit[0];
                                        guessBestMusp = gOp.Musp;
                                        bestMusp = fit[1];
                                        bestChiSquared = chiSquared;
                                    }
                                    meanMua += fit[0];
                                    meanMusp += fit[1];
                                    meanChiSquared += chiSquared;
                                    covergedCounter += 1;
                                }
                            }
                            end = DateTime.Now;
                            meanMua /= covergedCounter;
                            meanMusp /= covergedCounter;
                            meanChiSquared /= covergedCounter;
                            elapsedSeconds = (end - start).TotalSeconds;

                            MakeDirectoryIfNonExistent(new string[]{spaceDomainFolder, timeDomainFolder, problemFolder, fST.ToString(), oT.ToString(), IFT.ToString()});
                            //write results to array
                            double[] inverseProblemValues = FillInverseSolverValuesArray(bestMua, meanMua, guessBestMua,
                                                                                         bestMusp, meanMusp, guessBestMusp,
                                                                                         bestChiSquared, meanChiSquared,
                                                                                         elapsedSeconds, numberOfPoints);
                            // write array to binary
                            LocalWriteArrayToBinary<double>(inverseProblemValues, @"Output/" + spaceDomainFolder + "/" +
                                                            timeDomainFolder + "/" + problemFolder + "/" + fST.ToString() + "/" +
                                                            oT.ToString() + "/" + IFT.ToString() + "/" + filename, FileMode.Create);

                            Console.WriteLine("Real MUA = {0} - best MUA = {1} - mean MUA = {2}", rOp.Mua, bestMua, meanMua);
                            Console.WriteLine("Real MUSp = {0} - best MUSp = {1} - mean MUSp = {2}", rOp.Musp, bestMusp, meanMusp);
                            if (stepByStep) { Console.ReadLine(); }
                        }
                        else
                        {
                            Console.WriteLine("The file has not been found.");
                        }

                        Console.Clear();
                    }
                }
            }
        }

        private static void ReportInverseSolverRofRhoAndT(double dt,
                                                          double riseMarker,
                                                          double tailMarker,
                                                          string stDevMode,
                                                          InverseFitType IFT,
                                                          string projectName,
                                                          string inputPath,
                                                          ForwardSolverType[] forwardSolverTypes,
                                                          OptimizerType[] optimizerTypes,
                                                          IEnumerable<OpticalProperties> guessOps,
                                                          IEnumerable<OpticalProperties> realOps,
                                                          double[] rhos,
                                                          double noisePercentage, 
                                                          bool stepByStep)
        {
            Console.WriteLine("#############################################");
            Console.WriteLine("##### REPORT INVERSE SOLVER: RofRhoAndT #####");
            Console.WriteLine("#############################################");
            //path definition
            string spaceDomainFolder = "Real";
            string timeDomainFolder = "TimeDomain";
            string noiseFolder = "noise" + noisePercentage.ToString();
            string problemFolder =  "dt" + (dt * 1000).ToString() + "markers" + riseMarker.ToString() +
                                    tailMarker.ToString();
            problemFolder = problemFolder.Replace(".", "p");

            foreach (var fST in forwardSolverTypes)
            {

                //initialize forward solver
                Console.WriteLine("Forward Solver Type: {0}", fST.ToString());
                foreach (var oT in optimizerTypes)
                {
                    Console.WriteLine("Optimizer Type: {0}", oT.ToString());
                    foreach (var rho in rhos)
                    {
                        string rhoFolder = rho.ToString();
                        Console.WriteLine("=================================================");
                        Console.WriteLine("SOURCE DETECTOR SEPARETION: R = {0} mm", rhoFolder);
                        if (stepByStep) { Console.WriteLine("Press enter to continue"); }
                        Console.WriteLine("=================================================");
                        if (stepByStep) { Console.ReadLine(); }
                        rhoFolder = rhoFolder.Replace(".", "p");
                        rhoFolder = "rho" + rhoFolder;
                        double[] constantVals = { rho };

                        foreach (var rOp in realOps)
                        {
                            //output 
                            double bestMua = 0.0;
                            double meanMua = 0.0;
                            double guessBestMua = 0.0;
                            double bestMusp = 0.0;
                            double meanMusp = 0.0;
                            double guessBestMusp = 0.0;
                            double bestChiSquared = 10000000000000.0;//initialize very large to avoid if first
                            double meanChiSquared = 0.0;
                            DateTime start = new DateTime();//processing start time
                            DateTime end = new DateTime();//processing finish time
                            double elapsedSeconds;//processing time

                            //set filename based on real optical properties
                            var filename = "musp" + rOp.Musp.ToString() + "mua" + rOp.Mua.ToString();
                            filename = filename.Replace(".", "p");
                            Console.WriteLine("Looking for file {0}", filename);

                            if (File.Exists(inputPath + spaceDomainFolder + "/" + timeDomainFolder + "/" + problemFolder + "/" + rhoFolder + "/" + filename + "Range"))
                            {
                                Console.WriteLine("The file has been found for rho = {0} mm.", rho);
                                //read binary files
                                var timeRange = (double[])FileIO.ReadArrayFromBinaryInResources<double>
                                                      ("Resources/" + spaceDomainFolder + "/" + timeDomainFolder + "/" + problemFolder + "/" + rhoFolder + "/" + filename + "Range", projectName, 2);
                                int numberOfPoints = Convert.ToInt32((timeRange[1] - timeRange[0]) / dt) + 1;
                                var T = new DoubleRange(timeRange[0], timeRange[1], numberOfPoints).AsEnumerable().ToArray();
                                var R = (IEnumerable<double>)FileIO.ReadArrayFromBinaryInResources<double>
                                                      ("Resources/" + spaceDomainFolder + "/" + timeDomainFolder + "/" + problemFolder + "/" + rhoFolder + "/" + filename + "R", projectName, numberOfPoints);
                                var S = GetStandardDeviationValues("Resources/" + spaceDomainFolder + "/" + timeDomainFolder + "/" + problemFolder + "/" + rhoFolder + "/" + filename + "S",
                                                                   projectName, stDevMode, numberOfPoints, R.ToArray());
                                if (noisePercentage != 0.0)
                                {
                                    //why the extension for double[] is void?
                                    R = (((IEnumerable<double>)R).AddNoise(noisePercentage)).ToArray();
                                }
                                start = DateTime.Now;
                                int convergedCounter = 0;
                                foreach (var gOp in guessOps)
                                {
                                    bool converged;
                                    if (IFT == InverseFitType.Mua) { gOp.Musp = rOp.Musp; }
                                    if (IFT == InverseFitType.Musp) { gOp.Mua = rOp.Mua; }
                                    //solve inverse problem
                                    double[] fit = ComputationFactory.ConstructAndExecuteVectorizedOptimizer(
                                                                   fST, oT, SolutionDomainType.RofRhoAndT,
                                                                   IndependentVariableAxis.T, T, R, S, gOp,
                                                                   IFT, constantVals);
                                    if (fit[0] != 0 && fit[1] != 0)
                                    {
                                        converged = true;
                                    }
                                    else
                                    {
                                        converged = false;
                                    }
                                    if (converged)
                                    {
                                        OpticalProperties fOp = new OpticalProperties(fit[0], fit[1], gOp.G, gOp.N);
                                        //calculate chi squared and change values if it improved
                                        double chiSquared = EvaluateChiSquared(R.ToArray(), SolverFactory.GetForwardSolver(fST).RofRhoAndT(fOp.AsEnumerable(), rho.AsEnumerable(), T).ToArray(), S.ToArray());
                                        if (chiSquared < bestChiSquared)
                                        {
                                            guessBestMua = gOp.Mua;
                                            bestMua = fit[0];
                                            guessBestMusp = gOp.Musp;
                                            bestMusp = fit[1];
                                            bestChiSquared = chiSquared;
                                        }
                                        meanMua += fit[0];
                                        meanMusp += fit[1];
                                        meanChiSquared += chiSquared;
                                        convergedCounter += 1;
                                    }
                                }
                                end = DateTime.Now;
                                meanMua /= convergedCounter;
                                meanMusp /= convergedCounter;
                                meanChiSquared /= convergedCounter;
                                elapsedSeconds = (end - start).TotalSeconds;

                                MakeDirectoryIfNonExistent(new string[]{spaceDomainFolder, timeDomainFolder, noiseFolder, problemFolder, fST.ToString(), oT.ToString(), IFT.ToString(), rhoFolder});
                                //write results to array
                                double[] inverseProblemValues = FillInverseSolverValuesArray(bestMua, meanMua, guessBestMua,
                                                                                             bestMusp, meanMusp, guessBestMusp,
                                                                                             bestChiSquared, meanChiSquared,
                                                                                             elapsedSeconds, numberOfPoints);
                                // write array to binary
                                LocalWriteArrayToBinary<double>(inverseProblemValues, @"Output/" + spaceDomainFolder + "/" +
                                                                timeDomainFolder + "/" + noiseFolder + "/" + problemFolder + "/" + fST.ToString() + "/" +
                                                                oT.ToString() + "/" + IFT.ToString() + "/" + rhoFolder + "/" + filename, FileMode.Create);

                                Console.WriteLine("Real MUA = {0} - best MUA = {1} - mean MUA = {2}", rOp.Mua, bestMua, meanMua);
                                Console.WriteLine("Real MUSp = {0} - best MUSp = {1} - mean MUSp = {2}", rOp.Musp, bestMusp, meanMusp);
                                if (stepByStep) { Console.ReadLine(); }
                            }
                            else
                            {
                                Console.WriteLine("The file has not been found.");
                            }

                            Console.Clear();
                        }
                    }
                }
            }
        }

        private static double[] FillInverseSolverValuesArray(double bestMua, double meanMua,
                                                       double guessBestMua, double bestMusp,
                                                      double meanMusp, double guessBestMusp,
                                               double bestChiSquared, double meanChiSquared,
                                                     double elapsedSeconds, int numberOfPoints)
        {
            double[] inverseProblemValues = new double[10];

            inverseProblemValues[0] = bestMua;
            inverseProblemValues[1] = meanMua;
            inverseProblemValues[2] = guessBestMua;
            inverseProblemValues[3] = bestMusp;
            inverseProblemValues[4] = meanMusp;
            inverseProblemValues[5] = guessBestMusp;
            inverseProblemValues[6] = bestChiSquared;
            inverseProblemValues[7] = meanChiSquared;
            inverseProblemValues[8] = elapsedSeconds;
            inverseProblemValues[9] = numberOfPoints;

            return inverseProblemValues; ;
        }

        private static double[] FilterArray(double[] arrayIn, int ratioPointToUse)
        {
            int numberOfPoints = arrayIn.Count() / ratioPointToUse;
            if (ratioPointToUse != 2)
            {
                numberOfPoints += 1;
            }
            double[] arrayOut = new double[numberOfPoints];
            int j = 0;
            for (int i = 0; i < arrayIn.Count(); i += ratioPointToUse)
            {
                arrayOut[j] = arrayIn[i];
                j += 1;
            }
            return arrayOut;
        }

        private static IEnumerable<double> GetStandardDeviationValues(string path, string projectName, string stDevMode, int numberOfPoints, double[] R)
        {
            double[] S = (double[])FileIO.ReadArrayFromBinaryInResources<double>
                                                      (path, projectName, numberOfPoints);
            if (stDevMode == "U")
            {
                for (int i = 0; i < S.Length; i++)
                {
                    S[i] = 1.0;
                }
            }
            else if (stDevMode == "R")
            {
                for (int i = 0; i < S.Length; i++)
                {
                    S[i] = S[i] / R[i];
                }
            }
            return S;
        }

        private static void MakeDirectoryIfNonExistent(string[] folders)
        {
            string fullPath = @"Output/";
            for (int i = 0; i < folders.Length; i++)
            {
                fullPath += folders[i] + "/";
            }
            if (!(Directory.Exists( fullPath)))
            {
                Directory.CreateDirectory(fullPath);
            }
        }

        private static void LocalWriteArrayToBinary<T>(Array dataIN, string filename, FileMode mode) where T : struct
        {
            // Create a file to write binary data 
            using (Stream s = StreamFinder.GetFileStream(filename, mode))
            {
                using (BinaryWriter bw = new BinaryWriter(s))
                {
                    new ArrayCustomBinaryWriter<T>().WriteToBinary(bw, dataIN);
                }
            }
        }

        private static double EvaluateChiSquared(double[] measuredR, double[] modelR, double[] measuredS)
        {
            double chiSquared = 0.0;
            for (int i = 0; i < measuredR.Length; i++)
            {
                chiSquared += (measuredR[i] - modelR[i]) * (measuredR[i] - modelR[i]) / (measuredS[i] * measuredS[i]);
            }
            return chiSquared;
        }
    }
}

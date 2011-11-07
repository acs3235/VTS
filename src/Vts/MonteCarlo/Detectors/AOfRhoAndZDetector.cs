using System;
using System.Runtime.Serialization;
using System.Collections.Generic;
using System.Linq;
using Vts.Common;
using Vts.MonteCarlo.PhotonData;
using Vts.MonteCarlo.Helpers;
using Vts.MonteCarlo.Tissues;

namespace Vts.MonteCarlo.Detectors
{
    /// <summary>
    /// Implements IDetector&lt;double[,]&gt;.  Tally for Absorption(rho,z).
    /// </summary>
    [KnownType(typeof(AOfRhoAndZDetector))]
    public class AOfRhoAndZDetector : IHistoryDetector<double[,]>
    {
        private Func<PhotonDataPoint, PhotonDataPoint, int, double> _absorptionWeightingMethod;

        private ITissue _tissue;
        private bool _tallySecondMoment;

        /// <summary>
        /// Returns an instance of AOfRhoAndZDetector
        /// </summary>
        /// <param name="rho"></param>
        /// <param name="z"></param>
        /// <param name="tissue"></param>
        public AOfRhoAndZDetector(
            DoubleRange rho, 
            DoubleRange z, 
            ITissue tissue, 
            bool tallySecondMoment,
            String name 
            )
        {
            Rho = rho;
            Z = z;
            Mean = new double[Rho.Count - 1, Z.Count - 1];
            _tallySecondMoment = tallySecondMoment;
            SecondMoment = null;
            if (tallySecondMoment)
            {
                SecondMoment = new double[Rho.Count - 1, Z.Count - 1];
            }
            TallyType = TallyType.AOfRhoAndZ;
            Name = name;
            TallyCount = 0;
            _tissue = tissue;
            _absorptionWeightingMethod = AbsorptionWeightingMethods.GetAbsorptionWeightingMethod(tissue, this);
        }

        /// <summary>
        /// Returns a default instance of AOfRhoAndZDetector (for serialization purposes only)
        /// </summary>
        public AOfRhoAndZDetector()
            : this(new DoubleRange(), new DoubleRange(), new MultiLayerTissue(), true, TallyType.AOfRhoAndZ.ToString())
        {
        }

        [IgnoreDataMember]
        public double[,] Mean { get; set; }

        [IgnoreDataMember]
        public double[,] SecondMoment { get; set; }

        public TallyType TallyType { get; set; }

        public String Name { get; set; }

        public long TallyCount { get; set; }

        public DoubleRange Rho { get; set; }

        public DoubleRange Z { get; set; }

        public void Tally(Photon photon)
        {
            PhotonDataPoint previousDP = photon.History.HistoryData.First();
            foreach (PhotonDataPoint dp in photon.History.HistoryData.Skip(1))
            {
                TallySingle(previousDP, dp, _tissue.GetRegionIndex(dp.Position)); // unoptimized version, but HistoryDataController calls this once
                previousDP = dp;
            }
        }

        public void TallySingle(PhotonDataPoint previousDP, PhotonDataPoint dp, int currentRegionIndex)
        {
            var ir = DetectorBinning.WhichBin(DetectorBinning.GetRho(dp.Position.X, dp.Position.Y), Rho.Count - 1, Rho.Delta, Rho.Start);
            var iz = DetectorBinning.WhichBin(dp.Position.Z, Z.Count - 1, Z.Delta, Z.Start);

            var weight = _absorptionWeightingMethod(previousDP, dp, currentRegionIndex);

            if (weight != 0.0)
            {
                Mean[ir, iz] += weight;
                if (_tallySecondMoment)
                {
                    SecondMoment[ir, iz] += weight * weight;
                }
                TallyCount++;
            }
        }

        public void Normalize(long numPhotons)
        {
            var normalizationFactor = 2.0 * Math.PI * Rho.Delta * Z.Delta;
            for (int ir = 0; ir < Rho.Count - 1; ir++)
            {
                for (int iz = 0; iz < Z.Count - 1; iz++)
                {
                    var areaNorm = (Rho.Start + (ir + 0.5) * Rho.Delta) * normalizationFactor;
                    Mean[ir, iz] /= areaNorm * numPhotons;
                    if (_tallySecondMoment)
                    {
                        SecondMoment[ir, iz] /= areaNorm * areaNorm * numPhotons;
                    }
                }
            }
        }

        public bool ContainsPoint(PhotonDataPoint dp)
        {
            return true;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using Vts.Common;
using Vts.MonteCarlo.Helpers;
using Vts.MonteCarlo.PhotonData;

namespace Vts.MonteCarlo.TallyActions
{
    /// <summary>
    /// Implements ITerminationTally<double[]>.  Tally for reflectance as a function 
    /// of Angle.
    /// </summary>
    public class ROfAngleTally : ITerminationTally<double[]>
    {
        private DoubleRange _angle;

        public ROfAngleTally(DoubleRange angle)
        {
            _angle = angle;
            Mean = new double[_angle.Count - 1]; 
            SecondMoment = new double[_angle.Count - 1];
        }

        public bool ContainsPoint(PhotonDataPoint dp)
        {
            return (dp.StateFlag == PhotonStateType.ExitedOutTop);
        }

        public void Tally(PhotonDataPoint dp, IList<OpticalProperties> ops)
        {
            var totalTime = dp.SubRegionInfoList.Select((sub, i) =>
                DetectorBinning.GetTimeDelay(
                    sub.PathLength,
                    ops[i].N)
            ).Sum();

            var ia = DetectorBinning.WhichBin(Math.Acos(dp.Direction.Uz), _angle.Count - 1, _angle.Delta, _angle.Start);

            Mean[ia] += dp.Weight;
            SecondMoment[ia] += dp.Weight * dp.Weight;
        }

        public void Normalize(long numPhotons)
        {
            for (int ia = 0; ia < _angle.Count - 1; ia++)
            {
                Mean[ia] /= 2.0 * Math.PI * Math.Sin((ia + 0.5) * _angle.Delta) * _angle.Delta * numPhotons;
            }
        }

        public double[] Mean { get; set; }
        public double[] SecondMoment { get; set; }

    }
}

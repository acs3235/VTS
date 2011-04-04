﻿using System;
using Vts.Common;
using Vts.MonteCarlo.PhotonData;
using Vts.MonteCarlo.Helpers;

namespace Vts.MonteCarlo.Sources
{
    /// <summary>
    /// 
    /// </summary>
    public class LineSourceDivergingFlat : ISource
    {
        private double _numericalAperture;
        private double _polarAngle;
        private DoubleRange _azimuthalAngleEmissionRange;
        private Position _translationFromOrigin;
        private PolarAzimuthalAngles _rotationFromInwardNormal;
        private ThreeAxisRotation _rotationOfPrincipalSourceAxis;
         private SourceFlags _rotationAndTranslationFlags;
        private double _lineLength = 1.0;
        private double _height; 

        /// <summary>
        /// Returns an instance of Diverging Flat Line Source with a specified translation, inward normal rotation, and source axis rotation
        /// </summary>
        /// <param name="lineLength"></param>
        /// <param name="numericalAperture"></param>
        /// <param name="translationFromOrigin"></param>
        /// <param name="rotationFromInwardNormal"></param>
        /// <param name="rotationOfPrincipalSourceAxis"></param>
        public LineSourceDivergingFlat(
            double lineLength,
            double numericalAperture,
            Position translationFromOrigin,
            PolarAzimuthalAngles rotationFromInwardNormal,
            ThreeAxisRotation rotationOfPrincipalSourceAxis)
        {
            _translationFromOrigin = translationFromOrigin.Clone();
            _rotationFromInwardNormal = rotationFromInwardNormal.Clone();
            _rotationOfPrincipalSourceAxis = rotationOfPrincipalSourceAxis.Clone();
            _rotationAndTranslationFlags = new SourceFlags(true, true, true);
        }

        /// <summary>
        /// Returns an instance of Diverging Flat Line Source with a specified translation and inward normal rotation, but without source axis rotation
        /// </summary>
        /// <param name="lineLength"></param>
        /// <param name="numericalAperture"></param>
        /// <param name="translationFromOrigin"></param>
        /// <param name="rotationFromInwardnormal"></param>
        public LineSourceDivergingFlat(
            double lineLength,
            double numericalAperture,
            Position translationFromOrigin,
            PolarAzimuthalAngles rotationFromInwardnormal)
            : this(
                lineLength,
                numericalAperture,
                translationFromOrigin,
                rotationFromInwardnormal,
                new ThreeAxisRotation(0, 0, 0))
        {
            _rotationAndTranslationFlags = new SourceFlags(true, true, false);
        }

        /// <summary>
        /// Returns an instance of Diverging Flat Line Source with a specified translation and source axis rotation, but without inward normal rotation 
        /// </summary>
        /// <param name="lineLength"></param>
        /// <param name="numericalAperture"></param>
        /// <param name="translationFromOrigin"></param>
        /// <param name="rotationOfPrincipalSourceAxis"></param>
        public LineSourceDivergingFlat(
            double lineLength,
            double numericalAperture,
            Position translationFromOrigin,
            ThreeAxisRotation rotationOfPrincipalSourceAxis
            )
            : this(
                lineLength,
                numericalAperture,
                translationFromOrigin,
                new PolarAzimuthalAngles(0, 0),
                rotationOfPrincipalSourceAxis)
        {
            _rotationAndTranslationFlags = new SourceFlags(true, false, true);
        }

        /// <summary>
        /// Returns an instance of Diverging Flat Line Source with a specified translation but without inward normal rotation or source axis rotation
        /// </summary>
        /// <param name="lineLength"></param>
        /// <param name="numericalAperture"></param>
        /// <param name="translationFromOrigin"></param>
        public LineSourceDivergingFlat(
            double lineLength,
            double numericalAperture,
            Position translationFromOrigin)
            : this(
                lineLength,
                numericalAperture,
                translationFromOrigin,
                new PolarAzimuthalAngles(0, 0),
                new ThreeAxisRotation(0, 0, 0))
        {
            _rotationAndTranslationFlags = new SourceFlags(true, false, false);
        }

        /// <summary>
        /// Returns an instance of Diverging Flat Line Source with an inward normal rotation and source axis rotation
        /// </summary>
        /// <param name="lineLength"></param>
        /// <param name="numericalAperture"></param>
        /// <param name="rotationFromInwardnormal"></param>
        /// <param name="rotationOfPrincipalSourceAxis"></param>
        public LineSourceDivergingFlat(
            double lineLength,
            double numericalAperture,
            PolarAzimuthalAngles rotationFromInwardnormal,
            ThreeAxisRotation rotationOfPrincipalSourceAxis)
            : this(
                lineLength,
                numericalAperture,
                new Position(0, 0, 0),
                rotationFromInwardnormal,
                rotationOfPrincipalSourceAxis)
        {
            _rotationAndTranslationFlags = new SourceFlags(false, true, false);
        }


        /// <summary>
        /// Returns an instance of Diverging Flat Line Source with an inward normal rotation, but without source axis rotation
        /// </summary>
        /// <param name="lineLength"></param>
        /// <param name="numericalAperture"></param>
        /// <param name="rotationFromInwardnormal"></param>
        public LineSourceDivergingFlat(
            double lineLength,
            double numericalAperture,
            PolarAzimuthalAngles rotationFromInwardnormal)
            : this(
                lineLength,
                numericalAperture,
                new Position(0, 0, 0),
                rotationFromInwardnormal,
                new ThreeAxisRotation(0, 0, 0))
        {
            _rotationAndTranslationFlags = new SourceFlags(false, true, false);
        }

        /// <summary>
        /// Returns an instance of Diverging Flat Line Source with a source axis rotation, but without inward normal rotation
        /// </summary>
        /// <param name="lineLength"></param>
        /// <param name="numericalAperture"></param>
        /// <param name="rotationOfPrincipalSourceAxis"></param>
        public LineSourceDivergingFlat(
            double lineLength,
            double numericalAperture,
            ThreeAxisRotation rotationOfPrincipalSourceAxis)
            : this(
                lineLength,
                numericalAperture,
                new Position(0, 0, 0),
                new PolarAzimuthalAngles(0, 0),
                rotationOfPrincipalSourceAxis)
        {
            _rotationAndTranslationFlags = new SourceFlags(false, false, true);
        }

        /// <summary>
        /// Returns an instance of Diverging Flat Line Source with no inward normal rotation or source axis rotation  
        /// </summary>
        /// <param name="lineLength"></param>
        /// <param name="numericalAperture"></param>
        public LineSourceDivergingFlat(
            double lineLength,
            double numericalAperture)
            : this(
                lineLength,
                numericalAperture,
                new Position(0, 0, 0),
                new PolarAzimuthalAngles(0, 0),
                new ThreeAxisRotation(0, 0, 0))
        {
            _rotationAndTranslationFlags = new SourceFlags(false, false, false);
        }


        public Photon GetNextPhoton(ITissue tissue)
        {
            //Source starts from anywhere in the line
            Position finalPosition = SourceToolbox.GetRandomFlatLinePosition(new Position(0, 0, 0), 
                _lineLength, 
                Rng);

            //Calculate polar angle
            _azimuthalAngleEmissionRange = new DoubleRange(0.0, 2 * Math.PI);
            _height = 0.5 * _lineLength * Math.Tan(Math.Asin(_numericalAperture));
            _polarAngle = Math.Atan(finalPosition.X/_height);

            //Sample angular distribution
            Direction finalDirection = SourceToolbox.GetRandomAzimuthalAngle(_polarAngle, _azimuthalAngleEmissionRange, Rng);

            //Rotation and translation
            SourceToolbox.DoRotationandTranslationForGivenFlags(
                ref finalPosition,
                ref finalDirection,
                _translationFromOrigin,
                _rotationFromInwardNormal,
                _rotationOfPrincipalSourceAxis,
                _rotationAndTranslationFlags);

            // the handling of specular needs work
            var weight = 1.0 - Helpers.Optics.Specular(tissue.Regions[0].RegionOP.N, tissue.Regions[1].RegionOP.N);

            var dataPoint = new PhotonDataPoint(
                finalPosition,
                finalDirection,
                weight,
                0.0,
                PhotonStateType.NotSet);

            var photon = new Photon { DP = dataPoint };

            return photon;
        }

        #region Random number generator code (copy-paste into all sources)
        /// <summary>
        /// The random number generator used to create photons. If not assigned externally,
        /// a Mersenne Twister (MathNet.Numerics.Random.MersenneTwister) will be created with
        /// a seed of zero.
        /// </summary>
        public Random Rng
        {
            get
            {
                if (_rng == null)
                {
                    _rng = new MathNet.Numerics.Random.MersenneTwister(0);
                }
                return _rng;
            }
            set { _rng = value; }
        }
        private Random _rng;
        #endregion

    }
}

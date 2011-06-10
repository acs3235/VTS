﻿using System;
using Vts.Common;
using Vts.MonteCarlo.Interfaces;
using Vts.MonteCarlo.PhotonData;
using Vts.MonteCarlo.Helpers;
using Vts.MonteCarlo.Sources.SourceProfiles;

namespace Vts.MonteCarlo.Sources
{
    public abstract class SurfaceEmittingCylindricalFiberSourceBase : ISource
    {        
        protected Direction _newDirectionOfPrincipalSourceAxis;
        protected Position _translationFromOrigin;        
        protected SourceFlags _rotationAndTranslationFlags;
        protected double _fiberRadius;
        protected double _fiberHeightZ;
        protected double _curvedSurfaceEfficiency;
        protected double _bottomSurfaceEfficiency;
        
        protected SurfaceEmittingCylindricalFiberSourceBase(
            double fiberRadius,
            double fiberHeightZ,  
            double curvedSurfaceEfficiency,
            double bottomSurfaceEfficiency,
            Direction newDirectionOfPrincipalSourceAxis,                  
            Position translationFromOrigin)
        {
            _rotationAndTranslationFlags = new SourceFlags(
                 newDirectionOfPrincipalSourceAxis != SourceDefaults.DefaultDirectionOfPrincipalSourceAxis.Clone().Clone(),
                 translationFromOrigin != SourceDefaults.DefaultPosition.Clone(),
                 false);
            
            _fiberRadius = fiberRadius;
            _fiberHeightZ = fiberHeightZ;
            _curvedSurfaceEfficiency = curvedSurfaceEfficiency;
            _bottomSurfaceEfficiency = bottomSurfaceEfficiency;
            _newDirectionOfPrincipalSourceAxis = newDirectionOfPrincipalSourceAxis.Clone();
            _translationFromOrigin = translationFromOrigin.Clone();      
        }

        public Photon GetNextPhoton(ITissue tissue)
        {
            double curved = 2 * Math.PI * _fiberRadius * _fiberHeightZ * _curvedSurfaceEfficiency;
            double bottom = Math.PI * _fiberRadius * _fiberRadius * _bottomSurfaceEfficiency;

            Direction finalDirection;
            Position finalPosition;

            if (_fiberRadius > 0.0)
            {
                

                if (Rng.NextDouble() > bottom / (curved + bottom))
                {
                    //sample angular distribution
                    finalDirection = SourceToolbox.GetDirectionForGivenPolarAndAzimuthalAngleRangeRandom(
                        SourceDefaults.DefaultHalfPolarAngleRange.Clone().Clone(),
                        SourceDefaults.DefaultAzimuthalAngleRange.Clone().Clone(),
                        Rng);

                    //Translate the photon to _tubeRadius length below the origin. Ring lies on yz plane.
                    finalPosition = new Position(0.0, 0.0, _fiberRadius);

                    //Sample a ring that emits photons outside.
                    SourceToolbox.UpdateDireactionAndPositionAfterRotatingAroundXAxis(
                        2.0 * Math.PI * Rng.NextDouble(),
                        ref finalDirection,
                        ref finalPosition);

                    //Ring lies on xy plane. z= 0;
                    SourceToolbox.UpdateDireactionAndPositionAfterRotatingAroundYAxis(
                        0.5 * Math.PI,
                        ref finalDirection,
                        ref finalPosition);

                    //Sample tube height
                    finalPosition.Z = _fiberHeightZ * (2.0 * Rng.NextDouble() - 1.0);
                }
                else
                {
                    finalPosition = SourceToolbox.GetPositionInACircleRandomFlat(
                        SourceDefaults.DefaultPosition.Clone(),
                        0.0,
                        _fiberRadius,
                        Rng);

                    finalDirection = SourceToolbox.GetDirectionForGivenPolarAndAzimuthalAngleRangeRandom(
                        SourceDefaults.DefaultHalfPolarAngleRange.Clone().Clone(),
                        SourceDefaults.DefaultAzimuthalAngleRange.Clone().Clone(),
                        Rng);
                }
            }
            else                 
            {

                finalPosition = SourceToolbox.GetPositionInALineRandomFlat(
                        SourceDefaults.DefaultPosition.Clone(),
                        _fiberHeightZ,
                        Rng);

                finalDirection = SourceToolbox.GetDirectionForGivenPolarAndAzimuthalAngleRangeRandom(
                        SourceDefaults.DefaultFullPolarAngleRange.Clone().Clone(),
                        SourceDefaults.DefaultAzimuthalAngleRange.Clone().Clone(),
                        Rng);

                //Rotate 90degrees around y axis
                SourceToolbox.UpdateDireactionAndPositionAfterRotatingAroundYAxis(
                        0.5 * Math.PI,
                        ref finalDirection,
                        ref finalPosition);
            }


            //Find the relevent polar and azimuthal pair for the direction
            PolarAzimuthalAngles _rotationalAnglesOfPrincipalSourceAxis = SourceToolbox.GetPolarAndAzimuthalAnglesFromDirection(_newDirectionOfPrincipalSourceAxis);
            
            //Translation and source rotation
            SourceToolbox.UpdateDirectionAndPositionAfterGivenFlags(
                finalPosition,
                ref finalDirection,
                _rotationalAnglesOfPrincipalSourceAxis,
                _translationFromOrigin,                
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

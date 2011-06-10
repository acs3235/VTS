﻿using Vts.Common;
using Vts.MonteCarlo.Helpers;
using Vts.MonteCarlo.Interfaces;
using Vts.MonteCarlo.Sources.SourceProfiles;

namespace Vts.MonteCarlo.Sources
{
    public class DirectionalCircularSourceInput : ISourceInput
    {
        // this handles directional circular
        public DirectionalCircularSourceInput(
            double thetaConvOrDiv,            
            double outerRadius,
            double innerRadius,
            ISourceProfile sourceProfile,
            Direction newDirectionOfPrincipalSourceAxis,
            Position translationFromOrigin,
            PolarAzimuthalAngles beamRotationFromInwardNormal) 
        {
            SourceType = SourceType.DirectionalCircular;
            ThetaConvOrDiv = thetaConvOrDiv;
            OuterRadius = outerRadius;
            InnerRadius = innerRadius;
            SourceProfile = sourceProfile;
            NewDirectionOfPrincipalSourceAxis = newDirectionOfPrincipalSourceAxis;
            TranslationFromOrigin = translationFromOrigin;
            BeamRotationFromInwardNormal = beamRotationFromInwardNormal;
        }

        public DirectionalCircularSourceInput(
            double thetaConvOrDiv,
            double outerRadius,
            double innerRadius,
            ISourceProfile sourceProfile)
            : this(
                thetaConvOrDiv,
                outerRadius,
                innerRadius,
                sourceProfile,
                SourceDefaults.DefaultDirectionOfPrincipalSourceAxis.Clone().Clone(),
                SourceDefaults.DefaultPosition.Clone(),
                SourceDefaults.DefaultBeamRoationFromInwardNormal.Clone().Clone()) { }

        public SourceType SourceType { get; set; }
        public double ThetaConvOrDiv { get; set; }
        public double OuterRadius { get; set; }
        public double InnerRadius { get; set; }
        public ISourceProfile SourceProfile { get; set; }
        public Direction NewDirectionOfPrincipalSourceAxis { get; set; }
        public Position TranslationFromOrigin { get; set; }
        public PolarAzimuthalAngles BeamRotationFromInwardNormal { get; set; }
    }
}

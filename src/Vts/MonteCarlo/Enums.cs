namespace Vts.MonteCarlo
{
    /// <summary>
    /// All Monte Carlo enums.
    /// </summary>
    public enum PhotonStateType : byte
    {
        NotSet,
        ExitedOutTop,
        ExitedOutBottom,
        ExitedOutSides,
        Absorbed,
        KilledOverMaximumPathLength,
        KilledOverMaximumCollisions,
        KilledRussianRoulette,
        PseudoCollision,
    }
    // Source enums
    public enum SourceType
    {
        Planar,
        Point,
        Cylindrical,
        Line,
    }
    public enum BeamType
    {
        Gaussian,
        Flat,
    }
    public enum AngleDistributionType
    {
        Collimated,
        Isotropic,
        AngleDistributed,
    }
    public enum SourceOrientationType
    {
        Angled,
        Normal,
    }
    // Tissue enums
    public enum TissueType
    {
        MultiLayer,  // includes homogenous
        SingleEllipsoid,
    }
    // Detector enums
    public enum DetectorType
    {
        Detector,
        pMCDetector,
    }

    public enum TallyType
    {
        ROfRhoAndAngle,
        ROfRho,
        ROfAngle,
        ROfRhoAndOmega,
        ROfRhoAndTime,
        ROfXAndY,
        RDiffuse,
        TOfRhoAndAngle,
        TOfRho,
        TOfAngle,
        TDiffuse,
        FluenceOfRhoAndZ,
        pMuaMusInROfRhoAndTime, // maybs these should be in separate enum?
        pMuaMusInROfRho,
        pMuaInROfRhoAndTime, // do I need next two?
        pMusInROfRhoAndTime,
    }
}

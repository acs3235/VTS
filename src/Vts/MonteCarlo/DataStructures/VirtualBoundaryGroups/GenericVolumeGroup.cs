using System.Collections.Generic;

namespace Vts.MonteCarlo
{
    /// <summary>
    /// Virtual boundary group that contains surface detectors;
    /// </summary>
   public class GenericVolumeGroup : IVirtualBoundaryGroup
   {
       /// <summary>
       /// Surface Boundary Group constructor 
       /// </summary>
       public GenericVolumeGroup(VirtualBoundaryType type, IList<IDetectorInput> detectorInputs, bool writeToDatabase, string name)
       {
           DetectorInputs = detectorInputs;
           WriteToDatabase = writeToDatabase;
           VirtualBoundaryType = type;
           Name = name;
       }

       /// <summary>
       /// Surface Boundary Group default constructor provides R(rho) detector list
       /// </summary>
       public GenericVolumeGroup()
           : this(
                VirtualBoundaryType.GenericVolumeBoundary,
                new List<IDetectorInput> 
                { 
                    new ROfRhoDetectorInput(),
                },
                false,
                VirtualBoundaryType.GenericVolumeBoundary.ToString())
       {
       }

       public IList<IDetectorInput> DetectorInputs { get; set; }
       public bool WriteToDatabase { get; set; }
       public VirtualBoundaryType VirtualBoundaryType { get; set; }
       public string Name { get; set; }

   }
}

using System.Collections.Generic;

namespace BeamAnalysis.Abstract
{
    public interface IBeam
    {
        List<double[,]> Matrix { get; }
        List<IBeamProperty> Properties { get; }
    }
}

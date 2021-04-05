using System.Collections.Generic;

namespace BeamAnalysis.Abstract
{
    public interface ITopology
    {
        List<int[]> Matrix { get; }
    }
}

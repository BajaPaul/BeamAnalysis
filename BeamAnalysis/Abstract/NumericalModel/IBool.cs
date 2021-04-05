using System.Collections.Generic;

namespace BeamAnalysis.Abstract
{
    public interface IBool
    {
        List<int[,]> Matrix { get; }
        List<int[,]> MatrixTransposed { get; }
    }
}

using System.Collections.Generic;

namespace BeamAnalysis.Abstract
{
    /// <summary>
    /// ISchemeHelper interface methods and variables.
    /// </summary>
    public interface ISchemeHelper
    {
        /// <summary>
        /// ISchemeHelper method. Return list of nodes that contain a load.
        /// </summary>
        /// <returns></returns>
        List<INode> ListNodesWithLoad();
    }
}

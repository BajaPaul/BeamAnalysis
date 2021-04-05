namespace BeamAnalysis.Abstract
{
    /// <summary>
    /// IBcSupport interface methods and variables.
    /// </summary>
    public interface IBcSupport
    {
        /// <summary>
        /// Integer matrix that contains 2 DOF values for each node in model. At load nodes, both DOF values are always 0.
        /// At support nodes, first DOF value enables or disables vertical displacement at node, second DOF value enables or disables rotation at node.
        /// Enable displacement or rotation at node if DOF value is 0. Disable displacement or rotation at node if DOF value is 1.
        /// </summary>
        int[,] IntMatrixSupportDOFs { get; }
    }
}

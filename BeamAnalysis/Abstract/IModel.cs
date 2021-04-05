namespace BeamAnalysis.Abstract
{
    /// <summary>
    /// IModel interface methods and variables.
    /// </summary>
    public interface IModel
    {
        /// <summary>
        /// IModel variable. Number of nodes per element. Each element has a left node and right node. Value is 2.
        /// </summary>
        int IntNodesPerElement { get; }

        /// <summary>
        /// IModel variable. Number of degrees of freedom (DOF) per node. Value is 2. First DOF value allows or disables vertical displacement at node.
        /// Second DOF value allows or disables rotation at node. Allow displacement or rotation at node if DOF value is 0.
        /// Disable displacement or rotation at node if DOF value is 1.
        /// </summary>
        int IntDOFPerNode { get; }

        /// <summary>
        /// IModel variable. Total number of nodes in model.
        /// </summary>
        int IntModelNodes { get; }

        /// <summary>
        /// IModel variable. Total number of elements in model.
        /// </summary>
        int IntModelElements { get; }

        /// <summary>
        /// IModel variable.
        /// </summary>
        ITopology Topology { get; }

        /// <summary>
        /// IModel variable.
        /// </summary>
        IBool Boolean { get; }

        /// <summary>
        /// IModel variable.
        /// </summary>
        IBeam Beam { get; }

        /// <summary>
        /// IModel variable.
        /// </summary>
        IBcSupport Support { get; }

        /// <summary>
        /// IModel variable.
        /// </summary>
        IBcLoad Load { get; }

        /// <summary>
        /// IModel variable.
        /// </summary>
        IIdentity Identity { get; }

        /// <summary>
        /// IModel method.
        /// </summary>
        void AssemblyModel();
    }
}

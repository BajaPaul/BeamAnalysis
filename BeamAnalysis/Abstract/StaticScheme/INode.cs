namespace BeamAnalysis.Abstract
{
    /// <summary>
    /// INode interface methods and variables. This interface inherits IComponentBase methods and variables.
    /// A node can be a support position or load position. Model elements are created from nodes sequentially left to right using node number.
    /// A support node has two degrees-of-freedom (DOF). First DOF enables or disables vertical displacement at the node. Second DOF enables or disable rotation at the node.
    /// A load node can have a vertical force and/or a moment. Downward forces are negative. Clockwise moments are negative.
    /// </summary>
    public interface INode : IComponentBase
    {
        /// <summary>
        /// INode variable. Value of node position. Position is distance from left end of beam to node.
        /// </summary>
        double DoubleNodePosition { get; set; }

        /// <summary>
        /// True if support node. False if load node.
        /// </summary>
        bool BoolNodeSupport { get; set; }

        /// <summary>
        /// If boolNodeIsSupport=true, restrain vertical displacement at node if value is 1. Enable vertical displacement at node if value is 0;
        /// If node is a support, DOF values of 1 and 0 become matrix values. Any other values are invalid. Value not used if boolNodeIsSupport=false.
        /// </summary>
        int IntNodeDofDisplacement { get; set; }

        /// <summary>
        /// If boolNodeIsSupport=true, restrain rotation at node if value is 1. Enable rotation at node if value is 0;
        /// If node is a support, DOF values of 1 and 0 become matrix values. Any other values are invalid. Value not used if boolNodeIsSupport=false.
        /// </summary>
        int IntNodeDofRotation { get; set; }

        /// <summary>
        /// If boolNodeIsSupport=false then node is a load node. Value is vertical force to apply at node. Downward forces are negative.
        /// Value not used if boolNodeIsSupport=true. 
        /// </summary>
        double DoubleNodeForce { get; set; }

        /// <summary>
        /// If boolNodeIsSupport=false then node is a load node. Value is moment to apply at node. Clockwise moments are negative.
        /// Value not used if boolNodeIsSupport=true. 
        /// </summary>
        double DoubleNodeMoment { get; set; }
    }
}

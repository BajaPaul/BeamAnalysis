namespace BeamAnalysis.Abstract
{
    /// <summary>
    /// ILoad interface methods and variables. This interface inherits IComponentBase methods and variables.
    /// </summary>
    public interface ILoad : IComponentBase
    {
        /// <summary>
        /// ILoad variable. Value of node to place load at. A node can contain a vertical force and/or a moment.
        /// </summary>
        int IntNode { get; set; }

        /// <summary>
        /// ILoad variable. Value of vertical force for load. Downward forces are negative.
        /// </summary>
        double DoubleForce { get; set; }

        /// <summary>
        /// ILoad variable. Value of moment for load. Clockwise moments are negative.
        /// </summary>
        double DoubleMoment { get; set; }
    }
}

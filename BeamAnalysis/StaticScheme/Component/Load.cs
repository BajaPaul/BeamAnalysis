using BeamAnalysis.Abstract;

namespace BeamAnalysis.StaticScheme.Component
{
    /// <summary>
    /// Load class methods and variables. This class inherits ComponentBase methods and variables.
    /// Public names in this class implement the signatures defined in interface ILoad.
    /// </summary>
    internal class Load : ComponentBase, ILoad
    {
        public int IntNode { get; set; }
        public double DoubleForce { get; set; }
        public double DoubleMoment { get; set; }
    }
}

using BeamAnalysis.Abstract;

namespace BeamAnalysis.StaticScheme.Component
{
    /// <summary>
    /// Element class methods and variables. This class inherits ComponentBase methods and variables.
    /// Public names in this class implement the signatures defined in interface IElement.
    /// </summary>
    internal class Element : ComponentBase, IElement
    {
        public int IntNodeLeft { get; set; }
        public int IntNodeRight { get; set; }
        public int IntMaterial { get; set; }
        public int IntCrossSection { get; set; }
    }
}

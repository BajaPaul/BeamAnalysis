using BeamAnalysis.Abstract;

namespace BeamAnalysis.StaticScheme.Component
{
    /// <summary>
    /// CrossSection class methods and variables. This class inherits ComponentBase methods and variables.
    /// Public names in this class implement the signatures defined in interface ICrossSection.
    /// </summary>
    internal class CrossSection : ComponentBase, ICrossSection
    {
        public double DoubleAreaMomentOfInertia { get; set; }
    }
}

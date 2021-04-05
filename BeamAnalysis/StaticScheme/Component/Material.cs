using BeamAnalysis.Abstract;

// More info at: https://mechanicalc.com/reference/material-properties-tables

namespace BeamAnalysis.StaticScheme.Component
{
    /// <summary>
    /// Material class methods and variables. This class inherits ComponentBase methods and variables.
    /// Public names in this class implement the signatures defined in interface IMaterial.
    /// </summary>
    internal class Material : ComponentBase, IMaterial
    {
        public double DoubleYoungsModulus { get; set; }

        public double DoublePoissonsRatio { get; set; }
    }
}

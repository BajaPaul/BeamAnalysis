namespace BeamAnalysis.Abstract
{
    // More at:
    // https://en.wikipedia.org/wiki/Young%27s_modulus
    // https://en.wikipedia.org/wiki/Poisson%27s_ratio
    //
    // https://en.wikipedia.org/wiki/A36_steel
    // https://mechanicalc.com/reference/material-properties-tables

    /// <summary>
    /// IMaterial interface methods and variables. This interface inherits IComponentBase methods and variables.
    /// </summary>
    public interface IMaterial : IComponentBase
    {
        /// <summary>
        /// IMaterial variable. Value of Young's modulus (Ey) for material.
        /// </summary>
        double DoubleYoungsModulus { get; set; }

        /// <summary>
        /// IMaterial variable. Value of Poisson's ratio (ν) for material.
        /// </summary>
        double DoublePoissonsRatio { get; set; }
    }
}

namespace BeamAnalysis.Abstract
{
    // More at:  https://en.wikipedia.org/wiki/List_of_second_moments_of_area

    /// <summary>
    /// ICrossSection interface methods and variables. This interface inherits IComponentBase methods and variables.
    /// </summary>
    public interface ICrossSection : IComponentBase
    {
        /// <summary>
        /// ICrossSection variable. Value of area moment of inertia (Iy) for cross section.
        /// </summary>
        double DoubleAreaMomentOfInertia { get; set; }
    }
}

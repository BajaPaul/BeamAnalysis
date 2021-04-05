namespace BeamAnalysis.Abstract
{
    public interface IBeamProperty
    {
        /// <summary>
        /// IBeamProperty variable. Element number. Each element is assigned a number. First element number is always 1 versus 0.
        /// </summary>
        double Number { get; }      // For some reason this is a double versus an int. Maybe something to explore by changing to an int after backup made!!!

        /// <summary>
        /// IBeamProperty variable. Ey is Young's modulus, also known as Elastic Modulus (Pa).
        /// Sample: Value for ASTM A36 steel, Ey=29e6 psi = 200 Gpa = 2.0e11 Pa = 200000000000 Pa.
        /// </summary>
        double Ey { get; }

        // More at:  https://en.wikipedia.org/wiki/List_of_second_moments_of_area
        /// <summary>
        /// IBeamProperty variable. Iy is Second moment of area (m^4).
        /// </summary>
        double Iy { get; }

        /// <summary>
        /// IBeamProperty variable. Length of beam.
        /// </summary>
        double Lb { get; }
    }
}

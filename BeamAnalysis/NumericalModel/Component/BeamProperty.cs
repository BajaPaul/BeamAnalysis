using BeamAnalysis.Abstract;

namespace BeamAnalysis.NumericalModel
{
    internal class BeamProperty : IBeamProperty
    {
        public double Number { get; private set; }
        public double Ey { get; private set; }
        public double Iy { get; private set; }
        public double Lb { get; private set; }
        
        public BeamProperty(int intNumberElement, double doubleElasticity, double doubleInertia, double doubleBeamLength)
        {
            Number = intNumberElement;
            Ey = doubleElasticity;
            Iy = doubleInertia;
            Lb = doubleBeamLength;
        }
    }
}

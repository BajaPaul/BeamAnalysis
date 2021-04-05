using System;

namespace BeamAnalysis.CommonFEM
{
    /// <summary>
    /// Output from shear and moment calculations.
    /// </summary>
    public class ValueResult
    {
        /// <summary>
        /// Class constructor method.
        /// </summary>
        /// <param name="boolIsNode">True if position is a node, false otherwise.</param>
        /// <param name="doubleValueLeft">Result left of position.</param>
        /// <param name="doubleValueRight">Result right of position.</param>
        public ValueResult(bool boolIsNode = false, double doubleValueLeft = 0, double doubleValueRight = 0)
        {
            BoolIsNode = boolIsNode;
            DoubleValueLeft = doubleValueLeft;
            if (BoolIsNode)
            {
                DoubleValueRight = doubleValueRight;
                DoubleValueDifference = Math.Abs(doubleValueLeft - doubleValueRight);
            }
            else
            {
                DoubleValueRight = DoubleValueLeft;
                DoubleValueDifference = 0;
            }
        }

        /// <summary>
        /// True if position is a node, false otherwise.
        /// </summary>
        public bool BoolIsNode;

        /// <summary>
        /// Result value slightly left of node position if BoolIsNode is true.
        /// </summary>
        public double DoubleValueLeft;

        /// <summary>
        /// Result value slightly right of node position if BoolIsNode is true.
        /// </summary>
        public double DoubleValueRight;

        /// <summary>
        /// Absolute difference of DoubleValueLeft minus DoubleValueRight.
        /// </summary>
        public double DoubleValueDifference;
    }
}

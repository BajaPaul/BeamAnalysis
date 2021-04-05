using BeamAnalysis.Abstract;

namespace BeamAnalysis.StaticScheme.Component
{
    /// <summary>
    /// Node class methods and variables. This class inherits ComponentBase methods and variables.
    /// Public names in this class implement the signatures defined in interface INode.
    /// </summary>
    internal class Node : ComponentBase, INode
    {
        internal Coordinates Coordinate { get; set; }

        /// <summary>
        /// Degree of freedom value that enables or disables displacement or rotation at a support.
        /// A support will have two DoFs. One for displacement and one for rotation.
        /// Value of 0 enables displacement or rotation at support.
        /// Value of 1 disables displacement or rotation at support.
        /// </summary>
        internal DoFs DoF { get; set; }

        /// <summary>
        /// Class constructor method.
        /// </summary>
        public Node()
        {
            Coordinate = new Coordinates();
            DoF = new DoFs();
        }

        /*** INode interface methods and variables. ****************************************************************************/

        public double DoubleNodePosition
        {
            get
            {
                return Coordinate.X;
            }
            set
            {
                Coordinate = new Coordinates(value, Coordinate.Y);
            }
        }

        public bool BoolNodeSupport { get; set; }

        public int IntNodeDofDisplacement { get; set; }

        public int IntNodeDofRotation { get; set; }

        public double DoubleNodeForce { get; set; }

        public double DoubleNodeMoment { get; set; }

        public override void UpdateProperty()
        {
            DoF = new DoFs(IntNumber);      // IntNumber is number assigned to a node. First node number is always 1 versus 0.
        }

        /*** Node class methods and variables. *********************************************************************************/

        public double Y
        {
            get
            {
                return Coordinate.Y;
            }
            set
            {
                Coordinate = new Coordinates(Coordinate.X, value);
            }
        }

    }

    internal struct Coordinates
    {
        public double X;
        public double Y;

        public Coordinates(double x = 0, double y = 0)
        {
            X = x;
            Y = y;
        }
    }

    internal struct DoFs
    {
        public int intIndexNodeDisplacement;
        public int intIndexNodeRotation;

        /// <summary>
        /// Set matrix index numbers for node displacement and rotation restraints using number assigned to node.
        /// </summary>
        /// <param name="intNumber">Number assigned to a node. First node number is always 1 versus 0.</param>
        public DoFs(int intNumber)
        {
            intIndexNodeDisplacement = 2 * intNumber - 1;
            intIndexNodeRotation = 2 * intNumber;
        }
    }

}
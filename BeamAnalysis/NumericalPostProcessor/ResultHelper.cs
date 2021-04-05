using BeamAnalysis.Abstract;
using System.Diagnostics;
using System.Linq;

namespace BeamAnalysis.NumericalPostProcessor
{
    internal static class ResultHelper
    {
        /// <summary>
        /// Search iScheme.ListINode to determine if any INode positions in list match parameter position.
        /// Return INode number if match was found and is > 0, otherwise return -1.
        /// </summary>
        /// <param name="iScheme">Scheme that contains list of INodes used in beam.</param>
        /// <param name="doublePosition">Position on beam from left to right.</param>
        /// <returns></returns>
        public static int ExactResultExists(IScheme iScheme, double doublePosition)
        {
            INode iNodeMatch = iScheme.ListINode.Where(iNode => iNode.DoubleNodePosition == doublePosition).FirstOrDefault();
            if (iNodeMatch != null)
            {
                return iNodeMatch.IntNumber > 0 ? iNodeMatch.IntNumber : -1;    // Is condition true ? yes : no
            }
            return -1;
        }

        public static void GetBoundaryNodes(IScheme scheme, IModel model, double x, out int startNodeNumber, out int endNodeNumber)
        {
            startNodeNumber = -1;
            endNodeNumber = -1;

            for (int i = 0; i < model.IntModelNodes; i++)
            {
                INode currentNode = scheme.ListINode[i];
                if (currentNode.DoubleNodePosition > x)
                {
                    endNodeNumber = currentNode.IntNumber;
                    startNodeNumber = currentNode.IntNumber - 1;
                    break;
                }
            }
        }

        public static int GetElementNumberContainingNodes(IScheme scheme, int startNodeNumber, int endNodeNumber)
        {
            IElement currentElement = scheme.ListIElement
                .Where(element => (element.IntNodeLeft == startNodeNumber && element.IntNodeRight == endNodeNumber))
                .FirstOrDefault();

            return currentElement.IntNumber > 0 ? currentElement.IntNumber : -1;
        }

        public static double GetScalingParameter(IScheme scheme, double x, int startNodeNumber, double L)
        {
            INode currentStartNode = scheme.ListINode.Where(node => node.IntNumber == startNodeNumber).FirstOrDefault();
            return (x - currentStartNode.DoubleNodePosition) / L;
        }
    }
}

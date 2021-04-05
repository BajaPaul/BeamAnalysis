using BeamAnalysis.Abstract;
using System.Collections.Generic;
using System.Diagnostics;

namespace BeamAnalysis.NumericalModel
{
    /// <summary>
    /// BcSupport class methods and variables. This class inherits IComponentBuilder methods and variables.
    /// Public names in this class implement the signatures defined in interface IBcSupport.
    /// </summary>
    internal class BcSupport : IComponentBuilder<BcSupport>, IBcSupport
    {
        private readonly Model BcSupportModel;

        /// <summary>
        /// Class constructor method.
        /// </summary>
        /// <param name="model"></param>
        public BcSupport(Model model)
        {
            BcSupportModel = model;
        }
        
        public int[,] IntMatrixSupportDOFs { get; private set; }

        /// <summary>
        /// IComponentBuilder method. Build Integer matrix that contains 2 DOF values for each node in model and save result to IntMatrixSupportDOFs.
        /// At load nodes, both DOF values are always 0. At support nodes, first DOF value enables or disables vertical displacement at node,
        /// second DOF value enables or disables rotation at node. Enable displacement or rotation at node if DOF value is 0.
        /// Disable displacement or rotation at node if DOF value is 1.
        /// </summary>
        public void SetComponent()
        {
            List<INode> listINode = BcSupportModel.AbstractIScheme.ListINode;   // Get list of nodes in model.

            // Debug.WriteLine($"\nBcSupport.SetComponent(): listINode.Count={listINode.Count}");
            // foreach (INode iNodeItem in listINode)
                // Debug.WriteLine($"  DoubleNodePosition={iNodeItem.DoubleNodePosition}, IntNumber={iNodeItem.IntNumber}, BoolNodeSupport={iNodeItem.BoolNodeSupport}, StringName={iNodeItem.StringName}, IntNodeDofDisplacement={iNodeItem.IntNodeDofDisplacement}, IntNodeDofRotation={iNodeItem.IntNodeDofRotation}, DoubleNodeForce={iNodeItem.DoubleNodeForce}, DoubleNodeMoment={iNodeItem.DoubleNodeMoment}");

            IntMatrixSupportDOFs = new int[BcSupportModel.IntModelNodes * BcSupportModel.IntDOFPerNode, 1];
            int i = 0;
            foreach (INode iNode in listINode)
            {
                if (iNode.BoolNodeSupport)
                {
                    IntMatrixSupportDOFs[i++, 0] = iNode.IntNodeDofDisplacement;
                    IntMatrixSupportDOFs[i++, 0] = iNode.IntNodeDofRotation;
                }
                else
                {
                    IntMatrixSupportDOFs[i++, 0] = 0;
                    IntMatrixSupportDOFs[i++, 0] = 0;
                }
            }

            // foreach (int intValue in IntMatrixSupportDOFs)
                // Debug.WriteLine($"BcSupport.SetComponent(): intValue={intValue}");
        }

        public void SetDedicatedData() { }

        public BcSupport Get()
        {
            return this;
        }
    }
}

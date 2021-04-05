using BeamAnalysis.Abstract;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

/// <summary>
/// SchemeHelper class methods and variables. This class inherits ISchemeHelper methods and variables.
/// Public names in this class implement the signatures defined in interface ISchemeHelper.
/// </summary>
namespace BeamAnalysis.StaticScheme
{
    internal class SchemeHelper : ISchemeHelper
    {
        private readonly IScheme _iScheme;

        /// <summary>
        ///  Class constructor method.
        /// </summary>
        /// <param name="iScheme"></param>
        public SchemeHelper(IScheme iScheme)
        {
            _iScheme = iScheme;
        }

        public List<INode> ListNodesWithLoad()
        {
            // Search ListILoad for node numbers and then return a list that contains matching INodes.
            IEnumerable<int> iNodeNumbers = _iScheme.ListILoad.Where(iLoad => iLoad.IntNode != 0).Select(iLoadNode => iLoadNode.IntNode);
            List<INode> listNodesWithLoad = _iScheme.ListINode.Where(iNode => iNodeNumbers.Contains(iNode.IntNumber)).ToList();

            // Debug.WriteLine($"\nSchemeHelper.cs.ListINodeWithLoads(): INodes with loads. listNodesWithLoad.Count={listNodesWithLoad.Count}");
            // foreach (INode iNode in listNodesWithLoad)
                // Debug.WriteLine($"  iNode Values: IntNumber={iNode.IntNumber}, StringName={iNode.StringName}, DoublePosition={iNode.DoubleNodePosition}");

            return listNodesWithLoad;
        }
    }
}

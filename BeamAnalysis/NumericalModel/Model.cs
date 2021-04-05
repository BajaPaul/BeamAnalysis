using System.Diagnostics;
using System.Linq;
using BeamAnalysis.Abstract;

namespace BeamAnalysis.NumericalModel
{
    /// <summary>
    /// Model class methods and variables.
    /// Public names in this class implement the signatures defined in interface IModel.
    /// </summary>
    public class Model : IModel
    {
        internal IScheme AbstractIScheme { get; private set; }
        public int IntNodesPerElement { get; private set; }
        public int IntDOFPerNode { get; private set; }
        public int IntModelNodes { get; private set; }
        public int IntModelElements { get; private set; }

        /// <summary>
        /// Class constructor method.
        /// </summary>
        /// <param name="scheme"></param>
        public Model(IScheme scheme)
        {
            AbstractIScheme = scheme;
            IntNodesPerElement = 2;
            IntDOFPerNode = 2;
            IntModelNodes = scheme.ListINode.Count();
            IntModelElements = scheme.ListIElement.Count();
            // Debug.WriteLine($"\nModel class constructor: IntModelNodes={IntModelNodes}, IntModelElements={IntModelElements}");
        }

        /*** IModel interface methods and variables. ***************************************************************************/

        public ITopology Topology { get; private set; }
        public IBool Boolean { get; private set; }
        public IBeam Beam { get; private set; }
        public IBcSupport Support { get; private set; }
        public IBcLoad Load { get; private set; }
        public IIdentity Identity { get; private set; }

        public void AssemblyModel()
        {
            ComponentBuilder<Topology> CreatorTopology = new ComponentBuilder<Topology>(new Topology(this));
            CreatorTopology.Set();
            Topology = CreatorTopology.Get();

            ComponentBuilder<Bool> CreatorBool = new ComponentBuilder<Bool>(new Bool(this));
            CreatorBool.Set();
            Boolean = CreatorBool.Get();

            ComponentBuilder<Beam> CreatorBeam = new ComponentBuilder<Beam>(new Beam(this));
            CreatorBeam.Set();
            Beam = CreatorBeam.Get();

            ComponentBuilder<BcSupport> CreatorSupport = new ComponentBuilder<BcSupport>(new BcSupport(this));
            CreatorSupport.Set();
            Support = CreatorSupport.Get();

            ComponentBuilder<BcLoad> CreatorLoad = new ComponentBuilder<BcLoad>(new BcLoad(this));
            CreatorLoad.Set();
            Load = CreatorLoad.Get();

            ComponentBuilder<Identity> CreatorIdentity = new ComponentBuilder<Identity>(new Identity(this));
            CreatorIdentity.Set();
            Identity = CreatorIdentity.Get();
        }

    }
}

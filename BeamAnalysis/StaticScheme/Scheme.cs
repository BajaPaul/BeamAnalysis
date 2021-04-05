using System.Collections.Generic;
using BeamAnalysis.Abstract;
using BeamAnalysis.StaticScheme.Component;

namespace BeamAnalysis.StaticScheme
{
    /// <summary>
    /// Scheme class methods and variables. Public names in this class implement the signatures defined in interface IScheme.
    /// </summary>
    public class Scheme : IScheme
    {
        /// <summary>
        /// Class constructor method.
        /// </summary>
        public Scheme()
        {
            ListIMaterial = new List<IMaterial>();
            ListICrossSection = new List<ICrossSection>();
            ListINode = new List<INode>();
            ListIElement = new List<IElement>();
            ListILoad = new List<ILoad>();
            SchemeHelper = new SchemeHelper(this);
        }

        /*** IScheme interface methods and variables. *******************************************************************/

        public List<IMaterial> ListIMaterial { get; private set; }
        public List<ICrossSection> ListICrossSection { get; private set; }
        public List<INode> ListINode { get; private set; }
        public List<IElement> ListIElement { get; private set; }
        public List<ILoad> ListILoad { get; private set; }
        public ISchemeHelper SchemeHelper { get; private set; }

        public IMaterial IMaterialNew()
        {
            return new Material();
        }

        public ICrossSection ICrossSectionNew()
        {
            return new CrossSection();
        }

        public INode INodeNew()
        {
            return new Node();
        }

        public IElement IElementNew()
        {
            return new Element();
        }

        public ILoad ILoadNew()
        {
            return new Load();
        }

    }
}

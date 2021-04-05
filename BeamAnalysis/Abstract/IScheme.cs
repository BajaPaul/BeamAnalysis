using System.Collections.Generic;

namespace BeamAnalysis.Abstract
{
    /// <summary>
    /// IScheme interface methods and variables.
    /// </summary>
    public interface IScheme
    {
        /// <summary>
        /// IScheme variable. List of Material class items.
        /// </summary>
        List<IMaterial> ListIMaterial { get; }

        /// <summary>
        /// IScheme variable. List of CrossSection class items.
        /// </summary>
        List<ICrossSection> ListICrossSection { get; }

        /// <summary>
        /// IScheme variable. List of Node class items.
        /// </summary>
        List<INode> ListINode { get; }

        /// <summary>
        /// IScheme variable. List of Element class items.
        /// </summary>
        List<IElement> ListIElement { get; }

        /// <summary>
        /// IScheme variable. List of Load class items.
        /// </summary>
        List<ILoad> ListILoad { get; }

        /// <summary>
        /// IScheme variable. Value of helper method for scheme.
        /// </summary>
        ISchemeHelper SchemeHelper { get; }

        /// <summary>
        /// IScheme method. Return new instance of Material class.
        /// </summary>
        /// <returns></returns>
        IMaterial IMaterialNew();

        /// <summary>
        /// IScheme method. Return new instance of CrossSection class.
        /// </summary>
        /// <returns></returns>
        ICrossSection ICrossSectionNew();

        /// <summary>
        /// IScheme method. Return new instance of Node class.
        /// </summary>
        /// <returns></returns>
        INode INodeNew();

        /// <summary>
        /// IScheme method. Return new instance of Element class.
        /// </summary>
        /// <returns></returns>
        IElement IElementNew();

        /// <summary>
        /// IScheme method. Return new instance of Load class.
        /// </summary>
        /// <returns></returns>
        ILoad ILoadNew();
    }
}

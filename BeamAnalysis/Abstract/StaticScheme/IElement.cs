namespace BeamAnalysis.Abstract
{
    /// <summary>
    /// IElement interface methods and variables. This interface inherits IComponentBase methods and variables.
    /// </summary>
    public interface IElement : IComponentBase
    {
        /// <summary>
        /// IElement variable. Value of node number at left end of element.
        /// </summary>
        int IntNodeLeft { get; set; }

        /// <summary>
        /// IElement variable. Value of node number at right end of element.
        /// </summary>
        int IntNodeRight { get; set; }

        /// <summary>
        /// IElement variable. Value of material number for element.
        /// </summary>
        int IntMaterial { get; set; }

        /// <summary>
        /// IElement variable. Value of cross section number for element.
        /// </summary>
        int IntCrossSection { get; set; }
    }
}

namespace BeamAnalysis.Abstract
{
    /// <summary>
    /// ISolver interface methods and variables.
    /// </summary>
    public interface ISolver
    {
        /// <summary>
        /// ISolver variable. Matrix containing global stiffness values.
        /// </summary>
        double[,] DoubleMatrixStiffnessGlobal { get; }

        /// <summary>
        /// ISolver variable. Matrix containing global stiffness values with applied boundary conditions.
        /// </summary>
        double[,] DoubleMatrixBoundaryGlobal { get; }

        /// <summary>
        /// ISolver variable. Matrix containing load values. Each load can have a vertical force and/or a moment.
        /// Downward forces and clockwise moments are negative.
        /// </summary>
        double[,] DoubleMatrixLoads { get; }

        /// <summary>
        /// ISolver variable. Matrix containing displacement results.
        /// </summary>
        double[,] DoubleMatrixDisplacements { get; }

        /// <summary>
        /// ISolver variable. Matrix containing reaction results.
        /// </summary>
        double[,] DoubleMatrixReactions { get; }

        /// <summary>
        /// ISolver method. Assemble the finite element model.
        /// </summary>
        void AssembleFiniteElementModel();

        /// <summary>
        /// ISolver method. Calculate displacements and reactions of finite element model.
        /// </summary>
        void Run();
    }
}

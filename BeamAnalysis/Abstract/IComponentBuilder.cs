namespace BeamAnalysis.Abstract
{
    /// <summary>
    /// IComponentBuilder interface methods and variables.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IComponentBuilder<T>
    {
        /// <summary>
        /// IComponentBuilder method.
        /// </summary>
        void SetComponent();

        /// <summary>
        /// IComponentBuilder method.
        /// </summary>
        void SetDedicatedData();

        /// <summary>
        /// IComponentBuilder method.
        /// </summary>
        /// <returns></returns>
        T Get();
    }
}

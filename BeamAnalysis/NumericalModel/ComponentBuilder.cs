using BeamAnalysis.Abstract;

namespace BeamAnalysis.NumericalModel
{
    /// <summary>
    /// ComponentBuilder class methods and variables.
    /// Public names in this class implement the signatures defined in interface IComponentBuilder.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class ComponentBuilder<T>
    {
        private readonly IComponentBuilder<T> component;

        /// <summary>
        /// Class constructor method.
        /// </summary>
        /// <param name="component"></param>
        public ComponentBuilder(IComponentBuilder<T> component)
        {
            this.component = component;
        }

        public void Set()
        {
            component.SetComponent();
            component.SetDedicatedData();
        }

        public T Get()
        {
            return (T)(object)component.Get();
        }
    }
}

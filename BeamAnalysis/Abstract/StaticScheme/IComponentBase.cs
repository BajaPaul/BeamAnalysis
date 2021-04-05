namespace BeamAnalysis.Abstract
{
    // More about interfaces: https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/interface

    /// <summary>
    /// IComponentBase interface methods and variables. This is a base interface.
    /// Methods and variables in this interface are inherited by other interfaces.
    /// </summary>
    public interface IComponentBase
    {
        /// <summary>
        /// IComponentBase variable inherited by other interfaces. Each component is assigned a number. First component number is always 1 versus 0.
        /// </summary>
        int IntNumber { get; set; }

        /// <summary>
        /// IComponentBase variable inherited by other interfaces. Each component is assigned a name.
        /// </summary>
        string StringName { get; set; }

        /// <summary>
        /// IComponentBase method inherited by other interfaces. Each component is assigned a method to update property.
        /// </summary>
        void UpdateProperty();
    }
}

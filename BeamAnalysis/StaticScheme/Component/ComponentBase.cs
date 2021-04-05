namespace BeamAnalysis.StaticScheme.Component
{
    // More about class inheriting another class and interface:
    // https://stackoverflow.com/questions/2059425/in-c-can-a-class-inherit-from-another-class-and-an-interface

    /// <summary>
    /// ComponentBase class methods and variables. This is a base class.
    /// Methods and variables in this class are inherited by other classes.
    /// Public names in this class implement the signatures defined in interface IComponentBase. Note: This class does not inherit IComponentBase!
    /// </summary>
    internal abstract class ComponentBase
    {
        private int intNumber;

        /// <summary>
        /// ComponentBase variable inherited by other classes. Each component is assigned a number. First component number is always 1 versus 0.
        /// </summary>
        public int IntNumber
        {
            get
            {
                return intNumber;
            }
            set
            {
                intNumber = value;
                UpdateProperty();
            }
        }

        /// <summary>
        /// ComponentBase variable inherited by other classes. Each component is assigned a name.
        /// </summary>
        public string StringName { get; set; }

        // The virtual keyword is used to modify a method, property, indexer, or event declaration and allow for it to be overridden in a derived class.
        // More at: https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/virtual

        /// <summary>
        /// ComponentBase method inherited by other classes. Each component is assigned a method to update property.
        /// </summary>
        public virtual void UpdateProperty()
        {
        }

    }
}

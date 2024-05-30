namespace EIS.Runtime.Interfaces
{
    /// <summary>
    /// This interface defines methods for enabling and disabling functionality of an object.
    /// </summary>
    public interface ITogglable
    {
        /// <summary>
        /// Enables the functionality of the object.
        /// </summary>
        public void Enable();

        /// <summary>
        /// Disables the functionality of the object.
        /// </summary>
        public void Disable();
    }
}
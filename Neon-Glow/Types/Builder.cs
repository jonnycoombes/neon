namespace JCS.Neon.Glow.Types
{
    /// <summary>
    /// Simple interface which should be defined for classes implementing the builder pattern
    /// </summary>
    public interface Builder<out T>
    {
        /// <summary>
        /// Builds the target type
        /// </summary>
        /// <returns>An instance of the type which this builder knows how to build</returns>
        T Build();
    }
}
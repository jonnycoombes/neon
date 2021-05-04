namespace JCS.Neon.Glow.Statics.Crypto
{
    public class EncodingException : System.Exception
    {
        public EncodingException(string? message) : base(message)
        {
        }

        public EncodingException(string? message, System.Exception? innerException) : base(message, innerException)
        {
        }
    }
}
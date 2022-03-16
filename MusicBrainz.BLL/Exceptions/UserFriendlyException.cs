using System.Runtime.Serialization;

namespace MusicBrainz.BLL.Exceptions
{
    public class UserFriendlyException : Exception
    {
        public UserFriendlyException() : base()
        {
        }

        public UserFriendlyException(string? message) : base(message)
        {
        }

        public UserFriendlyException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected UserFriendlyException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}

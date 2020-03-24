using System;
using System.Runtime.Serialization;

namespace Leaderboard.Areas.Uploads.Exceptions
{
    public enum FilePart
    {
        FileName,
        Content
    }

    public class MultipartBindingException : Exception
    {
        public MultipartBindingException()
        {
        }

        public MultipartBindingException(string message) : base(message)
        {
        }

        public MultipartBindingException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected MultipartBindingException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}

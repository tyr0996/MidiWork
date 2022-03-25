using System;
using System.Collections.Generic;
using System.Text;

namespace MidiWork.Exceptions
{
    class InvalidHeaderChunkException : Exception
    {
        public InvalidHeaderChunkException()
        {
        }

        public InvalidHeaderChunkException(string message)
            : base(message)
        {
        }

        public InvalidHeaderChunkException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }

}

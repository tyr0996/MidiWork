using System;
using System.Collections.Generic;
using System.Text;

namespace MidiWork.Exceptions
{
    class InvalidTrackChunkSizeException : Exception
    {
        public InvalidTrackChunkSizeException()
        {
        }

        public InvalidTrackChunkSizeException(string message)
            : base(message)
        {
        }

        public InvalidTrackChunkSizeException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }

}

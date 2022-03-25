using System;
using System.Collections.Generic;
using System.Text;

namespace MidiWork.Exceptions
{
    class InvalidMIDITypeException : Exception
    {
        public InvalidMIDITypeException()
        {
        }

        public InvalidMIDITypeException(string message)
            : base(message)
        {
        }

        public InvalidMIDITypeException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }

}

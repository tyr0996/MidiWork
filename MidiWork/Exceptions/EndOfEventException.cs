using System;
using System.Collections.Generic;
using System.Text;

namespace MidiWork.Exceptions
{
    class EndOfEventException : Exception
    {
        public EndOfEventException()
        {
        }

        public EndOfEventException(string message) : base(message)
        {
        }

        public EndOfEventException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}

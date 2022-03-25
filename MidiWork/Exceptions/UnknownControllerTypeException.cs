using System;
using System.Collections.Generic;
using System.Text;

namespace MidiWork.Exceptions
{
    class UnknownControllerTypeException : Exception
    {
        public UnknownControllerTypeException()
        {
        }

        public UnknownControllerTypeException(string message)
            : base(message)
        {
        }

        public UnknownControllerTypeException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }

}
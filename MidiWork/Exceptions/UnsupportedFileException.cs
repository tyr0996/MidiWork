using System;
using System.Collections.Generic;
using System.Text;

namespace MidiWork.Exceptions
{
    class UnsupportedFileException : Exception
    {
        public UnsupportedFileException()
        {
        }

        public UnsupportedFileException(string message)
            : base(message)
        {
        }

        public UnsupportedFileException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }

}
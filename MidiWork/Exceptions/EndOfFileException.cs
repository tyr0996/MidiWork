using System;
using System.Runtime.Serialization;

namespace MidiWork.Event
{
    [Serializable]
    internal class EndOfFileException : Exception
    {
        public EndOfFileException()
        {
        }

        public EndOfFileException(string message) : base(message)
        {
        }

        public EndOfFileException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
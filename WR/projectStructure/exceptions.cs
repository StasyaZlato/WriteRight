using System;

namespace ProjectStructure
{

    [System.Serializable]
    public class IncorrectNameOfSectionException : Exception
    {
        public IncorrectNameOfSectionException() { }

        public IncorrectNameOfSectionException(string message) : base(message) { }

        public IncorrectNameOfSectionException(string message, Exception inner) : base(message, inner) { }

        protected IncorrectNameOfSectionException(System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}

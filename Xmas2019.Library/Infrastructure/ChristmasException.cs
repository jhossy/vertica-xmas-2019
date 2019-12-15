using System;

namespace Xmas2019.Library.Infrastructure
{
    public class ChristmasException : Exception
    {
        public ChristmasException()
        {

        }

        public ChristmasException(string message) : base(message)
        {

        }

        public ChristmasException(string message, Exception inner) : base(message, inner)
        {

        }
    }
}

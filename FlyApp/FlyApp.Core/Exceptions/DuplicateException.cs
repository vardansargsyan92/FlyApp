using System;

namespace FlyApp.Core.Exceptions
{
    public class DuplicateException : Exception
    {
        public DuplicateException(string message) : base(message)
        {
        }
    }
}
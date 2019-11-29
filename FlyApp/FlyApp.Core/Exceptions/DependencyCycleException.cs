using System;
using System.Collections.Generic;

namespace FlyApp.Core.Exceptions
{
    public class DependencyCycleException : Exception
    {
        public DependencyCycleException(string message, List<List<Type>> cycles) : base(message)
        {
            Cycles = cycles;
        }

        public List<List<Type>> Cycles { get; }
    }
}
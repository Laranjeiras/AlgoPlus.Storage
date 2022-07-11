using System;

namespace AlgoPlus.Storage.Exceptions
{
    public class ServiceNotFoundException : Exception
    {
        public ServiceNotFoundException(string message) : base(message)
        {
        }
    }
}

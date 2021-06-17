using System;

namespace Sakuno.ING.Game
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
    internal sealed class ApiAttribute : Attribute
    {
        public string Api { get; }

        public ApiAttribute(string api)
        {
            Api = api;
        }
    }
}

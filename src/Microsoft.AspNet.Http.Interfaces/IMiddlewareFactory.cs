using System;
using Microsoft.Framework.Runtime;

namespace Microsoft.AspNet.Http
{
    [AssemblyNeutral]
    public interface IMiddlewareFactory
    {
        object CreateInstance(IServiceProvider serviceProvider, Type middlewareType, object[] parameters);
    }
}
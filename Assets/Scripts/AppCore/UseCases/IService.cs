using System;

namespace AppCore.UseCases
{
    public interface IService : IDisposable
    {
        public string Name { get; }
    }
}
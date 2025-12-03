using System;

namespace DIExtensionsDemo
{
    public interface ISingletonService { Guid Id { get; } }
    public interface IScopedService { Guid Id { get; } }
    public interface ITransientService { Guid Id { get; } }

    internal class LifetimeService : ISingletonService, IScopedService, ITransientService
    {
        public Guid Id { get; } = Guid.NewGuid();
    }
}

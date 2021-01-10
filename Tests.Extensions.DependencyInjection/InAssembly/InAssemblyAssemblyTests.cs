﻿using Bitz.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Samples.Extensions.DependencyInjection.Contracts;
using Samples.Extensions.DependencyInjection.Engines.Contracts;
using Xunit;

namespace Tests.Extensions.DependencyInjection.InAssembly
{
    public class InAssemblyAssemblyTests : TestBase
    {
        protected override void OnRegister(IServiceCollection services)
        {
            services.Register
            (r => r
                .InAssembly(typeof(IEngine).Assembly)
                .Implementing<IShippingEngine>()
                .AllInterfaces()
                .AsTransient()
                .ConfigureOrThrow()
            );
        }

        [Fact]
        public void InAssembly_Assembly_GetInstanceByContract()
        {
            this.AssertInstance<IShippingEngine>();
        }

        [Fact]
        public void InAssembly_Assembly_GetInstanceByBaseContract()
        {
            this.AssertInstance<IEngine>();
        }
    }
}

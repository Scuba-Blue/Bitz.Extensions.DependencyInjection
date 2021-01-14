﻿using Bitz.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Samples.Extensions.DependencyInjection.Contracts;
using Samples.Extensions.DependencyInjection.Engines.Contracts;
using Xunit;

namespace Tests.Extensions.DependencyInjection.ImplementationType
{
    public class ImplementingTests : TestBase
    {
        protected override void OnRegister(IServiceCollection services)
        {
            services.Register
            (r => r
                .InAssemblyOf<IEngine>()
                .Implementing<ITaxEngine>()
                .AllInterfaces()
                .AsTransient()
                .Configure()
            );
        }

        [Fact]
        public void Type_Implementing_GetInstanceByContract()
        {
            this.AssertInstance<ITaxEngine>();
        }

        [Fact]
        public void Type_Implementing_GetInstanceByBaseContract()
        {
            this.AssertInstance<IEngine>();
        }
    }
}
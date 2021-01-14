﻿using Bitz.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Samples.Extensions.DependencyInjection.Contracts;
using Tests.Extensions.DependencyInjection.InAssembly.Services.Contracts;
using Xunit;

namespace Tests.Extensions.DependencyInjection.InAssembly
{
    public class InThisAssemblyTests : TestBase
    {
        protected override void OnRegister(IServiceCollection services)
        {
            services.Register
            (r => r
                .InThisAssembly()
                .Implementing<IMessageService>()
                .AllInterfaces()
                .AsTransient()
                .ConfigureOrThrow()
            );
        }

        [Fact]
        public void InThisAssemblyGetInstanceByContract()
        {
            this.AssertInstance<IMessageService>();
        }

        [Fact]
        public void InThisAssembly_GetInstanceByBaseContract()
        {
            this.AssertInstance<IService>();
        }
    }
}
﻿using Bitz.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Samples.Extensions.DependencyInjection.Contracts;
using Samples.Extensions.DependencyInjection.Engines.Contracts;
using System;
using Xunit;

namespace Tests.Extensions.DependencyInjection.Interfaces
{
    public class OnlyInterfaceTests : TestBase
    {
        protected override void OnRegister(IServiceCollection services)
        {
            services.Register
            (r => r
                .InAssemblyOf<IEngine>()
                .Implementing<IShippingEngine>()
                .OnlyInterface<IShippingEngine>()
                .AsTransient()
                .ConfigureOrThrow()
            );
        }

        [Fact]
        public void Interfaces_OnlyInterface()
        {
            this.AssertInstance<IShippingEngine>();
        }

        [Fact]
        public void Interfaces_OnlyInterface_BaseInterface_Exception()
        {
            Assert.Throws<InvalidOperationException>(() => this.AssertInstance<IEngine>());
        }
    }
}
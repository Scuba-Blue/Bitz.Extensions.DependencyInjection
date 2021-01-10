﻿using Bitz.Extensions.DependencyInjection.Contracts;
using Bitz.Extensions.DependencyInjection.Exceptions;
using Bitz.Extensions.DependencyInjection.Implementation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Bitz.Extensions.DependencyInjection.Configuration
{
    internal partial class Registrar
    : IConfigure
    {
        /// <summary>
        /// Configure selected services.
        /// </summary>
        public void Configure()
        {
            List<Type> types = SelectTypes(_assembly, _basedOn);

            AssertOnlyInterface(types, _interfaces as OnlyContract);

            (new Configurator())
                .Configure
                (
                    _services, 
                    types, 
                    _interfaces, 
                    _lifetime
                );
        }

        private void AssertOnlyInterface(List<Type> types, OnlyContract onlyContract)
        {
            Type type = onlyContract?.Interface;

            if (types.All(t => type == null || type.IsAssignableFrom(t) == true) == false)
            {
                string result = types.Aggregate
                    (
                        new StringBuilder(),
                        (curr, next) => curr.Append
                        (
                            (curr.Length == 0 ? string.Empty : next.FullName)
                        )
                        .AppendLine(next.FullName)
                    )
                    .ToString();

                throw new OnlyInterfaceException
                (
                    "\n"
                    + $"unable to cast registered types to {onlyContract.Interface.FullName}"
                    + "\n"
                    + "-- <registered types> --"
                    + "\n"
                    + result
                );
            }
        }

        /// <summary>
        /// Ensures that the configuration is valid.
        /// </summary>
        /// <exception cref="OnlyInterfaceException">thrown if the configuration finds no types to register.</exception>
        public void ConfigureOrThrow()
        {
            List<Type> types = SelectTypes(_assembly, _basedOn);

            AssertTypeToRegister(types);
            AssertOnlyInterface(types, _interfaces as OnlyContract);

            (new Configurator())
                .Configure
                (
                    _services,
                    types,
                    _interfaces,
                    _lifetime
                );
        }

        /// <summary>
        /// assert that there are types to register.
        /// </summary>
        /// <param name="types"></param>
        private void AssertTypeToRegister(List<Type> types)
        {
            if (types.Count() == 0)
            {
                throw new NoTypesException("no types found to configure.");
            }
        }

        /// <summary>
        /// Get all types from that match the desired implementation type.
        /// </summary>
        /// <param name="assembly">Selected assembly.</param>
        /// <param name="implementationType">Selected implementation type.</param>
        /// <returns>All types that match the implementation type 
        /// within the assembly.</returns>
        private List<Type> SelectTypes
        (
            Assembly assembly,
            _ImplementationType implementationType
        )
        {
            return GetTypesImplementing
            (
                InstantiableTypesIn(assembly),
                implementationType
            );
        }

        /// <summary>
        /// Get all types from the selected assembly that are 
        /// not interfaces nor abstract.
        /// </summary>
        /// <param name="assembly">Selected assembly.</param>
        /// <returns>All instantiable types.</returns>
        private List<Type> InstantiableTypesIn
        (
            Assembly assembly
        )
        {
            return assembly
                .Modules
                .SelectMany
                (m => m
                    .GetTypes()
                    .Where
                    (t =>
                        t.IsInterface == false
                        && t.IsAbstract == false
                    )
                )
                .ToList();
        }

        /// <summary>
        /// Get types from list that conform to the selected implementation type.
        /// </summary>
        /// <param name="types"></param>
        /// <param name="implementationType"></param>
        /// <returns>Types for the selected implementation type.</returns>
        private List<Type> GetTypesImplementing
        (
            List<Type> types,
            _ImplementationType implementationType
        )
        {
            List<Type> selected = null;

            selected ??= SelectTypes(types, implementationType as BasedOn);
            selected ??= SelectTypes(types, implementationType as Implementing);
            selected ??= SelectTypes(types, implementationType as Only);

            return selected;            
        }

        /// <summary>
        /// Select types that are based-on a specific type.
        /// </summary>
        /// <param name="types">Instantiable types within the specified assembly.</param>
        /// <param name="basedOn">Types based-on a specific type.</param>
        /// <returns>List of types that are based-on a specific type.</returns>
        private List<Type> SelectTypes
        (
            List<Type> types,
            BasedOn basedOn
        )
        {
            return 
                basedOn == null 
                ? null 
                : types
                .Where
                (t => 
                    t.IsAbstract == false
                    && t.IsGenericType == false 
                    && DerivesFrom(t, basedOn.Type) == true
                )
                .ToList();
        }

        /// <summary>
        /// See if the class can be casted to the other.
        /// </summary>
        /// <param name="source">Source type.</param>
        /// <param name="derivesFrom">Type to that source could be derived from.</param>
        /// <returns>True if the class can be casted.</returns>
        private bool DerivesFrom
        (
            Type source, 
            Type derivesFrom
        )
        {
            bool isSubClassOf = false;

            while (source != null && source != typeof(object))
            {
                Type current = source.IsGenericType == true
                    ? source.GetGenericTypeDefinition() 
                    : source;

                if (current == derivesFrom)
                {
                    isSubClassOf = true;
                    break;
                }

                source = source.BaseType;
            }

            return isSubClassOf;
        }

        /// <summary>
        /// Select types that are implementing a specific interface.
        /// </summary>
        /// <param name="types">List of instantiable types.</param>
        /// <param name="implementing"></param>
        /// <returns>All types that implement a specific interface.</returns>
        private List<Type> SelectTypes
        (
            List<Type> types,
            Implementing implementing
        )
        {
            return 
                implementing == null 
                ? null 
                : types
                    .Where(t => implementing.Type.IsAssignableFrom(t) == true)
                    .ToList();
        }

        /// <summary>
        /// Get the specified type from the list of types.
        /// </summary>
        /// <param name="types">List of instantiable types.</param>
        /// <param name="only">Specific type to select.</param>
        /// <returns>The specified type from the list of instantiable types.</returns>
        private List<Type> SelectTypes
        (
            List<Type> types,
            Only only
        )
        {
            return 
                only == null 
                ? null 
                : types
                    .Where(t => t == only.Type)
                    .ToList();
        }
    }
}
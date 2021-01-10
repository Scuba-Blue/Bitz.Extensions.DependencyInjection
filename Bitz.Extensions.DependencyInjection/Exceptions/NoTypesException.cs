﻿namespace Bitz.Extensions.DependencyInjection.Exceptions
{
    /// <summary>
    /// thrown when the configuration finds no types.
    /// </summary>
    public class NoTypesException : BitzExceptionBase
    {
        /// <summary>
        /// must be constructed with a message.
        /// </summary>
        /// <param name="message">exception message.</param>
        public NoTypesException(string message)
        : base(message)
        { }
    }
}
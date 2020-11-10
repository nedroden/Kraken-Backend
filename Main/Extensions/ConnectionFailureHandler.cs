using System;
using Kraken.Api.Main.Services;
using Kraken.Api.Main.Models;

namespace Kraken.Api.Main.Extensions
{
    /// <summary>
    /// Used for automatically reconnecting after a timeout.
    /// </summary>
    public static class ConnectionFailureHandler
    {
        /// <summary>
        /// Wraps around an action and automatically reconnects upon a TimeoutException or ObjectDiposedException.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="action">The action to wrap around.</param>
        /// <typeparam name="T">The data model (equal to T in Service{T}).</typeparam>
        /// <exception cref="Exception">Thrown if any additional errors occur.</exception>
        public static void ReconnectOnFailure<T>(this Service<T> service, Action action) where T : IDatabaseModel
        {
            try
            {
                action();
            }
            catch (Exception exception)
            {
                if (exception is TimeoutException || exception is ObjectDisposedException)
                {
                    service.Reconnect();
                    action();

                    return;
                }

                throw;
            }
        }

        /// <summary>
        /// Wraps around an action and automatically reconnects upon a TimeoutException or ObjectDiposedException.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="action">The action to wrap around.</param>
        /// <typeparam name="T">The data model (equal to T in Service{T}).</typeparam>
        /// <typeparam name="R">The return type of the action.</typeparam>
        /// <exception cref="Exception">Thrown if any additional errors occur.</exception>
        /// <returns>The result of the action.</returns>
        public static R ReconnectOnFailure<T, R>(this Service<T> service, Func<R> action) where T : IDatabaseModel
        {
            try
            {
                return action();
            }
            catch (Exception exception)
            {
                if (exception is TimeoutException || exception is ObjectDisposedException)
                {
                    service.Reconnect();
                    return action();
                }

                throw;
            }
        }
    }
}
using System;
using System.Threading;

namespace Kraken.Api.Sensor.Util
{
    /// <summary>
    /// Allows for functions to be retried a certain number of times.
    /// </summary>
    /// <typeparam name="ExceptionType"></typeparam>
    public class RetryableTask<ExceptionType> where ExceptionType : Exception
    {
        private Action _action;
        private int _attempts;
        private int _backoffTimeInMs;

        /// <summary>
        /// Creates a new retryable task with a maximum of three attempts and an initial backoff time of 10s.
        /// </summary>
        /// <param name="action">The action to run.</param>
        public RetryableTask(Action action) : this(action, 3) { }

        /// <summary>
        /// Creates a new retryable task with an initial backoff time of 10s.
        /// </summary>
        /// <param name="action">The action to run.</param>
        /// <param name="attempts">A maximum number of attempts.</param>
        public RetryableTask(Action action, int attempts) : this(action, attempts, 10000) { }

        /// <summary>
        /// Creates a new retryable task.
        /// </summary>
        /// <param name="action"></param>
        /// <param name="attempts"></param>
        /// <param name="backoffTimeInMs"></param>
        public RetryableTask(Action action, int attempts, int backoffTimeInMs)
        {
            _action = action;
            _attempts = attempts;
            _backoffTimeInMs = backoffTimeInMs;
        }

        /// <summary>
        /// Executes the task.
        /// </summary>
        /// <param name="errorMessage">An error message to display upon failure.</param>
        public void Execute(string errorMessage)
        {
            Execute(errorMessage, 0);
        }

        /// <summary>
        /// Executes the task.
        /// </summary>
        /// <param name="errorMessage">An error message to display upon failure.</param>
        /// <param name="attempt">The current attempt.</param>
        public void Execute(string errorMessage, int attempt)
        {
            int additionalBackoffTimeInMs = 5000;
            _attempts--;

            try
            {
                _action();
            }
            catch (ExceptionType)
            {
                int backoffTime = _backoffTimeInMs + attempt * additionalBackoffTimeInMs;

                if (_attempts == 0)
                {
                    throw;
                }

                Console.Error.WriteLine($"{errorMessage} - Trying again in {backoffTime} ms");

                Thread.Sleep(backoffTime);
                Execute(errorMessage, attempt + 1);
            }
        }
    }
}
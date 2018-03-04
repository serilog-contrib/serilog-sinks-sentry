using System;

namespace Serilog
{
    public static class ExceptionExtensions
    {
        private const string CapturedKey = "CapturedBySink";

        /// <summary>
        ///     Checks if the exception was already captured by the logger.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <returns>Returns true if the exception was captured, otherwise false.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="exception" /> is <see langword="null" /></exception>
        public static bool CheckIfCaptured(this Exception exception)
        {
            if (exception == null)
            {
                throw new ArgumentNullException(nameof(exception));
            }

            return exception.Data.Contains(CapturedKey);
        }

        internal static void SetCaptured(this Exception exception)
        {
            if (exception == null)
            {
                return;
            }

            exception.Data[CapturedKey] = true;
        }
    }
}

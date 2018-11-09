using System.Text.RegularExpressions;
using SharpRaven.Logging;

namespace SentryWeb.Scrubbing
{
    /// <summary>
    /// Silly example of replacing zero with hero when the divide by zero exception is thrown.
    /// </summary>
    public class DivisionByZeroFilter : IFilter
    {
        private static readonly Regex phoneRegex =
            new Regex("zero", RegexOptions.Compiled);

        public string Filter(string input)
        {
            return phoneRegex.Replace(input, "##-HERO-##");
        }
    }
}
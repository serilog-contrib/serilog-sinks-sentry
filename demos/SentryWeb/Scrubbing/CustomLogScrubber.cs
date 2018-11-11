using System.Collections.Generic;
using System.Linq;
using SharpRaven.Logging;
using SharpRaven.Logging.Filters;
using SharpRaven.Utilities;

namespace SentryWeb.Scrubbing
{
    public class CustomLogScrubber : IScrubber
    {
        private readonly List<IFilter> _filters;

        public CustomLogScrubber()
        {
            _filters = new List<IFilter>
            {
                new CreditCardFilter(), // from RavenClient it self
                new DivisionByZeroFilter() // our example scrubber
            };
        }

        public string Scrub(string input)
        {
            if (SystemUtil.IsNullOrWhiteSpace(input))
            {
                return input;
            }

            return _filters.Aggregate(input, (current, f) => f.Filter(current));
        }
    }
}
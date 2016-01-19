using System;
using System.Text.RegularExpressions;
using System.Linq;
using Toubab.Beinder.Extend;

namespace Toubab.Beinder.Path
{
    public class CamelCasePathParser : IPathParser
    {
        readonly Regex _re = new Regex(@"(^[^A-Z]+|\G[A-Z]+[^A-Z]*)", RegexOptions.CultureInvariant);

        public Path Parse(string name)
        {
            var fragments = string.IsNullOrWhiteSpace(name) ? new string[0] : _re.Matches(name).Cast<Match>().Select(m => m.Value).ToArray();

            for (int i = 0; i < fragments.Length; i++)
            {
                fragments[i] = fragments[i].ToLowerInvariant();
            }
            return new Path(fragments);
        }

    }

}


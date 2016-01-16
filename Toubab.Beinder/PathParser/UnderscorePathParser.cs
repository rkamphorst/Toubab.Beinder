using System;
using System.Text.RegularExpressions;
using System.Linq;
using Toubab.Beinder.Extend;

namespace Toubab.Beinder.PathParser
{

    public class UnderscorePathParser : IPathParser
    {
        readonly Regex _re = new Regex(@"\G_*([^_]+)", RegexOptions.CultureInvariant);

        public Path Parse(string name)
        {
            var fragments = string.IsNullOrWhiteSpace(name) ? new string[0] : _re.Matches(name).Cast<Match>().Select(m => m.Groups[1].Value).ToArray();

            for (int i = 0; i < fragments.Length; i++)
            {
                fragments[i] = fragments[i].ToLowerInvariant();
            }
            return new Path(fragments);
        }
    }
}

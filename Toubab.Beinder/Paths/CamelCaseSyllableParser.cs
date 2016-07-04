namespace Toubab.Beinder.Paths
{
    using System;
    using System.Text.RegularExpressions;
    using System.Linq;

    public class CamelCaseSyllableParser : ISyllableParser
    {
        readonly Regex _re = new Regex(@"(^[^A-Z]+|\G[A-Z]+[^A-Z]*)", RegexOptions.CultureInvariant);

        public Syllables Parse(string name)
        {
            return new Syllables(string.IsNullOrWhiteSpace(name) ? null : _re.Matches(name).Cast<Match>().Select(m => m.Value));
        }

    }

}


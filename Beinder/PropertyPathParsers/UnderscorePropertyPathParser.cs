using System;
using System.Text.RegularExpressions;
using System.Linq;

namespace Beinder.PropertyPathParsers
{

    public class UnderscorePropertyPathParser : IPropertyPathParser
    {
        readonly Regex _re = new Regex(@"\G_*([^_]+)", RegexOptions.CultureInvariant);

        public PropertyPath Parse(string name)
        {
            var fragments = string.IsNullOrWhiteSpace(name) ? new string[0] : _re.Matches(name).Cast<Match>().Select(m => m.Groups[1].Value).ToArray();

            for (int i = 0; i < fragments.Length; i++)
            {
                fragments[i] = fragments[i].ToLowerInvariant();
            }
            return new PropertyPath(fragments);
        }
    }
}

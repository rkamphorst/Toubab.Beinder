namespace Toubab.Beinder.Paths
{
    using System;
    using System.Linq;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Path parser for paths with underscore ("_") as a word separator.
    /// </summary>
    public class UnderscoreSyllableParser : IFragmentParser
    {
        readonly Regex _re = new Regex(@"\G_*([^_]+)", RegexOptions.CultureInvariant);

        /// <inheritdoc />
        public Fragment Parse(string name)
        {
            return new Fragment(string.IsNullOrWhiteSpace(name) ? null : _re.Matches(name).Cast<Match>().Select(m => m.Groups[1].Value));
        }
    }
}

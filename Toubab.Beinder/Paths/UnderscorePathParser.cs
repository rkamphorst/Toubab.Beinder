namespace Toubab.Beinder.Paths
{
    using System;
    using System.Linq;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Path parser for paths with underscore ("_") as a word separator.
    /// </summary>
    public class UnderscorePathParser : IPathParser
    {
        readonly Regex _re = new Regex(@"\G_*([^_]+)", RegexOptions.CultureInvariant);

        /// <inheritdoc />
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

using System.Text.RegularExpressions;

namespace Game.Services.Utilities
{
    public static partial class RegexHelper
    {
        [GeneratedRegex(@"[a-zA-Z0-9_]")]
        public static partial Regex GuessRegex();
    }
}
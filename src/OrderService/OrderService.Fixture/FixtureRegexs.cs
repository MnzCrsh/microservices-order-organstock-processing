using System.Text.RegularExpressions;

namespace OrderService.Fixture;

public static partial class FixtureRegexs
{
    public static Match MatchDockerId(string dockerError) => DockerRegex().Match(dockerError);

    [GeneratedRegex(@"container (\w+) is not running", RegexOptions.IgnoreCase, "en-En")]
    private static partial Regex DockerRegex();
}
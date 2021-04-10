using CommandLine;

namespace Painter.CLI {
    [Verb("match", isDefault: false, HelpText = "Match color data")]
    public class MatchOptions {
        [Option("db", Required = true, HelpText = "Path to a SQLite database file containing downloaded color data")]
        public string DbFile { get; set; }

        [Option("hex-code", Required = true, HelpText = "Hex code of color to match with downloaded color data")]
        public string HexCode { get; set; }

        [Option("top", Required = false, Default = 10, HelpText = "The top number of results to display with the closest match")]
        public int Top { get; set; }
    }
}

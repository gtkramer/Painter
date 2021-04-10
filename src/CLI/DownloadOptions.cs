using CommandLine;

namespace Painter.CLI {
    [Verb("download", isDefault: false, HelpText = "Download color data")]
    public class DownloadOptions {
        [Option("db", Required = false, Default = "colors.db", HelpText = "Path to a SQLite database file to which color data is downloaded")]
        public string DbFile { get; set; }

        [Option("benjamin-moore", Required = false, Default = false, HelpText = "Whether to download Benjamin Moore color data")]
        public bool HasBenjaminMoore { get; set; }

        [Option("sherwin-williams", Required = false, Default = false, HelpText = "Whether to download Sherwin Williams color data")]
        public bool HasSherwinWilliams { get; set; }
    }
}

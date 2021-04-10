using CommandLine;
using Painter.CLI;

namespace Painter {
    public class Program {
        public static void Main(string[] args) {
            Parser.Default.ParseArguments<DownloadOptions, FilterOptions, MatchOptions>(args)
            .WithParsed<DownloadOptions>(DownloadRunner.Execute)
            .WithParsed<FilterOptions>(FilterRunner.Execute)
            .WithParsed<MatchOptions>(MatchRunner.Execute);
        }
    }
}

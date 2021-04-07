using CommandLine;
using Colorist.CLI;

namespace Colorist {
    public class Program {
        public static void Main(string[] args) {
            Parser.Default.ParseArguments<DownloadOptions, FilterOptions>(args)
            .WithParsed<DownloadOptions>(DownloadRunner.Execute)
            .WithParsed<FilterOptions>(FilterRunner.Execute);
        }
    }
}

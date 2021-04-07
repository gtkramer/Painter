using System.Net;
using CommandLine;
using System.Threading;
using Data;
using ColorHelper;
using System.Collections.Generic;
using System;

namespace ColorModel {
    public class Program {
        public static void Main(string[] args) {
            Parser.Default.ParseArguments<DownloadOptions, FilterOptions>(args)
            .WithParsed<DownloadOptions>(RunDownloadOptions)
            .WithParsed<FilterOptions>(RunFilterOptions);
        }

        [Verb("filter", isDefault: false, HelpText = "Filter color data")]
        public class FilterOptions {
            [Option("db", Required = true, HelpText = "Path to a SQLite database file containing downloaded color data")]
            public string DbFile { get; set; }

            [Option("min-hue", Required = false, Default = 0.0f, HelpText = "Min hue value")]
            public double MinHue { get; set; }
            [Option("max-hue", Required = false, Default = 360.0f, HelpText = "Max hue value")]
            public double MaxHue { get; set; }

            [Option("min-saturation", Required = false, Default = 0.0f, HelpText = "Min saturation value")]
            public double MinSaturation { get; set; }
            [Option("max-saturation", Required = false, Default = 1.0f, HelpText = "Max saturation value")]
            public double MaxSaturation { get; set; }

            [Option("min-lightness", Required = false, Default = 0.0f, HelpText = "Min lightness value")]
            public double MinLightness { get; set; }
            [Option("max-lightness", Required = false, Default = 1.0f, HelpText = "Max lightness value")]
            public double MaxLightness { get; set; }

            [Option("min-lrv", Required = false, Default = 0.0, HelpText = "Min LRV value")]
            public double MinLrv { get; set; }
            [Option("max-lrv", Required = false, Default = 100.0, HelpText = "Max LRV value")]
            public double MaxLRV { get; set; }
        }

        [Verb("download", isDefault: false, HelpText = "Download color data")]
        public class DownloadOptions {
            [Option("db", Required = false, Default = "colors.db", HelpText = "Path to a SQLite database file to which color data is downloaded")]
            public string DbFile { get; set; }

            [Option("benjamin-moore", Required = false, Default = false, HelpText = "Whether to download Benjamin Moore color data")]
            public bool HasBenjaminMoore { get; set; }
        }

        private static void RunFilterOptions(FilterOptions opts) {

        }

        private static void RunDownloadOptions(DownloadOptions opts) {
            using ColorContext colorContext = new ColorContext(opts.DbFile);
            colorContext.Database.EnsureCreated();

            using WebClient webClient = new WebClient();
            List<IColorHelper> colorHelpers = new List<IColorHelper>{new BenjaminMooreColorHelper()};
            foreach (IColorHelper colorHelper in colorHelpers) {
                string urlPrefix = colorHelper.GetDataUrlPrefix();
                string[] colorNumbers = colorHelper.GetAllColorNumbers();
                foreach (string colorNumber in colorNumbers) {
                    string url = urlPrefix + "/" + colorNumber;
                    string html = DownloadHtml(webClient, url);
                    ColorSwatch colorSwatch = colorHelper.GetColorSwatchFromHtml(html);
                    if (!colorContext.TryAddNewColorSwatch(colorSwatch)) {
                        Console.WriteLine("Already added " + colorSwatch.Name + " " + colorSwatch.ColorNumbers[0].Number);
                    }
                }
            }
        }

        private static string DownloadHtml(WebClient webClient, string url) {
            string html = "";
            for (int i = 0; i != 3; i++) {
                try {
                    html = webClient.DownloadString(url);
                    break;
                }
                catch {
                    Thread.Sleep(10000);
                }
            }
            return html;
        }
    }
}

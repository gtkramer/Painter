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
            Parser.Default.ParseArguments<Options>(args)
            .WithParsed<Options>(RunOptions);
            //.WithNotParsed<Options>(HandleParseError);
        }

        public class Options {
            [Option("db", Required = false, Default = "colors.db", HelpText = "Path to a SQLite database file")]
            public string DbFile { get; set; }
        }

        private static void RunOptions(Options opts) {
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

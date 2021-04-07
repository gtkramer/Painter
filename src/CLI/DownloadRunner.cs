using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using Colorist.Data;
using Colorist.Domain;
using Colorist.Download;

namespace Colorist.CLI {
    public class DownloadRunner {
        public static void Execute(DownloadOptions opts) {
            List<IColorHelper> colorHelpers = new List<IColorHelper>();
            if (opts.HasBenjaminMoore) {
                colorHelpers.Add(new BenjaminMooreColorHelper());
            }
            if (colorHelpers.Count == 0) {
                Console.WriteLine("No color brands selected to download color data");
                return;
            }

            using ColorContext colorContext = new ColorContext(opts.DbFile);
            colorContext.Database.EnsureCreated();
            using WebClient webClient = new WebClient();
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

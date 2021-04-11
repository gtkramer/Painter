using System;
using System.Collections.Generic;
using System.Net.Http;
using Painter.Data;
using Painter.Domain;
using Painter.Download;

namespace Painter.CLI {
    public class DownloadRunner {
        public static void Execute(DownloadOptions opts) {
            using ColorDownloader colorDownloader = new ColorDownloader(new HttpClient());
            if (opts.HasBenjaminMoore) {
                SaveColors(colorDownloader.ParallelDownloadColors(BenjaminMooreColorDownloader.GetUrls(), BenjaminMooreColorDownloader.GetColorSwatch), opts.DbFile);
            }
            if (opts.HasSherwinWilliams) {
                SaveColors(colorDownloader.DownloadColors(SherwinWilliamsColorDownloader.JsonUrl, SherwinWilliamsColorDownloader.GetColorSwatches), opts.DbFile);
            }
        }

        private static void SaveColors(IEnumerable<ColorSwatch> colorSwatches, string dbFile) {
            using ColorContext colorContext = new ColorContext(dbFile);
            colorContext.Database.EnsureCreated();
            foreach (ColorSwatch colorSwatch in colorSwatches) {
                if (!colorContext.TryAddNewColorSwatch(colorSwatch)) {
                    Console.WriteLine("Already added " + colorSwatch.Name + " " + colorSwatch.ColorNumbers[0].Number);
                }
            }
        }
    }
}

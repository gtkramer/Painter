using System;
using System.Collections.Generic;
using Colorist.Data;
using Colorist.Domain;
using Colorist.Download;

namespace Colorist.CLI {
    public class DownloadRunner {
        public static void Execute(DownloadOptions opts) {
            List<ColorDownloader> colorDownloaders = new List<ColorDownloader>();
            if (opts.HasBenjaminMoore) {
                colorDownloaders.Add(new BenjaminMooreColorDownloader());
            }
            if (opts.HasSherwinWilliams) {
                colorDownloaders.Add(new SherwinWilliamsColorDownloader());
            }
            if (colorDownloaders.Count == 0) {
                Console.WriteLine("No color brands selected to download color data");
                return;
            }
            foreach (ColorDownloader colorDownloader in colorDownloaders) {
                SaveColors(colorDownloader.DownloadColors(), opts.DbFile);
            }
        }

        private static void SaveColors(List<ColorSwatch> colorSwatches, string dbFile) {
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

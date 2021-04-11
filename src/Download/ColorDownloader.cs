using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Painter.Domain;

namespace Painter.Download {
    public class ColorDownloader : HttpClient {
        public IEnumerable<ColorSwatch> ParallelDownloadColors(IEnumerable<string> urls, Func<string, ColorSwatch> action) {
            ConcurrentBag<ColorSwatch> colorSwatches = new ConcurrentBag<ColorSwatch>();
            Parallel.ForEach(urls,
                // Limit parallel downloads to not overwhelm a service and cause throttling
                new ParallelOptions {MaxDegreeOfParallelism = 4},
                url => {
                    colorSwatches.Add(action(GetStringAsync(url).Result));
                }
            );
            return colorSwatches;
        }

        public IEnumerable<ColorSwatch> DownloadColors(string url, Func<string, IEnumerable<ColorSwatch>> action) {
            return action(GetStringAsync(url).Result);
        }
    }
}

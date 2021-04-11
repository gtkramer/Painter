using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Painter.Domain;

namespace Painter.Download {
    public static class SherwinWilliamsColorDownloader {
        public static string GetUrl() {
            return "https://prism-api.sherwin-williams.com/v1/colors/sherwin?lng=en-US&_corev=2.0.5";
        }

        public static IEnumerable<ColorSwatch> GetColorSwatches(string json) {
            ConcurrentBag<ColorSwatch> colorSwatches = new ConcurrentBag<ColorSwatch>();
            IEnumerable<JsonElement> colorElements = JsonDocument.Parse(json).RootElement.EnumerateArray().GetEnumerator();
            Parallel.ForEach(colorElements, colorElement => {
                ColorSwatch colorSwatch = GetColorSwatchFromJson(colorElement);
                colorSwatches.Add(colorSwatch);
                Console.WriteLine("Downloaded " + colorSwatch.Name);
            });
            return colorSwatches;
        }

        private static ColorSwatch GetColorSwatchFromJson(JsonElement json) {
            string name = WebUtility.HtmlDecode(json.GetProperty("name").GetString());
            string number = json.GetProperty("brandKey").GetString() + " " + json.GetProperty("colorNumber").GetString();
            Color color = ColorTranslator.FromHtml(json.GetProperty("hex").GetString());
            double lrv = json.GetProperty("lrv").GetDouble();

            return new ColorSwatch{
                Name = name,
                ColorNumbers = new List<ColorNumber>{
                    new ColorNumber{Number = number}
                },
                Brand = ColorBrand.SherwinWilliams,
                Red = color.R,
                Green = color.G,
                Blue = color.B,
                Hue = color.GetHue(),
                Saturation = color.GetSaturation(),
                Lightness = color.GetBrightness(),
                Lrv = lrv
            };
        }
    }
}

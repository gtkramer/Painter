using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Painter.Domain;

namespace Painter.Download {
    public class SherwinWilliamsClient : ColorClient
    {
        public SherwinWilliamsClient(HttpClient httpClient) : base(httpClient)
        {
        }

        public override IEnumerable<string> GetUrls() {
            return new List<string>{"https://prism-api.sherwin-williams.com/v1/colors/sherwin?lng=en-US&_corev=2.0.5"};
        }

        public override void PopulateColors(string url, ConcurrentBag<ColorSwatch> colorSwatches)
        {
            Task<string?> contents = ResilientDownloadAsync(url);
            contents.Wait();
            if (contents.Result != null) {
                foreach (ColorSwatch colorSwatch in GetColorSwatches(contents.Result)) {
                    colorSwatches.Add(colorSwatch);
                }
            }
        }

        private IEnumerable<ColorSwatch> GetColorSwatches(string json) {
            ConcurrentBag<ColorSwatch> colorSwatches = new();
            IEnumerable<JsonElement> colorElements = JsonDocument.Parse(json).RootElement.EnumerateArray().GetEnumerator();
            Parallel.ForEach(colorElements, colorElement => {
                ColorSwatch colorSwatch = GetColorSwatchFromJson(colorElement);
                colorSwatches.Add(colorSwatch);
                Console.WriteLine("Downloaded " + colorSwatch.Brand.GetDescription() + " " + colorSwatch.Name + " " + colorSwatch.ColorNumbers[0].Number);
            });
            return colorSwatches;
        }

        private ColorSwatch GetColorSwatchFromJson(JsonElement json) {
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

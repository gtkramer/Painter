using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net;
using System.Text.Json;
using Colorist.Domain;

namespace Colorist.Download {
    public class SherwinWilliamsColorDownloader : ColorDownloader {
        public override List<ColorSwatch> DownloadColors() {
            List<ColorSwatch> colorSwatches = new List<ColorSwatch>();
            using WebClient webClient = new WebClient();
            string json = RetryDownloadString(webClient, "https://prism-api.sherwin-williams.com/v1/colors/sherwin?lng=en-US&_corev=2.0.5");
            if (!string.IsNullOrEmpty(json)) {
                JsonElement.ArrayEnumerator colorEnum = JsonDocument.Parse(json).RootElement.EnumerateArray();
                while (colorEnum.MoveNext()) {
                    ColorSwatch colorSwatch = GetColorSwatchFromJson(colorEnum.Current);
                    colorSwatches.Add(colorSwatch);
                    Console.WriteLine("Downloaded " + colorSwatch.Name);
                }
            }
            return colorSwatches;
        }

        private ColorSwatch GetColorSwatchFromJson(JsonElement json) {
            string name = WebUtility.HtmlDecode(json.GetProperty("name").GetString());
            string number = json.GetProperty("brandKey").GetString() + json.GetProperty("colorNumber").GetString();
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

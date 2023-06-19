
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using Painter.Domain;

namespace Painter.Download {
    public class BenjaminMooreClient : ColorClient
    {
        public BenjaminMooreClient(HttpClient httpClient) : base(httpClient)
        {
        }

        public override IEnumerable<string> GetUrls() {
            List<string[]> collections = new()
            {
                GetHistoricalColorCodes(),
                GetAffinityColorCodes(),
                GetAuraColorCodes(),
                GetWilliamsburgColorCodes(),
                GetPreviewColorCodes(),
                GetClassicColorCodes(),
                GetOffWhiteColorCodes(),
                GetAmericaColorCodes(),
                GetDesignerColorCodes()
            };

            int numCodes = 0;
            foreach (string[] collection in collections) {
                numCodes += collection.Length;
            }
            List<string> urls = new(numCodes);

            foreach (string[] collection in collections) {
                foreach (string code in collection) {
                    urls.Add("https://www.benjaminmoore.com/en-us/paint-colors/color/" + code);
                }
            }
            return urls;
        }

        private static string[] GetHistoricalColorCodes() {
            string[] codes = new string[191];
            for (int i = 0; i != codes.Length; i++) {
                codes[i] = "hc-" + (i + 1);
            }
            return codes;
        }

        private static string[] GetAffinityColorCodes() {
            string[] codes = new string[144];
            for (int i = 0; i != codes.Length; i++) {
                codes[i] = "af-" + ((i + 1) * 5);
            }
            return codes;
        }

        private static string[] GetAuraColorCodes() {
            string[] codes = new string[240];
            for (int i = 0; i != codes.Length; i++) {
                codes[i] = "csp-" + ((i + 1) * 5);
            }
            return codes;
        }

        private static string[] GetWilliamsburgColorCodes() {
            string[] codes = new string[144];
            for (int i = 0; i != codes.Length; i++) {
                codes[i] = "cw-" + ((i + 1) * 5);
            }
            return codes;
        }

        private static string[] GetPreviewColorCodes() {
            List<string> codes = new(1232);
            for (int i = 0; i != 176; i++) {
                for (int j = 0; j != 7; j++) {
                    codes.Add((2000 + i) + "-" + ((j + 1) * 10));
                }
            }
            return codes.ToArray();
        }

        private static string[] GetClassicColorCodes() {
            string[] codes = new string[1680];
            for (int i = 0; i != codes.Length; i++) {
                int value = i + 1;
                int numZeros = 3 - value.ToString().Length;
                if (numZeros > 0) {
                    string padding = new('0', numZeros);
                    codes[i] = padding + value;
                }
                else {
                    codes[i] = value.ToString();
                }
            }
            return codes;
        }

        private static string[] GetOffWhiteColorCodes() {
            string[] codes = new string[152];
            for (int i = 0; i != codes.Length; i++) {
                codes[i] = "oc-" + (i + 1);
            }
            return codes;
        }

        private static string[] GetAmericaColorCodes() {
            string[] codes = new string[42];
            for (int i = 0; i != codes.Length; i++) {
                codes[i] = "ac-" + (i + 1);
            }
            return codes;
        }

        private static string[] GetDesignerColorCodes() {
            List<string> codes = new(231);
            for (int i = 0; i != 33; i++) {
                int rangeStart = i * 30;
                codes.Add("cc-" + (rangeStart + 2));
                codes.Add("cc-" + (rangeStart + 4));
                codes.Add("cc-" + (rangeStart + 6));
                codes.Add("cc-" + (rangeStart + 8));
                codes.Add("cc-" + (rangeStart + 10));
                codes.Add("cc-" + (rangeStart + 20));
                codes.Add("cc-" + (rangeStart + 30));
            }
            return codes.ToArray();
        }

        public override void PopulateColors(string url, ConcurrentBag<ColorSwatch> colorSwatches)
        {
            Task<string?> contents = ResilientDownloadAsync(url);
            contents.Wait();
            if (contents.Result != null) {
                colorSwatches.Add(GetColorSwatch(contents.Result));
            }
        }

        private ColorSwatch GetColorSwatch(string html) {
            HtmlParser htmlParser = new();
            IHtmlDocument htmlDocument = htmlParser.ParseDocument(html);
            IElement htmlElement = htmlDocument.QuerySelector("script#__NEXT_DATA__[type=\"application/json\"]");
            JsonDocument jsonDocument = JsonDocument.Parse(htmlElement.TextContent);
            JsonElement jsonElement = jsonDocument.RootElement
                .GetProperty("props")
                .GetProperty("pageProps")
                .GetProperty("componentData")
                .GetProperty("components").EnumerateArray().GetEnumerator().First()
                .GetProperty("color_data")
                .GetProperty("props")
                .GetProperty("color");
            return GetColorSwatchFromJson(jsonElement);
        }

        private ColorSwatch GetColorSwatchFromJson(JsonElement json) {
            string name = WebUtility.HtmlDecode(json.GetProperty("name").GetString());
            string number = json.GetProperty("number").GetString();
            Color color = ColorTranslator.FromHtml("#" + json.GetProperty("hex").GetString());
            double lrv = json.GetProperty("lrv").GetDouble();
            ColorSwatch colorSwatch = new()
            {
                Name = name,
                ColorNumbers = new List<ColorNumber>{
                    new ColorNumber{Number = number}
                },
                Brand = ColorBrand.BenjaminMoore,
                Red = color.R,
                Green = color.G,
                Blue = color.B,
                Hue = color.GetHue(),
                Saturation = color.GetSaturation(),
                Lightness = color.GetBrightness(),
                Lrv = lrv
            };
            Console.WriteLine("Downloaded " + name);
            return colorSwatch;
        }
    }
}

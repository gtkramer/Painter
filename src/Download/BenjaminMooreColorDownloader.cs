using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net;
using System.Text.Json;
using System.Text.RegularExpressions;
using Painter.Domain;
using Painter.Utilities;

namespace Painter.Download {
    public static class BenjaminMooreColorDownloader {
        private static string UrlPrefix = "https://www.benjaminmoore.com/en-us/color-overview/find-your-color/color/";

        public static ColorSwatch GetColorSwatch(string html) {
            return GetColorSwatchFromJson(GetJsonColorData(html));
        }

        public static IEnumerable<string> GetUrls() {
            string[] historical = GetHistoricalColorUrls();
            string[] affinity = GetAffinityColorUrls();
            string[] aura = GetAuraColorUrls();
            string[] williamsburg = GetWilliamsburgColorUrls();
            string[] preview = GetPreviewColorUrls();
            string[] classic = GetClassicColorUrls();
            string[] offWhite = GetOffWhiteColorUrls();
            string[] america = GetAmericaColorUrls();
            string[] designer = GetDesignerColorUrls();

            List<string[]> collections = new List<string[]>{historical, affinity, aura, williamsburg, preview, classic, offWhite, america, designer};

            int length = 0;
            foreach (string[] collection in collections) {
                length += collection.Length;
            }
            string[] all = new string[length];

            int destinationIndex = 0;
            foreach (string[] collection in collections) {
                Array.Copy(collection, 0, all, destinationIndex, collection.Length);
                destinationIndex += collection.Length;
            }

            return all;
        }

        private static string[] GetHistoricalColorUrls() {
            string[] nums = new string[191];
            for (int i = 0; i != nums.Length; i++) {
                nums[i] = UrlPrefix + "hc-" + (i + 1);
            }
            return nums;
        }

        private static string[] GetAffinityColorUrls() {
            string[] nums = new string[144];
            for (int i = 0; i != nums.Length; i++) {
                nums[i] = UrlPrefix + "af-" + ((i + 1) * 5);
            }
            return nums;
        }

        private static string[] GetAuraColorUrls() {
            string[] nums = new string[240];
            for (int i = 0; i != nums.Length; i++) {
                nums[i] = UrlPrefix + "csp-" + ((i + 1) * 5);
            }
            return nums;
        }

        private static string[] GetWilliamsburgColorUrls() {
            string[] nums = new string[144];
            for (int i = 0; i != nums.Length; i++) {
                nums[i] = UrlPrefix + "cw-" + ((i + 1) * 5);
            }
            return nums;
        }

        private static string[] GetPreviewColorUrls() {
            List<string> nums = new List<string>(1232);
            for (int i = 0; i != 176; i++) {
                for (int j = 0; j != 7; j++) {
                    nums.Add(UrlPrefix + (2000 + i) + "-" + ((j + 1) * 10));
                }
            }
            return nums.ToArray();
        }

        private static string[] GetClassicColorUrls() {
            string[] nums = new string[1680];
            for (int i = 0; i != nums.Length; i++) {
                int value = i + 1;
                int numZeros = 3 - value.ToString().Length;
                if (numZeros > 0) {
                    string padding = new string('0', numZeros);
                    nums[i] = UrlPrefix + padding + value;
                }
                else {
                    nums[i] = UrlPrefix + value.ToString();
                }
            }
            return nums;
        }

        private static string[] GetOffWhiteColorUrls() {
            string[] nums = new string[152];
            for (int i = 0; i != nums.Length; i++) {
                nums[i] = UrlPrefix + "oc-" + (i + 1);
            }
            return nums;
        }

        private static string[] GetAmericaColorUrls() {
            string[] nums = new string[42];
            for (int i = 0; i != nums.Length; i++) {
                nums[i] = UrlPrefix + "ac-" + (i + 1);
            }
            return nums;
        }

        private static string[] GetDesignerColorUrls() {
            List<string> nums = new List<string>(231);
            for (int i = 0; i != 33; i++) {
                int rangeStart = i * 30;
                nums.Add(UrlPrefix + "cc-" + (rangeStart + 2));
                nums.Add(UrlPrefix + "cc-" + (rangeStart + 4));
                nums.Add(UrlPrefix + "cc-" + (rangeStart + 6));
                nums.Add(UrlPrefix + "cc-" + (rangeStart + 8));
                nums.Add(UrlPrefix + "cc-" + (rangeStart + 10));
                nums.Add(UrlPrefix + "cc-" + (rangeStart + 20));
                nums.Add(UrlPrefix + "cc-" + (rangeStart + 30));
            }
            return nums.ToArray();
        }

        private static JsonElement GetJsonColorData(string html) {
            Regex regex = new Regex(@"\s*window.appData\s*=\s*\{.*\};\s*\n", RegexOptions.Compiled);
            System.Text.RegularExpressions.Match match = regex.Match(html);
            string json = Regex.Replace(Regex.Replace(match.Value, @"\s*window.appData\s*=\s*", ""), @";\s*\n", "");
            return JsonDocument.Parse(json).RootElement;
        }

        private static ColorSwatch GetColorSwatchFromJson(JsonElement json) {
            JsonElement colorDetailElem = json.GetProperty("page").GetProperty("colorDetail");
            JsonElement colorElem = colorDetailElem.GetProperty("color");
            string name = WebUtility.HtmlDecode(colorElem.GetProperty("name").GetString());
            string number = colorElem.GetProperty("number").GetString();
            Color color = ColorTranslator.FromHtml("#" + colorElem.GetProperty("hex").GetString());
            double lrv = colorDetailElem.GetProperty("lrv").GetDouble();

            ColorSwatch colorSwatch = new ColorSwatch{
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
            Console.WriteLine("Downloaded " + colorSwatch.Brand.GetDescription() + " " + colorSwatch.Name + " " + colorSwatch.ColorNumbers[0].Number);
            return colorSwatch;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net;
using System.Text.Json;
using System.Text.RegularExpressions;
using Painter.Domain;

namespace Painter.Download {
    public static class BenjaminMoore {
        private static string _urlPrefix = "https://www.benjaminmoore.com/en-us/color-overview/find-your-color/color/";

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
                nums[i] = _urlPrefix + "hc-" + (i + 1);
            }
            return nums;
        }

        private static string[] GetAffinityColorUrls() {
            string[] nums = new string[144];
            for (int i = 0; i != nums.Length; i++) {
                nums[i] = _urlPrefix + "af-" + ((i + 1) * 5);
            }
            return nums;
        }

        private static string[] GetAuraColorUrls() {
            string[] nums = new string[240];
            for (int i = 0; i != nums.Length; i++) {
                nums[i] = _urlPrefix + "csp-" + ((i + 1) * 5);
            }
            return nums;
        }

        private static string[] GetWilliamsburgColorUrls() {
            string[] nums = new string[144];
            for (int i = 0; i != nums.Length; i++) {
                nums[i] = _urlPrefix + "cw-" + ((i + 1) * 5);
            }
            return nums;
        }

        private static string[] GetPreviewColorUrls() {
            List<string> nums = new List<string>(1232);
            for (int i = 0; i != 176; i++) {
                for (int j = 0; j != 7; j++) {
                    nums.Add(_urlPrefix + (2000 + i) + "-" + ((j + 1) * 10));
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
                    nums[i] = _urlPrefix + padding + value;
                }
                else {
                    nums[i] = _urlPrefix + value.ToString();
                }
            }
            return nums;
        }

        private static string[] GetOffWhiteColorUrls() {
            string[] nums = new string[152];
            for (int i = 0; i != nums.Length; i++) {
                nums[i] = _urlPrefix + "oc-" + (i + 1);
            }
            return nums;
        }

        private static string[] GetAmericaColorUrls() {
            string[] nums = new string[42];
            for (int i = 0; i != nums.Length; i++) {
                nums[i] = _urlPrefix + "ac-" + (i + 1);
            }
            return nums;
        }

        private static string[] GetDesignerColorUrls() {
            List<string> nums = new List<string>(231);
            for (int i = 0; i != 33; i++) {
                int rangeStart = i * 30;
                nums.Add(_urlPrefix + "cc-" + (rangeStart + 2));
                nums.Add(_urlPrefix + "cc-" + (rangeStart + 4));
                nums.Add(_urlPrefix + "cc-" + (rangeStart + 6));
                nums.Add(_urlPrefix + "cc-" + (rangeStart + 8));
                nums.Add(_urlPrefix + "cc-" + (rangeStart + 10));
                nums.Add(_urlPrefix + "cc-" + (rangeStart + 20));
                nums.Add(_urlPrefix + "cc-" + (rangeStart + 30));
            }
            return nums.ToArray();
        }

        public static ColorSwatch GetColorSwatch(string html) {
            ColorSwatch colorSwatch = GetColorSwatchFromJson(GetJsonColorData(html));
            Console.WriteLine("Downloaded " + colorSwatch.Brand.GetDescription() + " " + colorSwatch.Name + " " + colorSwatch.ColorNumbers[0].Number);
            return colorSwatch;
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

            return new ColorSwatch{
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
        }
    }
}

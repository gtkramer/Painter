using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net;
using System.Text.Json;
using System.Text.RegularExpressions;
using Painter.Domain;

namespace Painter.Download {
    public class BenjaminMooreColorDownloader : ColorDownloader {
        public override List<ColorSwatch> DownloadColors() {
            List<ColorSwatch> colorSwatches = new List<ColorSwatch>();
            using WebClient webClient = new WebClient();
            string[] colorNumbers = GetAllColorNumbers();
            foreach (string colorNumber in colorNumbers) {
                string url = "https://www.benjaminmoore.com/en-us/color-overview/find-your-color/color/" + colorNumber;
                string html = RetryDownloadString(webClient, url);
                if (!string.IsNullOrEmpty(html)) {
                    ColorSwatch colorSwatch = GetColorSwatchFromJson(GetJsonColorData(html));
                    colorSwatches.Add(colorSwatch);
                    Console.WriteLine("Downloaded " + colorSwatch.Name);
                }
            }
            return colorSwatches;
        }

        private string[] GetAllColorNumbers() {
            string[] historical = GetHistoricalColorNumbers();
            string[] affinity = GetAffinityColorNumbers();
            string[] aura = GetAuraColorNumbers();
            string[] williamsburg = GetWilliamsburgColorNumbers();
            string[] preview = GetPreviewColorNumbers();
            string[] classic = GetClassicColorNumbers();
            string[] offWhite = GetOffWhiteColorNumbers();
            string[] america = GetAmericaColorNumbers();
            string[] designer = GetDesignerColorNumbers();

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

        private string[] GetHistoricalColorNumbers() {
            string[] nums = new string[191];
            for (int i = 0; i != nums.Length; i++) {
                nums[i] = "hc-" + (i + 1);
            }
            return nums;
        }

        private string[] GetAffinityColorNumbers() {
            string[] nums = new string[144];
            for (int i = 0; i != nums.Length; i++) {
                nums[i] = "af-" + ((i + 1) * 5);
            }
            return nums;
        }

        private string[] GetAuraColorNumbers() {
            string[] nums = new string[240];
            for (int i = 0; i != nums.Length; i++) {
                nums[i] = "csp-" + ((i + 1) * 5);
            }
            return nums;
        }

        private string[] GetWilliamsburgColorNumbers() {
            string[] nums = new string[144];
            for (int i = 0; i != nums.Length; i++) {
                nums[i] = "cw-" + ((i + 1) * 5);
            }
            return nums;
        }

        private string[] GetPreviewColorNumbers() {
            List<string> nums = new List<string>(1232);
            for (int i = 0; i != 176; i++) {
                for (int j = 0; j != 7; j++) {
                    nums.Add((2000 + i) + "-" + ((j + 1) * 10));
                }
            }
            return nums.ToArray();
        }

        private string[] GetClassicColorNumbers() {
            string[] nums = new string[1680];
            for (int i = 0; i != nums.Length; i++) {
                int value = i + 1;
                int numZeros = 3 - value.ToString().Length;
                if (numZeros > 0) {
                    string padding = new string('0', numZeros);
                    nums[i] = padding + value;
                }
                else {
                    nums[i] = value.ToString();
                }
            }
            return nums;
        }

        private string[] GetOffWhiteColorNumbers() {
            string[] nums = new string[152];
            for (int i = 0; i != nums.Length; i++) {
                nums[i] = "oc-" + (i + 1);
            }
            return nums;
        }

        private string[] GetAmericaColorNumbers() {
            string[] nums = new string[42];
            for (int i = 0; i != nums.Length; i++) {
                nums[i] = "ac-" + (i + 1);
            }
            return nums;
        }

        private string[] GetDesignerColorNumbers() {
            List<string> nums = new List<string>(231);
            for (int i = 0; i != 33; i++) {
                int rangeStart = i * 30;
                nums.Add("cc-" + (rangeStart + 2));
                nums.Add("cc-" + (rangeStart + 4));
                nums.Add("cc-" + (rangeStart + 6));
                nums.Add("cc-" + (rangeStart + 8));
                nums.Add("cc-" + (rangeStart + 10));
                nums.Add("cc-" + (rangeStart + 20));
                nums.Add("cc-" + (rangeStart + 30));
            }
            return nums.ToArray();
        }

        private JsonElement GetJsonColorData(string html) {
            Regex regex = new Regex(@"\s*window.appData\s*=\s*\{.*\};\s*\n", RegexOptions.Compiled);
            Match match = regex.Match(html);
            string json = Regex.Replace(Regex.Replace(match.Value, @"\s*window.appData\s*=\s*", ""), @";\s*\n", "");
            return JsonDocument.Parse(json).RootElement;
        }

        private ColorSwatch GetColorSwatchFromJson(JsonElement json) {
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

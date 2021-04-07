using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using Colorist.Data;
using Colorist.Domain;
using Colorist.Filter;

namespace Colorist.CLI {
    public class FilterRunner {
        public static void Execute(FilterOptions opts) {
            using ColorContext colorContext = new ColorContext(opts.DbFile);

            List<ColorSwatch> colorSwatches = colorContext.ColorSwatches.ToList();
            List<ColorFilter> colorFilters = GetColorFilters();
            foreach (ColorFilter colorFilter in colorFilters) {
                ColorSwatch[] filteredColorSwatches = colorFilter.FilterColors(colorSwatches).Where(x => ColorHslSelector(x, opts)).ToArray();
                string fileName = colorFilter.Name + ".png";
                if (File.Exists(fileName)) {
                    File.Delete(fileName);
                }
                if (filteredColorSwatches.Length != 0) {
                    ColorWriter colorWriter = new ColorWriter(fileName, ImageFormat.Png);
                    colorWriter.WriteColors(filteredColorSwatches);
                    PrintFilteredColors(colorFilter, filteredColorSwatches);
                }
            }
        }

        private static List<ColorFilter> GetColorFilters() {
            List<ColorFilter> colorFilters = new List<ColorFilter>();
            colorFilters.Add(new ColorFilter("Red", 355, 10, ColorFilter.ColorHueBreakSelector));
            colorFilters.Add(new ColorFilter("Red-Orange", 10, 20, ColorFilter.ColorHueContiguousSelector));
            colorFilters.Add(new ColorFilter("Orange-Brown", 20, 40, ColorFilter.ColorHueContiguousSelector));
            colorFilters.Add(new ColorFilter("Orange-Yellow", 40, 50, ColorFilter.ColorHueContiguousSelector));
            colorFilters.Add(new ColorFilter("Yellow", 50, 60, ColorFilter.ColorHueContiguousSelector));
            colorFilters.Add(new ColorFilter("Yellow-Green", 60, 80, ColorFilter.ColorHueContiguousSelector));
            colorFilters.Add(new ColorFilter("Green", 80, 140, ColorFilter.ColorHueContiguousSelector));
            colorFilters.Add(new ColorFilter("Green-Cyan", 140, 170, ColorFilter.ColorHueContiguousSelector));
            colorFilters.Add(new ColorFilter("Cyan", 170, 200, ColorFilter.ColorHueContiguousSelector));
            colorFilters.Add(new ColorFilter("Cyan-Blue", 200, 220, ColorFilter.ColorHueContiguousSelector));
            colorFilters.Add(new ColorFilter("Blue", 220, 240, ColorFilter.ColorHueContiguousSelector));
            colorFilters.Add(new ColorFilter("Blue-Magenta", 240, 280, ColorFilter.ColorHueContiguousSelector));
            colorFilters.Add(new ColorFilter("Magenta", 280, 320, ColorFilter.ColorHueContiguousSelector));
            colorFilters.Add(new ColorFilter("Magenta-Pink", 320, 330, ColorFilter.ColorHueContiguousSelector));
            colorFilters.Add(new ColorFilter("Pink", 330, 345, ColorFilter.ColorHueContiguousSelector));
            colorFilters.Add(new ColorFilter("Pink-Red", 345, 355, ColorFilter.ColorHueContiguousSelector));
            return colorFilters;
        }

        private static bool ColorHslSelector(ColorSwatch colorSwatch, FilterOptions opts) {
            double hue = colorSwatch.Hue;
            double saturation = colorSwatch.Saturation;
            double lightness = colorSwatch.Lightness;
            double lrv = colorSwatch.Lrv;
            return
                hue >= opts.MinHue               && hue <= opts.MaxHue &&
                saturation >= opts.MinSaturation && saturation <= opts.MaxSaturation &&
                lightness >= opts.MinLightness   && lightness <= opts.MaxLightness &&
                lrv >= opts.MinLrv               && lrv <= opts.MaxLrv;
        }

        private static void PrintFilteredColors(ColorFilter colorFilter, ColorSwatch[] colorSwatches) {
            Console.WriteLine(colorFilter.Name);
            foreach (ColorSwatch colorSwatch in colorSwatches) {
                Console.WriteLine(colorSwatch.Hue.ToString("000.000") + ", " + colorSwatch.Saturation.ToString("0.000") + ", " + colorSwatch.Lightness.ToString("0.000") + ": " + colorSwatch.Name);
            }
            Console.WriteLine();
        }
    }
}

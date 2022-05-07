using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Painter.Data;
using Painter.Domain;
using Painter.Filter;

namespace Painter.CLI {
    public class FilterRunner {
        public static void Execute(FilterOptions opts) {
            using ColorContext colorContext = new ColorContext(opts.DbFile);

            List<ColorSwatch> colorSwatches = colorContext.ColorSwatches.Include(s => s.ColorNumbers).ToList();
            List<ColorFilter> colorFilters = GetColorFilters();
            foreach (ColorFilter colorFilter in colorFilters) {
                ColorSwatch[] filteredColorSwatches = colorFilter.FilterColors(colorSwatches).Where(x => ColorMeetsCriteria(x, opts)).ToArray();
                if (filteredColorSwatches.Length != 0) {
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

        private static bool ColorMeetsCriteria(ColorSwatch colorSwatch, FilterOptions opts) {
            double hue = colorSwatch.Hue;
            double saturation = colorSwatch.Saturation;
            double lightness = colorSwatch.Lightness;
            double lrv = colorSwatch.Lrv;
            double intensity = colorSwatch.Intensity;
            return
                hue >= opts.MinHue               && hue <= opts.MaxHue &&
                saturation >= opts.MinSaturation && saturation <= opts.MaxSaturation &&
                lightness >= opts.MinLightness   && lightness <= opts.MaxLightness &&
                lrv >= opts.MinLrv               && lrv <= opts.MaxLrv &&
                intensity >= opts.MinIntensity   && intensity <= opts.MaxIntensity;
        }

        private static void PrintFilteredColors(ColorFilter colorFilter, ColorSwatch[] colorSwatches) {
            Console.WriteLine(colorFilter);
            foreach (ColorSwatch colorSwatch in colorSwatches) {
                Console.WriteLine(colorSwatch);
            }
            Console.WriteLine();
        }
    }
}

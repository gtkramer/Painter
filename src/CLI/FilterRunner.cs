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
            using ColorContext colorContext = new(opts.DbFile);

            List<ColorSwatch> colorSwatches = colorContext.ColorSwatches.Include(s => s.ColorNumbers).ToList();
            List<ColorFilter> colorFilters = GetColorFilters();
            foreach (ColorFilter colorFilter in colorFilters) {
                ColorSwatch[] filteredColorSwatches = colorFilter.FilterColors(colorSwatches).Where(x => ColorMeetsCriteria(x, opts)).OrderBy(x => x.Hue).ToArray();
                if (filteredColorSwatches.Length != 0) {
                    PrintFilteredColors(colorFilter, filteredColorSwatches);
                }
            }
        }

        // http://www.workwithcolor.com/red-color-hue-range-01.htm
        private static List<ColorFilter> GetColorFilters() {
            List<ColorFilter> colorFilters = new()
            {
                new ColorFilter("Red", 355, 10),
                new ColorFilter("Red-Orange", 10, 20),
                new ColorFilter("Orange-Brown", 20, 40),
                new ColorFilter("Orange-Yellow", 40, 50),
                new ColorFilter("Yellow", 50, 60),
                new ColorFilter("Yellow-Green", 60, 80),
                new ColorFilter("Green", 80, 140),
                new ColorFilter("Green-Cyan", 140, 170),
                new ColorFilter("Cyan", 170, 200),
                new ColorFilter("Cyan-Blue", 200, 220),
                new ColorFilter("Blue", 220, 240),
                new ColorFilter("Blue-Magenta", 240, 280),
                new ColorFilter("Magenta", 280, 320),
                new ColorFilter("Magenta-Pink", 320, 330),
                new ColorFilter("Pink", 330, 345),
                new ColorFilter("Pink-Red", 345, 355)
            };
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

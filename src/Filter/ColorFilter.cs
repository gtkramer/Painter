using System;
using System.Collections.Generic;
using System.Linq;
using Painter.Domain;

namespace Painter.Filter {
    public class ColorFilter {
        private readonly string _name;
        private readonly double _minInclusiveHue;
        private readonly double _maxExclusiveHue;

        public ColorFilter(string name, double minInclusiveHue, double maxExclusiveHue) {
            _name = name;
            _minInclusiveHue = minInclusiveHue;
            _maxExclusiveHue = maxExclusiveHue;
        }

        public IEnumerable<ColorSwatch> FilterColors(IEnumerable<ColorSwatch> colorSwatches) {
            if (_minInclusiveHue > _maxExclusiveHue) {
                return colorSwatches.Where(x => ColorHueBreakSelector(x, _minInclusiveHue, _maxExclusiveHue));
            }
            else if (_minInclusiveHue > _maxExclusiveHue) {
                return colorSwatches.Where(x => ColorHueContiguousSelector(x, _minInclusiveHue, _maxExclusiveHue));
            }
            else {
                throw new ArgumentException("Hue ranges are equal and do not represent a range");
            }
        }

        private static bool ColorHueContiguousSelector(ColorSwatch colorSwatch, double minInclusiveHue, double maxExclusiveHue) {
            double h = colorSwatch.Hue;
            return h >= minInclusiveHue && h < maxExclusiveHue;
        }

        private static bool ColorHueBreakSelector(ColorSwatch colorSwatch, double minInclusiveHue, double maxExclusiveHue) {
            double h = colorSwatch.Hue;
            return h >= minInclusiveHue || h < maxExclusiveHue;
        }

        public override string ToString() {
            return _name;
        }
    }
}

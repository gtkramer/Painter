using System;
using System.Collections.Generic;
using System.Linq;
using Painter.Domain;

namespace Painter.Filter {
    public class ColorFilter {
        private string _name;
        private int _minInclusiveHue;
        private int _maxExclusiveHue;
        private Func<ColorSwatch, int, int, bool> _selector;

        public ColorFilter(string name, int minInclusiveHue, int maxExclusiveHue, Func<ColorSwatch, int, int, bool> selector) {
            _name = name;
            _minInclusiveHue = minInclusiveHue;
            _maxExclusiveHue = maxExclusiveHue;
            _selector = selector;
        }

        public IEnumerable<ColorSwatch> FilterColors(IEnumerable<ColorSwatch> colorSwatches) {
            return colorSwatches.Where(x => _selector(x, _minInclusiveHue, _maxExclusiveHue));
        }

        public static bool ColorHueContiguousSelector(ColorSwatch colorSwatch, int minInclusiveHue, int maxExclusiveHue) {
            double h = colorSwatch.Hue;
            return h >= minInclusiveHue && h < maxExclusiveHue;
        }

        public static bool ColorHueBreakSelector(ColorSwatch colorSwatch, int minInclusiveHue, int maxExclusiveHue) {
            double h = colorSwatch.Hue;
            return h >= minInclusiveHue || h < maxExclusiveHue;
        }

        public override string ToString() {
            return _name;
        }
    }
}

using System;
using System.Drawing;
using System.Linq;
using Painter.Domain;
using Painter.Utilities;

namespace Painter.Match {
    public class ColorMatch {
        public double MatchError { get; }
        public ColorSwatch ColorSwatch { get; }

        public ColorMatch(Color customColor, ColorSwatch colorSwatch) {
            MatchError = (double)(Math.Abs(customColor.R - colorSwatch.Red) + Math.Abs(customColor.G - colorSwatch.Green) + Math.Abs(customColor.B - colorSwatch.Blue)) / (256 * 3);
            ColorSwatch = colorSwatch;
        }

        public override string ToString() {
            return (MatchError * 100).ToString("0.00") + "%: " + ColorSwatch.Brand.GetDescription() + " " + string.Join(", ", ColorSwatch.ColorNumbers.Select(x => x.Number));
        }
    }
}

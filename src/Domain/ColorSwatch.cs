using System.Collections.Generic;
using System.Linq;

namespace Painter.Domain {
    public class ColorSwatch {
        public int Id { get; set; }
        public ColorBrand Brand { get; set; }
        public List<ColorNumber> ColorNumbers { get; set; }
        public string Name { get; set; }
        public int Red { get; set; }
        public int Green { get; set; }
        public int Blue { get; set; }
        public double Hue { get; set; }
        public double Saturation { get; set; }
        public double Lightness { get; set; }
        public double Lrv { get; set; }

        public override string ToString() {
            return Hue.ToString("0.00").PadLeft(6) + ", " + Saturation.ToString("0.0000") + ", " + Lightness.ToString("0.0000") + ", " + Lrv.ToString("0.00") + ": " + Name + " (" + Brand.GetDescription() + " " + string.Join(", ", ColorNumbers.Select(x => x.Number)) + ")";
        }
    }
}

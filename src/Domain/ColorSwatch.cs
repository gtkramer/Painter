using System.Collections.Generic;

namespace Colorist.Domain {
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
    }
}

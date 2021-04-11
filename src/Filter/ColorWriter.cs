using System.Drawing;
using System.Drawing.Imaging;
using Painter.Domain;

namespace Painter.Filter {
    public static class ColorWriter {
        public static int UnitWidth = 200;
        public static int UnitHeight = 200;

        public static void WriteColors(ColorSwatch[] colorSwatches, string filePath) {
            WriteColors(colorSwatches, filePath, ImageFormat.Png);
        }

        public static void WriteColors(ColorSwatch[] colorSwatches, string filePath, ImageFormat fileFormat) {
            Bitmap image = new Bitmap(UnitWidth, UnitHeight * colorSwatches.Length);
            for (int i = 0; i != colorSwatches.Length; i++) {
                ColorSwatch colorSwatch = colorSwatches[i];
                Color color = Color.FromArgb(colorSwatch.Red, colorSwatch.Green, colorSwatch.Blue);
                for (int x = 0; x != UnitWidth; x++) {
                    for (int y = UnitHeight * i; y != UnitHeight * (i + 1); y++) {
                        image.SetPixel(x, y, color);
                    }
                }
            }
            image.Save(filePath, fileFormat);
        }
    }
}

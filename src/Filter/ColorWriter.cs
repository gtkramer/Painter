using System.Drawing;
using System.Drawing.Imaging;
using Colorist.Domain;

namespace Colorist.Filter {
    public class ColorWriter {
        private static int ImageWidth = 200;
        private static int ImageHeight = 200;

        private string FileName;
        private ImageFormat FileFormat;

        public ColorWriter(string fileName, ImageFormat fileFormat) {
            FileName = fileName;
            FileFormat = fileFormat;
        }

        public void WriteColors(ColorSwatch[] colorSwatches) {
            Bitmap image = new Bitmap(ImageWidth, ImageHeight * colorSwatches.Length);
            for (int i = 0; i != colorSwatches.Length; i++) {
                ColorSwatch colorSwatch = colorSwatches[i];
                Color color = Color.FromArgb(colorSwatch.Red, colorSwatch.Green, colorSwatch.Blue);
                for (int x = 0; x != ImageWidth; x++) {
                    for (int y = ImageHeight * i; y != ImageHeight * (i + 1); y++) {
                        image.SetPixel(x, y, color);
                    }
                }
            }
            image.Save(FileName, FileFormat);
        }
    }
}

using System;
using System.Drawing;
using Painter.Predict;

namespace Painter.CLI {
    public class PredictRunner {
        public static void Execute(PredictOptions opts) {
            Color customColor = ColorTranslator.FromHtml("#" + opts.HexCode.Replace("#", ""));
            MLModelLrv.ModelInput sampleData = new(){Red = customColor.R, Green = customColor.G, Blue = customColor.B};
            MLModelLrv.ModelOutput result = MLModelLrv.Predict(sampleData);
            Console.WriteLine($"Predicted LRV: {result.Score:F2}");
        }
    }
}

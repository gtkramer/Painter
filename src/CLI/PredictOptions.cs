using CommandLine;

namespace Painter.CLI {
    [Verb("predict", isDefault: false, HelpText = "Predict LRV based on Red, Green, and Blue values")]
    public class PredictOptions {
        [Option("hex-code", Required = true, HelpText = "Hex code of color to predict LRV")]
        public string HexCode { get; set; }
    }
}

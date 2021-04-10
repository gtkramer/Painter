using CommandLine;

namespace Painter.CLI {
    [Verb("filter", isDefault: false, HelpText = "Filter color data")]
    public class FilterOptions {
        [Option("db", Required = true, HelpText = "Path to a SQLite database file containing downloaded color data")]
        public string DbFile { get; set; }

        [Option("min-hue", Required = false, Default = 0.0f, HelpText = "Min hue value")]
        public double MinHue { get; set; }
        [Option("max-hue", Required = false, Default = 360.0f, HelpText = "Max hue value")]
        public double MaxHue { get; set; }

        [Option("min-saturation", Required = false, Default = 0.0f, HelpText = "Min saturation value")]
        public double MinSaturation { get; set; }
        [Option("max-saturation", Required = false, Default = 1.0f, HelpText = "Max saturation value")]
        public double MaxSaturation { get; set; }

        [Option("min-lightness", Required = false, Default = 0.0f, HelpText = "Min lightness value")]
        public double MinLightness { get; set; }
        [Option("max-lightness", Required = false, Default = 1.0f, HelpText = "Max lightness value")]
        public double MaxLightness { get; set; }

        [Option("min-lrv", Required = false, Default = 0.0, HelpText = "Min LRV value")]
        public double MinLrv { get; set; }
        [Option("max-lrv", Required = false, Default = 100.0, HelpText = "Max LRV value")]
        public double MaxLrv { get; set; }
    }
}

using System.Collections.Generic;
using Painter.Domain;

namespace Painter.Download {
    public interface IColorClient {
        public abstract IEnumerable<ColorSwatch> DownloadColors(string url);
        public abstract IEnumerable<string> GetUrls();
    }
}

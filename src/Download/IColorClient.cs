using System.Collections.Concurrent;
using System.Collections.Generic;
using Painter.Domain;

namespace Painter.Download {
    public interface IColorClient {
        public abstract void PopulateColors(string url, ConcurrentBag<ColorSwatch> colorSwatches);
        public abstract IEnumerable<string> GetUrls();
    }
}

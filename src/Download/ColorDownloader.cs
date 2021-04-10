using System.Collections.Generic;
using System.Net;
using System.Threading;
using Colorist.Domain;

namespace Colorist.Download {
    public abstract class ColorDownloader {
        public abstract List<ColorSwatch> DownloadColors();

        protected string RetryDownloadString(WebClient webClient, string url) {
            string str = "";
            for (int i = 0; i != 3; i++) {
                try {
                    str = webClient.DownloadString(url);
                    break;
                }
                catch {
                    Thread.Sleep(10000);
                }
            }
            return str;
        }
    }
}

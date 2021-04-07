using Colorist.Domain;

namespace Colorist.Download {
    public interface IColorHelper {
        public string GetDataUrlPrefix();
        public string[] GetAllColorNumbers();
        public ColorSwatch GetColorSwatchFromHtml(string html);
    }
}

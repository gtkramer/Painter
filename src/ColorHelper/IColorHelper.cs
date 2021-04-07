using Data;

namespace ColorHelper {
    public interface IColorHelper {
        public string GetDataUrlPrefix();
        public string[] GetAllColorNumbers();
        public ColorSwatch GetColorSwatchFromHtml(string html);
    }
}

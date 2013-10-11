
namespace HydroDesktop.WebServices.WaterOneFlow
{
    internal static class SeriesCodeHelper
    {
        public static string CreateSeriesCode(string methodCode, string qualityCode, string sourceCode)
        {
            return methodCode + "|" + qualityCode + "|" + sourceCode;
        }

        public static string GetMethodCode(string fullCode)
        {
            var index = fullCode.IndexOf("|", System.StringComparison.Ordinal);
            if (index < 0) return fullCode;
            return fullCode.Substring(0, index);
        }

        public static string GetQualityCode(string fullCode)
        {
            var firstIndex = fullCode.IndexOf("|", System.StringComparison.Ordinal);
            var lastIndex = fullCode.LastIndexOf("|", System.StringComparison.Ordinal);
            var length = lastIndex - firstIndex - 1;
            if (firstIndex < 0) return fullCode;
            if (lastIndex + 1 == fullCode.Length) return fullCode;
            return fullCode.Substring(firstIndex + 1, length);
        }

        public static string GetSourceCode(string fullCode)
        {
            var lastIndex = fullCode.LastIndexOf("|", System.StringComparison.Ordinal);
            if (lastIndex < 0) return fullCode;
            if (lastIndex + 1 == fullCode.Length) return fullCode;
            return fullCode.Substring(lastIndex + 1);
        }
    }
}


namespace HydroDesktop.WebServices.WaterOneFlow
{
    internal static class SeriesCode
    {
        public static string CreateSeriesCode(string methodCode, string qualityCode, string sourceCode)
        {
            return methodCode + "|" + qualityCode + "|" + sourceCode;
        }

        public static string GetMethodCode(string fullCode)
        {
            int index = fullCode.IndexOf("|");
            if (index < 0) return fullCode;
            return fullCode.Substring(0, fullCode.IndexOf("|"));
        }

        public static string GetQualityCode(string fullCode)
        {
            int firstIndex = fullCode.IndexOf("|");
            int lastIndex = fullCode.LastIndexOf("|");
            int length = lastIndex - firstIndex - 1;
            if (firstIndex < 0) return fullCode;
            if (lastIndex + 1 == fullCode.Length) return fullCode;
            return fullCode.Substring(firstIndex + 1, length);
        }

        public static string GetSourceCode(string fullCode)
        {
            int lastIndex = fullCode.LastIndexOf("|");
            if (lastIndex < 0) return fullCode;
            if (lastIndex + 1 == fullCode.Length) return fullCode;
            return fullCode.Substring(lastIndex + 1);
        }
    }
}
